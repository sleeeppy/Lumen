using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class CommandLineBuild
{
    public static void BuildMac()
    {
        string[] scenes =
        {
            "Assets/Scenes/Main.unity",
            "Assets/Scenes/Lobby.unity",
            "Assets/Scenes/Game.unity",
            "Assets/Scenes/Boss2.unity",
        };

        var options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = "Builds/macOS/Lumen.app",
            target = BuildTarget.StandaloneOSX,
            options = BuildOptions.None,
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
        {
            Debug.LogError($"Mac build failed: {report.summary.result}");
            EditorApplication.Exit(1);
            return;
        }

        Debug.Log($"Mac build succeeded: {options.locationPathName}");
        EditorApplication.Exit(0);
    }
}
