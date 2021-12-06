using Hertzole.GoldPlayer;
using UnityEngine;

/// the player
public class Player: MonoBehaviour {
    // -- nodes --
    [Header("nodes")]
    [Tooltip("the target for writing")]
    [SerializeField] Transform m_WritingTarget;

    [Tooltip("desc")]
    [SerializeField] GoldPlayerInteraction m_Interaction;

    // -- lifecycle --
    void Start() {
        var t = m_WritingTarget;

        // move the writing target to the interaction distance
        var p = t.localPosition;
        p.z = m_Interaction.InteractionRange;
        t.localPosition = p;

        // and disable it until we have the typewriter
        t.SetActive(false);
    }

    /// -- commands --
    /// grab the typewriter and start writing
    public void GrabTypewriter() {
        m_WritingTarget.SetActive(true);
    }
}
