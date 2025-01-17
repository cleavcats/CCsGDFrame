using Godot;
using System;
using System.IO;
using System.Collections.Generic;
using Cleavcats;

namespace CCsGDFrame
{
    /// <summary>
    /// <br>// 所有自定义场景应继承自此类</br>
    /// </summary>
    [Godot.Tool]
    public class Godot_SceneNode : Node
    {
        public Godot_SceneSystem scene_system = null;
        /// <summary>
        /// // 该场景的名，不要覆盖此项，只有系统自动设置此项才是安全的。
        /// </summary>
        public string Scene_name;
    }
}