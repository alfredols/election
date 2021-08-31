namespace Votacao.Service.DTO
{
    public class TypeVotingStringConverter
    {
        public string ConvertTypeVotingToString(TypeVoting typeVoting)
        {
            if (typeVoting == TypeVoting.CANDIDATE)
                return "CANDIDATE";
            else
                return "COALITION";
        }

        public TypeVoting ConvertStringToTypeVoting(string value)
        {
            if (!string.IsNullOrEmpty(value)
                && value.IndexOf("COALITION") > -1)
                return TypeVoting.COALITION;
            else
                return TypeVoting.CANDIDATE;
        }
    }
}
