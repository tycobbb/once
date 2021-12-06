using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// a factory for unlock requests
public static class Unlock {
    // -- command --
    /// create an unlock request
    public static Remote<Nothing, UnlockPayload> Request() {
        return new Remote<Nothing, UnlockPayload>(
            "/unlock",
            default
        );
    }
}
