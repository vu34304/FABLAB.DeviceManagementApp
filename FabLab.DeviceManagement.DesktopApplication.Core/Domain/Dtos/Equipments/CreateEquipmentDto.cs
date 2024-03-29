﻿using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Models.Equipments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FabLab.DeviceManagement.DesktopApplication.Core.Domain.Dtos.Equipments
{
    public class CreateEquipmentDto
    {
        public string equipmentId { get; set; }
        public string equipmentName { get; set; }
        public DateTime yearOfSupply { get; set; }
        public string codeOfManager { get; set; }
        public EStatus status { get; set; }
        public string locationId { get; set; }
        public string supplierName { get; set; }
        public string equipmentTypeId { get; set; }
        public CreateEquipmentDto(string equipmentId, string equipmentName, DateTime yearOfSupply, string codeOfManager, EStatus status, string locationId, string supplierName, string? equipmentTypeId)
        {
            this.equipmentId = equipmentId;
            this.equipmentName = equipmentName;
            this.yearOfSupply = yearOfSupply;
            this.codeOfManager = codeOfManager;
            this.status = status;
            this.locationId = locationId;
            this.supplierName = supplierName;
#pragma warning disable CS8601 // Possible null reference assignment.
            this.equipmentTypeId = equipmentTypeId;
#pragma warning restore CS8601 // Possible null reference assignment.
        }
    }
}
