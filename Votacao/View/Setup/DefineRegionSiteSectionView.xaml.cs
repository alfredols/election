using System;
using System.Windows.Controls;
using Votacao.Data;
using Votacao.Service;
using Votacao.Service.Interface;

namespace Votacao.View.Setup
{
    public partial class DefineRegionSiteSectionView : UserControl
    {
        #region Attributes

        private ISite siteService = null;
        private ISection sectionService = null;

        #endregion

        #region Constructor

        public DefineRegionSiteSectionView()
        {
            siteService = FactoryService.getSite();
            sectionService = FactoryService.getSection();
            InitializeComponent();                        
            cbbSite.IsEnabled = false;
            cbbSection.IsEnabled = false;
        }

        #endregion

        #region Events

        private void cbbRegion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            string selectedValue = (string)cmb.SelectedValue;                        
            cbbSite.IsEnabled = true;
            cbbSite.ItemsSource = siteService.ListByRegion(Convert.ToInt32(selectedValue));            
        }
        
        private void cbbSite_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            
            if(cmb.SelectedValue != null)
            {
                string siteSelected = (string)cmb.SelectedValue;

                if (string.IsNullOrEmpty(siteSelected))
                {
                    cbbSection.IsEnabled = false;
                    cbbSection.SelectedIndex = -1;
                    cbbSection.ItemsSource = null;
                    ParametersSingleton.Instance.Site = 0;                    
                }
                else
                {
                    cbbSection.IsEnabled = true;
                    cbbSection.SelectedIndex = -1;
                    cbbSection.ItemsSource = sectionService.ListBySite(Convert.ToInt32(siteSelected));
                }
            }           
        }

        #endregion
    }
}
