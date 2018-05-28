using System;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yurui.Tools.Test
{
    [TestClass]
    public class CaptchaUnitTest
    {

        private static Logger log = new Logger("CaptchaUnitTest");
        private static System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

        [TestMethod]
        public void TestGray()
        {

            string captcha01Path = $@"{System.Environment.CurrentDirectory}\imgs\Captcha01.jpg";

            watch.Start();
            Captcha
                .Gray(new Bitmap(captcha01Path), Captcha.GrayscaleType.BComponent)
                .Save($@"{System.Environment.CurrentDirectory}\imgs\Captcha01BComponent.jpg", ImageFormat.Jpeg);
            watch.Stop();
            TimeSpan timespan = watch.Elapsed;
            System.Diagnostics.Debug.WriteLine("灰度化B向量法消耗：{0}(毫秒)", timespan.TotalMilliseconds);
            log.Info($"灰度化B向量法消耗：{timespan.TotalMilliseconds}(毫秒)");

            watch.Reset();
            watch.Start();
            Captcha
                .Gray(new Bitmap(captcha01Path), Captcha.GrayscaleType.GComponent)
                .Save($@"{System.Environment.CurrentDirectory}\imgs\Captcha01GComponent.jpg", ImageFormat.Jpeg)
                ;
            watch.Stop();
            System.Diagnostics.Debug.WriteLine("灰度化G向量法消耗：{0}(毫秒)", timespan.TotalMilliseconds);
            log.Info($"灰度化G向量法消耗：{timespan.TotalMilliseconds}(毫秒)");
            watch.Reset();

            Captcha
                 .Gray(new Bitmap(captcha01Path), Captcha.GrayscaleType.RComponent)
                 .Save($@"{System.Environment.CurrentDirectory}\imgs\Captcha01RComponent.jpg", ImageFormat.Jpeg)
                 ;
            Captcha
                 .Gray(new Bitmap(captcha01Path), Captcha.GrayscaleType.Max)
                 .Save($@"{System.Environment.CurrentDirectory}\imgs\Captcha01Max.jpg", ImageFormat.Jpeg)
                 ;
            Captcha
                 .Gray(new Bitmap(captcha01Path), Captcha.GrayscaleType.Mean)
                 .Save($@"{System.Environment.CurrentDirectory}\imgs\Captcha01Mean.jpg", ImageFormat.Jpeg)
                 ;
            Captcha
                .Gray(new Bitmap(captcha01Path), Captcha.GrayscaleType.WeightedMean)
                .Save($@"{System.Environment.CurrentDirectory}\imgs\Captcha01WeightedMean.jpg", ImageFormat.Jpeg)
                ;
        }


        [TestMethod]
        public void TestGetHistGram()
        {
            string captcha01Path = $@"{System.Environment.CurrentDirectory}\imgs\Captcha01WeightedMean.jpg";


            watch.Reset();
            watch.Start();
            var h1 = Captcha.GetHistGram(new Bitmap(captcha01Path));
            watch.Stop();
            System.Diagnostics.Debug.WriteLine("GetHistGram消耗：{0}(毫秒)", watch.Elapsed.TotalMilliseconds);

            watch.Reset();
            watch.Start();
            var h2 = Captcha.GetHistGram2(new Bitmap(captcha01Path));
            watch.Stop();
            System.Diagnostics.Debug.WriteLine("GetHistGram2消耗：{0}(毫秒)", watch.Elapsed.TotalMilliseconds);

            if (h1.Length != h2.Length) Assert.Fail();
            for (var i = 0; i < h1.Length; i++)
            {
                if (h1[i] != h2[i])
                {
                    System.Diagnostics.Debug.WriteLine($"异常的序号{i},值分别是{h1[i]},{h2[i]}");
                    //Assert.Fail();
                }
            }

        }


        [TestMethod]
        public void TestOstu()
        {
            string captcha01Path = $@"{System.Environment.CurrentDirectory}\imgs\Captcha01WeightedMean.jpg";



            watch.Reset();
            watch.Start();
            var t2 = Captcha.GetThreshValue(new Bitmap(captcha01Path));
            watch.Stop();
            System.Diagnostics.Debug.WriteLine("t2消耗：{0}(毫秒)", watch.Elapsed.TotalMilliseconds);

            watch.Reset();
            watch.Start();
            var h1 = Captcha.GetHistGram(new Bitmap(captcha01Path));
            var t1 = Captcha.Threshold.GetOSTUThreshold(h1);
            watch.Stop();
            System.Diagnostics.Debug.WriteLine("t1消耗：{0}(毫秒)", watch.Elapsed.TotalMilliseconds);


        }

    }
}
