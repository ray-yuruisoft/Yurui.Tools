//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Yurui.Tools.Captcha
//{
//    public class CaptchaHandler
//    {

//        #region private

//        private Bitmap _GrayBmp;
//        private int[] _HistGramS = new int[256];

//        #endregion

//        public bool isSmooth;
//        public bool isGray;
//        public Bitmap SrcBmp { get; set; }
//        public Bitmap DestBmp { get; private set; }
//        public Bitmap GrayBmp
//        {
//            get
//            {
//                if (_GrayBmp == null)
//                {
//                    return ImageProcess.Gray(SrcBmp, grayscaleType);
//                }
//                return _GrayBmp;
//            }
//            private set
//            {
//                _GrayBmp = value;
//            }
//        }
//        public ImageProcess.ThresholdType thresholdType = ImageProcess.ThresholdType.OSTU;
//        public ImageProcess.GrayscaleType grayscaleType = ImageProcess.GrayscaleType.WeightedMean;
//        public int[] HistGram = new int[256];
//        public int[] HistGramS
//        {
//            get
//            {
//                if (_HistGramS == null)
//                {
//                    Array.Copy(HistGram, _HistGramS, 256);
//                    ImageProcess.SmoothHistGram(_HistGramS);
//                }
//                return _HistGramS;
//            }
//            set
//            {
//                Array.Copy(value, _HistGramS, 256);
//            }
//        }
//        public int Thr { get; set; }
//        public void Reload()
//        {
//            #region 1.灰度化

//            if (isGray)
//            {
//                GrayBmp = ImageProcess.Gray(SrcBmp, grayscaleType);
//            }

//            #endregion

//            #region 2.生成直方图

//            HistGram = ImageProcess.GetHistGram(isGray ? GrayBmp : SrcBmp);

//            #endregion

//            #region 3.直方图平滑化

//            if (isSmooth)
//            {
//                Array.Copy(HistGram, _HistGramS, 256);
//                ImageProcess.SmoothHistGram(_HistGramS);
//            }

//            #endregion

//            #region 4.获取阀值

//            if (thresholdType != ImageProcess.ThresholdType.Minimum && thresholdType != ImageProcess.ThresholdType.Intermodes)
//            {
//                Thr = ImageProcess.GetThreshValue(isSmooth ? HistGramS : HistGram, thresholdType);
//            }
//            else
//            {
//                Thr = ImageProcess.GetThreshValue(HistGram, thresholdType, HistGramS);
//            }

//            #endregion

//            #region 5.二值化

//            DestBmp = ImageProcess.PBinary(SrcBmp, Thr);

//            #endregion
//        }
//        public CaptchaHandler(Image img, bool isGrayFirst = false, bool isSmoothFirst = false)
//        {
//            SrcBmp = new Bitmap(img);
//            isSmooth = isSmoothFirst;
//            isGray = isGrayFirst;
//            Reload();
//        }

//    }
//}
