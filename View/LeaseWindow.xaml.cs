using FluentValidation;
using LLMS.Validators;
using LLMS.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using Unity;

namespace LLMS.View

{
    /// <summary>
    /// Interaction logic for LeaseWindow.xaml
    /// </summary>
    public partial class LeaseWindow : Window
    {
        private testdb1Entities db = new testdb1Entities();
        private LeaseWindowViewModel _viewModel;
        private readonly IUnityContainer _container;
      


        public LeaseWindow()
        {
            InitializeComponent();
            _viewModel = new LeaseWindowViewModel();
            this.DataContext = _viewModel;
        




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

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


        }




    }
}