using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static Define;
using UnityEngine;

[Serializable]
public class DataRes
{
    public int status;
    public string message;
    public bool data;
}

public static class Util
{
    public static T GetOrAddComponent<T>(GameObject go) where T : Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();

        return component;
    }

    public static float UnitySpeed(float speed)
    {
        return MIN_SPEED + (speed/SPEED_RATE) * RANGE;
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
        ColorUtility.TryParseHtmlString("#" + color, out parsedColor);

        return parsedColor;
    }

    #region 일반 웹 통신

    // Get (json 형식으로)
    public static IEnumerator GetRequest(string uri, Action<string> callback)
    {
        string finalUri = BASE_URI + uri;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(finalUri))
        {
            // 요청 보내기
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
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

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
                Debug.LogError($"Server Response: {webRequest.downloadHandler.text}"); // 서버 응답 로깅
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

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
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

    #endregion

    #region JWT 통신

    // 토큰 재발급 요청
    private static IEnumerator RequestNewToken()
    {
        string tokenUri = BASE_URI + "members/reissue";

        string refreshToken = Managers.Game.RefreshToken;

        string jsonData = JsonUtility.ToJson(new { refreshToken = refreshToken });

        using (UnityWebRequest webRequest = new UnityWebRequest(BASE_URI + tokenUri, "POST"))
        {
            byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // 토큰 파싱
                Managers.Game.AccessToken = webRequest.downloadHandler.text;
            }
            else
            {
                Debug.LogError("Token request failed: " + webRequest.error);
            }
        }
    }

    public static IEnumerator JWTGetRequest(string uri, Action<string> callback)
    {
        string finalUri = BASE_URI + uri;

        string accessToken = Managers.Game.AccessToken;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(finalUri))
        {
            webRequest.SetRequestHeader("Authorization", "Bearer " + accessToken);

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);

                // json -> 객체로 변환
                DataRes dataRes = JsonUtility.FromJson<DataRes>(webRequest.downloadHandler.text);

                // 토큰이 만료되었다면?
                if (dataRes.message == "JWT가 만료되었습니다.")
                {
                    yield return RequestNewToken(); // 토큰 재발급 요청
                    yield return JWTGetRequest(uri, callback); // 요청 재시도
                }
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
                callback(webRequest.downloadHandler.text);
            }
        }
    }

    public static IEnumerator JWTPostRequest(string uri, string jsonData, Action<string> callback)
    {
        string finalUri = BASE_URI + uri;

        string accessToken = Managers.Game.AccessToken;

        using (UnityWebRequest webRequest = new UnityWebRequest(finalUri, "POST"))
        {
            byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", "Bearer " + accessToken);

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
                Debug.LogError($"Server Response: {webRequest.downloadHandler.text}");

                // json -> 객체로 변환
                DataRes dataRes = JsonUtility.FromJson<DataRes>(webRequest.downloadHandler.text);

                // 토큰이 만료되었다면?
                if (dataRes.message == "JWT가 만료되었습니다.")
                {
                    yield return RequestNewToken(); // 토큰 재발급 요청
                    yield return JWTPostRequest(uri, jsonData, callback); // 요청 재시도
                }
            }
            else
            {
                Debug.Log("Received: " + webRequest.downloadHandler.text);
                callback(webRequest.downloadHandler.text);
            }
        }
    }

    public static IEnumerator JWTPatchRequest(string uri, string jsonData, Action<string> callback)
    {
        string finalUri = BASE_URI + uri;

        string accessToken = Managers.Game.AccessToken;

        using (UnityWebRequest webRequest = new UnityWebRequest(finalUri, "PATCH"))
        {
            byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", "Bearer " + accessToken);

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
                Debug.LogError($"Server Response: {webRequest.downloadHandler.text}");

                // json -> 객체로 변환
                DataRes dataRes = JsonUtility.FromJson<DataRes>(webRequest.downloadHandler.text);

                // 토큰이 완료되었다면?
                if (dataRes.message == "JWT가 만료되었습니다.")
                {
                    yield return RequestNewToken(); // 토큰 재발급 요청
                    yield return JWTPostRequest(uri, jsonData, callback); // 요청 재시도
                }
            }
            else
            {
                Debug.Log("Received: " + webRequest.downloadHandler.text);
                callback(webRequest.downloadHandler.text);
            }
        }
    }
    
    public static IEnumerator JWTDeleteRequest(string uri, Action<string> callback)
    {
        string finalUri = BASE_URI + uri;

        string accessToken = Managers.Game.AccessToken;

        using (UnityWebRequest webRequest = new UnityWebRequest(finalUri, "DELETE"))
        {
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", "Bearer " + accessToken);

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);

                // json -> 객체로 변환
                DataRes dataRes = JsonUtility.FromJson<DataRes>(webRequest.downloadHandler.text);

                // 토큰이 만료되었다면?
                if (dataRes.message == "JWT가 만료되었습니다.")
                {
                    yield return RequestNewToken(); // 토큰 재발급 요청
                    yield return JWTDeleteRequest(uri, callback); // 요청 재시도
                }
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
                callback(webRequest.downloadHandler.text);
            }
        }
    }

    #endregion

    #region 안드로이드 플러그인 통신

    private static AndroidJavaObject permissionHelper;
    private static AndroidJavaObject unityActivity;
    private static AndroidJavaObject unityContext;

    public static void InitializePlugin()
    {
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            unityContext = unityActivity.Call<AndroidJavaObject>("getApplicationContext");
        }

        permissionHelper = new AndroidJavaObject("com.ssafy.idlegamearr.PermissionIdleHelper", unityContext);
    }

    public static void CheckAndRequestPermissions()
    {
        if (permissionHelper.Call<bool>("checkPermissions"))
        {
            //StartIdleService();
            Debug.Log("허용 두가지 했음여");
        }
        else
        {
            permissionHelper.Call("showPermissionModal");
            
        }
    }

    public static void StartIdleService()
    {
        using (var idleService = new AndroidJavaClass("com.ssafy.idlegamearr.IdleService"))
        {
            idleService.CallStatic("startService", unityContext);
        }
    }

    public static void StopIdleService()
    {
        using (var idleService = new AndroidJavaClass("com.ssafy.idlegamearr.IdleService"))
        {
            idleService.CallStatic("stopService", unityContext);
        }
    }

    public static void OnServiceStopped(string result)
    {
        Debug.Log("Service stopped: " + result);
        // Here, load your main scene or perform other actions.
    }

    #endregion
}