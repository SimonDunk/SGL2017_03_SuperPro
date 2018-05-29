using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

public class SettingsMenuManager {
    // PUBLIC
    public bool Close = false;
    // PRIVATE
    MenuManager m_MenuManager = null;
    GameManager m_GameManager = null;

    private Dropdown RoundsDropdown;
    private Dropdown RoundDurationDropdown;
    private Toggle ShopperEnabledToggle;
    private Toggle SecurityEnabledToggle;
    private Dropdown QualityDropdown;
    private Dropdown ResolutionDropdown;
    private Toggle DefaultResolutionToggle;
    private Toggle FullScreenToggle;
    private Toggle AudioToggle;
    private Toggle MusicToggle;
    private Button SaveExit;
    private UnityEvent onSubmit;

    public SettingsMenuManager(MenuManager menu, GameManager game)
    {
        m_MenuManager = menu;
        m_GameManager = game;

        RoundsDropdown = GameObject.Find("Rounds").GetComponent<Dropdown>();
        RoundDurationDropdown = GameObject.Find("RoundDuration").GetComponent<Dropdown>();
        ShopperEnabledToggle = GameObject.Find("ShopperEnabled").GetComponent<Toggle>();
        SecurityEnabledToggle = GameObject.Find("SecurityEnabled").GetComponent<Toggle>();
        QualityDropdown = GameObject.Find("Quality").GetComponent<Dropdown>();
        ResolutionDropdown = GameObject.Find("Resolution").GetComponent<Dropdown>();
        DefaultResolutionToggle = GameObject.Find("DefaultResolution").GetComponent<Toggle>();
        FullScreenToggle = GameObject.Find("Fullscreen").GetComponent<Toggle>();
        AudioToggle = GameObject.Find("Mute").GetComponent<Toggle>();
        MusicToggle = GameObject.Find("Music").GetComponent<Toggle>();
        SaveExit = GameObject.Find("SaveExit").GetComponent<Button>();

        if (onSubmit == null)
        {
            onSubmit = new UnityEvent();
        }

        //removing preexisting values in dropdowns
        RoundsDropdown.ClearOptions();
        RoundDurationDropdown.ClearOptions();
        QualityDropdown.ClearOptions();
        ResolutionDropdown.ClearOptions();

        //adding lists
        RoundsDropdown.AddOptions(m_GameManager.Get_Settings().RoundOptions);
        RoundDurationDropdown.AddOptions(m_GameManager.Get_Settings().RoundDurationOptions);
        QualityDropdown.AddOptions(m_GameManager.Get_Settings().QualitySettingsOptions);
        ResolutionDropdown.AddOptions(m_GameManager.Get_Settings().ResolutionOptions);

        //assigning values
        RoundsDropdown.value = m_GameManager.Get_Settings().RoundsToWin;
        RoundDurationDropdown.value = (int)((m_GameManager.Get_Settings().RoundDuration - 60) / 30);
        ShopperEnabledToggle.isOn = m_GameManager.Get_Settings().ShopperEnabled;
        SecurityEnabledToggle.isOn = m_GameManager.Get_Settings().SecurityEnabled;
        QualityDropdown.value = m_GameManager.Get_Settings().Quality;
        ResolutionDropdown.value = m_GameManager.Get_Settings().ResolutionValue;
        //DefaultResolutionToggle.isOn = (Screen.width == Screen.currentResolution.width) ? true : false;
        DefaultResolutionToggle.isOn = false;
        FullScreenToggle.isOn = m_GameManager.Get_Settings().FullScreen;
        AudioToggle.isOn = m_GameManager.Get_Settings().MutedAll;
        MusicToggle.isOn = m_GameManager.Get_Settings().MutedMusic;

        //adding listeners
        //designed for making all changes at once
        onSubmit.AddListener(RoundsListener);
        onSubmit.AddListener(RoundDurationListener);
        onSubmit.AddListener(ShopperEnabledListener);
        onSubmit.AddListener(SecurityEnabledListener);
        onSubmit.AddListener(QualityListener);
        onSubmit.AddListener(ResolutionListener);
        onSubmit.AddListener(SetDefaultResolutionListener);
        onSubmit.AddListener(FullscreenListener);
        onSubmit.AddListener(MuteAllListener);
        onSubmit.AddListener(MuteMusicListener);
        SaveExit.onClick.AddListener(SaveExitListener);

        //or change settings individually
        /*
        RoundsDropdown.onValueChanged.AddListener(delegate { RoundsListener(); });
        RoundDurationDropdown.onValueChanged.AddListener(delegate { RoundDurationListener(); });
        AIEnabledToggle.onValueChanged.AddListener(delegate { AIEnabledListener(); });
        QualityDropdown.onValueChanged.AddListener(delegate { QualityListener(); });
        ResolutionDropdown.onValueChanged.AddListener(delegate { ResolutionListener(); });
        DefaultResolutionToggle.onValueChanged.AddListener(delegate { SetDefaultResolutionListener(); });
        FullScreenToggle.onValueChanged.AddListener(delegate { FullscreenListener(); });
        AudioToggle.onValueChanged.AddListener(delegate { MuteAllListener(); });
        MusicToggle.onValueChanged.AddListener(delegate { MuteMusicListener(); });
        SaveExit.onClick.AddListener(SaveExitListener);
        */
    }

    // Update is called once per frame
    public void Update()
    {
        SaveExit.GetComponentInChildren<Text>().text = (CheckChanges()) ? "Save and Exit" : "Exit";

        if (m_MenuManager.Get_Controllers().Any_X_Pressed())
        {
            //make changes
            onSubmit.Invoke();
        }
    }

    public void RoundsListener()
    {
        m_GameManager.Get_Settings().RoundsToWin = RoundsDropdown.value;
    }

    public void RoundDurationListener()
    {
        m_GameManager.Get_Settings().RoundDuration = 60 + RoundDurationDropdown.value * 30;
    }

    public void ShopperEnabledListener()
    {
        m_GameManager.Get_Settings().ShopperEnabled = (ShopperEnabledToggle.isOn) ? true : false;
    }

    public void SecurityEnabledListener()
    {
        m_GameManager.Get_Settings().SecurityEnabled = (SecurityEnabledToggle.isOn) ? true : false;
    }

    public void QualityListener()
    {
        QualitySettings.SetQualityLevel(QualityDropdown.value, m_GameManager.Get_Settings().FullScreen);
        m_GameManager.Get_Settings().Quality = QualityDropdown.value;
    }

    public void ResolutionListener()
    {
        int[] t = RipResolution(ResolutionDropdown.options[ResolutionDropdown.value].text);
        SetResolution(t[0], t[1], m_GameManager.Get_Settings().FullScreen);
        m_GameManager.Get_Settings().ResolutionValue = ResolutionDropdown.value;
    }

    public void SetDefaultResolutionListener()
    {
        if (DefaultResolutionToggle.isOn)
        {
            SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, m_GameManager.Get_Settings().FullScreen);
        }
    }

    public void FullscreenListener()
    {
        bool b = (FullScreenToggle.isOn) ? true : false;
        SetResolution(m_GameManager.Get_Settings().Resolution.width, m_GameManager.Get_Settings().Resolution.height, b);
    }

    public void MuteAllListener()
    {
        m_GameManager.Get_Settings().MutedAll = AudioToggle.isOn;
        AudioListener.pause = (m_GameManager.Get_Settings().MutedAll) ? true : false;
    }

    public void MuteMusicListener()
    {
        //doesn't work yet... same as mute all
        //m_GameManager.Get_Settings().MutedMusic = MusicToggle.isOn;
        //AudioListener.pause = (m_GameManager.Get_Settings().MutedMusic) ? true : false;
    }

    public void SaveExitListener()
    {
        onSubmit.Invoke();
        Close = true;
    }

    private int[] RipResolution(string s)
    {
        string[] i = s.Split('X');
        return Array.ConvertAll(i, int.Parse);
    }

    public void SetResolution(int w, int h, bool b)
    {
        Screen.SetResolution(w, h, b);
        m_GameManager.Get_Settings().Resolution.width = w;
        m_GameManager.Get_Settings().Resolution.height = h;
        m_GameManager.Get_Settings().FullScreen = b;
    }

    private bool CheckChanges()
    {
        return (m_GameManager.Get_Settings().RoundsToWin != RoundsDropdown.value
        || m_GameManager.Get_Settings().RoundDuration != 60 + RoundDurationDropdown.value * 30
        || m_GameManager.Get_Settings().ShopperEnabled != ShopperEnabledToggle.isOn
        || m_GameManager.Get_Settings().SecurityEnabled != SecurityEnabledToggle.isOn
        || QualitySettings.GetQualityLevel() != QualityDropdown.value
        || m_GameManager.Get_Settings().Resolution.width != RipResolution(ResolutionDropdown.options[ResolutionDropdown.value].text)[0]
        || DefaultResolutionToggle.isOn && Screen.width != Screen.currentResolution.width
        || !DefaultResolutionToggle.isOn && Screen.width == Screen.currentResolution.width
        || m_GameManager.Get_Settings().FullScreen != FullScreenToggle.isOn
        || m_GameManager.Get_Settings().MutedAll != AudioToggle.isOn
        || m_GameManager.Get_Settings().MutedMusic != MusicToggle.isOn)
        ? true : false;
    }
}
