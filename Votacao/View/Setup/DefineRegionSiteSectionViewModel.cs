using MVVMC;
using System;
using System.Collections.Generic;
using Votacao.Data;
using Votacao.Service;

namespace Votacao.View.Setup
{
    public class DefineRegionSiteSectionViewModel : MVVMCViewModel
    {
        #region Attributes

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Constructor

        public DefineRegionSiteSectionViewModel()
        {
            Regions = FactoryService.getRegion().List();

            RestoreConfiguration();
        }

        #endregion

        #region Properties

        public List<Votacao.Service.DTO.Region> Regions { get; set; }

        private string _regionIdView;
        public string RegionIdView
        {
            get { return _regionIdView; }
            set
            {
                _regionIdView = value;
                OnPropertyChanged("RegionIdView");
            }
        }

        private string _msgRegionViewValidate;
        public string MsgRegionViewValidate
        {
            get { return _msgRegionViewValidate; }
            set
            {
                _msgRegionViewValidate = value;
                OnPropertyChanged("MsgRegionViewValidate");
            }
        }

        private string _regionViewValidate;
        public string RegionViewValidate
        {
            get { return _regionViewValidate; }
            set
            {
                _regionViewValidate = value;
                OnPropertyChanged("RegionViewValidate");
            }
        }


        private string _siteViewValidate;
        public string SiteViewValidate
        {
            get { return _siteViewValidate; }
            set
            {
                _siteViewValidate = value;
                OnPropertyChanged("SiteViewValidate");
            }
        }

        private string _sectionViewValidade;
        public string SectionViewValidade
        {
            get { return _sectionViewValidade; }
            set
            {
                _sectionViewValidade = value;
                OnPropertyChanged("SectionViewValidade");
            }
        }

        private string _siteIdView;
        public string SiteIdView
        {
            get { return _siteIdView; }
            set
            {
                _siteIdView = value;
                OnPropertyChanged("SiteIdView");
            }
        }

        private string _sectionIdView;
        public string SectionIdView
        {
            get { return _sectionIdView; }
            set
            {
                _sectionIdView = value;
                OnPropertyChanged("SectionIdView");
            }
        }

        #endregion

        #region Public Methods

        public bool ValidateScreen()
        {
            ShowErrorFields();

            if (HasFieldsNotSelected())
            {
                ShowErrorMessage();
                return false;
            }
            else
            {
                SaveConfiguration();
            }

            return true;
        }

        #endregion

        #region Private Methods

        private void RestoreConfiguration()
        {
            RegionIdView = ParametersSingleton.Instance.Region.ToString();
            SiteIdView = ParametersSingleton.Instance.Site.ToString();
            SectionIdView = ParametersSingleton.Instance.Section.ToString();
        }

        private void SaveConfiguration()
        {
            ParametersSingleton.Instance.Region = Convert.ToInt32(RegionIdView);
            ParametersSingleton.Instance.Site = Convert.ToInt32(SiteIdView);
            ParametersSingleton.Instance.Section = Convert.ToInt32(SectionIdView);
            log.Info(string.Format("Informed RegionId {0} SiteId {1} SectionId {2}", RegionIdView, SiteIdView, SectionIdView));
        }

        private bool HasFieldsNotSelected()
        {
            return string.IsNullOrEmpty(RegionIdView)
                            || string.IsNullOrEmpty(SiteIdView)
                            || string.IsNullOrEmpty(SectionIdView);
        }

        private void ShowErrorMessage()
        {
            MsgRegionViewValidate = "Os campos com o (*) em vermelho devem ser selecionados.";
        }

        private void ShowErrorFields()
        {
            if (string.IsNullOrEmpty(RegionIdView))
            {
                RegionViewValidate = "*";
            }

            if (string.IsNullOrEmpty(SiteIdView))
            {
                SiteViewValidate = "*";
            }

            if (string.IsNullOrEmpty(SectionIdView))
            {
                SectionViewValidade = "*";
            }
        }

        #endregion
    }
}
