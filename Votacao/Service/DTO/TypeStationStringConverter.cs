using System;
using System.Globalization;
using System.Windows.Data;

namespace Votacao.Service.DTO
{
    public class TypeStationStringConverter
    {
        public string ConvertStationToString(TypeStation? stationType)
        {
            if (stationType != null)
            {
                if (stationType == TypeStation.VOTING_OFFICER)
                    return "VOTING_OFFICER";
                else if (stationType == TypeStation.BALLOT_BOX)
                    return "BALLOT_BOX";
                else
                    return "VOTER_SEARCH";
            }
            else
            {
                return null;
            }
        }

        public TypeStation ConvertStringToStation(string value)
        {
            if (value == "VOTING_OFFICER")
                return TypeStation.VOTING_OFFICER;
            else if (value == "BALLOT_BOX")
                return TypeStation.BALLOT_BOX;
            else
                return TypeStation.VOTER_SEARCH;
        }
    }
}
