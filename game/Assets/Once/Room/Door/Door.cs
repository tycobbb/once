using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

/// the door that unlocks the game
public class Door: MonoBehaviour {
    // -- nodes --
    [Header("nodes")]
    [Tooltip("the door as a room item")]
    [SerializeField] RoomItem m_Item;

    [Tooltip("the ambient noise")]
    [SerializeField] Musicker m_Ambient;

    // -- lifecycle --
    void Start() {
        // play audio
        m_Ambient.PlayLoop(new Loop(
            fade: 1.5f,
            blend: 0.6f,
            Tone.I
        ));
    }

    // -- queries --
    /// a lens for the door's alpha
    public Lens<float> AlphaLens() {
        return m_Item.AlphaLens()
            .Map(
                (v) => v,
                (v) => {
                    m_Ambient.SetMaxVolume(v);
                    return v;
                }
            );
    }
}
