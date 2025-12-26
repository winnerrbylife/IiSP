namespace IPR1_2
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            
            var navPage = new NavigationPage(new MainPage());
            MainPage = navPage;
        }
    }
}