using System;

/// the request params for a key
[Serializable]
struct RemoteKeyParams {
    /// the nonce as a base64 string
    public string Nonce;

    /// the next key as a base64 string
    public string Next;

    // -- lifetime --
    public RemoteKeyParams(string nonce, string next) {
        Nonce = nonce;
        Next = next;
    }
}