using CDBurner.Service;
using CDBurner.Service.Common;
using CDBurner.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Windows;

namespace CDBurner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider? _serviceProvider;

        public App()
        {
            IServiceCollection service = new ServiceCollection();

            service.AddSingleton<MainWindow>(provider => new MainWindow
            {
                DataContext = provider.GetRequiredService<MainViewModel>()
            });

            service.AddSingleton<MainViewModel>();
            service.AddSingleton<HomeViewModel>();

            service.AddSingleton<INavigationService, NavigationService>();
            service.AddSingleton<IApiService, ApiService>();
            service.AddSingleton<IDialogService, DialogService>();
            service.AddSingleton<Func<Type, ViewModelBase>>(serviceProvider => viewModelType => (ViewModelBase)serviceProvider.GetRequiredService(viewModelType));

            _serviceProvider = service.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = _serviceProvider?.GetRequiredService<MainWindow>();
            mainWindow?.Show();
            base.OnStartup(e);
        }
    }

}
