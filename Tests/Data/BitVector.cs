using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Worldpack.Data;

namespace Tests.Data;
[TestFixture]
internal class BitVectorTest
{
    [Test]
    public void PopCountWorks()
    {
        BitVector b = new([ulong.MaxValue,0,uint.MaxValue,255ul<<16]);
        Assert.That(b.PopCount(), Is.EqualTo(64 + 32 + 8));
        Assert.That(b.PopCountTo(128),Is.EqualTo( 64));
        b[0] = false;
        Assert.That(b.PopCount(),Is.EqualTo(63 + 32 + 8));
        Assert.That(b.PopCountTo(128), Is.EqualTo(63));
    }
    [Test]
    public void SerializeWorks()
    {
        BitVector b = new([ulong.MaxValue, 0, uint.MaxValue, 255ul << 16]);
        MemoryStream s = new(36);
        b.WriteTo(new BinaryWriter(s));
        s.Seek(0, SeekOrigin.Begin);
        BitVector b2 = BitVector.ReadFrom(new BinaryReader(s));
        Assert.That(b,Is.EqualTo(b2));
    }

}
