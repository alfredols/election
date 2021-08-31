using MVVMC;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using Votacao.Data;
using Votacao.Service.DTO;
using Votacao.SocketConn;

namespace Votacao.View.Setup
{
    public class ChooseIPAddressViewModel : MVVMCViewModel
    {
        #region Attributes
        
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Constructor

        public ChooseIPAddressViewModel()
        {
            Itens = ConvertIPAddressToIPStation(ListLocalIPAddress());

            if (JustOneIP(Itens))
            {
                log.Info(string.Format("Just one IP found {0}", Itens.First().IP));
                SaveIPConfiguration(Itens.First().IP);
            }
            else
            {
                if (IPConfigurationSaved())
                {
                    RestoreIPConfiguration();
                }
            }

            HideMessageError();
        }

        #endregion

        #region Properties

        private ObservableCollection<IPStation> _itens = new ObservableCollection<IPStation>();
        public ObservableCollection<IPStation> Itens
        {
            get { return _itens; }
            set
            {
                _itens = value;
                OnPropertyChanged("Itens");
            }
        }

        private Visibility _visibilityErrorMsg;
        public Visibility VisibilityErrorMsg
        {
            get { return _visibilityErrorMsg; }
            set
            {
                _visibilityErrorMsg = value;
                OnPropertyChanged("VisibilityErrorMsg");
            }
        }

        #endregion

        #region Public Methods

        public bool NextScreen()
        {
            return JustOneIP(this.Itens);
        }

        public bool ValidateScreen()
        {
            if (JustOneIP(this.Itens))
            {
                return true;
            }
            else
            {
                if (NoIPSelected())
                {
                    ShowMessageError();
                    return false;
                }
                else
                {
                    SaveIPConfiguration(GetIPSelected());
                }
            }

            return true;
        }

        #endregion

        #region Private Methods

        private bool JustOneIP(ObservableCollection<IPStation> itens)
        {
            List<IPStation> ips = itens != null ? itens.ToList() : null;
            return ips != null && ips.Count() == 1;
        }

        private List<IPAddress> ListLocalIPAddress()
        {
            Network nt = new Network();
            return nt.ListLocalIPAddress();
        }

        private ObservableCollection<IPStation> ConvertIPAddressToIPStation(List<IPAddress> list)
        {
            return new ObservableCollection<IPStation>(from ip in list
                                                       select new IPStation { IP = ip.ToString() });
        }

        private void SaveIPConfiguration(string IP)
        {
            ParametersSingleton.Instance.IPStation = IP;
            log.Info(string.Format("IP saved {0}", IP));

        }

        private void RestoreIPConfiguration()
        {
            foreach (IPStation ip in Itens)
                if (ip.IP == ParametersSingleton.Instance.IPStation)
                    ip.IsConnected = true;
        }

        private static bool IPConfigurationSaved()
        {
            return !string.IsNullOrEmpty(ParametersSingleton.Instance.IPStation);
        }

        private bool NoIPSelected()
        {
            return !Itens.Where(x => x.IsConnected).Any();
        }

        private string GetIPSelected()
        {
            IPStation ip = this.Itens.Where(x => x.IsConnected).First();

            if (ip != null)
            {
                return ip.IP;
            }
            else 
            {
                return null;
            }
        }

        private void HideMessageError()
        {
            VisibilityErrorMsg = Visibility.Hidden;
        }

        private void ShowMessageError()
        {
            VisibilityErrorMsg = Visibility.Visible;
        }

        #endregion
    }
}
