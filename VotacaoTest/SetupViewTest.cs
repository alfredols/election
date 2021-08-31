using Microsoft.VisualStudio.TestTools.UnitTesting;
using Votacao.Data;
using Votacao.View.Setup;

namespace VotacaoTest
{
    [TestClass]
    public class SetupViewTest
    {
        [ClassInitialize]
        public static void SetupElectionSQLite(TestContext context)
        {
            ParametersSingleton.Instance.IsTest = true;
        }

        [TestMethod]
        public void SetupFirstStepTest()
        {
            DefineTypeStationViewModel viewModel = new DefineTypeStationViewModel();
            viewModel.TypeStation = Votacao.Service.DTO.TypeStation.BALLOT_BOX;
            viewModel.NicknameStation = "urna";

            Assert.IsTrue(viewModel.ValidateScreen());
        }
    }
}