﻿using UnityEngine;
using System.IO;

namespace com.tencent.pandora
{
    public class GIFDecoder
    {
        /**
             * File read status: No errors.
             */
        public static readonly int STATUS_OK = 0;

        /**
		 * File read status: Error decoding file (may be partially decoded)
		 */
        public static readonly int STATUS_FORMAT_ERROR = 1;

        /**
		 * File read status: Unable to open source.
		 */
        public static readonly int STATUS_OPEN_ERROR = 2;

        static GIFDecoder mInstance;

        protected Stream inStream;
        protected int status;

        protected int width; // full image width
        protected int height; // full image height
        protected bool gctFlag; // global color table used
        protected int gctSize; // size of global color table
        protected int loopCount = 1; // iterations; 0 = repeat forever

        protected int[] gct; // global color table
        protected int[] lct; // local color table
        protected int[] act; // active color table

        protected int bgIndex; // background color index
        protected int bgColor; // background color
        protected int lastBgColor; // previous bg color
        protected int pixelAspect; // pixel aspect ratio

        protected bool lctFlag; // local color table flag
        protected bool interlace; // interlace flag
        protected int lctSize; // local color table size

        protected int frameCount;

        protected Texture2D mTexture;

        protected int ix, iy, iw, ih; // current image rectangle
        protected Rect lastRect; // last image rect

        protected byte[] block = new byte[256]; // current data block
        protected int blockSize = 0; // block size

        // last graphic control extension info
        protected int dispose = 0;
        // 0=no action; 1=leave in place; 2=restore to bg; 3=restore to prev
        protected int lastDispose = 0;
        protected bool transparency = false; // use transparent color
        protected int delay = 0; // delay in milliseconds
        protected int transIndex; // transparent color index

        protected static readonly int MaxStackSize = 4096;
        // max decoder pixel stack size

        // LZW decoder working arrays
        protected short[] prefix;
        protected byte[] suffix;
        protected byte[] pixelStack;
        protected byte[] pixels;

        public static Texture2D Decode(byte[] data)
        {
            if (mInstance == null)
            {
                mInstance = new GIFDecoder();
            }

            if (mInstance.Read(data) == STATUS_OK)
            {
                Texture2D tex = mInstance.mTexture;
                mInstance.mTexture = null;
                return tex;
            }

            return null;
        }

        protected void SetPixels()
        {
            Color32[] Dest = new Color32[width * height];

            // copy each source line to the appropriate place in the destination
            int pass = 1;
            int inc = 8;
            int iline = 0;
            for (int i = 0; i < ih; i++)
            {
                int line = i;
                if (interlace)
                {
                    if (iline >= ih)
                    {
                        pass++;
                        switch (pass)
                        {
                            case 2:
                                iline = 4;
                                break;
                            case 3:
                                iline = 2;
                                inc = 4;
                                break;
                            case 4:
                                iline = 1;
                                inc = 2;
                                break;
                        }
                    }
                    line = iline;
                    iline += inc;
                }
                line += iy;
                if (line < height)
                {
                    int k = (height - line - 1) * width;
                    int dx = k + ix; // start of line in dest
                    int dlim = dx + iw; // end of dest line
                    if ((k + width) < dlim)
                    {
                        dlim = k + width; // past dest edge
                    }
                    int sx = i * iw; // start of line in source
                    while (dx < dlim)
                    {
                        // map color and insert in destination
                        int index = ((int)pixels[sx++]) & 0xff;
                        int c = act != null ? act[index] : 0;
                        if (c != 0)
                        {
                            Dest[dx].a = (byte)(c >> 24);
                            Dest[dx].r = (byte)(c >> 16);
                            Dest[dx].g = (byte)(c >> 8);
                            Dest[dx].b = (byte)(c);
                        }
                        dx++;
                    }
                }
            }

            mTexture.SetPixels32(Dest);
            mTexture.Apply(false, false);
        }

        /**
		 * Reads GIF image from stream
		 *
		 * @param BufferedInputStream containing GIF file.
		 * @return read status code (0 = no errors)
		 */
        public int Read(byte[] data)
        {
            Init();
            if (data != null)
            {
                inStream = new MemoryStream(data);
                ReadHeader();
                if (!Error())
                {
                    ReadContents();
                    if (frameCount < 0)
                    {
                        status = STATUS_FORMAT_ERROR;
                    }
                }
            }
            else
            {
                status = STATUS_OPEN_ERROR;
            }

            inStream.Dispose();
            inStream.Close();
            inStream = null;
            return status;
        }

        /**
		 * Decodes LZW image data into pixel array.
		 * Adapted from John Cristy's ImageMagick.
		 */
        protected void DecodeImageData()
        {
            int NullCode = -1;
            int npix = iw * ih;
            int available,
                clear,
                code_mask,
                code_size,
                end_of_information,
                in_code,
                old_code,
                bits,
                code,
                count,
                i,
                datum,
                data_size,
                first,
                top,
                bi,
                pi;

            if ((pixels == null) || (pixels.Length < npix))
            {
                pixels = new byte[npix]; // allocate new pixel array
            }
            if (prefix == null) prefix = new short[MaxStackSize];
            if (suffix == null) suffix = new byte[MaxStackSize];
            if (pixelStack == null) pixelStack = new byte[MaxStackSize + 1];

            //  Initialize GIF data stream decoder.

            data_size = Read();
            clear = 1 << data_size;
            end_of_information = clear + 1;
            available = clear + 2;
            old_code = NullCode;
            code_size = data_size + 1;
            code_mask = (1 << code_size) - 1;
            for (code = 0; code < clear; code++)
            {
                prefix[code] = 0;
                suffix[code] = (byte)code;
            }

            //  Decode GIF pixel stream.

            datum = bits = count = first = top = pi = bi = 0;

            for (i = 0; i < npix;)
            {
                if (top == 0)
                {
                    if (bits < code_size)
                    {
                        //  Load bytes until there are enough bits for a code.
                        if (count == 0)
                        {
                            // Read a new data block.
                            count = ReadBlock();
                            if (count <= 0)
                                break;
                            bi = 0;
                        }
                        datum += (((int)block[bi]) & 0xff) << bits;
                        bits += 8;
                        bi++;
                        count--;
                        continue;
                    }

                    //  Get the next code.

                    code = datum & code_mask;
                    datum >>= code_size;
                    bits -= code_size;

                    //  Interpret the code

                    if ((code > available) || (code == end_of_information))
                        break;
                    if (code == clear)
                    {
                        //  Reset decoder.
                        code_size = data_size + 1;
                        code_mask = (1 << code_size) - 1;
                        available = clear + 2;
                        old_code = NullCode;
                        continue;
                    }
                    if (old_code == NullCode)
                    {
                        pixelStack[top++] = suffix[code];
                        old_code = code;
                        first = code;
                        continue;
                    }
                    in_code = code;
                    if (code == available)
                    {
                        pixelStack[top++] = (byte)first;
                        code = old_code;
                    }
                    while (code > clear)
                    {
                        pixelStack[top++] = suffix[code];
                        code = prefix[code];
                    }
                    first = ((int)suffix[code]) & 0xff;

                    //  Add a new string to the string table,

                    if (available >= MaxStackSize)
                        break;
                    pixelStack[top++] = (byte)first;
                    prefix[available] = (short)old_code;
                    suffix[available] = (byte)first;
                    available++;
                    if (((available & code_mask) == 0)
                        && (available < MaxStackSize))
                    {
                        code_size++;
                        code_mask += available;
                    }
                    old_code = in_code;
                }

                //  Pop a pixel off the pixel stack.

                top--;
                pixels[pi++] = pixelStack[top];
                i++;
            }

            for (i = pi; i < npix; i++)
            {
                pixels[i] = 0; // clear missing pixels
            }

        }

        /**
		 * Returns true if an error was encountered during reading/decoding
		 */
        protected bool Error()
        {
            return status != STATUS_OK;
        }

        /**
		 * Initializes or re-initializes reader
		 */
        protected void Init()
        {
            status = STATUS_OK;
            frameCount = 0;
            gct = null;
            lct = null;
        }

        /**
		 * Reads a single byte from the input stream.
		 */
        protected int Read()
        {
            int curByte = 0;
            try
            {
                curByte = inStream.ReadByte();
            }
            catch (IOException)
            {
                status = STATUS_FORMAT_ERROR;
            }
            return curByte;
        }

        /**
		 * Reads next variable length block from input.
		 *
		 * @return number of bytes stored in "buffer"
		 */
        protected int ReadBlock()
        {
            blockSize = Read();
            int n = 0;
            if (blockSize > 0)
            {
                try
                {
                    int count = 0;
                    while (n < blockSize)
                    {
                        count = inStream.Read(block, n, blockSize - n);
                        if (count == -1)
                            break;
                        n += count;
                    }
                }
                catch (IOException)
                {
                }

                if (n < blockSize)
                {
                    status = STATUS_FORMAT_ERROR;
                }
            }
            return n;
        }

        /**
		 * Reads color table as 256 RGB integer values
		 *
		 * @param ncolors int number of colors to read
		 * @return int array containing 256 colors (packed ARGB with full alpha)
		 */
        protected int[] ReadColorTable(int ncolors)
        {
            int nbytes = 3 * ncolors;
            int[] tab = null;
            byte[] c = new byte[nbytes];
            int n = 0;
            try
            {
                n = inStream.Read(c, 0, c.Length);
            }
            catch (IOException)
            {
            }
            if (n < nbytes)
            {
                status = STATUS_FORMAT_ERROR;
            }
            else
            {
                tab = new int[256]; // max size to avoid bounds checks
                int i = 0;
                int j = 0;
                while (i < ncolors)
                {
                    byte r = c[j++];
                    byte g = c[j++];
                    byte b = c[j++];
                    tab[i++] = (int)((0xff << 24) | (r << 16) | (g << 8) | b);
                }
            }
            return tab;
        }

        /**
		 * Main file parser.  Reads GIF content blocks.
		 */
        protected void ReadContents()
        {
            // read GIF file content blocks
            bool done = false;
            while (!(done || Error()) && (frameCount == 0))
            {
                int code = Read();
                switch (code)
                {
                    case 0x2C: // image separator
                        ReadImage();
                        break;

                    case 0x21: // extension
                        code = Read();
                        switch (code)
                        {
                            case 0xf9: // graphics control extension
                                ReadGraphicControlExt();
                                break;

                            case 0xff: // application extension
                                ReadBlock();
                                string app = "";
                                for (int i = 0; i < 11; i++)
                                {
                                    app += (char)block[i];
                                }
                                if (app.Equals("NETSCAPE2.0"))
                                {
                                    ReadNetscapeExt();
                                }
                                else
                                    Skip(); // don't care
                                break;

                            default: // uninteresting extension
                                Skip();
                                break;
                        }
                        break;

                    case 0x3b: // terminator
                        done = true;
                        break;

                    case 0x00: // bad byte, but keep going and see what happens
                        break;

                    default:
                        status = STATUS_FORMAT_ERROR;
                        break;
                }
            }
        }

        /**
		 * Reads Graphics Control Extension values
		 */
        protected void ReadGraphicControlExt()
        {
            Read(); // block size
            int packed = Read(); // packed fields
            dispose = (packed & 0x1c) >> 2; // disposal method
            if (dispose == 0)
            {
                dispose = 1; // elect to keep old image if discretionary
            }
            transparency = (packed & 1) != 0;
            delay = ReadShort() * 10; // delay in milliseconds
            transIndex = Read(); // transparent color index
            Read(); // block terminator
        }

        /**
		 * Reads GIF file header information.
		 */
        protected void ReadHeader()
        {
            string id = "";
            for (int i = 0; i < 6; i++)
            {
                id += (char)Read();
            }
            if (!id.StartsWith("GIF"))
            {
                status = STATUS_FORMAT_ERROR;
                return;
            }

            ReadLSD();
            if (gctFlag && !Error())
            {
                gct = ReadColorTable(gctSize);
                if (null != gct)
                {
                    bgColor = gct[bgIndex];
                }
            }
        }

        /**
		 * Reads next frame image
		 */
        protected void ReadImage()
        {
            ix = ReadShort(); // (sub)image position & size
            iy = ReadShort();
            iw = ReadShort();
            ih = ReadShort();

            int packed = Read();
            lctFlag = (packed & 0x80) != 0; // 1 - local color table flag
            interlace = (packed & 0x40) != 0; // 2 - interlace flag
                                              // 3 - sort flag
                                              // 4-5 - reserved
            lctSize = 2 << (packed & 7); // 6-8 - local color table size

            if (lctFlag)
            {
                lct = ReadColorTable(lctSize); // read table
                act = lct; // make local table active
            }
            else
            {
                act = gct; // make global table active
                if (bgIndex == transIndex)
                    bgColor = 0;
            }
            int save = 0;
            if (transparency)
            {
                save = act != null ? act[transIndex] : 0;

                if (null != act)
                {
                    act[transIndex] = 0; // set transparent color if specified
                }
            }

            if (act == null)
            {
                status = STATUS_FORMAT_ERROR; // no color table defined
            }

            if (Error()) return;

            DecodeImageData(); // decode pixel data
            Skip();

            if (Error()) return;

            ++frameCount;

            mTexture = new Texture2D(width, height, TextureFormat.ARGB32, false, false);
            mTexture.name = "GIFDecoder Tex";

            SetPixels(); // transfer pixel data to image

            if (transparency)
            {
                if (act != null) act[transIndex] = save;
            }
            ResetFrame();

        }

        /**
		 * Reads Logical Screen Descriptor
		 */
        protected void ReadLSD()
        {

            // logical screen size
            width = ReadShort();
            height = ReadShort();

            // packed fields
            int packed = Read();
            gctFlag = (packed & 0x80) != 0; // 1   : global color table flag
                                            // 2-4 : color resolution
                                            // 5   : gct sort flag
            gctSize = 2 << (packed & 7); // 6-8 : gct size

            bgIndex = Read(); // background color index
            pixelAspect = Read(); // pixel aspect ratio
        }

        /**
		 * Reads Netscape extenstion to obtain iteration count
		 */
        protected void ReadNetscapeExt()
        {
            do
            {
                ReadBlock();
                if (block[0] == 1)
                {
                    // loop count sub-block
                    int b1 = ((int)block[1]) & 0xff;
                    int b2 = ((int)block[2]) & 0xff;
                    loopCount = (b2 << 8) | b1;
                }
            } while ((blockSize > 0) && !Error());
        }

        /**
		 * Reads next 16-bit value, LSB first
		 */
        protected int ReadShort()
        {
            // read 16-bit value, LSB first
            return Read() | (Read() << 8);
        }

        /**
		 * Resets frame state for reading next image.
		 */
        protected void ResetFrame()
        {
            lastDispose = dispose;
            lastBgColor = bgColor;
            lct = null;
        }

        /**
		 * Skips variable length blocks up to and including
		 * next zero length block.
		 */
        protected void Skip()
        {
            do
            {
                ReadBlock();
            } while ((blockSize > 0) && !Error());
        }
    }
}
