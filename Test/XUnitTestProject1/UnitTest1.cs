using System;
using Xunit;
using MMCApi.Controllers;


namespace XUnitTestProject1
{
    public class UnitTest1
    {
        ValuesController vc = new ValuesController();

       
         [Fact]
        public void GetReturnValue()
        {
           var str2 = vc.Get(1);
            Assert.Equal("Atul",str2.Value);

        }

        [Fact]
        public void Test1()
        {

        }
    }
}
