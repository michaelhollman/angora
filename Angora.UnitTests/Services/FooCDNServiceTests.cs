using System;
using System.Configuration;
using System.Text;
using System.Threading;
using Angora.Services;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Angora.UnitTests.Services
{
    [TestClass]
    public class FooCDNServiceTests
    {
        private IUnityContainer unityContainer;

        [TestInitialize()]
        public void TestInitialize()
        {
            unityContainer = new UnityContainer();
            unityContainer.RegisterType<IFooCDNService, FooCDNService>();
        }


        [TestMethod]
        public void TestPostAndInfo()
        {
            var service = unityContainer.Resolve<IFooCDNService>();
            service.PostToBlob("c745b6e4-66fc-4eeb-ac64-5499ab4ec118", Encoding.ASCII.GetBytes("LolDerpDerp"), ".txt");
            string s = service.GetBlobInfo("c745b6e4-66fc-4eeb-ac64-5499ab4ec118");
            Assert.IsTrue(s.Contains("BlobSize\":11"));
        }

        [TestMethod]
        public void TestGetBlobURL()
        {
            var service = unityContainer.Resolve<IFooCDNService>();
            Assert.AreEqual(service.GetBlobURL("c745b6e4-66fc-4eeb-ac64-5499ab4ec118"), "http://foocdn.azurewebsites.net/api/content/c745b6e4-66fc-4eeb-ac64-5499ab4ec118");
        }

        [TestMethod]
        public void TestPutAndInfo()
        {
            var service = unityContainer.Resolve<IFooCDNService>();
            service.PutBlob("c745b6e4-66fc-4eeb-ac64-5499ab4ec118", "Memcache");
            string s = service.GetBlobInfo("c745b6e4-66fc-4eeb-ac64-5499ab4ec118");
            Assert.IsTrue(s.Contains("Location\":0"));
        }

        [TestMethod]
        public void TestCreatePostInfoAndDelete()
        {
            var service = unityContainer.Resolve<IFooCDNService>();
            string id = service.CreateNewBlob("text/plain");
            service.PostToBlob(id, Encoding.ASCII.GetBytes("delete me"), ".txt");
            Thread.Sleep(500);
            service.GetBlobInfo(id);
            Thread.Sleep(500);
            service.DeleteBlob(id);
            //made it this far?
            Assert.IsTrue(true);
        }

    }
}
