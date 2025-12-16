using System;
using System.IO;
using Course.FileManager.Core;

namespace Course.FileManager.Infrastructure
{
    public class FolderService : IFolderService
    {
        public void CreateFolder(string path)
        {
            Directory.CreateDirectory(path);
        }

        public void DeleteFolder(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }

        public void RenameFolder(string path, string newName)
        {
            string parent = Path.GetDirectoryName(path)!;
            string newPath = Path.Combine(parent, newName);
            Directory.Move(path, newPath);
        }

        public void CopyFolder(string source, string destination)
        {
            Directory.CreateDirectory(destination);

            foreach (var file in Directory.GetFiles(source))
                File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), true);

            foreach (var dir in Directory.GetDirectories(source))
                CopyFolder(dir, Path.Combine(destination, Path.GetFileName(dir)));
        }

        public string[] GetFiles(string path)
        {
            return Directory.Exists(path) ? Directory.GetFiles(path) : Array.Empty<string>();
        }

        public string[] GetDirectories(string path)
        {
            return Directory.Exists(path) ? Directory.GetDirectories(path) : Array.Empty<string>();
        }

        public DirectoryInfo GetFolderInfo(string path)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException();

            return new DirectoryInfo(path);
        }
        

        public FolderDetails GetFolderDetails(string path)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException();

            var dirInfo = new DirectoryInfo(path);
            long size = 0;

            try
            {
                foreach (var file in dirInfo.GetFiles("*", SearchOption.AllDirectories))
                    size += file.Length;
            }
            catch (UnauthorizedAccessException)
            {
            }

            return new FolderDetails
            {
                Info = dirInfo,
                Size = size
            };
        }
    }
}
