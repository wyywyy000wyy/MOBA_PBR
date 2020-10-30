using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BundleExtraInfoUtils
{
    public static string GetBundleExtraCacheInfoText()
    {
        return GetBundleExtraText(Defines.BundleExtraCacheInfoPath);
    }

    public static string GetBundleExtraText(string path)
    {
        string data = string.Empty;
        //        #if UNITY_STANDALONE
        string extPath = path;
        //        #else
        //        string extPath = System.IO.Path.Combine(Application.persistentDataPath,BundleExtraInfoName + ".txt");
        //        #endif
        if( System.IO.File.Exists(extPath) )
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(extPath);
            data = sr.ReadToEnd();
            sr.Close();

            Debug.LogFormat("...Load BundleExtraInfo in path: {0}...", extPath);
        }
        else
        {
            TextAsset text = Resources.Load<TextAsset>(Defines.BundleExtraInfoName);
            if( text != null )
            {
                data = text.text;
                Debug.Log("...Load BundleExtraInfo in Resources...");
            }
        }

        if( string.IsNullOrEmpty(data) )
        {
            Debug.LogFormat("...BundleExtraInfo not exist...", extPath);
        }
        return data;
    }

    public static List<T> ArrayToList<T>(T[] arr)
    {
        if (arr != null)
        {
            List<T> generated = new List<T>();
            for (int i = 0; i < arr.Length; i++)
            {
                generated.Add(arr[i]);
            }
            return generated;
        }
        return null;
    }

}


[System.Serializable]
public class BundleExtraCacheInfo
{
    public string[] NotCache;
    public string[] CacheInScene;
    public string[] CacheAllTime;

    public string[] NotCompress;
    public string[] CompressLZMA;
    public string[] CompressLZ4;

    // in default, we decide compress type by cache type
    public void ResetCompressToDefault()
    {
        this.NotCompress = this.CacheAllTime;
        this.CompressLZMA = this.NotCache;
        this.CompressLZ4 = this.CacheInScene;
    }
}

[System.Serializable]
public class BundleExtraPackageInfo
{
    public string[] PackgesCategory1;
    public string[] PackgesCategory2;
    public string[] PackgesCategory3;
}
