using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIContainer : MonoBehaviour
{
    // Start is called before the first frame update
    [System.Serializable]
    public class UIInfo
    {
        public string name;
        public Object comp;
    }

    public List<UIInfo> uiList = new List<UIInfo>();
}
