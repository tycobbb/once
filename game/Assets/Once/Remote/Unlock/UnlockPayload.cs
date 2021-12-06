using System;

/// the unlock response payload
[Serializable]
public struct UnlockPayload {
    /// the list of lines
    public RemoteLine[] Lines;
}