using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using IPR1_2.Services;

namespace IPR1_2
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
            
            builder.Services.AddHttpClient<IRateService>(opt => 
                opt.BaseAddress = "https://www.nbrb.by/api/exrates/");
            
            builder.Services.AddSingleton<IRateService, RateService>();

            builder.Services.AddTransient<CurrencyConverterPage>();
            

            return builder.Build();
        }
    }
}