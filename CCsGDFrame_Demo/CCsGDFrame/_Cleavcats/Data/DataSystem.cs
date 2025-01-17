using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.IO;

namespace Cleavcats
{
    /// <summary>
    /// // 管理 数据库，以及提供了 存取 功能
    /// <br>// 创建、加载、保存之前，需要Add</br>
    /// </summary>
    public partial class DataSystem
    {
        public DataGroup _default = new DataGroup("_default");
        SortedDictionary<string, DataGroup> pool = new SortedDictionary<string, DataGroup>();

        public DataGroup this[string name]
        {
            set
            {
                DataGroup returns;
                if (pool.TryGetValue(name, out returns))
                {
                    if (value == null) pool.Remove(name);
                    else pool[name] = value;
                }
                if (value != null) pool.Add(name, value);
            }
            get
            {
                DataGroup returns;
                if (pool.TryGetValue(name, out returns)) return returns;
                returns = new DataGroup(name);
                pool.Add(name, returns);
                return returns;
            }
        }

        public void Add<T>(T obj) where T : DataGroup { pool.Add(obj.name, obj); }
        public T Get<T>() where T : DataGroup
        {
            foreach (DataGroup founded in pool.Values) if (founded is T) return founded as T;
            return null;
        }
        public T Get<T>(string name) where T : DataGroup
        {
            DataGroup returns;
            pool.TryGetValue(name, out returns);
            return returns as T;
        }
        public DataGroup Get(string name)
        {
            DataGroup returns;
            pool.TryGetValue(name, out returns);
            return returns;
        }
        public void Clear() { _default = new DataGroup();foreach(var founded in pool.Values) founded.Clear(); }

        /// <summary>
        /// // Save 与 Read 之前需要先 Add对应的 DataGroup 才能正常工作
        /// </summary>
        public void SaveTo(string path) { FileStream file = File.OpenWrite(path); SaveTo(file); file.Close(); }
        /// <summary>
        /// // Save 与 Read 之前需要先 Add对应的 DataGroup 才能正常工作
        /// </summary>
        public void ReadFrom(string path) { FileStream file = File.OpenRead(path); ReadFrom(file); file.Close(); }
        /// <summary>
        /// // Save 与 Read 之前需要先 Add对应的 DataGroup 才能正常工作
        /// </summary>
        public void SaveTo(Stream stream)
        {
            _default.SaveTo(stream);
            DataFactory.TypeStream.IntWrite(stream, pool.Count);
            foreach (var founded in pool.Values) founded.SaveTo(stream);
        }
        /// <summary>
        /// // Save 与 Read 之前需要先 Add对应的 DataGroup 才能正常工作
        /// </summary>
        public void ReadFrom(Stream stream)
        {
            _ = DataFactory.TypeStream.StringRead(stream);
            _default.ReadFrom(stream);
            pool.Clear();
            int count = DataFactory.TypeStream.IntRead(stream);
            for (int i = 0; i < count; i++)
            {
                DataGroup new_group = DataGroup.CreateByStream(stream);
                pool.Add(new_group.name, new_group);
            }
        }
    }
}