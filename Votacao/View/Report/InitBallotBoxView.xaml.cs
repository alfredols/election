using MVVMC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Votacao.Data;
using Votacao.Service;
using Votacao.Service.DTO;
using Votacao.Service.Interface;
using Votacao.SocketConn;
using Votacao.SocketConn.TCP;
using DTO = Votacao.Service.DTO;

namespace Votacao.View.Report
{
    public partial class InitBallotBoxView : UserControl
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Window window;
        private bool loginSuccess = false;

        public InitBallotBoxView()
        {
            InitializeComponent();
            VerifyFileGrid();
            SetStateMachine();
            prReport.IsActive = false;
        }

        private async void btnEmissionZero_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                prReport.IsActive = true;
                await Task.Run(() => GenerateZeresima());
            }
            catch (Exception ex)
            {
                log.Error("Error creating BU", ex);
            }
            finally
            {
                prReport.IsActive = false;
            }
        }

        private void GenerateZeresima()
        {
            if (IsDateTimeToOpenElection())
            {
                loginSuccess = false;

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    ucLogin loginCtl = new ucLogin();
                    loginCtl.LoginSuccessFired += LoginCtl_LoginSuccessFired;
                    CreateLoginWindow(loginCtl);
                    window.ShowDialog();
                }));

                if (loginSuccess)
                {
                    string text = GenerateReport(true);

                    IUploadBU upload = FactoryService.getUploadBU();
                    StatusTransaction status = upload.UploadZeresima(text);
                    bool? resultBU = false;

                    if (status != StatusTransaction.OK)
                    {
                        resultBU = GenerateBU(text, true);
                    }
                    else
                    {
                        MessageBox.Show("Zerésima enviada para a Central!");
                        log.Info("BU sent to server");
                    }

                    ParametersSingleton.Instance.StatusStation = StatusStation.INITIALIZED;

                    //SetElectionStateOnBallotBoxes(ParametersSingleton.Instance.BallotBoxes, PacketType.ELECTION_STARTED);

                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        SetStateMachine();
                        VerifyFileGrid();
                        ReloadMainMenu();
                        NavigateToNextScreen();
                    }));
                }
            }
        }

        private bool IsDateTimeToOpenElection()
        {
            IElection electionService = FactoryService.getElectionService(ParametersSingleton.Instance.IsConnected);
            DateTime dateTimeStart = electionService.GetElectionDateTimeStart(ParametersSingleton.Instance.Election.ToString());
            DateTime dateTimeEnd = electionService.GetElectionDateTimeEnd(ParametersSingleton.Instance.Election.ToString());

            if (System.DateTime.Now > dateTimeStart
                && dateTimeEnd > System.DateTime.Now) 
            {
                return true;
            } 
            else 
            {
                MessageBox.Show(string.Format("Eleição poderá ser aberta entre {0} e {1}", dateTimeStart.ToString("dd/MM/yyyy HH:mm"), dateTimeEnd.ToString("dd/MM/yyyy HH:mm")), "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private bool IsDateTimeToCloseElection()
        {
            IElection electionService = FactoryService.getElectionService(ParametersSingleton.Instance.IsConnected);
            DateTime dateTimeEnd = electionService.GetElectionDateTimeEnd(ParametersSingleton.Instance.Election.ToString());

            if (System.DateTime.Now > dateTimeEnd)
            {
                return true;
            }
            else
            {
                MessageBox.Show(string.Format("Eleição poderá ser fechada após às {0}", dateTimeEnd.ToString("dd/MM/yyyy HH:mm")), "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private async void btnEmissionBU_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                prReport.IsActive = true;
                await Task.Run(() => GenerateBU());
            }
            catch (Exception ex)
            {
                log.Error("Error creating BU", ex);
            }
            finally
            {
                prReport.IsActive = false;
            }
        }

        private void GenerateBU()
        {
            if (IsDateTimeToCloseElection())
            {
                loginSuccess = false;

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    ucLogin loginCtl = new ucLogin();
                    loginCtl.LoginSuccessFired += LoginCtl_LoginSuccessFired;
                    CreateLoginWindow(loginCtl);
                    window.ShowDialog();
                }));

                if (loginSuccess)
                {
                    string text = GenerateReport(false);

                    IUploadBU upload = FactoryService.getUploadBU();
                    StatusTransaction status = upload.Upload(text);
                    bool? resultBU = false;

                    if (status != StatusTransaction.OK)
                    {
                        resultBU = GenerateBU(text, false);
                    }
                    else
                    {
                        MessageBox.Show("Boletim de urna enviado para a Central!");
                        log.Info("BU sent to server");
                    }

                    ParametersSingleton.Instance.StatusStation = StatusStation.FINALIZED;

                    SetElectionStateOnBallotBoxes(ParametersSingleton.Instance.BallotBoxes, PacketType.ELECTION_FINISHED);

                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        SetStateMachine();
                        VerifyFileGrid();
                        ReloadMainMenu();
                        NavigateToNextScreen();
                    }));
                }
            }
        }

        private void SetElectionStateOnBallotBoxes(List<BallotBox> ballots, PacketType packetType)
        {
            if (HasBallotBoxes(ballots))
            {
                foreach (var model in ballots)
                {
                    SendTCPRequestBallotBox(model, packetType);
                }
            }
        }

        private void SendTCPRequestBallotBox(BallotBox model, PacketType packetType)
        {
            TCPClient client = new TCPClient();
            client.SendMessage(new Packet() { IP = model.IP, PacketType = packetType }, 23001);
        }

        private bool HasBallotBoxes(List<BallotBox> ballots)
        {
            return ballots != null;
        }

        private static bool? GenerateBU(string text, bool zeresima)
        {
            bool? resultBU;
            var saveFileDialogBU = new Microsoft.Win32.SaveFileDialog()
            {
                DefaultExt = "*.bu",
                Filter = "Boletim urna (*.bu)|*.bu",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            };

            resultBU = saveFileDialogBU.ShowDialog();

            if (resultBU != null && resultBU == true)
            {
                string buName = string.Format(@".\data\file{0}.bu", System.DateTime.Now.ToString("yyyyMMddHHmmss"));
                File.WriteAllText(saveFileDialogBU.FileName, text);
                File.WriteAllText(buName, text);

                string identifier = string.Empty;

                if (zeresima)
                    identifier = "Arquivo Zerésima";
                else
                    identifier = "Arquivo BU";

                IReport reportDAO = FactoryService.getReport();
                reportDAO.Save(new Service.DTO.Report() { Identifier = identifier, Path = buName });

                if (File.Exists(saveFileDialogBU.FileName))
                {
                    MessageBox.Show("Não foi possível transmitir o arquivo para a Central, portanto ele foi salvo localmente. Por favor, importe o arquivo (.bu) pelo sistema web.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

            return resultBU;
        }

        private static string GenerateReport(bool zeresima)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog()
            {
                DefaultExt = "*.pdf",
                Filter = "PDF Files (*.pdf)|*.pdf",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            };

            var viewModel = new InitBallotBoxViewModel();
            var ms = viewModel.EmissionPdf(zeresima);
            var result = saveFileDialog.ShowDialog();

            if (result != null && result == true)
            {
                string pdfName = string.Format(@".\data\file{0}.pdf", System.DateTime.Now.ToString("yyyyMMddHHmmss"));
                File.WriteAllBytes(saveFileDialog.FileName, ms.ToArray());
                File.WriteAllBytes(pdfName, ms.ToArray());

                string identifier = string.Empty;

                if (zeresima)
                    identifier = "Relatório abertura";
                else
                    identifier = "Relatório fechamento";

                IReport reportDAO = FactoryService.getReport();
                reportDAO.Save(new Service.DTO.Report() { Identifier = identifier, Path = pdfName });

                if (File.Exists(saveFileDialog.FileName))
                {
                    MessageBox.Show("O relatório foi salvo com sucesso!");
                    log.Info("BU pdf file created");
                }
            }

            string text = viewModel.QRText;
            return text;
        }

        private void CreateLoginWindow(ucLogin loginCtl)
        {
            window = new Window
            {
                Content = loginCtl
            };

            window.Height = 550;
            window.Width = 550;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.WindowState = WindowState.Normal;
            window.WindowStyle = WindowStyle.None;
            window.Topmost = true;
        }

        private void LoginCtl_LoginSuccessFired(Service.DTO.Login login)
        {
            window.Close();
            ParametersSingleton.Instance.Token = login.Token;
            loginSuccess = true;
        }

        private void ReloadMainMenu()
        {
            INavigationService svc = NavigationServiceProvider.GetNavigationServiceInstance();
            MVVMC.Region region = (MVVMC.Region)svc.GetType().GetMethod("FindRegionByID", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(svc, new object[] { "BallotBox" });
            FrameworkElement parent = (FrameworkElement)region.Parent;

            while (parent.GetType() != typeof(Main))
            {
                parent = (FrameworkElement)parent.Parent;
            }

            Main menuScreen = (Main)parent;

            menuScreen.SetStateMachine();
        }

        private void NavigateToNextScreen()
        {
            INavigationService svc = NavigationServiceProvider.GetNavigationServiceInstance();
            MVVMC.Region region = (MVVMC.Region)svc.GetType().GetMethod("FindRegionByID", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(svc, new object[] { "BallotBox" });
            FrameworkElement parent = (FrameworkElement)region.Parent;

            while (parent.GetType() != typeof(Main))
            {
                parent = (FrameworkElement)parent.Parent;
            }

            Main menuScreen = (Main)parent;

            menuScreen.NavigateToScreen();
        }

        private void OnCellHyperlinkClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DTO.Report fileEntity = (((Hyperlink)e.OriginalSource).DataContext as DTO.Report);

                if (IsReport(fileEntity))
                {
                    ShowReport(fileEntity);
                }
                else
                {
                    SaveBU(fileEntity);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error open zeresima or BU", ex);
            }
        }

        private bool IsReport(DTO.Report fileEntity)
        {
            return fileEntity.Path != null
                && fileEntity.Path.IndexOf(".pdf") > -1;
        }

        private void SaveBU(DTO.Report fileEntity)
        {
            var saveFileDialogBU = new Microsoft.Win32.SaveFileDialog()
            {
                DefaultExt = "*.bu",
                Filter = "Boletim urna (*.bu)|*.bu",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            };

            bool? resultBU = saveFileDialogBU.ShowDialog();
            if (resultBU != null && resultBU == true)
            {
                File.WriteAllText(saveFileDialogBU.FileName, System.IO.File.ReadAllText(fileEntity.Path));
            }
        }

        private void ShowReport(DTO.Report fileEntity)
        {
            System.Diagnostics.Process.Start(fileEntity.Path);
        }

        public void SetStateMachine()
        {
            btnEmissionZero.Visibility = Visibility.Collapsed;
            btnEmissionBU.Visibility = Visibility.Collapsed;
            if (ParametersSingleton.Instance.StatusStation.HasValue)
            {
                if (ParametersSingleton.Instance.StatusStation == StatusStation.CONFIGURED)
                {
                    btnEmissionZero.Visibility = Visibility.Visible;
                    btnEmissionBU.Visibility = Visibility.Collapsed;
                }
                else if (ParametersSingleton.Instance.StatusStation == StatusStation.INITIALIZED)
                {
                    btnEmissionZero.Visibility = Visibility.Collapsed;
                    btnEmissionBU.Visibility = Visibility.Visible;
                }
            }
        }

        private void VerifyFileGrid()
        {
            IReport reportDAO = FactoryService.getReport();
            List<DTO.Report> list = reportDAO.List();

            if (list != null && list.Count > 0)
            {
                dgFile.ItemsSource = list;
                dgFile.Visibility = Visibility.Visible;
            }
            else
            {
                dgFile.Visibility = Visibility.Hidden;
            }
        }
    }
}
