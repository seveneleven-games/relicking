using System;
using System.Collections;
using System.Collections.Generic;
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
    
    
    // 통신 관련 (json 형식으로 반환하는 함수)
    // Get
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
    
    
    // Post
    public static IEnumerator PostRequest(string uri, string postData)
    {
        
        string finalUri = BASE_URI + uri;
        
        using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(finalUri, postData))
        {
            // 요청 보내기
            yield return webRequest.SendWebRequest();
            
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
            
        }
    }
    
    
    
    
    // 통신 관련 (데이터로 바로 변환하는 함수)
    //Get
    public static IEnumerator GetRequest<T>(string uri, Action<T> callback)
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
                T data = JsonUtility.FromJson<T>(webRequest.downloadHandler.text);
                callback(data);
            }
        }
    }
    // Post
    public static IEnumerator PostRequest<T>(string uri, T postData, Action<T> callback)
    {
        
        string finalUri = BASE_URI + uri;
        string jsonPostData = JsonUtility.ToJson(postData);
        
        using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(finalUri, jsonPostData))
        {
            
            // webRequest.SetRequestHeader("Content-Type", "application/json");
            
            // 요청 보내기
            yield return webRequest.SendWebRequest();
            
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
                
                T data = JsonUtility.FromJson<T>(webRequest.downloadHandler.text);
                callback(data);
            }
            
        }
    }
}