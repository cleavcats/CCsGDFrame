using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Cleavcats
{
    public class CsvBook
    {
        static void LogInner(string obj) { }
        public delegate void LogDele(string obj);
        public static LogDele Log = new LogDele(LogInner);

        protected List<string> list_name = new List<string>();
        protected List<List<List<object>>> list_strs = new List<List<List<object>>>();

        /// <summary>
        /// // 从自定义格式的文本流读取
        /// <br>// 返回读取了多少字节</br>
        /// </summary>
        /// <param name="stream"></param>
        public virtual int Read(Stream stream)
        {
            long position_start = stream.Position;
            list_name.Clear(); list_strs.Clear();
            int name_count = DataFactory.TypeStream.IntRead(stream);// 共有多少个名称
            for (int i = 0; i < name_count; i++)
            {
                string name = DataFactory.TypeStream.StringRead(stream);
                list_name.Add(name); list_strs.Add(new List<List<object>>());// 新的名称
                int line_count = DataFactory.TypeStream.IntRead(stream);// 该名称共有多少行
                for (int n = 0; n < line_count; n++)
                {
                    list_strs[i].Add(new List<object>());// 新的一行数据
                    int value_count = DataFactory.TypeStream.IntRead(stream);// 该行有多少个数值
                    for (int m = 0; m < value_count; m++)
                    {
                        string value = DataFactory.TypeStream.StringRead(stream);
                        list_strs[i][n].Add(value);// 新的数值
                    }
                }
            }
            return (int)(stream.Position - position_start);
        }
        /// <summary>
        ///  // 以自定义格式的文本流储存
        ///  <br>// 返回写入了多少字节</br>
        /// </summary>
        /// <param name="stream"></param>
        public virtual int Save(Stream stream)
        {
            long position_start = stream.Position;
            DataFactory.TypeStream.IntWrite(stream, list_name.Count);// 一共多少名称
            for (int i = 0; i < list_name.Count; i++)
            {
                DataFactory.TypeStream.StringWrite(stream, list_name[i]);// 名称
                DataFactory.TypeStream.IntWrite(stream, list_strs[i].Count);// 一共多少行
                for (int n = 0; n < list_strs[i].Count; n++)
                {
                    DataFactory.TypeStream.IntWrite(stream, list_strs[i][n].Count);// 一共多少个数值
                    for (int m = 0; m < list_strs[i][n].Count; m++) { DataFactory.TypeStream.StringWrite(stream, Convert.ToString(list_strs[i][n][m])); }
                }
            }
            return (int)(stream.Position - position_start);
        }
        public bool ReadFromCsv(Csv csv)
        {
            if (csv == null) return false;
            for (int y = 0; y < csv.list.Count; y++)
            {
                if (csv.list[y].Count <= 0) continue;
                string name = csv.list[y][0];// 数据名称
                int name_index = this.list_name.BinarySearch(name);
                if (name_index < 0) { name_index = ~name_index; this.list_name.Insert(name_index, name); this.list_strs.Insert(name_index, new List<List<object>>()); }
                List<object> line = new List<object>(csv.list[y].Count - 1);// 行数据内容
                if (csv.list[y].Count == 0) this.list_strs[name_index].Add(new List<object>());
                else
                {
                    for (int x = 1; x < csv.list[y].Count; x++) line.Add(csv.list[y][x]);
                    this.list_strs[name_index].Add(line);// 添加一行
                }
            }
            return true;
        }
        public bool SaveToCsv(Csv csv)
        {
            if (csv == null) return false;
            csv.Clear();
            int line = 0;
            for (int i = 0; i < list_name.Count; i++)
                for (int y = 0; y < list_strs[i].Count; y++)
                {
                    csv[line, 0] = list_name[i];
                    for (int x = 0; x < list_strs[i][y].Count; x++) csv[line, x + 1] = list_strs[i][y][x].ToString();
                    line++;
                }
            return true;
        }

        /// <summary>
        /// // 一共有多少种名称
        /// </summary>
        /// <returns></returns>
        public int NameCount() { return list_name.Count; }
        /// <summary>
        /// // 返回名称标签集合
        /// </summary>
        /// <returns></returns>
        public List<string> NameList() { return list_name; }
        public void Clear() { list_name.Clear(); list_strs.Clear(); }

        public object Get(string name, double y = 0, double x = 0)
        {
            int index = list_name.BinarySearch(name);
            if (index < 0) return null;// 键不存在
            List<List<object>> strs = list_strs[index];
            if (y >= strs.Count) return null;// 错误的行号
            else if (x < 0 || x >= strs[(int)y].Count) return null;// 错误的下标
            else if (strs[(int)y][(int)x] == null) return null;
            else if (strs[(int)y][(int)x].Equals("")) return null;
            return strs[(int)y][(int)x];
        }
        public void Set(string name, object value, double y = 0, double x = 0)
        {
            int index = list_name.BinarySearch(name);
            if (index < 0) { list_name.Insert(~index, name); list_strs.Insert(~index, new List<List<object>>()); index = ~index; }// 二分插入
            List<List<object>> strs = list_strs[index];
            while (strs.Count <= y) strs.Add(new List<object>());
            while (strs[(int)y].Count <= x) strs[(int)y].Add(null);
            strs[(int)y][(int)x] = value;
        }
        /// <summary>
        /// // 取得某名称共有多少行
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int Count(string name)
        {
            int index = list_name.BinarySearch(name);
            if (index < 0) return 0;
            return list_strs[index].Count;
        }
        public string GetString(string name, double y = 0, double x = 0)
        {
            object src = Get(name, y, x); if (src == null) return null;
            object returns = null;
            try { returns = Convert.ToString(src); } catch (Exception e) { Log("Convert Error " + src.ToString() + " to string \n" + e.Message); }
            return (string)returns;
        }
        public int GetInt(string name, double y = 0, double x = 0)
        {
            object src = Get(name, y, x); if (src == null) return 0;
            object returns = 0;
            try { returns = Convert.ToInt32(src); } catch (Exception e) { Log("Convert Error " + src.ToString() + " to int \n" + e.Message); }
            return (int)returns;
        }
        public float GetSingle(string name, double y = 0, double x = 0)
        {
            object src = Get(name, y, x);
            if (src == null) return 0f;
            try { return Convert.ToSingle(src); }
            catch (Exception e)
            { Log("Convert Error " + src.ToString() + " to float \n" + e.Message); return 0f; }
        }
        public double GetDouble(string name, double y = 0, double x = 0)
        {
            object src = Get(name, y, x);
            if (src == null) return 0d;
            try { return Convert.ToDouble(src); }
            catch (Exception e)
            { Log("Convert Error " + src.ToString() + " to double \n" + e.Message); return 0d; }
        }
        public bool GetBool(string name, double y = 0, double x = 0)
        {
            object src = Get(name, y, x); if (src == null) return false;
            object returns = false;
            try { returns = Convert.ToBoolean(src); } catch (Exception e) { Log("Convert Error " + src.ToString() + " to bool \n" + e.Message); }
            return (bool)returns;
        }

        /// <summary>
        /// // 加密存入 Int
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <param name="key"></param>
        public void SetIntOfKey(string name, object value, double y = 0, double x = 0, int key = 0)
        {
            int use = Convert.ToInt32(value);
            this.Set(name, use ^ key, y, x);
        }
        /// <summary>
        /// // 加密读取 Int
        /// </summary>
        /// <param name="name"></param>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetIntOfKey(string name, double y = 0, double x = 0, int key = 0)
        {
            object returns = GetInt(name, y, x);
            if (returns == null) return null;
            return (int)returns ^ key;
        }

        public List<dynamic> LineGet(string name, double y)
        {
            int index = list_name.BinarySearch(name);
            if (index < 0) { Log(name + " get line " + y + "error not found"); return null; }
            List<List<object>> strs = list_strs[index];
            if (strs.Count <= y || y < 0) { Log(name + " get line " + y + "error index"); return null; }
            return strs[(int)y];
        }
        public void LineSet(string name, List<object> list, double y)
        {
            int index = list_name.BinarySearch(name);
            if (index < 0) { Log(name + " set line " + y + "error not found"); return; }
            List<List<object>> strs = list_strs[index];
            if (strs.Count <= y || y < 0) { Log(name + " set line " + y + "error index"); return; }
            List<object> list_new = new List<object>();
            try { for (int i = 0; ; i++) list_new.Add(list[i]); }
            catch { strs[(int)y] = list_new; }
        }
        public void LineRemove(string name, double y)
        {
            int index = list_name.BinarySearch(name);
            if (index < 0) { Log(name + " remove line " + y + "error not found"); return; }
            List<List<object>> strs = list_strs[index];
            if (strs.Count <= y || y < 0) { Log(name + " remove line " + y + "error index"); return; }
            strs.RemoveAt((int)y);
        }
    }
}