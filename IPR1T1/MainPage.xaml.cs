using Microsoft.Maui.Controls;

namespace IPR1_1  
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnIntegralClicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new IntegralPage());
        }
    }
}