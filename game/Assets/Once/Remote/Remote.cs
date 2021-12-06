using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// a command to unlock the game
public sealed class Remote<I, O> {
    // -- constants --
    // the service host url
    #if false
    const string k_HostUrl = "http://localhost:5000";
    #else
    const string k_HostUrl = "https://once-upon.herokuapp.com";
    #endif

    // -- props --
    /// the request path
    string m_Path;

    /// the request data
    I m_Params;

    /// the response data
    O m_Payload;

    /// if the request succeeded
    bool m_IsSuccess;

    // -- lifetime --
    /// create a new request
    public Remote(string path, I @params) {
        m_Path = path;
        m_Params = @params;
    }

    // -- command --
    /// verify the can run
    public IEnumerator Call() {
        Debug.Log($"[req {m_Path}] starting...");

        // read the current key from disk
        var curr = RemoteKey.Read();

        // create input for next key from current and a nonce
        var nonce = Guid.NewGuid().ToString("n");
        var next = curr.ComputeNext(nonce);

        // build request attrs
        var url = $"{k_HostUrl}{m_Path}";
        var json = JsonUtility.ToJson(new RemoteParams<I>(
            m_Params,
            new RemoteKeyParams(
                nonce,
                next.ToString()
            )
        ));

        // build request
        var req = new UnityWebRequest(url, "POST");
        req.SetRequestHeader("Content-Type", "application/json");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();

        // send request
        yield return req.SendWebRequest();

        // get response data
        var str = Encoding.UTF8.GetString(req.downloadHandler.data);

        // log errors
        var isSuccess = req.responseCode == 200;
        if (!isSuccess) {
            Debug.Log($"[req {m_Path}] {str}");
        } else {
            m_Payload = JsonUtility.FromJson<RemotePayload<O>>(str).Data;
            Debug.Log($"[req {m_Path}] success");
        }

        // if success, write the new key to disk
        if (isSuccess) {
            next.Save();
            Debug.Log($"[req {m_Path}] saved next key");
        }

        // update state
        m_IsSuccess = isSuccess;
    }

    // -- queries --
    /// if the request succeeded
    public bool IsSuccess {
        get => m_IsSuccess;
    }

    /// the response payload, if any
    public O Payload {
        get => m_Payload;
    }
}
