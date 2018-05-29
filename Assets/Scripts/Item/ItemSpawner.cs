using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public ItemFactory m_ItemFactory;
    public Transform m_SpawnPoint;
    public ItemManager m_Manager;
    public ItemSpawnCheck spawnCheck;
    bool m_Trigger = false;
    public bool Spawn_Rare = false;
    public bool m_rndStarted = false;

    public AudioClip spawnSound;
    private AudioSource source;
    // Use this for initialization
    void Start()
    {
        m_SpawnPoint = this.gameObject.transform.Find("SpawnPoint").transform;
        spawnCheck = this.gameObject.transform.Find("SpawnPoint").GetComponent<ItemSpawnCheck>();

        source = GetComponent<AudioSource>();
        spawnSound = Resources.Load<AudioClip>(GLOBAL_VALUES.SOUND_ITEM_SPAWN);
    }

    // Update is called once per frame
    void Update()
    {
        // check trigger
        if (m_Trigger && m_ItemFactory != null)
        {
            if (Spawn_Rare)
            {
                Item i = m_ItemFactory.Get_Rare_Item(this);
                m_Manager.Get_SaleManager().m_Current_Sale = i.m_ItemName;
                m_Manager.m_Items.Add(i);
                i.OnSale = true;
            }
            else
            {
                Item i = m_ItemFactory.Get_Normal_Item(this);
                m_Manager.m_Items.Add(i);
                if (m_Manager.Get_SaleManager().m_Current_Sale == i.m_ItemName)
                {
                    i.OnSale = true;
                }
            }
            m_Trigger = false;

            if (m_rndStarted)
            {
                source.PlayOneShot(spawnSound);
            }
        }
    }

    public void Trigger()
    {
        m_Trigger = true;
    }
}
