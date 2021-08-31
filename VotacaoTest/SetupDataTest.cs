using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Votacao.Data;
using Votacao.Data.Model;

namespace VotacaoTest
{
    [TestClass]
    public class SetupDataTest
    {
        public TestContext TestContext { get; set; }

        private static TestContext _testContext;

        [ClassInitialize]
        public static void SetupTests(TestContext testContext)
        {
            _testContext = testContext;
        }

        [TestMethod]
        public void SetupElectionSQLite()
        {
            //string pathSQLite = Path.GetDirectoryName(Path.GetDirectoryName(TestContext.TestDir)) + @"\Votacao\station.dat";
            //using (var ctx = new VotationContext(@"Data Source=" + pathSQLite))
            //{
            //    ctx.Parameters.RemoveRange(ctx.Parameters);
            //    ctx.Elections.RemoveRange(ctx.Elections);
            //    ctx.Users.RemoveRange(ctx.Users);
            //    ctx.Regions.RemoveRange(ctx.Regions);
            //    ctx.Sites.RemoveRange(ctx.Sites);
            //    ctx.Sections.RemoveRange(ctx.Sections);

            //    var parameter = new Parameter() { Key = "HOST", Value = "http://testeapuracao.prodam/api" };
            //    ctx.Parameters.Add(parameter);

            //    var election = new Election() { Name = "Eleição Conselho da Habitação" };
            //    ctx.Elections.Add(election);

            //    var region01 = new Region() { Name = "Vila Mariana", Election = election };
            //    ctx.Regions.Add(region01);

            //    var site0101 = new Site() { Name = "Escola 01", Region = region01 };
            //    ctx.Sites.Add(site0101);

            //    var site0102 = new Site() { Name = "Escola 02", Region = region01 };
            //    ctx.Sites.Add(site0102);

            //    var section010201 = new Section() { Number = 1, Site = site0102 };
            //    ctx.Sections.Add(section010201);

            //    var region02 = new Region() { Name = "Belem", Election = election };
            //    ctx.Regions.Add(region02);

            //    var region03 = new Region() { Name = "Itaquera", Election = election };
            //    ctx.Regions.Add(region03);

            //    var site0301 = new Site() { Name = "Escola 03", Region = region03 };
            //    ctx.Sites.Add(site0301);

            //    var section030101 = new Section() { Number = 1, Site = site0301 };
            //    ctx.Sections.Add(section030101);

            //    //var user = new User() { Login = "teste", AccessToken = BCrypt.Net.BCrypt.HashPassword("123456", BCrypt.Net.BCrypt.GenerateSalt(5)) };

            //    //ctx.Users.Add(user);

            //    ctx.SaveChanges();
            //}
        }
    }
}
