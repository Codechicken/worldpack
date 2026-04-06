using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Worldpack.API;

namespace Worldpack.Util;
class SelfEncoder<T> : IEncoder<T> where T : ISelfEncoder<T>
{
    private SelfEncoder() { }
    static readonly SelfEncoder<T> Instance = new();
    public T ReadFrom(BinaryReader br) => T.ReadFrom(br);

    public void WriteTo(BinaryWriter bw, T val) => val.WriteTo(bw);
    public static IEncoder<T> Value => Instance;
}
class ByteEncoder : IEncoder<byte>
{
    public byte ReadFrom(BinaryReader br) => br.ReadByte();

    public void WriteTo(BinaryWriter bw, byte val) => bw.Write(val);
}
class SByteEncoder : IEncoder<sbyte>
{
    public sbyte ReadFrom(BinaryReader br) => br.ReadSByte();

    public void WriteTo(BinaryWriter bw, sbyte val) => bw.Write(val);
}
public class Encoder
{

    public static readonly IEncoder<byte> Byte = new ByteEncoder();
    public static readonly IEncoder<sbyte> SByte = new SByteEncoder();
    public static IEncoder<T> Self<T>() where T : ISelfEncoder<T> => SelfEncoder<T>.Value;
}