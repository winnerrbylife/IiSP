namespace IPR1_1
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