using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GLOBAL_VALUES {
    /*  __  __                     _____      _   _   _
       |  \/  |                   / ____|    | | | | (_) 
       | \  / | ___ _ __  _   _  | (___   ___| |_| |_ _ _ __   __ _ ___ 
       | |\/| |/ _ \ '_ \| | | |  \___ \ / _ \ __| __| | '_ \ / _` / __|
       | |  | |  __/ | | | |_| |  ____) |  __/ |_| |_| | | | | (_| \__ \
       |_|  |_|\___|_| |_|\__,_| |_____/ \___|\__|\__|_|_| |_|\__, |___/
                                                               __/ |    
                                                              |___/*/
    public const int MINIMUM_PLAYERS = 2;
    public const float READY_UP_TIME = 30;
    public const float READY_UP_TIME_SHORT = 5;
    //=====================================================================================  
    /*  _  __                 _    _                _     __      __   _                 
       | |/ /                | |  | |              | |    \ \    / /  | |                
       | ' / _ __   ___   ___| | _| |__   __ _  ___| | __  \ \  / /_ _| |_   _  ___  ___ 
       |  < | '_ \ / _ \ / __| |/ / '_ \ / _` |/ __| |/ /   \ \/ / _` | | | | |/ _ \/ __|
       | . \| | | | (_) | (__|   <| |_) | (_| | (__|   <     \  / (_| | | |_| |  __/\__ \
       |_|\_\_| |_|\___/ \___|_|\_\_.__/ \__,_|\___|_|\_\     \/ \__,_|_|\__,_|\___||___/*/
    public const float KNOCKBACK_DASH = 150f;
    public const float KNOCKBACK_PUNCH_ONE = 20f;
    public const float KNOCKBACK_PUNCH_TWO = 190f;
    public const float KNOCKBACK_GROUNDSLAM = 100f;
    public const float KNOCKBACK_PUNCH_ITEM_MULTIPLIER = 5f;
    //===================================================================================== 
    /*   _____ _                 _______ _                     
        / ____| |               |__   __(_)                    
       | (___ | |_ _   _ _ __      | |   _ _ __ ___   ___  ___ 
        \___ \| __| | | | '_ \     | |  | | '_ ` _ \ / _ \/ __|
        ____) | |_| |_| | | | |    | |  | | | | | | |  __/\__ \
       |_____/ \__|\__,_|_| |_|    |_|  |_|_| |_| |_|\___||___/*/
    public const float STUN_TIME_DASH = 3f;
    public const float STUN_TIME_ITEM = 3f;
    public const float STUN_TIME_PUNCH = 1f;
    public const float STUN_TIME_GROUND_SLAM = 3f;
    //=====================================================================================
    /* _____                       ____  _     _           _     _______              
      / ____|                     / __ \| |   (_)         | |   |__   __|             
     | |  __  __ _ _ __ ___   ___| |  | | |__  _  ___  ___| |_     | | __ _  __ _ ___ 
     | | |_ |/ _` | '_ ` _ \ / _ \ |  | | '_ \| |/ _ \/ __| __|    | |/ _` |/ _` / __|
     | |__| | (_| | | | | | |  __/ |__| | |_) | |  __/ (__| |_     | | (_| | (_| \__ \
      \_____|\__,_|_| |_| |_|\___|\____/|_.__/| |\___|\___|\__|    |_|\__,_|\__, |___/
                                             _/ |                            __/ |    
                                            |__/                            |___/*/

    public const string PICKUP_ITEM = "PICKUP_ITEM";
    public const string PLAYER_SPAWN = "PlayerSpawn";
    public const string TAG_HEATMAP_BLOCK = "HEATMAP_BLOCK";
    // Not currently in use, want to update names of these variables but will do it later
    // public const string TAG_PICKUP_ITEM = "ITEM_PICKUP"
    // public const string TAG_ZONE_SPAWN = "ZONE_SPAWN"
    public const string TAG_PLAYER = "PLAYER";
    // public const string TAG_LIGHT_GLOBAL = "LIGHT_GLOBAL"
    // public const string TAG_LIGHT_CHECKOUT = "LIGHT_CHECKOUT"
    // Not currently in use, for when the AI update comes in
    public const string TAG_AI_BASIC = "AI_BASIC";
    public const string TAG_AI_LARGE = "AI_LARGE";
    public const string TAG_AI_SECURITY = "AI_SECURITY";
    public const string TAG_AI_MANAGER = "AI_MANAGER";
    public const string TAG_RARE_SPAWNER = "ITEM_SPAWNER_RARE";
    public const string TAG_NORMAL_SPAWNER = "ITEM_SPAWNER_NORMAL";
    public const string TAG_ENVIRONMENT_FLOOR = "ENVIRONMENT_FLOOR";
    public const string TAG_DESTRUCTABLE_PARENT = "DESTRUCT_PARENT_OBJECT";
    public const string TAG_DESTRUCTABLE_CLEAN = "DESTRUCT_CLEAN";
    public const string TAG_DESTRUCTABLE_DAMAGED = "DESTRUCT_DAMAGED";
    public const string TAG_DESTRUCTABLE_DEBRIS = "DESTRUCT_DEBRIS";
    //=====================================================================================
    /*  _                               
       | |                              
       | |     __ _ _   _  ___ _ __ ___ 
       | |    / _` | | | |/ _ \ '__/ __|
       | |___| (_| | |_| |  __/ |  \__ \
       |______\__,_|\__, |\___|_|  |___/
                     __/ |              
                    |___/*/

    public const int LAYER_DEFAULT = 1;
    public const int LAYER_IGNORE_RAYCAST = 3;
    public static readonly Dictionary<int, string> LAYER_NAMES = new Dictionary<int, string>()
    {
        {0, "DEFAULT" },
        {3, "IGNORE_RAYCAST" }
    };
    //=====================================================================================
    /*   _____                      _ _            _____                     _ 
        / ____|                    (_) |          / ____|                   | |
       | (___   ___  ___ _   _ _ __ _| |_ _   _  | |  __ _   _  __ _ _ __ __| |
        \___ \ / _ \/ __| | | | '__| | __| | | | | | |_ | | | |/ _` | '__/ _` |
        ____) |  __/ (__| |_| | |  | | |_| |_| | | |__| | |_| | (_| | | | (_| |
       |_____/ \___|\___|\__,_|_|  |_|\__|\__, |  \_____|\__,_|\__,_|_|  \__,_|
                                           __/ |                               
                                          |___/          */
    // AI Values
    public const float AI_TARGET_RADIUS = 0.5f;
    public const float SECURITY_ITEM_SPEED_THRESHOLD = 20;
    public const float SECURITY_THROWBACK_SPEED = 0.5f;
    public const float SECURITY_THROWBACK_DISTANCE = 20;
    public const float SECURITY_HEAL_DELAY = 0.25f;
    public const float SECURITY_PUNCH_DISTANCE = 2f;
    public const float SECURITY_DASH_DISTANCE = 6f;
    //=====================================================================================
    /*   _____                       _____                          
        / ____|                     / ____|                         
       | (___   ___ ___  _ __ ___  | (___   ___ _ __ ___  ___ _ __  
        \___ \ / __/ _ \| '__/ _ \  \___ \ / __| '__/ _ \/ _ \ '_ \ 
        ____) | (_| (_) | | |  __/  ____) | (__| | |  __/  __/ | | |
       |_____/ \___\___/|_|  \___| |_____/ \___|_|  \___|\___|_| |_|*/
    public const float SCORE_SCREEN_DROP_SPEED = 0.2f;
    public const float SCORE_SCREEN_BADGE_DELAY_AFTER_WINNER = 2.0f;
    public const float SCORE_SCREEN_WAIT_TO_COMPLETE = 15.0f;
    //=====================================================================================
    /*   _____                       _ 
        / ____|                     | |
       | (___   ___  _   _ _ __   __| |
        \___ \ / _ \| | | | '_ \ / _` |
        ____) | (_) | |_| | | | | (_| |
       |_____/ \___/ \__,_|_| |_|\__,_|*/

    public const string SOUND_GROUNDSLAM = "Sound/snd_Groundslam";
    public const string SOUND_PUNCH = "Sound/snd_PlayerPunch";
    //=====================================================================================
    // NAMES
    /*  {0, "BLUE"},
        {1, "GREEN"},
        {2, "PINK"},
        {3, "RED"},
        {4, "YELLOW"},*/
    public static string[,] PLAYER_NAMES_ARRAY = { 
        { "BLU", "BLU", "BLU", "BLU", "BLU", "BLU", "BLU" },
        { "GRN", "GRN", "GRN", "GRN", "GRN", "GRN", "GRN" },
        { "PNK", "PNK", "PNK", "PNK", "PNK", "PNK", "PNK" },
        { "RED", "RED", "RED", "RED", "RED", "RED", "RED" },
        { "YLW", "YLW", "YLW", "YLW", "YLW", "YLW", "YLW" }
    };


    //=====================================================================================
    /*  _____                                       
       |  __ \                                      
       | |__) |____      _____ _ __ _   _ _ __  ___ 
       |  ___/ _ \ \ /\ / / _ \ '__| | | | '_ \/ __|
       | |  | (_) \ V  V /  __/ |  | |_| | |_) \__ \
       |_|   \___/ \_/\_/ \___|_|   \__,_| .__/|___/
                                         | |        
                                         |_| */
    public const float POWERUP_SPAWN_TIMER_MIN = 15.0f;
    public const float POWERUP_SPAWN_TIMER_MAX = 25.0f;
    public const float POWERUP_DESPAWN_TIMER = 20.0f;
    // Shield
    public const float POWERUP_SHIELD_TIME = 10.0f;
    public const float POWERUP_SHIELD_PUNCH_DAMAGE = 0.5f;
    // Earthquake
    public const float POWERUP_QUAKE_STUN_TIME = 5.0f;
    public const float POWERUP_QUAKE_CAMERASHAKE_TIME = 2.0f;
    public const float POWERUP_QUAKE_CAMERASHAKE_INTENSITY = 1.0f;
    // Magnet
    public const float POWERUP_MAGNET_TIME = 7.0f;
    public const float POWERUP_MAGNET_STRENGTH = 15.0f;
    //=====================================================================================
    // Player Actions
    public const float BASE_PLAYER_MOVEMENT_SPEED = 100f;
    public const float BASE_PLAYER_ROTATION_SPEED = 30f;
    public const float BASE_PLAYER_STABILITY = 50f;
    public const float BASE_PLAYER_STRENGTH = 100f;
    public const float GROUNDSLAM_SLOWDOWN_AMOUNT = 0.5f;

    // Active Player Stats
    public const float PLAYER_ACTIVE_MASS = 50;
    public const float PLAYER_ACTIVE_DRAG = 45;
    public const float PLAYER_ACTIVE_ANGULARDRAG = 50;

    // Stunned Player Stats
    public const float PLAYER_STUNNED_MASS = 50;
    public const float PLAYER_STUNNED_DRAG = 35;
    public const float PLAYER_STUNNED_ANGULARDRAG = 40;
    public const float PLAYER_STUNNED_A_HEAL = 0.1f;

    // Action Cooldowns and Durations
    public const float DASH_COOLDOWN = 0.6f;
    public const float DASH_DURATION = 0.2f;
    public const float DASH_EFFECT_SPEED = 2.5f; // multiplicative
    public const float DASH_EFFECT_ROTATE = 0.0001f; // multiplicative

    //Round Functionality
    public const float ROUND_TIME_SECONDS = 120f;
    public const int ROUNDS_TO_WIN = 1;
    public const float ROUND_START_WAIT_TIME = 4.5f;

    //Player Stats
    public const float DAMAGE_RECOVERY_PER_SECOND = 5f;

    //Default numbers
    public const float ITEM_SLEEP_THRESHOLD = 0.0001f;
    public const int ITEM_POSITION_TRACK_LENGTH = 3;
    public const float DEFAULT_THROW_MULTIPLIER = 10;
    public const float ITEM_SPAWN_RATE_SECONDS = 15;
    public const int ITEMS_PER_SPAWN = 3;

    // Animation Trigger Name
    public const string ANIM_TRIGGER_DROP = "DropObject";
    public const string ANIM_TRIGGER_PICKUP = "PickUp";
    public const string ANIM_TRIGGER_WAVE = "Wave";
    public const string ANIM_FLOAT_SPEED = "Speed";
    public const string ANIM_STUN_START = "StunStart";
    public const string ANIM_STUN_LOOP = "StunLoop";
    public const string ANIM_THROW = "Throw";
    public const string ANIM_PUNCH_1 = "Punch1";
    public const string ANIM_PUNCH_2 = "Punch2";
    public const string ANIM_PUNCH_3 = "Punch_GroundSlam";
    public const string ANIM_PUNCH_CHARGE = "Punch_Charge";

    // Player materials
    public const string PLAYER_MATERIAL_BLUE = "Materials\\Players\\PlayerMat_Blue_Cel";
    public const string PLAYER_MATERIAL_GREEN = "Materials\\Players\\PlayerMat_Green_Cel";
    public const string PLAYER_MATERIAL_PINK = "Materials\\Players\\PlayerMat_Pink_Cel";
    public const string PLAYER_MATERIAL_RED = "Materials\\Players\\PlayerMat_Red_Cel";
    public const string PLAYER_MATERIAL_YELLOW = "Materials\\Players\\PlayerMat_Yellow_Cel";
    public const string PLAYER_HEAD_MATERIAL_BLUE = "Materials\\Player\\PlayerMat_Blue_Cel_Head";
    public const string PLAYER_HEAD_MATERIAL_GREEN = "Materials\\Player\\PlayerMat_Green_Cel_Head";
    public const string PLAYER_HEAD_MATERIAL_PINK = "Materials\\Player\\PlayerMat_Pink_Cel_Head";
    public const string PLAYER_HEAD_MATERIAL_RED = "Materials\\Player\\PlayerMat_Red_Cel_Head";
    public const string PLAYER_HEAD_MATERIAL_YELLOW = "Materials\\Player\\PlayerMat_Yellow_Cel_Head";

    // Camera Controls
    public const float CAMERA_DAMPENING_TIME = 0.2f;
    public const float CAMERA_EDGE_BUFFER = 20f;
    public const float CAMERA_MIN_SIZE = 90f;//85
    public const float CAMERA_MAX_SIZE = 95f;//95
    public const float CAMERA_SHAKE_INTENSITY = 0.5f;
    public const float CAMERA_RIG_MOVEMENT_INTENSITY = 10f;
    public const float CAMERA_FORCE_MOVEMENT_INTENSITY = 100f;
    public const float CAMERA_FORCE_SHAKE_INTENSITY = 10f;
    public const float CAMERA_FORCE_TIME_DELAY = .2f;
    public const float CAMERA_LIGHTS_TIME_DELAY = 3f;
    public const float CAMERA_LIGHTS_HI_INTENSITY_1 = .7f;
    public const float CAMERA_LIGHTS_HI_INTENSITY_2 = .95f;
    public const float CAMERA_LIGHTS_LO_INTENSITY_1 = .3f;
    public const float CAMERA_LIGHTS_LO_INTENSITY_2 = .5f;

    // Object Names
    public const string ITEM_EFFECTS_DUST_IMPACT = "Impact_Dust";
    // Sounds
    //Player Sounds
    public const string SOUND_PICKUP = "Sound/snd_Pickup";
    public const string SOUND_DROP = "Sound/snd_Drop";
    public const string SOUND_THROW = "Sound/snd_PlayerThrow";
    public const string SOUND_PLAYER_HIT = "Sound/snd_PlayerHit";
    public const string SOUND_PLAYER_STEP = "Sound/snd_Step";
    public const string SOUND_PLAYER_FOOTSTEP_01 = "Sound/snd_Footstep01";
    public const string SOUND_PLAYER_FOOTSTEP_02 = "Sound/snd_Footstep02";
    public const string SOUND_PLAYER_FOOTSTEP_03 = "Sound/snd_Footstep03";
    public const string SOUND_PLAYER_DASH = "Sound/snd_PlayerDash";
    public const string SOUND_PLAYER_HIT_01 = "Sound/snd_PlayerHit01";
    public const string SOUND_PLAYER_HIT_02 = "Sound/snd_PlayerHit02";
    public const string SOUND_PLAYER_HIT_03 = "Sound/snd_PlayerHit03";
    public const string SOUND_PLAYER_HIT_04 = "Sound/snd_PlayerHit04";

    //Item Sounds
    public const string SOUND_ITEM_SPAWN = "Sound/snd_ItemSpawn";
    public const string SOUND_ITEM_IMPACT = "Sound/snd_ItemImpact";
    public const string SOUND_ITEM_LAND = "Sound/snd_ItemLand";

    //Environment Sounds
    public const string SOUND_ROUND_END = "Sound/snd_EndRoundMW";
    public const string SOUND_SCORE = "Sound/snd_ItemScore";
    public const string SOUND_LOBBY_ENTER = "Sound/snd_LobbyEnter";
    public const string SOUND_LOBBY_LEAVE = "Sound/snd_LobbyLeave";
    public const string SOUND_NORMAL_SALE = "Sound/snd_NormalSale";
    public const string SOUND_RARE_SALE = "Sound/snd_RareSale";
    public const string SOUND_RARE_CHECKOUT = "Sound/snd_RareCheckout";
    public const string SOUND_LOGO_HIT = "Sound/snd_LogoHit";


    // Score UI Values
    public const float SCORE_UI_X_POS = -60;
    public const float SCORE_UI_FIRST_Y_POS = -80;
    public const float SCORE_UI_SECOND_Y_POS = -130;
    public const float SCORE_UI_THIRD_Y_POS = -180;
    public const float SCORE_UI_FOURTH_Y_POS = -230;
    public const float SCORE_UI_HEAD_MOVE_SPEED = 1;
    public const int COLOR_FIRST_INT = 0;
    public const int COLOR_LAST_INT = 4;
    public static readonly Color32 COLOR_GREY = new Color32(192, 192, 192, 255);
    public static readonly Dictionary<int, Color32> COLOR_NUMBERS = new Dictionary<int, Color32>()
    {
        {0, new Color32(35, 161, 255, 255)}, // Blue
        {1, new Color32(0, 255, 0, 255)}, // Green
        {2, new Color32(255, 0, 255, 255)}, // Pink
        {3, new Color32(255, 0, 0, 255)}, // Red
        {4, new Color32(255, 255, 0, 255)}, // Yellow
    };

    public static readonly Dictionary<int, string> COLOR_NAMES = new Dictionary<int, string>()
    {
        {0, "BLUE"},
        {1, "GREEN"},
        {2, "PINK"},
        {3, "RED"},
        {4, "YELLOW"},
    };

    public static readonly Dictionary<int, string> HAT_NAMES = new Dictionary<int, string>()
    {
        {0, "BEARD" },
        {1, "BERET" },
        {2, "CHICKEN" },
        {3, "FIRE" },
        {4, "HEADSET" },
        {5, "TOPHAT" },
        {6, "VIKING" }
    };

    public const string MUSIC_MENU_1 = "Sound\\Menu";
    public const string MUSIC_TRACK_1 = "Sound\\Game";
    public const string MUSIC_TRACK_2 = "Sound\\LevelLoopMW";

    //Music
    public const string MUSIC_ATTRACT_MW = "Sound/snd_AttractLoopMW"; //Mitch's Attract Loop
    public const string MUSIC_ATTRACT_YD = "Sound/snd_AttractLoopYD"; //Yolanda's Attract Loop

    //Mitch's Level Loop Layers
    public const string MUSIC_MW_LEVEL_BED = "Sound/LevelLoopMW/snd_LevelBedMW";
    public const string MUSIC_MW_LEVEL_CRAZY_ORGAN = "Sound/LevelLoopMW/snd_LevelCrazyOrganMW";
    public const string MUSIC_MW_LEVEL_GUITAR = "Sound/LevelLoopMW/snd_LevelGuitarMW";
    public const string MUSIC_MW_LEVEL_MELODY = "Sound/LevelLoopMW/snd_LevelMelodyMW";
    public const string MUSIC_MW_LEVEL_ORGAN = "Sound/LevelLoopMW/snd_LevelOrganMW";

    public const string MUSIC_FULL_LEVEL_LOOP_YD = "Sound/snd_LevelLoopYD"; //Yolanda's Full Level Loop
    public const string MUSIC_FULL_LEVEL_LOOP_MW = "Sound/snd_LevelLoopMW"; //Mitch's Full Level Loop

    //Particle Systems
    public static readonly Color32 MOTION_TRAIL_BLUE = new Color32(0, 130, 255, 255);
    public static readonly Color32 MOTION_TRAIL_GREEN = new Color32(100, 255, 0, 255);
    public static readonly Color32 MOTION_TRAIL_PINK = new Color32(255, 75, 200, 255);
    public static readonly Color32 MOTION_TRAIL_RED = new Color32(255, 60, 60, 255);
    public static readonly Color32 MOTION_TRAIL_YELLOW = new Color32(255, 255, 40, 255);

    public static readonly Dictionary<int, Color32> IMPACT_COLOURS_BACKGROUND = new Dictionary<int, Color32>()
    {
        {0, new Color32(75, 135, 255, 255)}, // Blue
        {1, new Color32(70, 205, 70, 255)}, // Green
        {2, new Color32(255, 105, 255, 255)}, // Pink
        {3, new Color32(255, 75, 75, 255)}, // Red
        {4, new Color32(255, 255, 0, 255)}, // Yellow
    };

    public static readonly Dictionary<int, Color32> IMPACT_COLOURS_FOREGROUND = new Dictionary<int, Color32>()
    {
        {0, new Color32(140, 245, 255, 255)}, // Blue
        {1, new Color32(165, 255, 145, 255)}, // Green
        {2, new Color32(255, 200, 255, 255)}, // Pink
        {3, new Color32(255, 145, 145, 255)}, // Red
        {4, new Color32(255, 255, 190, 255)}, // Yellow
    };

    public static readonly Dictionary<int, GradientColorKey[]> GROUNDSLAM_INWARD_LINES_COLOUR = new Dictionary<int, GradientColorKey[]>()
    {
        {0, new GradientColorKey[]{new GradientColorKey(Color.white, 0.55f), new GradientColorKey(new Color(0.0f, 192f / 255.0f, 1.0f), 1.0f)}}, // Blue
        {1, new GradientColorKey[]{new GradientColorKey(Color.white, 0.55f), new GradientColorKey(new Color(69f / 255.0f, 1.0f, 132f / 255.0f), 1.0f)}}, // Green
        {2, new GradientColorKey[]{new GradientColorKey(Color.white, 0.55f), new GradientColorKey(new Color(1.0f, 129f / 255.0f, 174f / 255.0f), 1.0f)}}, // Pink
        {3, new GradientColorKey[]{new GradientColorKey(Color.white, 0.55f), new GradientColorKey(new Color(1.0f, 72f / 255.0f, 72f / 255.0f), 1.0f)}}, // Red
        {4, new GradientColorKey[]{new GradientColorKey(Color.white, 0.55f), new GradientColorKey(new Color(1.0f, 220f / 255.0f, 72f / 255.0f), 1.0f)}}, // Yellow
    };

    public static readonly GradientAlphaKey[] GROUNDSLAM_INWARD_LINES_ALPHA = new GradientAlphaKey[]
    {
        new GradientAlphaKey(1.0f, 0.9f), new GradientAlphaKey(0.0f, 1.0f)
    };

    public static readonly Dictionary<int, Color32> GROUNDSLAM_OUTWARD_PULSE = new Dictionary<int, Color32>()
    {
        {0, new Color32(0, 192, 255, 255)}, // Blue
        {1, new Color32(69, 255, 132, 255)}, // Green
        {2, new Color32(255, 129, 174, 255)}, // Pink
        {3, new Color32(255, 72, 72, 255)}, // Red
        {4, new Color32(255, 220, 72, 255)}, // Yellow
    };

    public static readonly Dictionary<int, GradientColorKey[]> GROUNDSLAM_CIRCLES_COLOUR = new Dictionary<int, GradientColorKey[]>()
    {
        {0, new GradientColorKey[]{new GradientColorKey(new Color(0.0f, 192f/255.0f, 1.0f), 1.0f), new GradientColorKey(Color.white, 0.55f)}}, // Blue
        {1, new GradientColorKey[]{new GradientColorKey(new Color(69f / 255.0f, 1.0f, 132f / 255.0f), 1.0f), new GradientColorKey(Color.white, 0.55f)}}, // Green
        {2, new GradientColorKey[]{new GradientColorKey(new Color(1.0f, 129f / 255.0f, 174f / 255.0f), 1.0f), new GradientColorKey(Color.white, 0.55f)}}, // Pink
        {3, new GradientColorKey[]{new GradientColorKey(new Color(1.0f, 72f / 255.0f, 72f / 255.0f), 1.0f), new GradientColorKey(Color.white, 0.55f)}}, // Red
        {4, new GradientColorKey[]{new GradientColorKey(new Color(1.0f, 220f / 255.0f, 72f / 255.0f), 1.0f), new GradientColorKey(Color.white, 0.55f)}}, // Yellow
    };

    public static readonly GradientAlphaKey[] GROUNDSLAM_CIRCLES_ALPHA = new GradientAlphaKey[]
    {
        new GradientAlphaKey(0.0f, 1.0f), new GradientAlphaKey(1.0f, 0.0f)
    };

    // Item Sales
    public const float SALE_SCORE_MULTIPLIER = 10;
    public const string PARTICLE_TRANSFORM_NAME = "SaleParticles";
    public const int SALE_MEGA_CHANCE = 10;
    public const float SALE_MEGA_PITTY_TIMER_SECONDS = 45.0f;
    public const float SALE_LENGTH_SECONDS = 15f;
    public const float SALE_SLOW_TIMER_NORMAL = 0f;
    public const float SALE_SLOW_TIMER_MEGA = 0f;
    public const float SALE_SLOW_SCALE_NORMAL = 1f;
    public const float SALE_SLOW_SCALE_MEGA = 1f;

    // Environment Damage
    public const int WALL_HEALTH = 2;
    
    // Cooldowns
    public const float COOLDOWN_PUNCH = 0.5f;
    public const float COOLDOWN_GROUNDSLAM = 3f;
    public const float GROUNDSLAM_CHARGE_TIME = 1.0f;

    //Controller Rumble
    public const float CONTROLLER_RUMBLE_DURATION = .5f;
    public const float CONTROLLER_RUMBLE_INTENSITY = .5f;

    // Camera Shake
    public const float CAMERA_SHAKE_MAX_FORCE = 15.0f;
}
