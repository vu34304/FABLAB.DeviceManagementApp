﻿using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Models.Equipments;
using FabLab.DeviceManagement.DesktopApplication.Core.Domain.Models.EquipmentTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FabLab.DeviceManagement.DesktopApplication.Core.Domain.Dtos.EquipmentTypes
{
    public class CreateEquimentTypeDto
    {
        public string equipmentTypeId { get; set; }
        public string equipmentTypeName { get; set; }
        public string description { get; set; }
        public ECategory category { get; set; }
        public List<string> tags { get; set; }
        public ObservableCollection<FileDataEquimentType> pictures { get; set; }
        public ObservableCollection<SpecificationEquimentType> specifications { get; set; }

        public CreateEquimentTypeDto( string equipmentTypeId, string equipmentTypeName, string description, ECategory category, List<string> tags, ObservableCollection<FileDataEquimentType> pictures, ObservableCollection<SpecificationEquimentType> specifications) 
        { 
            this.equipmentTypeId = equipmentTypeId;
            this.equipmentTypeName = equipmentTypeName;
            this.description = description;
            this.category = category;
            this.tags = tags;
            this.pictures = pictures;
            this.specifications = specifications;

        }
    }
}
