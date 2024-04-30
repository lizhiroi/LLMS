using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using LLMS;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Win32;
using System.IO;
using dotenv.net;
using static System.Net.WebRequestMethods;
using System.Threading.Tasks;
using LLMS.Validators;
using FluentValidation;

namespace LLMS.ViewModel
{
    public class TenantWindowViewModel : INotifyPropertyChanged 
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string BlobConnectionString;
        private testdb1Entities db;
        private string ContainerName;
        private TenantValidator _validator;

        private string _imageUrl; // Property to store the image URL

        public string ImageUrl
        {
            get { return _imageUrl; }
            set
            {
                _imageUrl = value;
                OnPropertyChanged(nameof(ImageUrl));
            }
        }


        public TenantWindowViewModel()
        {
            
            try
            {
                db = new testdb1Entities();
                AddCommand = new RelayCommand(Add);
                UpdateCommand = new RelayCommand(Update, CanUpdate);
                DeleteCommand = new RelayCommand(Delete, CanDelete);
                UploadImageCommand = new RelayCommand(UploadImage);
                _validator = new TenantValidator();

                LoadTenantData();

                // Initialize Azure Blob connection settings
                BlobConnectionString = Environment.GetEnvironmentVariable("ConnectionString");
                ContainerName = Environment.GetEnvironmentVariable("NameContainer");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tenant data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            LoadTenantData(); // Load tenant data including image URL
        }


        public void LoadTenantData()
        {
            // Fetch tenants with image URL
            var tenantsWithImageUrls = db.tenants.ToList().Select(tenant =>
            {
                // Fetch image URL based on profile_image_id
                tenant.ImageUrl = db.images.FirstOrDefault(img => img.id == tenant.profile_image_id)?.image_url;
                return tenant;
            });

            Tenants = new ObservableCollection<tenant>(tenantsWithImageUrls);
        }

        private ObservableCollection<tenant> _tenants;
        public ObservableCollection<tenant> Tenants
        {
            get { return _tenants; }
            set
            {
                _tenants = value;
                OnPropertyChanged(nameof(Tenants));
            }
        }

        private tenant _selectedTenant;
        public tenant SelectedTenant
        {
            get { return _selectedTenant; }
            set
            {
                _selectedTenant = value;
                OnPropertyChanged(nameof(SelectedTenant));

                // Update text box bindings when a new item is selected
                OnPropertyChanged(nameof(Id));
                OnPropertyChanged(nameof(Email));
                OnPropertyChanged(nameof(FirstName));
                OnPropertyChanged(nameof(LastName));
                OnPropertyChanged(nameof(StreetNumber));
                OnPropertyChanged(nameof(StreetName));
                OnPropertyChanged(nameof(CityName));
                OnPropertyChanged(nameof(Postcode));
                OnPropertyChanged(nameof(Province));
                OnPropertyChanged(nameof(PhoneNumber));
               
            }
        }

      

        public string Id
        {
            get { return SelectedTenant?.id.ToString(); }
            set
            {
                if (SelectedTenant != null && int.TryParse(value, out int result))
                {
                    SelectedTenant.id = result;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public string Email
        {
            get { return SelectedTenant?.email; }
            set
            {
                if (SelectedTenant != null)
                {
                    SelectedTenant.email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

        public string FirstName
        {
            get { return SelectedTenant?.first_name; }
            set
            {
                if (SelectedTenant != null)
                {
                    SelectedTenant.first_name = value;
                    OnPropertyChanged(nameof(FirstName));
                }
            }
        }

        public string LastName
        {
            get { return SelectedTenant?.last_name; }
            set
            {
                if (SelectedTenant != null)
                {
                    SelectedTenant.last_name = value;
                    OnPropertyChanged(nameof(LastName));
                }
            }
        }

        public string StreetNumber
        {
            get { return SelectedTenant?.street_number; }
            set
            {
                if (SelectedTenant != null)
                {
                    SelectedTenant.street_number = value;
                    OnPropertyChanged(nameof(StreetNumber));
                }
            }
        }

        public string StreetName
        {
            get { return SelectedTenant?.street_name; }
            set
            {
                if (SelectedTenant != null)
                {
                    SelectedTenant.street_name = value;
                    OnPropertyChanged(nameof(StreetName));
                }
            }
        }

        public string CityName
        {
            get { return SelectedTenant?.city_name; }
            set
            {
                if (SelectedTenant != null)
                {
                    SelectedTenant.city_name = value;
                    OnPropertyChanged(nameof(CityName));
                }
            }
        }

        public string Postcode
        {
            get { return SelectedTenant?.postcode; }
            set
            {
                if (SelectedTenant != null)
                {
                    SelectedTenant.postcode = value;
                    OnPropertyChanged(nameof(Postcode));
                }
            }
        }

        public string Province
        {
            get { return SelectedTenant?.province; }
            set
            {
                if (SelectedTenant != null)
                {
                    SelectedTenant.province = value;
                    OnPropertyChanged(nameof(Province));
                }
            }
        }

        public string PhoneNumber
        {
            get { return SelectedTenant?.phone_number; }
            set
            {
                if (SelectedTenant != null)
                {
                    SelectedTenant.phone_number = value;
                    OnPropertyChanged(nameof(PhoneNumber));
                }
            }
        }

        



        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand UploadImageCommand { get; }


        private bool CanUpdate(object parameter) => SelectedTenant != null;

        private void Add(object parameter)
        {
            try
            {
                // Validate the tenant model before adding
                var validationResult = _validator.Validate(SelectedTenant);
                if (!validationResult.IsValid)
                {
                    // Validation failed, handle error
                    MessageBox.Show(validationResult.Errors.First().ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Tenant model is valid, proceed with adding
                tenant newTenant = new tenant
                {
                    email = Email,
                    first_name = FirstName,
                    last_name = LastName,
                    street_number = StreetNumber,
                    street_name = StreetName,
                    city_name = CityName,
                    postcode = Postcode,
                    province = Province,
                    phone_number = PhoneNumber
                };

                db.tenants.Add(newTenant);
                db.SaveChanges();

                LoadTenantData();
                MessageBox.Show("Tenant added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding tenant: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Update(object parameter)
        {
            try
            {
                // Validate the tenant model before updating
                var validationResult = _validator.Validate(SelectedTenant);
                if (!validationResult.IsValid)
                {
                    // Validation failed, handle error
                    MessageBox.Show(validationResult.Errors.First().ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                db.SaveChanges();
                LoadTenantData();
                MessageBox.Show("Tenant updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating tenant: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanDelete(object parameter) => SelectedTenant != null;

        private void Delete(object parameter)
        {
            try
            {
                db.tenants.Remove(SelectedTenant);
                db.SaveChanges();
                LoadTenantData();
                MessageBox.Show("Tenant deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting tenant: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void UploadImage(object parameter)
        {
            try
            {
                // Open file dialog to select an image file
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    string filePath = openFileDialog.FileName;

                    // Upload the selected image file to the database
                    string imageUrl = await UploadImageToDatabase(filePath);

                    // Update the tenant's profile_image_id with the ID of the uploaded image
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        int? imageId = SaveImageUrlToDatabase(imageUrl);
                        if (imageId.HasValue)
                        {
                            SelectedTenant.profile_image_id = imageId.Value;

                            // Reload the image URL
                            SelectedTenant.ImageUrl = db.images.FirstOrDefault(img => img.id == imageId.Value)?.image_url;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private int? SaveImageUrlToDatabase(string imageUrl)
        {
            try
            {
                // Create a new image entity with the image URL
                image newImage = new image
                {
                    image_url = imageUrl,
                    uploaded_at = DateTime.Now
                };

                // Add the new image to the database and save changes
                db.images.Add(newImage);
                db.SaveChanges();

                // Return the ID of the newly added image
                return newImage.id;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving the image URL to the database: {ex.Message}");
                return null;
            }
        }



        private async Task<string> UploadImageToDatabase(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    MessageBox.Show("Please select a file first.");
                    return null;
                }

                // Create Blob client and container reference
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(BlobConnectionString);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(ContainerName);
                await container.CreateIfNotExistsAsync();

                // Upload image to Blob
                string fileName = System.IO.Path.GetFileName(filePath);
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
                await blockBlob.UploadFromFileAsync(filePath);

                MessageBox.Show($"File uploaded successfully.");

                // Return the URL of the uploaded image
                return blockBlob.Uri.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
                return null;
            }
        }



        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
