using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFactory{
    
    public List<GameObject> Items = new List<GameObject>();
    private int Roll;

    public ItemFactory()
    {
        Object[] Objects = Resources.LoadAll("Pickup_Items");
        foreach (Object o in Objects)
        {
            Items.Add((GameObject)o);
        }
    }

    private GameObject GetItemWithName(string name)
    {
        foreach(GameObject i in Items)
        {
            if(i.name == name)
            {
                return i;
            }
        }
        return null;
    }

    // Change Roll values to change odds of Items spawning
    // Currently Regular = 10, Sale = 30, Rare = 100
    public Item Get_Normal_Item(ItemSpawner spawner)
    {
        Roll = Random.Range(0, 11);

        switch (Roll)
        {
            case 0:
                {
                    return SpawnNewItem(spawner.transform, "banana", 100, false, 0);
                }
            case 1:
                {
                    return SpawnNewItem(spawner.transform, "BarFridge", 100, false, 0);
                }
            case 2:
                {
                    return SpawnNewItem(spawner.transform, "beachBall", 100, false, 0);
                }
            case 3:
                {
                    return SpawnNewItem(spawner.transform, "bread", 100, false, 0);
                }
            case 4:
                {
                    return SpawnNewItem(spawner.transform, "FancyMicrowave", 100, false, 0);
                }
            case 5:
                {
                    return SpawnNewItem(spawner.transform, "FancyToaster", 100, false, 0);
                }
            case 6:
                {
                    return SpawnNewItem(spawner.transform, "Flamingo", 100, false, 90);
                }
            case 7:
                {
                    return SpawnNewItem(spawner.transform, "Flatscreen_TV", 100, false, 0);
                }
            case 8:
                {
                    return SpawnNewItem(spawner.transform, "hotdog", 100, false, 0);
                }
            case 9:
                {
                    return SpawnNewItem(spawner.transform, "pizza", 100, false, 0);
                }
            case 10:
                {
                    return SpawnNewItem(spawner.transform, "shoes", 100, false, 90);
                }
            default:
                {
                    return SpawnNewItem(spawner.transform, "banana", 100, false, 0);
                }
        }
    }

    public Item Get_Rare_Item(ItemSpawner spawner)
    {
        Roll = Random.Range(0, 4);

        switch (Roll)
        {
            case 0:
                {
                    return SpawnNewItem(spawner.transform, "diamond_rock", 5000, true, 0);
                }
            case 1:
                {
                    return SpawnNewItem(spawner.transform, "holygrail", 5000, true, 0);
                }
            case 2:
                {
                    return SpawnNewItem(spawner.transform, "Toilet", 5000, true, 0);
                }
            case 3:
                {
                    return SpawnNewItem(spawner.transform, "ufo", 5000, true, 0);
                }
            default: 
                {
                    return SpawnNewItem(spawner.transform, "diamond_rock", 5000, true, 0);
                }
        }
    }

    //public Item SpawnNewItem(Transform SpawnPoint, string name, float mass, float scale, int value, float collisionMod, float encumbMod, string quality, float rotOffset)
    public Item SpawnNewItem(Transform SpawnPoint, string name, int value, bool rare, float rotOffset)
    {
        //TEST
        if (rare == true)
        {
            Debug.Log("Spawning Rare: " + name);
        }
        
        Vector3 pos = new Vector3(0, 0, 0);
        try
        {
            pos.y = (GetItemWithName(name).GetComponent<Renderer>().bounds.extents.y + 1);
        }
        catch
        {
            try
            {
                pos.y = (GetItemWithName(name).GetComponentInChildren<Renderer>().bounds.extents.y + 1);
            }
            catch
            {
                Debug.Log("ERROR: Cannot spawn item [" + name + "]");
            }
        }

        Quaternion rotation = SpawnPoint.rotation;
        rotation = Quaternion.Euler(0, rotOffset, 0);

        GameObject newItem = GameObject.Instantiate(GetItemWithName(name), (SpawnPoint.position + pos), rotation) as GameObject;
        Item newScript = newItem.GetComponent<Item>();
        newScript.m_ItemName = "PICKUP_" + name.ToUpper();
        newScript.itemBody = newScript.gameObject.GetComponent<Rigidbody>();
        newScript.itemBody.mass = 1;
        newScript.itemBody.drag = 1;
        newScript.itemBody.transform.localScale *= 1;
        newScript.scale = newScript.gameObject.transform.localScale;
        newScript.value = value;
        newScript.collision_mod = 1.5f;
        newScript.encumb_multiplier = 0.8f;
        newScript.isRare = rare;

        return newScript;
    }
}
