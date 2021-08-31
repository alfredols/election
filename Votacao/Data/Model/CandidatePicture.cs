using System.ComponentModel.DataAnnotations.Schema;

namespace Votacao.Data.Model
{
    [Table("candidate_picture")]
    public class CandidatePicture
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("data")]
        public byte[] Data { get; set; }

        public int candidate_id { get; set; }

        [ForeignKey("candidate_id")]
        public Candidate Candidate { get; set; }
    }
}
