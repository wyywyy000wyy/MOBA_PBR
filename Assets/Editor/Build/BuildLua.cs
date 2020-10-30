using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq;

public class BuildLua
{
    public static void ClearLuas()
    {
        if (Directory.Exists(Defines.LuaByteCodeLoadPath))
            Directory.Delete(Defines.LuaByteCodeLoadPath, true);
        AssetDatabase.Refresh();
    }

    private static readonly string tmp_postfix = "_tmp";

    [MenuItem("Build/Build Lua")]
    public static void GenLuaBytes()
    {
        if (Directory.Exists(Defines.LuaByteCodeOutPath))
        {
            Directory.Delete(Defines.LuaByteCodeOutPath, true);
        }
        Directory.CreateDirectory(Defines.LuaByteCodeOutPath);

        string[] scripts = Directory.GetFiles(Defines.LuaSourcePath, "*.lua", SearchOption.AllDirectories);
        string[] configs = Directory.GetFiles(Defines.LuaConfigPath, "*.lua", SearchOption.AllDirectories);

#if UNITY_EDITOR_WIN
        var toolPath = Path.GetFullPath("./Tools/xLua/luac.exe");
#else
        var toolPath = Path.GetFullPath("./Tools/xLua/luac");
#endif      
        List<string> cmdList = new List<string>();
        List<string> checkList = new List<string>();
        Dictionary<string, string> checkMap = new Dictionary<string, string>();
#if UNITY_EDITOR_WIN
#else
        cmdList.Add(string.Format("chmod 777 {0}", toolPath));
#endif
        var fullLuaSourcePath = Path.GetFullPath(Defines.LuaSourcePath).Replace("\\", "/");

        //准备命令
        foreach (var file in scripts)
        {
            var fullFilleName = Path.GetFullPath(file).Replace("\\", "/");
            var subName = fullFilleName.Substring(fullLuaSourcePath.Length + 1, fullFilleName.Length - fullLuaSourcePath.Length - 1);
            var newName = subName.Replace("/", "+");
            newName = GHelper.Md5Sum(Path.ChangeExtension(newName, null));

            var genPath = Path.GetFullPath(Path.Combine(Defines.LuaByteCodeOutPath, newName));// + tmp_postfix));
            var cmdStr = string.Format("{0} -o {1} {2}", toolPath, genPath, subName);

            cmdList.Add(cmdStr);
            //UnityEngine.Debug.Log("build lua... " + cmdStr);

            checkMap.Add(genPath, file);
        }

        UnityEngine.Debug.Log(string.Format("Defines.fullLuaSourcePath={0}", fullLuaSourcePath));
        UnityEngine.Debug.Log(string.Format("Defines.LuaSourcePath={0}", Defines.LuaSourcePath));
        BatchRunShell(cmdList, Defines.LuaSourcePath);

        cmdList.Clear();

        var fullConfigSourcePath = Path.GetFullPath(Defines.LuaConfigPath).Replace("\\", "/");
        foreach (var file in configs)
        {
            var fullFilleName = Path.GetFullPath(file).Replace("\\", "/");
            var subName = fullFilleName.Substring(fullConfigSourcePath.Length + 1, fullFilleName.Length - fullConfigSourcePath.Length - 1);
            var newName = subName.Replace("/", "+");
            newName = GHelper.Md5Sum(Path.ChangeExtension(newName, null));

            var genPath = Path.GetFullPath(Path.Combine(Defines.LuaByteCodeOutPath, newName));// + tmp_postfix));
            cmdList.Add(string.Format("{0} -o {1} {2}", toolPath, genPath, subName));

            checkMap.Add(genPath, file);
        }

        BatchRunShell(cmdList, Defines.LuaConfigPath);

        //检查是否生成成功
        foreach (var pair in checkMap)
        {
            if (!File.Exists(pair.Key))
            {
                UnityEngine.Debug.LogErrorFormat("Gen bytecode faild for:{0}", pair.Value);
                return;
            }
        }

        UnityEngine.Debug.Log("<color=yellow>Gen Lua bytecode Complete!</color>\n");

    }

    private static void BatchRunShell(List<string> cmdList, string workDirectory)
    {
        System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
        psi.UseShellExecute = false;
        psi.WorkingDirectory = Path.GetFullPath(workDirectory);
        while (cmdList.Count > 0)
        {
            var cmd = cmdList[0];
            cmdList.RemoveAt(0);

            List<string> gs = new List<string>();
            gs.AddRange(cmd.Split(' '));
            psi.FileName = gs[0];
            gs.RemoveAt(0);
            psi.Arguments = string.Join(" ", gs);


            System.Diagnostics.Process process = System.Diagnostics.Process.Start(psi);
            process.WaitForExit();

            //string fileName = gs[1];

            //System.IO.FileStream fs = System.IO.File.OpenRead(fileName);
            //int len = (int)fs.Length;
            //byte[] buffer = new byte[len];
            //fs.Read(buffer, 0, len);
            //fs.Close();


            //System.IO.File.WriteAllBytes(fileName.Substring(0, fileName.Length - tmp_postfix.Length), buffer);
            //System.IO.File.Delete(fileName);
        }

    }
}
