﻿using SeniorCapstoneProject;
using Microsoft.Maui.Controls;

namespace SeniorCapstoneProject
{
    public partial class LandingPage : ContentPage
    {
        private readonly User? _user;

        public LandingPage(User user)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            _user = user;

            if (!string.IsNullOrEmpty(_user?.FirstName))
                ProfileInitial.Text = _user.FirstName.Substring(0, 1).ToUpper();
            else
                ProfileInitial.Text = "?";
        }

        private async void OnProfileClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProfilePage(_user));
        }

        private async void OnChatClicked(object sender, EventArgs e)
        {
            // await Navigation.PushAsync(new ChatPage(_user));
        }

        private async void OnHistoryClicked(object sender, EventArgs e)
        {
            // await Navigation.PushAsync(new HistoryPage(_user));
        }

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            // await Navigation.PushAsync(new SettingsPage(_user));
        }

        private async void OnHelpClicked(object sender, EventArgs e)
        {
            //  await Navigation.PushAsync(new HelpPage(_user));
        }

        private async void OnCalendarTapped(object sender, EventArgs e)
        {
            if (_user != null && !string.IsNullOrEmpty(_user.Email))
                await Navigation.PushAsync(new CalendarPage(_user.Email));
        }
    }
}