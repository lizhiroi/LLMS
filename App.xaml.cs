using LLMS.Service;
using LLMS.View;
using System;
using System.Windows;
using Unity;
using Unity.Injection;
using dotenv.net;
using System.Reflection;
using System.IO;

namespace LLMS
{
    public partial class App : Application
    {
        private IUnityContainer _container = new UnityContainer();
        private string blobconnectionString;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string exePath = Assembly.GetExecutingAssembly().Location;
            string exeDirectory = Path.GetDirectoryName(exePath);
            string envFilePath = Path.Combine(exeDirectory, "..", "..", ".env");
            string envFileAbsolutePath = Path.GetFullPath(envFilePath);

            DotEnv.Load(new DotEnvOptions(envFilePaths: new[] { envFileAbsolutePath }));

            blobconnectionString = Environment.GetEnvironmentVariable("ConnectionString");

            _container = new UnityContainer();
            ConfigureContainer();
            
            var mainWindow = _container.Resolve<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureContainer()
        {
            var connectionString = blobconnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Azure Storage connection String is not set.");
            }

            _container.RegisterType<IAzureBlobStorageClient, AzureBlobStorageClient>(
                new InjectionConstructor(connectionString));

            _container.RegisterType<IImageService, ImageService>();
            _container.RegisterType<IPropertyService, PropertyService>();
            _container.RegisterType<PropertyView>();
            _container.RegisterType<MainWindow>();
        }
    }

}

