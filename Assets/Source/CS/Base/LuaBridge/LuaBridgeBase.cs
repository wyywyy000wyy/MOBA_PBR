using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[GCOptimize]
public enum LuaBridgeType
{
    Base,
    Application,
    DragableUI,
    Dragable3D
}

//[GCOptimize]
[LuaCallCSharp]
public enum UnityBehaviourFunc
{
    Start,
    OnDestroy,
    Update,

    OnApplicationFocus,
    OnApplicationPause,
    OnApplicationQuit,

    OnBeginDrag,
    OnDrag,
    OnEndDrag,

    OnAnimatorStateEnter,
    OnAnimatorStateExit,

    Max,
}

[LuaCallCSharp]
public class LuaBridgeBase : MonoBehaviour
{
    protected LuaFunction[] _luaBindFuncs = new LuaFunction[(int)UnityBehaviourFunc.Max];
    protected LuaTable _luaEntity;

    public static Component Bind(LuaBridgeType bridgeType, GameObject go, LuaTable luaTbl, LuaTable LuaDLL)
    {
        LuaBridgeBase luaComp = null;
        switch (bridgeType)
        {
            case LuaBridgeType.Base:
                luaComp = go.AddComponent<LuaBridgeBase>();
                break;
            case LuaBridgeType.Application:
                luaComp = go.AddComponent<LuaBridgeApplication>();
                break;
            case LuaBridgeType.DragableUI:
                luaComp = go.AddComponent<LuaBridgeDragableUI>();
                break;
            case LuaBridgeType.Dragable3D:
                luaComp = go.AddComponent<LuaBridgeDragable3D>();
                break;
        }
        luaComp.Initilize(luaTbl, LuaDLL);
        return luaComp;
    }

    void Initilize(LuaTable luaTbl, LuaTable LuaDLL)
    {
        if (luaTbl == null)
        {
            Debug.LogError("LuaComponent.Initilize: initilize with nil lua table");
            return;
        }
        
        LuaDLL.ForEach<int,LuaFunction>((int func,LuaFunction cb) =>{
            //int func;
            //LuaFunction cb;
            //tb.Get<int,int>(1,out func);
            //tb.Get<int,LuaFunction>(1,out cb);
            _luaBindFuncs[func] = cb;
        });
        _luaEntity = luaTbl;
    }

    protected void CallLuaBindFunc(UnityBehaviourFunc funcIndex)
    {
        LuaFunction func = _luaBindFuncs[(int)funcIndex];
        if (func != null)
            func.Call(_luaEntity);

    }

    protected void CallLuaBindFunc(UnityBehaviourFunc funcIndex, object a1)
    {
        LuaFunction func = _luaBindFuncs[(int)funcIndex];
        if (func != null)
            func.Call(_luaEntity, a1);

    }

    protected void CallLuaBindFunc(UnityBehaviourFunc funcIndex, object a1, object a2)
    {
        LuaFunction func = _luaBindFuncs[(int)funcIndex];
        if (func != null)
            func.Call(_luaEntity, a1, a2);

    }

    void Start()
    {
        CallLuaBindFunc(UnityBehaviourFunc.Start);
    }

    void OnDestroy()
    {
        CallLuaBindFunc(UnityBehaviourFunc.OnDestroy);
    }

    void OnAnimatorStateEnter(AnimatorStateInfo stateInfo)
    {
        CallLuaBindFunc(UnityBehaviourFunc.OnAnimatorStateEnter, stateInfo);
    }

    void OnAnimatorStateExit(AnimatorStateInfo stateInfo)
    {
        CallLuaBindFunc(UnityBehaviourFunc.OnAnimatorStateExit, stateInfo);
    }
}
