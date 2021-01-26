using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using ClientApplication;
using ServerLibrary;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestWrongPortMethod()
        {
            try
            {
                ServerAsync server = new ServerAsync(IPAddress.Parse("127.0.0.1"), 1000);
                Assert.Fail();
            }
            catch(Exception e)
            {
               Assert.AreEqual(e.Message, "błędna wartość portu");
            }

        }

        [TestMethod]
        public void TestCorrectPortMethod()
        {
            try
            {
                ServerAsync server = new ServerAsync(IPAddress.Parse("127.0.0.1"), 3000);
                Assert.AreEqual(3000, server.Port);
            }
            catch (Exception)
            {
                Assert.Fail();
            }

        }



    }
}
