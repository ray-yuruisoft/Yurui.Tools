using System;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yurui.Tools.Test
{
    [TestClass]
    public class ImageProcessUnitTest
    {

        private static Logger log = new Logger("ImageProcessUnitTest");
        [TestMethod]
        public void TestGray()
        {

            string captcha01Path = $@"{System.Environment.CurrentDirectory}\imgs\Captcha01.jpg";
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

            watch.Start();
            ImageProcess
                .Gray(new Bitmap(captcha01Path), ImageProcess.GrayscaleType.BComponent)
                .Save($@"{System.Environment.CurrentDirectory}\imgs\Captcha01BComponent.jpg", ImageFormat.Jpeg);
            watch.Stop();
            TimeSpan timespan = watch.Elapsed;
            System.Diagnostics.Debug.WriteLine("灰度化B向量法消耗：{0}(毫秒)", timespan.TotalMilliseconds);
            log.Info($"灰度化B向量法消耗：{timespan.TotalMilliseconds}(毫秒)");

            watch.Reset();
            watch.Start();
            ImageProcess
                .Gray(new Bitmap(captcha01Path), ImageProcess.GrayscaleType.GComponent)
                .Save($@"{System.Environment.CurrentDirectory}\imgs\Captcha01GComponent.jpg", ImageFormat.Jpeg)
                ;
            watch.Stop();
            System.Diagnostics.Debug.WriteLine("灰度化G向量法消耗：{0}(毫秒)", timespan.TotalMilliseconds);
            log.Info($"灰度化G向量法消耗：{timespan.TotalMilliseconds}(毫秒)");
            watch.Reset();

            ImageProcess
                 .Gray(new Bitmap(captcha01Path), ImageProcess.GrayscaleType.RComponent)
                 .Save($@"{System.Environment.CurrentDirectory}\imgs\Captcha01RComponent.jpg", ImageFormat.Jpeg)
                 ;
            ImageProcess
                 .Gray(new Bitmap(captcha01Path), ImageProcess.GrayscaleType.Max)
                 .Save($@"{System.Environment.CurrentDirectory}\imgs\Captcha01Max.jpg", ImageFormat.Jpeg)
                 ;
            ImageProcess
                 .Gray(new Bitmap(captcha01Path), ImageProcess.GrayscaleType.Mean)
                 .Save($@"{System.Environment.CurrentDirectory}\imgs\Captcha01Mean.jpg", ImageFormat.Jpeg)
                 ;
            ImageProcess
                .Gray(new Bitmap(captcha01Path), ImageProcess.GrayscaleType.WeightedMean)
                .Save($@"{System.Environment.CurrentDirectory}\imgs\Captcha01WeightedMean.jpg", ImageFormat.Jpeg)
                ;
        }
    }
}
