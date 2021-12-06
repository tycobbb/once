using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// a thing in the room (that can be faded)
public class RoomItem: MonoBehaviour {
    // -- props --
    /// the door materials
    Material[] m_Materials;

    // -- props --
    void Awake() {
        // set props
        m_Materials = FindMaterials();
    }

    // -- props/hot --
    /// a lens for the door's alpha
    public Lens<float> AlphaLens() {
        return new Lens<float>(
            ( ) => m_Materials[0].color.a,
            (v) => {
                // fade the materials
                foreach (var m in m_Materials) {
                    var c = m.color;
                    c.a = v;
                    m.color = c;
                }
            }
        );
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

}
