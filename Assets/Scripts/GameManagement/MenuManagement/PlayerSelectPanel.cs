using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PanelState { Empty, Occupied, Ready };
public class PlayerSelectPanel : UnityEngine.MonoBehaviour {
    // PUBLIC
    public int m_Panel_Number;
    // PRIVATE
    private PanelState m_State;
    private Canvas m_Canvas;
    private GameObject m_JoinImage;
    private GameObject m_ReadyImage;
    private GameObject m_LeaveImage;
    private GameObject m_HatCycleImage;
    private GameObject m_ColorCycleImage;
    private Vector3 ColorCycleDotScale = new Vector3(200, 200, 200);
    private Vector3 ColorCycleDotScaleSelected = new Vector3(300, 300, 300);
    private Vector3 HatCycleHatScale = new Vector3(1.5f,1.5f, 1.5f);
    private Vector3 HatCycleHatScaleSelected = new Vector3(2.6f, 2.6f, 2.6f);
    private Vector3 HatWaitPos = new Vector3(0, -1000, 1000);
    private Text m_Player_Name;
    private GameObject m_BackImage;
    private AgentManager m_Agent = null;
    private Color m_BackgroundColor;
    private UnityEngine.UI.Image m_BackgroundImage;
    private GameManager m_GameManager = null;
    private Vector3 m_PlayerPos;
    private Vector3 m_NextHatPos;
    private Vector3 m_PrevHatPos;
    private Vector3 m_CurHatPos;
    private int m_HatNum = -1;
    private List<PlayerSelectPanel> m_AllPanels;
    private Camera m_Camera;
    
    private List<GameObject> m_ColorCycleImages = new List<GameObject>();
    private GameObject m_colourIcon;
    private GameObject m_LT_Trigger;
    private GameObject m_RT_Trigger;

    private List<GameObject> m_HatListPrefabs = new List<GameObject>();
    private int m_HatCount;
    private int m_HatWheelRotateAmount;
    
    void Awake ()
    {
        m_ColorCycleImage = gameObject.transform.Find("ColorCycleImage").gameObject as GameObject;
        SetupColourCycle();
        m_ColorCycleImage.SetActive(false);
        m_GameManager = GameObject.Find("GameManagerObject").GetComponent<GameManager>();
        m_Canvas = gameObject.GetComponent<Canvas>();
        m_BackgroundImage = gameObject.GetComponent<UnityEngine.UI.Image>();
        m_JoinImage = gameObject.transform.Find("JoinImage").gameObject as GameObject;
        m_JoinImage.SetActive(false);
        m_ReadyImage = gameObject.transform.Find("ReadyImage").gameObject as GameObject;
        m_ReadyImage.SetActive(false);
        m_LeaveImage = gameObject.transform.Find("LeaveImage").gameObject as GameObject;
        m_LeaveImage.SetActive(false);
        m_HatCycleImage = gameObject.transform.Find("HatCycleImage").gameObject as GameObject;
        m_HatCycleImage.SetActive(false);
        m_BackImage = gameObject.transform.Find("BackImage").gameObject as GameObject;
        m_BackImage.SetActive(false);
        m_Camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        m_Player_Name = gameObject.transform.Find("Player_Name").GetComponent<Text>();
        m_Player_Name.enabled = false;
        Align_Agent();
        SetState(PanelState.Empty);
        CreateHatList();
    }
    void Update()
    {
        Align_Agent();
        Update_ColorCycleColors();
        Update_Name();
    }
    public void Link_Panels(List<PlayerSelectPanel> panels)
    {
        m_AllPanels = panels;
    }

    private void Update_Name()
    {
        if (isOccupied())
        {
            m_Player_Name.text = m_Agent.Get_Name();
        }
    }
    private void SetState(PanelState new_state)
    {
        LeaveState();
        m_State = new_state;
        switch (m_State)
        {
            case PanelState.Empty:
                {
                    m_BackgroundColor = GLOBAL_VALUES.COLOR_GREY;
                    m_BackgroundImage.color = m_BackgroundColor;
                    m_JoinImage.SetActive(true);
                    m_Player_Name.enabled = false;
                    break;
                }
            case PanelState.Occupied:
                {
                    m_ReadyImage.SetActive(true);
                    m_HatCycleImage.SetActive(true);
                    m_ColorCycleImage.SetActive(true);
                    m_LeaveImage.SetActive(true);
                    m_Player_Name.enabled = true;
                    m_BackgroundImage.color = Color.white;
                    break;
                }
            case PanelState.Ready:
                {
                    // Give agent their hat
                    m_Agent.Put_On_Hat(m_HatNum);
                    m_Agent.Get_Animator().SetTrigger(GLOBAL_VALUES.ANIM_TRIGGER_WAVE);
                    m_BackImage.SetActive(true);
                    m_Player_Name.enabled = true;
                    break;
                }
            default:
                {
                    break;
                    // throw error?
                }
        }
    }

    public void ForceState(PanelState new_state)
    {
        // use at your own risk
        if (m_State != new_state)
        {
            SetState(new_state);
        }
    }
    private void LeaveState()
    {
        switch(m_State)
        {
            case PanelState.Empty:
                {
                    m_JoinImage.SetActive(false);
                    // hide images
                    break;
                }
            case PanelState.Occupied:
                {
                    m_ReadyImage.SetActive(false);
                    m_HatCycleImage.SetActive(false);
                    m_ColorCycleImage.SetActive(false);
                    m_LeaveImage.SetActive(false);
                    // hide images
                    break;
                }
            case PanelState.Ready:
                {
                    m_Agent.Get_Animator().Play("Idle", 0);
                    // take the hat off
                    m_Agent.RemoveHat();
                    m_BackImage.SetActive(false);
                    // hide images
                    break;
                }
            default:
                {
                    break;
                    // throw error?
                }
        }
    }

    //
    // ALL OTHER FUNCTIONS
    //
    public bool Ready_Up()
    {
        if (isOccupied() && m_State != PanelState.Ready)
        {
            SetState(PanelState.Ready);
            return true;
        }
        return false;
    }

    public bool isOccupied()
    {
        return m_Agent != null;
    }

    public AgentManager Get_Player()
    {
        return m_Agent;
    }

    private void Align_Agent()
    {
        switch(m_Panel_Number)
        {
            case 1:
                {
                    m_PlayerPos = m_Camera.ViewportToWorldPoint(new Vector3(0.25f, 0.6f, 30));
                    break;
                }
            case 2:
                {
                    m_PlayerPos = m_Camera.ViewportToWorldPoint(new Vector3(0.75f, 0.6f, 30));
                    break;
                }
            case 3:
                {
                    m_PlayerPos = m_Camera.ViewportToWorldPoint(new Vector3(0.25f, 0.1f, 30));
                    break;
                }
            case 4:
                {
                    m_PlayerPos = m_Camera.ViewportToWorldPoint(new Vector3(0.75f, 0.1f, 30));
                    break;
                }

        }
        if (m_Agent != null)
        {
            
            if (m_Agent.Get_Position() != m_PlayerPos)
            {
                m_Agent.Set_Position(m_PlayerPos);
            }
        }
    }
    public int? Get_Controller_Number()
    {
        if(isOccupied())
        {
            return m_Agent.Get_Controller_Number();
        }
        return null;
    }

    public Vector3 Get_Spawn_Pos()
    {
        return m_PlayerPos;
    }

    public bool Add_Player(int controller_num)
    {
        if (isOccupied())
        {
            return false;
        }
        else
        {
            AgentManager new_player = new AgentManager();
            //create players
            new_player.Set_Instance(GameObject.Instantiate(m_GameManager.m_PlayerPrefab, m_PlayerPos, Quaternion.Euler(0, 180, 0)) as GameObject);
            new_player.Get_Object().AddComponent<Persistant>();
            new_player.Initialise_Agent(Agent_Type.Player, controller_num);
            new_player.Disable();
            m_Agent = new_player;
            m_GameManager.Add_Player(m_Agent);
            SetState(PanelState.Occupied);
            Next_Color();
            Next_Hat();
            return true;
        }
    }
    public void Cancel()
    {
        switch(m_State)
        {
            case PanelState.Ready:
                {
                    SetState(PanelState.Occupied);
                    break;
                }
            case PanelState.Occupied:
                {
                    if (isOccupied())
                    {
                        m_GameManager.Remove_Player(m_Agent);
                        m_Agent = null;
                        SetState(PanelState.Empty);
                    }
                    break;
                }
        }
    }

    public bool isReady()
    {
        return m_State == PanelState.Ready;
    }
    private void Change_Hat(int direction)
    {
        if (isOccupied() && (!isReady()))
        {
            if (m_HatNum == -1)
            {
                m_HatNum = 0;
            }
            else
            {
                m_HatNum += direction;
                m_HatNum = (m_HatNum > (m_HatListPrefabs.Count - 1)) ? 0 : (m_HatNum < 0) ? (m_HatListPrefabs.Count - 1) : m_HatNum;
            }
            // change hat in direction
            int next_hat_num = m_HatNum + 1;
            int prev_hat_num = m_HatNum - 1;
            next_hat_num = (next_hat_num > (m_HatListPrefabs.Count - 1)) ? 0 : (next_hat_num < 0) ? (m_HatListPrefabs.Count - 1) : next_hat_num;
            prev_hat_num = (prev_hat_num > (m_HatListPrefabs.Count - 1)) ? 0 : (prev_hat_num < 0) ? (m_HatListPrefabs.Count - 1) : prev_hat_num;

            for (int i = 0; i < m_HatListPrefabs.Count; i++)
            {
                GameObject t_Hat = m_HatListPrefabs[i];
                if (i == prev_hat_num)
                {
                    t_Hat.transform.position = m_PrevHatPos;
                    t_Hat.transform.localScale = HatCycleHatScale;
                }
                else if (i == m_HatNum)
                {
                    m_Agent.RemoveHat();
                    m_Agent.Put_On_Hat(m_HatNum);
                    t_Hat.transform.position = HatWaitPos;
                    t_Hat.transform.localScale = HatCycleHatScale;
                }
                else if (i == next_hat_num)
                {
                    t_Hat.transform.position = m_NextHatPos;
                    t_Hat.transform.localScale = HatCycleHatScale;
                }
                else
                {
                    t_Hat.transform.position = HatWaitPos;
                    t_Hat.transform.localScale = HatCycleHatScale;
                }
            }

            m_Player_Name.text = m_Agent.Get_Name();
        }
    }

    private void Change_Color(int direction)
    {
        if (isOccupied() && (!isReady()))
        {
            int curr_col = m_Agent.Get_Color();
            int next_col = curr_col;
            bool taken = false;
            do
            {
                // Do this until the break or we get back to our original color
                // increment next_col and wrap around the ends
                next_col = (next_col + direction) < GLOBAL_VALUES.COLOR_FIRST_INT ?
                    GLOBAL_VALUES.COLOR_LAST_INT :
                    (next_col + direction) > GLOBAL_VALUES.COLOR_LAST_INT ?
                    GLOBAL_VALUES.COLOR_FIRST_INT :
                    (next_col + direction);
                // reset taken for this color
                taken = false;
                // for every other panel
                foreach (PlayerSelectPanel panel in m_AllPanels)
                {
                    // check its color
                    if (!panel.isOccupied())
                    {
                        // No player in this panel
                        break;
                    }
                    // player in the panel
                    if (panel.Get_Player().Get_Color() == next_col)
                    {
                        // color matches the one we are looking at, taken
                        taken = true;
                        break;
                    }
                }
                if (!taken)
                {
                    // We got through all the panels without hitting a match, its free
                    int prev_col = m_Agent.Get_Color();
                    m_Agent.Update_Color(next_col);
                    Update_Hat_Colors();
                    Update_ColorCycleScales(prev_col);
                    Update_ColorCycleColors();
                    break; // break the loop now we found a matching color
                }
            } while (next_col != curr_col);
            m_Player_Name.text = m_Agent.Get_Name();
            m_Player_Name.color = GLOBAL_VALUES.COLOR_NUMBERS[m_Agent.Get_Color()];
        }
    }

    private void Update_Hat_Colors()
    {
        if (m_State == PanelState.Occupied)
        {
            // tell hats to change color next
            foreach (GameObject hat in m_HatListPrefabs)
            {
                hat.GetComponent<HatColorManagement>().SetColor(m_Agent.Get_Color());
            }
            if (m_HatNum != -1)
            {
                m_Agent.RemoveHat();
                m_Agent.Put_On_Hat(m_HatNum);
            }
            // change the color of the hat circle
            m_HatCycleImage.transform.Find("Grey_Circle_Sprite").GetComponent<SpriteRenderer>().color = GLOBAL_VALUES.COLOR_NUMBERS[m_Agent.Get_Color()];
        }
    }
    
    private void Update_ColorCycleScales(int prev_col)
    {
        if(isOccupied())
        {
            int prev_index = prev_col + 1;
            int new_index = m_Agent.Get_Color() + 1;
            // Update Sizes
            for (int i = 0; i < m_ColorCycleImages.Count; i++)
            {
                if (i == prev_index)
                {
                    m_ColorCycleImages[i].transform.localScale = ColorCycleDotScale;
                }
                if (i == new_index)
                {
                    m_ColorCycleImages[i].transform.localScale = ColorCycleDotScaleSelected;
                }
                m_ColorCycleImages[0].transform.localScale = new Vector3(141, 141, 71);
            }
        }
    }

    private void Update_ColorCycleColors()
    {
        // Update Colors to normal
        for (int i = 0; i < m_ColorCycleImages.Count - 2; i++)
        {
            // not the lt or rt images
            m_ColorCycleImages[i + 1].GetComponent<SpriteRenderer>().color = GLOBAL_VALUES.COLOR_NUMBERS[i];
        }
        // Change taken to grey
        foreach (PlayerSelectPanel panel in m_AllPanels)
        {
            if (panel.m_Panel_Number != m_Panel_Number && panel.isOccupied())
            {
                m_ColorCycleImages[panel.Get_Player().Get_Color() + 1].GetComponent<SpriteRenderer>().color = GLOBAL_VALUES.COLOR_GREY;
            }
        }
    }
    public void Next_Hat()
    {
        Change_Hat(1);
    }

    public void Previous_Hat()
    {
        Change_Hat(-1);
    }

    public void Next_Color()
    {
        Change_Color(1);
    }

    public void Previous_Color()
    {
        Change_Color(-1);
    }

    private void CreateHatList()
    {
        // load all hats
        Object[] c = Resources.LoadAll("Hats") as Object[];
        // add them to the hat wheel
        foreach (Object hat in c)
        {
            m_HatListPrefabs.Add(GameObject.Instantiate(hat, m_HatCycleImage.transform) as GameObject );
        }
        // move the hats to their wait
        foreach (GameObject hat in m_HatListPrefabs)
        {
            hat.transform.position = HatWaitPos;
            hat.transform.localScale = HatCycleHatScale;
        }
        switch (m_Panel_Number)
        {
            case 1:
                {
                    m_PrevHatPos = m_Camera.ViewportToWorldPoint(new Vector3(0.20f, 0.86f, 35));
                    m_CurHatPos = m_Camera.ViewportToWorldPoint(new Vector3(0.25f, 0.86f, 35));
                    m_NextHatPos = m_Camera.ViewportToWorldPoint(new Vector3(0.30f, 0.86f, 35));
                    break;
                }
            case 2:
                {
                    m_PrevHatPos = m_Camera.ViewportToWorldPoint(new Vector3(0.70f, 0.86f, 35));
                    m_CurHatPos = m_Camera.ViewportToWorldPoint(new Vector3(0.75f, 0.86f, 35));
                    m_NextHatPos = m_Camera.ViewportToWorldPoint(new Vector3(0.80f, 0.86f, 35));
                    break;
                }
            case 3:
                {
                    m_PrevHatPos = m_Camera.ViewportToWorldPoint(new Vector3(0.20f, 0.36f, 35));
                    m_CurHatPos = m_Camera.ViewportToWorldPoint(new Vector3(0.25f, 0.36f, 35));
                    m_NextHatPos = m_Camera.ViewportToWorldPoint(new Vector3(0.30f, 0.36f, 35));
                    break;
                }
            case 4:
                {
                    m_PrevHatPos = m_Camera.ViewportToWorldPoint(new Vector3(0.70f, 0.36f, 35));
                    m_CurHatPos = m_Camera.ViewportToWorldPoint(new Vector3(0.75f, 0.36f, 35));
                    m_NextHatPos = m_Camera.ViewportToWorldPoint(new Vector3(0.80f, 0.36f, 35));
                    break;
                }
        }
}

    private void SetupColourCycle()
    {
        // gather resources
        m_colourIcon = Resources.Load("Menu/PlayerSelect/ColourIcon") as GameObject;
        m_LT_Trigger = Resources.Load("Menu/PlayerSelect/LT_Object") as GameObject;
        m_RT_Trigger = Resources.Load("Menu/PlayerSelect/RT_Object") as GameObject;
        // instantiate objects
        m_ColorCycleImages.Add(GameObject.Instantiate(m_LT_Trigger));
        foreach (Color32 c in GLOBAL_VALUES.COLOR_NUMBERS.Values)
        {
            GameObject setColour = GameObject.Instantiate(m_colourIcon) as GameObject;
            setColour.GetComponent<SpriteRenderer>().color = c;
            m_ColorCycleImages.Add(setColour);
        }
        m_ColorCycleImages.Add(GameObject.Instantiate(m_RT_Trigger));

        // loop through objects and place them in the image
        for (int i = 0; i < m_ColorCycleImages.Count; i++)
        {
            Vector3 scale = (i == 0) || (i == m_ColorCycleImages.Count - 1) ? new Vector3(141, 141, 71) : ColorCycleDotScale;
            AddColourCycleElement(m_ColorCycleImages[i], scale, i);
        }
    }

    private void AddColourCycleElement(GameObject obj, Vector3 scale, int position)
    {
        obj.transform.SetParent(m_ColorCycleImage.transform, true);
        obj.transform.localScale = scale;
        Vector3 pos = new Vector3((-50 * (m_ColorCycleImages.Count - 1) + 100 * position), 0, 0);
        obj.transform.localPosition = pos;
    }
}
