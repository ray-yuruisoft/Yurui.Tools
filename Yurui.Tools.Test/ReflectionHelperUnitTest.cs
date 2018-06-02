using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yurui.Tools;
using Yurui.Tools.Reflection;

namespace Yurui.Tools.Test
{
    [TestClass]
    public class ReflectionHelperUnitTest
    {
        Person person = new Person
        {
            Name = "yurui",
            Sex = 1
        };
        private static System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

        [TestMethod]
        public void TestGetPropertyNameFromExpression()
        {
            var res = ReflectionHelper.GetPropertyNameFromExpression<Person>(c => c.Name);
            if ("Name" != res) Assert.Fail();
            var list = ReflectionHelper.GetPropertyNamesFromExpressions<Person>(new System.Linq.Expressions.Expression<Func<Person, object>>[] {
                c=>c.Name,
                c=>c.Sex
            });
        }

        [TestMethod]
        public void TestGetPropertyValue()
        {
            var name = ReflectionHelper.GetPropertyValue(person, "Name");
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("person", person);
            var name2 = ReflectionHelper.GetPropertyValueDynamic(dic, "person");
        }

        [TestMethod]
        public void TestGetProperties()
        {
            var infos = ReflectionHelper.GetProperties(person.GetType());
            dynamic person2 = ReflectionHelper.GetDefault(typeof(Person));

        }

        [TestMethod]
        public void TestGetInstanceValue()
        {
            watch.Reset();
            watch.Start();
            var name2 = ReflectionHelper.GetPropertyValue(person, "Name");
            watch.Stop();
            System.Diagnostics.Debug.WriteLine("GetPropertyValue消耗：{0}(毫秒)", watch.Elapsed.TotalMilliseconds);
            watch.Reset();
            watch.Start();
            var name = person.GetInstanceValue("Name");
            watch.Stop();
            System.Diagnostics.Debug.WriteLine("GetInstanceValue消耗：{0}(毫秒)", watch.Elapsed.TotalMilliseconds);
        }

        [TestMethod]
        public void TestInvokeMethodOrGetProperty()
        {
            var ret = typeof(Person).InvokeMethodOrGetProperty<string>("Print", null, "mayuru");
            //var ret2 = typeof(Person).InvokeMethodOrGetProperty<string>("Name", new string[] { "wangrui" } , null);
            System.Diagnostics.Debug.WriteLine(ret);
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
