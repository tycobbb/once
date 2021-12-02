using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// a command to unlock the game
public sealed class Unlock {
    // -- constants --
    // the service host url
    const string k_HostUrl = "https://once-upon.herokuapp.com";
    // const string k_HostUrl = "http://localhost:5000";

    // -- props --
    /// if the request succeeded
    bool m_IsSuccess;

    // -- command --
    /// verify the can run
    public IEnumerator Call() {
        // read the current key from disk
        var curr = UnlockKey.Read();

        // create input for next key from current and a nonce
        var nonce = Guid.NewGuid().ToString("n");
        var next = curr.ComputeNext(nonce);

        // build request attrs
        var url = $"{k_HostUrl}/unlock";
        var json = JsonUtility.ToJson(new UnlockParams(
            nonce,
            next.ToString()
        ));

        // build request
        var unlock = new UnityWebRequest(url, "POST");
        unlock.SetRequestHeader("Content-Type", "application/json");
        unlock.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        unlock.downloadHandler = new DownloadHandlerBuffer();

        // send request
        yield return unlock.SendWebRequest();

        // log errors
        var isSuccess = unlock.responseCode == 200;
        if (!isSuccess) {
            var err = Encoding.UTF8.GetString(unlock.downloadHandler.data);
            Debug.Log($"[unlock] {err}");
        }

        // if success, write the new key to disk
        if (isSuccess) {
            next.Save();
        }
        // otherwise, delete the key
        else {
            next.Delete();
        }

        // update state
        m_IsSuccess = isSuccess;
    }

    // -- queries --
    /// if the request succeeded
    public bool IsSuccess {
        get => m_IsSuccess;
    }

}
