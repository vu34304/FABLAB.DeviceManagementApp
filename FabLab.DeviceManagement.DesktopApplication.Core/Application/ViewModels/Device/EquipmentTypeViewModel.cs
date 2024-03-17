﻿using AutoMapper;
using CommunityToolkit.Mvvm.Input;
using FabLab.DeviceManagement.DesktopApplication.Core.Application.Store;
using FabLab.DeviceManagement.DesktopApplication.Core.Application.ViewModels.SeedWork;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Dtos.Equipments;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Dtos.EquipmentTypes;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Exceptions;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Models.Equipments;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Models.EquipmentTypes;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Services;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;
using System.Windows.Media.Imaging;
using NPOI.POIFS.FileSystem;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Dtos.Tags;
using static System.Net.Mime.MediaTypeNames;

namespace FabLab.DeviceManagement.DesktopApplication.Core.Application.ViewModels.Device
{
    public class EquipmentTypeViewModel : BaseViewModel
    {
        public ObservableCollection<ImageEquimentType> ImageEquimentTypes { get; set; } = new();
        private readonly IApiService _apiService;
        private readonly IMapper _mapper;
        private readonly EquipmentTypeStore _equipmentTypeStore;
        public string EquipmentTypeId { get; set; } = "";
        public string EquipmentTypeName { get; set; } = "";
        public ECategory Category { get; set; }

        private EquipmentTypeEntryViewModel _SelectedItem;
        public EquipmentTypeEntryViewModel SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();

                if (SelectedItem != null)
                {
                    NewEquipmentTypeId = SelectedItem.EquipmentTypeId;
                    NewEquipmentTypeName = SelectedItem.EquipmentTypeName;
                    NewCategory = SelectedItem.Category;
                    NewDescription = SelectedItem.Description;
                    NewTag = SelectedItem.Tag;
                    NewSpecificationEquimentTypes = SelectedItem.SpecificationEquimentTypes;
                }
            }
        }

        public List<FileDataBase64EquipmentType> DataPics { get; set; }
        private List<TagDto> Tags = new();
        public List<string> TagIds { get; set; } = new();

        //Create New Equipment
        public string NewEquipmentTypeId { get; set; } = "";
        public string NewEquipmentTypeName { get; set; } = "";
        public ECategory NewCategory { get; set; }
        public string NewDescription { get; set; } = "";
        public List<string> NewTag { get; set; } = new();


        private List<EquipmentTypeDto> equipmentTypes = new();
        private List<EquipmentTypeDto> filteredEquipmentTypes = new();
        public ObservableCollection<EquipmentTypeEntryViewModel> EquipmentTypeEntries { get; set; } = new();
        public ObservableCollection<string> EquipmentTypeIds => _equipmentTypeStore.EquipmentTypeIds;
        public ObservableCollection<string> EquipmentTypeNames => _equipmentTypeStore.EquipmentTypeNames;
        //public ObservableCollection<string> Tags => _tagStore.TagIds;
     

        //Thong so
        public string NewName { get;set; }
        public string NewValue { get; set; }
        public string NewUnit { get; set; }
        public ObservableCollection<SpecificationEquimentType> NewSpecificationEquimentTypes { get; set; } = new();

        //Picture
        public ObservableCollection<FileDataEquimentType> NewPictures { get; set; } = new();
        



        public ICommand LoadEquipmentTypeEntriesCommand { get; set; }
        public ICommand LoadInitialCommand { get; set; }
        public ICommand LoadEquipmentTypeViewCommand { get; set; }
        public ICommand CreateEquipmentTypeCommand { get; set; }
        public ICommand AddSpecification { get; set; }
        public ICommand DeleteSpecification { get; set; }

        public ICommand DeleteImageCommand { get; set; }
        public ICommand SelectImageCommand { get; set; }

        public EquipmentTypeViewModel(IApiService apiService, IMapper mapper, EquipmentTypeStore equipmentTypeStore)
        { 
            _apiService = apiService;
            _mapper = mapper;
            _equipmentTypeStore = equipmentTypeStore;

            LoadInitialCommand = new RelayCommand(LoadInitial);
            LoadEquipmentTypeEntriesCommand = new RelayCommand(LoadEquipmentTypeEntries);
            LoadEquipmentTypeViewCommand = new RelayCommand(LoadEquipmentTypeView);
            CreateEquipmentTypeCommand = new RelayCommand(CreateEquipmentTypeAsync);
            SelectImageCommand = new RelayCommand(SelectImage);
            AddSpecification = new RelayCommand(AddSpec);
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
            DeleteSpecification = new RelayCommand<SpecificationEquimentType>(execute: DeleteSpec);
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
            DeleteImageCommand = new RelayCommand<ImageEquimentType>(execute: DeleteImage);
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async void LoadEquipmentTypeView()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            LoadInitial();
            OnPropertyChanged(nameof(EquipmentTypeIds));
            OnPropertyChanged(nameof(EquipmentTypeNames));
            UpdateTag();
           
        }

        private async void LoadInitial()
        {   

            Category = ECategory.All;

            EquipmentTypeId = "";
            EquipmentTypeName = "";

            NewName = "";
            NewValue = "";
            NewUnit = "";
            NewSpecificationEquimentTypes = new();
            NewPictures = new();
            ImageEquimentTypes = new();
      
            try
            {
                equipmentTypes = (await _apiService.GetAllEquipmentTypesAsync()).ToList();
                var viewModels = _mapper.Map<IEnumerable<EquipmentTypeDto>, IEnumerable<EquipmentTypeEntryViewModel>>(equipmentTypes);

                EquipmentTypeEntries = new(viewModels);
                if (_mapper is not null && _apiService is not null)
                {
                    foreach (var entry in EquipmentTypeEntries)
                    {
                        entry.SetApiService(_apiService);
                        entry.SetMapper(_mapper);
                        entry.Updated += LoadInitial;
                        entry.OnException += Error;                     
                    }
                }  
            }
            catch (HttpRequestException)
            {
                ShowErrorMessage("Đã có lỗi xảy ra: Mất kết nối với server.");
            }

            //Load Info
            Task[] LoadingInfo = EquipmentTypeEntries.Select(entry => LoadInfo(entry)).ToArray();
            await Task.WhenAll(LoadingInfo);


        }

        private async Task LoadInfo(EquipmentTypeEntryViewModel entry)
        {
            if (_mapper is not null && _apiService is not null)
            {
                try
                {
                    if (!String.IsNullOrEmpty(entry.EquipmentTypeId))
                    {
                        var Dto = (await _apiService.GetInformationEquipmenAsync(entry.EquipmentTypeId));
                        entry.SpecificationEquimentTypes = new(Dto.Specs);
                        DataPics = Dto.Pics;
                        foreach (var pic in DataPics)
                        {
                            if (!String.IsNullOrEmpty(pic.fileData))
                            {
                                entry.Pictures.Add(new ImageBitmap()
                                {
                                    Source = Base64toImage(pic.fileData)
                                });
                            }
                        }
                    }
                }
                catch (HttpRequestException)
                {
                    ShowErrorMessage("Đã có lỗi xảy ra: Mất kết nối với server.");
                }
            }
        }

        private void LoadEquipmentTypeEntries()
        {
            try
            {
                if (Category == ECategory.All)
                {
                    filteredEquipmentTypes = equipmentTypes;
                    if (!String.IsNullOrEmpty(EquipmentTypeId))
                    {
                        filteredEquipmentTypes = equipmentTypes.Where(i => i.EquipmentTypeId.Contains(EquipmentTypeId)).ToList();
                    }
                    if (!String.IsNullOrEmpty(EquipmentTypeName))
                    {
                        filteredEquipmentTypes = equipmentTypes.Where(i => i.EquipmentTypeName.Contains(EquipmentTypeName)).ToList();
                    }
                    
                    var viewModels = _mapper.Map<IEnumerable<EquipmentTypeDto>, IEnumerable<EquipmentTypeEntryViewModel>>(filteredEquipmentTypes);
                    EquipmentTypeEntries = new(viewModels);
                }
                else
                {
                    filteredEquipmentTypes = equipmentTypes.Where(i => i.Category == Category).ToList();
                    if (!String.IsNullOrEmpty(EquipmentTypeId))
                    {
                        filteredEquipmentTypes = equipmentTypes.Where(i => i.EquipmentTypeId.Contains(EquipmentTypeId)).ToList();
                    }
                    if (!String.IsNullOrEmpty(EquipmentTypeName))
                    {
                        filteredEquipmentTypes = equipmentTypes.Where(i => i.EquipmentTypeName.Contains(EquipmentTypeName)).ToList();
                    }

                    var viewModels = _mapper.Map<IEnumerable< EquipmentTypeDto>, IEnumerable<EquipmentTypeEntryViewModel>>(filteredEquipmentTypes);
                    EquipmentTypeEntries = new(viewModels);
                }

                foreach (var entry in EquipmentTypeEntries)
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

        private void SelectImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Image files|*.bmp;*.jpg;*.png";
            openFileDialog.FilterIndex = 1;
            int index = 0;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
               
                string []files = openFileDialog.FileNames;
                foreach (string file in files)
                {
                    
                    index++;
                    ImageEquimentTypes.Add(new ImageEquimentType()
                    {  
                        ImagePath = file,
                        ImageName = "Picture " + $"{index}",
                        
                    })  ;
                    NewPictures.Add(new FileDataEquimentType()
                    {
                        fileData = System.IO.File.ReadAllBytes(file)

                    }) ;

                    
                }
            }
        }

        
        public void AddSpec()
        {
            NewSpecificationEquimentTypes.Add(new SpecificationEquimentType()
            {
                name = NewName,
                value = NewValue,
                unit = NewUnit,
            }
            );
        }

        private void DeleteSpec(SpecificationEquimentType obj)
        {
            NewSpecificationEquimentTypes.Remove(obj);           
        }

        private void DeleteImage(ImageEquimentType obj)
        {
            ImageEquimentTypes.Remove(obj);
        }

        
        private async void CreateEquipmentTypeAsync()
        {
            
            var createDto = new CreateEquimentTypeDto(
                NewEquipmentTypeId,
                NewEquipmentTypeName,
                NewDescription,
                NewCategory,
                NewTag,
                NewPictures,
                NewSpecificationEquimentTypes);

            
            try
            {
                await _apiService.CreateEquipmentType(createDto);
                LoadEquipmentTypeEntries();
                LoadEquipmentTypeView();
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

            NewEquipmentTypeId = "";
            NewEquipmentTypeName = "";
            NewDescription = "";
            

        }

        private async void UpdateTag()
        {
            try
            {
                Tags = (await _apiService.GetAllTagAsync()).ToList();
                TagIds = Tags.Select(i => i.TagId).ToList();

            }
            catch (HttpRequestException)
            {
                ShowErrorMessage("Đã có lỗi xảy ra: Mất kết nối với server.");
            }
        }
        public BitmapImage Base64toImage(string Base64)
        {
            byte[] binarydata = Convert.FromBase64String(Base64);

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = new MemoryStream(binarydata);
            bi.EndInit();
            return bi;
        }

       

    }
}
