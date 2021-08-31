
namespace Votacao.Service.DTO
{
    public class Election
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Nickname { get; set; }

        public byte[] ElectionLogo { get; set; }

        public byte[] secretaryLogo { get; set; }

    }
}
