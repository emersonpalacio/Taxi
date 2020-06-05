using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Taxi.Common.Helpers;
using Taxi.Common.Models;

namespace Taxi.Prism.ViewModels
{
    public class TaxiMasterDetailPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private UserResponse _user;

        public TaxiMasterDetailPageViewModel(INavigationService navigationService): base(navigationService)
        {
            _navigationService = navigationService;
            LoadUser();
            LoadMenus();
        }

        public ObservableCollection<MenuItemViewModel> Menus { get; set; }

        public UserResponse User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }

        private void LoadUser()
        {
            if (Settings.IsLogin)
            {
                User = JsonConvert.DeserializeObject<UserResponse>(Settings.User);
            }
        }



        private void LoadMenus()
        {
            List<Menu> menus = new List<Menu>
            {
                new Menu
                {
                    Icon = "ic_airport_shuttle",
                    PageName = "HomePage",
                    Title = "New trip"
                },
                new Menu
                {
                    Icon = "ic_local_taxi",
                    PageName = "TaxiHistoryPage",
                    Title = "See taxi history"
                },
                new Menu
                {
                    Icon = "ic_people",
                    PageName = "GroupPage",
                    Title = "Admin my user group"
                },
                new Menu
                {
                    Icon = "ic_account_circle",
                    PageName = "ModifyUserPage",
                    Title = "Modify User"
                },
                new Menu
                {
                    Icon = "ic_report",
                    PageName = "ReportPage",
                    Title = "Report an incident"
                },
                new Menu
                {
                    Icon = "ic_exit_to_app",
                    PageName = "LoginPage",
                    Title = "Login"//Settings.IsLogin ? $"Logout" : $"Login"
                }
            };

            Menus = new ObservableCollection<MenuItemViewModel>(menus.Select(m => new MenuItemViewModel(_navigationService){
                    Icon = m.Icon,
                    PageName = m.PageName,
                    Title = m.Title
                }).ToList());

        }
    }
}
