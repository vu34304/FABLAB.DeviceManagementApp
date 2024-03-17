using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FabLab.DeviceManagement.DesktopApplication.Core.Application.ViewModels.Project
{
    public class ProjectManagementNavigationViewModel
    {
        public CreateNewProjectViewModel CreateNewProject {  get; set; }
        public ProjectManagementViewModel ProjectManagement { get; set; }

        public ProjectManagementNavigationViewModel(CreateNewProjectViewModel createNewProject, ProjectManagementViewModel projectManagement)
        {
            CreateNewProject = createNewProject;
            ProjectManagement = projectManagement;
        }
    }
}
