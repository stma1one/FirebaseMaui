using Microsoft.Extensions.Logging;
using MVVMSample.Services;
using MVVMSample.ViewModels;
using MVVMSample.Views;

namespace MVVMSample
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
                    fonts.AddFont("MaterialSymbolsOutlined.ttf", "MaterialSymbols");
                    fonts.AddFont("gadi-almog-regular-aaa.otf", "AlmogRegular");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            #region Dependency Inject the views, viewmodel and builder
            //רישום של דפים
          //  builder.Services.AddSingleton<AddToyPage>();
            builder.RegisterViews().RegisterServices().RegisterViewModels();
           
            
            #endregion
            return builder.Build();
        }
        public static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
        {
            builder.Services.AddTransient<LoadingPage>();
            builder.Services.AddSingleton<AddToyPage>();
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddTransient<ToyDetailsPage>();
            builder.Services.AddSingleton<ViewToysPage>();
            return builder;
        }
        public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
        {
            builder.Services.AddTransient<AddToysPageViewModel>();
            builder.Services.AddSingleton<LoginPageViewModel>();
            builder.Services.AddTransient<ToyDetailsPageViewModel>();
            builder.Services.AddSingleton<ViewToysPageViewModel>();
            return builder;           
        }
        public static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
        {
            
            builder.Services.AddSingleton<IToys, FirebaseServices>();

            return builder;
        }
    }
}
