using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemState { idle, carried, thrown, dropped, checkedout, paused};

public class Item : MonoBehaviour
{
    //PUBLIC
    public float encumb_multiplier = 1;
    public Rigidbody itemBody;
    public AgentManager prev_owner = null;
    public AgentManager orig_owner = null;
    public AgentManager m_DeliveredBy = null;
    public Vector3 scale;
    public float collision_mod = 1;
    public bool impactFirstHit = true; // So Impact/Decal Particle only triggers on first collision after thrown
    public bool OnSale = false;

    //score
    private int baseValue;
    public int value;
    public float saleMultiplier;
    public bool isRare;
    public string m_ItemName;
    
    //PRIVATE
    private ItemState m_State;
    private ItemState TempState;
    private List<AgentManager> m_Targetting_Agents = new List<AgentManager>();
    private List<GameObject> m_Outlines = new List<GameObject>();

    //movement
    private float SLEEP_THRESHOLD = GLOBAL_VALUES.ITEM_SLEEP_THRESHOLD;
    private Vector3[] previous_movements = new Vector3[GLOBAL_VALUES.ITEM_POSITION_TRACK_LENGTH];
    private Vector3 saved_velocity;
    private Vector3 saved_angular_velocity;
    private bool saved_kinematic_state;
    private Vector3 particleBaseRotation;

    //Sound
    private AudioClip land;
    private AudioClip impact;
    private AudioSource source;

    //Particle Effects
    public GameObject motionEffects;
    private string particleTransformName;
    private DecalManager m_decalManager;

    public void Register_Agent_Targetting(AgentManager targetting)
    {
        if (!m_Targetting_Agents.Contains(targetting))
        {
            m_Targetting_Agents.Add(targetting);
            Update_Outlines();
        }
    }

    public void Deregister_Agent_Targetting(AgentManager deregister)
    {
        if (m_Targetting_Agents.Contains(deregister))
        {
            m_Targetting_Agents.Remove(deregister);
            Update_Outlines();
        }
    }

    private void Update_Outlines()
    {
        int count = m_Targetting_Agents.Count;
        // turn off all outlines
        foreach(GameObject go in m_Outlines)
        {
            go.SetActive(false);
        }
        // count the registered targetters and update the outlines.
        if (count > 0)
        {
            for (int i = 1; i <= count; i++)
            {
                // turn on outline
                m_Outlines[i - 1].SetActive(true);
                // change its color
                m_Outlines[i - 1].GetComponent<MeshRenderer>().material.color = GLOBAL_VALUES.COLOR_NUMBERS[m_Targetting_Agents[i - 1].Get_Color()];
            }
        }
    }
    // Use this for initialization
    public void Start()
    {
        try
        {
            m_decalManager = this.transform.Find("Decal").GetComponent<DecalManager>();
        }
        catch
        {
            m_decalManager = null;
            Debug.Log("No Decal found on " + this.name);
        }

        particleTransformName = GLOBAL_VALUES.PARTICLE_TRANSFORM_NAME;
        saleMultiplier = GLOBAL_VALUES.SALE_SCORE_MULTIPLIER;
        for (int i = 1; i <= 4; i++)
        {
            m_Outlines.Add(transform.Find("Outline" + i).gameObject);
        }
        foreach (GameObject go in m_Outlines)
        {
            go.SetActive(false);
        }
        for (int i = 0; i < previous_movements.Length; i++)
        {
            previous_movements[i] = Vector3.zero;
        }

        //Sound
        source = GetComponent<AudioSource>();
        land = Resources.Load<AudioClip>(GLOBAL_VALUES.SOUND_ITEM_LAND);
        impact = Resources.Load<AudioClip>(GLOBAL_VALUES.SOUND_ITEM_IMPACT);

        //Particle Effects
        motionEffects = gameObject.transform.Find("Item_Trail").gameObject;
        motionEffects.SetActive(false);
        // Initialize base value (static throughout round)
        baseValue = value;

        if (OnSale)
        {
            StartSale();
        }
        else
        {
            EndSale();
        }
        SetState(ItemState.idle);
    }
    
    public void Update()
    {
        if ((orig_owner == null) && (prev_owner != null))
        {
            // first time it has been owned
            orig_owner = prev_owner;
        }
        switch (m_State)
        {
            case ItemState.idle:
                {
                    UpdateMovement();
                    break;
                }
            case ItemState.carried:
                {
                    break;
                }
            case ItemState.thrown:
                {
                    UpdateMovement();
                    if (NotMoving())
                    {
                        SetState(ItemState.idle);
                    }
                    break;
                }
            case ItemState.dropped:
                {
                    UpdateMovement();
                    if (NotMoving())
                    {
                        SetState(ItemState.idle);
                    }
                    break;
                }
            case ItemState.checkedout:
                {
                    break;
                }
            case ItemState.paused:
                {
                    break;
                }
        }
        
    }

    public AgentManager Get_Original_Owner()
    {
        return orig_owner;
    }
    public void SetState(ItemState state)
    {
        LeaveState(); 
        m_State = state;
        switch (m_State)
        {
            case ItemState.idle:
                {
                    itemBody.velocity = Vector3.zero;
                    itemBody.drag = 3;
                    prev_owner = null;
                    break;
                }
            case ItemState.carried:
                {
                    if(OnSale)
                    {
                        SetParticles(true, "Item_Carry");
                        SetParticles(false, "Item_Glow");
                    }
                    break;
                }
            case ItemState.thrown:
                {
                    motionEffects.SetActive(true);
                    break;
                }
            case ItemState.dropped:
                {
                    break;
                }
            case ItemState.checkedout:
                {
                    SetAnimator(false);
                    SetParticles(false, "Impact_Dust");
                    SetParticles(false, "Item_Trail");
                    SetParticles(false, "Item_Carry");
                    SetParticles(false, "Item_Glow");
                    break;
                }
            case ItemState.paused:
                {
                    saved_velocity = itemBody.velocity;
                    saved_angular_velocity = itemBody.angularVelocity;
                    saved_kinematic_state = itemBody.isKinematic;
                    itemBody.isKinematic = true;
                    break;
                }
        }
    }

    private void LeaveState()
    {
        switch (m_State)
        {
            case ItemState.idle:
                {
                    itemBody.drag = 1;
                    break;
                }
            case ItemState.carried:
                {
                    if (OnSale)
                    {
                        SetParticles(false, "Item_Carry");
                        SetParticles(true, "Item_Glow");
                    }
                    break;
                }
            case ItemState.thrown:
                {
                    impactFirstHit = true;
                    break;
                }
            case ItemState.dropped:
                {
                    break;
                }
            case ItemState.checkedout:
                {
                    //will never leave state
                    break;
                }
            case ItemState.paused:
                {
                    itemBody.velocity = saved_velocity;
                    itemBody.angularVelocity = saved_angular_velocity;
                    itemBody.isKinematic = saved_kinematic_state;
                    break;
                }
        }
    }

    //enable particles
    public void StartSale()
    {
        SetAnimator(true);
        if(m_State == ItemState.idle)
        {
            SetParticles(true, "Item_Glow");
        }
        else if(m_State == ItemState.carried)
        {
            SetParticles(true, "Item_Carry");
        }
        if (!isRare)
        {
            value = baseValue * (int)saleMultiplier;
        }
        OnSale = true;
    }

    //disable particles
    public void EndSale()
    {
        SetAnimator(false);
        SetParticles(false, "Item_Glow");
        SetParticles(false, "Item_Carry");
        if (!isRare)
        {
            value = baseValue;
        }
        OnSale = false;
    }

    //used for returning to previous state if paused
    public void Resume()
    {
        SetState(TempState);
    }

    public void Pause()
    {
        TempState = m_State;
        SetState(ItemState.paused);
    }

    private void UpdateMovement()
    {
        for (int i = 0; i < previous_movements.Length - 1; i++)
        {
            previous_movements[i] = previous_movements[i + 1];
        }
        previous_movements[previous_movements.Length - 1] = gameObject.transform.position;
    }

    //Sound + Particle Effects
    void OnCollisionEnter(Collision col)
    {
        if (m_State == ItemState.thrown)
        {
            if ((prev_owner != null) &&
            (prev_owner.Get_Body() != col.rigidbody))
            if ((prev_owner != null) && (prev_owner.Get_Body() != col.rigidbody))
            {
                if (impactFirstHit == true)
                {

                    impactFirstHit = false;
                    source.PlayOneShot(impact);
                    GameObject.Find("GameManagerObject").GetComponent<GameManager>().Get_MatchManager().Get_RoundManager().Get_Camera_Manager().AddForce(GLOBAL_VALUES.CAMERA_FORCE_TIME_DELAY, col.relativeVelocity * -1);
                    motionEffects.SetActive(false);

                    if (col.collider.CompareTag(GLOBAL_VALUES.TAG_DESTRUCTABLE_PARENT) || col.collider.CompareTag(GLOBAL_VALUES.TAG_ENVIRONMENT_FLOOR))
                    {
                        if (m_decalManager != null)
                        {
                            col.collider.GetComponent<Destructable>().Add_Decal(m_decalManager.AddParticle(col), m_decalManager);
                        }
                    }
                    else if (!col.collider.CompareTag(GLOBAL_VALUES.TAG_PLAYER) && !col.collider.CompareTag(GLOBAL_VALUES.TAG_AI_SECURITY) && !col.collider.CompareTag(GLOBAL_VALUES.TAG_DESTRUCTABLE_PARENT))
                    {
                        gameObject.transform.Find(GLOBAL_VALUES.ITEM_EFFECTS_DUST_IMPACT).gameObject.transform.eulerAngles = particleBaseRotation;
                        gameObject.transform.Find(GLOBAL_VALUES.ITEM_EFFECTS_DUST_IMPACT).gameObject.transform.position = gameObject.transform.position;
                        foreach (ParticleSystem p in gameObject.transform.Find(GLOBAL_VALUES.ITEM_EFFECTS_DUST_IMPACT).GetComponentsInChildren<ParticleSystem>())
                        {
                            p.Play();
                        }
                    }
                }
            }
        }
    }
    
    //activates or deactivates particles
    private void SetParticles(bool b, string s)
    {
        Transform tran = gameObject.transform.Find(s);
        //check
        if (tran == null)
        {
            tran = gameObject.transform.Find("SaleParticles").Find(s);
        }

        //check again
        if (tran != null)
        {
            if (tran.GetComponentsInChildren<ParticleSystem>() != null)
            {
                foreach (ParticleSystem p in tran.GetComponentsInChildren<ParticleSystem>())
                {
                    if (b == false)
                    {
                        p.Stop();
                        p.Clear();
                    }
                    else
                    {
                        p.Play();
                    }
                }
            }
            else
            {
                foreach (ParticleSystem p in tran.GetComponents<ParticleSystem>())
                {
                    if (b == false)
                    {
                        p.Stop();
                        p.Clear();
                    }
                    else
                    {
                        p.Play();
                    }
                }
            }
        } 
        else
        {
            Debug.Log("No Particle System " + s + " Found on " + m_ItemName);
        }
    }
    
    private void SetAnimator(bool b)
    {
        if(gameObject.GetComponent<Animator>() != null)
        {
            if(b)
            {
                gameObject.GetComponent<Animator>().enabled = true;
            }
            else
            {
                gameObject.GetComponent<Animator>().enabled = false;
            }
        }
    }

    public ItemState GetItemState()
    {
        return m_State;
    }

    //determines if an item isn't moving much
    private bool NotMoving()
    {
        return (Vector3.Distance(previous_movements[0], previous_movements[previous_movements.Length - 1]) < SLEEP_THRESHOLD) ? true : false;
    }

    public bool BeingCarried()
    {
        return m_State == ItemState.carried;
    }
}
