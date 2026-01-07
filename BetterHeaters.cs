using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using HutongGames.PlayMaker;
using System.Collections;

[BepInPlugin("com.jager.betterheaters", "Better Heaters", "1.0.0")]
public class BetterHeaters : BaseUnityPlugin
{
    // Garage heater variables
    private FsmFloat garageHeatingPowerVar;
    private PlayMakerFSM garageLogicFSM;
    private bool garageModEnabled = false;
    private HeatLevel garageHeatLevel = HeatLevel.Hot;

    // House heater variables
    private FsmFloat houseHeatingPowerVar;
    private PlayMakerFSM houseLogicFSM;
    private bool houseModEnabled = false;
    private HeatLevel houseHeatLevel = HeatLevel.Hot;

    // GUI variables
    private Rect windowRect = new Rect(20, 20, 300, 280); // Taller window
    private bool showGUI = false;

    // Heating levels
    private enum HeatLevel { Low, Warm, Hot, Scorching }

    // Config entries
    private ConfigEntry<bool> configGarageEnabled;
    private ConfigEntry<HeatLevel> configGarageHeatLevel;
    private ConfigEntry<bool> configHouseEnabled;
    private ConfigEntry<HeatLevel> configHouseHeatLevel;
    private ConfigEntry<KeyCode> configToggleKey;

    // Track last key press to prevent multiple toggles
    private bool f10WasPressed = false;

    private void Awake()
    {
        // Setup config
        configGarageEnabled = Config.Bind("Garage", "Enabled", false, "Enable/disable the garage radiator control");
        configGarageHeatLevel = Config.Bind("Garage", "HeatLevel", HeatLevel.Hot, "Heat level for the garage radiator");

        configHouseEnabled = Config.Bind("House", "Enabled", false, "Enable/disable the house radiator control");
        configHouseHeatLevel = Config.Bind("House", "HeatLevel", HeatLevel.Hot, "Heat level for the house radiator");

        configToggleKey = Config.Bind("Controls", "ToggleGUI", KeyCode.F10, "Key to toggle the GUI window");

        // Load saved settings
        garageModEnabled = configGarageEnabled.Value;
        garageHeatLevel = configGarageHeatLevel.Value;
        houseModEnabled = configHouseEnabled.Value;
        houseHeatLevel = configHouseHeatLevel.Value;
    }

    private void Start()
    {
        // Start the update coroutines
        StartCoroutine(UpdateGarageHeatingRoutine());
        StartCoroutine(UpdateHouseHeatingRoutine());
    }

    private IEnumerator UpdateGarageHeatingRoutine()
    {
        // Wait a bit to let the game initialize
        yield return new WaitForSeconds(5f);

        while (true)
        {
            UpdateGarageHeatingValue();
            yield return new WaitForSeconds(30f);
        }
    }

    private IEnumerator UpdateHouseHeatingRoutine()
    {
        // Wait a bit to let the game initialize
        yield return new WaitForSeconds(5f);

        while (true)
        {
            UpdateHouseHeatingValue();
            yield return new WaitForSeconds(30f);
        }
    }

    private void UpdateGarageHeatingValue()
    {
        // Find RadiatorsGarage if not already found
        if (garageLogicFSM == null)
        {
            GameObject radiatorsGarage = GameObject.Find("RadiatorsGarage");
            if (radiatorsGarage != null)
            {
                garageLogicFSM = radiatorsGarage.GetComponent<PlayMakerFSM>();
                if (garageLogicFSM != null)
                {
                    garageHeatingPowerVar = garageLogicFSM.FsmVariables.FindFsmFloat("HeatingPower");
                    if (garageHeatingPowerVar == null)
                    {
                        Debug.LogError("[BetterHeaters] HeatingPower variable not found in Garage Logic FSM!");
                    }
                    else
                    {
                        Debug.Log("[BetterHeaters] Found Garage HeatingPower variable!");
                    }
                }
                else
                {
                    Debug.LogError("[BetterHeaters] Logic FSM not found on RadiatorsGarage!");
                }
            }
            else
            {
                Debug.LogError("[BetterHeaters] RadiatorsGarage object not found!");
            }
        }

        // Set the variable if found and mod is enabled
        if (garageHeatingPowerVar != null && garageModEnabled)
        {
            garageHeatingPowerVar.Value = GetHeatLevelValue(garageHeatLevel);
            Debug.Log($"[BetterHeaters] Garage HeatingPower set to {garageHeatingPowerVar.Value} ({garageHeatLevel})!");
        }
    }

    private void UpdateHouseHeatingValue()
    {
        // Find RadiatorsHouse if not already found
        if (houseLogicFSM == null)
        {
            GameObject radiatorsHouse = GameObject.Find("RadiatorsHouse");
            if (radiatorsHouse != null)
            {
                houseLogicFSM = radiatorsHouse.GetComponent<PlayMakerFSM>();
                if (houseLogicFSM != null)
                {
                    houseHeatingPowerVar = houseLogicFSM.FsmVariables.FindFsmFloat("HeatingPower");
                    if (houseHeatingPowerVar == null)
                    {
                        Debug.LogError("[BetterHeaters] HeatingPower variable not found in House Logic FSM!");
                    }
                    else
                    {
                        Debug.Log("[BetterHeaters] Found House HeatingPower variable!");
                    }
                }
                else
                {
                    Debug.LogError("[BetterHeaters] Logic FSM not found on RadiatorsHouse!");
                }
            }
            else
            {
                Debug.LogError("[BetterHeaters] RadiatorsHouse object not found!");
            }
        }

        // Set the variable if found and mod is enabled
        if (houseHeatingPowerVar != null && houseModEnabled)
        {
            houseHeatingPowerVar.Value = GetHeatLevelValue(houseHeatLevel);
            Debug.Log($"[BetterHeaters] House HeatingPower set to {houseHeatingPowerVar.Value} ({houseHeatLevel})!");
        }
    }

    private void OnGUI()
    {
        // Check for F10 key press in OnGUI (which already runs every frame)
        bool f10IsPressed = Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.F10;

        // Only toggle if F10 was just pressed (not held)
        if (f10IsPressed && !f10WasPressed)
        {
            showGUI = !showGUI;
        }

        // Store the current state for next frame
        f10WasPressed = f10IsPressed;

        // Only draw window if it should be visible
        if (!showGUI) return;

        windowRect = GUI.Window(0, windowRect, WindowFunction, "Better Heaters");

        // Keep window on screen
        windowRect.x = Mathf.Clamp(windowRect.x, 0, Screen.width - windowRect.width);
        windowRect.y = Mathf.Clamp(windowRect.y, 0, Screen.height - windowRect.height);
    }

    private void WindowFunction(int windowID)
    {
        // Make window draggable
        GUI.DragWindow(new Rect(0, 0, 280, 20));

        // === GARAGE SECTION ===
        GUI.Label(new Rect(10, 30, 280, 25), "GARAGE HEATERS", GetSectionHeaderStyle());

        // Garage toggle
        bool newGarageEnabled = GUI.Toggle(new Rect(10, 55, 30, 30), garageModEnabled, "");
        if (GUI.Button(new Rect(40, 55, 170, 30), "Enable Garage Heaters", GUI.skin.label))
        {
            newGarageEnabled = !garageModEnabled;
        }

        if (newGarageEnabled != garageModEnabled)
        {
            garageModEnabled = newGarageEnabled;
            configGarageEnabled.Value = garageModEnabled;
            Config.Save();
            UpdateGarageHeatingValue();
        }

        // Garage status
        string garageStatus = garageModEnabled ? "ACTIVE" : "DISABLED";
        GUI.color = garageModEnabled ? Color.green : Color.red;
        GUI.Label(new Rect(220, 55, 70, 30), garageStatus);
        GUI.color = Color.white;

        // Garage slider
        float garageSliderValue = GUI.HorizontalSlider(new Rect(10, 90, 200, 30), (float)garageHeatLevel, 0, 3);
        int garageSnappedValue = Mathf.RoundToInt(garageSliderValue);
        HeatLevel newGarageHeatLevel = (HeatLevel)garageSnappedValue;

        // Garage heat level display
        GUIStyle garageHeatStyle = GetHeatLevelStyle(newGarageHeatLevel);
        garageHeatStyle.alignment = TextAnchor.MiddleCenter;
        GUI.Label(new Rect(10, 115, 200, 25), GetHeatLevelLabel(newGarageHeatLevel), garageHeatStyle);

        if (newGarageHeatLevel != garageHeatLevel)
        {
            garageHeatLevel = newGarageHeatLevel;
            configGarageHeatLevel.Value = garageHeatLevel;
            Config.Save();
            if (garageModEnabled) UpdateGarageHeatingValue();
        }

        // === HOUSE SECTION ===
        GUI.Label(new Rect(10, 145, 280, 25), "HOUSE HEATERS", GetSectionHeaderStyle());

        // House toggle
        bool newHouseEnabled = GUI.Toggle(new Rect(10, 170, 30, 30), houseModEnabled, "");
        if (GUI.Button(new Rect(40, 170, 170, 30), "Enable House Heaters", GUI.skin.label))
        {
            newHouseEnabled = !houseModEnabled;
        }

        if (newHouseEnabled != houseModEnabled)
        {
            houseModEnabled = newHouseEnabled;
            configHouseEnabled.Value = houseModEnabled;
            Config.Save();
            UpdateHouseHeatingValue();
        }

        // House status
        string houseStatus = houseModEnabled ? "ACTIVE" : "DISABLED";
        GUI.color = houseModEnabled ? Color.green : Color.red;
        GUI.Label(new Rect(220, 170, 70, 30), houseStatus);
        GUI.color = Color.white;

        // House slider
        float houseSliderValue = GUI.HorizontalSlider(new Rect(10, 205, 200, 30), (float)houseHeatLevel, 0, 3);
        int houseSnappedValue = Mathf.RoundToInt(houseSliderValue);
        HeatLevel newHouseHeatLevel = (HeatLevel)houseSnappedValue;

        // House heat level display
        GUIStyle houseHeatStyle = GetHeatLevelStyle(newHouseHeatLevel);
        houseHeatStyle.alignment = TextAnchor.MiddleCenter;
        GUI.Label(new Rect(10, 230, 200, 25), GetHeatLevelLabel(newHouseHeatLevel), houseHeatStyle);

        if (newHouseHeatLevel != houseHeatLevel)
        {
            houseHeatLevel = newHouseHeatLevel;
            configHouseHeatLevel.Value = houseHeatLevel;
            Config.Save();
            if (houseModEnabled) UpdateHouseHeatingValue();
        }

        // === INFO SECTION ===
        GUI.Label(new Rect(10, 260, 280, 20), "Press F10 to hide/show window");
    }

    // Helper methods
    private float GetHeatLevelValue(HeatLevel level)
    {
        switch (level)
        {
            case HeatLevel.Low: return 0.25f;
            case HeatLevel.Warm: return 1f;
            case HeatLevel.Hot: return 2f;
            case HeatLevel.Scorching: return 3f;
            default: return 2f;
        }
    }

    private string GetHeatLevelLabel(HeatLevel level)
    {
        switch (level)
        {
            case HeatLevel.Low: return "LOW";
            case HeatLevel.Warm: return "WARM";
            case HeatLevel.Hot: return "HOT";
            case HeatLevel.Scorching: return "SCORCHING";
            default: return "HOT";
        }
    }

    private GUIStyle GetHeatLevelStyle(HeatLevel level)
    {
        var style = new GUIStyle(GUI.skin.label);
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 14;

        switch (level)
        {
            case HeatLevel.Low: style.normal.textColor = Color.blue; break;
            case HeatLevel.Warm: style.normal.textColor = Color.green; break;
            case HeatLevel.Hot: style.normal.textColor = new Color(1, 0.5f, 0); break; // Orange
            case HeatLevel.Scorching: style.normal.textColor = Color.red; break;
        }

        return style;
    }

    private GUIStyle GetSectionHeaderStyle()
    {
        var style = new GUIStyle(GUI.skin.label);
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 12;
        style.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
        return style;
    }
}