using CommunityToolkit.Mvvm.Input;
using FabLab.DeviceManagement.DesktopApplication.Core.Application.ViewModels.BorrowReturn;
using FabLab.DeviceManagement.DesktopApplication.Core.Application.ViewModels.Device;
using FabLab.DeviceManagement.DesktopApplication.Core.Application.ViewModels.Maintenance;
using FabLab.DeviceManagement.DesktopApplication.Core.Application.ViewModels.Project;
using FabLab.DeviceManagement.DesktopApplication.Core.Application.ViewModels.SeedWork;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FabLab.DeviceManagement.DesktopApplication.Core.Application.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IDatabaseSynchronizationService _databaseSynchronizationService;
        public DeviceManagementNavigationViewModel DeviceManagementNavigation { get; set; }
        public ProjectManagementNavigationViewModel ProjectManagementNavigation { get; set; }
        public BorrowReturnNavigationViewModel BorrowReturnNavigation { get; set; }
        public CreateNewLendRequestViewModel CreateNewLendRequest { get; set; }
        public MaintenanceNavigationViewModel MaintenanceNavigation { get; set; }
        public ICommand LoadDataStoreCommand { get; set; }

        public MainViewModel(IDatabaseSynchronizationService databaseSynchronizationService, DeviceManagementNavigationViewModel deviceManagementNavigation, ProjectManagementNavigationViewModel projectManagementNavigation, BorrowReturnNavigationViewModel borrowReturnNavigation, CreateNewLendRequestViewModel createNewLendRequest, MaintenanceNavigationViewModel maintenanceNavigation)
        {
            _databaseSynchronizationService = databaseSynchronizationService;
            DeviceManagementNavigation = deviceManagementNavigation;
            ProjectManagementNavigation = projectManagementNavigation;    
            BorrowReturnNavigation = borrowReturnNavigation;
            CreateNewLendRequest = createNewLendRequest;
            MaintenanceNavigation = maintenanceNavigation;
            LoadDataStoreCommand = new RelayCommand(LoadDataStoreAsync);
        }

        private async void LoadDataStoreAsync()
        {
            await Task.WhenAll(
                _databaseSynchronizationService.SynchronizeLocationsData(),
                _databaseSynchronizationService.SynchronizeEquipmentsData(),
                _databaseSynchronizationService.SynchronizeEquipmentTypesData(),
                _databaseSynchronizationService.SynchronizeSuppliersData(),
                _databaseSynchronizationService.SynchronizeTagsData(),
                _databaseSynchronizationService.SynchronizeProjectsData()
                );
        }
    }
}
