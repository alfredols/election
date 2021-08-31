using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Votacao.Data;
using Votacao.Service;
using Votacao.Service.DTO;
using Votacao.Service.Interface;
using DTO = Votacao.Service.DTO;

namespace Votacao.View
{
    /// <summary>
    /// Interaction logic for ucLogin.xaml
    /// </summary>
    public partial class ucLogin : UserControl
    {
        private IConnectBackend serviceConnect = null;
        private ILogin serviceLogin = null;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public event LoginSuccessHandler LoginSuccessFired;

        public delegate void LoginSuccessHandler(DTO.Login login);

        public Visibility VisibilityCancelButton
        {
            get
            {
                return (Visibility)GetValue(VisibilityCancelButtonProperty);
            }
            set
            {
                SetValue(VisibilityCancelButtonProperty, value);
            }
        }

        public static readonly DependencyProperty VisibilityCancelButtonProperty =
            DependencyProperty.Register(nameof(VisibilityCancelButton), typeof(Visibility), typeof(ucLogin),
                new FrameworkPropertyMetadata(null)
                {
                    BindsTwoWayByDefault = true
                });

        public ucLogin()
        {
            InitializeComponent();

            Task.Run(() =>
            {
                serviceConnect = FactoryService.getConnectService();
                ParametersSingleton.Instance.IsConnected = serviceConnect.TryConnection();
                serviceLogin = FactoryService.getLoginService(ParametersSingleton.Instance.IsConnected);
            });

            SetFocus();
        }

        public void SetFocus()
        {
            txtLogin.Focus();
        }

        private async void Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            progressRingLogin.IsActive = true;
            await Task.Delay(1000);
            ValidateLogin();
            progressRingLogin.IsActive = false;
        }

        private void ValidateLogin()
        {
            if (VerifyLoginPassword(txtLogin.Text, txtPassword.Password))
            {
                DTO.Login login = serviceLogin.Login(txtLogin.Text, txtPassword.Password);

                if (login.Status == DTO.StatusTransaction.OK)
                {
                    LoginSuccessFired(login);
                }
                else if (login.Status == DTO.StatusTransaction.ACCESSDENIED)
                {
                    MessageBox.Show("Login e/ou senha inválidos.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    CleanFields();
                    log.Info(string.Format("Login failed with user: {0}", txtLogin.Text));
                }
                else
                {
                    MessageBox.Show("Login e/ou senha inválidos.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    log.Info(string.Format("Login failed with user: {0}", txtLogin.Text));
                }
            }
            else
            {
                MessageBox.Show("Login e/ou senha inválidos.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                log.Info(string.Format("Login failed with user: {0}", txtLogin.Text));
            }
        }

        private void Textbox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Button_ClickAsync(this, e);
            }
        }

        private bool VerifyLoginPassword(string login, string password)
        {
            if (string.IsNullOrEmpty(login)
                || string.IsNullOrEmpty(password))
                return false;
            else
                return true;
        }

        private void SnackbarMessage_ActionClick(object sender, RoutedEventArgs e)
        {
            SnackbarMessageLogin.IsActive = false;
        }

        private void CleanFields()
        {
            txtLogin.Text = string.Empty;
            txtPassword.Password = string.Empty;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((Window)this.Parent).Close();
        }
    }
}
