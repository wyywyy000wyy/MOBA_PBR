using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using XLua;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;
using System.Security.Cryptography;


[LuaCallCSharp]
public static class UnityEngineObjectExtention
{
    public static bool IsNull(this UnityEngine.Object o) // 或者名字叫IsDestroyed等等
    {
        return o == null;
    }
}

[LuaCallCSharp]
public static class GHelper
{
    static System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
    static Crc32 crc32 = new Crc32();
    static private string deviceCryptKey_ = null;

    public static int HashString(string str)
    {
        const uint InitialFNV = 2166136261U;
        const uint FNVMultiple = 16777619;

        uint hash = InitialFNV;
        for (int i = 0; i < str.Length; i++)
        {
            hash = hash ^ str[i];
            hash = hash * FNVMultiple;
        }

        return (int)(hash & 0x7FFFFFFF);
    }


    public static string Md5Sum(string input)
    {
        return Md5Sum(System.Text.Encoding.UTF8.GetBytes(input));
    }

    [BlackList]
    public static string Md5Sum(byte[] bytes)
    {
        byte[] hash = md5.ComputeHash(bytes);
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < hash.Length; i++)
        {
            sb.Append(hash[i].ToString("x2"));
        }
        return sb.ToString();
    }

    public static uint CrcHash(string input)
    {
        return CrcHash(System.Text.Encoding.ASCII.GetBytes(input));
    }

    [BlackList]
    public static uint CrcHash(byte[] bytes)
    {
        return Crc32.Compute(bytes);
    }

    public static uint ComputeFileChecksum(string path)
    {
        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        crc32.ComputeHash(fs);
        fs.Close();
        return crc32.GetHashResult();
    }

    [BlackList]
    public static string ByteArrayToHexString(byte[] bytes, int offset, int size)
    {
        StringBuilder result = new StringBuilder(bytes.Length * 2);
        string hexAlphabet = "0123456789ABCDEF";

        for (int i = offset; i < size; i++)
        {
            byte b = bytes[i];
            result.Append(hexAlphabet[(int)(b >> 4)]);
            result.Append(hexAlphabet[(int)(b & 0xF)]);
            result.Append(" ");
        }

        return result.ToString();
    }

    [BlackList]
    public static byte[] HexStringToByteArray(string hex)
    {
        byte[] bytes = new byte[hex.Length / 2];
        int[] hexValue = new int[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05,
        0x06, 0x07, 0x08, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };

        for (int x = 0, i = 0; i < hex.Length; i += 2, x += 1)
        {
            bytes[x] = (byte)(hexValue[Char.ToUpper(hex[i + 0]) - '0'] << 4 |
                            hexValue[Char.ToUpper(hex[i + 1]) - '0']);
        }

        return bytes;
    }

    public static DateTime TimeStampToDateTime(int timeStamp)
    {
        DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
        startTime = startTime.AddSeconds(timeStamp);
        return startTime;
    }

    public static int DateTimeToTimeStamp(DateTime dateTime)
    {
        DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
        return (int)(dateTime - startTime).TotalSeconds;
    }

    private static readonly DateTime utc_time = new DateTime(1970, 1, 1);
    public static uint iclock()
    {
        return (uint)(Convert.ToInt64(DateTime.UtcNow.Subtract(utc_time).TotalMilliseconds) & 0xffffffff);
    }

    public static int GetNaturalDaysCount(DateTime dtStart, DateTime dtEnd)
    {
        TimeSpan tsStart = new TimeSpan(dtStart.Ticks);
        TimeSpan tsEnd = new TimeSpan(dtEnd.Ticks);
        return tsEnd.Days - tsStart.Days;
    }

    public static int GetHoursCount(DateTime dtStart, DateTime dtEnd)
    {
        TimeSpan tsStart = new TimeSpan(dtStart.Ticks);
        TimeSpan tsEnd = new TimeSpan(dtEnd.Ticks);
        TimeSpan n = tsEnd - tsStart;
        return n.Hours + n.Days * 24;
    }

    public static int serverTimeOffset = 0;
    public static int ServerTime
    {
        get
        {
            return DateTimeToTimeStamp(DateTime.Now) + serverTimeOffset;
        }
    }

    public static DateTime ServerDateTime
    {
        get
        {
            return TimeStampToDateTime(ServerTime);
        }
    }

    public static void SyncTimeFromServer(int serverTime)
    {
        serverTimeOffset = serverTime - DateTimeToTimeStamp(DateTime.Now);
    }

    public static string NoCacheUrl(string url)
    {
        return string.Format("{0}?r={1}", url, ServerTime);
    }

    public static bool RayCastUI(float x, float y)
    {
        GraphicRaycaster rayCaster = GameObject.Find("UI/Canvas").GetComponent<GraphicRaycaster>();
        EventSystem eventSystem = GameObject.Find("UI/EventSystem").GetComponent<EventSystem>();

        PointerEventData eventData = new PointerEventData(eventSystem);
        eventData.position = new Vector2(x, y);

        List<RaycastResult> castResult = new List<RaycastResult>();
        rayCaster.Raycast(eventData, castResult);
        if (castResult.Count > 0 && !castResult[0].gameObject.name.Contains("Template"))
            return true;

        return false;
    }

    public static int GetStringLength(string str)
    {
        if (string.IsNullOrEmpty(str))
            return 0;

        int len = 0;
        char[] array = str.ToCharArray();
        Encoder code = Encoding.UTF8.GetEncoder();
        for (int i = 0; i < array.Length; i++)
        {
            int count = code.GetByteCount(array, i, 1, true);
            if (count > 1)
                len += 2;
            else
                len += 1;
        }

        return len;
    }

    public static string GetStringCut(string str, int length)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        if (length < 2)
            length = 2;

        Encoder code = Encoding.UTF8.GetEncoder();

        int len = 0;
        int i = 0;

        string finalStr = str;
        char[] array = str.ToCharArray();

        for (; i < array.Length; i++)
        {
            int count = code.GetByteCount(array, i, 1, true);
            if (count > 1)
                len += 2;
            else
                len += 1;

            if (len > length)
            {
                finalStr = str.Substring(0, i);
                break;
            }
        }

        return finalStr;
    }


    public static string EncodeNonAsciiCharacters(string value)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (char c in value)
        {
            if (c > 127)
            {
                // This character is too big for ASCII
                string encodedValue = "\\u" + ((int)c).ToString("x4");
                sb.Append(encodedValue);
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }


    public static string FilterEmojiString(string value)
    {
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < value.Length; i++)
        {
            char a = value[i];
            if (isEmojiCharacter(Convert.ToInt32(a)))
                continue;

            if (a == '%')
                builder.Append("%%");
            else
                builder.Append(a);
        }

        return builder.ToString();
    }

    public static bool StringHasEmoji(string value)
    {
        for (int i = 0; i < value.Length; i++)
        {
            char a = value[i];
            if (isEmojiCharacter(Convert.ToInt32(a)))
                return true;
        }
        return false;
    }

    public static Vector3 GetLastTouchPostion()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        return Input.mousePosition;
#else
        Touch touch = Input.GetTouch(0);
        return touch.position;
#endif
    }

    [BlackList]
    private static bool isEmojiCharacter(int codePoint)
    {
        return (codePoint >= 0xD800 && codePoint <= 0xDFFF);// 高低位替代符保留区域
        //    (codePoint >= 0x2600 && codePoint <= 0x27BF) // 杂项符号与符号字体
        //    || codePoint == 0x303D
        //    || codePoint == 0x2049
        //    || codePoint == 0x203C        
        //    || (codePoint >= 0x2000 && codePoint <= 0x200F)//
        //    || (codePoint >= 0x2028 && codePoint <= 0x202F)//        
        //    || codePoint == 0x205F //
        //    || (codePoint >= 0x2065 && codePoint <= 0x206F)//        
        //     /* 标点符号占用区域 */        
        //    || (codePoint >= 0x2100 && codePoint <= 0x214F)// 字母符号        
        //    || (codePoint >= 0x2300 && codePoint <= 0x23FF)// 各种技术符号        
        //    || (codePoint >= 0x2B00 && codePoint <= 0x2BFF)// 箭头A        
        //    || (codePoint >= 0x2900 && codePoint <= 0x297F)// 箭头B        
        //    || (codePoint >= 0x3200 && codePoint <= 0x32FF)// 中文符号
        //    || (codePoint >= 0xD800 && codePoint <= 0xDFFF);// 高低位替代符保留区域
        //    || (codePoint >= 0xE000 && codePoint <= 0xF8FF)// 私有保留区域
        //    || (codePoint >= 0xFE00 && codePoint <= 0xFE0F)// 变异选择器        
        //    || codePoint >= 0x10000; // Plane在第二平面以上的，char都不可以存，全部都转
    }

    public static void CreateDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    [BlackList]
    public static bool IsLowMemoryDevice()
    {
        return SystemInfo.systemMemorySize < 1100;
    }

    static public string EncodeXY(string j, string s)
    {
        string base64String = System.Convert.ToBase64String(Encoding.UTF8.GetBytes(j)).Replace("+", "-").Replace("/", "_").Replace("=", "");
        string signString = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(base64String + s))).Replace("-", "").Substring(8, 16).ToLower();

        return base64String + signString;
    }

    public static string[] ListFile(string fullPath)
    {
        List<string> fileNames = new List<string>();

        //获取指定路径下面的所有资源文件  
        if (Directory.Exists(fullPath))
        {
            DirectoryInfo direction = new DirectoryInfo(fullPath);
            FileInfo[] files = direction.GetFiles("*", SearchOption.TopDirectoryOnly);

            Debug.Log(files.Length);

            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".meta"))
                {
                    continue;
                }
                fileNames.Add(files[i].Name);
            }
        }
        return fileNames.ToArray();
    }

    public static string GetPersistentDataPath()
    {
        string fullPath = Application.persistentDataPath + "/";  //路径
        return fullPath;
    }

    public static LuaTable TraveEnumToLuaTable(Type eType, LuaTable tb)
    {
        foreach (var e in Enum.GetValues(eType))
        {
            tb.Set(e.ToString(), (int)e);
        }
        return tb;
    }

    //根据设备udid生成的32位key。
    //public static string Device32BitCryptKey
    //{
    //    get
    //    {
    //        if (string.IsNullOrEmpty(deviceCryptKey_))
    //        {
    //            var udidBytes = Encoding.UTF8.GetBytes(PhoneHelper.DeviceUDID());
    //            byte[] finalBytes = new byte[32];

    //            for (var i = 0; i < finalBytes.Length; ++i)
    //            {
    //                if (i < udidBytes.Length)
    //                {
    //                    finalBytes[i] = udidBytes[i];
    //                }
    //                else
    //                {
    //                    finalBytes[i] = (byte)i;
    //                }
    //            }
    //            // 不要用 UTF8.GetString,因为如果有特殊字符的话可能返回值是 > 32位的。
    //            deviceCryptKey_ = Encoding.ASCII.GetString(finalBytes);
    //        }
    //        return deviceCryptKey_;
    //    }
    //}

    //加密字符串，cryptKey需要长度为 32/64/128/258
    public static string EncryptText(string data, string cryptKey)
    {
        byte[] bs = Encoding.UTF8.GetBytes(data);

        RijndaelManaged aes256 = new RijndaelManaged();
        aes256.Key = Encoding.UTF8.GetBytes(cryptKey);
        aes256.Mode = CipherMode.ECB;
        aes256.Padding = PaddingMode.PKCS7;

        return Convert.ToBase64String(aes256.CreateEncryptor().TransformFinalBlock(bs, 0, bs.Length));
    }

    //解密字符串
    public static string DecryptText(string data, string cryptKey)
    {
        byte[] bs = Convert.FromBase64String(data);

        RijndaelManaged aes256 = new RijndaelManaged();
        aes256.Key = Encoding.UTF8.GetBytes(cryptKey);
        aes256.Mode = CipherMode.ECB;
        aes256.Padding = PaddingMode.PKCS7;

        return Encoding.UTF8.GetString(aes256.CreateDecryptor().TransformFinalBlock(bs, 0, bs.Length));
    }

    public static string GameVersion()
    {
        return GameConfig.version.GetVersion();
    }

    public static int GetVersionCode()
    {
        return GameConfig.version.GetVersionCode();
    }

    public static string GetPlatformName()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                return "android";
            case RuntimePlatform.IPhonePlayer:
                return "ios";
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
                return "win";
            default:
                return "unkown";
        }
    }

    public static string DecryptTextSafely(string data, string cryptKey)
    {
        try
        {
            var text = DecryptText(data, cryptKey);
            return text;
        }
        catch
        {
            return null;
        }
    }

    public static Texture2D GetRTPixels(RenderTexture texure) {
        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = texure;
        Texture2D tex = new Texture2D(texure.width, texure.height);
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        RenderTexture.active = prev;
        return tex;
    }

    public static void SaveRenderTexureToJPG(RenderTexture texture, string path) {
        RenderTexture temp = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.ARGB32);
        Graphics.Blit(texture, temp);
        Texture2D tex2D = GetRTPixels(texture);
        RenderTexture.ReleaseTemporary(temp);
        File.WriteAllBytes(path, tex2D.EncodeToJPG());
    }

    public static void SaveTexure2DToJPG(Texture2D texture2D, string path) {
        File.WriteAllBytes(path, texture2D.EncodeToJPG());
    }

    //[BlackList]
    //public static IEnumerator DoScreenShot_ScreenCapture(string fileName, System.Action<bool, Texture2D> callback){
    //    yield return new WaitForEndOfFrame();
    //    Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture();
    //    bool isSaveToGallery = fileName.Length > 0;
    //    if (isSaveToGallery) {
    //        NativeGallery.Permission type = NativeGallery.SaveImageToGallery(texture, "fruit", fileName);
    //        if (type == NativeGallery.Permission.Granted) {
    //            Debug.Log("保存至相册成功");
    //        } else {
    //            isSaveToGallery = false;
    //            Debug.Log("保存至相册失败");
    //        }
    //    }
    //    callback(isSaveToGallery, texture);
    //}

    ////Unity自带的截图功能
    //public static void ScreenShot_ScreenCapture(string fileName, System.Action<bool, Texture2D> callback){
    //    CoroutineHelper.Run(DoScreenShot_ScreenCapture(fileName, callback));
    //}

    //[BlackList]
    //public static IEnumerator DoScreenShot_ReadPixels(string fileName, Action<bool, Texture2D> callback)
    //{
    //    yield return new WaitForEndOfFrame();
    //    int width = Screen.width;
    //    int height = Screen.height;
    //    Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
    //    texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
    //    texture.Apply();
    //    bool isSaveToGallery = fileName.Length > 0;
    //    if (isSaveToGallery) {
    //        NativeGallery.Permission type = NativeGallery.SaveImageToGallery(texture, "fruit", fileName);
    //        if (type == NativeGallery.Permission.Granted) {
    //            Debug.Log("保存至相册成功");
    //        } else {
    //            isSaveToGallery = false;
    //            Debug.Log("保存至相册失败");
    //        }
    //    }
    //    callback(isSaveToGallery, texture);
    //}

    ////读取屏幕像素进行截图
    //public static void ScreenShot_ReadPixels(string fileName, Action<bool, Texture2D> callback)
    //{
    //    CoroutineHelper.Run(DoScreenShot_ReadPixels(fileName, callback));
    //}

    //[BlackList]
    //public static IEnumerator DoScreenShot_ReadPixelsWithCamera(Camera camera, string fileName, Action<bool, Texture2D> callback)
    //{
    //    yield return new WaitForEndOfFrame();
    //    //对指定相机进行 RenderTexture
    //    RenderTexture renTex = new RenderTexture(Screen.width, Screen.height, 16);
    //    camera.targetTexture = renTex;
    //    camera.Render();
    //    RenderTexture.active = renTex;

    //    //读取像素
    //    Texture2D texture = new Texture2D(Screen.width, Screen.height);
    //    texture.ReadPixels(new Rect(0, 0, texture.width, texture.height),0,0);
    //    texture.Apply();

    //    //读取目标相机像素结束，渲染恢复原先的方式
    //    camera.targetTexture = null;
    //    RenderTexture.active = null;

    //    GameObject.Destroy(renTex);

    //    bool isSaveToGallery = fileName.Length > 0;
    //    if (isSaveToGallery) {
    //        NativeGallery.Permission type = NativeGallery.SaveImageToGallery(texture, "fruit", fileName);
    //        if (type == NativeGallery.Permission.Granted) {
    //            Debug.Log("保存至相册成功");
    //        } else {
    //            isSaveToGallery = false;
    //            Debug.Log("保存至相册失败");
    //        }
    //    }
    //    callback(isSaveToGallery, texture);
    //}

    ////读取指定相机渲染的像素进行截图
    //public static void ScreenShot_ReadPixelsWithCamera(Camera camera, string fileName, Action<bool, Texture2D> callback)
    //{
    //    CoroutineHelper.Run(DoScreenShot_ReadPixelsWithCamera(camera, fileName, callback));
    //}
}
