using System;
using System.Windows;
using System.Windows.Controls;
using Votacao.Data;

namespace Votacao.View
{
    public partial class ControlFooter : UserControl
    {
        public ControlFooter()
        {
            InitializeComponent();

            if (!ParametersSingleton.Instance.IsConnected)
            {
                tbConnection.Visibility = Visibility.Visible;
            }
            else
            {
                tbConnection.Visibility = Visibility.Collapsed;
            }

            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            tbVersion.Text = $"v. {version}";
        }
    }
}
