---@type E
local E = {}
local CS = CS
---------- Custom
E.GHelper = CS.GHelper


-- Json = LuaJson

---------- UnityEngine
local UnityEngine = CS.UnityEngine
E.Application = UnityEngine.Application
E.UIContainer = CS.UIContainer
E.Debug = UnityEngine.Debug
E.GameObject = UnityEngine.GameObject
E.SpriteRenderer = UnityEngine.SpriteRenderer
E.Texture = UnityEngine.Texture
E.Resources = UnityEngine.Resources
E.Time = UnityEngine.Time
E.Mathf = UnityEngine.Mathf
E.Camera = UnityEngine.Camera
E.RectTransform = UnityEngine.RectTransform
E.Vector3 = UnityEngine.Vector3
E.Vector2 = UnityEngine.Vector2
E.Vector4 = UnityEngine.Vector4
E.Rect = UnityEngine.Rect
E.Canvas = UnityEngine.Canvas
E.CanvasGroup = UnityEngine.CanvasGroup
E.Color = UnityEngine.Color
E.Animator = UnityEngine.Animator
E.AnimatorUpdateMode = UnityEngine.AnimatorUpdateMode
E.Animation = UnityEngine.Animation
E.WaitForSecondsRealtime = UnityEngine.WaitForSecondsRealtime
E.WaitForEndOfFrame = UnityEngine.WaitForEndOfFrame
E.WaitForFixedUpdate = UnityEngine.WaitForFixedUpdate
E.WaitForSeconds = UnityEngine.WaitForSeconds
E.Screen = UnityEngine.Screen
E.AudioSource = UnityEngine.AudioSource
E.Renderer = UnityEngine.Renderer
E.Shader = UnityEngine.Shader
E.ParticleSystem = UnityEngine.ParticleSystem
E.Quaternion = UnityEngine.Quaternion
E.Collider = UnityEngine.Collider
E.Material = UnityEngine.Material
E.Plane = UnityEngine.Plane
E.LayerMask = UnityEngine.LayerMask
E.Transform = UnityEngine.Transform
E.RectTransformUtility = UnityEngine.RectTransformUtility
E.AudioListener = UnityEngine.AudioListener
E.QualitySettings = UnityEngine.QualitySettings
E.RuntimePlatform = UnityEngine.RuntimePlatform
E.WWWForm = UnityEngine.WWWForm
E.UnityWebRequest = UnityEngine.Networking.UnityWebRequest
E.Input = UnityEngine.Input
E.Touch = UnityEngine.Touch
E.TouchPhase = UnityEngine.TouchPhase
E.EventSystem = UnityEngine.EventSystems.EventSystem

E.NetworkReachability = UnityEngine.NetworkReachability
E.SystemInfo = UnityEngine.SystemInfo
E.GUIUtility = UnityEngine.GUIUtility
E.SkinnedMeshRenderer = UnityEngine.SkinnedMeshRenderer

---------- UnityEngine.UI
local UI = UnityEngine.UI
E.GraphicRaycaster = UI.GraphicRaycaster
E.Button = UI.Button
E.Toggle = UI.Toggle
E.ToggleGroup = UI.ToggleGroup
E.Image = UI.Image
E.Selectable = UI.Selectable
E.Text = UI.Text
E.InputField = UI.InputField
E.Shadow = UI.Shadow
E.Outline = UI.Outline
E.ScrollRect = UI.ScrollRect


-- for k, v in pairs(CS) do
--     E[k] = v
-- end

T = T or {}

T.YSLoader = CS.YSLoader
T.LuaBridgeBase = CS.LuaBridgeBase
T.LuaBridgeType = E.GHelper.TraveEnumToLuaTable(typeof(CS.LuaBridgeType), {})
T.UnityBehaviourFunc =E.GHelper.TraveEnumToLuaTable(typeof(CS.UnityBehaviourFunc), {})

T.LuaFilePicker = CS.LuaFilePicker

T.Packet = CS.Packet
T.WarNet = CS.WarNet

_G.E = E
