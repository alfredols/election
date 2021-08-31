using Newtonsoft.Json;
using System;
using System.Windows.Controls;
using Votacao.Data;
using Votacao.Service;
using Votacao.Service.DTO;
using Votacao.Service.Interface;
using Votacao.SocketConn;
using Votacao.SocketConn.TCP;

namespace Votacao.View.Search

{
    /// <summary>
    /// Interaction logic for Election.xaml
    /// </summary>
    public partial class PollingPlaceView : UserControl
    {
        public PollingPlaceView()
        {
            InitializeComponent();
        }
    }
}
