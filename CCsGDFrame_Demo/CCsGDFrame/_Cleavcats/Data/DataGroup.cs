using Godot;
using CCsGDFrame;
using System;
using System.Collections.Generic;
using System.IO;

namespace Cleavcats
{
    /// <summary>
    /// // 继承自 该类型的 子类必须存在 一个 无参数的构造函数，否则不能读取存档
    /// </summary>
    public class DataGroup
    {
        public string name { get; protected set; }
        public DataGroup(string name) { this.name = name; }
        public DataGroup() { }

        static SortedDictionary<string, Type> pool_type = new SortedDictionary<string, Type>();
        static DataGroup()
        {
            foreach (Type founded in TypeSearch.FindAllChildType(typeof(DataGroup)))
                pool_type.Add(founded.Name, founded);
        }
        public static DataGroup CreateByTypeName(string type_name)
        {
            Type type;
            if (pool_type.TryGetValue(type_name, out type)) return (DataGroup)Activator.CreateInstance(type);
            return null;
        }

        SortedDictionary<string, object> pool = new SortedDictionary<string, object>();
        public object this[string key]
        {
            set { this.Set(key, value); }
            get { return this.Get(key); }
        }
        public void Set(string key, object value)
        {
            if (pool.ContainsKey(key)) pool[key] = value;
            else pool.Add(key, value);
        }
        public T Get<T>(string key)
        {
            object returns;
            pool.TryGetValue(key, out returns);
            if (returns is T) return (T)returns;
            throw new Exception("DataSystem：" + this.name + "/" + key + "：不受支持的类型，必须为 int short flaot double string 其中之一");
        }
        public object Get(string key)
        {
            object returns;
            pool.TryGetValue(key, out returns);
            return returns;
        }
        public void Remove(string key) { pool.Remove(key); }
        public virtual void Clear() { pool.Clear(); }

        enum SaveType
        {
            Int32, Short, Single, Double, String
        }
        public virtual void SaveTo(Stream stream)
        {
            DataFactory.TypeStream.StringWrite(stream, this.GetType().Name);
            DataFactory.TypeStream.StringWrite(stream, this.name);
            DataFactory.TypeStream.IntWrite(stream, pool.Count);
            foreach (string key in pool.Keys)
            {
                DataFactory.TypeStream.StringWrite(stream, key);
                object data = pool[key];
                SaveType type;
                if (data is Int32) type = SaveType.Int32;
                else if (data is short) type = SaveType.Short;
                else if (data is float) type = SaveType.Single;
                else if (data is double) type = SaveType.Double;
                else if (data is string) type = SaveType.String;
                else { throw new Exception("数据：" + this.name + "/" + key + "  不是自动保存能够识别的类型，只能存入int short flaot double string，考虑手动在 SaveTo 中自定义保存方式"); }
                DataFactory.TypeStream.IntWrite(stream, (int)type);
                switch (type)
                {
                    case SaveType.Int32: DataFactory.TypeStream.IntWrite(stream, (int)data); break;
                    case SaveType.Short: DataFactory.TypeStream.ShortWrite(stream, (short)data); break;
                    case SaveType.Single: DataFactory.TypeStream.FloatWrite(stream, (float)data); break;
                    case SaveType.Double: DataFactory.TypeStream.DoubleWrite(stream, (double)data); break;
                    case SaveType.String: DataFactory.TypeStream.StringWrite(stream, (string)data); break;
                }
            }
        }
        public virtual void ReadFrom(Stream stream)
        {
            this.name = DataFactory.TypeStream.StringRead(stream);
            int count = DataFactory.TypeStream.IntRead(stream);
            pool.Clear();
            for (int i = 0; i < count; i++)
            {
                string data_name = DataFactory.TypeStream.StringRead(stream);
                SaveType data_type = (SaveType)DataFactory.TypeStream.IntRead(stream);
                object data;
                switch (data_type)
                {
                    case SaveType.Int32: data = DataFactory.TypeStream.IntRead(stream); break;
                    case SaveType.Short: data = DataFactory.TypeStream.ShortRead(stream); break;
                    case SaveType.Single: data = DataFactory.TypeStream.FloatRead(stream); break;
                    case SaveType.Double: data = DataFactory.TypeStream.DoubleRead(stream); break;
                    case SaveType.String: data = DataFactory.TypeStream.StringRead(stream); break;
                    default: throw new Exception("不能识别的变量格式！" + this.name + "/" + data_name);
                }
                pool.Add(data_name, data);
            }
        }
        public static DataGroup CreateByStream(Stream stream)
        {
            string type_name= DataFactory.TypeStream.StringRead(stream);
            DataGroup returns = CreateByTypeName(type_name);
            returns.ReadFrom(stream);
            return returns;
        }
    }
}