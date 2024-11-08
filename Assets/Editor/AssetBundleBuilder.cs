using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using Unity.Android.Gradle;

public class AssetBundleBuilder
{
    [MenuItem("Assets/Build AssetBundles")]
    private static void BuildAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles(
            "Assets/AssetBundles",
            BuildAssetBundleOptions.None,
            BuildTarget.StandaloneWindows
        );

        UpdateVersionJson();
        CommitAndPushToGit();
    }


    private static void UpdateVersionJson()
    {
        string versionFilePath = Path.Combine(Application.streamingAssetsPath, "version.json");
        if (File.Exists(versionFilePath))
        {
            string json = File.ReadAllText(versionFilePath);
            VersionConfig versionData = JsonUtility.FromJson<VersionConfig>(json);
            foreach (var bundle in versionData.bundles)
            {
                //version format is 1.0
                bundle.version = (float.Parse(bundle.version) + 0.1f).ToString();
            }
            File.WriteAllText(versionFilePath, JsonUtility.ToJson(versionData));
        }
    }


    private static void CommitAndPushToGit()
    {
        RunGitCommand("git add Assets/*");
        RunGitCommand("git commit -m 'Auto commit from Unity Asset Bundle Builder'");
        RunGitCommand("git push origin main");
    }

    private static void RunGitCommand(string command)
    {
        ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
        processStartInfo.WorkingDirectory = Application.dataPath;
        processStartInfo.RedirectStandardOutput = true;
        processStartInfo.UseShellExecute = false;
        processStartInfo.CreateNoWindow = false;

        using (Process process = Process.Start(processStartInfo))
        {
            process.WaitForExit();
            UnityEngine.Debug.Log(process.StandardOutput.ReadToEnd());
        }
    }
}
