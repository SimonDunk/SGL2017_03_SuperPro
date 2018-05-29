using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings {

    public int RoundsToWin;
    public float RoundDuration;
    public bool ShopperEnabled;
    public bool SecurityEnabled;
    public int ResolutionValue;
    public Resolution Resolution;
    public int Quality;
    public bool FullScreen;
    public bool MutedAll;
    public bool MutedMusic;

    public List<string> RoundOptions;
    public List<string> ResolutionOptions;
    public List<string> RoundDurationOptions;
    public List<string> QualitySettingsOptions;
    // ADD IN ALL GAME SETTINGS TO BE CHANGED FROM THE SettingsMenuManager

    public GameSettings()
    {
        RoundsToWin = 1;
        RoundDuration = GLOBAL_VALUES.ROUND_TIME_SECONDS;
        ShopperEnabled = false;
        SecurityEnabled = true;
        ResolutionValue = 0;
        Resolution.width = Screen.width;
        Resolution.height = Screen.height;
        Quality = QualitySettings.GetQualityLevel();
        FullScreen = true;
        MutedAll = false;
        MutedMusic = false;

        ResolutionOptions = new List<string>();
        ResolutionOptions.Add(Screen.width + "X" + Screen.height);
        ResolutionOptions.Add(Screen.currentResolution.width + "X" + Screen.currentResolution.height);
        ResolutionOptions.Add("800X600");
        ResolutionOptions.Add("1024X768");
        ResolutionOptions.Add("1280X720");
        ResolutionOptions.Add("1280X1024");
        ResolutionOptions.Add("1366X768");
        ResolutionOptions.Add("1600X900");
        ResolutionOptions.Add("1920X1080");
        ResolutionOptions.Add("2560X1440");

        RoundOptions = new List<string>();
        RoundOptions.Add("Infinite");
        RoundOptions.Add("1");
        RoundOptions.Add("2");
        RoundOptions.Add("3");
        RoundOptions.Add("4");
        RoundOptions.Add("5");

        RoundDurationOptions = new List<string>();
        RoundDurationOptions.Add("60");
        RoundDurationOptions.Add("90");
        RoundDurationOptions.Add("120");
        RoundDurationOptions.Add("150");
        RoundDurationOptions.Add("180");

        QualitySettingsOptions = new List<string>(QualitySettings.names);
    }
}
