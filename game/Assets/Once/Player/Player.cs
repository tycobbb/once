using Hertzole.GoldPlayer;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// the player
public class Player: MonoBehaviour {
    // -- config --
    [Header("config")]
    [Tooltip("the text prefab")]
    [SerializeField] GameObject m_TextPrefab;

    // -- tuning --
    [Header("tuning")]
    [Tooltip("the distance to write text at")]
    [SerializeField] float m_WritingDistance;

    // -- nodes --
    [Header("nodes")]
    [Tooltip("the target for writing")]
    [SerializeField] Transform m_WritingTarget;

    [Tooltip("the gold player controller")]
    [SerializeField] GoldPlayerController m_Controller;

    [Tooltip("the gold player input")]
    [SerializeField] GoldPlayerInputSystem m_Input;

    [Tooltip("the interaction config")]
    [SerializeField] GoldPlayerInteraction m_Interaction;

    // -- props --
    /// the keyboard
    Keyboard m_Keyboard;

    /// the active text, if any
    TMP_Text m_TextInput;

    // -- lifecycle --
    void Awake() {
        // set props
        m_Keyboard = Keyboard.current;
    }

    void Start() {
        var t = m_WritingTarget;

        // move the writing target to the interaction distance
        var p = t.localPosition;
        p.z = m_Interaction.InteractionRange;
        t.localPosition = p;

        // and disable it until we have the typewriter
        t.SetActive(false);
    }

    void Update() {
        FinishLine();
    }

    // -- commands --
    /// grab the typewriter and start writing
    public void GrabTypewriter() {
        m_WritingTarget.SetActive(true);
    }

    /// start writing a line of text
    public void StartLine() {
        // create a new text object
        var d = m_WritingTarget;
        var text = Instantiate(
            m_TextPrefab,
            d.position,
            d.rotation,
            d.parent
        );

        // grab the transform
        var t = text.transform;

        // move text to the writing distance
        var p = t.localPosition;
        p.z = m_WritingDistance;
        t.localPosition = p;

        // update state
        m_TextInput = text.GetComponent<TMP_Text>();

        // switch to text input
        // m_Controller.enabled = false;
        m_Input.enabled = false;
        m_Keyboard.onTextInput += OnTextInput;
    }

    /// finish the line of text
    void FinishLine() {
        // if there is a line of text
        var text = m_TextInput;
        if (text == null) {
            return;
        }

        // and enter is just pressed
        if (!m_Keyboard.enterKey.wasPressedThisFrame) {
            return;
        }

        // move text to root
        var t = text.transform;
        var p = t.position;
        t.SetParent(null, true);
        t.position = p;

        // update state
        m_TextInput = null;

        // switch to movement
        m_Input.enabled = true;
        // m_Controller.enabled = true;
        m_Keyboard.onTextInput -= OnTextInput;
    }

    // -- events --
    /// when text is entered during typing
    void OnTextInput(char ch) {
        m_TextInput.text += ch;
    }
}
