using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// the game
public class Game: MonoBehaviour {
    // -- lifecycle --
    void Start() {
        var verify = new Verify();
        verify.Call();
    }

    void Update() {
        RunCommands();
    }

    // -- commands --
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
