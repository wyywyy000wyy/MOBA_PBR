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
    public class GHelperWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(GHelper);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 33, 3, 1);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "HashString", _m_HashString_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Md5Sum", _m_Md5Sum_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "CrcHash", _m_CrcHash_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ComputeFileChecksum", _m_ComputeFileChecksum_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "TimeStampToDateTime", _m_TimeStampToDateTime_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "DateTimeToTimeStamp", _m_DateTimeToTimeStamp_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "iclock", _m_iclock_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetNaturalDaysCount", _m_GetNaturalDaysCount_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetHoursCount", _m_GetHoursCount_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SyncTimeFromServer", _m_SyncTimeFromServer_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "NoCacheUrl", _m_NoCacheUrl_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "RayCastUI", _m_RayCastUI_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetStringLength", _m_GetStringLength_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetStringCut", _m_GetStringCut_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "EncodeNonAsciiCharacters", _m_EncodeNonAsciiCharacters_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "FilterEmojiString", _m_FilterEmojiString_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "StringHasEmoji", _m_StringHasEmoji_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetLastTouchPostion", _m_GetLastTouchPostion_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "CreateDirectory", _m_CreateDirectory_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "EncodeXY", _m_EncodeXY_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ListFile", _m_ListFile_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetPersistentDataPath", _m_GetPersistentDataPath_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "TraveEnumToLuaTable", _m_TraveEnumToLuaTable_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "EncryptText", _m_EncryptText_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "DecryptText", _m_DecryptText_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GameVersion", _m_GameVersion_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetVersionCode", _m_GetVersionCode_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetPlatformName", _m_GetPlatformName_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "DecryptTextSafely", _m_DecryptTextSafely_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetRTPixels", _m_GetRTPixels_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SaveRenderTexureToJPG", _m_SaveRenderTexureToJPG_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SaveTexure2DToJPG", _m_SaveTexure2DToJPG_xlua_st_);
            
			
            
			Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "ServerTime", _g_get_ServerTime);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "ServerDateTime", _g_get_ServerDateTime);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "serverTimeOffset", _g_get_serverTimeOffset);
            
			Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "serverTimeOffset", _s_set_serverTimeOffset);
            
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "GHelper does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_HashString_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _str = LuaAPI.lua_tostring(L, 1);
                    
                        int gen_ret = GHelper.HashString( _str );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Md5Sum_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _input = LuaAPI.lua_tostring(L, 1);
                    
                        string gen_ret = GHelper.Md5Sum( _input );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CrcHash_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _input = LuaAPI.lua_tostring(L, 1);
                    
                        uint gen_ret = GHelper.CrcHash( _input );
                        LuaAPI.xlua_pushuint(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ComputeFileChecksum_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _path = LuaAPI.lua_tostring(L, 1);
                    
                        uint gen_ret = GHelper.ComputeFileChecksum( _path );
                        LuaAPI.xlua_pushuint(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_TimeStampToDateTime_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    int _timeStamp = LuaAPI.xlua_tointeger(L, 1);
                    
                        System.DateTime gen_ret = GHelper.TimeStampToDateTime( _timeStamp );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_DateTimeToTimeStamp_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    System.DateTime _dateTime;translator.Get(L, 1, out _dateTime);
                    
                        int gen_ret = GHelper.DateTimeToTimeStamp( _dateTime );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_iclock_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                        uint gen_ret = GHelper.iclock(  );
                        LuaAPI.xlua_pushuint(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetNaturalDaysCount_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    System.DateTime _dtStart;translator.Get(L, 1, out _dtStart);
                    System.DateTime _dtEnd;translator.Get(L, 2, out _dtEnd);
                    
                        int gen_ret = GHelper.GetNaturalDaysCount( _dtStart, _dtEnd );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetHoursCount_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    System.DateTime _dtStart;translator.Get(L, 1, out _dtStart);
                    System.DateTime _dtEnd;translator.Get(L, 2, out _dtEnd);
                    
                        int gen_ret = GHelper.GetHoursCount( _dtStart, _dtEnd );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SyncTimeFromServer_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _serverTime = LuaAPI.xlua_tointeger(L, 1);
                    
                    GHelper.SyncTimeFromServer( _serverTime );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_NoCacheUrl_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _url = LuaAPI.lua_tostring(L, 1);
                    
                        string gen_ret = GHelper.NoCacheUrl( _url );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RayCastUI_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    float _x = (float)LuaAPI.lua_tonumber(L, 1);
                    float _y = (float)LuaAPI.lua_tonumber(L, 2);
                    
                        bool gen_ret = GHelper.RayCastUI( _x, _y );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetStringLength_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _str = LuaAPI.lua_tostring(L, 1);
                    
                        int gen_ret = GHelper.GetStringLength( _str );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetStringCut_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _str = LuaAPI.lua_tostring(L, 1);
                    int _length = LuaAPI.xlua_tointeger(L, 2);
                    
                        string gen_ret = GHelper.GetStringCut( _str, _length );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_EncodeNonAsciiCharacters_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _value = LuaAPI.lua_tostring(L, 1);
                    
                        string gen_ret = GHelper.EncodeNonAsciiCharacters( _value );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_FilterEmojiString_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _value = LuaAPI.lua_tostring(L, 1);
                    
                        string gen_ret = GHelper.FilterEmojiString( _value );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_StringHasEmoji_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _value = LuaAPI.lua_tostring(L, 1);
                    
                        bool gen_ret = GHelper.StringHasEmoji( _value );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetLastTouchPostion_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    
                        UnityEngine.Vector3 gen_ret = GHelper.GetLastTouchPostion(  );
                        translator.PushUnityEngineVector3(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CreateDirectory_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _path = LuaAPI.lua_tostring(L, 1);
                    
                    GHelper.CreateDirectory( _path );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_EncodeXY_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _j = LuaAPI.lua_tostring(L, 1);
                    string _s = LuaAPI.lua_tostring(L, 2);
                    
                        string gen_ret = GHelper.EncodeXY( _j, _s );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ListFile_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _fullPath = LuaAPI.lua_tostring(L, 1);
                    
                        string[] gen_ret = GHelper.ListFile( _fullPath );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetPersistentDataPath_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                        string gen_ret = GHelper.GetPersistentDataPath(  );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_TraveEnumToLuaTable_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    System.Type _eType = (System.Type)translator.GetObject(L, 1, typeof(System.Type));
                    XLua.LuaTable _tb = (XLua.LuaTable)translator.GetObject(L, 2, typeof(XLua.LuaTable));
                    
                        XLua.LuaTable gen_ret = GHelper.TraveEnumToLuaTable( _eType, _tb );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_EncryptText_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _data = LuaAPI.lua_tostring(L, 1);
                    string _cryptKey = LuaAPI.lua_tostring(L, 2);
                    
                        string gen_ret = GHelper.EncryptText( _data, _cryptKey );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_DecryptText_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _data = LuaAPI.lua_tostring(L, 1);
                    string _cryptKey = LuaAPI.lua_tostring(L, 2);
                    
                        string gen_ret = GHelper.DecryptText( _data, _cryptKey );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GameVersion_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                        string gen_ret = GHelper.GameVersion(  );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetVersionCode_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                        int gen_ret = GHelper.GetVersionCode(  );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetPlatformName_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                        string gen_ret = GHelper.GetPlatformName(  );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_DecryptTextSafely_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _data = LuaAPI.lua_tostring(L, 1);
                    string _cryptKey = LuaAPI.lua_tostring(L, 2);
                    
                        string gen_ret = GHelper.DecryptTextSafely( _data, _cryptKey );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetRTPixels_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.RenderTexture _texure = (UnityEngine.RenderTexture)translator.GetObject(L, 1, typeof(UnityEngine.RenderTexture));
                    
                        UnityEngine.Texture2D gen_ret = GHelper.GetRTPixels( _texure );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SaveRenderTexureToJPG_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.RenderTexture _texture = (UnityEngine.RenderTexture)translator.GetObject(L, 1, typeof(UnityEngine.RenderTexture));
                    string _path = LuaAPI.lua_tostring(L, 2);
                    
                    GHelper.SaveRenderTexureToJPG( _texture, _path );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SaveTexure2DToJPG_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.Texture2D _texture2D = (UnityEngine.Texture2D)translator.GetObject(L, 1, typeof(UnityEngine.Texture2D));
                    string _path = LuaAPI.lua_tostring(L, 2);
                    
                    GHelper.SaveTexure2DToJPG( _texture2D, _path );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_ServerTime(RealStatePtr L)
        {
		    try {
            
			    LuaAPI.xlua_pushinteger(L, GHelper.ServerTime);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_ServerDateTime(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    translator.Push(L, GHelper.ServerDateTime);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_serverTimeOffset(RealStatePtr L)
        {
		    try {
            
			    LuaAPI.xlua_pushinteger(L, GHelper.serverTimeOffset);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_serverTimeOffset(RealStatePtr L)
        {
		    try {
                
			    GHelper.serverTimeOffset = LuaAPI.xlua_tointeger(L, 1);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
