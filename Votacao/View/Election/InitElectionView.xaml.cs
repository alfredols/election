using MVVMC;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Votacao.View.Election
{
    public partial class InitElectionView : UserControl
    {

        #region Constructor

        public InitElectionView()
        {
            InitializeComponent();
            DataObject.AddPastingHandler(txtTituloEleitor, OnPaste);
            this.Loaded += InitElectionView_Loaded;
        }

        #endregion

        #region Events

        private void InitElectionView_Loaded(object sender, RoutedEventArgs e)
        {
            txtTituloEleitor.Focus();
            FocusManager.SetFocusedElement(this, txtTituloEleitor);
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
                dgResult.Focus();
            }
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            e.CancelCommand();
        }

        #endregion

        #region Private Methods

        private bool AcceptOnlyNumbers(string text)
        {
            return Regex.IsMatch(text, "[^0-9]+");
        }

        #endregion

    }
}
