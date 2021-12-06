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
    [Tooltip("the key")]
    [SerializeField] RoomItem m_Key;

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

        // hide key & door
        HideItem(m_Key, destroy: true);
        HideItem(m_Door, destroy: true);

        // show desk & typewriter
        ShowItem(m_Desk).SetDelay(RevealDelay);
        ShowItem(m_Typewriter).SetDelay(RevealDelay);
    }

    /// grab the typewriter off the desk
    public void HideTypewriter() {
        HideItem(m_Typewriter);
    }

    public void ShowTypewriter() {
        ShowItem(m_Typewriter);
    }

    /// show the item
    Tweener ShowItem<I>(I item) where I: Component, IRoomItem {
        item.SetActive(true);

        var tween = item
            .AlphaLens()
            .Tween(0.0f, 1.0f, m_ItemFadeDuration);

        return tween;
    }

    /// hide the item
    Tweener HideItem<I>(I item, bool destroy = false) where I: Component, IRoomItem {
        var tween = item
            .AlphaLens()
            .Tween(1.0f, 0.0f, m_ItemFadeDuration)
            .OnComplete(() => {
                if (destroy) {
                    Destroy(item.gameObject);
                } else {
                    item.SetActive(false);
                }
            });

        return tween;
    }

    // -- queries --
    /// the delay until showing objects
    public float RevealDelay {
        get => m_RevealDuration - m_ItemFadeDuration;
    }

    /// the delay until showing objects
    public float ItemFadeDuration {
        get => m_ItemFadeDuration;
    }

    /// the lens for the room color
    Lens<Color> ColorLens() {
        return new Lens<Color>(
            ( ) => m_Camera.backgroundColor,
            (v) => m_Camera.backgroundColor = v
        );
    }
}
