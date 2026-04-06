using System.Buffers;
using System.Text;
using Worldpack.Util;

namespace Worldpack.NBT
{
    public class ListTag : ICollectionTag<int>, IMutableTag<IList<ITag>>
    {
        public ListTag()
        {
        }

        public ITag this[int k] { get => Value[k]; set => Value[k] = value; }

        public IList<ITag> Value { get; private set; } = [];

        public Tag Type => Tag.List;
        public Tag ContentTag { get; private set; }

        public static ITag<IList<ITag>> Parse(ArraySegment<byte> data, out int size)
        {
            ListTag ret = new();
            size = 5;
            Tag t = (Tag)data[0];
            ret.ContentTag = t;
            data = data.Slice(1);
            int count = BitConverter.ToInt32(data).FromBigEndian();
            if (t == Tag.End)
            {
                if (count == 0)
                {
                    ret.Value = Array.Empty<ITag>();
                    return ret;
                }
                throw new InvalidDataException("List of end tags does not have size 0");
            }
            data = data.Slice(4);
            var acc = new List<ITag>(count);
            ret.Value = acc;
            TagParser p = TagRegistry.lookupTag(t);
            for (int i = 0; i < count; i++)
            {
                acc.Add(p(data, out var sz));
                size += sz;
                data = data.Slice(sz);
            }
            acc.TrimExcess();
            return ret;
        }
        public override string ToString()
        {
            StringBuilder buf = new();
            buf.Append("[\n");
            foreach (var i in Value)
            {
                buf.Append($"{i.ToString()},\n");
            }
            buf.Append(']');
            return buf.ToString();
        }

        public void SetValue(IList<ITag> val) => Value = val;

        public void WriteTo(IBufferWriter<byte> writer)
        {
            writer.Write([(byte)ContentTag]);
            writer.Write(BitConverter.GetBytes(Value.Count.ToBigEndian()));
            foreach (var e in Value)
            {
                e.WriteTo(writer);
            }
        }

        public int Sizeof()
        {
            int inner = 5;
            foreach (var e in Value) inner += e.Sizeof();
            return inner;
        }
    }
    public class CompoundTag : ICollectionTag<string>, IMutableTag<IDictionary<string, ITag>>
    {
        public ITag this[string k]
        {
            get => Value[k]; set
            {
                if (value == null)
                {
                    Value.Remove(k);
                }
                else
                {
                    Value[k] = value;
                }
            }
        }
        public ITag? this[string k1, params string[] k2]
        {
            get
            {
                if (k2.Length == 0)
                {
                    return this[k1];
                }
                ITag? ret = null;
                if (this[k1] is CompoundTag d)
                {
                    foreach (var s in k2)
                    {
                        if (d[s] is CompoundTag t) d = t;
                        else
                        {
                            ret = d[s];
                            d = null!;
                        }
                    }
                }
                return ret;
            }
        }
        public IDictionary<string, ITag> Value { get; private set; } = new Dictionary<string, ITag>();

        public Tag Type => Tag.Compound;

        public static ITag<IDictionary<string, ITag>> Parse(ArraySegment<byte> data, out int size)
        {
            var ret = new Dictionary<string, ITag>();
            size = 1;
            while (data.Count!=0&&data[0] != 0)
            {
                Tag t = (Tag)data[0];
                data = data.Slice(1);
                string name = data.ReadJString(out var namesz);
                size += 1 + namesz;
                data = data.Slice(namesz);
                ret[name] = TagRegistry.lookupTag(t)(data, out var nestsz);
                size += nestsz;
                data = data.Slice(nestsz);
            }
            ret.TrimExcess();
            return new CompoundTag
            {
                Value = ret
            };
        }
        public override string ToString()
        {
            StringBuilder buf = new();
            buf.Append("{\n");
            foreach (var i in Value)
            {
                if (i.Key != "")
                {
                    buf.Append($"{i.Key}: {i.Value},\n");
                }
                else
                {
                    buf.Append($"<empty>: {i.Value}");
                }
            }
            buf.Append('}');
            return buf.ToString();
        }
        public void SetValue(IDictionary<string, ITag> val) => Value = val;

        public void WriteTo(IBufferWriter<byte> writer)
        {
            Span<byte> buf = stackalloc byte[1];
            foreach (var e in Value.OrderBy((e)=>e.Key))
            {
                buf[0] = (byte)e.Value.Type;
                writer.Write(buf);
                e.Key.WriteJString(writer);
                e.Value.WriteTo(writer);
            }
            buf[0] = 0;
            writer.Write(buf);//End Tag
        }

        public int Sizeof()
        {
            int inner = 1;
            foreach (var e in Value)
            {
                inner += 3 + Encoding.UTF8.GetByteCount(e.Key) + e.Value.Sizeof();
            }
            return inner;
        }
    }
    public class Shape : IEquatable<Shape>
    {
        Dictionary<string, Tag> shape = [];
        public Shape(CompoundTag t)
        {
            foreach (var e in t.Value)
            {
                shape[e.Key] = e.Value.Type;
            }
            shape.TrimExcess();
        }

        public bool Equals(Shape? other)
        {
            if (other == null) return this == null;
            foreach (var e in shape)
            {
                if (other.shape.TryGetValue(e.Key, out var v) && (e.Value == v)) continue;
                return false;
            }
            return true;
        }
        // override object.Equals
        public override bool Equals(object? obj)
        {

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            // TODO: write your implementation of Equals() here

            return Equals((Shape)obj);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            int hash = 0;
            foreach (var e in shape.OrderBy((x) => x.Key))
            {
                hash *= 33;
                hash ^= e.Key.GetHashCode();
                hash *= 33;
                hash ^= e.Value.GetHashCode();
            }
            return hash;
        }
        public override string ToString()
        {
            StringBuilder b = new();
            b.Append("Shape {\n");
            foreach (var e in shape.OrderBy((k) => k.Key))
            {
                b.Append($"{e.Key}: {Enum.GetName(e.Value)}\n");
            }
            b.Append("}\n");
            return b.ToString();
        }
    }
}