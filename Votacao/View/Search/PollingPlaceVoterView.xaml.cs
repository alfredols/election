using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace Votacao.View.Search
{
    public partial class PollingPlaceVoterView : UserControl
    {
        public delegate void NextPrimeDelegate();

        public PollingPlaceVoterView()
        {
            InitializeComponent();
            this.Loaded += PollingPlaceVoterView_Loaded;
        }

        private void PollingPlaceVoterView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            txtTituloEleitor.Focus();
            FocusManager.SetFocusedElement(this, txtTituloEleitor);
        }

        private bool AcceptOnlyNumbers(string text)
        {
            return Regex.IsMatch(text, "[^0-9]+");
        }

        private void txtTituloEleitor_KeyDown(object sender, KeyEventArgs e)
        {
            string text = ((TextBox)sender).Text;
            if (text.Length == 4 || text.Length == 9)
            {
                ((TextBox)sender).Text += ".";
                ((TextBox)sender).CaretIndex = ((TextBox)sender).Text.Length;
            }
        }

        private void txtTituloEleitor_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = AcceptOnlyNumbers(e.Text);
        }

        private void txtTituloEleitor_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }

            if (e.Key == Key.Enter)
            {
                btnForward.Command.Execute(btnForward.CommandParameter);
            }
        }
    }
}
