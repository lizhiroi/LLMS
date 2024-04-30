using FluentValidation;
using LLMS.Validators;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LLMS.ViewModel
{

   
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<string> uploadImage)
        {
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object parameter) => _execute(parameter);
    }

    public class LeaseWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private testdb1Entities db;
        private readonly IValidator<leas> _validator;

        public ObservableCollection<int> PropertyIds { get; set; }
        public ObservableCollection<int> TenantIds { get; set; }
        // Constructor 增加辅助方法调用

        public LeaseWindowViewModel()
        {
            try
            {
                db = new testdb1Entities();
                AddCommand = new RelayCommand(Add);
                UpdateCommand = new RelayCommand(Update, CanUpdate);
                DeleteCommand = new RelayCommand(Delete, CanDelete);
                _validator = new LeaseValidator();

                LoadLeaseData();
                LoadPropertyIds();
                LoadTenantIds();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading lease data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LoadLeaseData()
        {
            Leases = new ObservableCollection<leas>(db.leases.ToList());
        }

        private void LoadPropertyIds()
        {
            PropertyIds = new ObservableCollection<int>(db.properties.Select(p => p.id).ToList());
            OnPropertyChanged(nameof(PropertyIds));
        }

        private void LoadTenantIds()
        {
            TenantIds = new ObservableCollection<int>(db.tenants.Select(t => t.id).ToList());
            OnPropertyChanged(nameof(TenantIds));
        }

        private ObservableCollection<leas> _leases;
        public ObservableCollection<leas> Leases
        {
            get { return _leases; }
            set
            {
                _leases = value;
                OnPropertyChanged(nameof(Leases));
            }
        }

        private leas _selectedLease;
        public leas SelectedLease
        {
            get { return _selectedLease; }
            set
            {
                _selectedLease = value;
                OnPropertyChanged(nameof(SelectedLease));

                // Update text box bindings when a new item is selected
                OnPropertyChanged(nameof(PropertyId));
                OnPropertyChanged(nameof(TenantId));
                OnPropertyChanged(nameof(StartDate));
                OnPropertyChanged(nameof(EndDate));
                OnPropertyChanged(nameof(RentAmount));
                OnPropertyChanged(nameof(LeaseClauses));
                OnPropertyChanged(nameof(PaymentDueDay));
                OnPropertyChanged(nameof(UtilityByOwner));
                OnPropertyChanged(nameof(UtilityByTenant));
                OnPropertyChanged(nameof(RenewalTerm));
                OnPropertyChanged(nameof(EarlyTerminateCon));
            }
        }

        public string PropertyId
        {
            get { return SelectedLease?.property_id.ToString(); }
            set
            {
                if (SelectedLease != null && int.TryParse(value, out int result))
                {
                    SelectedLease.property_id = result;
                    OnPropertyChanged(nameof(PropertyId));
                }
            }
        }

        public string TenantId
        {
            get { return SelectedLease?.tenant_id.ToString(); }
            set
            {
                if (SelectedLease != null && int.TryParse(value, out int result))
                {
                    SelectedLease.tenant_id = result;
                    OnPropertyChanged(nameof(TenantId));
                }
            }
        }

        public string StartDate
        {
            get { return SelectedLease?.start_date.ToShortDateString(); }
            set
            {
                if (SelectedLease != null && DateTime.TryParse(value, out DateTime result))
                {
                    SelectedLease.start_date = result;
                    OnPropertyChanged(nameof(StartDate));
                }
            }
        }

        public string EndDate
        {
            get { return SelectedLease?.end_date.ToShortDateString(); }
            set
            {
                if (SelectedLease != null && DateTime.TryParse(value, out DateTime result))
                {
                    SelectedLease.end_date = result;
                    OnPropertyChanged(nameof(EndDate));
                }
            }
        }
        public string RentAmount
        {
            get { return SelectedLease?.rent_amount.ToString(); }
            set
            {
                if (SelectedLease != null && decimal.TryParse(value, out decimal result))
                {
                    SelectedLease.rent_amount = result;
                    OnPropertyChanged(nameof(RentAmount));
                }
            }
        }

        public string LeaseClauses
        {
            get { return SelectedLease?.lease_clauses; }
            set
            {
                if (SelectedLease != null)
                {
                    SelectedLease.lease_clauses = value;
                    OnPropertyChanged(nameof(LeaseClauses));
                }
            }
        }

        public string PaymentDueDay
        {
            get { return SelectedLease?.payment_due_day.ToString(); }
            set
            {
                if (SelectedLease != null && int.TryParse(value, out int result))
                {
                    SelectedLease.payment_due_day = result;
                    OnPropertyChanged(nameof(PaymentDueDay));
                }
            }
        }

        public string UtilityByOwner
        {
            get { return SelectedLease?.utility_by_owner; }
            set
            {
                if (SelectedLease != null)
                {
                    SelectedLease.utility_by_owner = value;
                    OnPropertyChanged(nameof(UtilityByOwner));
                }
            }
        }

        public string UtilityByTenant
        {
            get { return SelectedLease?.utility_by_tenant; }
            set
            {
                if (SelectedLease != null)
                {
                    SelectedLease.utility_by_tenant = value;
                    OnPropertyChanged(nameof(UtilityByTenant));
                }
            }
        }

        public string RenewalTerm
        {
            get { return SelectedLease?.renewal_term; }
            set
            {
                if (SelectedLease != null)
                {
                    SelectedLease.renewal_term = value;
                    OnPropertyChanged(nameof(RenewalTerm));
                }
            }
        }

        public string EarlyTerminateCon
        {
            get { return SelectedLease?.early_terminate_con; }
            set
            {
                if (SelectedLease != null)
                {
                    SelectedLease.early_terminate_con = value;
                    OnPropertyChanged(nameof(EarlyTerminateCon));
                }
            }
        }

       


        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }

        private bool CanUpdate(object parameter) => SelectedLease != null;

        private int? GetSelectedPropertyIdFromUI()
        {
            if (SelectedLease != null)
            {
                return SelectedLease.property_id;
            }
            return null;
        }

        // Method to retrieve the selected tenant ID from the UI
        private int? GetSelectedTenantIdFromUI()
        {
            if (SelectedLease != null)
            {
                return SelectedLease.tenant_id;
            }
            return null;
        }

        private void Add(object parameter)
        {
            try
            {
                // Validate the tenant model before adding
                var validationResult = _validator.Validate(SelectedLease);
                if (!validationResult.IsValid)
                {
                    // Validation failed, handle error
                    MessageBox.Show(validationResult.Errors.First().ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                leas newLease = new leas
                {
                    // Initialize properties from text boxes

                    property_id = int.Parse(PropertyId), // Assuming PropertyId is a string property representing the property ID from UI
                    tenant_id = int.Parse(TenantId), // Assuming TenantId is a string property representing the tenant ID from UI
                    start_date = DateTime.Parse(StartDate), // Assuming StartDate is a string property representing the start date from UI
                    end_date = DateTime.Parse(EndDate), // Assuming EndDate is a string property representing the end date from UI
                    rent_amount = decimal.Parse(RentAmount), // Assuming RentAmount is a string property representing the rent amount from UI
                    created_at = DateTime.Now,
                    updated_at = null, // Assuming UpdatedAt is a string property representing the updated date from UI
                    lease_clauses = LeaseClauses,
                    payment_due_day = int.Parse(PaymentDueDay), // Assuming PaymentDueDay is a string property representing the payment due day from UI
                    utility_by_owner = UtilityByOwner,
                    utility_by_tenant = UtilityByTenant,
                    renewal_term = RenewalTerm,
                    early_terminate_con = EarlyTerminateCon

        
    };

                db.leases.Add(newLease);
                db.SaveChanges();

                LoadLeaseData();
                MessageBox.Show("Lease added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (FormatException ex)
            {
                MessageBox.Show($"Error adding lease: Invalid input format. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding lease: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Update(object parameter)
        {
            try
            {    // Validate the tenant model before updating
                var validationResult = _validator.Validate(SelectedLease);
                if (!validationResult.IsValid)
                {
                    // Validation failed, handle error
                    MessageBox.Show(validationResult.Errors.First().ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                db.SaveChanges();
                LoadLeaseData();
                MessageBox.Show("Lease updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating lease: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private bool CanDelete(object parameter) => SelectedLease != null;

        private void Delete(object parameter)
        {
            try
            {
                db.leases.Remove(SelectedLease);
                db.SaveChanges();
                LoadLeaseData();
                MessageBox.Show("Lease deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting lease: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

       

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
