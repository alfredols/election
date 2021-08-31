using Microsoft.VisualStudio.TestTools.UnitTesting;
using Votacao.Data;

namespace VotacaoTest
{
    [TestClass]
    public class ParametersSingletonTest
    {
        [TestMethod]
        public void AddNewParameter()
        {
            ParametersSingleton.Instance.Region = 45;
            Assert.IsTrue(ParametersSingleton.Instance.Region == 45);
        }


        [TestMethod]
        public void AddNewValueParameter()
        {
            ParametersSingleton.Instance.Region = 46;
            Assert.IsTrue(ParametersSingleton.Instance.Region == 46);
        }
    }
}
