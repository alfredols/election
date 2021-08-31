using System;
using System.Globalization;
using System.Windows.Data;

namespace Votacao.Service.DTO
{
    public class StatusStationStringConverter
    {
        public string ConvertStatusStationToString(StatusStation? stationType)
        {
            if (stationType != null)
            {
                if (stationType == StatusStation.CONFIGURED)
                    return "CONFIGURED";
                else if(stationType == StatusStation.INITIALIZED)
                    return "INITIALIZED";
                else
                    return "FINALIZED";
            }
            else 
            {
                return null;
            }
        }

        public StatusStation ConvertStringToStatusStation(string value)
        {
            if (value == "INITIALIZED")
                return StatusStation.INITIALIZED;
            else if (value == "CONFIGURED")
                return StatusStation.CONFIGURED;
            else
                return StatusStation.FINALIZED;
        }
    }
}
