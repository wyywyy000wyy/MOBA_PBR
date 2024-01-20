using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class SpritePostprocessor : AssetPostprocessor
{
    enum TextureQuaility
    {
        Low,
        Hight,
    }

    [MenuItem("Tools/Reset Sprite Compress")]
    public static void ResetSpriteCompress()
    {
        var files = Directory.GetFiles(Defines.SpritePackerSourceImagePath, "*.*", SearchOption.AllDirectories);
        HashSet<string> allSprite = new HashSet<string>();
        int count = 0;
        int total = 0;
        foreach (var name in files)
        {
            if (name.EndsWith(".jpg") || name.EndsWith(".png"))
            {
                total++;
            }
        }
        foreach (var name in files)
        {
            if (name.EndsWith(".jpg") || name.EndsWith(".png"))
            {
                TextureImporter textureImporter = AssetImporter.GetAtPath(name) as TextureImporter;
                if (textureImporter == null)
                    continue;
                var tp = Path.GetDirectoryName(name);
                Deal(textureImporter, name);
                count++;
                textureImporter.SaveAndReimport();
                EditorUtility.DisplayProgressBar("ResetSpriteCompress", "compute sprite", count / (float)total);
            }
        }

        EditorUtility.ClearProgressBar();

        Debug.LogFormat("<color=yellow>[ResetSpriteCompress]Done with {0} files</color>", count);
    }


    private static TextureQuaility _GetTextureQuaility(string path)
    {
        //if (
        //    path.StartsWith(Defines.SpritePackerSourceImagePath
        //    //path.StartsWith(Defines.SpritePackerSourceImagePath + "/skill/") ||
        //    //path.StartsWith(Defines.SpritePackerSourceImagePath + "/card/") ||
        //    //path.StartsWith(Defines.SpritePackerSourceImagePath + "/tech/") ||
        //    //path.StartsWith(Defines.SpritePackerSourceImagePath + "/task/") ||
        //    //path.StartsWith(Defines.SpritePackerSourceImagePath + "/enemy/") ||
        //    //path.StartsWith(Defines.SpritePackerSourceImagePath + "/item/") ||
        //    //path.StartsWith(Defines.SpritePackerSourceImagePath + "/func/") ||
        //    // path.StartsWith(Defines.SpritePackerSourceImagePath + "/avatar/"
        //     )
        //)
        //{
        //    return TextureQuaility.Hight;
        //}
        //return TextureQuaility.Low;

        if (path.StartsWith("Assets/ArtRes/Effect/Spine/first_splash") 
            || path.StartsWith("Assets/Resources") 
            //|| path.StartsWith(Defines.SpritePackerSourceImagePath + "/common")
            //|| path.StartsWith(Defines.SpritePackerSourceImagePath + "/common_new")
            //|| path.StartsWith(Defines.SpritePackerSourceImagePath + "/common_new_2")
            //|| path.StartsWith(Defines.SpritePackerSourceImagePath + "/main")
            ////path.StartsWith(Defines.SpriteAltasSourceImagePath) || 
            //path.StartsWith(Defines.SpritePackerSourceImagePath) 
            || path.EndsWith("_VertData.png") )
        {
            return TextureQuaility.Hight;
        }

        return TextureQuaility.Hight;
    }

    public static void Deal(TextureImporter textureImporter, string assetPath)
    {
        var quality = _GetTextureQuaility(assetPath);
        textureImporter.mipmapEnabled = false;

        TextureImporterPlatformSettings androidTexture = textureImporter.GetPlatformTextureSettings("Android");// new TextureImporterPlatformSettings();
        TextureImporterPlatformSettings iosTexture = textureImporter.GetPlatformTextureSettings("iPhone");
        androidTexture.overridden = true;
        androidTexture.name = "Android";
        androidTexture.allowsAlphaSplitting = false;


        
        iosTexture.overridden = true;
        iosTexture.name = "Ios";
        iosTexture.allowsAlphaSplitting = false;

        textureImporter.textureCompression = TextureImporterCompression.Compressed;
        if (assetPath.StartsWith(Defines.SpritePackerSourceImagePath))
        {
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.maxTextureSize = 1024;
            if (!string.IsNullOrEmpty(textureImporter.spritePackingTag))
            {
                androidTexture.maxTextureSize = 1024;
                iosTexture.maxTextureSize = 1024;
            }
        }

        if (quality == TextureQuaility.Low)
        {
            androidTexture.format = TextureImporterFormat.ASTC_4x4;
            iosTexture.format = TextureImporterFormat.ASTC_4x4;
        }
        else
        {
            androidTexture.format = TextureImporterFormat.RGBA32;
            iosTexture.format = TextureImporterFormat.RGBA32;
           
        }

        textureImporter.SetPlatformTextureSettings(androidTexture);
        textureImporter.SetPlatformTextureSettings(iosTexture);
    }

    public void OnPreprocessTexture()
    {
        Deal((TextureImporter)assetImporter, assetPath);
    }
}
