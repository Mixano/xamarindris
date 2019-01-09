using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConnectionPage : ContentPage
    {
        public string UserName { get; private set; }

        public string Ip { get; private set; }

        public int Port { get; private set; }

        public bool IsCorrectData { get; private set; }

        private MainPage mainPage;

        public ConnectionPage(MainPage mainPage)
        {
            this.mainPage = mainPage;
            InitializeComponent();
        }

        private async void ButtonConnect_Clicked(object sender, EventArgs e)
        {
            try
            {
                UserName = entryLogin.Text;
                Ip = entryIP.Text;
                Port = Convert.ToInt32(entryPort.Text);
                IsCorrectData = true;
                mainPage.StartConnection(UserName, Ip, Port);
                await Navigation.PopModalAsync();
            }
            catch (FormatException)
            {
                await DisplayAlert("Chat", "Port must be number value", "OK");
            }
        }

        private void EnableConnectButton()
        {
            if (entryLogin.Text == null || entryIP.Text == null || entryPort.Text == null)
            {
                buttonConnect.IsEnabled = false;
            }
            else if (entryLogin.Text != null && entryIP.Text != null && entryPort.Text != null)
            {
                buttonConnect.IsEnabled = true;
            }
        }

        private void EntryLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableConnectButton();
        }

        private void EntryIP_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableConnectButton();
        }

        private void EntryPort_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableConnectButton();
        }
    }
}