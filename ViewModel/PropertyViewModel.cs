using LLMS.Dto;
using LLMS.Service;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Win32;

public class PropertyViewModel : BindableBase
{
    private readonly IPropertyService _propertyService;
    private readonly IImageService _imageService;
    private PropertyDto _selectedProperty;
    private string _statusMessage;
    private bool _isAddingNewProperty = false;

    public ObservableCollection<PropertyDto> Properties { get; private set; } = new ObservableCollection<PropertyDto>();
    public PropertyDto SelectedProperty
    {
        get => _selectedProperty;
        set => SetProperty(ref _selectedProperty, value, OnSelectedPropertyChanged);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public DelegateCommand UploadImageCommand { get; private set; }
    public DelegateCommand SavePropertyCommand { get; private set; }
    public DelegateCommand DeletePropertyCommand { get; private set; }

    public PropertyViewModel(IPropertyService propertyService, IImageService imageService)
    {
        _propertyService = propertyService ?? throw new ArgumentNullException(nameof(propertyService));
        _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));

        UploadImageCommand = new DelegateCommand(ExecuteUploadImageAsync, CanExecuteUploadImage)
            .ObservesProperty(() => SelectedProperty);
        SavePropertyCommand = new DelegateCommand(ExecuteSaveProperty, CanExecuteSaveProperty)
             .ObservesProperty(() => SelectedProperty);
        DeletePropertyCommand = new DelegateCommand(ExecuteDeleteProperty, CanExecuteDeleteProperty)
             .ObservesProperty(() => SelectedProperty);

        LoadPropertiesAsync();
    }
    
    public async void HandleFileDrop(string filePath)
    {
        try
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var fileName = Path.GetFileName(filePath);
                var imageUrl = await _imageService.UploadImageAsync(stream, fileName);
                if (SelectedProperty != null)
                {
                    SelectedProperty.ImageUrl = imageUrl;
                    RaisePropertyChanged(nameof(SelectedProperty));
                    var imageId = await _imageService.CreateImageRecordAsync(imageUrl);
                    SelectedProperty.ImageId = imageId;
                    StatusMessage = "Image uploaded successfully from drag and drop.";
                }
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to upload image from drag and drop: {ex.Message}";
        }
    }

    private bool CanExecuteUploadImage() => SelectedProperty != null && (_isAddingNewProperty || !string.IsNullOrEmpty(SelectedProperty.ImageUrl));
    private async void ExecuteUploadImageAsync()
    {
        try
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                using (var stream = File.OpenRead(openFileDialog.FileName))
                {
                    var imageUrl = await _imageService.UploadImageAsync(stream, Path.GetFileName(openFileDialog.FileName));
                    SelectedProperty.ImageUrl = imageUrl;
                    var imageId = await _imageService.CreateImageRecordAsync(imageUrl);
                    SelectedProperty.ImageId = imageId;
                    RaisePropertyChanged(nameof(SelectedProperty));
                    StatusMessage = "Image uploaded successfully.";
                }
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error uploading image: {ex.Message}";
        }
    }

    private bool CanExecuteSaveProperty() => SelectedProperty != null && (_isAddingNewProperty || SelectedProperty.Id == 0 || !string.IsNullOrWhiteSpace(SelectedProperty.Address));
    private async void ExecuteSaveProperty()
    {
        try
        {
            if (_isAddingNewProperty || SelectedProperty.Id == 0)
            { 
                PropertyDto createdProperty = await _propertyService.CreatePropertyAsync(SelectedProperty);
                if (createdProperty != null)
                {
                    Properties.Add(createdProperty);
                    RaisePropertyChanged(nameof(Properties));
                    StatusMessage = "Property added successfully.";
                }
            }
            else
            {
                var updatedProperty = await _propertyService.UpdatePropertyAsync(SelectedProperty);
                var index = Properties.IndexOf(SelectedProperty);
                if (index != -1)
                {
                    Properties[index] = updatedProperty;
                    RaisePropertyChanged(nameof(Properties));
                    StatusMessage = "Property updated successfully.";
                }
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error saving property: {ex.Message}";
        }
    }

    private bool CanExecuteDeleteProperty() => SelectedProperty != null && SelectedProperty.Id > 0;
    private async void ExecuteDeleteProperty()
    {
        try
        {
            var result = await _propertyService.DeletePropertyAsync(SelectedProperty.Id);
            if (result)
            {
                Properties.Remove(SelectedProperty);
                SelectedProperty = null;
                StatusMessage = "Property deleted successfully.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error deleting property: {ex.Message}";
        }
    }

    public async void LoadPropertiesAsync()
    {
        var properties = await _propertyService.GetAllPropertiesAsync();
        Properties.Clear();
        Properties.Add(new PropertyDto
        {
            Address = "Add new property...",
            YearBuilt = new DateTime(1800, 1, 1)
        });
        foreach (var property in properties)
        {
            Properties.Add(property);
        }
    }

    public void OnSelectedPropertyChanged()
    {
        if (SelectedProperty != null && (SelectedProperty.Id == 0 || SelectedProperty.Address == "Add new property..."))
        {
            _isAddingNewProperty = true;
        }
        else
        {
            _isAddingNewProperty = false;
        }

        UploadImageCommand.RaiseCanExecuteChanged();
        SavePropertyCommand.RaiseCanExecuteChanged();
        DeletePropertyCommand.RaiseCanExecuteChanged();
    }
}

