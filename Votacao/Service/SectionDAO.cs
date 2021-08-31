using System.Collections.Generic;
using Votacao.Data;
using Votacao.Service.DTO;
using Votacao.Service.Interface;
using System.Linq;
using Dapper;

namespace Votacao.Service
{
    public class SectionDAO : ISection
    {
        
        #region Public Methods

        public List<Section> ListBySite(int siteId)
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                //cnn.SetPassword(ParametersSingleton.Instance.PasswordSqlite);
                cnn.Open();
                return cnn.Query<Section>(@"SELECT  id Id,
                                                    number Number 
                                            FROM    section 
                                            WHERE   site_id = @siteId",
                                            new { siteId }).ToList();
            }
        }

        public Section Get(int sectionId)
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                //cnn.SetPassword(ParametersSingleton.Instance.PasswordSqlite);
                cnn.Open();
                return cnn.Query<Section>(@"SELECT  id Id,
                                                    number Number,
                                                    site_id SiteId
                                            FROM    section 
                                            WHERE   id = @sectionId",
                                            new { sectionId }).FirstOrDefault();
            }
        }

        #endregion

    }
}
