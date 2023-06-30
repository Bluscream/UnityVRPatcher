/*
 * Huge thanks to @Raic
*/

using MelonLoader;
using System;
using System.Net;
using System.Reflection;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

namespace UnityVRPatcher
{
    public partial class Mod : MelonMod
    {
        Type InputType;
        MethodInfo GetKeyDownMethod;

        const string CameraScaleArg = "cameraScale";
        const float ScaleUpMultiplier = 0.9f;
        const float ScaleDownMultiplier = 1.1f;
        Type CameraType;
        Type TransformType;
        Type Vector3Type;
        MethodInfo Vector3MultiplyMethod;

        MelonPreferences_Category cfg_general;
        MelonPreferences_Entry auto_rescale_on_scene_change;
        MelonPreferences_Entry auto_toggle_on_scene_change;


        MelonPreferences_Category cfg_keyboard_shortcuts;
        MelonPreferences_Entry keyboard_toggle_vr;
        MelonPreferences_Entry keyboard_center_vr;
        MelonPreferences_Entry keyboard_reparent_cam;
        MelonPreferences_Entry keyboard_scale_up;
        MelonPreferences_Entry keyboard_scale_down;
        MelonPreferences_Entry keyboard_scale_to_user;
        string keyboard_toggle_vr_key;
        string keyboard_center_vr_key;
        string keyboard_reparent_cam_key;
        string keyboard_scale_up_key;
        string keyboard_scale_down_key;
        string keyboard_scale_to_user_key;

        public override void OnInitializeMelon() {
            MelonLogger.Msg("UnityVRPatcher initializing");
            cfg_general = MelonPreferences.CreateCategory("VRPatcher", "VR Patcher");
            auto_rescale_on_scene_change = cfg_general.CreateEntry("auto_rescale_on_scene_change", false, "Automatically rescale camera when scene changes");
            auto_toggle_on_scene_change = cfg_general.CreateEntry("auto_toggle_on_scene_change", false, "Automatically toggles VR mode depending on which scene is loaded");

            cfg_keyboard_shortcuts = MelonPreferences.CreateCategory("VRPatcherKeys", "VR Patcher Keybinds");
            keyboard_toggle_vr = cfg_keyboard_shortcuts.CreateEntry("key_toggle_vr", "f11", "Key to toggle VR");
            keyboard_center_vr = cfg_keyboard_shortcuts.CreateEntry("key_center_vr", "f12", "Key to center VR");
            keyboard_reparent_cam = cfg_keyboard_shortcuts.CreateEntry("key_reparent_cam", "f2", "Key to reparent camera");
            keyboard_scale_up = cfg_keyboard_shortcuts.CreateEntry("key_scale_up", "f4", "Key to scale up");
            keyboard_scale_down = cfg_keyboard_shortcuts.CreateEntry("key_scale_down", "f3", "Key to scale down");
            keyboard_scale_to_user = cfg_keyboard_shortcuts.CreateEntry("key_scale_to_user", "f5", "Key to scale to user");
            //controller_toggle_vr = cfg_keyboard_shortcuts.CreateEntry("controller_toggle_vr", "joystick button 9", "Controller button to toggle VR");
            OnPreferencesLoaded();
            try {
                InputType = GetInputType();
                GetKeyDownMethod = InputType.GetMethod("GetKeyDown", new[] { typeof(string) });
            } catch (Exception e) {
                MelonLogger.Error($"Error while grabbing Input Methods: {e}");
            }
            try {
                SceneManager.sceneLoaded += OnSceneLoaded;
                CameraType = GetUnityType("Camera");
                TransformType = GetUnityType("Transform");
                Vector3Type = GetUnityType("Vector3");
                Vector3MultiplyMethod = Vector3Type.GetMethod("op_Multiply", new[] { Vector3Type, typeof(float) });
            } catch (Exception e) {
                MelonLogger.Error($"Error while initializing CameraReparent: {e}");
            }
            // MelonPreferences.OnPreferencesSaved.Subscribe(OnPreferencesSaved);
            // MelonPreferences.OnPreferencesLoaded.Subscribe(OnPreferencesSaved);
            MelonLogger.Msg("UnityVRPatcher initialized");
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if ((bool)auto_toggle_on_scene_change.BoxedValue) {
                if (scene.name.Contains("menu")) {
                    MelonLogger.Msg("Menu loaded, disabling VR...");
                    XRSettings.enabled = false;
                } else {
                    MelonLogger.Msg("Game loaded, enabling VR...");
                    XRSettings.enabled = false;
                }
            }
            if ((bool)auto_rescale_on_scene_change.BoxedValue) SetScaleToUser();
        }

        public override void OnPreferencesLoaded() {
            MelonLogger.Msg(" === Keybinds ===");
            keyboard_toggle_vr_key = keyboard_toggle_vr.GetValueAsString();
            MelonLogger.Msg($"Toggle VR: {keyboard_toggle_vr_key}");
            keyboard_center_vr_key = keyboard_center_vr.GetValueAsString();
            MelonLogger.Msg($"Center VR: {keyboard_center_vr_key}");
            keyboard_reparent_cam_key = keyboard_reparent_cam.GetValueAsString();
            MelonLogger.Msg($"Reparent Camera: {keyboard_reparent_cam_key}");
            keyboard_scale_up_key = keyboard_scale_up.GetValueAsString();
            MelonLogger.Msg($"Scale Up: {keyboard_scale_up_key}");
            keyboard_scale_down_key = keyboard_scale_down.GetValueAsString();
            MelonLogger.Msg($"Scale Down: {keyboard_scale_down_key}");
            keyboard_scale_to_user_key = keyboard_scale_to_user.GetValueAsString();
            MelonLogger.Msg($"Scale To User: {keyboard_scale_to_user_key}");
            MelonLogger.Msg("================");
        }

        public override void OnLateUpdate()
        {
            if (GetKeyDownMethod == null) return;
            if (GetKeyDown(keyboard_scale_to_user_key)) {
                SetScaleToUser();
            }
            else if (GetKeyDown(keyboard_scale_up_key)) {
                MultiplyCameraScale(ScaleUpMultiplier);
            }
            else if (GetKeyDown(keyboard_scale_down_key)) {
                MultiplyCameraScale(ScaleDownMultiplier);
            }
            else if (GetKeyDown(keyboard_reparent_cam_key)) {
                ReparentCamera();
            } else if (GetKeyDown(keyboard_toggle_vr_key)) {
                XRSettings.enabled = !XRSettings.enabled;
                MelonLogger.Msg($"VR has been {(XRSettings.enabled ? "enabled" : "disabled")}");
            } else if (GetKeyDown(keyboard_center_vr_key)) {
                InputTracking.Recenter();
                MelonLogger.Msg("VR has been recentered");
            }
        }

        private object MultiplyVector3(object vector3, float multiplier) {
            return Vector3MultiplyMethod.Invoke(null, new object[] { vector3, multiplier });
        }

        private float GetCommandLineArgument(string name) {
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++) {
                if (args[i] == $"-{name}") {
                    return float.Parse(args[i + 1]);
                }
            }

            return 1;
        }

        private Type GetUnityType(string typeName, string moduleName = "CoreModule") {
            return Type.GetType($"UnityEngine.{typeName}, UnityEngine.{moduleName}, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
        }

        private Type GetInputType() {
            return GetUnityType("Input") ?? GetUnityType("Input", "InputLegacyModule");
        }

        private object GetValue(Type type, string propertyName, object instance = null) {
            var property = type.GetProperty(propertyName);
            if (property != null) {
                return property.GetValue(instance, null);
            }
            return type.GetField(propertyName).GetValue(instance);


        }

        private void SetValue(Type type, string propertyName, object value, object instance = null) {
            var property = type.GetProperty(propertyName);
            type.GetProperty(propertyName).SetValue(instance, value, null);
        }

        private bool GetKeyDown(string key) {
            return (bool)GetKeyDownMethod.Invoke(null, new[] { key });
        }
    }
}
