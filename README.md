# Unity VR Patcher Mod for MelonLoader

| :warning: Deprecation Notice |
|:---------------------------|
```diff
- This mod has been superseeded by https://github.com/Raicuparta/uuvr
```

- A universal tweaker mod for Unity games that have been [patched with VR](https://www.notion.so/beastsaber/How-To-Force-Any-Unity-Game-to-Run-In-Native-VR-Mode-cf8c50f66f2740d5b692db786a8386a1) after the fact. 
- This mod is continuation of  @Raicuparta's great [unity-vr-camera-reparent](https://github.com/Raicuparta/unity-vr-camera-reparent) and [unity-scale-adjuster](https://github.com/Raicuparta/unity-scale-adjuster) Mods.
- It has modifyable keybinds and a new VR toggle so you can actually control the UI while ingame lol.

## VR Modding Tutorial

1. Patch your Game using https://www.notion.so/beastsaber/How-To-Force-Any-Unity-Game-to-Run-In-Native-VR-Mode-cf8c50f66f2740d5b692db786a8386a1 (Use the Manual Process)
2. Install MelonLoader to your game following https://melonwiki.xyz/#/?id=requirements
3. Install https://github.com/Bluscream/UnityVRPatcher/releases to your game's Mods folder.
4. Disable Steam Theater Mode for your game: https://steamcommunity.com/app/250820/discussions/0/1692669912408365970/
5. Add `--melonloader.disablestartscreen` to your game's command line arguments in steam: https://www.howtogeek.com/825209/add-command-line-arguments-to-steam-gog-epic-games-store/#autotoc_anchor_1
6. Start SteamVR with your HMD plugged in
7. Start your game
8. You can toggle VR with the default hotkey `F11`, so you can navigate the menus.
9. Ingame just toggle VR again and enjoy 🙃 🌪️
    
Note: If you need to edit hotkeys, you can find them in `<Game Folder>\UserData\MelonPreferences.cfg`

## Installing the mod

1. [Install MelonLoader by following the instructions on their page](https://melonwiki.xyz/#/?id=requirements);
2. [Download the .dll of the latest release of this mod](https://github.com/Bluscream/UnityVRPatcher/releases/latest);
3. Install the mod as any other MelonLoader mod.
<hr>

### Reparenting the camera

1. Start the game until you see the broken VR camera;
2. Press **F2** to reparent the camera.

### Changing the world scale in-game

1. Start the game until you're in-game, seeing the world that you want to scale;
2. Use keyboard keys **F4** and **F3** to scale the world up and down respectively.

### Saving your preferred scale

1. Notice that when you press **F3** and **F4** to change the world scale, a message shows in MelonLoader logs window saying `[UnityScaleAdjuster] Change camera scale to XXX`, where `XXX` is a number like `0.9`, `1.542`, etc.
2. Change the world scale to something you're satisfied with;
3. Copy the last scale number that was logged;
4. Add that number to the game launch parameter `-cameraScale`. Two ways to do this:
   * If you have the game on Steam:
      * Right click the game on your library;
      * Select "Properties";
      * In the "General" tab, select "Set Launch Options...";
      * In the text field add `-cameraScale XXX`, where `XXX` is the scale value from before.
   * If you don't have the game on Steam:
      * Create a shortcut to the game's exe (or use an existing shortcut);
      * Right click the shortcut;
      * Select "Properties";
      * Add `-cameraScale XXX` after the path in the "Target" field, where `XXX` is the scale value from before;
      * Example target value: `"C:\Program Files (x86)\Steam\steamapps\common\Overload\Overload.exe" -cameraScale 1.2`
5. The mod will try to set your preferred scale automatically whenever a new level / scene loads;
6. If this doesn't work or if the game resets the scale, press **F5** to manually set the scale to value in `-cameraScale` again.

## Default config:
```ini
[VRPatcher]
auto_rescale_on_scene_change = false
auto_toggle_on_scene_change = false

[VRPatcherKeys]
key_toggle_vr = "f11"
key_center_vr = "f12"
key_reparent_cam = "f2"
key_scale_up = "f4"
key_scale_down = "f3"
key_scale_to_user = "f5"
```
