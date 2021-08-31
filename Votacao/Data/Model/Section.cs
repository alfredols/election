using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Votacao.Data.Model
{
    [Table("section")]
    public class Section
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("number")]
        public int Number { get; set; }

        public int site_id { get; set; }

        [ForeignKey("site_id")]
        public Site Site { get; set; }

        public ICollection<Voter> Voters { get; set; }
    }
}
