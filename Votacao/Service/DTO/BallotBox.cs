using System;


namespace Votacao.Service.DTO
{
    public class BallotBox 
    {
        public string IP { get; set; }

        public string HostName { get; set; }

        public string DateConnection { get; set; }

        public string NicknameStation { get; set; }

        public bool IsConnected { get; set; }

        public bool IsPaired { get; set; }

        public bool SendElector { get; set; }
    }
}
