using System;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yurui.Tools.Captcha;

namespace Yurui.Tools.Test
{
    [TestClass]
    public class CaptchaUnitTest
    {

        private static Logger log = new Logger("CaptchaUnitTest");
        private static System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        [TestMethod]
        public void TestBinaryzation()
        {
            string path = $@"{System.Environment.CurrentDirectory}\imgs\";
            watch.Reset();
            watch.Start();
            Binaryzation binaryzation = new Binaryzation
            {
                SrcBmp = new Bitmap(path + "Codeimg1.jpg")
            };
            binaryzation.Generate();
            watch.Stop();
            log.Info($"二值化消耗：{watch.Elapsed.TotalMilliseconds}(毫秒)");

            string path2 = path + @"binaryzation\";
            if (!System.IO.Directory.Exists(path2))
            {
                System.IO.Directory.CreateDirectory(path2);
            }

            binaryzation.DestBmp.Save(path2 + "Codeimg1_DestBmp.jpg");//二值化图
            binaryzation.GrayBmp.Save(path2 + "Codeimg1_GrayBmp.jpg");//灰度化图
            binaryzation.HistBmp.Save(path2 + "Codeimg1_HistBmp.jpg");//直方图

            binaryzation.thresholdType = ImageProcess.ThresholdType.Minimum;
            binaryzation.Generate();         
            binaryzation.DestBmp.Save(path2 + "Codeimg1_DestBmp_Minimum.jpg");//二值化图            
            binaryzation.GrayBmp.Save(path2 + "Codeimg1_GrayBmp_Minimum.jpg");//灰度化图          
            binaryzation.HistBmp.Save(path2 + "Codeimg1_HistBmp_Minimum.jpg");//直方图           
            binaryzation.SmoothHistBmp.Save(path2 + "Codeimg1_SmoothHistBmp_Minimum.jpg");//平滑后直方图
        }
    }
}
