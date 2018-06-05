using System;
using System.Collections.Generic;
using PlatformTM.Services.DTOs;

namespace PlatformTM.Services.ViewModels
{
    public class FileContentVM
    {
        public List<FileDTO> Folders { get; set; }

        public DataTable Data { get; set; }
        public FileDTO File { get; set; }
    }
}
