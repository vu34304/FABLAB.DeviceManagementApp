﻿using AutoMapper;
using CommunityToolkit.Mvvm.Input;
using FabLab.DeviceManagement.DesktopApplication.Core.Application.ViewModels.SeedWork;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Dtos.Projects;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Services;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FabLab.DeviceManagement.DesktopApplication.Core.Application.ViewModels.Project
{
    public class ProjectManagementViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;
        private readonly IMapper _mapper;
        public List<string> ProjectNames { get; set; } = new();
        public string ProjectName { get; set; }

        public ObservableCollection<ProjectManagementEntryViewModel> ProjectManagementEntries { get; set; } = new();
        public ObservableCollection<string> Equipments { get; set; } = new ObservableCollection<string>();
        public List<ProjectDto> projects { get; set; } = new();
        private List<ProjectDto> filteredprojects = new();
        public ICommand LoadProjectManagementViewCommand { get; set; }
        public ICommand AddEquipmentCommand { get; set; }
        public ICommand LoadProjectEntriesCommand { get; set; }

        public ProjectManagementViewModel(IApiService apiService, IMapper mapper)
        {
            _apiService = apiService;
            _mapper = mapper;
            LoadProjectManagementViewCommand = new RelayCommand(LoadProjectManagementView);
            LoadProjectEntriesCommand = new RelayCommand(LoadProjectEntries);
        }

        private void LoadProjectManagementView()
        {
            LoadInitial();
            UpdateProjectNames();
            OnPropertyChanged(nameof(ProjectNames));
        }
        private void Error()
        {
            ShowErrorMessage("Đã có lỗi xảy ra: Mất kết nối với server.");
        }
        private async void UpdateProjectNames()
        {
            try
            {
                projects = (await _apiService.GetAllProjectsAsync()).ToList();
                ProjectNames = projects.Select(i => i.ProjectName).ToList();

            }
            catch (HttpRequestException)
            {
                ShowErrorMessage("Đã có lỗi xảy ra: Mất kết nối với server.");
            }
        }

        private async void LoadInitial()
        {
            try
            {
                projects = (await _apiService.GetAllProjectsAsync()).ToList();
                var viewModels = _mapper.Map<IEnumerable<ProjectDto>, IEnumerable<ProjectManagementEntryViewModel>>(projects);

                ProjectManagementEntries = new(viewModels);
                if (_mapper is not null && _apiService is not null)
                {
                    foreach (var entry in ProjectManagementEntries)
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
        }

        private void LoadProjectEntries()
        {
            try
            {

                if (!String.IsNullOrEmpty(ProjectName))
                {
                    filteredprojects = projects.Where(i => i.ProjectName.Contains(ProjectName)).ToList();
                }


                var viewModels = _mapper.Map<IEnumerable<ProjectDto>, IEnumerable<ProjectManagementEntryViewModel>>(filteredprojects);
                ProjectManagementEntries = new(viewModels);

                foreach (var entry in ProjectManagementEntries)
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
        private void AddEquipment()
        {

        }
    }
}
