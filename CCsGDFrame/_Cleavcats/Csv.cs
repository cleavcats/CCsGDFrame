using System.IO;
using System.Collections.Generic;

namespace Cleavcats
{
    public class Csv
    {
        public List<List<string>> list = new List<List<string>>();
        public int RowCount { get { return list.Count; } }
        public string this[int y, int x]
        {
            get
            {
                if (y < 0 || y >= list.Count) return null;
                if (x < 0 || x >= list[y].Count) return null;
                return list[y][x];
            }
            set
            {
                if (y < 0 || x < 0) return;
                while (y >= list.Count) list.Add(new List<string>());
                while (x >= list[y].Count) list[y].Add(null);
                list[y][x] = value;
            }
        }
        public string Get(int y, int x) { return this[y, x]; }
        public void Set(int y, int x, string value) { this[y, x] = value; }

        /// <summary>
        /// // 从csv文件读取
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Csv ReadFromFile(string url, char splite_key = ',')
        {
            Csv returns = new Csv();
            FileStream stream;
            try
            {
                stream = File.OpenRead(url);
            }
            catch { return null; }
            StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
            returns.ReadFromTextStream_Inner(reader, splite_key);
            stream.Close();
            return returns;
        }
        /// <summary>
        /// // 从 csv 文件的字符串读取
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Csv ReadFromTextData(string data, char splite_key = ',')
        {
            Csv returns = new Csv();
            returns.ReadFromTextData_Inner(data, splite_key);
            return returns;
        }
        /// <summary>
        /// // 从自定义的流读取
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Csv ReadFromCustomStream(Stream stream)
        {
            Csv returns = new Csv();
            returns.ReadFromCustomStream_Inner(stream);
            return returns;
        }

        protected bool ReadFromTextData_Inner(string data2, char splite_key = ',')
        {
            StringReader reader = new StringReader(data2);
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            List<string> list_cell = new List<string>();// 列 缓存

            bool 括号 = false;
            bool 下一列 = false, 下一行 = false;

            char data = '\0', last_data = '\0';
            while (reader.Peek() != -1)
            {
                last_data = data;
                data = (char)reader.Read();

                if (builder.Length == 0 && data == '\"'/*括号开始*/) { 括号 = true; }
                else if (last_data == '\"' && data == '\"'/*转义的引号*/) builder.Append("\"");
                else if (括号 && last_data == '\"' && data == splite_key/*括号结束，一列结束*/) { 括号 = false; 下一列 = true; }
                else if (括号 && last_data == '\"' && data == '\r'/*括号结束，一列结束*/) { 括号 = false; 下一列 = true; }
                else if (括号 && last_data == '\"' && data == '\n'/*括号结束，一列结束*/) { 括号 = false; 下一列 = true; }
                else if (!括号 && data == splite_key/*一列结束*/) { 下一列 = true; }
                else if (!括号 && data == '\r'/*一行结束*/) { if (builder.Length > 0) 下一列 = true; 下一行 = true; }
                else if (!括号 && data == '\n'/*一行结束*/) { if (builder.Length > 0) 下一列 = true; 下一行 = true; }
                else if (data == '\"') { }
                else
                {
                    builder.Append(data);
                }
                if (reader.Peek() == -1) { 下一列 = true; 下一行 = true; }
                if (下一列)
                {
                    括号 = false;
                    下一列 = false;
                    list_cell.Add(builder.ToString()); builder.Clear();
                }
                if (下一行)
                {
                    下一行 = false;
                    if (list_cell.Count > 0/*连续换行忽略之*/)
                    {
                        // 添加一行
                        int this_y = this.RowCount;
                        for (int i = 0; i < list_cell.Count; i++) this[this_y, i] = list_cell[i];
                        list_cell.Clear(); builder.Clear();
                    }
                }
            }
            reader.Close();
            return true;
        }
        protected bool ReadFromTextStream_Inner(StreamReader reader, char split_key = ',')
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            List<string> list_cell = new List<string>();// 列 缓存

            bool 括号 = false;
            bool 下一列 = false, 下一行 = false;

            char data = '\0', last_data = '\0';
            while (reader.EndOfStream == false)
            {
                last_data = data;
                data = (char)reader.Read();

                if (builder.Length == 0 && data == '\"'/*括号开始*/) { 括号 = true; }
                else if (last_data == '\"' && data == '\"'/*转义的引号*/) builder.Append("\"");
                else if (括号 && last_data == '\"' && data == split_key/*括号结束，一列结束*/) { 括号 = false; 下一列 = true; }
                else if (括号 && last_data == '\"' && data == '\r'/*括号结束，一列结束*/) { 括号 = false; 下一列 = true; }
                else if (括号 && last_data == '\"' && data == '\n'/*括号结束，一列结束*/) { 括号 = false; 下一列 = true; }
                else if (!括号 && data == split_key/*一列结束*/) { 下一列 = true; }
                else if (!括号 && data == '\r'/*一行结束*/) { if (builder.Length > 0) 下一列 = true; 下一行 = true; }
                else if (!括号 && data == '\n'/*一行结束*/) { if (builder.Length > 0) 下一列 = true; 下一行 = true; }
                else if (data == '\"') { }
                else
                {
                    builder.Append(data);
                }
                if (reader.EndOfStream == true) { 下一列 = true; 下一行 = true; }
                if (下一列)
                {
                    括号 = false;
                    下一列 = false;
                    list_cell.Add(builder.ToString()); builder.Clear();
                }
                if (下一行)
                {
                    下一行 = false;
                    if (list_cell.Count > 0/*连续换行忽略之*/)
                    {
                        // 添加一行
                        int this_y = this.RowCount;
                        for (int i = 0; i < list_cell.Count; i++) this[this_y, i] = list_cell[i];
                        list_cell.Clear(); builder.Clear();
                    }
                }
            }
            reader.Close();
            return true;
        }
        protected bool ReadFromCustomStream_Inner(Stream stream)
        {
            this.Clear();
            int i_count = DataFactory.TypeStream.IntRead(stream);
            for (int i = 0; i < i_count; i++)
            {
                this.list.Add(new List<string>());
                int n_count = DataFactory.TypeStream.IntRead(stream);
                for (int n = 0; n < n_count; n++) this.list[i].Add(DataFactory.TypeStream.StringRead(stream));
            }
            return true;
        }

        /// <summary>
        /// // 保存为Csv字符串
        /// </summary>
        /// <returns></returns>
        public string SaveToString(char splite_key = ',')
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            //写入 各行数据
            for (int y = 0; ;)
            {
                for (int x = 0; ;)
                {
                    string use = list[y][x];
                    bool 括号 = false;
                    if (use.Contains("\"")/* 英文双引号替换成2个，且应包涵在 双引号中 */) { use = use.Replace("\"", "\"\""); 括号 = true; }
                    if (use.Contains(splite_key.ToString()) || use.Contains("\r") || use.Contains("\n")/*含边界控制符的，应包括在 双引号中*/) { 括号 = true; }
                    if (括号) { builder.Append("\""); builder.Append(use); builder.Append("\""); }
                    else { builder.Append(use); }

                    x++;
                    if (x < list[y].Count) { builder.Append(splite_key); }
                    else { break; }
                }
                y++;
                if (y < list.Count) { builder.AppendLine(); }
                else break;
            }
            return builder.ToString();
        }
        /// <summary>
        /// // 保存为csv文件
        /// </summary>
        /// <param name="url"></param>
        public void SaveToFile(string url, char splite_key = ',')
        {
            // 创建路径中的目录
            FileInfo FatherDir = new FileInfo(url);
            if (!FatherDir.Directory.Exists)
            {
                FatherDir.Directory.Create();
            }
            FileStream stream = new FileStream(url, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream, System.Text.Encoding.UTF8);
            //写入 各行数据
            for (int y = 0; ;)
            {
                for (int x = 0; ;)
                {
                    string use = list[y][x];
                    bool 括号 = false;
                    if (use.Contains("\"")/* 英文双引号替换成2个，且应包涵在 双引号中 */) { use = use.Replace("\"", "\"\""); 括号 = true; }
                    if (use.Contains(splite_key.ToString()) || use.Contains("\r") || use.Contains("\n")/*含边界控制符的，应包括在 双引号中*/) { 括号 = true; }
                    if (括号) { writer.Write("\""); writer.Write(use); writer.Write("\""); }
                    else { writer.Write(use); }

                    x++;
                    if (x < list[y].Count) { writer.Write(splite_key); }
                    else { break; }
                }
                y++;
                if (y < list.Count) { writer.WriteLine(); }
                else break;
            }
            writer.Close();
            stream.Close();
        }
        /// <summary>
        /// // 保存为自定义流
        /// </summary>
        /// <param name="stream"></param>
        public void SaveToCustomStream(Stream stream)
        {
            DataFactory.TypeStream.IntWrite(stream, list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                DataFactory.TypeStream.IntWrite(stream, list[i].Count);
                for (int n = 0; n < list[i].Count; n++) DataFactory.TypeStream.StringWrite(stream, list[i][n]);
            }
        }
        /// <summary>
        /// // 将目标变化为该数据的副本
        /// </summary>
        /// <param name="target"></param>
        public void CopyTo(Csv target)
        {
            if (target == null) return;
            MemoryStream use_0 = new MemoryStream();
            this.SaveToCustomStream(use_0); use_0.Position = 0;
            target.ReadFromCustomStream_Inner(use_0);
            use_0.Dispose();
        }

        public void Clear() { list.Clear(); }
        /// <summary>
        /// // 取得指定的一整行
        /// </summary>
		public string[] RowGet(int index)
        {
            if (index < 0 || index >= list.Count) return null;
            return list[index].ToArray();
        }
        /// <summary>
        /// // 将指定的行 替换
        /// </summary>
		public void RowSet(int index, string[] data)
        {
            if (index < 0 || index >= list.Count) return;
            list[index].Clear();
            for (int i = 0; i < data.Length; i++) list[index].Add(data[i]);
        }

        /// <summary>
        /// // 添加新的空行
        /// </summary>
		public void RowAdd() { list.Add(new List<string>()); }
        /// <summary>
        /// // 插入新的空行到 index 下标
        /// </summary>
		public void RowAddAt(int index) { if (index < 0 || index >= list.Count) return; list.Insert(index, new List<string>()); }
        /// <summary>
        /// // 删除一个行
        /// </summary>
		public void RowRemoveAt(int index) { if (index < 0 || index >= list.Count) return; list.RemoveAt(index); }
        /// <summary>
        /// // 删除最后一行
        /// </summary>
		public void RowRemove() { list.RemoveAt(list.Count); }
    }
}