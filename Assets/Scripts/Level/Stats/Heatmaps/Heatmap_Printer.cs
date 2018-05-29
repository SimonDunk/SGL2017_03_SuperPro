using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heatmap_Printer : MonoBehaviour {

	public void Print_Heatmap_Stats()
    {
        string filename = "StatLogs\\Heatmaps\\Heatmap_" + System.DateTime.Now.ToString("MMddyyyyhhmm") + ".CSV";
        new System.IO.FileInfo(filename).Directory.Create();
        using (System.IO.StreamWriter outputFile = (System.IO.File.Exists(filename)) ? System.IO.File.AppendText(filename) : System.IO.File.CreateText(filename))
        {
            outputFile.WriteLine("BARGAIN BASH HEATMAP");
            outputFile.WriteLine("Level," + GameObject.Find("GameManagerObject").GetComponent<GameManager>().Current_Scene());
            string header = "";
            foreach (GameObject block in GameObject.FindGameObjectsWithTag(GLOBAL_VALUES.TAG_HEATMAP_BLOCK))
            {
                if (header == "")
                {
                    header = block.GetComponent<Heatmap_Area>().GetHeaderString();
                    outputFile.WriteLine(header);
                }
                string Line = block.GetComponent<Heatmap_Area>().GetDataString();
                outputFile.WriteLine(Line);
                block.GetComponent<Heatmap_Area>().Reset();
            }
        }
    }
}
