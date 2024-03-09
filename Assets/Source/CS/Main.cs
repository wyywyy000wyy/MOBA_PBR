using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using XLua;

public class Main : MonoBehaviour
{
    public static LuaEnv currentLuaEnv { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        GameObject.DontDestroyOnLoad(gameObject);
        //XLoader.Initialize(true);
        initLua();
    }
#if (UNITY_IPHONE || UNITY_TVOS || UNITY_WEBGL || UNITY_SWITCH) && !UNITY_EDITOR
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport("xlua", CallingConvention = CallingConvention.Cdecl)]
#endif
    static extern int luaopen_cmsgpack_safe(System.IntPtr L);

    void initLua()
    {
        currentLuaEnv = new LuaEnv();

        currentLuaEnv.Global.Set<string, bool>("_ANDROID", true);

        currentLuaEnv.AddLoader((ref string fn) =>
        {
            return LuaFilePicker.Load(fn);
        });
        currentLuaEnv.AddBuildin("pb", XLua.LuaDLL.Lua.LoadLuaProfobuf);
        luaopen_cmsgpack_safe(currentLuaEnv.L);
        currentLuaEnv.DoString("require '" + Defines.LuaEntryFileName + "'");
    }

#if UNITY_EDITOR
    [Shortcut("HotReload", KeyCode.F5)]
#endif
    public static void HotReload()
    {
        if (currentLuaEnv != null)
        {
            var rrquire_all = currentLuaEnv.Global.GetInPath<LuaFunction>("hot_require.rrquire_all");
            rrquire_all.Call();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentLuaEnv != null)
        {
            currentLuaEnv.Tick();
            currentLuaEnv.Global.Get<LuaFunction>("update").Call();
        }
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetKeyUp(KeyCode.F5))
        {
            HotReload();
        }
#endif
    }
}
