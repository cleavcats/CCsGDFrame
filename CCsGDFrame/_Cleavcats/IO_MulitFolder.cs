using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Cleavcats
{
    /// <summary>
    /// // 注入多个文件夹路径，查找文件时，依次从中寻找
    /// </summary>
    public class IO_MulitFolder
    {
        public IO_MulitFolder(params string[] path) { foreach (string founded in path) this.list_scan_dir.Add(founded); }
        public delegate bool delegate_IO_MulitFolder_Exist(string src_path);
        static bool delegate_IO_MulitFolder_FileExist_Csharp(string src_path) { return System.IO.File.Exists(src_path); }
        static bool delegate_IO_MulitFolder_DirExist_Csharp(string src_path) { return System.IO.Directory.Exists(src_path); }
        public delegate_IO_MulitFolder_Exist custom_file_exist = new delegate_IO_MulitFolder_Exist(delegate_IO_MulitFolder_FileExist_Csharp);
        public delegate_IO_MulitFolder_Exist custom_dir_exist = new delegate_IO_MulitFolder_Exist(delegate_IO_MulitFolder_DirExist_Csharp);

        public delegate List<string> delegate_IO_MulitFolder_Search(string path, string 正则表达式 = ".*", bool 包括子文件夹 = false);
        static List<string> delegate_IO_MulitFolder_SearchFile_inner(string path, string 正则表达式 = ".*", bool 包括子文件夹 = false)
        {
            List<string> returns = new List<string>();
            if (包括子文件夹)
            {
                foreach (string file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
                {
                    string file_name = Path.GetFileName(file);
                    if (Regex.Match(file_name, 正则表达式).Success) returns.Add(file);
                }
            }
            else
            {
                foreach (string file in Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly))
                {
                    string file_name = Path.GetFileName(file);
                    if (Regex.Match(file_name, 正则表达式).Success) returns.Add(file);
                }
            }
            return returns;
        }
        static List<string> delegate_IO_MulitFolder_SearchDir_inner(string path, string 正则表达式 = ".*", bool 包括子文件夹 = false)
        {
            List<string> returns = new List<string>();
            if (包括子文件夹)
            {
                foreach (string dir in Directory.GetDirectories(path, "*", SearchOption.AllDirectories))
                {
                    string dir_name = Path.GetDirectoryName(dir);
                    if (Regex.Match(dir_name, 正则表达式).Success) returns.Add(dir);
                }
            }
            else
            {
                foreach (string dir in Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly))
                {
                    string dir_name = Path.GetDirectoryName(dir);
                    if (Regex.Match(dir_name, 正则表达式).Success) returns.Add(dir);
                }
            }
            return returns;
        }
        public delegate_IO_MulitFolder_Search custom_search_files = new delegate_IO_MulitFolder_Search(delegate_IO_MulitFolder_SearchFile_inner);
        public delegate_IO_MulitFolder_Search custom_search_dirs = new delegate_IO_MulitFolder_Search(delegate_IO_MulitFolder_SearchDir_inner);
        /// <summary>
        /// // 查找子文件，匹配符为正则表达式（常用：^为开头，$为末尾，.为一切字符，*为重复字符，\\为转义符）
        /// </summary>
        public List<string> SearchFiles(string path, string 正则表达式 = ".*", bool 包括子文件夹 = false)
        {
            List<string> returns = new List<string>();
            foreach (string scan_path in list_scan_dir)
            {
                List<string> scan_buffer = custom_search_files(scan_path + path, 正则表达式, 包括子文件夹);
                foreach (string founded in scan_buffer) returns.Add(founded);
            }
            return returns;
        }
        /// <summary>
        /// // 查找文件夹，匹配符为正则表达式（常用：^为开头，$为末尾，.为一切字符，*为重复字符，\\为转义符）
        /// </summary>
        public List<string> SearchDirs(string path, string 正则表达式 = ".*", bool 包括子文件夹 = false)
        {
            List<string> returns = new List<string>();
            foreach (string scan_path in list_scan_dir)
            {
                List<string> scan_buffer = custom_search_dirs(scan_path + path, 正则表达式, 包括子文件夹);
                foreach (string founded in scan_buffer) returns.Add(founded);
            }
            return returns;
        }

        /// <summary>
        /// // 等待扫描的文件夹路径，需要带有末尾的 /
        /// </summary>
        public List<string> list_scan_dir = new List<string>() { "/" };

        /// <summary>
        /// // 取得 真实路径
        /// </summary>
        public string GetFileRealPath(string src_path)
        {
            foreach (string dir in list_scan_dir)
            {
                if (custom_file_exist(dir + src_path)) return dir + src_path;
            }
            return null;
        }
        /// <summary>
        /// // 取得 真实路径
        /// </summary>
        public string GetDirRealPath(string src_path)
        {
            foreach (string dir in list_scan_dir)
            {
                if (custom_dir_exist(dir + src_path)) return dir + src_path;
            }
            return null;
        }
    }
}