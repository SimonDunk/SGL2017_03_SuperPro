using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public List<Item> m_Items = new List<Item>();
    public List<GameObject> m_Spawners = new List<GameObject>();
    private ItemFactory m_Factory;
    public List<Item> m_Delivered_Items = new List<Item>();

    private Timer Spawn_Timer = new Timer(10);
    // Use this for initialization
    void Start()
    {
        m_Factory = new ItemFactory(); 
        foreach (GameObject i in GameObject.FindGameObjectsWithTag("ITEM_SPAWNER_NORMAL"))
        {
            m_Spawners.Add(i);
            ItemSpawner itemClass = (ItemSpawner)i.GetComponent(typeof(ItemSpawner));
            itemClass.m_Manager = this;
            itemClass.m_ItemFactory = m_Factory;
        }

        foreach (GameObject i in GameObject.FindGameObjectsWithTag("ITEM_SPAWNER_RARE")) // Add to list if needed
        {
            ItemSpawner itemClass = (ItemSpawner)i.GetComponent(typeof(ItemSpawner));
            itemClass.m_Manager = this;
            itemClass.m_ItemFactory = m_Factory;
            itemClass.Spawn_Rare = true;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        Update_Timer();
    }

    private void OnDestroy()
    {
        List<Item> items_copy = new List<Item>(m_Items);
        foreach (Item i in items_copy)
        {
            m_Items.Remove(i);
            if (i != null)
            {
                GameObject.Destroy(i.gameObject);
            }
        }

        List<Item> delivered_copy = new List<Item>(m_Delivered_Items);
        foreach (Item i in delivered_copy)
        {
            m_Delivered_Items.Remove(i);
            if (i != null)
            {
                GameObject.Destroy(i.gameObject);
            }
        }
    }
    private void Update_Timer()
    {
        Spawn_Timer.Update();
        if (Spawn_Timer.isComplete())
        {
            Spawn_Items();
            Spawn_Timer.Add(GLOBAL_VALUES.ITEM_SPAWN_RATE_SECONDS, true);
        }
    }

    public void Spawn_Items(int SpawnNum = GLOBAL_VALUES.ITEMS_PER_SPAWN)
    {
        int Spawn_Number = SpawnNum;
        List<GameObject> spawns = new List<GameObject>();
        List<GameObject> availableSpawners = GetAvailableSpawns();

        // Get three random spawners
        if (availableSpawners.Count >= Spawn_Number)
        {
            for (int i = 0; i < Spawn_Number; i++)
            {
                int num = Random.Range(0, (availableSpawners.Count));
                while (spawns.Contains(availableSpawners[num]))
                {
                    num = Random.Range(0, (availableSpawners.Count));
                }
                spawns.Add(availableSpawners[num]);
            }
            foreach (GameObject s in spawns)
            {
                ItemSpawner other = (ItemSpawner)s.GetComponent(typeof(ItemSpawner));
                other.Trigger();
            }
        }
    }

    public void EnableSounds()
    {
        foreach (GameObject g in m_Spawners)
        {
            g.GetComponent<ItemSpawner>().m_rndStarted = true;
        }
    }

    public void Reset_Timer()
    {
        Spawn_Timer.Reset();
        Spawn_Timer.Add(GLOBAL_VALUES.ITEM_SPAWN_RATE_SECONDS);
    }

    public void Enable_Timer()
    {
        Reset_Timer();
        Spawn_Timer.Start();
        Spawn_Items();
    }

    public void Disable_Timer()
    {
        Spawn_Timer.Pause();
    }

    public void Pause()
    {
        Disable_Timer();
        foreach (Item item in m_Items)
        {
            item.Pause();
        }
    }

    public void Resume()
    {
        Spawn_Timer.Start();
        foreach (Item item in m_Items)
        {
            item.Resume();
        }
    }
    public void Reset_Me()
    {

        foreach (Item item in m_Items)
        {
            //Object.Destroy(item.instance);
            Object.Destroy(item.gameObject);
        }
        foreach (GameObject obj in m_Spawners)
        {
            obj.transform.Find("SpawnPoint").gameObject.GetComponent<ItemSpawnCheck>().Reset();
        }
        m_Items.Clear();
        Reset_Timer();
    }

    public void Deliver_Item(GameObject item)
    {
        m_Items.Remove(item.GetComponent<Item>());
        m_Delivered_Items.Add(item.GetComponent<Item>());
        item.GetComponent<AudioSource>().enabled = false;
        item.GetComponent<Item>().motionEffects.SetActive(false);
    }

    private List<GameObject> GetAvailableSpawns()
    {
        List<GameObject> returnList = new List<GameObject>();

        foreach (GameObject g in m_Spawners)
        {
            ItemSpawner tempSpawner = (ItemSpawner)g.GetComponent(typeof(ItemSpawner));

            if (tempSpawner.spawnCheck.isFree())
            {
                returnList.Add(g);
            }
        }
        return returnList;
    }

    public SaleManager Get_SaleManager()
    {
        return GameObject.Find("GameManagerObject").GetComponent<GameManager>().Get_Match().Get_RoundManager().Get_SaleManager(); ;
    }
}