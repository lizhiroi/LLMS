using LLMS.View;
using LLMS.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Unity;

namespace LLMS
{
    /// <summary>
    /// Interaction logic for TenantWindow.xaml
    /// </summary>
    public partial class TenantWindow : Window
    {
        
        private testdb1Entities db = new testdb1Entities();
        private TenantWindowViewModel _viewModel;
        private readonly IUnityContainer _container;

        public TenantWindow()
        {
            InitializeComponent();
            _viewModel = new TenantWindowViewModel();
            this.DataContext = _viewModel;




        }

    

        private void Image_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void Image_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                // Assuming the first file is the one we're interested in
                var viewModel = this.DataContext as PropertyViewModel;
                viewModel?.HandleFileDrop(files[0]);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            // Handle Exit button click
            Close();
        }

        private void OpenLeaseWindow_Click(object sender, RoutedEventArgs e)
        {
            // Handle Lease Detail button click
            StatusBarText.Text = "Open Lease Detail";
            LeaseWindow leaseWindow = new LeaseWindow();
            leaseWindow.ShowDialog();
        }

        private void OpenTenantWindow_Click(object sender, RoutedEventArgs e)
        {
            // Handle Tenant Detail button click
            StatusBarText.Text = "Open Tenant Detail";
            TenantWindow tenantWindow = new TenantWindow();
            tenantWindow.ShowDialog();
        }


        private void OpenPropertyWindow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var propertyView = _container.Resolve<PropertyView>();
                propertyView.Show();
                StatusBarText.Text = "Open Property Detail";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }






    }
}