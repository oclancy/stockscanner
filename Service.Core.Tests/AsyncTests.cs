using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Service.Core.Tests
{
    [TestClass]
    public class AsyncTests
    {
        class TestClass
        {
            public string Data{get;set;}
            public TestClass TestProperty { get; set; }
        }

        [TestMethod]
        public void AwaitParameterPassing()
        {
            var t = Task.Factory.StartNew(async () =>
                {
                    var test = new TestClass() { Data = "Hello" };

                    await Task.Factory.StartNew(() => DoWork(test));

                    Assert.AreEqual("Goodbye", test.Data);
                    Assert.IsNotNull(test.TestProperty);
                    Assert.AreEqual("Nested", test.TestProperty.Data);
                });

            t.Wait();

        }

        

        private void DoWork(TestClass test)
        {
            Thread.Sleep(1000);
            test.TestProperty = new TestClass(){Data="Nested"};
            test.Data = "Goodbye";
        }
    }
}
