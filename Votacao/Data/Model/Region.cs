using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votacao.Data.Model
{
    [Table("region")]
    public class Region
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        [MaxLength(200)]
        public string Name { get; set; }

        public int election_id { get; set; }

        [ForeignKey("election_id")]
        public Election Election { get; set; }

        public ICollection<Site> Sites { get; set; }

        public ICollection<Candidate> Candidates { get; set; }
    }
}
