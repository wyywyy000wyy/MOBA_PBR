using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Build.Reporting;

public class BuildProject : Editor
{

    public static readonly string AndroidBundleName = "com.YiShaTech.MOBA_PBR";
    public static readonly string AndroidAppName = "MOBA_PBR";
    public static readonly int AndroidVendorId = 1;
    public static readonly int Version = 1000000;


    [MenuItem("Build/Build Android")]
    public static void BuildAndroid()
    {
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        EditorUserBuildSettings.development = true;
        EditorUserBuildSettings.connectProfiler = true;

        string targetDir = Application.dataPath.Replace("/Assets", "") + "/build/";
        string targetName = targetDir + AndroidAppName + ".apk";
        if (Directory.Exists(targetDir))
        {
            if (File.Exists(targetName))
            {
                File.Delete(targetName);
            }
        }
        else
        {
            Directory.CreateDirectory(targetDir);
        }

        
        GameConfig.version.SetVersionCode(Version);
        WriteVersion(Version, AndroidBundleName, AndroidVendorId);

        PlayerSettings.applicationIdentifier = AndroidBundleName;
        PlayerSettings.bundleVersion = GameConfig.version.GetVersion();
        PlayerSettings.Android.bundleVersionCode = GameConfig.version.GetVersionCode();
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;

        PlayerSettings.keystorePass = "63nd4PEd";
        PlayerSettings.keyaliasPass = "63nd4PEd";

        //string defineSymbols = "";
        //ConditionBuildDefineSymbols.Instance().Prepare(BuildTargetGroup.Android, defineSymbols);
        //ConditionBuildDefineSymbols.Instance().Apply();

        //AssetDatabase.Refresh();

        BuildTargetGroup build_group = BuildTargetGroup.Android;
        BuildTarget buildTarget = BuildTarget.Android;
        EditorUserBuildSettings.SwitchActiveBuildTarget(build_group, buildTarget);

        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(build_group);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(build_group, defines);

        AssetDatabase.SaveAssets();

        BuildReport report = BuildPipeline.BuildPlayer(buildList, targetName, buildTarget, BuildOptions.None);

        //ConditionBuildDefineSymbols.Instance().Clear();

    }

    static string[] buildList = {
            "Assets/Main.unity",
        };


    public static string[] FindEnabledEditorScenes()
    {
        return buildList;// EditorScenes.ToArray();
    }

    static void WriteVersion(int version, string id, int vendor)
    {
        StreamWriter saveFile = File.CreateText(Application.dataPath + "/Source/CS/Version.cs");
        int now = GHelper.DateTimeToTimeStamp(System.DateTime.Now);
        saveFile.Write(
        @"
		public static class GameVersion {
			public static int code = " + version.ToString() + @";
			public static string identifier = " + "\"" + id + "\"" + @";
			public static int vendor = " + vendor + @";
            public static int buildTime = " + now + @";
		}");
        saveFile.Close();
    }
}
