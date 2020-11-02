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
        Vector3 norVec= Vector3.Project(dir, Camera.main.transform.forward);
        //返回世界空间
        return Camera.main.ViewportToWorldPoint
            (
               new Vector3(
                   ScreenPoint.x/Screen.width,
                   ScreenPoint.y/Screen.height,
                   norVec.magnitude
               )
            );
    }
 
    [XLua.BlackList]
    public void OnMouseDown()
    {
        isDraging = true;
        CallLuaBindFunc(UnityBehaviourFunc.OnBeginDrag);
    }

    [XLua.BlackList]
    public void OnMouseUp()
    {
        isDraging = false;
        CallLuaBindFunc(UnityBehaviourFunc.OnEndDrag);
    }

    public void OnMouseDrag()
    {
        CallLuaBindFunc(UnityBehaviourFunc.OnDrag);
    }

    [XLua.BlackList]
    void Update()
    {
        if(isDraging)
        {
            
        #if UNITY_STANDALONE || UNITY_EDITOR
            CallLuaBindFunc(UnityBehaviourFunc.OnDrag, Input.mousePosition);
#else
            if(Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                CallLuaBindFunc(UnityBehaviourFunc.OnDrag, touch.position);
            }
#endif
        }
    }
}
