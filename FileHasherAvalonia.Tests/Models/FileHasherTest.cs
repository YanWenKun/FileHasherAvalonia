using FileHasherAvalonia.Models;

namespace FileHasherAvalonia.Tests.Models;

[TestFixture]
[TestOf(typeof(FileHasher))]
public class FileHasherTest
{
    private string _tempFolder;
    private string _smallFile;
    private string _largeFile;

    // 创建临时文件用于哈希
    [SetUp]
    public void Setup()
    {
        _tempFolder = Path.GetTempFileName();
        File.Delete(_tempFolder);
        Directory.CreateDirectory(_tempFolder);

        _smallFile = Path.Combine(_tempFolder, "small_file.txt");
        _largeFile = Path.Combine(_tempFolder, "large_file.dat");

        byte[] fileBytes = "Hello World!"u8.ToArray();
        File.WriteAllBytes(_smallFile, fileBytes);

        long largeFileSize = 1L * 1024 * 1024 * 1024; // 1GiB
        byte fillByte = 0x0B;

        FileStream fs = new FileStream(_largeFile, FileMode.CreateNew);
        fs.Seek(largeFileSize, SeekOrigin.Begin);
        fs.WriteByte(fillByte);
        fs.Close();
    }

    [TearDown]
    public void Cleanup()
    {
        Directory.Delete(_tempFolder, true);
    }

    [Test]
    public async Task TestSmallFile()
    {
        var fh = new FileHasher(Hasher.HashAlgo.MD5, _smallFile);
        await fh.StartHashFile();
        await TestContext.Out.WriteLineAsync(fh.ToString());
        Assert.That(fh.HashResult, Is.EqualTo("ED076287532E86365E841E92BFC50D8C"));

        fh = new FileHasher(Hasher.HashAlgo.SHA1, _smallFile);
        await fh.StartHashFile();
        await TestContext.Out.WriteLineAsync(fh.ToString());
        Assert.That(fh.HashResult, Is.EqualTo("2EF7BDE608CE5404E97D5F042F95F89F1C232871"));

        fh = new FileHasher(Hasher.HashAlgo.SHA256, _smallFile);
        await fh.StartHashFile();
        await TestContext.Out.WriteLineAsync(fh.ToString());
        Assert.That(fh.HashResult, Is.EqualTo("7F83B1657FF1FC53B92DC18148A1D65DFC2D4B1FA3D677284ADDD200126D9069"));

        fh = new FileHasher(Hasher.HashAlgo.SHA512, _smallFile);
        await fh.StartHashFile();
        await TestContext.Out.WriteLineAsync(fh.ToString());
        Assert.That(fh.HashResult,
            Is.EqualTo(
                "861844D6704E8573FEC34D967E20BCFEF3D424CF48BE04E6DC08F2BD58C729743371015EAD891CC3CF1C9D34B49264B510751B1FF9E537937BC46B5D6FF4ECC8"));

        fh = new FileHasher(Hasher.HashAlgo.BLAKE3, _smallFile);
        await fh.StartHashFile();
        await TestContext.Out.WriteLineAsync(fh.ToString());
        Assert.That(fh.HashResult, Is.EqualTo("5ca7815adcb484e9a136c11efe69c1d530176d549b5d18d038eb5280b4b3470c"));
    }

    [Test]
    public async Task TestLargeFile()
    {
        var fh = new FileHasher(Hasher.HashAlgo.MD5, _largeFile);
        await fh.StartHashFile();
        await TestContext.Out.WriteLineAsync(fh.ToString());
        Assert.That(fh.HashResult, Is.EqualTo("60A7C208F65E84C0DEC3B5FB9B8F6806"));

        fh = new FileHasher(Hasher.HashAlgo.SHA1, _largeFile);
        await fh.StartHashFile();
        await TestContext.Out.WriteLineAsync(fh.ToString());
        Assert.That(fh.HashResult, Is.EqualTo("BA1CFC181D779665C185E62607EF0D99697DE29D"));

        fh = new FileHasher(Hasher.HashAlgo.SHA256, _largeFile);
        await fh.StartHashFile();
        await TestContext.Out.WriteLineAsync(fh.ToString());
        Assert.That(fh.HashResult, Is.EqualTo("C2160C164C0037DE5D5A5EA92B04C1B70D6F2C70706A21BD382488A1E465384A"));

        fh = new FileHasher(Hasher.HashAlgo.SHA512, _largeFile);
        await fh.StartHashFile();
        await TestContext.Out.WriteLineAsync(fh.ToString());
        Assert.That(fh.HashResult,
            Is.EqualTo(
                "41C603C9D6CC0F1A58A9C28E414DEB4096E630B3D23D9A391B1FAFDC0D15364E9F9B77DAE5FB997A297CC557ED791D59B998D571D67B1499F113D6E0DBC1CF09"));

        fh = new FileHasher(Hasher.HashAlgo.BLAKE3, _largeFile);
        await fh.StartHashFile();
        await TestContext.Out.WriteLineAsync(fh.ToString());
        Assert.That(fh.HashResult, Is.EqualTo("fcd8dea6f75fb7608d7e6dbedf9d91bac4bdad5f6a60d40e686e0b34556506a5"));
    }
}
