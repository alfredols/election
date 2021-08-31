using Dapper;
using System.Linq;
using System.Text;
using Votacao.Data;
using Votacao.Service.DTO;
using Votacao.Service.Interface;

namespace Votacao.Service
{
    public class FilteredVoterDAO : IFilteredVoter
    {

        #region Public Methods

        public void Populate(int sectionId)
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                StringBuilder cmd = new StringBuilder();
                cmd.Append("INSERT INTO filtered_voter( identifier, ");
                cmd.Append("                            name,       ");
                cmd.Append("                            has_voted,  ");
                cmd.Append("                            tre_zone,   ");
                cmd.Append("                            election_id,");
                cmd.Append("                            section_id) ");
                cmd.Append("SELECT  identifier,                     ");
                cmd.Append("        name,                           ");
                cmd.Append("        has_voted,                      ");
                cmd.Append("        tre_zone,                       ");
                cmd.Append("        election_id,                    ");
                cmd.Append("        section_id                      ");
                cmd.Append("FROM    voter                           ");
                cmd.Append("WHERE   section_id  = @sectionId;       ");
               
                //cnn.SetPassword(ParametersSingleton.Instance.PasswordSqlite);
                cnn.Open();
                cnn.Execute(cmd.ToString(), new { sectionId });
            }
        }

        public void Clear()
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                //cnn.SetPassword(ParametersSingleton.Instance.PasswordSqlite);
                cnn.Open();
                cnn.Execute("DELETE FROM filtered_voter");
            }
        }

        public Voter Get(string identifier, int sectionId)
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                //cnn.SetPassword(ParametersSingleton.Instance.PasswordSqlite);
                cnn.Open();
                return cnn.Query<Voter>(@"  SELECT  id Id, 
                                                    identifier Identifier, 
                                                    name Name, 
                                                    has_voted HasVoted 
                                            FROM    filtered_voter 
                                            WHERE   identifier = @identifier
                                            AND     section_id = @sectionId",
                                            new { identifier, sectionId }).FirstOrDefault();
            }
        }

        public bool IsEmpty()
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                //cnn.SetPassword(ParametersSingleton.Instance.PasswordSqlite);
                cnn.Open();
                var count = cnn.Query<int>(@"   SELECT  COUNT(identifier) 
                                                FROM    filtered_voter").FirstOrDefault();

                return count == 0;
            }
        }

        public Voter Get(string identifier)
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                //cnn.SetPassword(ParametersSingleton.Instance.PasswordSqlite);
                cnn.Open();
                return cnn.Query<Voter>(@"  SELECT  id Id, 
                                                    identifier Identifier, 
                                                    name Name, 
                                                    has_voted HasVoted,
                                                    section_id SectionId
                                            FROM    filtered_voter 
                                            WHERE   identifier = @identifier",
                                            new { identifier }).FirstOrDefault();
            }
        }

        public void Voted(int voterId)
        {
            using (var cnn = SQLiteConnectionFactory.Create())
            {
                //cnn.SetPassword(ParametersSingleton.Instance.PasswordSqlite);
                cnn.Open();
                cnn.Execute("UPDATE filtered_voter SET has_voted = true WHERE id = @voterId", new { voterId });
            }
        }

        #endregion

    }
}
