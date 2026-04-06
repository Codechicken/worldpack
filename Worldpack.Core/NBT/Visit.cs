namespace Worldpack.NBT
{
    public enum VisitDisposition
    {
        DontVisitChildren,
        VisitAllChildren,
        VisitNonLeafChildren
    }
    public interface INBTVisitor
    {
        virtual VisitDisposition Visit(CompoundTag t,string name) => VisitDisposition.VisitAllChildren;
        virtual VisitDisposition Visit(ListTag t,string name) => VisitDisposition.VisitAllChildren;
        virtual void PostVisit(CompoundTag t,string name) { }
        virtual void PostVisit(ListTag t,string name) { }
        virtual void Visit(ByteTag t,string name) { }
        virtual void Visit(ShortTag t,string name) { }
        virtual void Visit(IntTag t,string name) { }
        virtual void Visit(LongTag t,string name) { }
        virtual void Visit(FloatTag t,string name) { }
        virtual void Visit(DoubleTag t,string name) { }
        virtual void Visit(ByteArrayTag t,string name) { }
        virtual void Visit(StringTag t,string name) { }
        virtual void Visit(IntArrayTag t,string name) { }
        virtual void Visit(LongArrayTag t,string name) { }
    }
    public static class VisitorImpl
    {
        public static void Visit<T>(this INBTVisitor v, ICollectionTag<T> tag)
        {
            switch (tag)
            {
                case ListTag t:
                    foreach (var e in t.Value)
                    {
                        v.Visit(e, string.Empty);
                    }
                    break;
                case CompoundTag t:
                    foreach (var e in t.Value)
                    {
                        v.Visit(e.Value, e.Key);
                    }
                    break;
            }
        }
        private static void Visit(this INBTVisitor v, ITag tag, string name)
        {
            switch (tag)
            {
                case ByteTag t:
                    v.Visit(t, name);
                    break;
                case ShortTag t:
                    v.Visit(t, name);
                    break;
                case IntTag t:
                    v.Visit(t, name);
                    break;
                case LongTag t:
                    v.Visit(t, name);
                    break;
                case FloatTag t:
                    v.Visit(t, name);
                    break;
                case DoubleTag t:
                    v.Visit(t, name);
                    break;
                case ByteArrayTag t:
                    v.Visit(t, name);
                    break;
                case StringTag t:
                    v.Visit(t, name);
                    break;
                case IntArrayTag t:
                    v.Visit(t, name);
                    break;
                case LongArrayTag t:
                    v.Visit(t, name);
                    break;
                case ListTag t:
                    {
                        var disp = v.Visit(t, name);
                        switch (disp)
                        {
                            case VisitDisposition.DontVisitChildren:
                                break;
                            case VisitDisposition.VisitNonLeafChildren:
                                if (t.ContentTag == Tag.List || t.ContentTag == Tag.Compound)
                                {
                                    goto case VisitDisposition.VisitAllChildren;
                                }
                                break;
                            case VisitDisposition.VisitAllChildren:
                                foreach (var e in t.Value)
                                {
                                    v.Visit(e, string.Empty);
                                }
                                v.PostVisit(t, name);
                                break;
                        }
                        break;
                    }
                case CompoundTag t:
                    {
                        var disp = v.Visit(t, name);
                        switch (disp)
                        {
                            case VisitDisposition.DontVisitChildren:
                                break;
                            case VisitDisposition.VisitNonLeafChildren:
                                foreach (var e in t.Value)
                                {
                                    if (e.Value.Type == Tag.Compound || e.Value.Type == Tag.List)
                                    {
                                        v.Visit(e.Value, e.Key);
                                    }
                                }
                                v.PostVisit(t, name);
                                break;
                            case VisitDisposition.VisitAllChildren:
                                foreach (var e in t.Value)
                                {
                                    v.Visit(e.Value, e.Key);
                                }
                                v.PostVisit(t, name);
                                break;
                        }
                    }
                    break;
            }
        }
    }
}