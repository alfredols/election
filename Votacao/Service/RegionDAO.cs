using System.Collections.Generic;
using Votacao.Data;
using Votacao.Service.DTO;
using Votacao.Service.Interface;
using System.Linq;
using Dapper;

namespace Votacao.Service
{
    public class RegionDAO : IRegion
    {
        
        #region Public Methods

        public List<Region> List()
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                //cnn.SetPassword(ParametersSingleton.Instance.PasswordSqlite);
                cnn.Open();
                return cnn.Query<Region>(@" SELECT  id Id,
                                                    name Name
                                            FROM    region").ToList();
            }
        }

        public Region Get(int regionId)
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                //cnn.SetPassword(ParametersSingleton.Instance.PasswordSqlite);
                cnn.Open();
                return cnn.Query<Region>(@" SELECT  id Id,
                                                    name Name 
                                            FROM    region 
                                            WHERE   id = @regionId",
                                            new { regionId }).FirstOrDefault();
            }
        }

        #endregion

    }
}
