using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour {

    public GameObject m_Prefab_Magnet = null;
    public GameObject m_Prefab_Earthquake = null;
    public GameObject m_Prefab_Shield = null;

    private List<GameObject> m_spawners = new List<GameObject>();
    private List<GameObject> m_powerups = new List<GameObject>();

    private Timer m_Spawn_Timer = new Timer();
    private Timer m_Despawn_Timer = new Timer();

    private bool isSpawned = false;
    private bool isActive = false;



    // Use this for initialization
    void Start ()
    {
        // Look for spawners
        m_spawners.AddRange(GameObject.FindGameObjectsWithTag("Powerup_Spawner"));
        if (m_spawners.Count > 0)
        {
            // spawners exist
            isActive = true;
            m_Spawn_Timer.Reset();
            m_Despawn_Timer.Reset();
            float spawn_time = Random.Range(GLOBAL_VALUES.POWERUP_SPAWN_TIMER_MIN, GLOBAL_VALUES.POWERUP_SPAWN_TIMER_MAX);
            m_Spawn_Timer.Add(spawn_time, true);
            // turn off all powerup spawner meshsyeha 
            foreach (GameObject o in m_spawners)
            {
                if (o.GetComponent<MeshRenderer>() != null)
                {
                    o.GetComponent<MeshRenderer>().enabled = false;
                }
            }
        }
        else
        {
            isActive = false;
        }
	}

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            Update_Timers();
        }
    }

    private void Update_Timers()
    {
        if (isActive)
        {
            if (isSpawned)
            {
                m_Despawn_Timer.Update();
                if (m_Despawn_Timer.isComplete())
                {
                    // powerups despawn
                    foreach (GameObject o in m_powerups)
                    {
                        GameObject.Destroy(o);
                    }
                    m_powerups.Clear();
                    m_Spawn_Timer.Reset();
                    float spawn_time = Random.Range(GLOBAL_VALUES.POWERUP_SPAWN_TIMER_MIN, GLOBAL_VALUES.POWERUP_SPAWN_TIMER_MAX);
                    m_Spawn_Timer.Add(spawn_time, true);
                    isSpawned = false;
                }
            }
            else
            {
                m_Spawn_Timer.Update();
                if (m_Spawn_Timer.isComplete())
                {
                    // powerups spawn
                    SpawnPowerups();
                }
            }
        }
    }

    public void SpawnPowerups()
    {
        if(isActive)
        {
            isSpawned = true;
            m_Despawn_Timer.Reset();
            m_Despawn_Timer.Add(GLOBAL_VALUES.POWERUP_DESPAWN_TIMER, true);
            int spawnAmount = 2;
            // Ensure we have enough spawners to spawn powerups
            if (spawnAmount > m_spawners.Count)
            {
                // wanting to spawn too many, scaling it down to possible number
                spawnAmount = m_spawners.Count;
            }
            // make sure you only spawn enough that you wont spawn doubles
            if (spawnAmount > System.Enum.GetNames(typeof(Powerup_Type)).Length)
            {
                spawnAmount = System.Enum.GetNames(typeof(Powerup_Type)).Length;
            }
            // pick random spawners (incase there are extra)
            List<int> chosen_spawner = new List<int>();
            List<Powerup_Type> chosen_powerups = new List<Powerup_Type>();
            while (chosen_spawner.Count < spawnAmount)
            {
                int num = Random.Range(0, (m_spawners.Count - 1));
                if (! chosen_spawner.Contains(num))
                {
                    chosen_spawner.Add(num);
                }
            }
            System.Array values = System.Enum.GetValues(typeof(Powerup_Type));
            while (chosen_powerups.Count < spawnAmount)
            {   
                int num = Random.Range(0, values.Length);
                Powerup_Type chosen = (Powerup_Type)values.GetValue(num);
                if (! chosen_powerups.Contains(chosen))
                {
                    chosen_powerups.Add(chosen);
                }
            }
            for(int i = 0; i < chosen_spawner.Count; i++)
            {
                GameObject prefab = null;
                Transform spawner = m_spawners[i].transform;
                switch(chosen_powerups[i])
                {
                    case Powerup_Type.Earthquake:
                        {
                            prefab = m_Prefab_Earthquake;
                            break;
                        }
                    case Powerup_Type.Magnet:
                        {
                            prefab = m_Prefab_Magnet;
                            break;
                        }
                    case Powerup_Type.Shield:
                        {
                            prefab = m_Prefab_Shield;
                            break;
                        }
                }
                Debug.Log("SPAWNING PU [" + prefab.name + "]");
                GameObject new_pu = GameObject.Instantiate(prefab, spawner.position, spawner.rotation) as GameObject;
                m_powerups.Add(new_pu);
            }
        }
    }
    
    private void Disable_Timers()
    {
        if (isActive)
        {
            m_Spawn_Timer.Pause();
            m_Despawn_Timer.Pause();
        }
    }

    private void Enable_Timers()
    {
        if (isActive)
        {
            m_Spawn_Timer.Start();
            m_Despawn_Timer.Start();
        }
    }
    private void Reset_Timers()
    {
        if (isActive)
        {
            m_Spawn_Timer.Reset();
            m_Despawn_Timer.Reset();
        }
    }

    public void Pause()
    {
        if (isActive)
        {
            Disable_Timers();
        }
    }

    public void Resume()
    {
        if (isActive)
        {
            Enable_Timers();
        }
    }
    public void Reset_Me()
    {
        if (isActive)
        {
            foreach (GameObject o in m_powerups)
            {
                GameObject.Destroy(o);
            }
            m_powerups.Clear();
            Reset_Timers();
        }
    }
}
