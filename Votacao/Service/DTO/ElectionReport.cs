using System;

namespace Votacao.Service.DTO
{
    [Serializable]
    public class ElectionReport
    {
        public string ElectionName { get; set; }
              
        public int SumWhiteVote { get; set; }

        public int SumNullVote { get; set; }

        public int SumTotalVote{ get; set; }

        public string PollingStation { get; set; }

        public string DateGenerate { get; internal set; }

        public byte[] QRCodeElection { get; set; }
        
        public string Region { get; set; }

        public string Site { get; set; }

        public string IDUE { get; set; }

        public string Section { get; set; }
        
        public byte[] ElectionLogo { get; set; }

        public byte[] SecretaryLogo { get; set; }
    }
}