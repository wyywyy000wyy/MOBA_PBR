using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[XLua.LuaCallCSharp]
public class LuaBridgeDragable3D : LuaBridgeBase
{
    bool isDraging = false;

    Vector3 ScreenPointToWorldPoint(Vector2 ScreenPoint, Vector3 pos)
    {
        //1 得到物体在主相机的xx方向
        Vector3 dir = (pos - Camera.main.transform.position);
        //2 计算投影 (计算单位向量上的法向量)
        Vector3 norVec = Vector3.Project(dir, Camera.main.transform.forward);
        //返回世界空间
        return Camera.main.ViewportToWorldPoint
            (
               new Vector3(
                   ScreenPoint.x / Screen.width,
                   ScreenPoint.y / Screen.height,
                   norVec.magnitude
               )
            );
    }

    [XLua.BlackList]
    public void OnBeginDrag()
    //public void OnMouseDown()
    {
        isDraging = true;
        CallLuaBindFunc(UnityBehaviourFunc.OnBeginDrag);
    }

    [XLua.BlackList]
    //public void OnMouseUp()
    public void OnEndDrag()
    {
        isDraging = false;
        CallLuaBindFunc(UnityBehaviourFunc.OnEndDrag);
    }

    public void OnMouseDrag()
    {
    }

    [XLua.BlackList]
    public void OnDrag()
    //public void OnMouseDrag()
    {
        CallLuaBindFunc(UnityBehaviourFunc.OnDrag);
    }

    int touchCount = 0;

    [XLua.BlackList]
    void Update()
    {
        CallLuaBindFunc(UnityBehaviourFunc.Update, Time.deltaTime);
#if UNITY_STANDALONE || UNITY_EDITOR

        if (Input.GetMouseButtonDown(0))
        {
            if(EventSystem.current.IsPointerOverGameObject() == false)
            OnBeginDrag();
        }
        else if(Input.GetMouseButtonUp(0) && isDraging)
        {
            OnEndDrag();
        }
        else if(Input.GetMouseButton(0) && isDraging)
        {
            OnDrag();
        }
#else

        int count = Input.touchCount;
        if(count > 0 )
        {
            if(touchCount == 0)
            {
                if(EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) == false)
                    OnBeginDrag();
            }
            else
            {
                if(isDraging)
                OnDrag();
            }
        }
        else
        {
            if (touchCount > 0 && isDraging)
            {
                OnEndDrag();
            }
        }
#endif

        //        if (isDraging)
        //        {
        //#if UNITY_STANDALONE || UNITY_EDITOR
        //            CallLuaBindFunc(UnityBehaviourFunc.OnDrag, Input.mousePosition);
        //#else
        //            if(Input.touchCount > 0)
        //            {
        //                Touch touch = Input.GetTouch(0);
        //                CallLuaBindFunc(UnityBehaviourFunc.OnDrag, touch.position);
        //            }
        //#endif
        //        }
    }
}
