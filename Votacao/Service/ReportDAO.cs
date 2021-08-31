using Dapper;
using System.Collections.Generic;
using System.Linq;
using Votacao.Data;
using Votacao.Service.DTO;
using Votacao.Service.Interface;

namespace Votacao.Service
{
    public class ReportDAO : IReport
    {
        public void Save(DTO.Report report)
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                //cnn.SetPassword(ParametersSingleton.Instance.PasswordSqlite);
                cnn.Open();
                cnn.Execute(@"INSERT INTO report (identifier, path, created_on) VALUES (@identifier, @path, datetime('now'))",
                    new
                    {
                        report.Identifier,
                        report.Path
                    });
            }
        }

        public List<DTO.Report> List()
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                //cnn.SetPassword(ParametersSingleton.Instance.PasswordSqlite);
                cnn.Open();
                return cnn.Query<Report>(@" SELECT  identifier Identifier,
                                                    path Path
                                            FROM    report").ToList();
            }
        }
    }
}
