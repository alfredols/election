using System;
using System.Windows;
using System.Windows.Controls;
using Votacao.Data;

namespace Votacao.View
{
    /// <summary>
    /// Interaction logic for EscolhaTipoTerminal.xaml
    /// </summary>
    public partial class Main : Window
    {
        #region Constructor

        public Main(string nameElection, string nameUser)
        {
            InitializeComponent();
            SetUserName(nameUser);
            SetStateMachine();
            NavigateToScreen();
            SetIdBallotBox();
        }

        #endregion

        #region Public Methods

        public void SetStateMachine()
        {
            if (!ParametersSingleton.Instance.StatusStation.HasValue)
            {
                ItemElection.Visibility = Visibility.Collapsed;
                ItemReport.Visibility = Visibility.Collapsed;
                ItemSearch.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (ParametersSingleton.Instance.StatusStation == Service.DTO.StatusStation.CONFIGURED)
                {
                    if (ParametersSingleton.Instance.TypeStation == Service.DTO.TypeStation.VOTER_SEARCH)
                    {
                        ItemElection.Visibility = Visibility.Collapsed;
                        ItemReport.Visibility = Visibility.Collapsed;
                        ItemSearch.Visibility = Visibility.Visible;
                    }
                    else if (ParametersSingleton.Instance.TypeStation == Service.DTO.TypeStation.VOTING_OFFICER)
                    {
                        ItemElection.Visibility = Visibility.Collapsed;
                        ItemReport.Visibility = Visibility.Visible;
                        ItemSearch.Visibility = Visibility.Collapsed;
                    }
                    else if (ParametersSingleton.Instance.TypeStation == Service.DTO.TypeStation.BALLOT_BOX)
                    {
                        ItemElection.Visibility = Visibility.Visible;
                        ItemReport.Visibility = Visibility.Collapsed;
                        ItemSearch.Visibility = Visibility.Collapsed;
                    }
                }
                else if (ParametersSingleton.Instance.StatusStation == Service.DTO.StatusStation.INITIALIZED)
                {
                    if (ParametersSingleton.Instance.TypeStation == Service.DTO.TypeStation.VOTING_OFFICER)
                    {
                        ItemElection.Visibility = Visibility.Visible;
                        ItemReport.Visibility = Visibility.Visible;
                        ItemSearch.Visibility = Visibility.Collapsed;
                    }
                }
                else if (ParametersSingleton.Instance.StatusStation == Service.DTO.StatusStation.FINALIZED)
                {
                    if (ParametersSingleton.Instance.TypeStation == Service.DTO.TypeStation.VOTING_OFFICER)
                    {
                        ItemElection.Visibility = Visibility.Collapsed;
                        ItemReport.Visibility = Visibility.Visible;
                        ItemSearch.Visibility = Visibility.Collapsed;
                    }
                    else if (ParametersSingleton.Instance.TypeStation == Service.DTO.TypeStation.BALLOT_BOX)
                    {
                        ItemElection.Visibility = Visibility.Visible;
                        ItemReport.Visibility = Visibility.Collapsed;
                        ItemSearch.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        public void NavigateToScreen()
        {
            if (!ParametersSingleton.Instance.StatusStation.HasValue)
            {
                NavigateToSetupWizard();
            }
            else
            {
                if (ParametersSingleton.Instance.StatusStation == Service.DTO.StatusStation.CONFIGURED)
                {
                    if (ParametersSingleton.Instance.TypeStation == Service.DTO.TypeStation.VOTER_SEARCH)
                    {
                        SelectItemOnMenu("ItemSearch");
                    }
                    else if (ParametersSingleton.Instance.TypeStation == Service.DTO.TypeStation.VOTING_OFFICER)
                    {
                        SelectItemOnMenu("ItemReport");
                    }
                    else if (ParametersSingleton.Instance.TypeStation == Service.DTO.TypeStation.BALLOT_BOX)
                    {
                        SelectItemOnMenu("ItemElection");
                    }
                }
                else if (ParametersSingleton.Instance.StatusStation == Service.DTO.StatusStation.INITIALIZED)
                {
                    if (ParametersSingleton.Instance.TypeStation == Service.DTO.TypeStation.VOTING_OFFICER)
                    {
                        SelectItemOnMenu("ItemElection");
                    }
                }
                else if (ParametersSingleton.Instance.StatusStation == Service.DTO.StatusStation.FINALIZED)
                {
                    if (ParametersSingleton.Instance.TypeStation == Service.DTO.TypeStation.VOTING_OFFICER)
                    {
                        SelectItemOnMenu("ItemReport");
                    }
                    else if (ParametersSingleton.Instance.TypeStation == Service.DTO.TypeStation.BALLOT_BOX)
                    { 
                        SelectItemOnMenu("ItemElection");
                    }
                }
            }
        }

        public void SetIdBallotBox()
        {
            if(!string.IsNullOrEmpty(ParametersSingleton.Instance.IdVotingStation))
                tbIdBallotBox.Text = string.Format("(ID: {0})", ParametersSingleton.Instance.IdVotingStation);
        }

        public void HideMenu()
        {
            GridMenu.Visibility = Visibility.Collapsed;
            GridMain.Margin = new Thickness(0,0,0,0);
            tbUser.Visibility = Visibility.Hidden;
        }

        public void ShowMenu()
        {
            GridMenu.Visibility = Visibility.Visible;
            GridMain.Margin = new Thickness(70,0,0,0);
            tbUser.Visibility = Visibility.Visible;
        }

        #endregion

        #region Private Methods

        private void SelectItemOnMenu(string itemName)
        {
            object itemStored = null;
            foreach (var item in ListViewMenu.Items)
            {
                if (((ListViewItem)item).Name == itemName)
                {
                    itemStored = item;
                    break;
                }
            }

            ListViewMenu.SelectedItem = itemStored;
        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((ListViewItem)((ListView)sender).SelectedItem).Name)
            {
                case "ItemSetup":
                    NavigateToSetupWizard();
                    break;

                case "ItemElection":
                    NavigateToVotation();
                    break;

                case "ItemReport":
                    NavigateToReport();
                    break;

                case "ItemSearch":
                    NavigateToSearchVoter();
                    break;

                default:
                    break;
            }
        }

        private void NavigateToSetupWizard()
        {
            GridMain.Children.Clear();
            UserControl usc = new Setup.Wizard();
            GridMain.Children.Add(usc);
        }

        private void NavigateToSearchVoter()
        {
            GridMain.Children.Clear();
            UserControl uscSearch = new Search.PollingPlaceView();
            GridMain.Children.Add(uscSearch);
        }

        private void NavigateToVotation()
        {
            GridMain.Children.Clear();
            UserControl uscElection = new Election.ElectionView();
            GridMain.Children.Add(uscElection);
        }

        private void NavigateToReport()
        {
            GridMain.Children.Clear();
            UserControl uscVote = new Report.BallotBoxView();
            GridMain.Children.Add(uscVote);
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Visible;
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            ButtonOpenMenu.Visibility = Visibility.Visible;
        }

        public void FocusMenu()
        {
            ItemElection.Focus();
        }

        private void SetUserName(string nameUser)
        {
            tbUser.Text = nameUser;
        }

        #endregion

    }
}
