using Godot;
using System;
using System.Collections.Generic;
using System.IO;

namespace CCsGDFrame
{
    /// <summary>
    /// // 场景系统的 系统模块入口
    /// <br>// 系统开始工作前需：</br>
    /// <br>// 设置 root_node（）</br>
    /// </summary>
    public class Godot_SceneSystem
    {
        /// <summary>
        /// // 场景挂载于此处，系统运行前必须设置之
        /// </summary>
        public Control parent_node;
        /// <summary>
        /// // 场景所使用的目录（带有/），其下的每一个 文件夹都被视为一个场景
        /// </summary>
        public string DataDir = null;
        /// <summary>
        /// // 目录需带有末尾的 /
        /// </summary>
        /// <param name="parent_node"></param>
        /// <param name="data_dir"></param>
        public Godot_SceneSystem(Control parent_node ,string data_dir) { this.parent_node = parent_node; this.DataDir = data_dir; }

        // 场景系统
        /// <summary>
        /// // // 当前正在渲染中的场景，的实例（Tool安全，场景内置控制台自动设置此变量）
        /// </summary>
        public Godot_SceneNode working_scene;
        /// <summary>
        /// // 读取目标场景并跳转至
        /// </summary>
        /// <param name="name"></param>
        public void SceneJumpTo(string name)
        {
            // 移除现有场景
            while (parent_node.GetChildCount() > 0)
            {
                Node delete_node = parent_node.GetChild(0);
                parent_node.RemoveChild(delete_node);
                delete_node.QueueFree();
            }
            // 加载新场景
            PackedScene new_scene = ResourceLoader.Load<PackedScene>(DataDir + name + "/" + name + ".tscn");
            working_scene = (Godot_SceneNode)new_scene.Instance();
            working_scene.scene_system = this;
            working_scene.Scene_name = name;
            System.Diagnostics.Debug.WriteLine("scene jump to " + name);
            parent_node.AddChild(working_scene);
        }
    }
}