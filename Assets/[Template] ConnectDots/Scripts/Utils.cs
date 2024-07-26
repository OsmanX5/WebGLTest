using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal static class Utils {

    public static GameObject[] GetChildrens(this GameObject value)
    {
        List<GameObject> result = new List<GameObject>();
        int length = value.transform.childCount;
        for (int i = 0; i < length; i++)
        {
            result.Add(value.transform.GetChild(i).gameObject);
        }
        return result.ToArray();
    }

    public static Transform[] GetChildren(this Transform value) 
    {
        List<Transform> result = new List<Transform>();
        int length = value.childCount;
        for (int i = 0; i < length; i++)
        {
            result.Add(value.GetChild(i));
        }
        return result.ToArray();
    }

    public static void RemoveAllChildren(this Transform value)
    {
        Transform[] childs = value.GetChildren();
        value.DetachChildren();

        foreach (Transform t in childs) {
            Transform.DestroyImmediate(t.gameObject);
        }
    }

    public static Vector2 MousePositionToCanvasPosition(this Camera main, Canvas canvas) {
        RectTransform rt = canvas.transform.GetComponent<RectTransform>();
        float width = rt.rect.width;
        float height = rt.rect.height;
        Vector2 canvasPos = Vector2.zero;
        if (!Application.isMobilePlatform)
        {
            Vector2 viewPort = main.ScreenToViewportPoint(Input.mousePosition);
            canvasPos = new Vector2((viewPort.x * width) - (width / 2),
                                            (viewPort.y * height) - (height / 2));
        }
        else {
            if (Input.touchCount > 0) {
                Vector2 viewPort = main.ScreenToViewportPoint(Input.touches[0].position);
                canvasPos = new Vector2((viewPort.x * width) - (width / 2),
                                            (viewPort.y * height) - (height / 2));
            }
        }
        return canvasPos;
    }

    public static Vector2 MousePositionToViewPortPosition(this Camera main) {
        return ((Vector2)main.ScreenToViewportPoint(Input.mousePosition) - new Vector2(0.5f, 0.5f));
    }

    public static void SetWidthHeight(this RectTransform value, float width, float height)
    {
        RectTransform rt = new RectTransform();
        rt.sizeDelta = new Vector2(width, height);
        value.sizeDelta = rt.sizeDelta;
    }

    public static string ListIntToString(this List<int> value)
    {
        string result = "";
        for (int i = 0; i < value.Count; i++) 
        {
            result += value[i].ToString();
            result += (i + 1 == value.Count) ? "" : ",";  
        }
        return result;
    }

    public static List<int> ToListInt(this string value) 
    {
        List<int> result = new List<int>();
        try
        {
            string[] arrayStr = value.Split(',');
            if (arrayStr.Length > 0)
            {
                for (int i = 0; i < arrayStr.Length; i++)
                {
                    result.Add(int.Parse(arrayStr[i]));
                }
            }
            else {
                result.Add(int.Parse(value));
            }
            
        }
        catch {
            
        }
        return result;
    }

    public static string ListColorToString(this List<Color> value)
    {
        string result = "";
        for (int i = 0; i < value.Count; i++)
        {
            result += value[i].ToString().Replace("RGBA(", "").Replace(")", "");
            result += (i + 1 == value.Count) ? "" : "|";
        }
        return result;
    }

    public static List<Color> ToListColor(this string value) 
    {
        List<Color> result = new List<Color>();
        try
        {
            string[] arrayStr = value.Split('|');
            if (arrayStr.Length > 0)
            {
                for (int i = 0; i < arrayStr.Length; i++)
                {
                    string[] floatArray = arrayStr[i].Split(',');
                    float r = float.Parse(floatArray[0]);
                    float g = float.Parse(floatArray[1]);
                    float b = float.Parse(floatArray[2]);
                    float a = float.Parse(floatArray[3]);
                    result.Add(new Color(r, g, b, a));
                }
            }
            else {
                string[] floatArray = arrayStr[0].Split(',');
                float r = float.Parse(floatArray[0]);
                float g = float.Parse(floatArray[1]);
                float b = float.Parse(floatArray[2]);
                float a = float.Parse(floatArray[3]);
                result.Add(new Color(r, g, b, a));
            }
        }
        catch { }
        return result;
    }

    public static string ListVectorToString(this List<Vector2Int> value) 
    {
        string result = "";

        for (int i = 0; i < value.Count; i++)
        {
            result += value[i].ToString();
            result += (i + 1 == value.Count) ? "" : "|";
        }

        return result;
    }

    public static List<Vector2Int> ToListVector2Int(this string value) 
    {
        List<Vector2Int> result = new List<Vector2Int>();
        try
        {
            string tempValue = value.Replace("(", "").Replace(")", "");
            string[] arrayStr = tempValue.Split('|');
            if (arrayStr.Length > 0)
            {
                for (int i = 0; i < arrayStr.Length; i++)
                {
                    string[] floatArray = arrayStr[i].Split(',');
                    int a = int.Parse(floatArray[0]);
                    int b = int.Parse(floatArray[1]);
                    result.Add(new Vector2Int(a, b));
                }
            }
            else { 
                    string[] floatArray = arrayStr[0].Split(',');
                    int a = int.Parse(floatArray[0]);
                    int b = int.Parse(floatArray[1]);
                    result.Add(new Vector2Int(a, b));
            }
        }
        catch { }
        return result;
    }
}
