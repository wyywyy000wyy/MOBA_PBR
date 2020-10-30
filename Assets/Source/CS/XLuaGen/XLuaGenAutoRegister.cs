#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using System;
using System.Collections.Generic;
using System.Reflection;


namespace XLua.CSObjectWrap
{
    public class XLua_Gen_Initer_Register__
	{
        
        
        static void wrapInit0(LuaEnv luaenv, ObjectTranslator translator)
        {
        
            translator.DelayWrapLoader(typeof(XLoader), XLoaderWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(CoroutineHelper), CoroutineHelperWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngineObjectExtention), UnityEngineObjectExtentionWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(GHelper), GHelperWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(object), SystemObjectWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Application), UnityEngineApplicationWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Object), UnityEngineObjectWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Texture), UnityEngineTextureWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Canvas), UnityEngineCanvasWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Plane), UnityEnginePlaneWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Screen), UnityEngineScreenWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Vector2), UnityEngineVector2Wrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Vector3), UnityEngineVector3Wrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Vector4), UnityEngineVector4Wrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Quaternion), UnityEngineQuaternionWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Rect), UnityEngineRectWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Color), UnityEngineColorWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Color32), UnityEngineColor32Wrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Ray), UnityEngineRayWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Bounds), UnityEngineBoundsWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Ray2D), UnityEngineRay2DWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Time), UnityEngineTimeWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Material), UnityEngineMaterialWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.GameObject), UnityEngineGameObjectWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Component), UnityEngineComponentWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Camera), UnityEngineCameraWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Behaviour), UnityEngineBehaviourWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Transform), UnityEngineTransformWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.RectTransform), UnityEngineRectTransformWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Resources), UnityEngineResourcesWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.TextAsset), UnityEngineTextAssetWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Animation), UnityEngineAnimationWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Animator), UnityEngineAnimatorWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.AnimatorUpdateMode), UnityEngineAnimatorUpdateModeWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.AnimatorStateInfo), UnityEngineAnimatorStateInfoWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.RuntimeAnimatorController), UnityEngineRuntimeAnimatorControllerWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Sprite), UnityEngineSpriteWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.SpriteRenderer), UnityEngineSpriteRendererWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Keyframe), UnityEngineKeyframeWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.AnimationCurve), UnityEngineAnimationCurveWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.AnimationClip), UnityEngineAnimationClipWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.AnimationState), UnityEngineAnimationStateWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.MonoBehaviour), UnityEngineMonoBehaviourWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Renderer), UnityEngineRendererWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Shader), UnityEngineShaderWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.LayerMask), UnityEngineLayerMaskWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Collider), UnityEngineColliderWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.BoxCollider), UnityEngineBoxColliderWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.FlareLayer), UnityEngineFlareLayerWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.CanvasRenderer), UnityEngineCanvasRendererWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.MeshFilter), UnityEngineMeshFilterWrap.__Register);
        
        }
        
        static void wrapInit1(LuaEnv luaenv, ObjectTranslator translator)
        {
        
            translator.DelayWrapLoader(typeof(UnityEngine.MeshRenderer), UnityEngineMeshRendererWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.LineRenderer), UnityEngineLineRendererWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.SkinnedMeshRenderer), UnityEngineSkinnedMeshRendererWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.PolygonCollider2D), UnityEnginePolygonCollider2DWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.CapsuleCollider), UnityEngineCapsuleColliderWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.RectTransformUtility), UnityEngineRectTransformUtilityWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.QualitySettings), UnityEngineQualitySettingsWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.PlayerPrefs), UnityEnginePlayerPrefsWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.RuntimePlatform), UnityEngineRuntimePlatformWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(System.Collections.Generic.List<int>), SystemCollectionsGenericList_1_SystemInt32_Wrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(System.Collections.Generic.List<string>), SystemCollectionsGenericList_1_SystemString_Wrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(System.Collections.Generic.List<UnityEngine.Vector3>), SystemCollectionsGenericList_1_UnityEngineVector3_Wrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Debug), UnityEngineDebugWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.AudioSource), UnityEngineAudioSourceWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.AudioListener), UnityEngineAudioListenerWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.CanvasGroup), UnityEngineCanvasGroupWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.SystemInfo), UnityEngineSystemInfoWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.RenderTexture), UnityEngineRenderTextureWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(System.Array), SystemArrayWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.ParticleSystem), UnityEngineParticleSystemWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Light), UnityEngineLightWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Mathf), UnityEngineMathfWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.WaitForEndOfFrame), UnityEngineWaitForEndOfFrameWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.WaitForFixedUpdate), UnityEngineWaitForFixedUpdateWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.WaitForSeconds), UnityEngineWaitForSecondsWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.WaitForSecondsRealtime), UnityEngineWaitForSecondsRealtimeWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.UI.GraphicRaycaster), UnityEngineUIGraphicRaycasterWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.UI.Selectable), UnityEngineUISelectableWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.UI.Selectable.Transition), UnityEngineUISelectableTransitionWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.UI.Button), UnityEngineUIButtonWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.UI.Image), UnityEngineUIImageWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.UI.RawImage), UnityEngineUIRawImageWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.UI.Text), UnityEngineUITextWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.UI.InputField), UnityEngineUIInputFieldWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.UI.ScrollRect), UnityEngineUIScrollRectWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.UI.Toggle), UnityEngineUIToggleWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.UI.ToggleGroup), UnityEngineUIToggleGroupWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Events.UnityEventBase), UnityEngineEventsUnityEventBaseWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.WWWForm), UnityEngineWWWFormWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Networking.UnityWebRequest), UnityEngineNetworkingUnityWebRequestWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.UI.Outline), UnityEngineUIOutlineWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.UI.Shadow), UnityEngineUIShadowWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.NetworkReachability), UnityEngineNetworkReachabilityWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.SphereCollider), UnityEngineSphereColliderWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.GUIUtility), UnityEngineGUIUtilityWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Touch), UnityEngineTouchWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.Input), UnityEngineInputWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.TouchPhase), UnityEngineTouchPhaseWrap.__Register);
        
        
            translator.DelayWrapLoader(typeof(UnityEngine.EventSystems.EventSystem), UnityEngineEventSystemsEventSystemWrap.__Register);
        
        
        
        }
        
        static void Init(LuaEnv luaenv, ObjectTranslator translator)
        {
            
            wrapInit0(luaenv, translator);
            
            wrapInit1(luaenv, translator);
            
            
            translator.AddInterfaceBridgeCreator(typeof(System.Collections.IEnumerator), SystemCollectionsIEnumeratorBridge.__Create);
            
        }
        
	    static XLua_Gen_Initer_Register__()
        {
		    XLua.LuaEnv.AddIniter(Init);
		}
		
		
	}
	
}
namespace XLua
{
	public partial class ObjectTranslator
	{
		static XLua.CSObjectWrap.XLua_Gen_Initer_Register__ s_gen_reg_dumb_obj = new XLua.CSObjectWrap.XLua_Gen_Initer_Register__();
		static XLua.CSObjectWrap.XLua_Gen_Initer_Register__ gen_reg_dumb_obj {get{return s_gen_reg_dumb_obj;}}
	}
	
	internal partial class InternalGlobals
    {
	    
	    static InternalGlobals()
		{
		    extensionMethodMap = new Dictionary<Type, IEnumerable<MethodInfo>>()
			{
			    
			};
			
			genTryArrayGetPtr = StaticLuaCallbacks.__tryArrayGet;
            genTryArraySetPtr = StaticLuaCallbacks.__tryArraySet;
		}
	}
}
