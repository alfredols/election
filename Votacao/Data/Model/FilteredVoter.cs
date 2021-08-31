using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Votacao.Data.Model
{
    [Table("filtered_voter")]
    public class FilteredVoter
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("identifier")]
        [MaxLength(25)]
        public string Identifier { get; set; }

        [Column("name")]
        [MaxLength(200)]
        public string Name { get; set; }

        [Column("tre_zone")]
        [MaxLength(10)]
        public string TREZone { get; set; }

        [Column("has_voted")]
        public bool? HasVoted { get; set; }

        public int election_id { get; set; }

        [ForeignKey("election_id")]
        public Election Election { get; set; }

        public int section_id { get; set; }

        [ForeignKey("section_id")]
        public Section Section { get; set; }
    }
}
