using System;
using UnityEngine;

/// the add line request params
[Serializable]
public struct AddLineParams {
    /// a line
    public RemoteLine Line;

    // -- lifetime --
    public AddLineParams(RemoteLine line) {
        Line = line;
    }
}