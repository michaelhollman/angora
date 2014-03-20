using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Angora.Services;

namespace Angora.UnitTests.Services
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestPostAndInfo()
        {
            var service = ServiceManager.GetService<IFooCDNService>();
            service.PostToBlob("c745b6e4-66fc-4eeb-ac64-5499ab4ec118", "../../../Angora.Web/Images/Rachael2.jpg");
            string s = service.GetBlobInfo("c745b6e4-66fc-4eeb-ac64-5499ab4ec118");

            Assert.IsTrue(s.Contains("BlobSize\":33507"));
        }


        [TestMethod]
        public void TestGetBlobURL()
        {
            var service = ServiceManager.GetService<IFooCDNService>();

            Assert.AreEqual(service.GetBlobURL("c745b6e4-66fc-4eeb-ac64-5499ab4ec118"), "http://foocdn.azurewebsites.net/api/content/c745b6e4-66fc-4eeb-ac64-5499ab4ec118");
        }

        [TestMethod]
        public void TestPutAndInfo()
        {
            var service = ServiceManager.GetService<IFooCDNService>();
            service.PutBlob("c745b6e4-66fc-4eeb-ac64-5499ab4ec118", "Memcache");
            string s = service.GetBlobInfo("c745b6e4-66fc-4eeb-ac64-5499ab4ec118");
            Assert.IsTrue(s.Contains("Location\":0"));
        }

        [TestMethod]
        public void TestCreatePostInfoAndDelete()
        {
            var service = ServiceManager.GetService<IFooCDNService>();
            string id = service.CreateNewBlob("image/jpg");
            service.PostToBlob(id, "../../../Angora.Web/Images/RachaelComb.jpg");
            service.GetBlobInfo(id);
            service.DeleteBlob(id);
        }
    }
}
