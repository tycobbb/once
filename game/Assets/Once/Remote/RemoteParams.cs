using System;

/// params for a network request
[Serializable]
struct RemoteParams<T> {
    /// the params for a specific request
    public T Data;

    /// the key params
    public RemoteKeyParams Key;

    // -- lifetime --
    /// create new request params
    public RemoteParams(T data, RemoteKeyParams key) {
        Key = key;
        Data = data;
    }
}