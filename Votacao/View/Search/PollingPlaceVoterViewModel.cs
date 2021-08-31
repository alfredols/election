using MVVMC;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Votacao.Service;
using Votacao.Service.DTO;
using Votacao.Service.Interface;
using Region = Votacao.Service.DTO.Region;

namespace Votacao.View.Search
{
    public class PollingPlaceVoterViewModel : MVVMCViewModel
    {

        #region Constructor

        public PollingPlaceVoterViewModel()
        {
            Voter = new Voter();
            ClearScreen();
        }

        #endregion

        #region Commands

        private ICommand searchCommand;
        public ICommand SearchCommand
        {
            get
            {
                if (searchCommand == null)
                    searchCommand = new DelegateCommand(() =>
                    {
                        ShowLoading();
                        Search();
                    },
                    () =>
                    {
                        return SearchCommand != null;
                    });
                return searchCommand;
            }
        }

        private ICommand clearCommand;
        public ICommand ClearCommand
        {
            get
            {
                if (clearCommand == null)
                    clearCommand = new DelegateCommand(() =>
                    {
                        ClearScreen();
                    },
                    () =>
                    {
                        return ClearCommand != null;
                    });
                return clearCommand;
            }
        }

        #endregion

        #region Private Methods

        private void Search()
        {
            if (!ValidateElectorNumber())
            {
                VisibilityMsgError = Visibility.Visible;
                VisibilityMsgNotFound = Visibility.Hidden;
                VisibilityResult = Visibility.Hidden;
                VisibilityVoter = Visibility.Hidden;
                IsActive = false;
            }
            else
            {
                LoadVoterAsync();
            }
        }

        private void ShowLoading()
        {
            IsActive = true;
            VisibilityMsgError = Visibility.Hidden;
            VisibilityMsgNotFound = Visibility.Hidden;
            VisibilityResult = Visibility.Visible;
            VisibilityVoter = Visibility.Hidden;
        }

        private void ClearScreen()
        {
            ElectorNumber = string.Empty;
            VisibilityMsgError = Visibility.Hidden;
            VisibilityMsgNotFound = Visibility.Hidden;
            VisibilityResult = Visibility.Hidden;
            VisibilityVoter = Visibility.Hidden;
            IsActive = false;
        }

        private void LoadVoterAsync()
        {
            Thread newWindowThread = new Thread(new ThreadStart(LoadVoter));
            newWindowThread.SetApartmentState(ApartmentState.STA);
            newWindowThread.IsBackground = true;
            newWindowThread.Start();
        }

        private void LoadVoter()
        {
            IVoter voterService = FactoryService.getVoter();
            Voter = voterService.Get(ElectorNumber.xAsNumber());

            if (Voter == null)
            {
                VisibilityMsgNotFound = Visibility.Visible;
                VisibilityMsgError = Visibility.Hidden;
                VisibilityResult = Visibility.Hidden;
                VisibilityVoter = Visibility.Hidden;
                IsActive = false;
            }
            else
            {
                ShowResult();
            }
        }

        private void ShowResult()
        {
            Section = FactoryService.getSection().Get(voter.SectionId.Value);

            Site = FactoryService.getSite().Get(Section.SiteId);

            Region = FactoryService.getRegion().Get(Site.RegionId);

            if (Site != null)
                Place = string.Format("{0} ({1})", Site.Address, Site.Name);

            IsActive = false;
            VisibilityMsgError = Visibility.Hidden;
            VisibilityMsgNotFound = Visibility.Hidden;
            VisibilityResult = Visibility.Visible;
            VisibilityVoter = Visibility.Visible;
        }

        private bool ValidateElectorNumber()
        {
            if (!string.IsNullOrEmpty(ElectorNumber))
            {
                if (ElectorNumber.Length < 14)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Properties

        private Voter voter;
        public Voter Voter
        {
            get { return voter; }
            set
            {
                voter = value;
                OnPropertyChanged("Voter");
            }
        }

        private Section section;
        public Section Section
        {
            get { return section; }
            set
            {
                section = value;
                OnPropertyChanged("Section");
            }
        }

        private bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                OnPropertyChanged("IsActive");
            }
        }

        private Site site;
        public Site Site
        {
            get { return site; }
            set
            {
                site = value;
                OnPropertyChanged("Site");
            }
        }

        private string place;

        public string Place
        {
            get { return place; }
            set
            {
                place = value;
                OnPropertyChanged("Place");
            }
        }

        private Region region;
        public Region Region
        {
            get { return region; }
            set
            {
                region = value;
                OnPropertyChanged("Region");
            }
        }

        private string electorNumber;
        public string ElectorNumber
        {
            get { return electorNumber; }
            set
            {
                electorNumber = value;
                OnPropertyChanged("electorNumber");
            }
        }

        private Visibility visibilityMsgError;
        public Visibility VisibilityMsgError
        {
            get { return visibilityMsgError; }
            set
            {
                visibilityMsgError = value;
                OnPropertyChanged("VisibilityMsgError");
            }
        }

        private Visibility visibilityMsgNotFound;
        public Visibility VisibilityMsgNotFound
        {
            get { return visibilityMsgNotFound; }
            set
            {
                visibilityMsgNotFound = value;
                OnPropertyChanged("VisibilityMsgNotFound");
            }
        }

        private Visibility visibilityResult;
        public Visibility VisibilityResult
        {
            get { return visibilityResult; }
            set
            {
                visibilityResult = value;
                OnPropertyChanged("VisibilityResult");
            }
        }

        private Visibility visibilityVoter;
        public Visibility VisibilityVoter
        {
            get { return visibilityVoter; }
            set
            {
                visibilityVoter = value;
                OnPropertyChanged("VisibilityVoter");
            }
        }

        #endregion

    }
}