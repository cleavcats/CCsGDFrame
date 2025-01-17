using System.Collections.Generic;
using Godot;
using System.Text.RegularExpressions;

namespace Cleavcats
{
    /// <summary>
    /// // 依次从 多个目录下寻找 目标资源，得到最先匹配的那个
    /// <br>// godot 实现：首先考虑运行目录下的资源，再考虑内置 res:// 路径下的资源</br>
    /// <br>//  Release 导出时 只考虑内置 res:// 资源</br>
    /// </summary>
    public class Godot_IO_MulitFolder : IO_MulitFolder
    {
        bool godot_file_exist(string src_path)
        {
            if (src_path.StartsWith("res://", System.StringComparison.OrdinalIgnoreCase)) return ResourceLoader.Exists(src_path);
            return System.IO.File.Exists(src_path);
        }
        bool godot_dir_exist(string src_path)
        {
            if (src_path.StartsWith("res://", System.StringComparison.OrdinalIgnoreCase))
            {
                Directory dir = new Directory();
                return dir.DirExists(src_path);
            }
            return System.IO.Directory.Exists(src_path);
        }
        List<string> godot_search_files(string path, string 正则表达式 = ".*", bool 包括子文件夹 = false)
        {
            List<string> returns = new List<string>();
            // 
            Directory dir = new Directory();
            Error err = dir.Open(path);
            if (err != Error.Ok) return returns;
            if (path.EndsWith("/") == false) path += "/";
            dir.ListDirBegin(true, true);
            string founded_file = dir.GetNext();
            while (founded_file != string.Empty)
            {
                if (dir.DirExists(founded_file) && 包括子文件夹)
                {
                    List<string> 递归返回值_list = godot_search_files(path + founded_file, 正则表达式, true);
                    foreach (string 递归返回值 in 递归返回值_list) returns.Add(递归返回值);
                }
                if (dir.FileExists(founded_file))
                {
                    if (Regex.Match(founded_file, 正则表达式).Success) returns.Add(path + founded_file);
                }
                founded_file = dir.GetNext(); continue;
            }
            return returns;
        }
        List<string> godot_search_dirs(string path, string 正则表达式 = ".*", bool 包括子文件夹 = false)
        {
            path = this.GetDirRealPath(path);
            if (path.EndsWith("/") == false) path += "/";
            List<string> returns = new List<string>();
            Directory dir = new Directory();
            Error err = dir.Open(path);
            if (err != Error.Ok) return returns;
            dir.ListDirBegin(true);
            string founded_file = dir.GetNext();
            while (founded_file != string.Empty)
            {
                if (dir.DirExists(founded_file) == false) { founded_file = dir.GetNext(); continue; }
                if (Regex.Match(founded_file, 正则表达式).Success) returns.Add(path + founded_file);
                if (包括子文件夹)
                {
                    List<string> 递归返回值_list = godot_search_dirs(path + founded_file, 正则表达式, true);
                    foreach (string 递归返回值 in 递归返回值_list) returns.Add(递归返回值);
                }
                founded_file = dir.GetNext(); continue;
            }
            return returns;
        }

        public Godot_IO_MulitFolder(params string[] path) : base(path)
        {
            this.custom_file_exist = godot_file_exist;
            this.custom_dir_exist = godot_dir_exist;
            this.custom_search_files = godot_search_files;
            this.custom_search_dirs = godot_search_dirs;
        }

        /// <summary>
        /// // 取得文本文件 的字符串
        /// </summary>
        public string GetFileAsText(string src_path)
        {
            string path = this.GetFileRealPath(src_path);
            if (path == null) return null;
            Godot.File file = new Godot.File();
            file.Open(path, Godot.File.ModeFlags.Read);
            string returns = file.GetAsText();
            file.Close();
            return returns;
        }
        /// <summary>
        /// // 读取一个 Godot ResourceLoader 可以识别的 资源
        /// </summary>
        public T GetResource<T>(string src_path) where T : class
        {
            string path = this.GetFileRealPath(src_path);
            if (path == null) return null;
            return Godot.ResourceLoader.Load<T>(path);
        }
    }
}