using System;

namespace FileHasherAvalonia.Models;

public abstract class Hasher(Hasher.HashAlgo algo, string input) // 使用C#12的主构造函数，以省略构造函数 
{
    public enum HashAlgo
    {
        // ReSharper disable InconsistentNaming
        MD5,
        SHA1,
        SHA256,
        SHA512
    }

    public HashAlgo UsingAlgo { get; } = algo;

    public string Input { get; } = input;

    public string HashResult { get; protected set; } = "";

    // 构造函数中的自动属性

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
}
