using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Sale_State { None, Normal, Mega };
public class SaleManager {
    //private
    private Timer m_Sale_Timer = new Timer();
    private Timer m_Slow_Timer = new Timer();
    private Timer m_Light_Timer = new Timer();
    private Sale_State m_State = Sale_State.None;
    private ItemManager m_Item_Manager = null;
    private ObjectFocussedSpotlight m_Sale_Spotlight = null;
    private List<ObjectFocussedSpotlight> m_Minor_Sale_Spotlights = new List<ObjectFocussedSpotlight>();
    private bool m_MegaSale_Trigger = false;
    public string m_Current_Sale = null;
    private Timer m_Mega_Sale_Pitty_Timer = new Timer();

    //UI effects
    private GameObject m_SalePersistant = null;
    private GameObject m_SaleTags = null;
    private GameObject m_SaleTimerText = null;
    private GameObject m_SaleImage = null;
    private GameObject m_RareSaleImage = null;
    private GameObject m_RarePopupImage = null;
    private GameObject m_RareSaleTags = null;
    private GameObject m_RareSalePersistant = null;
    private GameObject m_RarePopup = null;
    private Timer m_RarePopupTimer = new Timer();
    private float m_RarePopupTime = 2.0f;
    public List<Sprite> m_SpriteList = new List<Sprite>();
    private CameraManager m_CameraManager;
    private string Find_Least_Common()
    {
        // Create a dictionary of item names and count them
        Dictionary<string, int> counts = new Dictionary<string, int>();
        string lowest_key = "";
        int lowest_count = int.MaxValue;
        
        foreach(Item i in m_Item_Manager.m_Items)
        {
            if (counts.ContainsKey(i.m_ItemName))
            {
                counts[i.m_ItemName] += 1;
            }
            else
            {
                counts[i.m_ItemName] = 1;
            }
        }

        // Find the minimum
        foreach (KeyValuePair<string, int> entry in counts)
        {
            if (entry.Value < lowest_count)
            {
                lowest_count = entry.Value;
                lowest_key = entry.Key;
            }
        }
        // Return the name of the lowest occurance item
        return lowest_key;
    }

    public SaleManager(ItemManager item_man)
    {
        Object[] Objects = Resources.LoadAll("UI/SaleItems");
        foreach (Object o in Objects)
        {
            if (o.GetType() == typeof(Sprite))
            {
                m_SpriteList.Add((Sprite)o);
            }
        }
        m_Mega_Sale_Pitty_Timer.Reset();
        m_Mega_Sale_Pitty_Timer.Add(GLOBAL_VALUES.SALE_MEGA_PITTY_TIMER_SECONDS, true);
        // Set the item manager
        m_Item_Manager = item_man;
        m_Sale_Spotlight = GameObject.FindGameObjectWithTag("LIGHT_SALE_SPOTLIGHT").GetComponent<ObjectFocussedSpotlight>();
        m_Sale_Spotlight.Turn_Off();
        m_SalePersistant = GameObject.Find("SalePersistent");
        m_SalePersistant.SetActive(false);
        m_SaleTags = m_SalePersistant.transform.Find("Tags").gameObject;
        m_SaleTags.SetActive(false);
        m_SaleTimerText = m_SalePersistant.transform.Find("SaleBase").Find("SaleTimerText").gameObject;
        m_SaleImage = m_SalePersistant.transform.Find("SaleBase").Find("ItemPersistent").gameObject;
        m_RareSalePersistant = GameObject.Find("RareSalePersistent");
        m_RareSalePersistant.SetActive(false);
        m_RareSaleTags = m_RareSalePersistant.transform.Find("Tags").gameObject;
        m_RareSaleTags.SetActive(false);
        m_RareSaleImage = m_RareSalePersistant.transform.Find("SaleBase").Find("ItemPersistent").gameObject;
        m_RarePopup = GameObject.Find("RarePopup");
        m_RarePopup.SetActive(false);
        m_RarePopupImage = m_RarePopup.transform.Find("ItemImage").gameObject;
        m_CameraManager = GameObject.Find("GameManagerObject").GetComponent<GameManager>().Get_MatchManager().Get_RoundManager().Get_Camera_Manager();
    }
    private Sprite GetSpriteWithName(string name)
    {
        foreach (Sprite s in m_SpriteList)
        {
            if (s.name == name)
            {
                return s;
            }
        }
        return null;
    }
    private void Roll_Sale()
    {
        int roll = Random.Range(0, 100);
        if (roll <= GLOBAL_VALUES.SALE_MEGA_CHANCE || m_Mega_Sale_Pitty_Timer.isComplete())
        {
            Set_State(Sale_State.Mega);
        }
        else
        {
            Set_State(Sale_State.Normal);
        }
    }
    private void Set_State(Sale_State newState)
    {
        Leave_State();
        m_State = newState;
        switch(m_State)
        {
            case Sale_State.None:
                {
                    Roll_Sale();
                    break;
                }
            case Sale_State.Normal:
                {
                    m_Current_Sale = Find_Least_Common();
                    //UI
                    m_SalePersistant.SetActive(false);
                    Sprite new_sprite = GetSpriteWithName("Sale_" + m_Current_Sale);
                    m_SaleImage.GetComponent<Image>().sprite = new_sprite;
                    m_SaleTags.SetActive(false);
                    m_SalePersistant.SetActive(true);
                    m_SalePersistant.transform.Find("SaleBase").transform.Find("SaleBasePulse").GetComponent<Animator>().SetTrigger("Activate");
                    m_SaleTags.SetActive(true);

                    // Start the sale timer, start sale on all Find_Least_Common items
                    m_Sale_Timer.Reset();
                    m_Sale_Timer.Add(GLOBAL_VALUES.SALE_LENGTH_SECONDS, true);
                    foreach(Item i in m_Item_Manager.m_Items)
                    {
                        if (i.m_ItemName == m_Current_Sale)
                        {
                            i.StartSale();
                            Debug.Log("Sale On " + i.m_ItemName);
                            m_Minor_Sale_Spotlights.Add(GameObject.Instantiate(m_Sale_Spotlight.gameObject, new Vector3(i.transform.position.x, i.transform.position.y + 5, i.transform.position.z), Quaternion.Euler(90,0,0)).GetComponent<ObjectFocussedSpotlight>());
                            m_Minor_Sale_Spotlights[m_Minor_Sale_Spotlights.Count-1].Turn_On();
                            m_Minor_Sale_Spotlights[m_Minor_Sale_Spotlights.Count - 1].Set_Focus(i.gameObject);

                        }
                    }
                    m_Slow_Timer.Reset();
                    m_Slow_Timer.Add(GLOBAL_VALUES.SALE_SLOW_TIMER_NORMAL, true);
                    Time.timeScale = GLOBAL_VALUES.SALE_SLOW_SCALE_NORMAL;
                    m_CameraManager.SetLightIntensity(GLOBAL_VALUES.CAMERA_LIGHTS_LO_INTENSITY_1, GLOBAL_VALUES.CAMERA_LIGHTS_LO_INTENSITY_2);
                    m_Light_Timer = new Timer(GLOBAL_VALUES.CAMERA_LIGHTS_TIME_DELAY, true);
                    break;
                }
            case Sale_State.Mega:
                {
                    //UI
                    m_Mega_Sale_Pitty_Timer.Reset();
                    m_RareSalePersistant.SetActive(false);
                    m_RareSaleTags.SetActive(false);
                    m_RarePopup.SetActive(false);
                    m_RarePopup.SetActive(true);
                    m_RarePopupTimer.Add(m_RarePopupTime, true);
                    // Find the Rare Item Spawner and trigger it
                    GameObject.FindGameObjectWithTag(GLOBAL_VALUES.TAG_RARE_SPAWNER).GetComponent<ItemSpawner>().Trigger();
                    m_Sale_Timer.Reset();
                    m_Slow_Timer.Reset();
                    m_Slow_Timer.Add(GLOBAL_VALUES.SALE_SLOW_TIMER_MEGA, true);
                    Time.timeScale = GLOBAL_VALUES.SALE_SLOW_SCALE_MEGA;
                    m_CameraManager.SetLightIntensity(GLOBAL_VALUES.CAMERA_LIGHTS_LO_INTENSITY_1, GLOBAL_VALUES.CAMERA_LIGHTS_LO_INTENSITY_2);
                    m_Light_Timer = new Timer(GLOBAL_VALUES.CAMERA_LIGHTS_TIME_DELAY, true);
                    break;
                }
        }
    }

    private void Leave_State()
    {
        switch (m_State)
        {
            case Sale_State.None:
                {
                    break;
                }
            case Sale_State.Normal:
                {
                    //UI
                    m_SalePersistant.SetActive(false);
                    m_SaleTags.SetActive(false);

                    foreach (Item i in m_Item_Manager.m_Items)
                    {
                        i.EndSale();
                        m_Current_Sale = null;
                        GameObject.Destroy(m_Minor_Sale_Spotlights[m_Minor_Sale_Spotlights.Count - 1].gameObject);
                    }
                    m_Minor_Sale_Spotlights = new List<ObjectFocussedSpotlight>();
                    break;
                }
            case Sale_State.Mega:
                {
                    m_Mega_Sale_Pitty_Timer.Reset();
                    m_Mega_Sale_Pitty_Timer.Add(GLOBAL_VALUES.SALE_MEGA_PITTY_TIMER_SECONDS, true);
                    //UI
                    m_RareSalePersistant.SetActive(false);
                    m_RareSaleTags.SetActive(false);
                    m_RarePopup.SetActive(false);

                    m_Sale_Spotlight.Clear_Focus();
                    m_Sale_Spotlight.Turn_Off();
                    m_MegaSale_Trigger = false;
                    foreach (Item i in m_Item_Manager.m_Items)
                    {
                        i.EndSale();
                        m_Current_Sale = null;
                    }
                    break;
                }
        }
    }
	// Update is called once per frame
	public void Update () {
        m_Mega_Sale_Pitty_Timer.Update();
        switch (m_State)
        {
            case Sale_State.None:
                {
                    Roll_Sale();
                    break;
                }
            case Sale_State.Normal:
                {
                    Update_Sale_Timer();
                    Update_Other_Timer();
                    if (m_Sale_Timer.isComplete() || !SaleActive())
                    {
                        Set_State(Sale_State.None);
                    }
                    break;
                }
            case Sale_State.Mega:
                {
                    Update_Other_Timer();
                    if (m_MegaSale_Trigger == false)
                    {
                        foreach (Item i in m_Item_Manager.m_Items)
                        {
                            if (i.isRare)
                            {
                                m_Sale_Spotlight.Set_Focus(i.gameObject);
                                m_Sale_Spotlight.Turn_On();
                                Sprite new_sprite = GetSpriteWithName("Sale_" + i.m_ItemName);
                                m_RarePopupImage.GetComponent<Image>().sprite = new_sprite;
                                m_RareSaleImage.GetComponent<Image>().sprite = new_sprite;
                                m_MegaSale_Trigger = true;
                                break;
                            }
                        }
                    }
                    if (m_RarePopupTimer.isComplete())
                    {
                        m_RareSalePersistant.SetActive(true);
                        m_RareSalePersistant.transform.Find("SaleBase").Find("SaleBasePulse").gameObject.GetComponent<Animator>().SetTrigger("Activate");
                        m_RareSaleTags.SetActive(true);
                        m_RarePopup.SetActive(false);
                    }
                    else
                    {
                        m_RarePopupTimer.Update();
                    }
                    if(!SaleActive())
                    {
                        Set_State(Sale_State.None);
                    }
                    break;
                }
        }
    }

    private bool SaleActive()
    {
        foreach (Item i in m_Item_Manager.m_Items)
        {
            if (i.m_ItemName == m_Current_Sale)
            {
                return true;
            }
        }
        return false;
    }
    private void Update_Sale_Timer()
    {
        m_Sale_Timer.Update();
        // get the latest timer
        float time = m_Sale_Timer.Get_Time();
        // convert to minutes
        int minutes = (int)(Mathf.Round(time) / 60);
        int seconds = (int)(Mathf.Round(time) % 60);
        string sec_str = seconds >= 10 ? "" + seconds : "0" + seconds;
        m_SaleTimerText.GetComponent<Text>().text = minutes + ":" + sec_str;
    }

    private void Update_Other_Timer()
    {
        if (m_Slow_Timer.isComplete())
        {
            Time.timeScale = 1;
        }
        else
        {
            m_Slow_Timer.Update();
        }
        if (m_Light_Timer.isComplete())
        {
            m_CameraManager.SetLightIntensity(GLOBAL_VALUES.CAMERA_LIGHTS_HI_INTENSITY_1, GLOBAL_VALUES.CAMERA_LIGHTS_HI_INTENSITY_2);
        }
        else
        {
            m_Light_Timer.Update();
        }
    }
}
