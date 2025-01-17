using System.Collections.Generic;

namespace Cleavcats
{
    public class GameControlerManager
    {
        public List<Controler> list_obj = new List<Controler>();
        /// <summary>
        /// // 取得当前使用的 控制器
        /// </summary>
        /// <returns></returns>
        public Controler Get() { if (list_obj.Count <= 0) return null; else return list_obj[list_obj.Count - 1]; }

        /// <summary>
        /// // 在列表中添加一个新的控制器，使用该控制器
        /// </summary>
        /// <param name="p"></param>
        public void Add(Controler p, params object[] args) { if (list_obj.Count > 0) list_obj[list_obj.Count - 1].PauseOff(); list_obj.Add(p); p.Start(args); p.PauseOn(); }
        /// <summary>
        /// // 移除当前控制器，或者移除指定的控制器
        /// </summary>
        /// <param name="p"></param>
        public void Remove(Controler p = null)
        {
            if (p == null) { p = this.Get(); if (p == null) return; }
            p.PauseOff();
            p.Over();
            list_obj.Remove(p);
            if (list_obj.Count > 0) list_obj[list_obj.Count - 1].PauseOn();
        }
        /// <summary>
        /// // 移除全部控制器，在难以判断控制器前后关系时使用，注意此时 Get 将会返回Null。
        /// </summary>
        public void RemoveAll() { while (list_obj.Count > 0) { list_obj[0].Over(); list_obj.RemoveAt(0); } }
    }
    public class Controler
    {
        /// <summary>
        /// // 控制器 装载
        /// </summary>
        /// <param name="args"></param>
        public virtual void Start(params object[] args) { }
        /// <summary>
        /// // 控制器取得焦点
        /// </summary>
        public virtual void PauseOn() { }
        /// <summary>
        /// // 控制器 失去焦点
        /// </summary>
        public virtual void PauseOff() { }
        /// <summary>
        /// // 控制器 卸载
        /// </summary>
        public virtual void Over() { }
    }
}