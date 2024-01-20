// #define USE_BUNDLE_IN_EDITOR // test

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[XLua.LuaCallCSharp]
public class LuaFilePicker
{
    public const string fileFolder = "files";

    public static string dataBasePath = System.IO.Path.Combine(Application.persistentDataPath, fileFolder);
    private static string streamBasePath = System.IO.Path.Combine(Application.streamingAssetsPath, fileFolder).Replace("\\", "/");

    static List<string> fileMap = new List<string>();
    static List<string> fileMapKey = new List<string>();

    public static byte[] LoadFrom(string dir, string fn)
    {
        string filePath = System.IO.Path.Combine(dir, fn + ".lua");
        return LoadFrom(filePath);
    }

    public static byte[] LoadFrom(string filePath)
    {
        if (System.IO.File.Exists(filePath))
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(filePath);
            int len = (int)fs.Length;
            byte[] buffer = new byte[len];
            fs.Read(buffer, 0, len);
            fs.Close();
            return buffer;
        }
        return null;
    }

    public static void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            for (int i = 0; i < fileMap.Count; i++)//loop check all loaded
            {
                string name = fileMap[i];
                string key = fileMapKey[i];

                for (int j = 0; j < Defines.LuaFileSearchPath.Length; j++)//loop check all paths
                {
                    string p = Path.Combine(Defines.LuaFileSearchPath[j], name + ".lua");
                    if (File.Exists(p))//find one maybe it is on new path
                    {
                        FileInfo fi = new FileInfo(p);
                        string lastWrite = fi.LastWriteTime.ToString();
                        lastWrite += p;
                        if (lastWrite != key)//path or time not eaqual
                        {
                            Debug.LogFormat("<color=yellow>[LUA LOADER]  File Update Checked! Load lua from path: {0} | time stamp is {1} </color>", p, lastWrite);
                            Main.currentLuaEnv.DoString(File.ReadAllText(p));
                        }
                        break;
                    }
                }
            }
        }
    }

    public static string GetFilePath(string name)
    {
        string fn = name.Replace(".", "/");
        foreach (var path in Defines.LuaFileSearchPath)
        {
            string p = Path.Combine(path, fn + ".lua");
            if (File.Exists(p))
            {
                return p;
            }
        }
        return "not find path";
    }
    public static long FileWriteTime(string path)
    {
        var sysTime = File.GetLastWriteTime(path);
        return sysTime.Ticks;
    }

    public static bool IsFileExist(string name)
    {
        foreach (var path in Defines.LuaFileSearchPath)
        {
            string p = Path.Combine(path, name);
            if (File.Exists(p))
            {
                return true;
            }
        }
        return false;
    }   

    public static List<string> GetFolderFiles(string folder, bool recursive)
    {
        List<string> files = new List<string>();

        string folderPath = "";

        foreach (var path in Defines.LuaFileSearchPath)
        {
            string p = Path.Combine(path, folder);
            if (Directory.Exists(p))
            {
                folderPath = p;
                break;
            }
        }

        if (Directory.Exists(folderPath))
        {
            DirectoryInfo dir = new DirectoryInfo(folderPath);
            FileInfo[] fis = dir.GetFiles("*.lua", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            foreach (var fi in fis)
            {
                string p = Path.GetRelativePath(folderPath, fi.FullName);
                files.Add(p.Replace(fi.Extension, ""));
            }
        }
        int a = 1;
        a = 2;
        return files;
    }   

    public static byte[] Load(string fn)
    {
#if !RELEASE
        //Debug.LogFormat("LuasLoader {0}", fn);
#endif

        fn = fn.Replace(".", "/");
        string bytefn = fn.Replace("/", "+");
        string hashName = GHelper.Md5Sum(bytefn);

#if !USE_BUNDLE_IN_EDITOR && (UNITY_EDITOR || UNITY_STANDALONE || !PUBLISH || ENABLE_GM)

        //foreach (var path in Defines.LuaFileSearchPath)
        //{
        //    byte[] buffer = LoadFrom(path, fn);
        //    string p = Path.Combine(path, fn + ".lua");
        //    if (buffer != null)
        //    {
        //        FileInfo fi = new FileInfo(p);
        //        string lastWrite = fi.LastWriteTime.ToString();
        //        fileMap.Add(fn);
        //        fileMapKey.Add(lastWrite + p);
        //        return buffer;
        //    }
        //}
        {
            string filePath = GetFilePath(fn);
            byte[] buffer = LoadFrom(filePath);
            if (buffer != null)
            {
                FileInfo fi = new FileInfo(filePath);
                string lastWrite = fi.LastWriteTime.ToString();
                fileMap.Add(fn);
                fileMapKey.Add(lastWrite + filePath);
                return buffer;
            }

        }
#endif
        XManifest.AssetInfo info = XManifest.Instance.Find(hashName);
        if (info != null)
        {
            if (info.location == XManifest.Location.Streaming)
            {
                return ResHelper.LoadBytesFromStreamingAssets(System.IO.Path.Combine(streamBasePath, info.fullName));
            }
            else
            {
                var filePath1 = System.IO.Path.Combine(dataBasePath, info.fullName);
                if (System.IO.File.Exists(filePath1))
                {
                    System.IO.FileStream fs = System.IO.File.OpenRead(filePath1);
                    byte[] buffer = new byte[(int)fs.Length];
                    fs.Read(buffer, 0, (int)fs.Length);
                    fs.Close();

                    return buffer;
                }
            }
        }
        else
        {

        }
        return null;
    }
}
