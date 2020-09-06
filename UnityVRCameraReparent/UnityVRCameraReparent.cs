using MelonLoader;
using System;
using System.Reflection;

namespace Raicuparta.UnityVRCameraReparent
{
    public class UnityVRCameraReparent : MelonMod
    {
        Type InputType;
        MethodInfo GetKeyDownMethod;

        public override void OnApplicationStart()
        {
            InputType = Type.GetType("UnityEngine.Input, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
            if (InputType == null)
            {
                InputType = Type.GetType("UnityEngine.Input, UnityEngine.InputLegacyModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
            }
            GetKeyDownMethod = InputType.GetMethod("GetKeyDown", new[] { typeof(string) });
        }

        public override void OnUpdate()
        {
            var isKeyDown = (bool)GetKeyDownMethod.Invoke(null, new[] { "f2" });
            if (isKeyDown)
            {
                ReparentCamera();
            }
        }

        private void ReparentCamera()
        {
            MelonLogger.Log("Reparenting camera");

            var cameraType = Type.GetType("UnityEngine.Camera, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
            var mainCamera = cameraType.GetProperty("main").GetValue(null, null);
            cameraType.GetProperty("tag").SetValue(mainCamera, "", null);
            cameraType.GetProperty("enabled").SetValue(mainCamera, false, null);

            var gameObjectType = Type.GetType("UnityEngine.GameObject, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
            var vrCameraObject = Activator.CreateInstance(gameObjectType);
            var addComponentMethod = gameObjectType.GetMethod("AddComponent", new[] { typeof(Type) });
            var vrCamera = addComponentMethod.Invoke(vrCameraObject, new[] { cameraType });
            var mainCameraTransform = cameraType.GetProperty("transform").GetValue(mainCamera, null);
            var vrCameraTransform = cameraType.GetProperty("transform").GetValue(vrCamera, null);
            var transformType = Type.GetType("UnityEngine.Transform, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");

            transformType.GetProperty("parent").SetValue(vrCameraTransform, mainCameraTransform, null);
            cameraType.GetProperty("tag").SetValue(vrCamera, "MainCamera", null);
        }
    }
}
