namespace Worldpack.Util;

public static class DDSWriter
{
    static readonly byte[] DDS_RESERVED1 = new byte[44];
    public static void Write2DImage(byte[,] buf, BinaryWriter b) // Y,X
    {
        WriteByte2DImageHeader(buf.GetLength(0), buf.GetLength(1), b);
        byte[] row = new byte[buf.GetLength(1)];
        for (int h = 0; h < buf.GetLength(0); h++)
        {
            for (int w = 0; w < row.Length; w++) row[w] = buf[h, w];
            b.Write(row);
        }
        

    }
    public static void Write2DImage(byte[] buf, int width, BinaryWriter b)
    {
        var height = buf.Length / width;
        if (height * width != buf.Length) throw new ArgumentException("Buffer length not multiple of width");
        WriteByte2DImageHeader(height, width, b);
        b.Write(buf);
    }
    static void WriteByte2DImageHeader(int Height, int width, BinaryWriter b)
    {
         b.Write(0x20534444); //'DDS '
        b.Write(124); // sizeof(DDS_HEADER)
        b.Write(0x1007); //DDSD_CAPS|DDSD_HEIGHT|DDSD_WIDTH|DDSD_PIXELFORMAT
        b.Write(Height); //Height
        b.Write(width); //Width
        b.Write(width); //Pitch
        b.Write(0); //depth;
        b.Write(0); //mipmap count
        b.Write(DDS_RESERVED1);
        //pixel formar
        b.Write(32); //sizeof
        b.Write(0x20000); //DDPF_LUMINANCE
        b.Write(0);//4CC
        b.Write(8); //bpp
        b.Write(0xff); //Luma/R mask
        b.Write(0); //G mask
        b.Write(0); //B mask
        b.Write(0); //A mask
        //end pixel format
        b.Write(0x1000); // DDSCAPS_TEXTURE
        b.Write(0); //caps2
        b.Write(0); //caps3
        b.Write(0); //caps4
        b.Write(0); //reserved2
    }
}