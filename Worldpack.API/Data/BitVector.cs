using Worldpack.API;

namespace Worldpack.Data;

public class BitVector : ISelfEncoder<BitVector>, IEquatable<BitVector>, IEquatable<ulong[]>
{
    ulong[] data;
    int popcount_valid_to;
    int[] popcount_cache;
    public bool this[int idx]
    {
        get
        {
            return (data[idx >> 6] >> (idx & 0x3f)) != 0;
        }
        set
        {
            var wd = data[idx >> 6];
            var mask = 1ul << idx & 0x3f;
            if (value) data[idx >> 6] = wd | mask; else data[idx >> 6] = wd & ~mask;
            popcount_valid_to = int.Min(popcount_valid_to, (idx >> 6) - 1);
        }
    }
    public void EnsureCapacity(int cap)
    {
        var realcap = (cap + 63) >> 6;
        if (realcap > data.Length)
        {
            var ndata = new ulong[realcap];
            var new_pop = new int[realcap];
            Array.Copy(data, ndata, data.Length);
            Array.Copy(popcount_cache, new_pop, popcount_cache.Length);
            data = ndata;
            popcount_cache = new_pop;
        }
    }
    public BitVector(int cap)
    {
        var realcap = (cap + 63) >> 6;
        data = new ulong[realcap];
        popcount_cache = new int[realcap];
        popcount_valid_to = realcap;
    }
    public BitVector(ulong[] data)
    {
        this.data = data;
        popcount_cache = new int[data.Length];
        buildPopCacheTo(data.Length);
        popcount_valid_to = data.Length;
    }
    void buildPopCacheTo(int wd)
    {
        popcount_valid_to = int.Max(popcount_valid_to, 0);
        for (int i = popcount_valid_to; i < wd - 1; i++)
        {
            popcount_cache[i + 1] = popcount_cache[i] + (int)ulong.PopCount(data[i]);
        }
    }
    public int PopCountTo(int idx)
    {
        int wd = idx >> 6;
        ulong mask = (1ul << idx & 0x3f) - 1;
        if (wd == 0)
        {
            return (int)ulong.PopCount(data[0] & mask);
        }
        buildPopCacheTo(wd);
        return popcount_cache[wd] + (int)ulong.PopCount(data[wd] & mask);
    }
    public int PopCount()
    {
        buildPopCacheTo(data.Length);
        return popcount_cache[^1] + (int)ulong.PopCount(data[^1]);
    }
    public void WriteTo(BinaryWriter bw)
    {
        bw.Write7BitEncodedInt(data.Length);
        for (int i = 0; i < data.Length; i++)
        {
            bw.Write(data[i]);
        }
    }
    public static BitVector ReadFrom(BinaryReader br)
    {
        int len = br.Read7BitEncodedInt();
        ulong[] data = new ulong[len];
        for (int i = 0; i < len; i++)
        {
            data[i] = br.ReadUInt64();
        }
        return new(data);
    }

    public bool Equals(BitVector? other) => other?.data.SequenceEqual(data) ?? this == null;

    public bool Equals(ulong[]? other) => other?.SequenceEqual(data)??this==null;

    public void Set(int index) => this[index] = true;
    public void Reset(int index) => this[index] = false;
    public bool Toggle(int idx)
    {
        var wd = data[idx >> 6];
        var mask = 1ul << idx & 0x3f;
        data[idx >> 6] = wd ^ mask;
        popcount_valid_to = int.Min(popcount_valid_to, (idx >> 6) - 1);
        return (wd & mask) != 0;
    }

    public override bool Equals(object? obj)
    {
      
        if (obj is BitVector other)
        {
            return Equals(other);
        }
        return obj==null&&this==null;
    }

    public override int GetHashCode()
    {
        return data.GetHashCode();
    }

    public ReadOnlySpan<ulong> Borrow => data;
}