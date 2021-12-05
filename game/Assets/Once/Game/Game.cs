using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// the game
public class Game: MonoBehaviour {
    // -- lifecycle --
    void Start() {
        Unlock();
    }

    void Update() {
        RunCommands();
    }

    // -- commands --
    /// unlock the game
    void Unlock() {
        StartCoroutine(UnlockAsync());
    }

    /// unlock the game
    IEnumerator UnlockAsync() {
        var unlock = new Unlock();

        // try to unlock the game
        yield return unlock.Call();

        // if success, start the game, otherwise quit
        if (unlock.IsSuccess) {
            Debug.Log("have fun");
        } else {
            Debug.Log("goodbye");
        }
    }

    // quit the game
    void Quit() {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    /// run commands based on inputs
    void RunCommands() {
        var k = Keyboard.current;
        var c = k.ctrlKey;
        var s = k.sKey;

        if (c.isPressed && s.wasPressedThisFrame) {
            Screenshot();
        }
    }

    /// take a screenshot of the scene
    void Screenshot() {
        var app = Application.productName.ToLower();
        ScreenCapture.CaptureScreenshot($"{app}.png");
    }
}
