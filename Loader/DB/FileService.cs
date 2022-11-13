using System;
using PlatformTM.Core.Domain.Model.DatasetModel;

namespace Loader.DB
{
    public class FileService
    {

        private int projectId { get; set; }

        public FileService(int pid)
        {
            projectId = pid;
        }

        public DataFile? AddOrUpdateFile(string studyFolderName, string subFolderName, FileInfo fileInfo)
        {
           

            if (fileInfo == null)
                return null;
            DataFile file;
            //var filePath = fi.DirectoryName.Substring(fi.DirectoryName.IndexOf("P-"+projectId));
            //var file = _fileRepository.FindSingle(d => d.FileName.Equals(fi.Name) && d.Path.Equals(filePath) && d.ProjectId == projectId);

            int? fileFolderId = GetFolder(studyFolderName, subFolderName);

            if (fileFolderId == null)
                throw new Exception("Failure in creating folder info");

            using var dBcontext = new FilesDBcontext();

            file = dBcontext.Files.Where(d => d.FileName == fileInfo.Name && d.ProjectId == projectId && d.FolderId == fileFolderId).FirstOrDefault();

            if (file == null)
            {
                file = new DataFile
                {
                    FileName = fileInfo.Name, 
                    Created = fileInfo.CreationTime.ToString("d") + " " + fileInfo.CreationTime.ToString("t"),
                    State = "NEW",
                    //Path = fi.DirectoryName.Substring(fi.DirectoryName.IndexOf("P-" + projectId)),
                    IsDirectory = false,
                    ProjectId = projectId,
                    FolderId = fileFolderId
                };
                dBcontext.Files.Add(file);
            }
            else
            {
                file.Modified = fileInfo.LastWriteTime.ToString("d") + " " + fileInfo.LastWriteTime.ToString("t");
                file.State = "UPDATED";
                dBcontext.Files.Update(file);
            }


            dBcontext.SaveChanges();
            return file;
        }

        internal int? GetFolder(string studyFolderName, string subFolderName)
        {
            using var dBcontext = new FilesDBcontext();
            DataFile pFolder;

            if (studyFolderName == null || subFolderName == "") return null;


            pFolder = dBcontext.Files.Where(f => f.FileName == studyFolderName && f.ProjectId == projectId).FirstOrDefault();

            if (pFolder == null)
            {

                //Create a study folder (e.g. BT, MAAS...etc)
                var folder = new DataFile()
                {
                    FileName = studyFolderName,
                    IsDirectory = true,
                    ProjectId = projectId,
                    Created = DateTime.Now.ToString("D")
                };

                dBcontext.Files.Add(folder);
                dBcontext.SaveChanges();
                pFolder = dBcontext.Files.Where(f => f.FileName == studyFolderName).FirstOrDefault();
            }


            DataFile subFolder = dBcontext.Files.Where(f => f.FileName == subFolderName && f.ProjectId == projectId).FirstOrDefault();

            if (subFolder == null)
            {
                //Create a study folder (e.g. BT, MAAS...etc)
                var folder = new DataFile()
                {
                    FileName = subFolderName,
                    IsDirectory = true,
                    ProjectId = projectId,
                    Created = DateTime.Now.ToString("D"),
                    FolderId = pFolder.Id
                };

                dBcontext.Files.Add(folder);
                dBcontext.SaveChanges();
                subFolder = dBcontext.Files.Where(f => f.FileName == subFolderName && f.ProjectId == projectId).FirstOrDefault();
            }

            return subFolder.Id;
            //var project = dBcontext.Projects.Where(p => p.Id == projectId).FirstOrDefault();
            //if (project == null || parentFolder == "" || parentFolder == null)
            //throw new Exception("Project Not Found");

        
        
        }
    }
}

