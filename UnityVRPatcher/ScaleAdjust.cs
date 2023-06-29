using MelonLoader;

namespace UnityVRPatcher {
    public partial class Mod {

        private void MultiplyCameraScale(float scale) {
            var mainCamera = GetMainCameraTransform();
            var currentScale = GetValue(TransformType, "localScale", mainCamera);
            var multipliedScale = MultiplyVector3(currentScale, scale);
            SetCameraScale(multipliedScale);
        }

        private float GetUserScale() {
            return GetCommandLineArgument(CameraScaleArg);
        }

        private void SetScaleToUser() {
            SetCameraScale(MultiplyVector3(GetValue(Vector3Type, "one"), GetUserScale()));
        }

        private void SetCameraScale(object scale) {
            var mainCamera = GetMainCameraTransform();
            SetValue(TransformType, "localScale", scale, mainCamera);
            MelonLogger.Msg($"Changed camera scale to {GetValue(Vector3Type, "x", scale)}");
        }

        private object GetMainCameraTransform() {
            var mainCamera = GetValue(CameraType, "main");
            return GetValue(CameraType, "transform", mainCamera);
        }
    }
}
