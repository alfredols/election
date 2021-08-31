using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Votacao.Data.Model
{
    [Table("election")]
    public class Election
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        [MaxLength(200)]
        public string Name { get; set; }

        [Column("voting")]
        [MaxLength(9)]
        public string Voting { get; set; }

        [Column("votesPerVoter")]
        public int VotesPerVoter { get; set; }

        public ICollection<Region> Regions { get; set; }

        public ICollection<Voter> Voters { get; set; }
    }
}
