# Better Heaters Mod for My Winter Car

Control the heating systems in My Winter Car to survive extreme cold weather.

## What This Mod Does
This mod modifies the `HeatingPower` variable in the game's heating systems to allow you to:
- **Heat your garage** to warm up vehicles and equipment (takes time for objects to heat)
- **Heat your house** to keep the player character warm (player warmth is almost instant)
- Control both heating systems independently via a simple GUI

## Important Notes
⚠️ **READ THIS FIRST:**
- **Player warmth**: Heats the player character almost immediately when near heaters
- **Object/Vehicle heating**: Vehicles, tools, and garage objects take several in game hours to warm up
- **Override system**: The game constantly tries to reset HeatingPower values
- **Continuous reapplication**: This mod reapplies your settings every 30 seconds
- **No power bill increase**: The 30-second intervals prevent heating from registering as continuous power usage

## How It Actually Works
- **Player in house**: Gets warm within seconds of heaters being active
- **Cars in garage**: Need several in-game hours of pre-heating to start in extreme cold
- **Game resistance**: The heating systems reset values constantly - this mod overrides them
- **No extra cost**: Doesn't increase your electricity bill
- **Realistic timing**: Don't expect a frozen car to start after 5 minutes - it takes time to warm the space and vehicle

## Installation
1. Install **BepInEx 5** for My Winter Car
2. Download the latest release
3. Extract the `BetterHeaters` folder into `[Game Folder]/BepInEx/plugins/`
4. Launch the game

## How to Use
1. Press **F10** to open the control panel
2. **Enable Garage Heaters** to increase garage heating (for vehicles/tools)
3. **Enable House Heaters** to increase house heating (for player warmth)  
4. Use the slider to set heating level (higher = stronger heating output)
5. **For cars**: Turn on garage heaters several in-game hours before you need to drive
6. **For survival**: Turn on house heaters when you need to warm up or sleep

## Why This Mod?
My Winter Car's survival mechanics make extreme cold deadly. This mod helps by:
- Allowing you to warm up the player character in the house (near instant)
- Making vehicles startable after proper garage pre-heating (takes several in game hours)
- Giving control over a game system that's normally hard-limited
- **No extra costs**: Doesn't increase your power bill

## Realistic Expectations
- **Player warmth**: Almost immediate when near active heaters
- **Vehicle starting**: Requires hours of garage pre-heating in extreme cold
- **Continuous battle**: Fights the game's constant value resets
- **Cost effective**: Won't increase your electricity bill
- **Survival tool**: Prevents freezing, makes winter work possible


## Compatibility
- **Game**: My Winter Car
- **Requires**: BepInEx 5, .NET Framework 4.8
- **Version**: Works with v.260106-02

## Support/Issues
If heating doesn't seem to work:
1. Verify mod shows ACTIVE (green) in GUI
2. Check you're near the heaters (they have range)
3. For vehicles: wait actual in-game hours, not minutes
4. Monitor BepInEx log for any errors

## Credits
- Mod created with assistance from DeepSeek AI
- Mod created with assistance from ChatGPT AI
- For My Winter Car by Amistech Games
- Uses BepInEx framework

## License
MIT License - do whatever you want with it.
