using System.Security.Cryptography;
using System.Text;

namespace FileHasherAvalonia.Models;

public class StringHasher : Hasher
{
    public StringHasher(HashAlgo algo, string input) : base(algo, input)
    {
        byte[] byteInput = Encoding.Default.GetBytes(input);
        if (algo == HashAlgo.BLAKE3)
        {
            HashResult = Blake3.Hasher.Hash(byteInput).ToString();
        }
        else
        {
            byte[] byteResult = algo switch
            {
                HashAlgo.MD5 => MD5.HashData(byteInput),
                HashAlgo.SHA1 => SHA1.HashData(byteInput),
                HashAlgo.SHA256 => SHA256.HashData(byteInput),
                HashAlgo.SHA512 => SHA512.HashData(byteInput),
                _ => SHA256.HashData(byteInput),
            };
            HashResult = FormatBytes(byteResult);
        }

    }
}
