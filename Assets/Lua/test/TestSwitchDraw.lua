
local CameraGo = E.GameObject.Find("MainCamera")
local Camera = CameraGo:GetComponent(typeof(E.Camera))
Camera.orthographic = true
local Plane = E.GameObject.Find("MainCamera/Plane")
local Plane2 = E.GameObject.Find("MainCamera/Plane2")

local Renderer = Plane:GetComponent(typeof(E.Renderer))
local Renderer2 = Plane2:GetComponent(typeof(E.Renderer))

-- local material = Renderer.material--sharedMaterial
-- local material2 = Renderer2.material--sharedMaterial
local material = Renderer.sharedMaterial
local material2 = Renderer2.sharedMaterial

local shader = material.shader

local cloneContainer = {}

local DrawTest = E.GameObject.Find("DrawTest")
local DrawTestComp = DrawTest:GetComponent(typeof(CS.DrawTest))

local imgs = {
    DrawTestComp.verylow,
    DrawTestComp.low,
    DrawTestComp.mid,
    DrawTestComp.high,
}
local imgs2 = {
    DrawTestComp.verylow2,
    DrawTestComp.low2,
    DrawTestComp.mid2,
    DrawTestComp.high2,
}

local imgss = {}
for i = 0, DrawTestComp.imgs.Count-1 do
    local img = DrawTestComp.imgs[i]
    table.insert(imgss, img)
end

local curImage = 4

material:SetTexture("_MainTex",imgs[curImage])
material2:SetTexture("_MainTex",imgs2[curImage])


local buttonGo = E.GameObject.Find("Canvas/Button")
local button = buttonGo:GetComponent(typeof(E.Button))
local UnityEvent = E.ButtonClickedEvent()

local set_texture=function(go, imgs)
    local Renderer = go:GetComponent(typeof(E.Renderer))
    local material = Renderer.material
    material:SetTexture("_MainTex",imgs[curImage])
end

local set_sortingOrder=function(go, sortingOrder)
    local Renderer = go:GetComponent(typeof(E.Renderer))
    Renderer.sortingOrder = sortingOrder
end

UnityEvent:AddListener(function()
    logger.print("button onClick")
    curImage = (curImage % (#imgs))+1
    material:SetTexture("_MainTex",imgs[curImage])
    material2:SetTexture("_MainTex",imgs2[curImage])
end)

button.onClick = UnityEvent

local InputFieldGo = E.GameObject.Find("Canvas/InputField")
local InputField = InputFieldGo:GetComponent(typeof(E.InputField))
local count = 1
local SubmitEvent = E.SubmitEvent()

local set_count = function()
    for _, v in ipairs(cloneContainer) do
        E.GameObject.Destroy(v)
    end
    cloneContainer = {}
    Plane:SetActive(true)
    Plane2:SetActive(true)
    for i = 1, count do
        local go =E.GameObject.Instantiate(Plane);
        go.transform.localPosition = E.Vector3(0,0,i*2)
        set_sortingOrder(go, i*2)
        -- set_texture(go, imgs)
        local go2 =E.GameObject.Instantiate(Plane2);
        go2.transform.localPosition = E.Vector3(0,0,i*2+1)
        set_sortingOrder(go2, i*2+1)
        -- set_texture(go2, imgs2)
        table.insert(cloneContainer, go)
        table.insert(cloneContainer, go2)
    end
    Plane:SetActive(false)
    Plane2:SetActive(false)
end

SubmitEvent:AddListener(function(v)
    logger.print("on_value_changed " .. (v or "nil"))
    count = tonumber(v)
    set_count()
end)
set_count()
InputField.onEndEdit = SubmitEvent
InputField.text = count


local support = E.SystemInfo.SupportsRenderTextureFormat(E.RenderTextureFormat.ARGBFloat)

logger.print("SupportsRenderTextureFormat ARGBFloat " .. (support and "support" or "not support"))

local ScaleGo = E.GameObject.Find("Canvas/Scale")
local ScaleSlider = ScaleGo:GetComponent(typeof(E.Slider))
local ScaleSliderEvent = E.SliderEvent()
local scale = 1
ScaleSliderEvent:AddListener(function(v)
    logger.print("on_value_changed " .. (v or "nil"))
    scale = tonumber(v)
    material:SetTextureScale("_MainTex",E.Vector2(scale, scale))
    material2:SetTextureScale("_MainTex",E.Vector2(scale, scale))
end)
ScaleSlider.onValueChanged = ScaleSliderEvent
ScaleSlider.value = scale

local LoopGo = E.GameObject.Find("Canvas/Loop")
local LoopSlider = LoopGo:GetComponent(typeof(E.InputField))
local LoopSliderEvent = E.SubmitEvent()
local loop = 1
local set_loop = function()
    material:SetFloat("loop",loop)
    material2:SetFloat("loop",loop)
end
LoopSlider.text = loop
LoopSliderEvent:AddListener(function(v)
    logger.print("loop_on_value_changed " .. (v or "nil"))
    loop = tonumber(v)
    set_loop()
end)
LoopSlider.onEndEdit = LoopSliderEvent
set_loop()



