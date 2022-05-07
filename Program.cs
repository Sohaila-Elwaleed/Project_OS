using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    class Program
    {
        public static Directory current;
        public static string path;
        static void Main(string[] args)
        {
        start://علشان الشاشه تفضل شغاله
            Directory root = new Directory("S:", 0x10, 5, null);
            current = root;
            path = current.Dir_Name;
            Console.Write(path + ">>");
            List<string> list = new List<string>();
            list = DealWithInput();
            Command(list);
            goto start;
        }
        static List<string> DealWithInput()
        {
            string cmd = Console.ReadLine();//باخد الامر من المستخدم
            string[] part;
            List<string> list = new List<string>();
            part = cmd.Split(' ');
            foreach (string s in part)
            {
                if (!s.Equals(""))
                {
                    list.Add(s);
                }
            }
            return list;
        }
        static void Command(List<string> ls)
        {
            ls[0] = ls[0].ToLower();
            if (ls[0] == "help" && ls.Count() == 1)
            {
                Console.WriteLine("cd - Change the current default directory to . If the argument is not present, report the current directory. If the directory does not exist an appropriate error should be reported.");
                Console.WriteLine("cls - Clear the screen.");
                Console.WriteLine("dir - List the contents of directory.");
                Console.WriteLine("quit - Quit the shell.");
                Console.WriteLine("copy - Copies one or more files to another location");
                Console.WriteLine("del - Deletes one or more files.");
                Console.WriteLine("help - Provides Help information for commands.");
                Console.WriteLine("md - Creates a directory.");
                Console.WriteLine("rd - Removes a directory.");
                Console.WriteLine("rename - Renames a file.");
                Console.WriteLine("type - Displays the contents of a text file.");
                Console.WriteLine("import – import text file(s) from your computer");
                Console.WriteLine("export – export text file(s) to your computer");
            }
            else if (ls[0] == "cd" && ls.Count() == 1)
            {
                Console.WriteLine(Program.path);
            }
            else if (ls[0] == "dir" && ls.Count() == 1)
            {
                Directory root = new Directory("S:", 0x10, 5, null);//how?? compare with dirname or what?
                int countFile = 0, countDir = 0, sumFileSizes = 0;
                if (current.Dir_Name == root.Dir_Name)//compare by dirname
                {
                    for (int i = 0; i < current.DirsOrFiles.Count(); i++)
                    {
                        if (current.DirsOrFiles[i].Dir_Attribute == 0x10)
                        {//empty or full
                            Console.WriteLine($"<DIR>\t{current.DirsOrFiles[i].Dir_Name}");
                            countDir++;
                        }
                        else if (current.DirsOrFiles[i].Dir_Attribute == 0x0)
                        {
                            Console.WriteLine($"{current.DirsOrFiles[i].Dir_FileSize}\t{current.DirsOrFiles[i].Dir_Name}");
                            countFile++;
                            sumFileSizes += current.DirsOrFiles[i].Dir_FileSize;
                        }
                    }
                    Console.WriteLine($"Number of files {countFile}\ttheir size {sumFileSizes}");
                    Console.WriteLine($"Number of dirs {countDir}\tFree space{Virtual_Disk.Get_FreeSize() * 1024}");//Is this right??
                }
                else
                {
                    Console.WriteLine("<DIR>\t.");
                    Console.WriteLine("<DIR>\t..");
                    for (int i = 0; i < current.DirsOrFiles.Count(); i++)
                    {
                        if (current.DirsOrFiles[i].Dir_Attribute == 0x10)
                        {//empty or full
                            Console.WriteLine($"<DIR>\t{current.DirsOrFiles[i].Dir_Name}");
                            countDir++;
                        }
                        else if (current.DirsOrFiles[i].Dir_Attribute == 0x0)
                        {
                            Console.WriteLine($"{current.DirsOrFiles[i].Dir_FileSize}\t{current.DirsOrFiles[i].Dir_Name}");
                            countFile++;
                            sumFileSizes += current.DirsOrFiles[i].Dir_FileSize;
                        }
                    }
                    Console.WriteLine($"Number of files {countFile}\ttheir size {sumFileSizes}");
                    Console.WriteLine($"Number of dirs {countDir + 2}\tFree space{Virtual_Disk.Get_FreeSize() * 1024}");//Is this right??
                }
            }
            else if (ls.Count() > 1)//check if it more than 2
            {
                if (ls[0] == "help")
                {
                    switch (ls[1])
                    {
                        case "cd":
                            Console.WriteLine("cd - Change the current default directory to . If the argument is not present, report the current directory. If the directory does not exist an appropriate error should be reported.");
                            break;

                        case "cls":
                            Console.WriteLine("cls - Clear the screen.");
                            break;
                        case "dir":
                            Console.WriteLine("dir - List the contents of directory.");
                            break;
                        case "quit":
                            Console.WriteLine("quit - Quit the shell.");
                            break;
                        case "copy":
                            Console.WriteLine("copy - Copies one or more files to another location");
                            break;
                        case "del":
                            Console.WriteLine("del - Deletes one or more files.");
                            break;
                        case "help":
                            Console.WriteLine("help - Provides Help information for commands.");
                            break;
                        case "md":
                            Console.WriteLine("md - Creates a directory.");
                            break;
                        case "rd":
                            Console.WriteLine("rd - Removes a directory.");
                            break;
                        case "rename":
                            Console.WriteLine("rename - Renames a file.");
                            break;
                        case "type":
                            Console.WriteLine("type - Displays the contents of a text file.");
                            break;
                        case "import":
                            Console.WriteLine("import – import text file(s) from your computer");
                            break;
                        case "export":
                            Console.WriteLine("export – export text file(s) to your computer");
                            break;
                        default:
                            Console.WriteLine($"error :{ls[1]} is not a command");
                            break;
                    }
                }
                else if (ls[0] == "cd")
                {
                    List<string> pathList = new List<string>();
                    string[] part;
                    part = ls[1].Split('\\');
                    foreach (string s in part)
                    {
                        pathList.Add(s);
                    }
                    if (pathList.Count == 1)
                    {
                        int index = current.SearchDirOrFiles(pathList[0]);
                        if (index != -1)
                        {
                            current = new Directory(current.DirsOrFiles[index].Dir_Name.ToString(), current.DirsOrFiles[index].Dir_Attribute, current.DirsOrFiles[index].Dir_FirstCluster, current);
                            path = path + '\\' + pathList[0];
                        }
                        else
                        {
                            Console.WriteLine($"error : {pathList[0]} No such a directory");
                        }
                    }
                    else if (pathList.Count > 1)
                    {
                        Directory directory = new Directory();
                        bool found = MoveToDir(ls[1], ref directory);
                        if (found)
                        {
                            current = directory;
                            path = ls[1];
                        }
                        else
                        {
                            Console.WriteLine("error : No such a directory");
                        }
                    }

                }
                else if (ls[0] == "dir")
                {
                    List<string> pathList = new List<string>();
                    string[] part;
                    part = ls[1].Split('\\');
                    foreach (string s in part)
                    {
                        pathList.Add(s);
                    }
                    if (pathList.Count == 1)
                    {
                        Directory directory;
                        int index = current.SearchDirOrFiles(pathList[0]);
                        if (index != -1 && current.DirsOrFiles[index].Dir_Attribute == 0x10)
                        {
                            directory = new Directory(current.DirsOrFiles[index].Dir_Name.ToString(), 0x10, current.DirsOrFiles[index].Dir_FirstCluster, current);
                            directory.Read_Dir();
                            int countFile = 0, countDir = 0, sumFileSizes = 0;
                            Console.WriteLine($"<DIR>\t{directory.Dir_Name}");
                            Console.WriteLine($"<DIR>\t{directory.Parent.Dir_Name}");
                            for (int i = 0; i < directory.DirsOrFiles.Count(); i++)
                            {
                                if (directory.DirsOrFiles[i].Dir_Attribute == 0x10)
                                {//empty or full
                                    Console.WriteLine($"<DIR>\t{directory.DirsOrFiles[i].Dir_Name}");
                                    countDir++;
                                }
                                else if (directory.DirsOrFiles[i].Dir_Attribute == 0x0)
                                {
                                    Console.WriteLine($"{directory.DirsOrFiles[i].Dir_FileSize}\t{directory.DirsOrFiles[i].Dir_Name}");
                                    countFile++;
                                    sumFileSizes += directory.DirsOrFiles[i].Dir_FileSize;
                                }
                            }
                            Console.WriteLine($"Number of files {countFile}\ttheir size {sumFileSizes}");
                            Console.WriteLine($"Number of dirs {countDir + 2}\tFree space{Virtual_Disk.Get_FreeSize()}");//Is this right??
                        }
                        else if (index != -1 && current.DirsOrFiles[index].Dir_Attribute == 0x0)
                        {
                            Console.WriteLine($"File Size {current.DirsOrFiles[index].Dir_FileSize}\tFile Name {current.DirsOrFiles[index].Dir_Name}");
                        }
                        else
                        {
                            Console.WriteLine($"error : {pathList[0]} No such a file or directory");
                        }
                    }
                    else if (pathList.Count > 1)
                    {
                        Directory directory = new Directory();
                        bool foundDir = MoveToDir(ls[1], ref directory);
                        File_entry file_Entry = new File_entry();
                        bool foundFile = MoveToFile(ls[1], ref file_Entry);
                        if (foundDir)
                        {
                            directory.Read_Dir();
                            int countFile = 0, countDir = 0, sumFileSizes = 0;
                            Console.WriteLine($"<DIR>\t{directory.Dir_Name}");
                            Console.WriteLine($"<DIR>\t{directory.Parent.Dir_Name}");
                            for (int i = 0; i < directory.DirsOrFiles.Count(); i++)
                            {
                                if (directory.DirsOrFiles[i].Dir_Attribute == 0x10)
                                {//empty or full
                                    Console.WriteLine($"<DIR>\t{directory.DirsOrFiles[i].Dir_Name}");
                                    countDir++;
                                }
                                else if (directory.DirsOrFiles[i].Dir_Attribute == 0x0)
                                {
                                    Console.WriteLine($"{directory.DirsOrFiles[i].Dir_FileSize}\t{directory.DirsOrFiles[i].Dir_Name}");
                                    countFile++;
                                    sumFileSizes += directory.DirsOrFiles[i].Dir_FileSize;
                                }
                            }
                            Console.WriteLine($"Number of files {countFile}\ttheir size {sumFileSizes}");
                            Console.WriteLine($"Number of dirs {countDir + 2}\tFree space{Virtual_Disk.Get_FreeSize()}");//Is this right??
                        }
                        else if (foundFile)
                        {
                            Console.WriteLine($"size {file_Entry.Dir_FileSize}\tname {file_Entry.Dir_Name}");
                        }
                        else
                        {
                            Console.WriteLine("error : No such a file or directory");
                        }
                    }
                }
                else if (ls[0] == "md")
                {
                    List<string> pathList = new List<string>();
                    string[] part;
                    part = ls[1].Split('\\');
                    foreach (string s in part)
                    {
                        pathList.Add(s);
                    }
                    if (pathList.Count == 1)
                    {
                        int index = current.SearchDirOrFiles(pathList[0]);
                        if (index == -1)
                        {
                            Directory_Entry directory_Entry = new Directory_Entry(pathList[0], 0x10, 0);//dirfilesize=0
                            if (current.CanAddEntery(directory_Entry))
                            {
                                current.AddEntry(directory_Entry);
                            }
                        }
                        else
                        {
                            Console.WriteLine("error : the directory name is already exist");
                        }
                    }
                    else
                    {
                        Directory directory = new Directory();
                        string dirName = pathList[pathList.Count - 1];
                        ls[1].Remove(ls[1].LastIndexOf('\\'));
                        if (MoveToDir(ls[1], ref directory))
                        {
                            directory.Read_Dir();
                            int index = directory.SearchDirOrFiles(dirName);
                            if (index == -1)
                            {
                                Directory_Entry directory_Entry = new Directory_Entry(dirName, 0x10, 0);
                                if (directory.CanAddEntery(directory_Entry))
                                {
                                    directory.AddEntry(directory_Entry);
                                }
                                else
                                {
                                    Console.WriteLine("error : there is no space to create the directory");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Duplicate name of the directory");
                            }
                        }
                        else
                        {
                            Console.WriteLine("error");//
                        }
                    }
                }
                else if (ls[0] == "rd")
                {
                    List<string> pathList = new List<string>();
                    string[] part;
                    part = ls[1].Split('\\');
                    foreach (string s in part)
                    {
                        pathList.Add(s);
                    }
                    if (pathList.Count == 1)
                    {
                        int index = current.SearchDirOrFiles(pathList[0]);
                        if (index != -1 && current.DirsOrFiles[index].Dir_Attribute == 0x10)
                        {
                            Directory directory = new Directory(current.DirsOrFiles[index].Dir_Name, 0x10, current.DirsOrFiles[index].Dir_FirstCluster, current);
                            if (directory.Dir_FirstCluster == 0)
                            {
                                Console.WriteLine("Are you sure you want to delete this directory? yes or no");
                                string s;
                                do
                                {
                                    s = Console.ReadLine().ToLower();
                                    if (s == "yes")
                                    {
                                        directory.DeleteDir(directory);
                                    }
                                    else if (s == "no")
                                    {
                                        break;
                                    }
                                } while (s != "yes" || s != "no");
                            }

                        }
                    }
                    else
                    {
                        Directory directory = new Directory();
                        if (MoveToDir(ls[1], ref directory))
                        {
                            if (directory.Dir_FirstCluster == 0)
                            {
                                Console.WriteLine("Are you sure you want to delete this directory? yes or no");
                                string s;
                                do
                                {
                                    s = Console.ReadLine().ToLower();
                                    if (s == "yes")
                                    {
                                        directory.DeleteDir(directory);
                                    }
                                    else if (s == "no")
                                    {
                                        break;
                                    }
                                } while (s != "yes" || s != "no");
                            }
                        }
                        else
                        {
                            Console.WriteLine("error : directory is not exist ");
                        }
                    }
                }
                else if (ls[0] == "type")
                {
                    List<string> pathList = new List<string>();
                    string[] part;
                    part = ls[1].Split('\\');
                    foreach (string s in part)
                    {
                        pathList.Add(s);
                    }
                    if (pathList.Count == 1)
                    {
                        int index = current.SearchDirOrFiles(pathList[0]);
                        if (index != -1)
                        {
                            File_entry file = new File_entry(pathList[0], 0x0, current.DirsOrFiles[index].Dir_FirstCluster, current, current.DirsOrFiles[index].Dir_FileSize, null);
                            //file.Read_File();
                            //file.PrintContent();
                        }
                        else
                        {
                            Console.WriteLine($"error : {pathList[0]} not exist");
                        }
                    }
                    else
                    {
                        File_entry file = new File_entry();
                        if (MoveToFile(ls[1], ref file))
                        {
                            //file.Read_File();
                            //file.PrintContent();
                        }
                        else
                        {
                            Console.WriteLine("error :no such a file");
                        }
                    }
                }
                else if (ls[0] == "rename")
                {
                    List<string> pathList = new List<string>();
                    string[] part;
                    part = ls[1].Split('\\');
                    foreach (string s in part)
                    {
                        pathList.Add(s);
                    }
                    if (pathList.Count == 1)//just the name of old file
                    {
                        int index = current.SearchDirOrFiles(pathList[0]);
                        if (index != -1 && current.DirsOrFiles[index].Dir_Attribute == 0x0)
                        {
                            Directory_Entry OLD = new Directory_Entry(pathList[0], 0x0, current.DirsOrFiles[index].Dir_FirstCluster);
                            if (current.SearchDirOrFiles(ls[2]) == -1)
                            {
                                Directory_Entry NEW = new Directory_Entry(ls[2], 0x0, current.DirsOrFiles[index].Dir_FirstCluster);
                                current.changeContent(OLD, NEW);
                                current.Write_Dir();
                            }
                            else
                            {
                                Console.WriteLine($"{ls[2]} is already exist");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"{pathList[0]} is not exist");
                        }
                    }
                    else//full path
                    {
                        File_entry file = new File_entry();
                        if (MoveToFile(ls[1], ref file))
                        {
                            Directory_Entry OLD = new Directory_Entry(file.Dir_Name.ToString(), 0x0, file.Dir_FirstCluster);
                            if (current.SearchDirOrFiles(ls[2]) == -1)
                            {
                                Directory_Entry NEW = new Directory_Entry(ls[2], 0x0, file.Dir_FirstCluster);
                                current.changeContent(OLD, NEW);
                                current.Write_Dir();
                            }
                            else
                            {
                                Console.WriteLine($"{ls[2]} is already exist");
                            }
                        }
                        else
                        {
                            Console.WriteLine("the file is not exist");
                        }
                    }
                }
                else if (ls[0] == "del")
                {
                    List<string> pathList = new List<string>();
                    string[] part;
                    part = ls[1].Split('\\');
                    foreach (string s in part)
                    {
                        pathList.Add(s);
                    }
                    if (pathList.Count == 1)
                    {
                        int index = current.SearchDirOrFiles(pathList[0]);
                        if (index != -1 && current.DirsOrFiles[index].Dir_Attribute == 0x0)//name of file
                        {
                            File_entry file = new File_entry(pathList[0], 0x0, current.DirsOrFiles[index].Dir_FirstCluster, current, current.DirsOrFiles[index].Dir_FileSize, null);
                            Console.WriteLine("Are you sure you want to delete this file? yes or no");
                            string s;
                            do
                            {
                                s = Console.ReadLine().ToLower();
                                if (s == "yes")
                                {
                                    file.DeleteFile(file);
                                }
                                else if (s == "no")
                                {
                                    break;
                                }
                            } while (s != "yes" || s != "no");
                        }
                        else if (index != -1 && current.DirsOrFiles[index].Dir_Attribute == 0x10)//name of directory
                        {
                            Directory directory = new Directory(current.DirsOrFiles[index].Dir_Name.ToString(), 0x10, current.DirsOrFiles[index].Dir_FirstCluster, current);
                            if (directory.Dir_FirstCluster == 0)
                            {
                                Console.WriteLine("Are you sure you want to delete this directory? yes or no");
                                string s;
                                do
                                {
                                    s = Console.ReadLine().ToLower();
                                    if (s == "yes")
                                    {
                                        for (int i = 0; i < directory.DirsOrFiles.Count; i++)
                                        {
                                            if (directory.DirsOrFiles[i].Dir_Attribute == 0x0)
                                            {
                                                File_entry file1 = new File_entry(directory.DirsOrFiles[i].Dir_Name.ToString(), directory.DirsOrFiles[i].Dir_Attribute, directory.DirsOrFiles[i].Dir_FirstCluster, directory, directory.DirsOrFiles[i].Dir_FileSize, null);
                                                file1.DeleteFile(file1);
                                            }
                                        }
                                    }
                                    else if (s == "no")
                                    {
                                        break;
                                    }
                                } while (s != "yes" || s != "no");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"{pathList[0]} is not exist");
                        }
                    }
                    else
                    {
                        File_entry file = new File_entry();//full path to file
                        if (MoveToFile(ls[1], ref file))
                        {
                            Console.WriteLine("Are you sure you want to delete this file? yes or no");
                            string s;
                            do
                            {
                                s = Console.ReadLine().ToLower();
                                if (s == "yes")
                                {
                                    file.DeleteFile(file);
                                }
                                else if (s == "no")
                                {
                                    break;
                                }
                            } while (s != "yes" || s != "no");
                        }
                        else
                        {
                            Directory directory = new Directory();//full path to directory
                            if (MoveToDir(ls[1], ref directory))
                            {
                                if (directory.Dir_FirstCluster == 0)
                                {
                                    Console.WriteLine("Are you sure you want to delete files in this directory? yes or no");
                                    string s;
                                    do
                                    {
                                        s = Console.ReadLine().ToLower();
                                        if (s == "yes")
                                        {
                                            for (int i = 0; i < directory.DirsOrFiles.Count; i++)
                                            {
                                                if (directory.DirsOrFiles[i].Dir_Attribute == 0x0)
                                                {
                                                    File_entry file1 = new File_entry(directory.DirsOrFiles[i].Dir_Name.ToString(), directory.DirsOrFiles[i].Dir_Attribute, directory.DirsOrFiles[i].Dir_FirstCluster, directory, directory.DirsOrFiles[i].Dir_FileSize, null);
                                                    file1.DeleteFile(file1);
                                                }
                                            }
                                        }
                                        else if (s == "no")
                                        {
                                            break;
                                        }
                                    } while (s != "yes" || s != "no");
                                }
                            }
                            else
                            {
                                Console.WriteLine("error :the file or directory is not exist");
                            }
                        }
                    }
                }
                else if (ls[0] == "import")//not complete
                {
                    if (File.Exists(ls[1]))
                    {
                        string contntent = File.ReadAllText(ls[1]);//get content
                        int siz = contntent.Length;
                        int nAme = ls[1].LastIndexOf("\\");//name that start of it after this mark
                        string n = ls[1].Substring(nAme + 1);
                        int index = Program.current.SearchDirOrFiles(n);
                        if (index == -1)
                        {
                            int first_cluster = 0;
                            File_entry file = new File_entry(n, 0x0, current.DirsOrFiles[index].Dir_FirstCluster, current, current.DirsOrFiles[index].Dir_FileSize, null);  // (nAme, 0x0, file.Dir_FirstCluster, null);//not sure
                            if (file.Dir_FirstCluster > 0)//
                                first_cluster = Mini_Fat.Get_AvailableClusters();
                            else
                                first_cluster = 0;
                            Directory_Entry f = new Directory_Entry();
                            current.AddEntry(f);
                            current.Write_Dir();
                        }
                        else
                        {
                            Console.WriteLine("file is exist you can't add it");
                        }
                    }
                    else
                    {
                        Console.WriteLine("the file does not exist");
                    }
                }
                else if (ls[0] == "copy")
                {

                }
                else if (ls[0] == "export")//just one test case
                {
                    int index = current.SearchDirOrFiles(ls[1]);//check if the file does exist
                    if (index != -1)
                    {
                        if (System.IO.Directory.Exists(ls[2]))
                        {//check if the path of destination does exist

                            File_entry file = new File_entry(ls[1], 0x0, current.DirsOrFiles[index].Dir_FirstCluster, current, current.DirsOrFiles[index].Dir_FileSize, null);
                            //file.Read_File();
                            StreamWriter f = new StreamWriter(ls[2] + "\\" + ls[1]);
                            f.Write(file.content);
                            f.Flush();
                            f.Close();
                        }
                        else
                        {
                            Console.WriteLine("the path of directory does not exist");
                        }
                    }
                    else
                    {
                        Console.WriteLine("the file does not exist");
                    }
                }
                else if (ls[0] == "cls") Console.Clear();
                else if (ls[0] == "quit") Environment.Exit(1);
                else Console.WriteLine("'" + ls[0] + ls[1] + "'" + "not command");//for loop 
            }
            else if (ls[0] == "cls") Console.Clear();
            else if (ls[0] == "quit") Environment.Exit(1);
            else Console.WriteLine("'" + ls[0] + "'" + "not command");
        }
        public static bool MoveToDir(string path, ref Directory directory)
        {
            bool found = false;
            Directory root = new Directory("S:", 0x10, 5, null);
            List<string> list = new List<string>();
            string[] part;
            part = path.Split('\\');
            foreach (string s in part)
            {
                list.Add(s);
            }
            if (list[0] == "S:")
            {
                for (int i = 1; i < list.Count(); i++)
                {
                    int index = root.SearchDirOrFiles(list[i]);
                    if (index != -1)
                    {
                        found = true;
                        Directory root1 = new Directory(root.DirsOrFiles[index].Dir_Name.ToString(), root.DirsOrFiles[index].Dir_Attribute, root.DirsOrFiles[index].Dir_FirstCluster, root);
                        root = root1;
                    }
                    else
                    {
                        found = false;
                        break;
                    }
                }
            }
            else
            {
                found = false;
                return found;
            }
            directory = root;
            return found;
        }
        public static bool MoveToFile(string path, ref File_entry file_Entry)
        {
            bool found = false;
            List<string> pathList = new List<string>();
            string[] part;
            part = path.Split('\\');
            foreach (string s in part)
            {
                pathList.Add(s);
            }
            string fileName = pathList[pathList.Count - 1];
            path.Remove(path.LastIndexOf('\\'));
            Directory directory = new Directory();
            if (MoveToDir(path, ref directory))
            {
                directory.Read_Dir();
                int index = directory.SearchDirOrFiles(fileName);
                if (index != -1 && directory.DirsOrFiles[index].Dir_Attribute == 0x0)
                {
                    found = true;
                    file_Entry = new File_entry(fileName, directory.DirsOrFiles[index].Dir_Attribute, directory.DirsOrFiles[index].Dir_FirstCluster, directory, directory.DirsOrFiles[index].Dir_FileSize, null);
                }
                else
                {
                    found = false;
                    return found;
                }
            }
            else
            {
                found = false;
                return found;
            }
            return found;
        }
    }
    public class Virtual_Disk
    {
        static FileStream Disk;
        public void Initialize_File(string name)
        {
            if (!File.Exists(name))
            {
                Disk = new FileStream(name, FileMode.Create);
                byte[] b = new byte[1024];
                Write_Cluster(0, b);//mini_fat->continue
                Mini_Fat.Prepare_Fat();
                Mini_Fat.Write_Fat();
            }
            else
            {
                Disk = new FileStream(name, FileMode.Open);//mini_fat->continue
                Mini_Fat.Read_Fat();

            }
        }
        public static void Write_Cluster(int v, byte[] b)
        {
            Disk.Seek(v * 1024, SeekOrigin.Begin);
            for (int i = 0; i < b.Length; i++)
            {
                Disk.WriteByte(b[i]);
            }
            Disk.Flush();
        }
        public static byte[] Read_Cluster(int v)//not sure of this code 
        {
            Disk.Seek(v * 1024, SeekOrigin.Begin);
            byte[] b = new byte[1024];
            for (int i = 0; i < 1024; i++)
            {
                b[i] = (byte)Disk.ReadByte();
            }
            return b;
        }
        public static int Get_FreeSize()
        {
            return (1024 * 1024) - (Mini_Fat.Get_AvailableClusters() * 1024);
        }
    }
    public class Mini_Fat
    {
        static int[] Fat = new int[1024];
        public static void Prepare_Fat()
        {
            for (int i = 0; i < 1024; i++)
            {
                if (i == 0 || i == 4)
                {
                    Fat[i] = -1;
                }
                else if (i > 0 && i < 4)
                {
                    Fat[i] = i + 1;
                }
                else
                    Fat[i] = 0;
            }
        }
        public static void Write_Fat()
        {
            byte[] Fat_Convert = new byte[4096];
            System.Buffer.BlockCopy(Fat, 0, Fat_Convert, 0, 1024);
            for (int i = 0; i < 4; i++)
            {
                byte[] c = new byte[1024];//continue
                for (int j = i * 1024, k = 0; j < (i + 1) * 1024; j++, k++)
                {
                    c[k] = Fat_Convert[j];
                }
                Virtual_Disk.Write_Cluster(i + 1, c);

            }
        }
        public static int[] Read_Fat()
        {
            byte[] Fat_Convert = new byte[4096];
            for (int i = 0; i < 4; i++)
            {
                byte[] c = Virtual_Disk.Read_Cluster(i + 1); //continue
                for (int j = i * 1024, k = 0; j < (i + 1) * 1024; j++, k++)
                {
                    Fat_Convert[j] = c[k];
                }
            }
            int[] arr = new int[1024];
            for (int i = 0; i < 1024; i++)
            {
                arr[i] = BitConverter.ToInt32(Fat_Convert, i * 4);
            }
            return arr;
        }
        public static int Get_EmptyCluster()
        {
            for (int i = 5; i < 1024; i++)
            {
                if (Fat[i] == 0)
                {
                    return i;
                }
            }
            return -1;
        }
        public static void Set_ClusterStatus(int v, int status)
        {
            Fat[v] = status;
        }
        public static int Get_ClusterStatus(int v)
        {
            return Fat[v];
        }
        public static int Get_AvailableClusters()
        {
            int counter = 0;
            for (int i = 5; i < 1024; i++)
            {
                if (Fat[i] == 0)
                {
                    counter++;
                }
            }
            return counter;
        }
    }
    public class Directory_Entry
    {
        public string Dir_Name;
        //public char[] Dir_Name = new char[11];
        public byte Dir_Attribute;
        public byte[] Dir_Empty = new byte[12];
        public int Dir_FirstCluster, Dir_FileSize;
        public Directory_Entry()
        {

        }
        public Directory_Entry(string name, byte attr, int FC)
        {
            Dir_Name = name;
            Dir_Attribute = attr;
            Dir_FirstCluster = FC;
        }
        public string validNameFile(string name)
        {
            if (name.Length < 7)
            {
                for (int i = name.Length + 1; i <= 7; i++)
                {
                    name += " ";
                }
            }
            else
            {
                name = name.Substring(0, 7);
            }
            return name;
        }
        public string validNameDirectory(string name)
        {
            if (name.Length < 11)
            {
                for (int i = name.Length + 1; i <= 11; i++)
                {
                    name += " ";
                }
            }
            else
            {
                name = name.Substring(0, 11);
            }
            return name;
        }
    }
    public class Directory : Directory_Entry
    {
        public List<Directory_Entry> DirsOrFiles;
        public Directory Parent;
        public Directory()
        {

        }
        public Directory(string name, byte attr, int FC, Directory pa) : base(name, attr, FC)//still not complete!
        {
            DirsOrFiles = new List<Directory_Entry>();
            //Directory_Entry directory_Entry = new Directory_Entry(name, attr, FC);
            //DirsOrFiles.Add(directory_Entry);
            /* if (Parent != null)
             {
                 Directory_Entry pa=new Directory_Entry()
             }*/
        }
        public void Write_Dir()//WTF?!
        {
            Directory_Entry directory_Entry = GetMyDirectory_Entry();
            byte[] b = new byte[DirsOrFiles.Count * 32];
            byte[] c = new byte[1024];
            List<byte[]> listOfArrays = new List<byte[]>();
            for (int i = 0; i < DirsOrFiles.Count; i++)//not compelete
            {
                for (int j = 0; j < 32; j++)//how to convert????
                {
                    System.Buffer.BlockCopy(Dir_Name.ToCharArray(), 0, b, 0 + (i * 32), Dir_Name.Length);
                    b[11 + (i * 32)] = DirsOrFiles[i].Dir_Attribute;
                    System.Buffer.BlockCopy(Dir_Empty, 0, b, 12 + (i * 32), Dir_Empty.Length);
                    System.Buffer.BlockCopy(BitConverter.GetBytes(Dir_FirstCluster), 0, b, 24 + (i * 32), 4);
                    System.Buffer.BlockCopy(BitConverter.GetBytes(Dir_FileSize), 0, b, 28 + (i * 32), 4);
                }
            }
            for (int i = 0, j = 0; i < b.Length; i++, j++)
            {
                if (j % 1024 == 0 && j != 0)//if length less than 1024
                {
                    listOfArrays.Add(c);
                    c = new byte[1024];
                    j = 0;
                }
                c[j] = b[i];
            }
            int lastCluster = -1, clusterIndex;
            if (Dir_FirstCluster == 0)
            {
                clusterIndex = Mini_Fat.Get_EmptyCluster();
                Dir_FirstCluster = clusterIndex;
            }
            else
            {
                EmptyMyCluster();
                clusterIndex = Mini_Fat.Get_EmptyCluster();
                Dir_FirstCluster = clusterIndex;
            }
            for (int i = 0; i < listOfArrays.Count; i++)
            {
                Virtual_Disk.Write_Cluster(clusterIndex, listOfArrays[i]);
                Mini_Fat.Set_ClusterStatus(clusterIndex, -1);
                if (lastCluster != -1)
                {
                    Mini_Fat.Set_ClusterStatus(lastCluster, clusterIndex);
                }
                lastCluster = clusterIndex;
                clusterIndex = Mini_Fat.Get_EmptyCluster();
            }
            if (Parent != null)
            {
                Parent.changeContent(directory_Entry, GetMyDirectory_Entry());
                Parent.Write_Dir();
            }
            Mini_Fat.Write_Fat();
        }
        public void Read_Dir()
        {
            if (Dir_FirstCluster != 0)
            {
                DirsOrFiles = new List<Directory_Entry>();
                int clusterIndex = Dir_FirstCluster;
                int next = Mini_Fat.Get_ClusterStatus(clusterIndex);
                if (clusterIndex == 5 && next == 0)
                    return;
                List<byte> ls = new List<byte>();
                do
                {
                    ls.AddRange(Virtual_Disk.Read_Cluster(clusterIndex));
                    byte[] c = Virtual_Disk.Read_Cluster(clusterIndex);
                    clusterIndex = next;
                    if (next != -1)
                    {
                        next = Mini_Fat.Get_ClusterStatus(clusterIndex);
                    }
                } while (clusterIndex != -1);
                for (int i = 0; i < ls.Count; i++)
                {
                    byte[] b = new byte[32];
                    for (int m = 0; m < b.Length; m++)
                    {
                        b[i] = ls[m];
                    }
                    Directory_Entry directory_Entry = new Directory_Entry();
                    System.Buffer.BlockCopy(b, 0, directory_Entry.Dir_Name.ToCharArray(), 0, 11);
                    directory_Entry.Dir_Attribute = b[11];
                    System.Buffer.BlockCopy(b, 12, directory_Entry.Dir_Empty, 0, 12);
                    System.Buffer.BlockCopy(b, 24, BitConverter.GetBytes(directory_Entry.Dir_FirstCluster), 0, 4);
                    System.Buffer.BlockCopy(b, 28, BitConverter.GetBytes(directory_Entry.Dir_FileSize), 0, 4);
                    DirsOrFiles.Add(directory_Entry);
                }
            }
        }
        public Directory_Entry GetMyDirectory_Entry()
        {
            Directory_Entry directory = new Directory_Entry(this.Dir_Name, this.Dir_Attribute, this.Dir_FirstCluster);
            directory.Dir_Empty = this.Dir_Empty;
            directory.Dir_FileSize = this.Dir_FileSize;
            return directory;
        }
        public int SearchDirOrFiles(string name)
        {
            name = validNameDirectory(name);
            for (int i = 0; i < DirsOrFiles.Count; i++)
            {
                if (DirsOrFiles[i].Dir_Name == name)
                {
                    return i;
                }
            }
            return -1;
        }
        /*public void updateInfo(Directory_Entry directory, Directory_Entry directory1)
        {
            directory.Dir_Name = directory1.Dir_Name;
            directory.Dir_Attribute = directory1.Dir_Attribute;
            directory.Dir_Empty = directory1.Dir_Empty;
            directory.Dir_FileSize = directory1.Dir_FileSize;
            directory.Dir_FirstCluster = directory1.Dir_FirstCluster;
        }*/
        public void changeContent(Directory_Entry old, Directory_Entry New)//try,catch if i=-1
        {
            Read_Dir();
            if (SearchDirOrFiles(old.Dir_Name) != -1)
            {
                int index = SearchDirOrFiles(old.Dir_Name);
                DirsOrFiles.RemoveAt(index);
                DirsOrFiles.Add(New);
            }
            else
            {
                Console.WriteLine($"error :{old.Dir_Name} no such file or directory");
            }
        }
        public void RemoveEntry(Directory_Entry directory)//edit parent
        {
            DirsOrFiles.RemoveAt(SearchDirOrFiles(directory.Dir_Name));
        }
        public void DeleteDir(Directory directory)

        {
            EmptyMyCluster();
            if (this.Parent != null)
            {
                this.Parent.RemoveEntry(GetMyDirectory_Entry());
            }
            if (Program.current == this)
            {
                if (this.Parent != null)
                {
                    Program.current = this.Parent;
                    Program.path = Program.path.Substring(0, Program.path.IndexOf("\\"));
                }
            }
        }
        public void AddEntry(Directory_Entry directory)//edit parent
        {
            directory.Dir_Name = validNameDirectory(Dir_Name);
            DirsOrFiles.Add(directory);
            Write_Dir();
        }
        public void EmptyMyCluster()
        {
            int cluster, next;
            if (this.Dir_FirstCluster != 0)
            {
                cluster = this.Dir_FirstCluster;
                next = Mini_Fat.Get_ClusterStatus(cluster);
                if (cluster == 5 && next == 0)
                    return;
                while (cluster != -1)
                {
                    Mini_Fat.Set_ClusterStatus(cluster, 0);
                    cluster = next;
                    if (cluster != -1)
                    {
                        next = Mini_Fat.Get_ClusterStatus(cluster);
                    }
                }
            }

        }
        public int Get_SizeOfDir(Directory_Entry directory)//return number of clusters the dir has
        {
            int cluster, next, counter = 0;
            if (directory.Dir_FirstCluster != 0)
            {
                cluster = directory.Dir_FirstCluster;
                next = Mini_Fat.Get_ClusterStatus(cluster);
                while (next != -1)
                {
                    counter++;
                    cluster = next;
                    next = Mini_Fat.Get_ClusterStatus(cluster);
                }
            }
            return counter;
        }
        public bool CanAddEntery(Directory_Entry directory)
        {
            int remainder, numOfClusters = 0;
            numOfClusters = ((DirsOrFiles.Count + 1) * 32) / 1024;
            remainder = ((DirsOrFiles.Count + 1) * 32) % 1024;
            if (remainder > 0)
            {
                numOfClusters++;
            }
            if (Mini_Fat.Get_AvailableClusters() >= numOfClusters)//is that true?
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public class File_entry : Directory_Entry
    {
        public string content;
        Directory Parent;
        public File_entry()
        {

        }
        public File_entry(string name, byte attr, int FC, Directory Parent, int fileSize, string content) : base(name, attr, FC)
        {
            this.Parent = Parent;
            Dir_FileSize = fileSize;
            this.content = content;
        }
        public Directory_Entry GetMyDirectory_Entry()
        {
            Directory_Entry directory = new Directory_Entry();

            return directory;
        }
        public void EmptyMyCluster(Directory_Entry directory)
        {
            int cluster, next;
            if (directory.Dir_FirstCluster != 0)
            {
                cluster = directory.Dir_FirstCluster;
                next = Mini_Fat.Get_ClusterStatus(cluster);
                while (next != -1)
                {
                    Mini_Fat.Set_ClusterStatus(cluster, 0);
                    cluster = next;
                    next = Mini_Fat.Get_ClusterStatus(cluster);
                }
            }

        }
        public int Get_SizeOfFile(Directory_Entry directory)//return number of clusters the dir has
        {
            int cluster, next, counter = 0;
            if (directory.Dir_FirstCluster != 0)
            {
                cluster = directory.Dir_FirstCluster;
                next = Mini_Fat.Get_ClusterStatus(cluster);
                while (next != -1)
                {
                    counter++;
                    cluster = next;
                    next = Mini_Fat.Get_ClusterStatus(cluster);
                }
            }
            return counter;
        }
        public void DeleteFile(File_entry file)
        {
            EmptyMyCluster(file);
            if (this.Parent != null)
            {
                this.Parent.RemoveEntry(GetMyDirectory_Entry());
            }
        }
        public void PrinteContent()
        {
            Console.WriteLine(content);
        }

    }
}
