using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Angora.Services;

namespace Angora.UnitTests.Services
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.IsFalse(false);
        }

        [TestMethod]
        public void TestMethod2()
        {
            ServiceManager.GetService<IFooService>();
            Assert.IsTrue(true);
        }
    }
}
