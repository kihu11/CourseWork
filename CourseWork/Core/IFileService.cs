namespace Course.FileManager.Core;

public interface IFileService
{
    void CreateFile(string path, string content);
    void WriteFile(string path, string content);
    string ReadFile(string path);

    void DeleteFile(string path);
    void CopyFile(string source, string destination);
    void MoveFile(string source, string destination);
    void RenameFile(string path, string newName);
    FileInfo GetFileInfo(string path);
}
