using Dapper;
using System.Linq;
using Votacao.Data;
using Votacao.Service.DTO;
using Votacao.Service.Interface;

namespace Votacao.Service
{
    public class VoterDAO : IVoter
    {

        #region Public Methods

        public Voter Get(string identifier)
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                //cnn.SetPassword(ParametersSingleton.Instance.PasswordSqlite);
                cnn.Open();
                return cnn.Query<Voter>(
                                string.Format(@"SELECT  id Id,
                                                        identifier Identifier,
                                                        name Name, 
                                                        section_id SectionId
                                                FROM    voter 
                                                WHERE   identifier = '{0}'",
                                identifier)).FirstOrDefault();
            }
        }

        #endregion

    }
}
