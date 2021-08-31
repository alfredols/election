using System.Collections.Generic;
using Votacao.Data;
using Votacao.Service.DTO;
using Votacao.Service.Interface;
using System.Linq;
using Dapper;

namespace Votacao.Service
{
    public class SiteDAO : ISite
    {

        #region Public Methods

        public List<Site> ListByRegion(int regionId)
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                //cnn.SetPassword(ParametersSingleton.Instance.PasswordSqlite);
                cnn.Open();
                return cnn.Query<Site>(@"   SELECT  id Id,
                                                    name Name 
                                            FROM    site 
                                            WHERE   region_id = @regionId",
                                            new { regionId }).ToList();
            }
        }

        public Site Get(int siteId)
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                //cnn.SetPassword(ParametersSingleton.Instance.PasswordSqlite);
                cnn.Open();
                return cnn.Query<Site>(@"   SELECT  id Id,
                                                    name Name,
                                                    address Address,
                                                    region_id RegionId
                                            FROM    site 
                                            WHERE   id = @siteId",
                                            new { siteId }).FirstOrDefault();
            }
        }

        #endregion

    }
}
