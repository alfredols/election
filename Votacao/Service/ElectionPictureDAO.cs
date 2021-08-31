using Dapper;
using System;
using System.Linq;
using Votacao.Data;
using Votacao.Service.DTO;

namespace Votacao.Service
{
    public class ElectionPictureDAO
    {
        #region Attributes

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        public Election GetElectionPic(string electionId)
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                try
                {
                    //cnn.SetPassword(ParametersSingleton.Instance.PasswordSqlite);
                    cnn.Open();

                    if (String.IsNullOrEmpty(electionId))
                    {
                        electionId = ParametersSingleton.Instance.Election.ToString();
                    }

                    int id = Convert.ToInt32(electionId);

                    return cnn.Query<Election>(@"   SELECT  id Id
                                                            secretaryLogo SecretaryLogo,
                                                            electionLogo ElectionLogo
                                                    FROM    election_picture 
                                                    WHERE   election_id = @id",
                                                    new { id }).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    log.Error("Table election_picture was not created", ex);
                    return null;
                }
            }
        }

    }
}
