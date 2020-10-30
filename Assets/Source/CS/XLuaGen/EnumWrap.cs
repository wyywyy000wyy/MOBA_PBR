#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;


namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    
    public class UnityEngineAnimatorUpdateModeWrap
    {
		public static void __Register(RealStatePtr L)
        {
		    ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
		    Utils.BeginObjectRegister(typeof(UnityEngine.AnimatorUpdateMode), L, translator, 0, 0, 0, 0);
			Utils.EndObjectRegister(typeof(UnityEngine.AnimatorUpdateMode), L, translator, null, null, null, null, null);
			
			Utils.BeginClassRegister(typeof(UnityEngine.AnimatorUpdateMode), L, null, 4, 0, 0);

            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "Normal", UnityEngine.AnimatorUpdateMode.Normal);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "AnimatePhysics", UnityEngine.AnimatorUpdateMode.AnimatePhysics);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "UnscaledTime", UnityEngine.AnimatorUpdateMode.UnscaledTime);
            

			Utils.RegisterFunc(L, Utils.CLS_IDX, "__CastFrom", __CastFrom);
            
            Utils.EndClassRegister(typeof(UnityEngine.AnimatorUpdateMode), L, translator);
        }
		
		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CastFrom(RealStatePtr L)
		{
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			LuaTypes lua_type = LuaAPI.lua_type(L, 1);
            if (lua_type == LuaTypes.LUA_TNUMBER)
            {
                translator.PushUnityEngineAnimatorUpdateMode(L, (UnityEngine.AnimatorUpdateMode)LuaAPI.xlua_tointeger(L, 1));
            }
			
            else if(lua_type == LuaTypes.LUA_TSTRING)
            {

			    if (LuaAPI.xlua_is_eq_str(L, 1, "Normal"))
                {
                    translator.PushUnityEngineAnimatorUpdateMode(L, UnityEngine.AnimatorUpdateMode.Normal);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "AnimatePhysics"))
                {
                    translator.PushUnityEngineAnimatorUpdateMode(L, UnityEngine.AnimatorUpdateMode.AnimatePhysics);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "UnscaledTime"))
                {
                    translator.PushUnityEngineAnimatorUpdateMode(L, UnityEngine.AnimatorUpdateMode.UnscaledTime);
                }
				else
                {
                    return LuaAPI.luaL_error(L, "invalid string for UnityEngine.AnimatorUpdateMode!");
                }

            }
			
            else
            {
                return LuaAPI.luaL_error(L, "invalid lua type for UnityEngine.AnimatorUpdateMode! Expect number or string, got + " + lua_type);
            }

            return 1;
		}
	}
    
    public class UnityEngineRuntimePlatformWrap
    {
		public static void __Register(RealStatePtr L)
        {
		    ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
		    Utils.BeginObjectRegister(typeof(UnityEngine.RuntimePlatform), L, translator, 0, 0, 0, 0);
			Utils.EndObjectRegister(typeof(UnityEngine.RuntimePlatform), L, translator, null, null, null, null, null);
			
			Utils.BeginClassRegister(typeof(UnityEngine.RuntimePlatform), L, null, 36, 0, 0);

            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "OSXEditor", UnityEngine.RuntimePlatform.OSXEditor);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "OSXPlayer", UnityEngine.RuntimePlatform.OSXPlayer);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "WindowsPlayer", UnityEngine.RuntimePlatform.WindowsPlayer);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "WindowsEditor", UnityEngine.RuntimePlatform.WindowsEditor);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "IPhonePlayer", UnityEngine.RuntimePlatform.IPhonePlayer);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "Android", UnityEngine.RuntimePlatform.Android);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "LinuxPlayer", UnityEngine.RuntimePlatform.LinuxPlayer);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "LinuxEditor", UnityEngine.RuntimePlatform.LinuxEditor);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "WebGLPlayer", UnityEngine.RuntimePlatform.WebGLPlayer);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "WSAPlayerX86", UnityEngine.RuntimePlatform.WSAPlayerX86);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "WSAPlayerX64", UnityEngine.RuntimePlatform.WSAPlayerX64);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "WSAPlayerARM", UnityEngine.RuntimePlatform.WSAPlayerARM);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "PS4", UnityEngine.RuntimePlatform.PS4);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "XboxOne", UnityEngine.RuntimePlatform.XboxOne);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "tvOS", UnityEngine.RuntimePlatform.tvOS);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "Switch", UnityEngine.RuntimePlatform.Switch);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "Lumin", UnityEngine.RuntimePlatform.Lumin);
            

			Utils.RegisterFunc(L, Utils.CLS_IDX, "__CastFrom", __CastFrom);
            
            Utils.EndClassRegister(typeof(UnityEngine.RuntimePlatform), L, translator);
        }
		
		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CastFrom(RealStatePtr L)
		{
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			LuaTypes lua_type = LuaAPI.lua_type(L, 1);
            if (lua_type == LuaTypes.LUA_TNUMBER)
            {
                translator.PushUnityEngineRuntimePlatform(L, (UnityEngine.RuntimePlatform)LuaAPI.xlua_tointeger(L, 1));
            }
			
            else if(lua_type == LuaTypes.LUA_TSTRING)
            {

			    if (LuaAPI.xlua_is_eq_str(L, 1, "OSXEditor"))
                {
                    translator.PushUnityEngineRuntimePlatform(L, UnityEngine.RuntimePlatform.OSXEditor);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "OSXPlayer"))
                {
                    translator.PushUnityEngineRuntimePlatform(L, UnityEngine.RuntimePlatform.OSXPlayer);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "WindowsPlayer"))
                {
                    translator.PushUnityEngineRuntimePlatform(L, UnityEngine.RuntimePlatform.WindowsPlayer);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "WindowsEditor"))
                {
                    translator.PushUnityEngineRuntimePlatform(L, UnityEngine.RuntimePlatform.WindowsEditor);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "IPhonePlayer"))
                {
                    translator.PushUnityEngineRuntimePlatform(L, UnityEngine.RuntimePlatform.IPhonePlayer);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "Android"))
                {
                    translator.PushUnityEngineRuntimePlatform(L, UnityEngine.RuntimePlatform.Android);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "LinuxPlayer"))
                {
                    translator.PushUnityEngineRuntimePlatform(L, UnityEngine.RuntimePlatform.LinuxPlayer);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "LinuxEditor"))
                {
                    translator.PushUnityEngineRuntimePlatform(L, UnityEngine.RuntimePlatform.LinuxEditor);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "WebGLPlayer"))
                {
                    translator.PushUnityEngineRuntimePlatform(L, UnityEngine.RuntimePlatform.WebGLPlayer);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "WSAPlayerX86"))
                {
                    translator.PushUnityEngineRuntimePlatform(L, UnityEngine.RuntimePlatform.WSAPlayerX86);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "WSAPlayerX64"))
                {
                    translator.PushUnityEngineRuntimePlatform(L, UnityEngine.RuntimePlatform.WSAPlayerX64);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "WSAPlayerARM"))
                {
                    translator.PushUnityEngineRuntimePlatform(L, UnityEngine.RuntimePlatform.WSAPlayerARM);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "PS4"))
                {
                    translator.PushUnityEngineRuntimePlatform(L, UnityEngine.RuntimePlatform.PS4);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "XboxOne"))
                {
                    translator.PushUnityEngineRuntimePlatform(L, UnityEngine.RuntimePlatform.XboxOne);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "tvOS"))
                {
                    translator.PushUnityEngineRuntimePlatform(L, UnityEngine.RuntimePlatform.tvOS);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "Switch"))
                {
                    translator.PushUnityEngineRuntimePlatform(L, UnityEngine.RuntimePlatform.Switch);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "Lumin"))
                {
                    translator.PushUnityEngineRuntimePlatform(L, UnityEngine.RuntimePlatform.Lumin);
                }
				else
                {
                    return LuaAPI.luaL_error(L, "invalid string for UnityEngine.RuntimePlatform!");
                }

            }
			
            else
            {
                return LuaAPI.luaL_error(L, "invalid lua type for UnityEngine.RuntimePlatform! Expect number or string, got + " + lua_type);
            }

            return 1;
		}
	}
    
    public class UnityEngineUISelectableTransitionWrap
    {
		public static void __Register(RealStatePtr L)
        {
		    ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
		    Utils.BeginObjectRegister(typeof(UnityEngine.UI.Selectable.Transition), L, translator, 0, 0, 0, 0);
			Utils.EndObjectRegister(typeof(UnityEngine.UI.Selectable.Transition), L, translator, null, null, null, null, null);
			
			Utils.BeginClassRegister(typeof(UnityEngine.UI.Selectable.Transition), L, null, 5, 0, 0);

            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "None", UnityEngine.UI.Selectable.Transition.None);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "ColorTint", UnityEngine.UI.Selectable.Transition.ColorTint);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "SpriteSwap", UnityEngine.UI.Selectable.Transition.SpriteSwap);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "Animation", UnityEngine.UI.Selectable.Transition.Animation);
            

			Utils.RegisterFunc(L, Utils.CLS_IDX, "__CastFrom", __CastFrom);
            
            Utils.EndClassRegister(typeof(UnityEngine.UI.Selectable.Transition), L, translator);
        }
		
		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CastFrom(RealStatePtr L)
		{
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			LuaTypes lua_type = LuaAPI.lua_type(L, 1);
            if (lua_type == LuaTypes.LUA_TNUMBER)
            {
                translator.PushUnityEngineUISelectableTransition(L, (UnityEngine.UI.Selectable.Transition)LuaAPI.xlua_tointeger(L, 1));
            }
			
            else if(lua_type == LuaTypes.LUA_TSTRING)
            {

			    if (LuaAPI.xlua_is_eq_str(L, 1, "None"))
                {
                    translator.PushUnityEngineUISelectableTransition(L, UnityEngine.UI.Selectable.Transition.None);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "ColorTint"))
                {
                    translator.PushUnityEngineUISelectableTransition(L, UnityEngine.UI.Selectable.Transition.ColorTint);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "SpriteSwap"))
                {
                    translator.PushUnityEngineUISelectableTransition(L, UnityEngine.UI.Selectable.Transition.SpriteSwap);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "Animation"))
                {
                    translator.PushUnityEngineUISelectableTransition(L, UnityEngine.UI.Selectable.Transition.Animation);
                }
				else
                {
                    return LuaAPI.luaL_error(L, "invalid string for UnityEngine.UI.Selectable.Transition!");
                }

            }
			
            else
            {
                return LuaAPI.luaL_error(L, "invalid lua type for UnityEngine.UI.Selectable.Transition! Expect number or string, got + " + lua_type);
            }

            return 1;
		}
	}
    
    public class UnityEngineNetworkReachabilityWrap
    {
		public static void __Register(RealStatePtr L)
        {
		    ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
		    Utils.BeginObjectRegister(typeof(UnityEngine.NetworkReachability), L, translator, 0, 0, 0, 0);
			Utils.EndObjectRegister(typeof(UnityEngine.NetworkReachability), L, translator, null, null, null, null, null);
			
			Utils.BeginClassRegister(typeof(UnityEngine.NetworkReachability), L, null, 4, 0, 0);

            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "NotReachable", UnityEngine.NetworkReachability.NotReachable);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "ReachableViaCarrierDataNetwork", UnityEngine.NetworkReachability.ReachableViaCarrierDataNetwork);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "ReachableViaLocalAreaNetwork", UnityEngine.NetworkReachability.ReachableViaLocalAreaNetwork);
            

			Utils.RegisterFunc(L, Utils.CLS_IDX, "__CastFrom", __CastFrom);
            
            Utils.EndClassRegister(typeof(UnityEngine.NetworkReachability), L, translator);
        }
		
		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CastFrom(RealStatePtr L)
		{
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			LuaTypes lua_type = LuaAPI.lua_type(L, 1);
            if (lua_type == LuaTypes.LUA_TNUMBER)
            {
                translator.PushUnityEngineNetworkReachability(L, (UnityEngine.NetworkReachability)LuaAPI.xlua_tointeger(L, 1));
            }
			
            else if(lua_type == LuaTypes.LUA_TSTRING)
            {

			    if (LuaAPI.xlua_is_eq_str(L, 1, "NotReachable"))
                {
                    translator.PushUnityEngineNetworkReachability(L, UnityEngine.NetworkReachability.NotReachable);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "ReachableViaCarrierDataNetwork"))
                {
                    translator.PushUnityEngineNetworkReachability(L, UnityEngine.NetworkReachability.ReachableViaCarrierDataNetwork);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "ReachableViaLocalAreaNetwork"))
                {
                    translator.PushUnityEngineNetworkReachability(L, UnityEngine.NetworkReachability.ReachableViaLocalAreaNetwork);
                }
				else
                {
                    return LuaAPI.luaL_error(L, "invalid string for UnityEngine.NetworkReachability!");
                }

            }
			
            else
            {
                return LuaAPI.luaL_error(L, "invalid lua type for UnityEngine.NetworkReachability! Expect number or string, got + " + lua_type);
            }

            return 1;
		}
	}
    
    public class UnityEngineTouchPhaseWrap
    {
		public static void __Register(RealStatePtr L)
        {
		    ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
		    Utils.BeginObjectRegister(typeof(UnityEngine.TouchPhase), L, translator, 0, 0, 0, 0);
			Utils.EndObjectRegister(typeof(UnityEngine.TouchPhase), L, translator, null, null, null, null, null);
			
			Utils.BeginClassRegister(typeof(UnityEngine.TouchPhase), L, null, 6, 0, 0);

            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "Began", UnityEngine.TouchPhase.Began);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "Moved", UnityEngine.TouchPhase.Moved);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "Stationary", UnityEngine.TouchPhase.Stationary);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "Ended", UnityEngine.TouchPhase.Ended);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "Canceled", UnityEngine.TouchPhase.Canceled);
            

			Utils.RegisterFunc(L, Utils.CLS_IDX, "__CastFrom", __CastFrom);
            
            Utils.EndClassRegister(typeof(UnityEngine.TouchPhase), L, translator);
        }
		
		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CastFrom(RealStatePtr L)
		{
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			LuaTypes lua_type = LuaAPI.lua_type(L, 1);
            if (lua_type == LuaTypes.LUA_TNUMBER)
            {
                translator.PushUnityEngineTouchPhase(L, (UnityEngine.TouchPhase)LuaAPI.xlua_tointeger(L, 1));
            }
			
            else if(lua_type == LuaTypes.LUA_TSTRING)
            {

			    if (LuaAPI.xlua_is_eq_str(L, 1, "Began"))
                {
                    translator.PushUnityEngineTouchPhase(L, UnityEngine.TouchPhase.Began);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "Moved"))
                {
                    translator.PushUnityEngineTouchPhase(L, UnityEngine.TouchPhase.Moved);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "Stationary"))
                {
                    translator.PushUnityEngineTouchPhase(L, UnityEngine.TouchPhase.Stationary);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "Ended"))
                {
                    translator.PushUnityEngineTouchPhase(L, UnityEngine.TouchPhase.Ended);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "Canceled"))
                {
                    translator.PushUnityEngineTouchPhase(L, UnityEngine.TouchPhase.Canceled);
                }
				else
                {
                    return LuaAPI.luaL_error(L, "invalid string for UnityEngine.TouchPhase!");
                }

            }
			
            else
            {
                return LuaAPI.luaL_error(L, "invalid lua type for UnityEngine.TouchPhase! Expect number or string, got + " + lua_type);
            }

            return 1;
		}
	}
    
}