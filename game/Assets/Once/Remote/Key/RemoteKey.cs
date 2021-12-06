using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

/// a key for unlock requests
public struct RemoteKey {
    // -- statics --
    static string s_TempKey = null;

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
    public RemoteKey(string val) {
        m_Val = val;
    }

    // -- commands --
    /// save to disk
    public void Save(bool temp = false) {
        // if temp, store in-memory
        if (temp) {
            s_TempKey = m_Val;
            return;
        }

        // otherwise, destroy the temp key
        s_TempKey = null;

        // create dir if necessary
        Directory.CreateDirectory(FindDir());

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
    public RemoteKey ComputeNext(string nonce) {
        // build input from value and nonce
        var input = Convert.FromBase64String(m_Val)
            .Concat(Convert.FromBase64String(nonce))
            .ToArray();

        // compute the next key
        var sha = SHA256.Create();
        var next = Convert.ToBase64String(sha.ComputeHash(input));

        return new RemoteKey(next);
    }

    /// get the path to the key dir on disk
    static string FindDir() {
        return Path.Combine(Application.persistentDataPath, k_KeyDir);
    }

    /// get the path on disk
    static string FindPath() {
        return Path.Combine(FindDir(), k_KeyCurrent);
    }

    /// the string value
    public override string ToString() {
        return m_Val;
    }

    // -- factories --
    /// create a new key from disk (or temp key if available)
    public static RemoteKey Read() {
        // if temp key is available use that
        if (s_TempKey != null) {
            return new RemoteKey(s_TempKey);
        }

        var path = FindPath();

        // read the current key from disk
        string val;
        if (File.Exists(path)) {
            val = File.ReadAllText(path);
        } else {
            val = "";
        }

        return new RemoteKey(val);
    }
}
