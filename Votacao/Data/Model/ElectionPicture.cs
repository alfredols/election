using System.ComponentModel.DataAnnotations.Schema;

namespace Votacao.Data.Model
{
    [Table("election_picture")]
    public class ElectionPicture
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("electionLogo")]
        public byte[] ElectionLogo { get; set; }
        
        [Column("secretaryLogo")]
        public byte[] SecretaryLogo { get; set; }

        public int election_id { get; set; }


        [ForeignKey("election_id")]
        public Election Election { get; set; }

    }
}
