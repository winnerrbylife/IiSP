using Microsoft.Maui.Controls;

namespace IPR1_2
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnOpenConverterClicked(object sender, System.EventArgs e)
        {
            var page = Handler.MauiContext.Services.GetService<CurrencyConverterPage>();
            await Navigation.PushAsync(page);
        }
    }
}