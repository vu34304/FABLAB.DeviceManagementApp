using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FabLab.DeviceManagement.DesktopApplication.Core.Domain.Dtos.Projects
{
    public class CreateProjectDto
    {
        public string ProjectName { get; set; }
        public DateTime StartDay { get; set; }
        public DateTime EndDay { get; set; }
        public string Description { get; set; }
        public List<string> Equipments { get; set; }

        public CreateProjectDto(string projectName, DateTime startDay, DateTime endDay, string description, List<string> equipments)
        {
            ProjectName = projectName;
            StartDay = startDay;
            EndDay = endDay;
            Description = description;
            Equipments = equipments;
        }
    }
}
