using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votacao.Service.DTO
{
    public class StatusTransactionStationStringConverter
    {

        public string ConvertStatusStationToString(StatusTransactionStationStatus? stationType)
        {
            if (stationType != null)
            {
                if (stationType == StatusTransactionStationStatus.AVAILABLE)
                    return "AVAILABLE";
                else
                    return "UNAVAILABLE";
            }
            else
            {
                return null;
            }
        }
               
        public StatusTransactionStationStatus ConvertStringToStationStatus(string value)
        {
            if (value == "AVAILABLE")
                return StatusTransactionStationStatus.AVAILABLE;
            else
                return StatusTransactionStationStatus.UNAVAILABLE;
        }


    }
}
