using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// the game
public class Game: MonoBehaviour {
    // -- types --
    enum State {
        Door,
        Opening,
        Typewriter,
        Writing,
    }

    // -- nodes --
    [Header("nodes")]
    [Tooltip("the player")]
    [SerializeField] Player m_Player;

    [Tooltip("the room")]
    [SerializeField] Room m_Room;

    // -- props --
    /// the game state (machine)
    State m_State = State.Door;

    // -- lifecycle --
    // void Start() {
    //     OpenDoor();
    //     GrabTypewriter();
    // }

    void Update() {
        RunCommands();
    }

    // -- commands --
    /// try to unlock the room
    void TryUnlockRoom() {
        StartCoroutine(TryUnlockRoomAsync());
    }

    /// try to unlock the room
    IEnumerator TryUnlockRoomAsync() {
        // update game state
        m_State = State.Opening;

        // try the unlock service
        var unlock = new Unlock();
        yield return unlock.Call();

        // if success, start the game, otherwise quit
        if (unlock.IsSuccess) {
            OpenDoor();
        } else {
            Quit();
        }
    }

    /// open the door
    void OpenDoor() {
        Debug.Log("have fun");

        // update game state
        m_State = State.Typewriter;

        // update entities
        m_Room.Unlock();
    }

    /// grab the typewriter
    void GrabTypewriter() {
        // update game state
        m_State = State.Writing;

        // update entities
        m_Room.HideTypewriter();
        m_Player.GrabTypewriter();
    }

    /// start writing a new line of text
    void StartLine() {
        m_Player.StartLine();
    }

    /// quit the game
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
            TryUnlockRoom();
        }
    }

    /// when the player grabs the typewriter
    public void OnGrabTypewriter() {
        if (m_State == State.Typewriter) {
            GrabTypewriter();
        }
    }

    /// when the player adds a line of text
    public void OnStartLine() {
        if (m_State == State.Writing) {
            StartLine();
        }
    }
}
