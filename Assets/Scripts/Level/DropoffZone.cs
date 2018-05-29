using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropoffZone : MonoBehaviour {

    List<GameObject> list_of_items = new List<GameObject>();
    Transform CollectionPoint;
    ItemManager m_ItemManager;
    GameManager m_GameManager;
    List<SpinningLight> m_RareScoreLights = new List<SpinningLight>();

    //Sound
    public AudioClip sound;
    private AudioSource source;

    void OnTriggerEnter(Collider Other)
    {
        if ((Other.CompareTag(GLOBAL_VALUES.PICKUP_ITEM)))
        {
            list_of_items.Add(Other.gameObject);
        }
        else if ((Other.CompareTag(GLOBAL_VALUES.TAG_PLAYER)))
        {
            Other.GetComponent<CollisionDetection>().m_Manager.Drop(true);
        }
    }
    //if inside trigger collect item
    private void OnTriggerStay(Collider Other)
    {
        if ((Other.CompareTag(GLOBAL_VALUES.PICKUP_ITEM)))
        {
            list_of_items.Add(Other.gameObject);
        }
        else if ((Other.CompareTag(GLOBAL_VALUES.TAG_PLAYER)))
        {
            Other.GetComponent<CollisionDetection>().m_Manager.Drop(true);
        }
    }

    void OnTriggerExit(Collider Other)
    {
        try
        {
            list_of_items.Remove(Other.gameObject);
        }
        catch
        {
            // do nothing it just means its not in a list
        }
    }
    // Use this for initialization
    void Start () {
        GameObject a = GameObject.Find("CollectionBox");
        CollectionPoint = a.transform.Find("CollectionPoint");
        m_ItemManager = GameObject.Find("HUD").GetComponent<ItemManager>();
        //Sound
        source = GetComponent<AudioSource>();
        sound = Resources.Load<AudioClip>(GLOBAL_VALUES.SOUND_SCORE);
        m_GameManager = GameObject.Find("GameManagerObject").GetComponent<GameManager>();
        foreach(GameObject light in GameObject.FindGameObjectsWithTag("CheckoutLight"))
        {
            m_RareScoreLights.Add(light.GetComponent<SpinningLight>());
        }

    }
	
	// Update is called once per frame
	void Update () {
        Check_If_Dropped();
	}

    private void Check_If_Dropped()
    {
        for (int i = 0; i < list_of_items.Count; i++)
        {
            GameObject tempObj = list_of_items[i];
            try
            {
                Item tempItem = tempObj.GetComponent<Item>();
                if ((tempItem.prev_owner != null) && ((tempItem.GetItemState() == ItemState.thrown) || ((tempItem.prev_owner != null) && (tempItem.GetItemState() == ItemState.dropped)) || (tempItem.GetItemState() == ItemState.carried)))
                {
                    // SCORE!
                    if (tempItem.isRare)
                    {
                        tempItem.prev_owner.m_StatCollector.Count_MegaSale_Item_Checkout();
                    }
                    else if (tempItem.OnSale)
                    {
                        tempItem.prev_owner.m_StatCollector.Count_Sale_Item_Checkout();
                    }
                    else
                    {
                        tempItem.prev_owner.m_StatCollector.Count_Item_Checkout();
                    }
                    if((tempItem.Get_Original_Owner() != tempItem.prev_owner))
                    {
                        tempItem.prev_owner.m_StatCollector.Count_Item_Stolen();
                    }
                    tempItem.prev_owner.Add_Score(tempItem.value);
                    if (tempItem.isRare == true)
                    {
                        GameObject holder = GameObject.Find("RareScoreParticles");
                        foreach (ParticleSystem p in holder.transform.Find("Rare_Score_" + GLOBAL_VALUES.COLOR_NAMES[tempItem.prev_owner.Get_Color()]).GetComponentsInChildren<ParticleSystem>())
                        {
                            p.Play();
                        }
                    }
                    tempObj.transform.Find("CheckoutCashParticles").transform.eulerAngles = Vector3.zero;
                    tempObj.transform.Find("CheckoutCashParticles").GetComponent<ParticleSystem>().Emit(tempItem.value);
                    tempObj.transform.position = CollectionPoint.position;
                    tempObj.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    tempItem.SetState(ItemState.checkedout);
                    tempItem.m_DeliveredBy = tempItem.prev_owner;
                    m_ItemManager.Deliver_Item(tempItem.gameObject);
                    list_of_items.RemoveAt(i);
                    //Sound
                    source.PlayOneShot(sound);
					
					if (tempItem.isRare == true)
                    {
                        foreach (SpinningLight light in m_RareScoreLights)
                        {
                            int direction = Random.Range(0, 2);
                            int speed = Random.Range(5, 9);
                            light.Activate(direction, speed, 2f, tempItem.prev_owner.Get_Color());
                        }
                        m_GameManager.Get_MatchManager().Get_RoundManager().Get_Camera_Manager().SetScreenShake(1f, 0.075f);
                    }
                }
            }
            catch
            {
                Debug.Log("Trying to check an item that has been deleted.");
                list_of_items.RemoveAt(i);
            }
        }
        
    }
}
