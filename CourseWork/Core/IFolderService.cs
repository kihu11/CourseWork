namespace Course.FileManager.Core;

public interface IFolderService
{
    void CreateFolder(string path);
    void DeleteFolder(string path);
    void RenameFolder(string path, string newName);
    void CopyFolder(string source, string destination);

    string[] GetFiles(string path);
    string[] GetDirectories(string path);
    FolderDetails GetFolderDetails(string path);
    
}