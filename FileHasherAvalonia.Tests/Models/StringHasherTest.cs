using FileHasherAvalonia.Models;

namespace FileHasherAvalonia.Tests.Models;

[TestFixture]
[TestOf(typeof(StringHasher))]
public class StringHasherTest
{

    [Test]
    public void Ctor_Hash()
    {
        var sh = new StringHasher(Hasher.HashAlgo.MD5, "123456");
        Assert.That(sh.HashResult, Is.EqualTo("E10ADC3949BA59ABBE56E057F20F883E"));

        sh = new StringHasher(Hasher.HashAlgo.SHA1, "123456");
        Assert.That(sh.HashResult, Is.EqualTo("7C4A8D09CA3762AF61E59520943DC26494F8941B"));

        sh = new StringHasher(Hasher.HashAlgo.SHA256, "123456");
        Assert.That(sh.HashResult, Is.EqualTo("8D969EEF6ECAD3C29A3A629280E686CF0C3F5D5A86AFF3CA12020C923ADC6C92"));

        sh = new StringHasher(Hasher.HashAlgo.SHA512, "123456");
        Assert.That(sh.HashResult, Is.EqualTo("BA3253876AED6BC22D4A6FF53D8406C6AD864195ED144AB5C87621B6C233B548BAEAE6956DF346EC8C17F5EA10F35EE3CBC514797ED7DDD3145464E2A0BAB413"));
    }
}
