using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static Define;



public static class Util
{
    
    
    public static T GetOrAddComponent<T>(GameObject go) where T : Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();

        return component;
    }

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;

        return transform.gameObject;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }

    public static T ParseEnum<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }
    
    // 색깔 관련
    public static Color HexToColor(string color)
    {
        Color parsedColor;
        ColorUtility.TryParseHtmlString("#"+color, out parsedColor);

        return parsedColor;
    }
    
    
    // Get (json 형식으로)
    public static IEnumerator GetRequest(string uri, Action<string> callback)
    {
        
        string finalUri = BASE_URI + uri;
        
        using (UnityWebRequest webRequest = UnityWebRequest.Get(finalUri))
        {
            // 요청 보내기
            yield return webRequest.SendWebRequest();
            
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
                string data = webRequest.downloadHandler.text;
                callback(data);
            }
        }
    }
    
    
    // POST 요청 (json 형식으로)
    public static IEnumerator PostRequest(string uri, string jsonData, Action<string> callback)
    {
        string finalUri = BASE_URI + uri;
        using (UnityWebRequest webRequest = new UnityWebRequest(finalUri, "POST"))
        {
            byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            // 요청 보내기
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
                // callback(null);
            }
            else
            {
                Debug.Log("Received: " + webRequest.downloadHandler.text);
                callback(webRequest.downloadHandler.text);
            }
        }
    }
    
    // PATCH 요청
    public static IEnumerator PatchRequest(string uri, string jsonData, Action<string> callback)
    {
        string finalUri = BASE_URI + uri;
        using (UnityWebRequest webRequest = new UnityWebRequest(finalUri, "PATCH"))
        {
            byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            // 요청 보내기
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
                // callback(null); // 오류 시 콜백을 null로 호출할 경우 주석을 해제
            }
            else
            {
                Debug.Log("Received: " + webRequest.downloadHandler.text);
                callback(webRequest.downloadHandler.text);
            }
        }
    }
    
}