using MVVMC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using Votacao.Data;
using Votacao.Service;
using Votacao.Service.DTO;
using Votacao.Service.Interface;

namespace Votacao.View.Report
{
    public class InitBallotBoxViewModel: MVVMCViewModel
    {
        private ICandidate candidateService = null;
        private IVote voteService = null;
        private IQRCode QrCodeService = null;
        private IElection electionService = null;
        private static byte[] logoDefaultReport = null;

        public string QRText { get; private set; }

        public InitBallotBoxViewModel()
        {
            candidateService = FactoryService.getCandidate();
            voteService = FactoryService.getVote();
            QrCodeService = FactoryService.getQRCode();
            electionService = FactoryService.getElectionService(ParametersSingleton.Instance.IsConnected);
        }

        public MemoryStream EmissionPdf(bool zeresima)
        {
            var lstDataSource = GetDataSouceReport(zeresima);

            var r = new ReportingService();

            var stream = r.RunReport("RelZero", lstDataSource.ToArray());

            return stream;
        }

        private List<DataSource> GetDataSouceReport(bool zeresima)
        {
            var infoTypeElection = ListInfoCandidate();

            var elections = new List<ElectionReport> {GetInfoElection()};
                                               
            var electionPic =  new ElectionPictureDAO().GetElectionPic(ParametersSingleton.Instance.Election.ToString());

            //Set images
            elections[0].QRCodeElection = GetReportQRCode(infoTypeElection, elections, zeresima);

            elections[0].ElectionLogo = electionPic != null ? electionPic.ElectionLogo : GetImageDefault();

            elections[0].SecretaryLogo = electionPic != null ? electionPic.secretaryLogo : GetImageDefault();

            var lstDataSource = new List<Service.DataSource>
            {
                new DataSource("dsCandidate", infoTypeElection),
                new DataSource("dsElection", elections)
            };

            return lstDataSource;
        }

      
        public byte[] GetImageDefault()
        {
           
            if(logoDefaultReport == null)
            {
                string logoDefault = @"logoReportDefault.jpg";

                var bitmapImage = new BitmapImage(new Uri(logoDefault, UriKind.Relative));

                var encoder = new JpegBitmapEncoder();

                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));

                using (MemoryStream ms = new MemoryStream())
                {
                    encoder.Save(ms);
                    logoDefaultReport = ms.ToArray();
                }

                return logoDefaultReport;

            }

            return logoDefaultReport;
        }

        public List<CandidateReport> ListInfoCandidate()
        {
            var candidates = candidateService.ListCandidate(Convert.ToInt32(ParametersSingleton.Instance.Region))
                                            .Where(x => x.Name != "Branco")
                                            .Where(x => x.Name != "Nulo");

            var votes = voteService.ListVote();

            return (candidates.Select(c => new CandidateReport
            {
                CandidateId = c.Number.Value,
                SumVote = (votes.Where(x => x.CandidateId == c.Id).ToList().Count()),
                CandidateName = c.Name
            })).ToList();
        }

        public ElectionReport GetInfoElection()
        {
            var candidates = candidateService.ListCandidate(Convert.ToInt32(ParametersSingleton.Instance.Region));
            var votes = voteService.ListVote();
            var region = ParametersSingleton.Instance.Region;
            var site = ParametersSingleton.Instance.Site;
            var section = ParametersSingleton.Instance.Section;
            var election = new ElectionReport
            {
                ElectionName = ParametersSingleton.Instance.ElectionName,

                PollingStation = string.Format("Local de votação: {0} {1} {2}", region,
                                                        site, section),
                DateGenerate = DateTime.Now.xToDateTimeString(),

                SumWhiteVote = votes.Where(x => x.CandidateId == 0).Count(),

                SumNullVote = votes.Where(x => x.CandidateId == null).Count(),

                SumTotalVote = votes.Count(),

                Region = region.ToString(),

                Site = site.ToString(),

                Section = section.ToString(),

                IDUE = ParametersSingleton.Instance.IdVotingStation
            };

            return election;
        }

        private byte[] GetReportQRCode(List<CandidateReport> candidates, List<ElectionReport> elections, bool zeresima)
        {
            string dtab = "";
            string hrab = "";
            string dtfc = "";
            string hrgc = "";
            
            var openBallotBoxDate = ParametersSingleton.Instance.OpenBallotBoxDate.xNullDateTime();
            openBallotBoxDate = System.DateTime.Now;
            var closeBallotBoxDate = ParametersSingleton.Instance.CloseBallotBoxDate.xNullDateTime();
            closeBallotBoxDate = System.DateTime.Now;

            if (openBallotBoxDate != null)
            {
                //Data da abertura da urna
                dtab = openBallotBoxDate.xToShortDateStringYYYYMMDD();
                //Hora da abertura da urna hhmmss
                hrab = openBallotBoxDate.xToTimeString();
            }

            if (closeBallotBoxDate != null)
            {
                //Data do fechamento da urna
                dtfc = closeBallotBoxDate.xToShortDateStringYYYYMMDD();
                //Hora do fechamento da urna
                hrgc = closeBallotBoxDate.xToTimeString();
            }

            var election = elections[0];
            var candidate = string.Empty;

            foreach (var item in candidates)
            {
                if(item.SumVote != 0)
                    candidate += $"{item.CandidateId}:{item.SumVote} ";
            }

            string origem = "VOTA";

            if (zeresima)
                origem = "ZERE";

            QRText = BuildStringQrCode(dtab, hrab, dtfc, hrgc, election, candidate, ParametersSingleton.Instance.IdVotingStation, origem);

            return QrCodeService.GetQrCodeToByte(QRText);
        }

        private string BuildStringQrCode(   string dataAberturaUrna, 
                                            string horaAberturaUrna, 
                                            string dataFechamentoUrna, 
                                            string horaFechamentoUrna, 
                                            ElectionReport election, 
                                            string candidate,
                                            string idVotingStationOffice,
                                            string origem)
        {
            var sb = new StringBuilder();
            sb.Append("QRBU:1:1 VRQR:1.5 VRCH:20190129 ");
            sb.Append("ORIG:{12} ORLC:COM PROC:29 DTPL:{7} ");
            sb.Append("PLEI:29 TURN:1 FASE:O UNFE:SP MUNI:{0} ZONA:{1} SECA:{2} IDUE:{11} IDCA:381049858870888481927639 ");
            sb.Append("VERS:6.32.0.13 LOCA:{1} APTO:3565 COMP:46 FALT:3519 DTAB:{7} HRAB:{8} DTFC:{9} ");
            sb.Append("HRFC:{10} IDEL:28 CARG:25 TIPO:0 VERC:201909231808 {3}APTA:3565 ");
            sb.Append("NOMI:{6} BRAN:{4} NULO:{5} TOTC:{6} ");
            sb.Append("HASH:31454C0E35D267AE3A8C6DCB3FD5077C78057FAD8C05ACB27388952577C149CD70049B813BEC7D3B84FE8017C0B72DBEAA087EDEF59A35F3B02EF0CB8E547140 ");
            sb.Append("ASSI:08EEA489C2B06CF91D799352F51C1CE84E45A92E9ED892A9B3FCB522E03B8D6D50ECDFF27BEE5D0F83FBF33B758473E7AF87789E4C3BD142B55563B28E65F60D");

            return string.Format(   sb.ToString(), 
                                    election.Region, 
                                    election.Site, 
                                    election.Section, 
                                    candidate,
                                    election.SumWhiteVote, 
                                    election.SumNullVote,
                                    election.SumTotalVote, 
                                    dataAberturaUrna, 
                                    horaAberturaUrna, 
                                    dataFechamentoUrna, 
                                    horaFechamentoUrna,
                                    idVotingStationOffice,
                                    origem);         
        }
    }
}

