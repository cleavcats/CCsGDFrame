using Cleavcats;
using System.Collections.Generic;
using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace CCsGDFrame
{
    /// <summary>
    /// // 放置诸多子系统的 快捷入口
    /// </summary>
    public static partial class GameSystem
    {
        /// <summary>
        /// // 游戏是否处于 Debug 模式下
        /// </summary>
        public static bool is_debug = false;
        /// <summary>
        /// // 子系统初始化是否完成
        /// </summary>
        public static bool is_ready = false;
        public static void Log(object str)
        {
            if (is_debug) { System.Diagnostics.Debug.WriteLine(str.ToString()); }
            else { System.IO.File.AppendAllText("log.txt", str.ToString() + "\n"); }
        }
        /// <summary>
        /// // 需访问此处，来初始化各子系统
        /// </summary>
        public static void Init(_RootNode root)
        {
            if (OS.IsDebugBuild()/*编辑器内运行时（包括vscode debug），将工作路径导向至 runtime*/)
            {
                if (OS.HasFeature("editor"))
                {
                    System.IO.Directory.SetCurrentDirectory(System.IO.Directory.GetCurrentDirectory() + "/../" +
                        System.IO.Path.GetFileName(System.IO.Directory.GetCurrentDirectory()) + "_Runtime/");
                }
                System.Diagnostics.Debug.WriteLine("DebugPath= " + System.IO.Directory.GetCurrentDirectory());
                is_debug = true;
            }
            else { System.IO.File.Delete("log.txt"); }
            root_node = root;
            //
            if (is_debug) game_io = new Godot_IO_MulitFolder(System.IO.Directory.GetCurrentDirectory() + "/", "res://");
            else game_io = new Godot_IO_MulitFolder("res://");
            //
            GameSystem.ConfigRead();
            //
            data = new DataSystem();
            foreach (Type founded in TypeSearch.FindAllChildType(typeof(DataGroup)))
            {
                DataGroup obj = (DataGroup)System.Activator.CreateInstance(founded);
                data.Add(obj);
            }
            //
            KeyBoardControler_Listener keyborder_listener = new KeyBoardControler_Listener();
            root.AddChild(keyborder_listener);
            keybord_controler = keyborder_listener.controler;
            keybord_controler.Add(new KeyBoardControler());
            //
            Node node_voice = root.GetNodeOrNull("Voice") as Node;
            if (node_voice == null)
            {
                node_voice = new Node()
                {
                    Name = "Voice",
                };
                root.AddChild(node_voice);
            }
            voice_controler = new Godot_GameVoice(node_voice, root.Path_Voice);
            //
            Control node_scene = root.GetNodeOrNull("Scene") as Control;
            if (node_scene == null)
            {
                node_scene = new Control()
                {
                    Name = "Scene",
                    AnchorLeft = 0,
                    AnchorTop = 0,
                    AnchorRight = 1,
                    AnchorBottom = 1,
                    MarginLeft = 0,
                    MarginTop = 0,
                    MarginRight = 0,
                    MarginBottom = 0
                };
                root.AddChild(node_scene);
            }
            scene_system = new Godot_SceneSystem(node_scene, root.Path_Scene);
            //
            Control node_ui = root.GetNodeOrNull("UI") as Control;
            if (node_scene == null)
            {
                node_scene = new Control()
                {
                    Name = "UI",
                    AnchorLeft = 0,
                    AnchorTop = 0,
                    AnchorRight = 1,
                    AnchorBottom = 1,
                    MarginLeft = 0,
                    MarginTop = 0,
                    MarginRight = 0,
                    MarginBottom = 0
                };
                root.AddChild(node_scene);
            }
            ui_controler = new UI_Controler(node_ui);
            //
            translate = new GameTranslate(root.LanguageDir, root.LanguageDefault);
            //
            is_ready = true;
        }
        public static _RootNode root_node = null;

        public static Cleavcats.GameControlerManager keybord_controler;
        public static UI_Controler ui_controler = null;
        public static Godot_GameVoice voice_controler = null;
        public static Godot_SceneSystem scene_system = null;
        public static Godot_IO_MulitFolder game_io = null;
        public static GameTranslate translate = null;
        /// <summary>
        /// // 数据模块，用于存档
        /// <br>// 默认抓取所有 继承自 Cleavcats.DataGroup 的子类，自动创建一个实例并注入此模块</br>
        /// <br>// 如有需要则 修改 GameSystem.Init 中关于此变量的部分</br>
        /// </summary>
        public static DataSystem data = null;

        /// <summary>
        /// // 将存档保存为指定的名称
        /// <br>// 返回是否成功</br>
        /// </summary>
        public static bool GameSave(string save_name)
        {
            System.IO.FileStream file;
            System.IO.Directory.CreateDirectory("Save/");
            try { file = System.IO.File.Create("Save/" + save_name + ".savedata"); }
            catch { return false; }
            data._default.Set("_system_working_scene", scene_system.working_scene.Scene_name);
            data.SaveTo(file);
            file.Close();
            return true;
        }
        /// <summary>
        /// // 加载指定存档，并跳转到存档所处的场景
        /// </summary>
        public static bool GameRead(string save_name)
        {
            System.IO.FileStream file;
            try { file = System.IO.File.Open("Save/" + save_name + ".savedata", System.IO.FileMode.Open); }
            catch { return false; }
            data.ReadFrom(file);
            string 当前场景 = data._default.Get<string>("_system_working_scene");
            file.Close();
            //
            scene_system.SceneJumpTo(当前场景);
            return true;
        }
        /// <summary>
        /// // 存档初始化
        /// </summary>
        public static void GameCreate()
        {
            GameSystem.data.Clear();
        }
        /// <summary>
        ///  // 回到开始界面
        /// </summary>
        public static void GameRestore()
        {
            GameSystem.voice_controler.Bgm1.Stop();
            GameSystem.voice_controler.Bgm2.Stop();
            GameSystem.voice_controler.Bgm3.Stop();
            GameCreate();
            scene_system.SceneJumpTo(root_node.StartScene);
        }
        public static void GameQuit() { if (root_node.IsInsideTree()) root_node.GetTree().Quit(); }

        /// <summary>
        /// // 应用程序配置，从 运行目录下的 config.csv 存取
        /// </summary>
        public static CsvBook config = null;
        /// <summary>
        /// // 加载配置文件 : config.csv
        /// </summary>
        public static void ConfigRead()
        {
            config = new CsvBook();
            Csv config_file = Csv.ReadFromFile("config.csv");
            if (config_file == null) { GD.Print("加载系统配置 config.csv 失败！"); return; }
            config.ReadFromCsv(config_file);
        }
        /// <summary>
        /// // 保存配置文件：config.csv
        /// </summary>
        public static void ConfigSave()
        {
            config.Set("bgm_volume", voice_controler.Bgm_volume);
            config.Set("se_volume", voice_controler.SE_volume);
            Csv config_file = new Csv();
            config.SaveToCsv(config_file);
            try { config_file.SaveToFile("config.csv"); }
            catch { GD.Print("保存系统配置 config.csv 失败！"); }
        }
    }
}