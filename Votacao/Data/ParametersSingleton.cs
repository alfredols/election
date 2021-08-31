using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Votacao.Service;
using Votacao.Service.DTO;

namespace Votacao.Data
{
    public sealed class ParametersSingleton
    {
        #region Private Attributes

        private static readonly object padlock = new object();
        private static ParametersSingleton instance = null;
        private string host;
        private int region;
        private int site;
        private int section;
        private TypeVoting typeVoting;
        private TypeStation? typeStation;
        private string idVotingStation;
        private string nicknameStation;
        private string electionName;
        private int votesPerVoter;
        private int election;
        private string ipStation;
        private List<BallotBox> ballotBoxes;
        private string token;
        private StatusStation? statusStation;
        private StatusTransactionStationStatus? statusVotingStation;
        private VotingStationOfficer votingStationOfficer;
        private Dictionary<string, string> memoryTest = new Dictionary<string, string>();
        private string openBallotBoxDate;
        private string closeBallotBoxDate;
        private string passwordSqlite;


        #endregion

        #region Private Static Attributes

        private static readonly string HOST = "HOST";
        private static readonly string REGION = "REGION";
        private static readonly string SITE = "SITE";
        private static readonly string SECTION = "SECTION";
        private static readonly string TYPE_VOTING = "TYPE_VOTING";
        private static readonly string STATION_TYPE = "STATION_TYPE";
        private static readonly string TOKEN = "TOKEN";
        private static readonly string BALLOT_BOXES = "BALLOT_BOXES";
        private static readonly string STATUS_STATION = "STATUS_STATION";
        private static readonly string IP_STATION = "IP_STATION";
        private static readonly string NICKNAME_STATION = "NICKNAME_STATION";
        private static readonly string VOTING_STATION_OFFICER = "VOTING_STATION_OFFICER";
        private static readonly string VOTES_PER_VOTER = "VOTES_PER_VOTER";
        private static readonly string ELECTION_NAME = "ELECTION_NAME";
        private static readonly string ELECTION = "ELECTION";
        private static readonly string OPEN_BALLOT_BOX_DATE = "OPEN_BALLOT_BOX_DATE";
        private static readonly string CLOSE_BALLOT_BOX_DATE = "CLOSE_BALLOT_BOX_DATE";
        private static readonly string ID_VOTING_STATION = "ID_VOTING_STATION";
        private static readonly string STATUS_VOTING_STATION = "STATUS_VOTING_STATION";

        #endregion

        #region Properties

        public static ParametersSingleton Instance
        {
            get
            {
                if (instance == null)
                    lock (padlock)
                        if (instance == null)
                            instance = new ParametersSingleton();

                return instance;
            }
        }

        public bool IsConnected
        {
            get;
            set;
        }

        public bool IsTest
        {
            get;
            set;
        }
       
        public string PasswordSqlite {            
            get
            {
                if (string.IsNullOrEmpty(passwordSqlite))
                {
                    passwordSqlite = "prodam";
                }
                return passwordSqlite;
            }
            set
            {
                passwordSqlite = value;                
            }
        }

        public string Host
        {
            get
            {
                if (string.IsNullOrEmpty(host))
                {
                    host = GetValue(HOST);
                }

                return host;
            }
            set
            {
                host = value;
                SetValue(HOST, host);
            }
        }

        public TypeVoting TypeVoting
        {
            get
            {
                TypeVotingStringConverter converter = new TypeVotingStringConverter();
                string valueSQL = GetValue(TYPE_VOTING);

                if (!string.IsNullOrEmpty(valueSQL))
                    typeVoting = converter.ConvertStringToTypeVoting(valueSQL);

                return typeVoting;
            }
            set
            {
                TypeVotingStringConverter converter = new TypeVotingStringConverter();
                typeVoting = value;
                SetValue(TYPE_VOTING, converter.ConvertTypeVotingToString(typeVoting));
            }
        }

        public TypeStation? TypeStation
        {
            get
            {
                if (typeStation == null)
                {
                    TypeStationStringConverter converter = new TypeStationStringConverter();
                    string valueSQL = GetValue(STATION_TYPE);
                    if (!string.IsNullOrEmpty(valueSQL))
                        typeStation = converter.ConvertStringToStation(valueSQL);
                }

                return typeStation;
            }
            set
            {
                TypeStationStringConverter converter = new TypeStationStringConverter();
                typeStation = value;
                SetValue(STATION_TYPE, converter.ConvertStationToString(typeStation));
            }
        }

        public int Region
        {
            get
            {
                if (region == 0)
                {
                    string value = GetValue(REGION);
                    region = string.IsNullOrEmpty(value) ? 0 : Convert.ToInt32(value);
                }

                return region;
            }
            set
            {
                region = value;
                SetValue(REGION, region.ToString());
            }
        }

        public int Election
        {
            get
            {
                if (election == 0)
                {
                    string value = GetValue(ELECTION);
                    election = string.IsNullOrEmpty(value) ? 0 : Convert.ToInt32(value);
                }

                return election;
            }
            set
            {
                election = value;
                SetValue(ELECTION, election.ToString());
            }
        }

        public int VotesPerVoter
        {
            get
            {
                if (votesPerVoter == 0)
                {
                    string value = GetValue(VOTES_PER_VOTER);
                    votesPerVoter = string.IsNullOrEmpty(value) ? 0 : Convert.ToInt32(value);
                }

                return votesPerVoter;
            }
            set
            {
                votesPerVoter = value;
                SetValue(VOTES_PER_VOTER, votesPerVoter.ToString());
            }
        }

        public int Site
        {
            get
            {
                if (site == 0)
                {
                    string value = GetValue(SITE);
                    site = string.IsNullOrEmpty(value) ? 0 : Convert.ToInt32(value);
                }

                return site;
            }
            set
            {
                site = value;
                SetValue(SITE, site.ToString());
            }
        }

        public int Section
        {
            get
            {
                if (section == 0)
                {
                    string value = GetValue(SECTION);
                    section = string.IsNullOrEmpty(value) ? 0 : Convert.ToInt32(value);
                }

                return section;
            }
            set
            {
                section = value;
                SetValue(SECTION, section.ToString());
            }
        }

        public string IPStation
        {
            get
            {
                if (string.IsNullOrEmpty(ipStation))
                {
                    ipStation = GetValue(IP_STATION);
                }

                return ipStation;
            }
            set
            {
                ipStation = value;
                SetValue(IP_STATION, ipStation);
            }
        }

        public string IdVotingStation
        {
            get
            {
                if (string.IsNullOrEmpty(idVotingStation))
                {
                    idVotingStation = GetValue(ID_VOTING_STATION);
                }

                return idVotingStation;
            }
            set
            {
                idVotingStation = value;
                SetValue(ID_VOTING_STATION, idVotingStation);
            }
        }

        public string NicknameStation
        {
            get
            {
                if (string.IsNullOrEmpty(nicknameStation))
                {
                    nicknameStation = GetValue(NICKNAME_STATION);
                }

                return nicknameStation;
            }
            set
            {
                nicknameStation = value;
                SetValue(NICKNAME_STATION, nicknameStation);
            }
        }

        public string ElectionName
        {
            get
            {
                if (string.IsNullOrEmpty(electionName))
                {
                    electionName = GetValue(ELECTION_NAME);
                }

                return electionName;
            }
            set
            {
                electionName = value;
                SetValue(ELECTION_NAME, electionName);
            }
        }


        public List<BallotBox> BallotBoxes
        {
            get
            {
                if (ballotBoxes == null)
                {
                    string valueSQL = GetValue(BALLOT_BOXES);
                    if (!string.IsNullOrEmpty(valueSQL))
                        ballotBoxes = JsonConvert.DeserializeObject<List<BallotBox>>(valueSQL);
                }

                return ballotBoxes;
            }
            set
            {
                ballotBoxes = value;
                if (value != null)
                    SetValue(BALLOT_BOXES, JsonConvert.SerializeObject(ballotBoxes));
                else
                    SetValue(BALLOT_BOXES, null);
            }
        }

        public VotingStationOfficer VotingStationOfficer
        {
            get
            {
                if (votingStationOfficer == null)
                {
                    string valueSQL = GetValue(VOTING_STATION_OFFICER);
                    if (!string.IsNullOrEmpty(valueSQL))
                        votingStationOfficer = JsonConvert.DeserializeObject<VotingStationOfficer>(valueSQL);
                }

                return votingStationOfficer;
            }
            set
            {
                votingStationOfficer = value;
                if (value != null)
                    SetValue(VOTING_STATION_OFFICER, JsonConvert.SerializeObject(votingStationOfficer));
                else
                    SetValue(VOTING_STATION_OFFICER, null);
            }
        }

        public StatusStation? StatusStation
        {
            get
            {
                if (statusStation == null)
                {
                    StatusStationStringConverter converter = new StatusStationStringConverter();
                    string valueSQL = GetValue(STATUS_STATION);
                    if (!string.IsNullOrEmpty(valueSQL))
                        statusStation = converter.ConvertStringToStatusStation(valueSQL);
                }

                return statusStation;
            }
            set
            {
                StatusStationStringConverter converter = new StatusStationStringConverter();
                statusStation = value;
                SetValue(STATUS_STATION, converter.ConvertStatusStationToString(statusStation));
            }
        }


        public StatusTransactionStationStatus? StatusVotingStation
        {
            get
            {
                if (statusVotingStation != null)
                {

                    var converter = new StatusTransactionStationStringConverter();
                    string valueSQL = GetValue(STATUS_VOTING_STATION);
                    if (!string.IsNullOrEmpty(valueSQL))
                        statusVotingStation = converter.ConvertStringToStationStatus(valueSQL);
                }


                return statusVotingStation;
            }
            set
            {
                var converter = new StatusTransactionStationStringConverter();
                statusVotingStation = value;
                SetValue(STATUS_VOTING_STATION, converter.ConvertStatusStationToString(statusVotingStation));
            }
        }


        public string Token
        {
            get
            {
                if (string.IsNullOrEmpty(token))
                {
                    token = GetValue(TOKEN);
                }

                return token;
            }
            set
            {
                token = value;
                SetValue(TOKEN, token);
            }
        }

        public string OpenBallotBoxDate
        {
            get
            {
                if (string.IsNullOrEmpty(openBallotBoxDate))
                {
                    openBallotBoxDate = GetValue(OPEN_BALLOT_BOX_DATE);
                }

                return openBallotBoxDate;
            }
            set
            {
                openBallotBoxDate = value;
                SetValue(OPEN_BALLOT_BOX_DATE, openBallotBoxDate);
            }
        }

        public string CloseBallotBoxDate
        {
            get
            {
                if (string.IsNullOrEmpty(closeBallotBoxDate))
                {
                    closeBallotBoxDate = GetValue(CLOSE_BALLOT_BOX_DATE);
                }

                return closeBallotBoxDate;
            }
            set
            {
                closeBallotBoxDate = value;
                SetValue(CLOSE_BALLOT_BOX_DATE, closeBallotBoxDate);
            }
        }

        #endregion

        #region Private Methods

        private string GetValue(string key)
        {
            if (IsTest)
                return GetValueMemory(key);
            else
                return GetValueDAO(key);
        }

        private string GetValueDAO(string key)
        {
            ParameterDAO dao = new ParameterDAO();
            return dao.Get(key);
        }

        private string GetValueMemory(string key)
        {
            string value = null;
            memoryTest.TryGetValue(key, out value);
            return value;
        }

        private void SetValue(string key, string value)
        {
            if (IsTest)
                SetValueMemory(key, value);
            else
                SetValueDAO(key, value);
        }

        private void SetValueMemory(string key, string value)
        {
            memoryTest.Add(key, value);
        }

        private static void SetValueDAO(string key, string value)
        {
            ParameterDAO dao = new ParameterDAO();

            if (dao.Exists(key))
                dao.Update(key, value);
            else
                dao.Add(key, value);
        }




        #endregion
    }
}