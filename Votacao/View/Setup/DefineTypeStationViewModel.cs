using MVVMC;
using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using Votacao.Data;
using Votacao.Service.DTO;

namespace Votacao.View.Setup
{
    public class DefineTypeStationViewModel : MVVMCViewModel
    {
        #region Attributes

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Constructor

        public DefineTypeStationViewModel()
        {
            RestoreConfiguration();
            HideMessageError();
            ReloadMainMenu();
            log.Info("Started setup application");
        }

        #endregion

        #region Properties

        private TypeStation? _typeStation;
        public TypeStation? TypeStation
        {
            get { return _typeStation; }
            set
            {
                _typeStation = value;
                OnPropertyChanged("TypeStation");
            }
        }

        private Visibility _messageVisibility;
        public Visibility MessageVisibility
        {
            get { return _messageVisibility; }
            set
            {
                _messageVisibility = value;
                OnPropertyChanged("MessageVisibility");
            }
        }

        private string _nicknameStation;
        public string NicknameStation
        {
            get { return _nicknameStation; }
            set
            {
                _nicknameStation = value;
                OnPropertyChanged("NicknameStation");
            }
        }

        #endregion

        #region Public Methods

        public bool IsSearchVoterStation()
        {
            return this.TypeStation == Service.DTO.TypeStation.VOTER_SEARCH;
        }

        public bool ValidateScreen()
        {
            if (TypeStation == null
                || string.IsNullOrEmpty(NicknameStation))
            {
                ShowMessageError();
                return false;
            }
            else
            {
                SaveConfiguration();
            }

            return true;
        }

        #endregion

        #region Private Methods

        private void HideMessageError()
        {
            this.MessageVisibility = Visibility.Hidden;
        }

        private void ShowMessageError()
        {
            this.MessageVisibility = Visibility.Visible;
        }

        private void SaveConfiguration()
        {
            ParametersSingleton.Instance.TypeStation = TypeStation;
            ParametersSingleton.Instance.NicknameStation = NicknameStation;
            log.Info(string.Format("Informed StationType {0} NicknameStation {1}", TypeStation, NicknameStation));
        }

        private void RestoreConfiguration()
        {
            TypeStation = ParametersSingleton.Instance.TypeStation;
            NicknameStation = ParametersSingleton.Instance.NicknameStation;
        }

        private void ReloadMainMenu()
        {
            INavigationService svc = NavigationServiceProvider.GetNavigationServiceInstance();
            MVVMC.Region region = (MVVMC.Region)svc.GetType().GetMethod("FindRegionByID", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(svc, new object[] { "Wizard" });
            FrameworkElement parent = (FrameworkElement)region.Parent;

            while (parent.GetType() != typeof(Main))
            {
                parent = (FrameworkElement)parent.Parent;
            }

            Main menuScreen = (Main)parent;

            menuScreen.SetStateMachine();
        }

        #endregion
    }

    #region EnumStationConverter

    public class StationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            string checkValue = value.ToString();
            string targetValue = parameter.ToString();
            return checkValue.Equals(targetValue,
                     StringComparison.InvariantCultureIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return null;

            TypeStationStringConverter converter = new TypeStationStringConverter();
            return converter.ConvertStringToStation(parameter.ToString());
        }
    }

    #endregion
}
