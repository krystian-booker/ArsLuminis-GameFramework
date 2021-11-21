using UnityEditor;
using UnityEditor.Compilation;

public class CustomIntegrations : Editor
{
    #if EASY_CHARACTER_MOVEMENT
    [MenuItem("Tools/ExAmore/Third Party Integrations/Disable 'Easy Charater Movement'")]
    public static void DisableEasyCharacterMovement()
    {
        var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
        if (symbols.Contains("EASY_CHARACTER_MOVEMENT"))
        {
            symbols = symbols.Replace(";EASY_CHARACTER_MOVEMENT", "");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, symbols);
        }
    }
    #else
    [MenuItem("Tools/ExAmore/Third Party Integrations/Enable 'Easy Character Movement'")]
    public static void EnableEasyCharacterMovement()
    {
        var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
        if (!symbols.Contains("EASY_CHARACTER_MOVEMENT"))
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, symbols + ";EASY_CHARACTER_MOVEMENT");
        }
    }
    #endif
}
