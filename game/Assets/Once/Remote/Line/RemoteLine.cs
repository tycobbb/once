using System;
using UnityEngine;

/// a serializable line
[Serializable]
public struct RemoteLine {
    /// the line text
    public string Text;

    /// the line's world position
    public Vector3 Pos;

    /// the line's world rotation
    public Quaternion Rot;

    // -- lifetime --
    /// create a line from a text and transform
    public RemoteLine(string text, Transform transform) {
        Text = text;
        Pos = transform.position;
        Rot = transform.rotation;
    }
}