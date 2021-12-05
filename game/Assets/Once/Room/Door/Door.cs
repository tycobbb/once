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
    [Tooltip("the door's ambient noise")]
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

    // -- commands --
    /// hide the door
    public void Hide() {
        AlphaLens()
            .Tween(1.0f, 0.0f, m_HideDuration)
            .OnComplete(() => {
                Destroy(gameObject);
            });
    }

    // -- queries --
    /// find all the door materials
    Material[] FindMaterials() {
        var rs = GetComponentsInChildren<Renderer>();
        var ms = new Material[rs.Length];

        for (var i = 0; i < rs.Length; i++) {
            ms[i] = rs[i].material;
        }

        return ms;
    }

    /// a lens for the door's alpha
    Lens<float> AlphaLens() {
        return new Lens<float>(
            ( ) => m_Materials[0].color.a,
            (v) => {
                // fade the materials
                foreach (var m in m_Materials) {
                    var c = m.color;
                    c.a = v;
                    m.color = c;
                }

                // and audio
                m_Ambient.SetMaxVolume(v);
            }
        );
    }
}
