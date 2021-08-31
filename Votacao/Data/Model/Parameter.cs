using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Votacao.Data.Model
{
    [Table("parameter")]
    public class Parameter
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("key")]
        [MaxLength(25)]
        public string Key { get; set; }

        [Column("value")]
        public string Value { get; set; }
    }
}
