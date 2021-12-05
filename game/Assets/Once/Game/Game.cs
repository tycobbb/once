using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// the game
public class Game: MonoBehaviour {
    // -- types --
    enum State {
        Door,
        Opening,
        Desk
    }

    // -- nodes --
    [Header("nodes")]
    [Tooltip("the player")]
    [SerializeField] Player m_player;

    [Tooltip("the room")]
    [SerializeField] Room m_Room;

    // -- props --
    /// the game state (machine)
    State m_State = State.Door;

    // -- lifecycle --
    void Update() {
        RunCommands();
    }

    // -- commands --
    /// try to unlock the door
    void TryOpenDoor() {
        StartCoroutine(TryOpenDoorAsync());
    }

    /// try to unlock the door
    IEnumerator TryOpenDoorAsync() {
        // update game state
        m_State = State.Opening;

        // try to unlock the game
        var unlock = new Unlock();
        yield return unlock.Call();

        // if success, start the game, otherwise quit
        if (unlock.IsSuccess) {
            OpenDoor();
        } else {
            Quit();
        }
    }

    // open the door
    void OpenDoor() {
        Debug.Log("have fun");

        // update game state
        m_State = State.Desk;

        // run transitions
        m_Room.Reveal();
    }

    // quit the game
    void Quit() {
        Debug.Log("goodbye");

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

    // -- events --
    /// when the player opens the door
    public void OnOpenDoor() {
        if (m_State == State.Door) {
            TryOpenDoor();
        }
    }
}
