using AutoMapper;
using System.Windows.Navigation;
using CommunityToolkit.Mvvm.Input;
using FabLab.DeviceManagement.DesktopApplication.Core.Application.Store;
using FabLab.DeviceManagement.DesktopApplication.Core.Application.ViewModels.SeedWork;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Dtos.Equipments;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Exceptions;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Models.Equipments;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;
using MathNet.Numerics.RootFinding;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Dtos.EquipmentTypes;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Dtos.Locations;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Dtos.Suppliers;
using System.Windows.Controls;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FabLab.DeviceManagement.DesktopApplication.Core.Application.ViewModels.Device
{
    public class DeviceManagementViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;
        private readonly IMapper _mapper;

        public DateTime StartDate { get; set; } = DateTime.Now.AddDays(-7).Date;
        public DateTime EndDate { get; set; } = DateTime.Now.Date;
        public string YearSelected { get; set; } = "";

        private DeviceEntryViewModel _SelectedItem;
        public DeviceEntryViewModel SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();

                if(SelectedItem != null)
                {
                    NewEquipmentId = SelectedItem.EquipmentId;
                    NewEquipmentName = SelectedItem.EquipmentName;
                    NewCodeOfManage = SelectedItem.CodeOfManager;
                    NewStatus = SelectedItem.Status;
                    NewYearOfSupply = SelectedItem.YearOfSupply;
                    NewLocationId = SelectedItem.LocationId;
                    NewSupplierName = SelectedItem.SupplierName;
                    NewEquipmentTypeId = SelectedItem.EquipmentTypeId;
                }
            }
        }

        public string EquipmentId { get; set; } = "";
        public string EquipmentName { get; set; } = "";
        public string EquipmentName1 { get; set; } = "";
        public string YearOfSupply { get; set; } = "";
        public string CodeOfManage { get; set; } = "";
        public string SupplierName { get; set; } = "";
        public string LocationId { get; set; } = "";
        public string EquipmentTypeName { get; set; } = "";
        public string EquipmentTypeId { get; set; } = "";
        public EStatus Status { get; set; }
        public ECategory Category   { get; set; }
     
        public string NewEquipmentId { get; set; } = "";
        public string NewEquipmentName { get; set; } = "";
        public DateTime NewYearOfSupply { get; set; } = DateTime.Now.Date;
        public string NewCodeOfManage { get; set; } = "";
        public string NewSupplierName { get; set; } = "";
        public string NewLocationId { get; set; } = "";
        public string? NewEquipmentTypeId { get; set; } = "";
        public EStatus NewStatus { get; set; }
        public ECategory NewCategory { get; set; }


        private List<EquipmentDto> equipments = new();
        private List<EquipmentDto> filteredEquipments = new();
        private List<SupplierDto> suppliers = new();
        private List<LocationDto> locations = new();
        private List<EquipmentTypeDto> equimentTypes = new();

        public DeviceEntryViewModel DeviceEntry { get; set; }
        public ObservableCollection<DeviceEntryViewModel> DeviceEntries { get; set; } = new();




        public List<string> EquipmentNames { get; set; } = new();
        public List<string> EquipmentIds { get; set; } = new();
        public List<string> EquipmentTypeIds { get; set; } = new();
        public List<string> EquipmentTypeNames { get; set; } = new();
        public List<string> LocationIds { get; set; } = new();
        public List<string> SupplierNames { get; set; } = new();
        public List<string> CodeOfManagers { get; set; } = new();



        public ObservableCollection<string> Years { get; set; } = new();
        public ICommand LoadDeviceEntriesCommand { get; set; }
        public ICommand LoadDeviceEntriesCommand1 { get; set; }
        public ICommand LoadInitialCommand { get; set; }
        public ICommand LoadDeviceManagementViewCommand { get; set; }
        public ICommand CreateEquipmentCommand { get; set; }
        public ICommand DeleteEquipmentCommand { get; set; }
        public ICommand FixEquipmentCommand { get; set; }

        public DeviceManagementViewModel(IApiService apiService, IMapper mapper)
        {
            _apiService = apiService;
            _mapper = mapper;
            Years = new ObservableCollection<string>();
            for (int i = DateTime.Now.Year; i >= 1975; i--)
            {
                Years.Add(i.ToString());
            }

            LoadInitialCommand = new RelayCommand(LoadInitial);
            LoadDeviceEntriesCommand = new RelayCommand(LoadDeviceEntriesAdvance);
            LoadDeviceEntriesCommand1 = new RelayCommand(LoadDeviceEntriesBase);
            LoadDeviceManagementViewCommand = new RelayCommand(LoadDeviceManagementView);
            CreateEquipmentCommand = new RelayCommand(CreateEquipmentAsync);
            DeleteEquipmentCommand = new RelayCommand(DeleteAsync);
            FixEquipmentCommand = new RelayCommand(SaveAsync);
        }

        private void LoadDeviceManagementView()
        {
            LoadInitial();

            UpdateSupplier();
            UpdateEquimentTypeId();
            UpdateEquimentTypeName();
            UpdateLocation();

            OnPropertyChanged(nameof(EquipmentIds));
            OnPropertyChanged(nameof(EquipmentNames));
            OnPropertyChanged(nameof(EquipmentTypeIds));
            OnPropertyChanged(nameof(EquipmentTypeNames));
            OnPropertyChanged(nameof(LocationIds));
            OnPropertyChanged(nameof(SupplierNames));

        }

        private async void LoadInitial()
        {
            Category = ECategory.All;

            YearSelected = "";
            EquipmentId = "";
            EquipmentName = "";
            EquipmentTypeId = "";
            EquipmentTypeName = "";           

            try
            {
                equipments = (await _apiService.GetAllEquipmentsAsync()).ToList();
                //var filteredEquipmentsDtos = equipments.Where(i => i.ItemId.Contains(ItemIdFilter));

                var viewModels = _mapper.Map<IEnumerable<EquipmentDto>, IEnumerable<DeviceEntryViewModel>>(equipments);
                DeviceEntries = new(viewModels);

                foreach (var entry in DeviceEntries)
                {
                    entry.SetApiService(_apiService);
                    entry.SetMapper(_mapper);
                    entry.Updated += LoadInitial;
                    entry.OnException += Error;
                }
                EquipmentNames = DeviceEntries.Select(i => i.EquipmentName).ToList();
                EquipmentIds = DeviceEntries.Select(i => i.EquipmentId).ToList();
                CodeOfManagers = DeviceEntries.Select(i => i.CodeOfManager).ToList();
            }
            catch (HttpRequestException)
            {
                ShowErrorMessage("Đã có lỗi xảy ra: Mất kết nối với server .");
            }
        }

      
     

        private async void LoadDeviceEntriesAdvance()
        {
            try
            {
                if (!String.IsNullOrEmpty(EquipmentId) || !String.IsNullOrEmpty(EquipmentName) || !String.IsNullOrEmpty(YearOfSupply) || !String.IsNullOrEmpty(EquipmentTypeName)) 
                {
                    filteredEquipments = (await _apiService.GetEquipmentsRecordsAsync(EquipmentId, EquipmentName, YearOfSupply, EquipmentTypeName, Category, Status)).ToList();
                }
       
                //filteredEquipments = (await _apiService.GetEquipmentsRecordsAsync()).ToList();
                var viewModels = _mapper.Map<IEnumerable<EquipmentDto>, IEnumerable<DeviceEntryViewModel>>(filteredEquipments);
                DeviceEntries = new(viewModels);

                foreach (var entry in DeviceEntries)
                {
                    entry.SetApiService(_apiService);
                    entry.SetMapper(_mapper);
                    entry.Updated += LoadInitial;
                    entry.OnException += Error;
                }
            }
            catch (HttpRequestException)
            {
                ShowErrorMessage("Đã có lỗi xảy ra: Mất kết nối với server test.");
            }

        }
        private async void LoadDeviceEntriesBase()
        {
            try
            {
                if (!String.IsNullOrEmpty(EquipmentName1))
                {
                    filteredEquipments = (await _apiService.GetEquipmentsRecordsAsync(EquipmentName1)).ToList();
                }
 
                //filteredEquipments = (await _apiService.GetEquipmentsRecordsAsync()).ToList();
                var viewModels = _mapper.Map<IEnumerable<EquipmentDto>, IEnumerable<DeviceEntryViewModel>>(filteredEquipments);
                DeviceEntries = new(viewModels);

                foreach (var entry in DeviceEntries)
                {
                    entry.SetApiService(_apiService);
                    entry.SetMapper(_mapper);
                    entry.Updated += LoadInitial;
                    entry.OnException += Error;
                }
            }
            catch (HttpRequestException)
            {
                ShowErrorMessage("Đã có lỗi xảy ra: Mất kết nối với server.");
            }

        }

        private void Error()
        {
            ShowErrorMessage("Đã có lỗi xảy ra: Mất kết nối với server.");
        }

        private async void CreateEquipmentAsync()
        {            
            var createDto = new CreateEquipmentDto(
                NewEquipmentId,
                NewEquipmentName,
                NewYearOfSupply,
                NewCodeOfManage,
                NewStatus,
                NewLocationId,
                NewSupplierName,
                NewEquipmentTypeId);
            try
            {
                await _apiService.CreateEquipment(createDto);
                LoadDeviceManagementView();
            }
            catch (HttpRequestException)
            {
                ShowErrorMessage("Đã có lỗi xảy ra: Mất kết nối với server.");
            }
            catch (DuplicateEntityException)
            {
                ShowErrorMessage("Đã có lỗi xảy ra: Mã vật tư đã tồn tại.");
            }
            catch (Exception)
            {
                ShowErrorMessage("Đã có lỗi xảy ra: Không thể tạo vật tư mới.");
            }
            MessageBox.Show("Đã Cập Nhật", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            NewEquipmentId = "";
            NewEquipmentName = "";
            NewYearOfSupply = DateTime.Now.Date;
            NewCodeOfManage = "";
            NewLocationId = "";
            NewSupplierName = "";
            NewStatus = EStatus.Active;
            NewEquipmentTypeId = "";
            //LoadManageItemView();
        }

        private async void UpdateSupplier()
        {
            try
            {
                suppliers = (await _apiService.GetAllSuppliersAsync()).ToList();
                SupplierNames = suppliers.Select(i => i.SupplierName).ToList();

            }
            catch (HttpRequestException)
            {
                ShowErrorMessage("Đã có lỗi xảy ra: Mất kết nối với server.");
            }
        }
        private async void UpdateLocation()
        {
            try
            {
                locations = (await _apiService.GetAllLocationsAsync()).ToList();
                LocationIds = locations.Select(i => i.LocationId).ToList();

            }
            catch (HttpRequestException)
            {
                ShowErrorMessage("Đã có lỗi xảy ra: Mất kết nối với server.");
            }
        }
        private async void UpdateEquimentTypeId()
        {
            try
            {
                equimentTypes = (await _apiService.GetAllEquipmentTypesAsync()).ToList();
                EquipmentTypeIds = equimentTypes.Select(i => i.EquipmentTypeId).ToList();

            }
            catch (HttpRequestException)
            {
                ShowErrorMessage("Đã có lỗi xảy ra: Mất kết nối với server.");
            }
        }
        private async void UpdateEquimentTypeName()
        {
            try
            {
                equimentTypes = (await _apiService.GetAllEquipmentTypesAsync()).ToList();
                EquipmentTypeNames = equimentTypes.Select(i => i.EquipmentTypeName).ToList();

            }
            catch (HttpRequestException)
            {
                ShowErrorMessage("Đã có lỗi xảy ra: Mất kết nối với server.");
            }
        }
        private async void DeleteAsync()
        {
            if (_mapper is not null && _apiService is not null)
            {
                try
                {
                    if (MessageBox.Show("Xác nhận xóa", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        await _apiService.DeleteEquipmentAsync(NewEquipmentId);
                        LoadDeviceManagementView();
                        MessageBox.Show("Đã Cập Nhật", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                    else { }

                }
                catch (HttpRequestException)
                {
                    ShowErrorMessage("Đã có lỗi xảy ra: Mất kết nối với server.");
                }
            }

        }

        private async void SaveAsync()
        {

            FixEquipmentDto fixDto = new FixEquipmentDto(NewEquipmentId, NewEquipmentName, NewYearOfSupply, NewCodeOfManage, NewStatus, NewLocationId, NewSupplierName, NewEquipmentTypeId);
            if (_mapper is not null && _apiService is not null)
            {
                try
                {
                    await _apiService.FixEquipmentAsync(fixDto);
                    MessageBox.Show("Đã Cập Nhật", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadDeviceManagementView();
                }
                catch (HttpRequestException)
                {
                    ShowErrorMessage("Đã có lỗi xảy ra: Mất kết nối với server.");
                }
            }
            NewEquipmentId = "";
            NewEquipmentName = "";
            NewYearOfSupply = DateTime.Now.Date;
            NewCodeOfManage = "";
            NewLocationId = "";
            NewSupplierName = "";
            NewStatus = EStatus.Active;
            NewEquipmentTypeId = "";

        }


    }
}
