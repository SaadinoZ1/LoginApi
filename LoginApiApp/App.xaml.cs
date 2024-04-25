using LoginApiApp.Services;

namespace LoginApiApp
{
    public partial class App : Application
    {
        private readonly ApiService _apiService;
        public App(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;


            MainPage = new AppShell();
        }
   

    }
}
