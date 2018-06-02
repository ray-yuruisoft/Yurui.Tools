using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Yurui.Tools.Captcha
{
    unsafe public class Binaryzation
    {
        public Bitmap SrcBmp;
        public Bitmap GrayBmp;
        public Bitmap DestBmp;
        public Bitmap HistBmp;
        public Bitmap SmoothHistBmp;
        public int[] HistGram = new int[256];
        public int[] HistGramS = new int[256];
        public int Thr;
        public ImageProcess.ThresholdType thresholdType = ImageProcess.ThresholdType.OSTU;

        public Binaryzation(Bitmap src) : this()
        {
            SrcBmp = src;
            Generate();
        }
        public Binaryzation()
        {
            HistBmp = CreateGrayBitmap(256, 100);
            SmoothHistBmp = CreateGrayBitmap(256, 100);
        }

        private Bitmap CreateGrayBitmap(int Width, int Height)
        {
            Bitmap Bmp = new Bitmap(Width, Height, PixelFormat.Format8bppIndexed);
            ColorPalette Pal = Bmp.Palette;
            for (int Y = 0; Y < Pal.Entries.Length; Y++) Pal.Entries[Y] = Color.FromArgb(255, Y, Y, Y);
            Bmp.Palette = Pal;
            return Bmp;
        }
        private bool IsGrayBitmap(Bitmap Bmp)
        {
            if (Bmp.PixelFormat != PixelFormat.Format8bppIndexed) return false;
            if (Bmp.Palette.Entries.Length != 256) return false;
            for (int Y = 0; Y < Bmp.Palette.Entries.Length; Y++)
                if (Bmp.Palette.Entries[Y] != Color.FromArgb(255, Y, Y, Y)) return false;
            return true;
        }
        private Bitmap ConvertToGrayBitmap(Bitmap Src)
        {
            Bitmap Dest = CreateGrayBitmap(Src.Width, Src.Height);
            BitmapData SrcData = Src.LockBits(new Rectangle(0, 0, Src.Width, Src.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData DestData = Dest.LockBits(new Rectangle(0, 0, Dest.Width, Dest.Height), ImageLockMode.ReadWrite, Dest.PixelFormat);
            int Width = SrcData.Width, Height = SrcData.Height;
            int SrcStride = SrcData.Stride, DestStride = DestData.Stride;
            byte* SrcP, DestP;
            for (int Y = 0; Y < Height; Y++)
            {
                SrcP = (byte*)SrcData.Scan0 + Y * SrcStride;
                DestP = (byte*)DestData.Scan0 + Y * DestStride;
                for (int X = 0; X < Width; X++)
                {
                    *DestP = (byte)((*SrcP + (*(SrcP + 1) << 1) + *(SrcP + 2)) >> 2);
                    SrcP += 3;
                    DestP++;
                }
            }
            Src.UnlockBits(SrcData);
            Dest.UnlockBits(DestData);
            return Dest;
        }
        private void GetHistGram(Bitmap Src, int[] HistGram)
        {
            BitmapData SrcData = Src.LockBits(new Rectangle(0, 0, Src.Width, Src.Height), ImageLockMode.ReadWrite, Src.PixelFormat);
            int Width = SrcData.Width, Height = SrcData.Height, SrcStride = SrcData.Stride;
            byte* SrcP;
            for (int Y = 0; Y < 256; Y++) HistGram[Y] = 0;
            for (int Y = 0; Y < Height; Y++)
            {
                SrcP = (byte*)SrcData.Scan0 + Y * SrcStride;
                for (int X = 0; X < Width; X++, SrcP++) HistGram[*SrcP]++;
            }
            Src.UnlockBits(SrcData);
        }
        private void DoBinaryzation(Bitmap Src, Bitmap Dest, int Threshold)
        {
            BitmapData SrcData = Src.LockBits(new Rectangle(0, 0, Src.Width, Src.Height), ImageLockMode.ReadWrite, Src.PixelFormat);
            BitmapData DestData = Dest.LockBits(new Rectangle(0, 0, Dest.Width, Dest.Height), ImageLockMode.ReadWrite, Dest.PixelFormat);
            int Width = SrcData.Width, Height = SrcData.Height;
            int SrcStride = SrcData.Stride, DestStride = DestData.Stride;
            byte* SrcP, DestP;
            for (int Y = 0; Y < Height; Y++)
            {
                SrcP = (byte*)SrcData.Scan0 + Y * SrcStride;
                DestP = (byte*)DestData.Scan0 + Y * DestStride;
                for (int X = 0; X < Width; X++, SrcP++, DestP++)
                    *DestP = *SrcP > Threshold ? byte.MaxValue : byte.MinValue;
            }
            Src.UnlockBits(SrcData);
            Dest.UnlockBits(DestData);
        }
        private int GetThreshold()
        {
            return ImageProcess.GetThreshValue(HistGram, thresholdType, HistGramS);
        }
        private void DrawHistGram(Bitmap SrcBmp, int[] Histgram)
        {
            BitmapData HistData = SrcBmp.LockBits(new Rectangle(0, 0, SrcBmp.Width, SrcBmp.Height), ImageLockMode.ReadWrite, SrcBmp.PixelFormat);
            int X, Y, Max = 0;
            byte* P;
            for (Y = 0; Y < 256; Y++) if (Max < Histgram[Y]) Max = Histgram[Y];
            for (X = 0; X < 256; X++)
            {
                P = (byte*)HistData.Scan0 + X;
                for (Y = 0; Y < 100; Y++)
                {
                    if ((100 - Y) > Histgram[X] * 100 / Max)
                        *P = 220;
                    else
                        *P = 0;
                    P += HistData.Stride;
                }
            }

            P = (byte*)HistData.Scan0 + Thr;
            for (Y = 0; Y < 100; Y++)
            {
                *P = 255;
                P += HistData.Stride;
            }
            SrcBmp.UnlockBits(HistData);
        }
        public void Generate()
        {
            if (SrcBmp == null) throw new Exception("SrcBmp should not be null.");
            if (IsGrayBitmap(SrcBmp) == true)
                GrayBmp = SrcBmp;
            else
            {
                GrayBmp = ConvertToGrayBitmap(SrcBmp);
            }
            DestBmp = CreateGrayBitmap(GrayBmp.Width, GrayBmp.Height);
            GetHistGram(GrayBmp, HistGram);
            Thr = GetThreshold();
            DoBinaryzation(GrayBmp, DestBmp, Thr);
            DrawHistGram(HistBmp, HistGram);
            if (thresholdType == ImageProcess.ThresholdType.Minimum || thresholdType == ImageProcess.ThresholdType.Intermodes)
            {
                DrawHistGram(SmoothHistBmp, HistGramS);
            }
        }
    }
}
