using System;

/// the unlock response payload
[Serializable]
public struct RemotePayload<T> {
    /// the payload data
    public T Data;
}