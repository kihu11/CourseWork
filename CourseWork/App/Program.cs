using Course.FileManager.Core;
using Course.FileManager.Infrastructure;
using System;
using System.Diagnostics;
using System.IO;

namespace Course
{
    public class Program
    {
        private static string currentDir = Directory.GetCurrentDirectory();

        private static void Main()
        {
            IFileService fileService = new FileService();
            IFolderService folderService = new FolderService();

            while (true)
            {
                Console.Write($"{currentDir}> ");
                string input = Console.ReadLine()!;
                if (string.IsNullOrWhiteSpace(input)) continue;

                string[] parts = input.Split(' ', 2);
                string command = parts[0].ToLower();
                string args = parts.Length > 1 ? parts[1] : "";

                try
                {
                    switch (command)
                    {
                        case "help":
                            ShowHelp();
                            break;
                        case "cd":
                            ChangeDirectory(args);
                            break;
                        case "dir":
                            ListDirectory(folderService);
                            break;
                        case "mkdir":
                        case "md":
                            folderService.CreateFolder(Path.Combine(currentDir, args));
                            break;
                        case "rmdir":
                        case "rd":
                            folderService.DeleteFolder(Path.Combine(currentDir, args));
                            break;
                        case "createfile":
                            fileService.CreateFile(Path.Combine(currentDir, args), "");
                            break;
                        case "write":
                            WriteFile(fileService, args);
                            break;
                        case "type":
                            TypeFile(fileService, args);
                            break;
                        case "del":
                            fileService.DeleteFile(Path.Combine(currentDir, args));
                            break;
                        case "copy":
                            CopyFile(fileService, args);
                            break;
                        case "copydir":
                            CopyDir(folderService, args);
                            break;
                        case "move":
                            MoveFile(fileService, args);
                            break;
                        case "ren":
                            RenameFile(fileService, args);
                            break;
                        case "renamedir":
                            RenameDir(folderService, args);
                            break;
                        case "info":
                            ShowInfo(fileService, folderService, args);
                            break;
                        case "run":
                            RunFile(args);
                            break;
                        case "exit":
                            return;
                        default:
                            Console.WriteLine("Команда не найдена. Введите 'help' для списка команд.");
                            break;
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("Ошибка: Отказано в доступе");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("""
                              Доступные команды:


                              help                  - показать список команд
                              dir                   - показать содержимое текущего каталога
                              cd <path>             - перейти в другой каталог
                              md | mkdir <name>     - создать каталог
                              rd | rmdir <name>     - удалить каталог
                              createfile <name>     - создать пустой файл
                              write <file>          - записать текст в файл
                              type <file>           - вывести содержимое файла (текст или часть бинарного)
                              del <file>            - удалить файл
                              copy <src> <dst>      - копировать файл
                              copydir <src> <dst>   - копировать каталог
                              move <src> <dst>      - переместить файл
                              ren <old> <new>       - переименовать файл
                              renamedir <old> <new> - переименовать каталог
                              info <name>           - информация о файле или каталоге
                              run <file>            - запустить .exe файл
                              exit                  - выход из программы
                              """);
        }

        private static void ChangeDirectory(string path)
        {
            string newDir = Path.IsPathRooted(path) ? path : Path.Combine(currentDir, path);
            if (Directory.Exists(newDir))
                currentDir = Path.GetFullPath(newDir);
            else
                Console.WriteLine("Путь не найден");
        }

        private static void ListDirectory(IFolderService folderService)
        {
            foreach (var dir in folderService.GetDirectories(currentDir))
                Console.WriteLine($"<DIR> {Path.GetFileName(dir)}");

            foreach (var file in folderService.GetFiles(currentDir))
                Console.WriteLine(Path.GetFileName(file));
        }

        private static void WriteFile(IFileService fileService, string fileName)
        {
            Console.WriteLine("Введите текст (пустая строка — конец):");
            string line;
            string content = "";

            while (!string.IsNullOrEmpty(line = Console.ReadLine()!))
                content += line + Environment.NewLine;

            fileService.WriteFile(Path.Combine(currentDir, fileName), content);
        }

        private static void TypeFile(IFileService fileService, string fileName)
        {
            string path = Path.Combine(currentDir, fileName);
            if (!File.Exists(path))
            {
                Console.WriteLine("Файл не найден");
                return;
            }

            try
            {
                if (Path.GetExtension(path).ToLower() == ".txt")
                {
                    Console.WriteLine(fileService.ReadFile(path));
                }
                else
                {
                    byte[] bytes = File.ReadAllBytes(path);
                    Console.WriteLine($"Файл {fileName} содержит {bytes.Length} байт");
                    for (int i = 0; i < Math.Min(16, bytes.Length); i++)
                        Console.Write($"{bytes[i]:X2} ");
                    Console.WriteLine();
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Ошибка: Отказано в доступе");
            }
        }

        private static void CopyFile(IFileService fileService, string args)
        {
            var p = args.Split(' ');
            fileService.CopyFile(Path.Combine(currentDir, p[0]), Path.Combine(currentDir, p[1]));
        }

        private static void CopyDir(IFolderService folderService, string args)
        {
            var p = args.Split(' ');
            folderService.CopyFolder(Path.Combine(currentDir, p[0]), Path.Combine(currentDir, p[1]));
        }


        private static void MoveFile(IFileService fileService, string args)
        {
            var p = args.Split(' ');
            fileService.MoveFile(Path.Combine(currentDir, p[0]), Path.Combine(currentDir, p[1]));
        }

        private static void RenameFile(IFileService fileService, string args)
        {
            var p = args.Split(' ');
            fileService.RenameFile(Path.Combine(currentDir, p[0]), p[1]);
        }

        private static void RenameDir(IFolderService folderService, string args)
        {
            var p = args.Split(' ');
            folderService.RenameFolder(Path.Combine(currentDir, p[0]), p[1]);
        }

        private static void ShowInfo(IFileService fileService, IFolderService folderService, string name)
        {
            string path = Path.Combine(currentDir, name);

            if (File.Exists(path))
            {
                var f = fileService.GetFileInfo(path);
                Console.WriteLine($"Файл: {f.Name}");
                Console.WriteLine($"Размер: {f.Length} байт");
                Console.WriteLine($"Создан: {f.CreationTime}");
                Console.WriteLine($"Изменён: {f.LastWriteTime}");
                Console.WriteLine($"Атрибуты: {f.Attributes}");
            }
            else if (Directory.Exists(path))
            {
                var folderDetails = folderService.GetFolderDetails(path);
                var d = folderDetails.Info;

                Console.WriteLine($"Каталог: {d.Name}");
                Console.WriteLine($"Размер: {folderDetails.Size} байт");
                Console.WriteLine($"Создан: {d.CreationTime}");
                Console.WriteLine($"Изменён: {d.LastWriteTime}");
                Console.WriteLine($"Атрибуты: {d.Attributes}");
            }
            else
            {
                Console.WriteLine("Объект не найден");
            }
        }
        private static void RunFile(string fileName)
        {
            string path = Path.Combine(currentDir, fileName);
            if (!File.Exists(path))
            {
                Console.WriteLine("Файл не найден");
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true
                });
                Console.WriteLine($"Файл {fileName} запущен");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка запуска: {ex.Message}");
            }
        }
    }
}

//добавить считчик файлов
//taskkill,tasklist,fc
