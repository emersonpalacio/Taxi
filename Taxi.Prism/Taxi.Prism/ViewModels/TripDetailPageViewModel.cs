using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using Taxi.Common.Models;

namespace Taxi.Prism.ViewModels
{
    public class TripDetailPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private TripResponse _trip;

        public TripDetailPageViewModel(INavigationService navigationService): base(navigationService) 
        {
            Title = "tripDetial";
            this._navigationService = navigationService;
        }

        public TripResponse Trip
        {
            get => _trip;
            set => SetProperty(ref _trip, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey("trip"))
            {
                Trip = parameters.GetValue<TripResponse>("trip");
            }
        }
    }
}
