using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class WebManager
{
    public string BaseUrl { get; set; } = "https://localhost:5001/api";

    public void SendPostRequest<T>(string url, object obj, Action<T> res)
    {
        Managers.Instance.StartCoroutine(CoSendWebRequest(url, UnityWebRequest.kHttpVerbPOST, obj, res));
    }

    // callback action (res)
    // 1. baseurl 끝 부분 2. 메시지 형태 get, post ... 3. 보내줄 json object 4. 응답 완료 call back
    IEnumerator CoSendWebRequest<T>(string url, string method, object obj, Action<T> res)
    {
        string sendUrl = $"{BaseUrl}/{url}";

        byte[] jsonBytes = null;
        if(obj != null)
        {
            string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            jsonBytes = Encoding.UTF8.GetBytes(jsonStr);
        }

        using (var uwr = new UnityWebRequest(sendUrl, method))
        {
            uwr.uploadHandler = new UploadHandlerRaw(jsonBytes);
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");

            yield return uwr.SendWebRequest();

            // 코루틴 이용, 응답이 온 후 실행
            if(uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                // 제이슨 형식의 웹 답장을 웹 패킷에 담아 콜백 핸들러 호출
                T resObj = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(uwr.downloadHandler.text);
                res.Invoke(resObj);
            }

        }
    }
}
