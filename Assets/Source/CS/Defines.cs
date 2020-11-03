using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defines
{
    public static string BundleExtraCacheInfoPath = "Assets/Editor/Build/BundleExtraInfo.txt";
    public static string BundleExtraInfoName = "BundleExtraInfo";

    public static readonly char ABNameSeparatorChar = '_';
    public static readonly string ABNameNonAutoPrefix = "__";
    public static readonly string AssetBoundleSuffix = "";

    public static readonly string AssetBundleSourcePath = "Assets/BundledRes";


    public static readonly string AssetBundleEntityPath = AssetBundleSourcePath + "/Entity";

    public static readonly string AssetBundleSceneSourcePath = AssetBundleSourcePath + "/Scenes";
    public static readonly string SpritePackerSourceImagePath = AssetBundleSourcePath + "/Images";

    public static readonly string LuaSourcePath = "Assets/Source/Lua";
    public static readonly string LuaConfigPath = "Assets/Source/Lua/Config";
    public static readonly string LuaByteCodeOutPath = "./build/luas";
    public static string LuaByteCodeLoadPath = System.IO.Path.Combine(Application.dataPath, string.Format("StreamingAssets/files/"));
    public static string AssetBundleLoadPath = System.IO.Path.Combine(Application.dataPath, string.Format("StreamingAssets/bundle/"));

    public static readonly string AssetTextureSourcePath = "Assets/RawRes/Textures";


    public static readonly string AssetArtEffectSourcePath = "Assets/RawRes/Effect";
    public static readonly string AssetArtEffectTexSourcePath = AssetTextureSourcePath + "/Effect";
    public static readonly string AssetArtEffectPrefabSourcePath = AssetBundleSourcePath + "/Effects";


    public static readonly string LuaEntryFileName = "main";


    public static readonly string[] LuaFileSearchPath = {
        LuaSourcePath,
        LuaConfigPath,
        "/sdcard/YX_TF2/Lua",
        "/sdcard/YX_TF2/Config",
        Application.persistentDataPath
    };
}
