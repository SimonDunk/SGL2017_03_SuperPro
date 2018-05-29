using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    public static bool isPaused;
    public static AgentManager pausedPlayer;

    public GameObject pauseMenuCanvas;
    public Button selectedButton; // default set to Resume button.

    public GameManager m_GameManager;

    void Start()
    {
        isPaused = false;
        pausedPlayer = null;
        m_GameManager = GameObject.Find("GameManagerObject").GetComponent<GameManager>();
        // Setup Button Colours
        Selectable tempBtn = selectedButton;
        tempBtn.image.color = Color.white; // Resume set white
        tempBtn = tempBtn.FindSelectableOnDown();
        tempBtn.image.color = Color.grey; // Quit set grey
        //tempBtn = tempBtn.FindSelectableOnDown();
        //tempBtn.image.color = Color.grey; // Exit set grey
        pauseMenuCanvas.SetActive(false);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (isPaused)
        {
            CheckInput();
            pauseMenuCanvas.SetActive(true);
        }
        else
        {
            pauseMenuCanvas.SetActive(false);
        }
    }

    public void CheckInput()
    {
        pausedPlayer.Get_Input().Update();

        Selectable newBtn = null;
        bool A_Pressed = pausedPlayer.Get_Input().Get_A_Pressed();
        bool Start_Pressed = pausedPlayer.Get_Input().Get_Start_Pressed();
        bool Vert_Positive = pausedPlayer.Get_Input().Get_Vert_Positive();
        bool Vert_Negative = pausedPlayer.Get_Input().Get_Vert_Negative();
        bool dPad_Positive = pausedPlayer.Get_Input().Get_DU_Pressed();
        bool dPad_Negative = pausedPlayer.Get_Input().Get_DD_Pressed();

        if (Vert_Positive || dPad_Positive)
        {
            newBtn = selectedButton.FindSelectableOnUp();
        }
        if (Vert_Negative || dPad_Negative)
        {
            newBtn = selectedButton.FindSelectableOnDown();
        }
        if (newBtn != null)
        {
            selectedButton.image.color = Color.gray;
            selectedButton = (Button)newBtn;
            selectedButton.Select();
            selectedButton.image.color = Color.white;
        }

        if (A_Pressed || Start_Pressed)
        {
            Submit(selectedButton.name);
        }
    }

    public void Submit(string bType)
    {
        switch (bType)
        {
            case "Resume":
                {
                    Resume();
                    break;
                }
            case "Quit":
                {
                    Quit();
                    break;
                }
            case "Exit":
                {
                    Exit();
                    break;
                }
        }
    }

    public void Resume()
    {
        pausedPlayer = null;
        isPaused = false;
    }

    public void Quit()
    {
        // Return to Main Menu
        m_GameManager.ReturnToMenu();
    }

    public void Exit()
    {
        /* ==================================
         * COMMENTED OUT FOR OPEN DAY VERSION
         * TO NOT ALLOW PEOPLE TO CLOSE OUR
         * GAME AND SCREW THINGS UP
         * ==================================
        #if UNITY_EDITOR

            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
        */
    }
}
