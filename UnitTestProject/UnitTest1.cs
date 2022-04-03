using Microsoft.VisualStudio.TestTools.UnitTesting;
using Insurance_app.SupportClasses;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreEqual("Notice",Msg.Notice);
        }
    }
}
