using LoginApiApp.Services;
using LoginApiApp.Views;
using Microsoft.Extensions.Logging;

namespace LoginApiApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
            builder.Services.AddHttpClient("ApiServiceClient", client => { client.BaseAddress = new Uri("http://192.168.4.223:5178/api/"); } );
#endif
            builder.Services.AddSingleton<ApiService>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<Trajet>();
            builder.Services.AddTransient<Vehicule>();
            builder.Services.AddTransient<AddTrajet>();
            builder.Services.AddTransient<EditTrajet>();
            builder.Services.AddTransient<DeleteTrajet>();
            builder.Services.AddTransient<AddVehicule>();
            builder.Services.AddTransient<EditVehicule>();
            builder.Services.AddTransient<DeleteVehicule>();







            return builder.Build();
        }
    }
}
