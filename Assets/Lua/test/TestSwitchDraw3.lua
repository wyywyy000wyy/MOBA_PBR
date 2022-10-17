
local CameraGo = E.GameObject.Find("MainCamera")
local Camera = CameraGo:GetComponent(typeof(E.Camera)) --- @type UnityEngine.Camera
-- Camera.orthographic = true

local Plane = E.GameObject.Find("MainCamera/Plane")
local Renderer = Plane:GetComponent(typeof(E.Renderer))
local material = Renderer.material
Plane:SetActive(false)
local Plane2 = E.GameObject.Find("MainCamera/Plane2")
Plane2:SetActive(false)

Plane = E.GameObject.Find("Canvas/ui_root/Plane")

local Image = Plane:GetComponent(typeof(E.RawImage))


local cloneContainer = {}

local DrawTest = E.GameObject.Find("DrawTest")
local DrawTestComp = DrawTest:GetComponent(typeof(CS.DrawTest))

local imgs = {}
for i = 0, DrawTestComp.imgs.Count-1 do
    local img = DrawTestComp.imgs[i]
    table.insert(imgs, img)
end

local for_each = function(func)
    for i, v in ipairs(cloneContainer) do
        local Image = v:GetComponent(typeof(E.RawImage))
        local material = Image.material
        func(i, v, Image, material)
    end
end

local curImage = 1

local buttonGo = E.GameObject.Find("Canvas/Button")
local button = buttonGo:GetComponent(typeof(E.Button))
local UnityEvent = E.ButtonClickedEvent()

local set_sortingOrder=function(go, sortingOrder)
    local Renderer = go:GetComponent(typeof(E.Renderer))
    Renderer.sortingOrder = sortingOrder
end

local set_textures = function()
    for_each(function(i, go, _, mat)
        local idx = i%(#imgs) + 1
        mat:SetTexture("_MainTex",imgs[idx])
    end)
end

logger.print("button TestSwitchDraw3")


UnityEvent:AddListener(function()
    curImage = (curImage % (#imgs))+1
    logger.print("button onClick")

    ys_loader.LoadAssetAsync("tex_grass",function(tex)
        for_each(function(i, go, img, mat)
            -- local idx = i%(#imgs) + 1
            img.texture = tex
            -- mat:SetTexture("_MainTex",tex)
        end)
    end)

    logger.print("button onClick")
    -- material:SetTexture("_MainTex",imgs[curImage])
    -- set_textures()
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
    for i = 1, count do
        local go =E.GameObject.Instantiate(Plane);
        -- go.transform.localPosition = E.Vector3(0,0,100 - i)
        -- set_sortingOrder(go, i)
        table.insert(cloneContainer, go)
    end
    
    for_each(function(i, v, Image, material)
        v.transform:SetParent(Plane.transform.parent,true)

        local pRectTransform = Plane:GetComponent(typeof(E.RectTransform))
        local RectTransform = v:GetComponent(typeof(E.RectTransform))
        -- RectTransform.sizeDelta = pRectTransform.sizeDelta
        RectTransform.offsetMin = pRectTransform.offsetMin
        RectTransform.offsetMax = pRectTransform.offsetMax
        local mat = E.Material(material)
        mat.name = "mat_" .. i
        Image.material = mat
    end)

    Plane:SetActive(false)
    set_textures()
end

-- SubmitEvent:AddListener(function(v)
--     logger.print("on_value_changed " .. (v or "nil"))
--     count = tonumber(v)
--     set_count()
-- end)
-- set_count()
-- InputField.onEndEdit = SubmitEvent
-- InputField.text = count


local support = E.SystemInfo.SupportsRenderTextureFormat(E.RenderTextureFormat.ARGBFloat)

logger.print("SupportsRenderTextureFormat ARGBFloat " .. (support and "support" or "not support"))

local ScaleGo = E.GameObject.Find("Canvas/Scale")
local ScaleSlider = ScaleGo:GetComponent(typeof(E.Slider))
local ScaleSliderEvent = E.SliderEvent()
local scale = 1
ScaleSliderEvent:AddListener(function(v)
    logger.print("on_value_changed " .. (v or "nil"))
    scale = tonumber(v)
    for_each(function(i, go, _, mat)
        mat:SetTextureScale("_MainTex",E.Vector2(scale, scale))
    end)
end)
ScaleSlider.onValueChanged = ScaleSliderEvent
ScaleSlider.value = scale

local LoopGo = E.GameObject.Find("Canvas/Loop")
local LoopSlider = LoopGo:GetComponent(typeof(E.InputField))
local LoopSliderEvent = E.SubmitEvent()
local loop = 1
local set_loop = function()
    for_each(function(i, go, _, mat)
        material:SetFloat("loop",loop)
    end)
end
LoopSlider.text = loop
-- LoopSliderEvent:AddListener(function(v)
--     logger.print("loop_on_value_changed " .. (v or "nil"))
--     loop = tonumber(v)
--     set_loop()
-- end)
-- LoopSlider.onEndEdit = LoopSliderEvent
set_loop()



