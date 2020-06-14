using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace BBFG {
    class Program {

        [DllImport("gdi32.dll")]
        static extern bool GetCharABCWidthsA(IntPtr hdc, uint wFirst, uint wLast, [Out] ABC[] lpabc);
        [StructLayout(LayoutKind.Sequential)]
        private struct ABC {
            public int abcfA;
            public uint abcfB;
            public int abcfC;
        }

        static void Main(string[] args) {

            var fs = new StreamWriter("font.csv", false, Encoding.Default);

            int HW_COUNT = 16;
            int HW_IMG = 512;
            int FONTSIZE = 26;
            float HW_CELL = (float)HW_IMG / (float)HW_COUNT;
            float upixel = 1.0f / HW_IMG;

            var coreImage = new Bitmap(HW_IMG, HW_IMG, PixelFormat.Format32bppArgb);
            var g = Graphics.FromImage(coreImage);
            g.Clear(Color.FromArgb(0, 255, 255, 255));
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            //var abcList = new ABC[256];
            //var hdc = g.GetHdc();
            //GetCharABCWidthsA(hdc, 0, 255, abcList);
            //g.ReleaseHdc(hdc);

            var ft = new Font("Squarish Sans CT", FONTSIZE, FontStyle.Regular, GraphicsUnit.Pixel);
            var sb = new SolidBrush(Color.White);
            var sf = StringFormat.GenericTypographic;
            sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            //var np = new Pen(Color.Red);

            float x = 0, y = 0;
            var counter = 0;
            byte[] singleChr = { 0, 0 };
            SizeF measureRes;
            for (int i = 0; i < HW_COUNT; i++) {
                for (int j = 0; j < HW_COUNT; j++) {
                    measureRes = g.MeasureString(Encoding.GetEncoding(1252).GetString(singleChr, 0, 1), ft, SizeF.Empty, sf);

                    x = j * HW_CELL;
                    y = i * HW_CELL + (HW_CELL - measureRes.Height) / 2;

                    g.DrawString(Encoding.GetEncoding(1252).GetString(singleChr, 0, 1), ft, sb, x, y, sf);
                    //g.DrawRectangle(np, new Rectangle((int)x, (int)y, (int)measureRes.Width, (int)measureRes.Height));

                    fs.WriteLine(string.Format("{0:F6}\t{1:F6}\t{2:F6}\t{3:F6}\t{4:F6}\t{5:F6}",
                        x / HW_IMG /*+ abcList[counter].abcfA * upixel*/,
                        (float)i / HW_COUNT,
                        /*abcList[counter].abcfB * upixel*/measureRes.Width / HW_IMG,
                        /*abcList[counter].abcfA * upixel*/0,
                        /*abcList[counter].abcfC * upixel*/0,
                        1.0f / HW_COUNT));

                    singleChr[0]++;
                    counter++;
                }
            }

            coreImage.Save("result.png", ImageFormat.Png);
            fs.Close();
            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    }
}
