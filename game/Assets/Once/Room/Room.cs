using DG.Tweening;
using UnityEngine;

/// the room
public class Room: MonoBehaviour {
    // -- config --
    [Header("config")]
    [Tooltip("the reveal duration")]
    [SerializeField] float m_RevealDuration = 5.0f;

    [Tooltip("the reveal duration")]
    [SerializeField] float m_ItemFadeDuration = 2.0f;

    [Tooltip("the color when hidden")]
    [SerializeField] Color m_HiddenColor = Color.black;

    [Tooltip("the color when revealed")]
    [SerializeField] Color m_VisibleColor = Color.white;

    // -- nodes --
    [Header("nodes")]
    [Tooltip("the door")]
    [SerializeField] Door m_Door;

    [Tooltip("the desk")]
    [SerializeField] RoomItem m_Desk;

    [Tooltip("the typewriter")]
    [SerializeField] RoomItem m_Typewriter;

    [Tooltip("the camera (for skybox)")]
    [SerializeField] Camera m_Camera;

    // -- lifecycle --
    void Start() {
        ColorLens()
            .Set(m_HiddenColor);

        m_Desk.SetActive(false);
        m_Typewriter.SetActive(false);
    }

    // -- commands --
    /// reveal the white room
    public void Unlock() {
        // fade background
        ColorLens()
            .Tween(m_HiddenColor, m_VisibleColor, m_RevealDuration);

        // hide doown
        m_Door.AlphaLens()
            .Tween(1.0f, 0.0f, m_ItemFadeDuration)
            .OnComplete(() => {
                Destroy(m_Door.gameObject);
            });

        // show desk and typewriter
        var delay = m_RevealDuration - m_ItemFadeDuration;
        m_Desk.SetActive(true);
        m_Desk.AlphaLens()
            .Tween(0.0f, 1.0f, m_ItemFadeDuration)
            .SetDelay(delay);

        m_Typewriter.SetActive(true);
        m_Typewriter.AlphaLens()
            .Tween(0.0f, 1.0f, m_ItemFadeDuration)
            .SetDelay(delay);
    }

    /// grab the typewriter off the desk
    public void HideTypewriter() {
        m_Typewriter.AlphaLens()
            .Tween(1.0f, 0.0f, m_ItemFadeDuration)
            .OnComplete(() => {
                m_Typewriter.SetActive(false);
            });
    }

    // -- queries --
    /// the lens for the room color
    Lens<Color> ColorLens() {
        return new Lens<Color>(
            ( ) => m_Camera.backgroundColor,
            (v) => m_Camera.backgroundColor = v
        );
    }
}
