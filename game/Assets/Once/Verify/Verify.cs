using System;
using System.Security.Cryptography;
using System.IO;
using System.Linq;

/// a command to verify the validity of the game
public sealed class Verify {
    // -- constants --
    // the directory to save keys
    const string k_KeyDir = "Keys";
    const string k_KeyCurrent = "Current.txt";

    // -- command --
    /// verify the can run
    public void Call() {
        var path = Path.Combine(k_KeyDir, k_KeyCurrent);

        // read the current hash from disk
        string curr;
        if (File.Exists(path)) {
            curr = File.ReadAllText(path);
        } else {
            curr = "";
        }

        // create input for next hash from current and a nonce
        var nonce = Guid.NewGuid().ToString("n");
        var input = Convert.FromBase64String(curr)
            .Concat(Convert.FromBase64String(nonce))
            .ToArray();

        // compute the next hash
        var sha = SHA256.Create();
        var next = sha.ComputeHash(input);

        // create the key dir if necessary
        Directory.CreateDirectory(k_KeyDir);

        // write the new hash to disk
        var s = Convert.ToBase64String(next);
        var f = File.CreateText(path);
        f.WriteLine(s);
        f.Close();
    }
}
