using System.Buffers;

namespace Worldpack.NBT
{
    public enum Tag : byte
    {
        End = 0,
        Byte,
        Short,
        Int,
        Long,
        Float,
        Double,
        ByteArray,
        String,
        List,
        Compound,
        IntArray,
        LongArray
    }
    public interface ITag
    {
        Tag Type { get; }
        void WriteTo(IBufferWriter<byte> writer);
        int Sizeof();
    }
    public interface ITag<out T> : ITag
    {
        T Value { get; }
        abstract static ITag<T> Parse(ArraySegment<byte> data, out int size);
    }
    public interface IMutableTag<T> : ITag<T>
    {
        void SetValue(T val);
    }
    public interface ICollectionTag<Key>
    {
        ITag this[Key k]{ get; set; }
    }
    public delegate ITag TagParser(ArraySegment<byte> data, out int size);
    public static class TagRegistry
    {
        static Dictionary<Tag, TagParser> d = new();
        public static void registerTag(Tag t, TagParser p)
        {
            lock (d)
            {
                if (d.ContainsKey(t))
                {
                    throw new InvalidOperationException($"Trying to redefine parser for tag {t}");
                }
                d.Add(t, p);
            }
        }
        public static TagParser lookupTag(Tag t)
        {
            lock (d)
            {
                return d[t];
            }
        }
        static TagRegistry()
        {
            registerTag(Tag.Byte, ByteTag.Parse);
            registerTag(Tag.Short, ShortTag.Parse);
            registerTag(Tag.Int, IntTag.Parse);
            registerTag(Tag.Long, LongTag.Parse);
            registerTag(Tag.Float, FloatTag.Parse);
            registerTag(Tag.Double, DoubleTag.Parse);
            registerTag(Tag.ByteArray, ByteArrayTag.Parse);
            registerTag(Tag.String, StringTag.Parse);
            registerTag(Tag.List, ListTag.Parse);
            registerTag(Tag.Compound, CompoundTag.Parse);
            registerTag(Tag.IntArray, IntArrayTag.Parse);
            registerTag(Tag.LongArray, LongArrayTag.Parse);
        }
    }
}