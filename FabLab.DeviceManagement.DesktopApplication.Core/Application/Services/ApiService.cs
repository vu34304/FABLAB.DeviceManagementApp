﻿using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Dtos;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Dtos.Borrowings;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Dtos.Equipments;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Dtos.EquipmentTypes;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Dtos.Locations;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Dtos.Projects;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Dtos.Returnings;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Dtos.Suppliers;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Dtos.Tags;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Exceptions;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Models.Equipments;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Models.EquipmentTypes;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Services;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace FabLab.DeviceManagement.DesktopApplication.Core.Application.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private const string serverUrl = "https://fablabequipment.azurewebsites.net/";

        public ApiService()
        {
            _httpClient = new HttpClient();
        }

        //Equipment
        public async Task<IEnumerable<EquipmentDto>> GetAllEquipmentsAsync()
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{serverUrl}/api/Equipment/Search1");

            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            var equipments = JsonConvert.DeserializeObject<IEnumerable<EquipmentDto>>(responseBody);
            if (equipments is null)
            {
                return new List<EquipmentDto>();
            }
            return equipments;
        }

        public async Task<IEnumerable<EquipmentDto>> GetEquipmentsRecordsAsync(string equipmentId, string equipmentName, string yearOfSupply, string equipmentTypeName, ECategory? category, EStatus? status)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{serverUrl}/api/Equipment/Search1?&equipmentName={equipmentName}&YearOfSupply={yearOfSupply}&equipmentTypeName={equipmentTypeName}&Category={category}&equipmentStatus={status}&pageSize=1000&pageNumber=1");

            response.EnsureSuccessStatusCode(); //equipmentId={equipmentId//
            string responseBody = await response.Content.ReadAsStringAsync();

            var equipments = JsonConvert.DeserializeObject<IEnumerable<EquipmentDto>>(responseBody);
            if (equipments is null)
            {
                return new List<EquipmentDto>();
            }
            return equipments;
        }


        public async Task<IEnumerable<EquipmentDto>> GetEquipmentsRecordsAsync(string yearSelected, string equipmentId, string equipmentTypeId, ECategory? category)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{serverUrl}/api/Equipment/Search1?equipmentId={equipmentId}&YearOfSupply={yearSelected}&equipmentTypeId={equipmentTypeId}&Category={category}");

            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            var equipments = JsonConvert.DeserializeObject<IEnumerable<EquipmentDto>>(responseBody);
            if (equipments is null)
            {
                return new List<EquipmentDto>();
            }
            return equipments;
        }

        public async Task<IEnumerable<EquipmentDto>> GetEquipmentsRecordsAsync(string equipmentName)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{serverUrl}/api/Equipment/Search1?equipmentName={equipmentName}&pageSize=1000&pageNumber=1");

            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            var equipments = JsonConvert.DeserializeObject<IEnumerable<EquipmentDto>>(responseBody);
            if (equipments is null)
            {
                return new List<EquipmentDto>();
            }
            return equipments;
        }

        public async Task CreateEquipment(CreateEquipmentDto equipment)
        {
            var json = JsonConvert.SerializeObject(equipment);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync($"{serverUrl}/api/Equipment", content);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var error = JsonConvert.DeserializeObject<ServerSideError>(responseBody);
                    if (error is not null)
                    {
                        switch (error.Code)
                        {
                            case "Domain.EntityDuplication":
                                throw new DuplicateEntityException();
                        }
                    }
                    else
                    {
                        throw ex;
                    }
                }
                else
                {
                    throw ex;
                }
            }
        }

        public async Task FixEquipmentAsync(FixEquipmentDto fixDto)
        {
            var json = JsonConvert.SerializeObject(fixDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync($"{serverUrl}/api/Equipment", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteEquipmentAsync(string equipmentId)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{serverUrl}/api/Equipment?name={equipmentId}");

            response.EnsureSuccessStatusCode();
        }

        //Location

        public async Task<IEnumerable<LocationDto>> GetAllLocationsAsync()
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{serverUrl}/api/Location");

            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            var responses = JsonConvert.DeserializeObject<IEnumerable<LocationDto>>(responseBody);
            if (responses is null)
            {
                return new List<LocationDto>();
            }
            return responses;
        }

        public async Task CreateLocation(LocationDto location)
        {
            var json = JsonConvert.SerializeObject(location);
            var jsonCamelCase = JsonNamingPolicy.CamelCase.ConvertName(json);

            var content = new StringContent(jsonCamelCase, Encoding.UTF8, "application/json");


            HttpResponseMessage response = await _httpClient.PostAsync($"{serverUrl}/api/Location", content);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var error = JsonConvert.DeserializeObject<ServerSideError>(responseBody);
                    if (error is not null)
                    {
                        switch (error.Code)
                        {
                            case "Domain.EntityDuplication":
                                throw new DuplicateEntityException();
                        }
                    }
                    else
                    {
                        throw ex;
                    }
                }
                else
                {
                    throw ex;
                }
            }
        }
        public async Task FixLocationAsync(LocationDto fixDto)
        {
            var json = JsonConvert.SerializeObject(fixDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync($"{serverUrl}/api/Location", content);
            response.EnsureSuccessStatusCode();
        }
        public async Task DeleteLocationAsync(string locationId)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{serverUrl}/api/Location?name={locationId}");

            response.EnsureSuccessStatusCode();
        }
        //Supplier

        public async Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync()
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{serverUrl}/api/Supplier");

            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            var responses = JsonConvert.DeserializeObject<IEnumerable<SupplierDto>>(responseBody);
            if (responses is null)
            {
                return new List<SupplierDto>();
            }
            return responses;
        }

        public async Task CreateSupplier(SupplierDto supplier)
        {
            var json = JsonConvert.SerializeObject(supplier);
            var jsonCamelCase = JsonNamingPolicy.CamelCase.ConvertName(json);

            var content = new StringContent(jsonCamelCase, Encoding.UTF8, "application/json");


            HttpResponseMessage response = await _httpClient.PostAsync($"{serverUrl}/api/Supplier", content);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var error = JsonConvert.DeserializeObject<ServerSideError>(responseBody);
                    if (error is not null)
                    {
                        switch (error.Code)
                        {
                            case "Domain.EntityDuplication":
                                throw new DuplicateEntityException();
                        }
                    }
                    else
                    {
                        throw ex;
                    }
                }
                else
                {
                    throw ex;
                }
            }
        }
        public async Task FixSupplierAsync(SupplierDto fixDto)
        {
            var json = JsonConvert.SerializeObject(fixDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync($"{serverUrl}/api/Supplier", content);
            response.EnsureSuccessStatusCode();
        }
        public async Task DeleteSupplierAsync(string supplierName)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{serverUrl}/api/Supplier?name={supplierName}");

            response.EnsureSuccessStatusCode();
        }

        //EquipmentType
        public async Task<IEnumerable<EquipmentTypeDto>> GetAllEquipmentTypesAsync()
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{serverUrl}/api/EquipmentType");

            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            var responses = JsonConvert.DeserializeObject<IEnumerable<EquipmentTypeDto>>(responseBody);
            if (responses is null)
            {
                return new List<EquipmentTypeDto>();
            }
            return responses;
        }
        public async Task<IEnumerable<EquipmentTypeDto>> GetEquipmentTypesRecordsAsync(string equiqmentTypeId, string equiqmentTypeName, ECategory category)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{serverUrl}/api/EquipmentType");

            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            var responses = JsonConvert.DeserializeObject<IEnumerable<EquipmentTypeDto>>(responseBody);
            if (responses is null)
            {
                return new List<EquipmentTypeDto>();
            }
            return responses;
        }

        public async Task<InformationEquipmentDto> GetInformationEquipmenAsync(string equipmentTypeId)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{serverUrl}/api/EquipmentType/Information?equipmentTypeId={equipmentTypeId}");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            var responses = JsonConvert.DeserializeObject<InformationEquipmentDto>(responseBody);
            if (responses is null)
            {
                //return new List<InformationEquipmentDto>();
            }
#pragma warning disable CS8603 // Possible null reference return.
            return responses;
#pragma warning restore CS8603 // Possible null reference return.
        }
        public async Task CreateEquipmentType(CreateEquimentTypeDto equipmentType)
        {

            string json = JsonConvert.SerializeObject(equipmentType);

            var jsonCamelCase = JsonNamingPolicy.CamelCase.ConvertName(json);

            var content = new StringContent(jsonCamelCase, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync($"{serverUrl}/api/EquipmentType/Detail", content);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var error = JsonConvert.DeserializeObject<ServerSideError>(responseBody);
                    if (error is not null)
                    {
                        switch (error.Code)
                        {
                            case "Domain.EntityDuplication":
                                throw new DuplicateEntityException();
                        }
                    }
                    else
                    {
#pragma warning disable CA2200 // Rethrow to preserve stack details
                        throw ex;
#pragma warning restore CA2200 // Rethrow to preserve stack details
                    }
                }
                else
                {
#pragma warning disable CA2200 // Rethrow to preserve stack details
                    throw ex;
#pragma warning restore CA2200 // Rethrow to preserve stack details
                }
            }
        }
      

        public async Task FixEquipmentTypesAsync(EquipmentTypeDto fixDto)
        {
            var json = JsonConvert.SerializeObject(fixDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync($"{serverUrl}/api/EquipmentType", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteEquipmentTypeAsync(string equipmentTypeId)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{serverUrl}/api/EquipmentType?name={equipmentTypeId}");

            response.EnsureSuccessStatusCode();
        }

        //Tag
        public async Task<IEnumerable<TagDto>> GetAllTagAsync()
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{serverUrl}/api/Tag?pageSize=5&pageNumber=1 ");

            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            var responses = JsonConvert.DeserializeObject<IEnumerable<TagDto>>(responseBody);
            if (responses is null)
            {
                return new List<TagDto>();
            }
            return responses;
        }
        public async Task CreateTagAsync(TagDto tag)
        {
            var json = JsonConvert.SerializeObject(tag);
            var jsonCamelCase = JsonNamingPolicy.CamelCase.ConvertName(json);

            var content = new StringContent(jsonCamelCase, Encoding.UTF8, "application/json");


            HttpResponseMessage response = await _httpClient.PostAsync($"{serverUrl}/api/Tag", content);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var error = JsonConvert.DeserializeObject<ServerSideError>(responseBody);
                    if (error is not null)
                    {
                        switch (error.Code)
                        {
                            case "Domain.EntityDuplication":
                                throw new DuplicateEntityException();
                        }
                    }
                    else
                    {
                        throw ex;
                    }
                }
                else
                {
                    throw ex;
                }
            }
        }
        public async Task FixTagAsync(TagDto fixDto)
        {
            var json = JsonConvert.SerializeObject(fixDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync($"{serverUrl}/api/Tag", content);
            response.EnsureSuccessStatusCode();
        }
        public async Task DeleteTagAsync(string tagId)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{serverUrl}/api/Tag?name={tagId}");

            response.EnsureSuccessStatusCode();
        }

        //Project
        public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{serverUrl}/api/Project/Search?pageSize=022&pageNumber=1");

            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            var responses = JsonConvert.DeserializeObject<IEnumerable<ProjectDto>>(responseBody);
            if (responses is null)
            {
                return new List<ProjectDto>();
            }
            return responses;
        }

        public async Task<IEnumerable<ProjectDto>> GetProjectsAsync(string projectName)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{serverUrl}/api/Project/Search?projectName={projectName}&pageSize=022&pageNumber=1");

            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            var responses = JsonConvert.DeserializeObject<IEnumerable<ProjectDto>>(responseBody);
            if (responses is null)
            {
                return new List<ProjectDto>();
            }
            return responses;
        }
        public async Task CreateProject(CreateProjectDto projectDto)
        {
            var json = JsonConvert.SerializeObject(projectDto);
            var jsonCamelCase = JsonNamingPolicy.CamelCase.ConvertName(json);

            var content = new StringContent(jsonCamelCase, Encoding.UTF8, "application/json");


            HttpResponseMessage response = await _httpClient.PostAsync($"{serverUrl}/api/Project/Init", content);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var error = JsonConvert.DeserializeObject<ServerSideError>(responseBody);
                    if (error is not null)
                    {
                        switch (error.Code)
                        {
                            case "Domain.EntityDuplication":
                                throw new DuplicateEntityException();
                        }
                    }
                    else
                    {
#pragma warning disable CA2200 // Rethrow to preserve stack details
                        throw ex;
#pragma warning restore CA2200 // Rethrow to preserve stack details
                    }
                }
                else
                {
#pragma warning disable CA2200 // Rethrow to preserve stack details
                    throw ex;
#pragma warning restore CA2200 // Rethrow to preserve stack details
                }
            }
        }

        public async Task DeleteProjectAsync(string projectName)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{serverUrl}/api/Project?name={projectName}");

            response.EnsureSuccessStatusCode();
        }

        public async Task ApprovedProjectAsync(ApprovedProjectDto approvedProjectDto)
        {
            var json = JsonConvert.SerializeObject(approvedProjectDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync($"{serverUrl}/api/Project/Approve", content);
            response.EnsureSuccessStatusCode();
        }
        public async Task EndProjectAsync(EndProjectDto endProjectDto)
        {
            var json = JsonConvert.SerializeObject(endProjectDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync($"{serverUrl}/Api/Project/End", content);
            response.EnsureSuccessStatusCode();
        }

        //Borrow
        public async Task<IEnumerable<BorrowEquipmentDto>> GetBorrowEquipmentAsync(string projectName)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{serverUrl}/api/Project/Equipment?projectName={projectName}");

            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            var equipments = JsonConvert.DeserializeObject<IEnumerable<BorrowEquipmentDto>>(responseBody);
            if (equipments is null)
            {
                return new List<BorrowEquipmentDto>();
            }
            return equipments;
        }
        public async Task CreateLendRequestAsync(CreateBorrowDto createBorrowDto)
        {
            string json = JsonConvert.SerializeObject(createBorrowDto);

            var jsonCamelCase = JsonNamingPolicy.CamelCase.ConvertName(json);

            var content = new StringContent(jsonCamelCase, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync($"{serverUrl}/api/Borrow/Borrow", content);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var error = JsonConvert.DeserializeObject<ServerSideError>(responseBody);
                    if (error is not null)
                    {
                        switch (error.Code)
                        {
                            case "Domain.EntityDuplication":
                                throw new DuplicateEntityException();
                        }
                    }
                    else
                    {
#pragma warning disable CA2200 // Rethrow to preserve stack details
                        throw ex;
#pragma warning restore CA2200 // Rethrow to preserve stack details
                    }
                }
                else
                {
#pragma warning disable CA2200 // Rethrow to preserve stack details
                    throw ex;
#pragma warning restore CA2200 // Rethrow to preserve stack details
                }
            }
        }

        public async Task<IEnumerable<BorrowDto>> GetBorrowsAsync(string projectName)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{serverUrl}/api/Borrow/Search?projectName={projectName}&pageSize=11");

            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            var equipments = JsonConvert.DeserializeObject<IEnumerable<BorrowDto>>(responseBody);
            if (equipments is null)
            {
                return new List<BorrowDto>();
            }
            return equipments;
        }

        public async Task ReturnAsync(ReturnDto returnDto)
        {
            var json = JsonConvert.SerializeObject(returnDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync($"{serverUrl}/api/Borrow/Return?", content);
            response.EnsureSuccessStatusCode();

        }

    }
}
