using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Votacao.Data.Model
{
    [Table("candidate")]
    public class Candidate
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Column("nickname")]
        [MaxLength(100)]
        public string Nickname { get; set; }

        [Column("number")]
        public int? Number { get; set; }

        [Column("valid")]
        public bool? Valid { get; set; }

        public int region_id { get; set; }

        [ForeignKey("region_id")]
        public Region Region { get; set; }
    }
}
