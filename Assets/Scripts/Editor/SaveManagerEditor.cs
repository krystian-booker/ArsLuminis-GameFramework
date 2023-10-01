using Assets.Scripts.Managers;
using UnityEditor;

namespace Assets.Scripts.Editor
{
    public class SaveManagerEditor : EditorWindow
    {
        [MenuItem("Tools/ArsLuminis/Enable Save Debug", false, 1)]
        public static void EnableDebugMode()
        {
            SaveManager.IsDebugMode = true;
        }

        [MenuItem("Tools/ArsLuminis/Enable Save Debug", true)]
        public static bool EnableDebugModeValidate()
        {
            return !SaveManager.IsDebugMode;
        }

        [MenuItem("Tools/ArsLuminis/Disable Save Debug", false, 1)]
        public static void DisableDebugMode()
        {
            SaveManager.IsDebugMode = false;
        }

        [MenuItem("Tools/ArsLuminis/Disable Save Debug", true)]
        public static bool DisableDebugModeValidate()
        {
            return SaveManager.IsDebugMode;
        }
    }
}
