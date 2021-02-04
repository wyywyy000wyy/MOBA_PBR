---@type U
local U = {}
local CS = CS
---------- Custom
U.GHelper = CS.GHelper


-- Json = LuaJson

---------- UnityEngine
local UnityEngine = CS.UnityEngine
U.Application = UnityEngine.Application
U.Debug = UnityEngine.Debug
U.GameObject = UnityEngine.GameObject
U.SpriteRenderer = UnityEngine.SpriteRenderer
U.Texture = UnityEngine.Texture
U.Resources = UnityEngine.Resources
U.Time = UnityEngine.Time
U.Mathf = UnityEngine.Mathf
U.Camera = UnityEngine.Camera
U.RectTransform = UnityEngine.RectTransform
U.Vector3 = UnityEngine.Vector3
U.Vector2 = UnityEngine.Vector2
U.Vector4 = UnityEngine.Vector4
U.Rect = UnityEngine.Rect
U.Canvas = UnityEngine.Canvas
U.CanvasGroup = UnityEngine.CanvasGroup
U.Color = UnityEngine.Color
U.Animator = UnityEngine.Animator
U.AnimatorUpdateMode = UnityEngine.AnimatorUpdateMode
U.Animation = UnityEngine.Animation
U.WaitForSecondsRealtime = UnityEngine.WaitForSecondsRealtime
U.WaitForEndOfFrame = UnityEngine.WaitForEndOfFrame
U.WaitForFixedUpdate = UnityEngine.WaitForFixedUpdate
U.WaitForSeconds = UnityEngine.WaitForSeconds
U.Screen = UnityEngine.Screen
U.AudioSource = UnityEngine.AudioSource
U.Renderer = UnityEngine.Renderer
U.Shader = UnityEngine.Shader
U.ParticleSystem = UnityEngine.ParticleSystem
U.Quaternion = UnityEngine.Quaternion
U.Collider = UnityEngine.Collider
U.Material = UnityEngine.Material
U.Plane = UnityEngine.Plane
U.LayerMask = UnityEngine.LayerMask
U.Transform = UnityEngine.Transform
U.RectTransformUtility = UnityEngine.RectTransformUtility
U.AudioListener = UnityEngine.AudioListener
U.QualitySettings = UnityEngine.QualitySettings
U.RuntimePlatform = UnityEngine.RuntimePlatform
U.WWWForm = UnityEngine.WWWForm
U.UnityWebRequest = UnityEngine.Networking.UnityWebRequest
U.Input = UnityEngine.Input
U.Touch = UnityEngine.Touch
U.TouchPhase = UnityEngine.TouchPhase
U.EventSystem = UnityEngine.EventSystems.EventSystem

U.NetworkReachability = UnityEngine.NetworkReachability
U.SystemInfo = UnityEngine.SystemInfo
U.GUIUtility = UnityEngine.GUIUtility
U.SkinnedMeshRenderer = UnityEngine.SkinnedMeshRenderer

---------- UnityEngine.UI
local UI = UnityEngine.UI
U.GraphicRaycaster = UI.GraphicRaycaster
U.Button = UI.Button
U.Toggle = UI.Toggle
U.ToggleGroup = UI.ToggleGroup
U.Image = UI.Image
U.Selectable = UI.Selectable
U.Text = UI.Text
U.InputField = UI.InputField
U.Shadow = UI.Shadow
U.Outline = UI.Outline
U.ScrollRect = UI.ScrollRect


-- for k, v in pairs(CS) do
--     U[k] = v
-- end

U.XLoader = CS.XLoader
U.LuaBridgeBase = CS.LuaBridgeBase
U.LuaBridgeType = U.GHelper.TraveEnumToLuaTable(typeof(CS.LuaBridgeType), {})
U.UnityBehaviourFunc =U.GHelper.TraveEnumToLuaTable(typeof(CS.UnityBehaviourFunc), {})

U.Packet = CS.Packet
U.WarNet = CS.WarNet

_G.U = U
