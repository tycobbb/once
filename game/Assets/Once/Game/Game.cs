using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// the game
public class Game: MonoBehaviour {
    // -- types --
    enum State {
        Door,
        Opening,
        Typewriter,
        Grabbing,
        Writing,
        Finished,
    }

    // -- config --
    [Header("config")]
    [Tooltip("the line prefab")]
    [SerializeField] GameObject m_LinePrefab;

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
    /// pick up the key
    void PickUpKey() {
        var str = GUIUtility.systemCopyBuffer;
        var key = new RemoteKey(str);
        key.Save(temp: true);
        Debug.Log($"[game] save key from clipboard {str}");
    }

    /// try to unlock the room
    void TryUnlockRoom() {
        StartCoroutine(TryUnlockRoomAsync());
    }

    /// try to unlock the room
    IEnumerator TryUnlockRoomAsync() {
        // update game state
        m_State = State.Opening;

        // try the unlock service
        var unlock = Unlock.Request();
        yield return unlock.Call();

        // if it failed, quit
        if (!unlock.IsSuccess) {
            Quit();
        }
        // otherwise, start the game
        else {
            UnlockRoom();
            ShowLines(unlock.Payload.Lines);
        }
    }

    /// open the door
    void UnlockRoom() {
        Debug.Log("have fun");

        // update game state
        m_State = State.Typewriter;

        // update entities
        m_Room.Unlock();
    }

    /// grab the typewriter
    void GrabTypewriter() {
        StartCoroutine(GrabTypewriterAsync());
    }

    /// grab the typewriter
    IEnumerator GrabTypewriterAsync() {
        // update game state
        m_State = State.Grabbing;

        // update entities
        m_Room.HideTypewriter();
        m_Player.GrabTypewriter();

        yield return new WaitForSeconds(3.0f);
        m_State = State.Writing;
    }

    /// start writing a new line of text
    void StartLine() {
        m_Player.StartLine();
    }

    /// return the typewriter and end the game
    void ReturnTypewriter() {
        StartCoroutine(ReturnTypewriterAsync());
    }

    /// return the typewriter and end the game
    IEnumerator ReturnTypewriterAsync() {
        // copy key to clipboard
        var key = RemoteKey.Read();
        GUIUtility.systemCopyBuffer = key.ToString();

        // show typewriter again
        m_Room.ShowTypewriter();

        // and then quit
        yield return new WaitForSeconds(m_Room.ItemFadeDuration);
        Quit();
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

    /// add remote lines to scene
    void ShowLines(RemoteLine[] lines) {
        StartCoroutine(ShowLinesAsync(lines));
    }

    /// add remote lines to scene
    IEnumerator ShowLinesAsync(RemoteLine[] lines) {
        // wait for stuff to start fading in
        yield return new WaitForSeconds(m_Room.RevealDelay);

        // slowly instantiate all the lines
        var i = 0;
        foreach (var line in lines) {
            // create a line
            var obj = Instantiate(m_LinePrefab, line.Pos, line.Rot);
            var txt = obj.GetComponent<TMP_Text>();
            txt.text = line.Text;

            // tween in the alpha
            var alpha = new Lens<float>(
                ( ) => txt.alpha,
                (v) => txt.alpha = v
            );

            alpha.Tween(0.0f, 1.0f, 1.0f);

            // wait a little for the next one
            i++;
            yield return new WaitForSeconds(0.1f);
        }
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
    /// when the player picks up the key
    public void OnPickUpKey() {
        if (m_State == State.Door) {
            PickUpKey();
        }
    }

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
        } else if (m_State == State.Writing) {
            ReturnTypewriter();
        }
    }

    /// when the player adds a line of text
    public void OnStartLine() {
        if (m_State > State.Typewriter) {
            StartLine();
        }
    }
}
