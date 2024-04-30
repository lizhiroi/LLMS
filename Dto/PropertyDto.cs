using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LLMS.Dto
{
    public class PropertyDto : IDataErrorInfo, INotifyPropertyChanged
    {
        private bool _hasBeenEdited = false;
        private int _id;
        private int _numberOfUnits;
        private string _address;
        private string _propertyType;
        private int _sizeInSqFt;
        private DateTime _yearBuilt;
        private decimal _rentalPrice;
        private string _amenities;
        private int _status;
        private string _leaseTerms;
        private string _imageUrl;
        private string _description;
        private int _imageId;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public int NumberOfUnits
        {
            get => _numberOfUnits;
            set => SetProperty(ref _numberOfUnits, value);
        }

        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        public string PropertyType
        {
            get => _propertyType;
            set => SetProperty(ref _propertyType, value);
        }

        public int SizeInSqFt
        {
            get => _sizeInSqFt;
            set => SetProperty(ref _sizeInSqFt, value);
        }

        public DateTime YearBuilt
        {
            get => _yearBuilt;
            set => SetProperty(ref _yearBuilt, value);
        }

        public decimal RentalPrice
        {
            get => _rentalPrice;
            set => SetProperty(ref _rentalPrice, value);
        }

        public string Amenities
        {
            get => _amenities;
            set => SetProperty(ref _amenities, value);
        }

        public int Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public string LeaseTerms
        {
            get => _leaseTerms;
            set => SetProperty(ref _leaseTerms, value);
        }

        public string ImageUrl
        {
            get => _imageUrl;
            set => SetProperty(ref _imageUrl, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public int ImageId
        {
            get => _imageId;
            set => SetProperty(ref _imageId, value);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (!Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
                MarkAsEdited();
            }
        }

        public void MarkAsEdited()
        {
            _hasBeenEdited = true;
        }

        public bool HasBeenEdited => _hasBeenEdited;

        public string Error => string.Empty;

        public string this[string propertyName]
        {
            get
            {
                if (!_hasBeenEdited) return string.Empty;

                string error = string.Empty;
                switch (propertyName)
                {
                    case nameof(NumberOfUnits):
                        if (NumberOfUnits < 1) error = "must be greater than 0.";
                        break;
                    case nameof(Address):
                        if (string.IsNullOrWhiteSpace(Address)) error = "required.";
                        break;
                    case nameof(PropertyType):
                        if (string.IsNullOrWhiteSpace(PropertyType)) error = "required.";
                        break;

                    case nameof(SizeInSqFt):
                        if (SizeInSqFt <= 0) error = "must be > 0 square feet.";
                        break;
                    case nameof(YearBuilt):
                        if (YearBuilt.Year < 1800 || YearBuilt.Year > DateTime.Now.Year)
                            error = $"must be from 1800 to {DateTime.Now.Year}.";
                        break;
                    case nameof(RentalPrice):
                        if (RentalPrice <= 0) error = "must be greater than 0.";
                        break;
                    case nameof(Amenities):
                        // TODO: Add validation logic as needed
                        break;
                    case nameof(Status):
                        // TODO: Add validation logic as needed
                        break;
                    case nameof(LeaseTerms):
                        if (string.IsNullOrWhiteSpace(LeaseTerms)) error = "Lease terms are required.";
                        break;
                    case nameof(ImageUrl):
                        // TODO: Add validation logic as needed
                        if (ImageUrl == null) error = "required.";
                        break;
                    case nameof(Description):
                        // TODO: Add validation logic as needed
                        break;
                }
                return error;
            }
        }
    }
}
