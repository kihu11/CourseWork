using Course.FileManager.Core;

namespace Course.FileManager.Infrastructure;

public class FileService : IFileService
{
    public void CreateFile(string path, string content)
    {
        File.WriteAllText(path, content);
    }

    public void WriteFile(string path, string content)
    {
        File.WriteAllText(path, content);
    }

    public string ReadFile(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException();

        return File.ReadAllText(path);
    }

    public void DeleteFile(string path)
    {
        if (File.Exists(path))
            File.Delete(path);
    }

    public void CopyFile(string source, string destination)
    {
        File.Copy(source, destination, true);
    }

    public void MoveFile(string source, string destination)
    {
        File.Move(source, destination, true);
    }

    public void RenameFile(string path, string newName)
    {
        string dir = Path.GetDirectoryName(path)!;
        string newPath = Path.Combine(dir, newName);
        File.Move(path, newPath);
    }

    public FileInfo GetFileInfo(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException();

        return new FileInfo(path);
    }
    
    
}