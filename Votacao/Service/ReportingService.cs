

using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Votacao.Service
{

    /// <summary>
    /// Esta classe implementa as operações necessárias para a geração dos diversos relatórios do sistema.
    /// </summary>
    public class ReportingService
    {
        private static Dictionary<String, Stream> reportListCache = new Dictionary<string, Stream>();


        /// <summary>
        /// Statically Initializes the <see cref="ReportingService"/> class and Cache all required information.
        /// </summary>
        static ReportingService()
        {
            var service = new ReportingService();
            reportListCache = service.FindReports();
        }


        /// <summary>
        /// Localiza todos os relatórios disponíveis no assembly.
        /// </summary>
        /// <returns>Dicionário contendo a lista de relatrórios e sua representação binária.</returns>
        public Dictionary<String, Stream> FindReports()
        {
            return CacheReports(false);
        }

        #region --- Supporting Methods ------------------------------------------------------------

        /// <summary>
        /// Método de apoio utilizado para inicilizar o cache de relatórios disponíveis.
        /// </summary>
        /// <param name="reload"></param>
        /// <returns></returns>
        private Dictionary<String, Stream> CacheReports(bool reload)
        {
            if (reportListCache.Count == 0 || reload == true)
            {
                var assembly = Assembly.GetExecutingAssembly();
                var reports = assembly.GetManifestResourceNames().Where(e => e.EndsWith(".rdlc")).ToList();
                var list = new Dictionary<string, Stream>();

                foreach (var item in reports)
                {
                    var stream = assembly.GetManifestResourceStream(item);
                    list.Add(item.Replace("Votacao.Reports.", "").Replace(".rdlc", ""), stream);
                }
                return list;
            }

            return reportListCache;
        }

        /// <summary>
        /// Atualiza o cache dos relatórios disponíveis.
        /// </summary>
        private void ReloadReports()
        {
            reportListCache = CacheReports(true);
        }

        public MemoryStream RunReport(string reportName, params DataSource[] dataSources)
        {
            if (string.IsNullOrWhiteSpace(reportName))
            {
                throw new ArgumentNullException("reportName");
            }

            if (dataSources == null || dataSources.Length == 0)
            {
                throw new ArgumentException("Ao menos um DataSource deve ser passado para o relatório.", "dataSources");
            }

            var resourceStream = reportListCache.Where(e => e.Key.ToLower() == reportName.ToLower()).Select(e => e.Value).FirstOrDefault();
            if (resourceStream == null || resourceStream.Length == 0)
            {
                throw new InvalidOperationException("Relatório especificado não pode ser carregado. Verifique corretamente o nome e tenha certeza que seu arquivo .rdlc está configurado como 'Embeded Resource'");
            }

            try
            {
                resourceStream.Position = 0;
                LocalReport localReport = new LocalReport();
                localReport.LoadReportDefinition(resourceStream);
                localReport.Refresh();
                foreach (var ds in dataSources)
                {
                    localReport.DataSources.Add(new ReportDataSource(ds.Name, ds.Source));
                }

                String mimeType = string.Empty;
                String encoding = string.Empty;
                String extension = string.Empty;
                Warning[] warnings = null;
                String[] streamIds = null;

                var bytes = localReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                var ms = new MemoryStream(bytes);
                ms.Position = 0;


                return ms;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion --- Supporting Methods ------------------------------------------------------------

    }

    #region -- Supporting Class -------------------------------------------------------------------

    /// <summary>
    /// Esta classe representa um dataSource utilizado pelos relatórios. Os relatórios do sistema
    /// utilizam objectDataSource, então, esta classe garante que a estrutura necessária de
    /// configuração do <see cref="ReportViewer"/> seja simplificada.
    /// </summary>
    public class DataSource
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSource"/> class.
        /// </summary>
        /// <param name="name">Nome do dataSource utilizado pelo arquivo '.rdlc' .</param>
        /// <param name="dataSource">Ffonte de dados representada pelo nome informado.</param>
        /// <exception cref="System.ArgumentNullException">
        /// name caso seja uma string nula ou vazia
        /// or
        /// dataSource caso seja um objeto nulo.
        /// </exception>
        public DataSource(string name, object dataSource)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            if (dataSource == null)
            {
                throw new ArgumentNullException("dataSource");
            }

            Name = name;
            Source = dataSource;
        }

        /// <summary>
        /// Nome do dataSource utilizado pelo arquivo .rdlc
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Objeto contendo a fonte de dados sob o Nome informado.
        /// </summary>
        public object Source { get; private set; }
    }

    #endregion -- Supporting Class -------------------------------------------------------------------
}

