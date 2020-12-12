using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using ClientApp;
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
                ServerAsync server = new ServerAsync(IPAddress.Parse("127.0.0.1"), 3000);
                Assert.Fail();
            }
            catch(AssertFailedException)
            {
                Assert.Fail();
            }
            catch (Exception e)
            {

            }
        }

        [TestMethod]
        public void TestLoginMethod()
        {

        }
    }
}
