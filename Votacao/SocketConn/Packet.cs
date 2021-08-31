namespace Votacao.SocketConn
{
    public enum PacketType
    {
        DISCOVERY, 
        CONFIRMATION,
        DISCONNECTED,
        AVAILABLE,
        UNAVAILABLE,
        OPEN_VOTE,
        VOTE,
        VOTE_SAVED,
        OPEN_SETTINGS,
        CANCEL_VOTE,
        ELECTION_FINISHED,
        CONFIGURATION_FINISHED,
        ELECTION_STARTED
    }
    public class Packet
    {
        public string Message { get; set; }
        public string NicknameStation { get; set; }
        public string HostName { get; set; }
        public string IP { get; set; }
        public string IPFrom { get; set; }
        public PacketType PacketType { get; set; }
    }
}
