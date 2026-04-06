using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worldpack.API
{
    public interface IFormat<T>
    {
        IFormat<T> Compress();
        IFormat<T> Decompress();
        void WriteTo(BinaryWriter bw, IEncoder<T> enc);
        abstract static IFormat<T> ReadFrom(BinaryReader br,IEncoder<T> enc);
    }
    public interface IEncoder<T>
    {
        void WriteTo(BinaryWriter bw, T val);
        T ReadFrom(BinaryReader br);
    }
    public interface ISelfEncoder<T> where T:ISelfEncoder<T>
    {
        void WriteTo(BinaryWriter bw);
        abstract static T ReadFrom(BinaryReader br);
    }
}
