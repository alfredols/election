using System;
using System.IO;
using System.Windows;

namespace Votacao.View
{
    public partial class SplashScreen : Window
    {
        public bool DatabaseExist 
        {
            get;
            set;
        }

        public SplashScreen()
        {
            InitializeComponent();
        }

        public void VerifyDatabase() 
        {
            if (!System.IO.File.Exists(@"data\station.dat"))
            {
                MessageBox.Show("Não foi possível encontrar a base de dados da urna. Por favor, faça a importação.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);

                var openFileDialog = new Microsoft.Win32.OpenFileDialog()
                {
                    DefaultExt = "*.dat",
                    Filter = "Base de dados (*.dat)|*.dat",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                };

                bool? result = openFileDialog.ShowDialog();

                if (result != null
                    && result == true)
                {
                    if (IsSQLiteFile(openFileDialog.FileName))
                    {
                        File.Copy(openFileDialog.FileName, @".\data\station.dat");
                        MessageBox.Show("Base importada com sucesso.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                        DatabaseExist = true;
                    }
                    else
                    {
                        MessageBox.Show("Arquivo inválido.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
                        DatabaseExist = false;
                    }
                }
                else
                {
                    MessageBox.Show("Sem a base de dados não é possivel iniciar a aplicação.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
                    DatabaseExist = false;
                }
            }
            else
            {
                DatabaseExist = true;
            }
        }

        private bool IsSQLiteFile(string path)
        {
            byte[] bytes = new byte[17];
            using (System.IO.FileStream fs = new System.IO.FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                fs.Read(bytes, 0, 16);
            }
            string chkStr = System.Text.ASCIIEncoding.ASCII.GetString(bytes);
            return chkStr.Contains("SQLite format");
        }
    }
}
