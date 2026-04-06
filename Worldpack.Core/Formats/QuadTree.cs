using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worldpack.Core.Formats;
public interface IQuadTree<T>
{
    T this[uint x, uint y] { get; }
    int Order { get; }
}
class QuadTreeNode<T> : IQuadTree<T> where T : IQuadTree<T>

{
    public int Order { get; private set; }
    readonly IQuadTree<T> UL, UR, LL, LR;

    QuadTreeNode(int order, IQuadTree<T> uL, IQuadTree<T> uR, IQuadTree<T> lL, IQuadTree<T> lR)
    {
        Order = order;
        UL = uL;
        UR = uR;
        LL = lL;
        LR = lR;
    }

    public T this[uint x, uint y]
    {
        get
        {
            bool bx = (x & (1u << Order)) != 0;
            bool by = (y & (1u << Order)) != 0;
            var member = ((bx ? 1 : 0) + (by ? 2 : 0)) switch
            {
                0 => UL,
                1 => UR,
                2 => LL,
                3 => LR,
                _ => throw new InvalidOperationException("Impossible")
            };
            var mask = (1u << Order) - 1;
            return member[x & mask, y & mask];
        }
    }


}
class QuadTreeOrder1Node<T> : IQuadTree<T>
{
    readonly T UL, UR, LL, LR;

    QuadTreeOrder1Node(T uL, T uR, T lL, T lR)
    {
        UL = uL;
        UR = uR;
        LL = lL;
        LR = lR;
    }

    public int Order => 1;
    public T this[uint x, uint y] =>

             ((x & 1) + (y & 1) << 1) switch
             {
                 0 => UL,
                 1 => UR,
                 2 => LL,
                 3 => LR,
                 _ => throw new InvalidOperationException("Impossible")
             };
}

class QuadTreeLiteral<T> : IQuadTree<T>
{
    public T this[uint x, uint y]
    {
        get
        {
            if (x < (1 << Order) && y < (1 << Order)) return Value;
            throw new IndexOutOfRangeException();
        }
    }
    QuadTreeLiteral(T value) { Value = value; }

    public int Order { get; private set; }
    T Value;
}
static class QuadtreeBuilder
{
    public static IQuadTree<T> Build<T>(T[] data) where T : IEquatable<T>
    {
        if (!int.IsPow2(data.Length) || (int.Log2(data.Length) & 1) == 1)
        {
            throw new ArgumentException("Not a square array with sides a power of 2");
        }
        int order = int.Log2(data.Length) >> 1;
        return null!;
    }
}
