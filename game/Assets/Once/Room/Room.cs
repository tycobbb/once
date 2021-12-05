using UnityEngine;

/// the room
public class Room: MonoBehaviour {
    // -- config --
    [Header("config")]
    [Tooltip("the reveal duration")]
    [SerializeField] float m_RevealDuration = 5.0f;

    [Tooltip("the color when hidden")]
    [SerializeField] Color m_HiddenColor = Color.black;

    [Tooltip("the color when revealed")]
    [SerializeField] Color m_VisibleColor = Color.white;

    // -- nodes --
    [Header("nodes")]
    [Tooltip("the door")]
    [SerializeField] Door m_Door;

    [Tooltip("the ground")]
    [SerializeField] Renderer m_Ground;

    [Tooltip("the camera (for skybox)")]
    [SerializeField] Camera m_Camera;

    // -- lifecycle --
    void Start() {
        ColorLens()
            .Set(m_HiddenColor);
    }

    // -- commands --
    /// reveal the white room
    public void Reveal() {
        m_Door
            .Hide();

        ColorLens()
            .Tween(m_HiddenColor, m_VisibleColor, m_RevealDuration);
    }

    // -- queries --
    /// the lens for the room color
    Lens<Color> ColorLens() {
        return new Lens<Color>(
            ( ) => m_Camera.backgroundColor,
            (v) => {
                m_Camera.backgroundColor = v;
                m_Ground.material.color = v;
            }
        );
    }
}
