using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yurui.Tools.Captcha
{
    public class CaptchaHandler
    {

        public Bitmap SrcBmp { get; set; }
        public Bitmap DestBmp { get; set; }

        public Bitmap GrayBmp
        {
            get
            {
                if (_GrayBmp == null)
                {
                    return ImageProcess.Gray(SrcBmp, grayscaleType);
                }
                return _GrayBmp;
            }
            set
            {
                _GrayBmp = value;
            }
        }
        private Bitmap _GrayBmp;


        private bool init = false;
        private ImageProcess.ThresholdType thresholdType = ImageProcess.ThresholdType.OSTU;
        private ImageProcess.GrayscaleType grayscaleType = ImageProcess.GrayscaleType.WeightedMean;


        public int[] HistGram = new int[256];
        private int[] _HistGramS = new int[256];

        public int[] HistGramS
        {
            get
            {
                if (_HistGramS == null)
                {
                    Array.Copy(HistGram, _HistGramS, 256);
                    ImageProcess.SmoothHistGram(_HistGramS);
                }
                return _HistGramS;
            }
            set
            {
                Array.Copy(value, _HistGramS, 256);
            }
        }

        private int Thr;
        private bool _isSmoothFirst;
        private bool _isGrayFirst;


        public CaptchaHandler(Image img, bool isGrayFirst = false, bool isSmoothFirst = false)
        {
            SrcBmp = new Bitmap(img);
            _isSmoothFirst = isSmoothFirst;
            _isGrayFirst = isGrayFirst;
            Init();
        }

        public void Init()
        {
            #region 1.灰度化
            if (_isGrayFirst)
            {
                GrayBmp = ImageProcess.Gray(SrcBmp, grayscaleType);
            }
            #endregion

            #region 2.生成直方图
            HistGram = ImageProcess.GetHistGram(_isGrayFirst ? GrayBmp : SrcBmp);
            #endregion

            #region 3.直方图平滑化
            if (_isSmoothFirst)
            {
                Array.Copy(HistGram, _HistGramS, 256);
                ImageProcess.SmoothHistGram(_HistGramS);
            }
            #endregion

            #region 4.获取阀值
            Thr = ImageProcess.GetThreshValue(_isSmoothFirst ? HistGramS : HistGram, thresholdType);
            #endregion

            #region 5.二值化
            DestBmp = ImageProcess.PBinary(SrcBmp, Thr);
            #endregion

            init = true;
        }

    }
}
