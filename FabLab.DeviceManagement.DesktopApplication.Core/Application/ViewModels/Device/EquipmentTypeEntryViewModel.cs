using AutoMapper;
using CommunityToolkit.Mvvm.Input;
using FabLab.DeviceManagement.DesktopApplication.Core.Application.Store;
using FabLab.DeviceManagement.DesktopApplication.Core.Application.ViewModels.SeedWork;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Dtos.Equipments;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Dtos.EquipmentTypes;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Models.Equipments;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Models.EquipmentTypes;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Services;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MessageBox = System.Windows.MessageBox;

namespace FabLab.DeviceManagement.DesktopApplication.Core.Application.ViewModels.Device
{
    public class EquipmentTypeEntryViewModel : BaseViewModel
    {
        private IApiService? _apiService;
        private IMapper? _mapper;
        public string EquipmentTypeId { get; set; }
        public string EquipmentTypeName { get; set; }
        public ECategory Category { get; set; }
        public string Description { get; set; }

        //Thong so
        public ObservableCollection<SpecificationEquimentType> SpecificationEquimentTypes { get; set; }
        //Hinh anh
        public ObservableCollection<ImageBitmap> Pictures { get; set; } = new();



        public List<string> Tag { get; set; } = new();


        public event Action? Updated;
        public event Action? OnException;
        public ICommand SaveCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
      

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public EquipmentTypeEntryViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            //SaveCommand = new RelayCommand(SaveAsync);
            DeleteCommand = new RelayCommand(DeleteAsync);
        }
        public EquipmentTypeEntryViewModel(string equipmentTypeId, string equipmentTypeName, ECategory category, string description) : this()
        {
            EquipmentTypeId = equipmentTypeId;
            EquipmentTypeName = equipmentTypeName;
            Category = category;
            Description = description;          
        }
     


        public void SetApiService(IApiService apiService)
        {
            _apiService = apiService;
        }
        public void SetMapper(IMapper mapper)
        {
            _mapper = mapper;
            OnPropertyChanged();
        }
        
        //private async void SaveAsync()
        //{

        //    EquipmentTypeDto fixDto = new EquipmentTypeDto(EquipmentTypeId,EquipmentTypeName, Description, Category);
        //    if (_mapper is not null && _apiService is not null)
        //    {
        //        try
        //        {
        //            await _apiService.FixEquipmentTypesAsync(fixDto);
        //            MessageBox.Show("Đã Cập Nhật", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        //        }
        //        catch (HttpRequestException)
        //        {
        //            OnException?.Invoke();
        //            ShowErrorMessage("Đã có lỗi xảy ra: Mất kết nối với server.");
        //        }
        //    }
        //    Updated?.Invoke();
        //}

        private async void DeleteAsync()
        {
            if (_mapper is not null && _apiService is not null)
            {
                try
                {
                    if (MessageBox.Show("Xác nhận xóa", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        await _apiService.DeleteEquipmentTypeAsync(EquipmentTypeId);
                        Updated?.Invoke();
                        MessageBox.Show("Đã Cập Nhật", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        
                    }
                    else { }
                }
                catch (HttpRequestException)
                {
                    OnException?.Invoke();
                    ShowErrorMessage("Đã có lỗi xảy ra: Mất kết nối với server.");
                }
            }

            
        }

    }
}
