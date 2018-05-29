using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnZone : MonoBehaviour{

    private Transform m_SpawnPlane;
    private List<Vector3> selected_points = new List<Vector3>();

    private void Awake()
    {
        m_SpawnPlane = this.gameObject.transform;
    }

    public Vector3 Get_Random_Spawn_Point()
    {
        float pos_x, pos_y, pos_z;
        pos_x = Random.Range((m_SpawnPlane.position.x - (m_SpawnPlane.localScale.x / 2)), (m_SpawnPlane.position.x + (m_SpawnPlane.localScale.x / 2)));
        pos_y = m_SpawnPlane.position.y;
        pos_z = Random.Range((m_SpawnPlane.position.z - (m_SpawnPlane.localScale.z / 2)), (m_SpawnPlane.position.z + (m_SpawnPlane.localScale.z / 2)));
        Vector3 newVect = new Vector3(pos_x, pos_y, pos_z);
        return newVect;
    }
    public Vector3 Get_Unique_Random_Spawn_Point()
    {
        bool new_selection = false;
        int attempts = 0;
        do
        {
            Vector3 newVect = Get_Random_Spawn_Point();
            if (!selected_points.Contains(newVect))
            {
                new_selection = true;
                selected_points.Add(newVect);
                return newVect;
            }
            attempts++;
        } while ((!new_selection) && (attempts < 50));
        return Get_Random_Spawn_Point();
    }

    public void Reset_Spawning()
    {
        selected_points.Clear();
    }
}
