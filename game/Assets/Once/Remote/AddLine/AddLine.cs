using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// a factory for add line request
public sealed class AddLine {
    // -- command --
    /// create an unlock request
    public static Remote<AddLineParams, Nothing> Request(
        string text,
        Transform transform
    ) {
        return new Remote<AddLineParams, Nothing>(
            "/unlock/line",
            new AddLineParams(
                line: new RemoteLine(
                    text,
                    transform
                )
            )
        );
    }
}
