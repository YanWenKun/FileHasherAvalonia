using System.Security.Cryptography;
using System.Text;

namespace FileHasherAvalonia.Models;

public class StringHasher : Hasher
{
    public StringHasher(HashAlgo algo, string input) : base(algo, input)
    {
        byte[] byteArr = Encoding.Default.GetBytes(input);
        HashResult = algo switch
        {
            HashAlgo.MD5 => FormatBytes(MD5.Create().ComputeHash(byteArr)),
            HashAlgo.SHA1 => FormatBytes(SHA1.Create().ComputeHash(byteArr)),
            HashAlgo.SHA256 => FormatBytes(SHA256.Create().ComputeHash(byteArr)),
            HashAlgo.SHA512 => FormatBytes(SHA512.Create().ComputeHash(byteArr)),
            _ => FormatBytes(SHA256.Create().ComputeHash(byteArr)),
        };
    }
}
