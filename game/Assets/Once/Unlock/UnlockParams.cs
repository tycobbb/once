using System;

/// the unlock request params
[Serializable]
struct UnlockParams {
    /// the nonce as a base64 string
    public string Nonce;

    /// the next key as a base64 string
    public string NextKey;

    // -- lifetime --
    public UnlockParams(string nonce, string nextKey) {
        Nonce = nonce;
        NextKey = nextKey;
    }
}