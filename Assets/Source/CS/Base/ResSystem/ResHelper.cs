using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;

public static class ResHelper
{
    public static string LoadTextFromStreamingAssets(string filePath)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(filePath);
        www.SendWebRequest();
        while (true)
        {
            if (www.isDone || !string.IsNullOrEmpty(www.error))
            {
                if (string.IsNullOrEmpty(www.error))
                {
                    return www.downloadHandler.text;
                }
                return null;
            }
        }
#else
        if (System.IO.File.Exists(filePath))
        {
            return System.IO.File.ReadAllText(filePath);
        }
#endif
        return null;
    }

    public static byte[] LoadBytesFromStreamingAssets(string filePath)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(filePath);
        www.SendWebRequest();
        while (true)
        {
            if (www.isDone || !string.IsNullOrEmpty(www.error))
            {
                if (string.IsNullOrEmpty(www.error))
                {
                    return www.downloadHandler.data;
                }
                return null;
            }
        }
#else
        if (System.IO.File.Exists(filePath))
        {
            return System.IO.File.ReadAllBytes(filePath);
        }
#endif
        return null;
    }

}
