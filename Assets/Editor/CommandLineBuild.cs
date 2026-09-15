using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class CommandLineBuild
{
    private static readonly string[] Scenes =
    {
        "Assets/Scenes/Main.unity",
        "Assets/Scenes/Lobby.unity",
        "Assets/Scenes/Game.unity",
        "Assets/Scenes/Boss2.unity",
    };

    public static void BuildMac()
        => Build("Builds/macOS/Lumen.app", BuildTarget.StandaloneOSX, "Mac");

    public static void BuildWindows()
        => Build("Builds/windows/Lumen.exe", BuildTarget.StandaloneWindows64, "Windows");

    private static void Build(string locationPathName, BuildTarget target, string label)
    {
        var options = new BuildPlayerOptions
        {
            scenes = Scenes,
            locationPathName = locationPathName,
            target = target,
            options = BuildOptions.None,
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
        {
            Debug.LogError($"{label} build failed: {report.summary.result}");
            EditorApplication.Exit(1);
            return;
        }

        Debug.Log($"{label} build succeeded: {options.locationPathName}");
        EditorApplication.Exit(0);
    }
}
