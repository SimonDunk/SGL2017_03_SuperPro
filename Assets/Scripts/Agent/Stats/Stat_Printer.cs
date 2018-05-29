using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Stat_Printer {
    GameManager m_GameManager;
	public Stat_Printer(GameManager gm)
    {
        m_GameManager = gm;
    }

    public void Print_Stats()
    {
        string headers = "Player Number, Dash_Usage, Punch_Usage, Groundslam Usage, Items_Thrown, Pickup_Usage, Items_Dropped, Stun_Duration, Heal_Done, Players_Stunned, AI_Stunned, Pauses, A_Missed, B_Missed, X_Missed, Y_Missed, Final_Score";
        string filename = "StatLogs\\" + System.DateTime.Now.ToString("MMddyyyyhhmm") + ".CSV";
        new System.IO.FileInfo(filename).Directory.Create();
        using (System.IO.StreamWriter outputFile = (System.IO.File.Exists(filename)) ? System.IO.File.AppendText(filename) : System.IO.File.CreateText(filename))
        {
            outputFile.WriteLine(headers);
            foreach (AgentManager player in m_GameManager.Get_Players())
            {
                string Line = player.m_StatCollector.Get_String();
                outputFile.WriteLine(Line);
            }
        }
    }
}

