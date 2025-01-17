using Godot;
using System;
using System.Threading;
using Cleavcats;

namespace CCsGDFrame
{
    /// <summary>
    /// // GameScene 系统的入口，需要挂载到游戏主场景上
    /// </summary>
    public class _RootNode : Node
    {
        [Export(PropertyHint.PlaceholderText, "主界面场景路径")]
        public string StartScene = "";
        [Export(PropertyHint.PlaceholderText, "默认语言目录，需要带有末尾的 / ")]
        public string LanguageDir = null;
        [Export(PropertyHint.PlaceholderText, "默认语言")]
        public string LanguageDefault = null;

        [Export(PropertyHint.MultilineText, "场景从此处寻找")]
        public string Path_Scene = "res://Scene/";// 场景从此处寻找
        [Export(PropertyHint.MultilineText, "音频从此处寻找")]
        public string Path_Voice = "res://Voice/";// 音频从此处寻找

        Button button_debug;
        public override void _Ready()
        {
            GameSystem.Init(this);
            button_debug = this.GetNode<Button>("debug");
            button_debug.Connect("pressed", this, "when_button_debug");
            if (GameSystem.is_debug == false) ((Control)this.FindNode("debug")).Visible = false;
            if (StartScene != "")
            {
                GameSystem.scene_system.SceneJumpTo(StartScene);
            }
            else
            {
                GD.PrintErr("起始场景不存在，需要在 根节点的 StartScene 填写一个场景名");
                GetTree().Quit();
            }
        }
        void when_button_debug()
        {
            UI_Debug ui_scene= (UI_Debug)GameSystem.ui_controler.UI_Get("Debug");
            ui_scene.UI_debug_button = this.button_debug;
            this.button_debug.Visible = false;
            GameSystem.ui_controler.UI_Open("Debug");
        }
    }
}