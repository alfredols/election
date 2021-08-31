using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Votacao.Data.Model
{
    [Table("vote")]
    public class Vote
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("created_on")]
        public DateTime? CreationDateTime { get; set; }

        [Column("nickname_station")]
        public string NicknameStation { get; set; }

        public int? candidate_id { get; set; }

        [ForeignKey("candidate_id")]
        public Candidate Candidate { get; set; }
    }
}
