using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

/// the door that unlocks the game
public class Door: MonoBehaviour {
    // -- config --
    [Header("config")]
    [Tooltip("the duration of the hide")]
    [SerializeField] float m_HideDuration;

    // -- nodes --
    [Header("nodes")]
    [Tooltip("the door as a room item")]
    [SerializeField] RoomItem m_Item;

    [Tooltip("the ambient noise")]
    [SerializeField] Musicker m_Ambient;

    // -- props --
    /// the door materials
    Material[] m_Materials;

    // -- props --
    void Awake() {
        // set props
        m_Materials = FindMaterials();
    }

    void Start() {
        // play audio
        m_Ambient.PlayLoop(new Loop(
            fade: 1.5f,
            blend: 0.6f,
            Tone.I
        ));
    }

    // -- queries --
    /// find all unique door materials
    Material[] FindMaterials() {
        var ms = new HashSet<Material>();

        var rs = GetComponentsInChildren<Renderer>();
        foreach (var r in rs) {
            ms.Add(r.material);
        }

        return ms.ToArray();
    }

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
