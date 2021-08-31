using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Votacao.Data.Model
{
    [Table("site")]
    public class Site
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        [MaxLength(200)]
        public string Name { get; set; }

        public int region_id { get; set; }

        [ForeignKey("region_id")]
        public Region Region { get; set; }

        public ICollection<Section> Sections { get; set; }
    }
}
