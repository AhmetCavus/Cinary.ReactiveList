using Cinary.Xamarin.Reactive;
using Cinary.Xamarin.Reactive.Event;
using Prism.Navigation;
using ReactiveList.Test.Models;
using System.Collections.Generic;

namespace ReactiveList.Test.ViewModels
{
    public class MainPageViewModel : ViewModelBase, IScrollListener
    {
        private IList<LocationModel> _locations;
        public IList<LocationModel> Locations
        {
            get { return _locations; }
            set { SetProperty(ref _locations, value); }
        }

        private bool _isListenerActive = true;
        public bool IsListenerActive => _isListenerActive;

        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "ReactiveList Demo App";
            Locations = new List<LocationModel>
            {
                new LocationModel { Country = "Turkey", City = "Konya", Desc = "A description regarding to Konya" },
                new LocationModel { Country = "Turkey", City = "Istanbul", Desc = "A description regarding to Istanbul" },
                new LocationModel { Country = "Germany", City = "Duisburg", Desc = "A description regarding to Duisburg" },
                new LocationModel { Country = "Germany", City = "Berlin", Desc = "A description regarding to Berlin" },
                new LocationModel { Country = "Azerbaijan", City = "Baku", Desc = "A description regarding to Baku" },
                new LocationModel { Country = "Turkmenistan", City = "Ashgabat", Desc = "A description regarding to Ashgabat" },
                new LocationModel { Country = "Bosnia", City = "Sarajevo", Desc = "A description regarding to Sarajevo" },
                new LocationModel { Country = "Egypt", City = "Cairo", Desc = "A description regarding to Kairo" },
                new LocationModel { Country = "Japan", City = "Tokyo", Desc = "A description regarding to Tokio" },
                new LocationModel { Country = "Syria", City = "Damascus", Desc = "A description regarding to Damascus" },
                new LocationModel { Country = "Irak", City = "Baghdat", Desc = "A description regarding to Baghdat" },
                new LocationModel { Country = "Somalia", City = "Mogadishu", Desc = "A description regarding to Mogadishu" },
                new LocationModel { Country = "France", City = "Paris", Desc = "A description regarding to Paris" },
                new LocationModel { Country = "Spain", City = "Madrid", Desc = "A description regarding to Madrid" },
                new LocationModel { Country = "Indonesia", City = "Jakarta", Desc = "A description regarding to Jakarta" },
                new LocationModel { Country = "Malaysia", City = "Kuala Lumpur", Desc = "A description regarding to Kuala Lumpur" },
                new LocationModel { Country = "USA", City = "Washington", Desc = "A description regarding to Washington" },
                new LocationModel { Country = "Mexico", City = "Mexico-City", Desc = "A description regarding to Mexico-City" },
                new LocationModel { Country = "Morocco", City = "Rabat", Desc = "A description regarding to Rabat" },
                new LocationModel { Country = "Ukraine", City = "Kiev", Desc = "A description regarding to Kiev" },
                new LocationModel { Country = "Pakistan", City = "Islamabad", Desc = "A description regarding to Islamabad" },
                new LocationModel { Country = "Senegal", City = "Dakar", Desc = "A description regarding to Dakar" },
                new LocationModel { Country = "Nigeria", City = "Abuja", Desc = "A description regarding to Abuja" },
                new LocationModel { Country = "Ghana", City = "Accra", Desc = "A description regarding to Accra" },
                new LocationModel { Country = "Pitcairn", City = "Adamstown", Desc = "A description regarding to Adamstown" },
                new LocationModel { Country = "Ethiopia", City = "Addis Ababa", Desc = "A description regarding to Addis Ababa" },
                new LocationModel { Country = "Gambia", City = "Banjul", Desc = "A description regarding to Banjul" },
                new LocationModel { Country = "China", City = "Beijing", Desc = "A description regarding to Beijing" },
                new LocationModel { Country = "Lebanon", City = "Beirut", Desc = "A description regarding to Beirut" },
                new LocationModel { Country = "Kyrgyzstan", City = "Bishkek", Desc = "A description regarding to Bishkek" },
                new LocationModel { Country = "Brazil", City = "Brasília", Desc = "A description regarding to Brasília" },
                new LocationModel { Country = "Romania", City = "Bucharest", Desc = "A description regarding to Bucharest" },
                new LocationModel { Country = "Australia", City = "Canberra", Desc = "A description regarding to Canberra" },
                new LocationModel { Country = "Turkey", City = "Konya", Desc = "A description regarding to Konya" },
                new LocationModel { Country = "Turkey", City = "Istanbul", Desc = "A description regarding to Istanbul" },
                new LocationModel { Country = "Germany", City = "Duisburg", Desc = "A description regarding to Duisburg" },
                new LocationModel { Country = "Germany", City = "Berlin", Desc = "A description regarding to Berlin" },
                new LocationModel { Country = "Azerbaijan", City = "Baku", Desc = "A description regarding to Baku" },
                new LocationModel { Country = "Turkmenistan", City = "Ashgabat", Desc = "A description regarding to Ashgabat" },
                new LocationModel { Country = "Bosnia", City = "Sarajevo", Desc = "A description regarding to Sarajevo" },
                new LocationModel { Country = "Egypt", City = "Cairo", Desc = "A description regarding to Kairo" },
                new LocationModel { Country = "Japan", City = "Tokyo", Desc = "A description regarding to Tokio" },
                new LocationModel { Country = "Syria", City = "Damascus", Desc = "A description regarding to Damascus" },
                new LocationModel { Country = "Irak", City = "Baghdat", Desc = "A description regarding to Baghdat" },
                new LocationModel { Country = "Somalia", City = "Mogadishu", Desc = "A description regarding to Mogadishu" },
                new LocationModel { Country = "France", City = "Paris", Desc = "A description regarding to Paris" },
                new LocationModel { Country = "Spain", City = "Madrid", Desc = "A description regarding to Madrid" },
                new LocationModel { Country = "Indonesia", City = "Jakarta", Desc = "A description regarding to Jakarta" },
                new LocationModel { Country = "Malaysia", City = "Kuala Lumpur", Desc = "A description regarding to Kuala Lumpur" },
                new LocationModel { Country = "USA", City = "Washington", Desc = "A description regarding to Washington" },
                new LocationModel { Country = "Mexico", City = "Mexico-City", Desc = "A description regarding to Mexico-City" },
                new LocationModel { Country = "Morocco", City = "Rabat", Desc = "A description regarding to Rabat" },
                new LocationModel { Country = "Ukraine", City = "Kiev", Desc = "A description regarding to Kiev" },
                new LocationModel { Country = "Pakistan", City = "Islamabad", Desc = "A description regarding to Islamabad" },
                new LocationModel { Country = "Senegal", City = "Dakar", Desc = "A description regarding to Dakar" },
                new LocationModel { Country = "Nigeria", City = "Abuja", Desc = "A description regarding to Abuja" },
                new LocationModel { Country = "Ghana", City = "Accra", Desc = "A description regarding to Accra" },
                new LocationModel { Country = "Pitcairn", City = "Adamstown", Desc = "A description regarding to Adamstown" },
                new LocationModel { Country = "Ethiopia", City = "Addis Ababa", Desc = "A description regarding to Addis Ababa" },
                new LocationModel { Country = "Gambia", City = "Banjul", Desc = "A description regarding to Banjul" },
                new LocationModel { Country = "China", City = "Beijing", Desc = "A description regarding to Beijing" },
                new LocationModel { Country = "Lebanon", City = "Beirut", Desc = "A description regarding to Beirut" },
                new LocationModel { Country = "Kyrgyzstan", City = "Bishkek", Desc = "A description regarding to Bishkek" },
                new LocationModel { Country = "Brazil", City = "Brasília", Desc = "A description regarding to Brasília" },
                new LocationModel { Country = "Romania", City = "Bucharest", Desc = "A description regarding to Bucharest" },
                new LocationModel { Country = "Australia", City = "Canberra", Desc = "A description regarding to Canberra" },
                new LocationModel { Country = "Turkey", City = "Konya", Desc = "A description regarding to Konya" },
                new LocationModel { Country = "Turkey", City = "Istanbul", Desc = "A description regarding to Istanbul" },
                new LocationModel { Country = "Germany", City = "Duisburg", Desc = "A description regarding to Duisburg" },
                new LocationModel { Country = "Germany", City = "Berlin", Desc = "A description regarding to Berlin" },
                new LocationModel { Country = "Azerbaijan", City = "Baku", Desc = "A description regarding to Baku" },
                new LocationModel { Country = "Turkmenistan", City = "Ashgabat", Desc = "A description regarding to Ashgabat" },
                new LocationModel { Country = "Bosnia", City = "Sarajevo", Desc = "A description regarding to Sarajevo" },
                new LocationModel { Country = "Egypt", City = "Cairo", Desc = "A description regarding to Kairo" },
                new LocationModel { Country = "Japan", City = "Tokyo", Desc = "A description regarding to Tokio" },
                new LocationModel { Country = "Syria", City = "Damascus", Desc = "A description regarding to Damascus" },
                new LocationModel { Country = "Irak", City = "Baghdat", Desc = "A description regarding to Baghdat" },
                new LocationModel { Country = "Somalia", City = "Mogadishu", Desc = "A description regarding to Mogadishu" },
                new LocationModel { Country = "France", City = "Paris", Desc = "A description regarding to Paris" },
                new LocationModel { Country = "Spain", City = "Madrid", Desc = "A description regarding to Madrid" },
                new LocationModel { Country = "Indonesia", City = "Jakarta", Desc = "A description regarding to Jakarta" },
                new LocationModel { Country = "Malaysia", City = "Kuala Lumpur", Desc = "A description regarding to Kuala Lumpur" },
                new LocationModel { Country = "USA", City = "Washington", Desc = "A description regarding to Washington" },
                new LocationModel { Country = "Mexico", City = "Mexico-City", Desc = "A description regarding to Mexico-City" },
                new LocationModel { Country = "Morocco", City = "Rabat", Desc = "A description regarding to Rabat" },
                new LocationModel { Country = "Ukraine", City = "Kiev", Desc = "A description regarding to Kiev" },
                new LocationModel { Country = "Pakistan", City = "Islamabad", Desc = "A description regarding to Islamabad" },
                new LocationModel { Country = "Senegal", City = "Dakar", Desc = "A description regarding to Dakar" },
                new LocationModel { Country = "Nigeria", City = "Abuja", Desc = "A description regarding to Abuja" },
                new LocationModel { Country = "Ghana", City = "Accra", Desc = "A description regarding to Accra" },
                new LocationModel { Country = "Pitcairn", City = "Adamstown", Desc = "A description regarding to Adamstown" },
                new LocationModel { Country = "Ethiopia", City = "Addis Ababa", Desc = "A description regarding to Addis Ababa" },
                new LocationModel { Country = "Gambia", City = "Banjul", Desc = "A description regarding to Banjul" },
                new LocationModel { Country = "China", City = "Beijing", Desc = "A description regarding to Beijing" },
                new LocationModel { Country = "Lebanon", City = "Beirut", Desc = "A description regarding to Beirut" },
                new LocationModel { Country = "Kyrgyzstan", City = "Bishkek", Desc = "A description regarding to Bishkek" },
                new LocationModel { Country = "Brazil", City = "Brasília", Desc = "A description regarding to Brasília" },
                new LocationModel { Country = "Romania", City = "Bucharest", Desc = "A description regarding to Bucharest" },
                new LocationModel { Country = "Australia", City = "Canberra", Desc = "A description regarding to Canberra" }
            };
        }

        public void OnScroll(object sender, ScrollEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"OnScroll Direction: {e.Direction}, Y: {e.ScrollY}");
        }

        public void OnScrollStateChange(object sender, ScrollStateChangeEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"ScrollStateChangeEventArgs Y: {e.ScrollY}");
        }

        public void OnScrollStateChanged(object sender, ScrollStateChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"OnScrollStateChanged State: {e.ScrollState}");
        }
    }
}
