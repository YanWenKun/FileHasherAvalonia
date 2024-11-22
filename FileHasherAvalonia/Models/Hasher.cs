using System;
using System.Text;

namespace FileHasherAvalonia.Models;

public abstract class Hasher(Hasher.HashAlgo algo, string input) // 使用C#12的主构造函数，以省略构造函数 
{
    public enum HashAlgo
    {
        // ReSharper disable InconsistentNaming
        MD5,
        SHA1,
        SHA256,
        SHA512,
        BLAKE3,
        // ReSharper restore InconsistentNaming
    }

    public HashAlgo UsingAlgo { get; } = algo;

    public string Input { get; } = input;

    public string HashResult { get; protected set; } = string.Empty;

    /// <summary>
    /// 将字节数组格式化到字符串
    /// </summary>
    /// <param name="b">byte型数组</param>
    /// <returns>去掉连接符后的十六进制数字符串</returns>
    public static string FormatBytes(byte[] b)
    {
        // 该方法不是解码，而是将HEX“音译”到字符串，1A->"1A"
        string s = BitConverter.ToString(b);
        s = s.Replace("-", string.Empty);
        return s;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("Hash Method: " + UsingAlgo + Environment.NewLine);
        sb.Append("Hash Result: " + HashResult + Environment.NewLine);
        sb.Append("Input: " + Input);
        return sb.ToString();
    }
}
