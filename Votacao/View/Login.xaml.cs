using System;
using System.Threading.Tasks;
using System.Windows;
using Votacao.Data;
using Votacao.Service;
using Votacao.Service.DTO;
using Votacao.Service.Interface;
using DTO = Votacao.Service.DTO;

namespace Votacao.View
{
    public partial class Login : Window
    {
        #region Private Attributes

        private IConnectBackend serviceConnect = null;
        private ILogin serviceLogin = null;
        private IElection serviceElection = null;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private bool finalizationStatus = false;
        #endregion

        #region Constructor

        public Login(Window splashScreen)
        {
            try
            {
                IsOK = true;

                SetupDatabase st = new SetupDatabase();
                st.Init();

                serviceConnect = FactoryService.getConnectService();
                ParametersSingleton.Instance.IsConnected = serviceConnect.TryConnection();

                InitializeComponent();

                if (ParametersSingleton.Instance.IsConnected)
                {
                    DateTime dateTimeServer = serviceConnect.GetDateTime();
                    IsOK = ValidateDateTimeMachine(dateTimeServer);
                }

                serviceLogin = FactoryService.getLoginService(ParametersSingleton.Instance.IsConnected);

                if (ParametersSingleton.Instance.IsConnected)
                    log.Info(string.Format("Application started with connection to backend {0}", ParametersSingleton.Instance.Host));
                else
                    log.Info(string.Format("Application started without connection to backend {0}", ParametersSingleton.Instance.Host));

                ucLoginCtl.SetFocus();
            }
            catch (Exception ex)
            {
                log.Error("Error on startup application", ex);
                MessageBox.Show("Erro ao iniciar a aplicação, contatar suporte.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                IsOK = false;
                this.Close();
            }
            finally 
            {
                if(splashScreen != null)
                    splashScreen.Close();
            }
        }

        private string GetElectionName(DTO.Login login)
        {
            serviceElection = FactoryService.getElectionService(ParametersSingleton.Instance.IsConnected);
            return serviceElection.GetName(login.Election);
        }

        private TypeVoting GetVoting(DTO.Login login)
        {
            serviceElection = FactoryService.getElectionService(ParametersSingleton.Instance.IsConnected);
            return serviceElection.GetVoting(login.Election);
        }

        private int GetVotesPerVoter(DTO.Login login)
        {
            serviceElection = FactoryService.getElectionService(ParametersSingleton.Instance.IsConnected);
            return serviceElection.GetVotesPerVoter(login.Election);
        }

        public Login(bool finalizationStatus) : this(null)
        {
            this.finalizationStatus = finalizationStatus;
        }

        #endregion

        #region Private Methods

        private bool ValidateDateTimeMachine(DateTime dateTimeServer)
        {
            DateTime dateTimeMachine = System.DateTime.Now;

            if (dateTimeMachine <= dateTimeServer.AddMinutes(1)
                && dateTimeMachine >= dateTimeServer.AddMinutes(-1)) 
            {
                log.Info(string.Format("DateTime of machine is ok machine: {0} server: {1}", dateTimeMachine, dateTimeServer));
                return true;
            }
            else 
            {
                MessageBox.Show(string.Format("Erro a hora da máquina local precisa ser corrigida. Hora certa: {0}", dateTimeServer.ToString("dd/MM/yyyy HH:mm")), "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
                log.Info(string.Format("DateTime of machine is wrong machine: {0} server: {1}", dateTimeMachine, dateTimeServer));
                this.Close();
                return false;
            }
        }

        private void ShowNextScreen(string nameElection, string nameUser)
        {
            if (!finalizationStatus)
            {
                var election = new Main(nameElection, nameUser);
                election.Show();
            }

            this.Close();
        }



        #endregion

        #region Properties

        public bool IsOK
        {
            get; private set;
        }

        #endregion

        private void ucLogin_LoginSuccessFired(DTO.Login login)
        {
            ParametersSingleton.Instance.ElectionName = GetElectionName(login);
            ParametersSingleton.Instance.VotesPerVoter = GetVotesPerVoter(login);
            ParametersSingleton.Instance.TypeVoting = GetVoting(login);
            ParametersSingleton.Instance.Token = login.Token;
            ParametersSingleton.Instance.Election = Convert.ToInt32(login.Election);
            log.Info(string.Format("Login successfully with user: {0}", login.NameUser));
            ShowNextScreen(ParametersSingleton.Instance.ElectionName, login.NameUser);
        }
    }
}