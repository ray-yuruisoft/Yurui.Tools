using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yurui.Tools.Cache;
namespace Yurui.Tools.Test
{
    [TestClass]
    public class CacheUnitTest
    {
        [TestMethod]
        public void TestGetOrCreate()
        {
            CacheService cacheService = new CacheService();
            try
            {
                var person = cacheService.GetOrCreate("int", () =>
                {
                    return new Person()
                    {
                        Name = "wangrui"
                    };
                });
                cacheService.Add("add", person, TimeSpan.Zero, TimeSpan.FromDays(1));
                cacheService.Add("add2", person, TimeSpan.FromDays(1), TimeSpan.MaxValue);
                var a = cacheService.Get("add");
                var b = cacheService.Get("add2");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public class Person
        {
            public string Name { get; set; }
            public int Sex { get; set; }
            public string Print(string str)
            {
                if (str != Name) Name = str;
                System.Diagnostics.Debug.WriteLine(Name);
                return "ok.";
            }

        }

    }
}
