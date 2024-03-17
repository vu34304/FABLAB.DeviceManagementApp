﻿using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Dtos.Borrowings;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Dtos.Equipments;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Dtos.EquipmentTypes;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Dtos.Locations;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Dtos.Projects;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Dtos.Returnings;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Dtos.Suppliers;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Dtos.Tags;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Models.Equipments;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Models.EquipmentTypes;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FabLab.DeviceManagement.DesktopApplication.Core.Domain.Services
{
    public interface IApiService
    {
        //Equipment
        Task<IEnumerable<EquipmentDto>> GetAllEquipmentsAsync();
        Task<IEnumerable<EquipmentDto>> GetEquipmentsRecordsAsync(string yearSelected, string equipmentId, string equipmentTypeId, ECategory? category);
        Task<IEnumerable<EquipmentDto>> GetEquipmentsRecordsAsync(string equipmentId, string equipmentName, string yearOfSupply, string equipmentTypeName, ECategory? category, EStatus? status);
        Task<IEnumerable<EquipmentDto>> GetEquipmentsRecordsAsync(string equimentName);
        Task CreateEquipment(CreateEquipmentDto equipment);
        Task FixEquipmentAsync(FixEquipmentDto fixDto);
        Task DeleteEquipmentAsync(string equipmentId);

        //Location
        Task<IEnumerable<LocationDto>> GetAllLocationsAsync();
        Task CreateLocation(LocationDto location);
        Task FixLocationAsync(LocationDto fixDto);
        Task DeleteLocationAsync(string locationId);
        //Supplier
        Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync();
        Task CreateSupplier(SupplierDto supplier);
        Task FixSupplierAsync(SupplierDto fixDto);
        Task DeleteSupplierAsync(string supplierName);
        //Tag
        Task<IEnumerable<TagDto>> GetAllTagAsync();
        Task CreateTagAsync(TagDto tag);
        Task FixTagAsync(TagDto fixDto);
        Task DeleteTagAsync(string tagId);

        //EquipmentType
        Task<IEnumerable<EquipmentTypeDto>> GetAllEquipmentTypesAsync();
        Task<IEnumerable<EquipmentTypeDto>> GetEquipmentTypesRecordsAsync(string equiqmentTypeId, string equiqmentTypeName, ECategory category);
        Task<InformationEquipmentDto> GetInformationEquipmenAsync(string equiqmentTypeId);
        Task CreateEquipmentType(CreateEquimentTypeDto equipmentType);
        Task FixEquipmentTypesAsync(EquipmentTypeDto fixDto);
        Task DeleteEquipmentTypeAsync(string equipmentTypeId);

        //Project
        Task<IEnumerable<ProjectDto>> GetAllProjectsAsync();
        Task<IEnumerable<ProjectDto>> GetProjectsAsync(string projectName);
        Task CreateProject(CreateProjectDto projectDto);
        Task DeleteProjectAsync(string projectName);
        Task ApprovedProjectAsync(ApprovedProjectDto approvedProjectDto);
        Task EndProjectAsync(EndProjectDto endProjectDto);

        //Borrow
        Task<IEnumerable<BorrowEquipmentDto>> GetBorrowEquipmentAsync(string projectName);
        Task CreateLendRequestAsync(CreateBorrowDto createBorrowDto);
        Task<IEnumerable<BorrowDto>> GetBorrowsAsync(string projectName);
        Task ReturnAsync(ReturnDto returnDto);


    }
}
