using EasyRsaPem;
using UnityEngine;
using System.Security.Cryptography;
using System.IO;

/// a command to verify the validity of the game
public sealed class Verify {
    // -- constants --
    // the directory to save keys
    const string k_KeyDir = "Keys";

    // -- command --
    /// verify the can run
    public void Call() {
        var csp = new RSACryptoServiceProvider();
        Save(csp);
    }

    // -- commands --
    /// save the rsa key to disk
    void Save(RSACryptoServiceProvider csp) {
        Save("upon-key", Crypto.ExportPrivateKeyToRSAPEM(csp));
        Save("upon-key.pub", Crypto.ExportPublicKeyToRSAPEM(csp));
    }

    void Save(string filename, string text) {
        // create the directory
        Directory.CreateDirectory(k_KeyDir);

        // build the path
        var path = Path.Combine(k_KeyDir, filename);
        Debug.Log($"saving {path}\n{text}");

        // write the file
        var f = File.CreateText(path);
        f.WriteLine(text);
        f.Close();
    }

    // -- queries --

}
