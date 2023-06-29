using MelonLoader;
using System;
using System.Net;
using System.Reflection;
using UnityEngine.XR;

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

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("UnityVRPatcher initializing");
            try {
                InputType = GetInputType();
                GetKeyDownMethod = InputType.GetMethod("GetKeyDown", new[] { typeof(string) });
            } catch (Exception e) {
                MelonLogger.Error($"Error while grabbing Input Methods: {e}");
            }
            try {
                CameraType = GetUnityType("Camera");
                TransformType = GetUnityType("Transform");
                Vector3Type = GetUnityType("Vector3");
                Vector3MultiplyMethod = Vector3Type.GetMethod("op_Multiply", new[] { Vector3Type, typeof(float) });
            } catch (Exception e) {
                MelonLogger.Error($"Error while initializing CameraReparent: {e}");
            }
            MelonLogger.Msg("UnityVRPatcher initialized");
        }

        public override void OnUpdate()
        {
            if (GetKeyDownMethod == null) return;
            if (GetKeyDown("f5")) {
                SetScaleToUser();
            }
            else if (GetKeyDown("f4")) {
                MultiplyCameraScale(ScaleUpMultiplier);
            }
            else if (GetKeyDown("f3")) {
                MultiplyCameraScale(ScaleDownMultiplier);
            }
            else if (GetKeyDown("f2")) {
                ReparentCamera();
            } else if (GetKeyDown("f11")) {
                XRSettings.enabled = !XRSettings.enabled;
                MelonLogger.Msg($"VR has been {(XRSettings.enabled ? "enabled" : "disabled")}");
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
