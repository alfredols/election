namespace Votacao.Service.DTO
{
    public enum StatusStation
    {
        // After setup between voting station and ballot box, station configured
        CONFIGURED, 
        // After zeresima generated and votation initiated
        INITIALIZED,
        // After BU generated and election finalized
        FINALIZED 
    }
}
