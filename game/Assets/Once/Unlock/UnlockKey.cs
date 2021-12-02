using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

/// an unlock key
public struct UnlockKey {
    // -- constants --
    /// the directory to save keys
    const string k_KeyDir = "Keys";

    /// the path to the current key file
    const string k_KeyCurrent = "Current.txt";

    // -- props --
    /// the string value
    string m_Val;

    // -- lifetime --
    /// create a new key
    public UnlockKey(string val) {
        m_Val = val;
    }

    // -- commands --
    /// save to disk
    public void Save() {
        // create the dir if necessary
        Directory.CreateDirectory(k_KeyDir);

        // write the file
        var f = File.CreateText(FindPath());
        f.WriteLine(m_Val);
        f.Close();
    }

    /// delete the key on disk
    public void Delete() {
        var path = FindPath();
        if (File.Exists(path)) {
            File.Delete(path);
        }
    }

    // -- queries --
    /// compute the next key
    public UnlockKey ComputeNext(string nonce) {
        // build input from value and nonce
        var input = Convert.FromBase64String(m_Val)
            .Concat(Convert.FromBase64String(nonce))
            .ToArray();

        // compute the next key
        var sha = SHA256.Create();
        var next = Convert.ToBase64String(sha.ComputeHash(input));

        return new UnlockKey(next);
    }

    /// get the path on disk
    static string FindPath() {
        return Path.Combine(k_KeyDir, k_KeyCurrent);
    }

    /// the string value
    public override string ToString() {
        return m_Val;
    }

    // -- factories --
    /// verify the can run
    public static UnlockKey Read() {
        var path = FindPath();

        // read the current key from disk
        string val;
        if (File.Exists(path)) {
            val = File.ReadAllText(path);
        } else {
            val = "";
        }

        return new UnlockKey(val);
    }
}