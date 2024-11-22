using System;
using System.Buffers;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace FileHasherAvalonia.Models;

public class FileHasher : Hasher
{
    public enum Phase
    {
        Init,
        Running,
        Completed,
        Canceled,
        Error,
    }

    public Phase CurrentPhase { get; private set; }
    public string FilePath { get; }
    public string FileName { get; }
    public long FileLength { get; }

    public long CurrentBytesPosition => GetCurrentBytesPosition();

    // ReSharper disable once InconsistentNaming
    private FileStream FS { get; }

    private CancellationTokenSource CancelSource { get; set; }
    private CancellationToken CancelToken { get; set; }

    /// <summary>
    /// 对文件进行哈希运算
    /// </summary>
    /// <param name="algo">指定哈希算法</param>
    /// <param name="input">指定文件路径</param>
    public FileHasher(HashAlgo algo, string input) : base(algo, input)
    {
        // 获取文件名是纯字符串操作，不会抛出文件系统异常。错误的文件名返回空串
        FilePath = Path.GetFullPath(Input);
        FileName = Path.GetFileName(FilePath);
        CurrentPhase = Phase.Init;

        try
        {
            // 以只读模式打开，不指定进程共享（独占）参数、异步读取参数
            // 因为写在try中，所以不必using(){}的用法，但需要注意Dispose()
            FS = File.OpenRead(FilePath);
            FileLength = FS.Length;
            //if (FileLength == 0) throw new IOException("File length is zero: " + FileName);
            CancelSource = new CancellationTokenSource();
            CancelToken = CancelSource.Token;
        }
        catch
        {
            CurrentPhase = Phase.Error;
            FileLength = 0L;
            // 读取过程中并不隐含Dispose()方法，但是GC会自动回收（有延迟）
            FS?.Dispose();
        }
    }

    /// <summary>
    /// 开始计算文件哈希值
    /// </summary>
    public async Task StartHashFile()
    {
        if (CurrentPhase == Phase.Init)
        {
            try
            {
                CurrentPhase = Phase.Running;

                if (UsingAlgo == HashAlgo.BLAKE3)
                {
                    // https://github.com/xoofx/Blake3.NET/issues/17#issuecomment-1716386501
                    int bufferSize = 4096 * 32;
                    using var hasher = Blake3.Hasher.New();

                    ArrayPool<byte> sharedArrayPool = ArrayPool<byte>.Shared;
                    byte[] buffer = sharedArrayPool.Rent(bufferSize);
                    Array.Fill<byte>(buffer, 0);

                    for (int read; (read = await FS.ReadAsync(buffer, CancelToken)) != 0;)
                    {
                        hasher.Update(buffer.AsSpan(start: 0, read));
                    }

                    HashResult = hasher.Finalize().ToString();
                    sharedArrayPool.Return(buffer);
                }

                else
                {
                    byte[] byteResult = UsingAlgo switch
                    {
                        HashAlgo.MD5 => await MD5.HashDataAsync(FS,CancelToken),
                        HashAlgo.SHA1 => await SHA1.HashDataAsync(FS,CancelToken),
                        HashAlgo.SHA256 => await SHA256.HashDataAsync(FS,CancelToken),
                        HashAlgo.SHA512 => await SHA512.HashDataAsync(FS,CancelToken),
                        _ => await SHA256.HashDataAsync(FS,CancelToken),
                    };
                    HashResult = FormatBytes(byteResult);
                }

                CurrentPhase = Phase.Completed;
            }
            catch (Exception e)
            {
                CurrentPhase = Phase.Error;
#if DEBUG
                Console.WriteLine(e);
#endif
            }
            finally
            {
                await FS.DisposeAsync();
            }
        }
    }

    /// <summary>
    /// 取消当前任务（如果任务存在）
    /// </summary>
    public void Stop()
    {
        if (CurrentPhase == Phase.Running)
        {
            CancelSource.Cancel();
            CurrentPhase = Phase.Canceled;
            FS.Dispose();
        }
    }

    /// <summary>
    /// 获取当前文件读取字节位置，如异常则返回文件长度（默认为0）
    /// </summary>
    private long GetCurrentBytesPosition()
    {
        // 无法直接得知IDisposable是否已被Dispose()，可catch异常，
        // 或额外用个bool挂旗，或进一步override Dispose()方法等
        try
        {
            return FS.Position;
        }
        catch
        {
            return FileLength;
        }
    }

}
