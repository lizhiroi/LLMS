using DocumentFormat.OpenXml.Spreadsheet;
using LLMS.Service;
using LLMS.View;
using LLMS.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Unity;

namespace LLMS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private List<MainWindowViewModel> originalItemsList = new List<MainWindowViewModel>();


        // Define AzureDbContext object
        private testdb1Entities db = new testdb1Entities();
        private readonly IUnityContainer _container;

        public MainWindow(IUnityContainer container)
        {
            try
            {
                InitializeComponent();
                _container = container;
                this.Loaded += Window_Loaded;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during MainWindow initialization: {ex.Message}");
            }
        }

        private void PropertyDetail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var propertyView = _container.Resolve<PropertyView>();
                propertyView.Show();
                StatusBarText.Text = "Open Property Detail";
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }


        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //load all lease information from the database
            LoadMainWindow();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            // Handle Exit button click
            Close();
        }


        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listView.SelectedItem is MainWindowViewModel selectedLease)
            {
                // update right panel with selected lease information
                UpdateRightPanel(selectedLease);
                StatusBarText.Text = "display the selected item";
            }
        }

        private void LeaseDetail_Click(object sender, RoutedEventArgs e)
        {
            // Handle Lease Detail button click
            StatusBarText.Text = "Open Lease Detail";
            LeaseWindow leaseWindow = new LeaseWindow();
            leaseWindow.ShowDialog();
        }

        private void TenantDetail_Click(object sender, RoutedEventArgs e)
        {
            // Handle Tenant Detail button click
            StatusBarText.Text = "Open Tenant Detail";
            TenantWindow tenantWindow = new TenantWindow();
            tenantWindow.ShowDialog();
        }

        private void LoadMainWindow()
        {

            try
            {
                originalItemsList = db.leases.Select(lease => new MainWindowViewModel
                {
                    LeaseId = lease.id,
                    PaymentDueDay = lease.payment_due_day,
                    RentAmount = lease.rent_amount,
                    Address = lease.property.address,
                    TenantName = lease.tenant.first_name + " " + (lease.tenant.last_name ?? ""), // tenantname is first name + last name
                    PhoneNo = lease.tenant.phone_number,
                    Email = lease.tenant.email,
                    ImageUrl = lease.property.image.image_url, // image_url: url of the image store in Azure blob Storage
                    EmergencyContactName = lease.tenant.emergency_contact_name,
                    EmergencyContactNo = lease.tenant.emergency_contact_number
                }).ToList();

                listView.ItemsSource = originalItemsList;
            }
            catch (Exception ex)
            {
                StatusBarText.Text = "Error accessing database";
                MessageBox.Show($"Error accessing database: {ex.Message}");
            }

            // default select the first item in the list
            if (listView.Items.Count > 0)
            {
                listView.SelectedItem = originalItemsList[0];
                StatusBarText.Text = "select the first row as default";
            }
        }

        private void UpdateRightPanel(MainWindowViewModel lease)
        {
            lblAddress.Content = lease.Address;
            lblTenantName.Content = lease.TenantName;
            lblPhoneNo.Content = lease.PhoneNo;
            lblEmail.Content = lease.Email;
            lblEmergencyContactName.Content = lease.EmergencyContactName;
            lblEmergencyContactNo.Content = lease.EmergencyContactNo;
            //update image URL
            imageDisplay.Source = new BitmapImage(new Uri(lease.ImageUrl, UriKind.Absolute));
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchText = Tbxsearch.Text.ToLower();
            var filteredItems = originalItemsList.Where(item => item.Address.ToLower().Contains(searchText) || item.TenantName.ToLower().Contains(searchText)).ToList();

            if (filteredItems.Any())
            {
                listView.ItemsSource = filteredItems;
                listView.SelectedItem = filteredItems[0];
                StatusBarText.Text = "Matching content found";
            }
            else
            {
                MessageBox.Show("No matching content found.", "Search Result", MessageBoxButton.OK, MessageBoxImage.Information);
                listView.ItemsSource = originalItemsList; // Optionally reset the list or keep it filtered
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            // reset the search textbox
            Tbxsearch.Text = string.Empty;

            // reset the listview to the original list
            listView.ItemsSource = originalItemsList;

            // reset the selected item to the first item in the list
            if (originalItemsList.Any())
            {
                listView.SelectedItem = originalItemsList[0];
            }

            StatusBarText.Text = "reset the search result";
        }

        private void ExportAllToExcel_Click(object sender, RoutedEventArgs e)
        {
            var items = listView.ItemsSource as IEnumerable<MainWindowViewModel>;
            if (items == null || !items.Any())
            {
                MessageBox.Show("No data available to export.");
                return;
            }

            SaveDataToExcel(items);
        }

        private void ExportSelectedToExcel_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = listView.SelectedItem as MainWindowViewModel;
            if (selectedItem == null)
            {
                MessageBox.Show("No item selected.");
                return;
            }

            SaveDataToExcel(new List<MainWindowViewModel> { selectedItem });
        }

        private void SaveDataToExcel(IEnumerable<MainWindowViewModel> items)
        {
            var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.Worksheets.Add("LeasesInfo");

            // add title
            worksheet.Cell(1, 1).Value = "Lease ID";
            worksheet.Cell(1, 2).Value = "Payment Due Day";
            worksheet.Cell(1, 3).Value = "Rent Amount";
            worksheet.Cell(1, 4).Value = "Address";
            worksheet.Cell(1, 5).Value = "Tenant Name";
            worksheet.Cell(1, 6).Value = "Phone No.";
            worksheet.Cell(1, 7).Value = "Email";
            worksheet.Cell(1, 8).Value = "Image Url";
            worksheet.Cell(1, 9).Value = "EmergencyContactName";
            worksheet.Cell(1, 10).Value = "EmergencyContact Phone No."; 

            int currentRow = 2;
            foreach (var item in items)
            {
                worksheet.Cell(currentRow, 1).Value = item.LeaseId;
                worksheet.Cell(currentRow, 2).Value = item.PaymentDueDay;
                worksheet.Cell(currentRow, 3).Value = item.RentAmount;
                worksheet.Cell(currentRow, 4).Value = item.Address;
                worksheet.Cell(currentRow, 5).Value = item.TenantName;
                worksheet.Cell(currentRow, 6).Value = item.PhoneNo;
                worksheet.Cell(currentRow, 7).Value = item.Email;
                worksheet.Cell(currentRow, 8).Value = item.ImageUrl;
                worksheet.Cell(currentRow, 9).Value = item.EmergencyContactName;
                worksheet.Cell(currentRow, 10).Value = item.EmergencyContactNo;

                currentRow++;
            }

            // 保存Excel文件
            var dialog = new Microsoft.Win32.SaveFileDialog()
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                FileName = "LeasesInfo.xlsx"
            };
            if (dialog.ShowDialog() == true)
            {
                workbook.SaveAs(dialog.FileName);
                MessageBox.Show("Data exported successfully.");
            }

        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            //load all lease information from the database
            StatusBarText.Text = "Refresh the Main Window";
            LoadMainWindow();
            
        }
    }
}