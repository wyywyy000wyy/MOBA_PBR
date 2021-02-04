using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class Main : MonoBehaviour
{
    public static LuaEnv currentLuaEnv { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        GameObject.DontDestroyOnLoad(gameObject);
        XLoader.Initialize(true);
        initLua();
    }

    void initLua()
    {
        currentLuaEnv = new LuaEnv();

        currentLuaEnv.Global.Set<string, bool>("_ANDROID", true);

        currentLuaEnv.AddLoader((ref string fn) =>
        {
            return LuaFilePicker.Load(fn);
        });
        currentLuaEnv.AddBuildin("pb", XLua.LuaDLL.Lua.LoadLuaProfobuf);
        currentLuaEnv.DoString("require '" + Defines.LuaEntryFileName + "'");

    }

    // Update is called once per frame
    void Update()
    {
        if (currentLuaEnv != null)
        {
            currentLuaEnv.Tick();
        }
    }
}
