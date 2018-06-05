using System;
using System.Collections.Generic;
using PlatformTM.Services.DTOs;

namespace PlatformTM.Services.ViewModels
{
    public class DriveVM
    {
        public List<FileDTO> Files { get; set; }
        public List<FileDTO> folders { get; set; }
        public DriveVM()
        {
            Files = new List<FileDTO>();
            folders = new List<FileDTO>();
        }
    }
}
