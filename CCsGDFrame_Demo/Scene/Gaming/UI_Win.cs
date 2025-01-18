using CCsGDFrame;
using Godot;
using System;

namespace Demo
{
    public class UI_Win : ColorRect
    {
        KeyBoardControler keyboard = new KeyBoardControler();
        public override void _Ready()
        {
            this.FindNode("Win").Connect("pressed", this, "When_ButtonWin");
        }
        public override void _EnterTree()
        {
            // 用一个新的控制器 阻断 原本的键盘控制
            GameSystem.keybord_controler.Add(keyboard);
            Translate();
        }
        public override void _ExitTree()
        {
            GameSystem.keybord_controler.Remove(keyboard);
        }
        void Translate()
        {
            GameSystem.translate.Translate(this, "Win", "ui", "you win");
        }

        void When_ButtonWin() 
        {
            GameSystem.ui_controler.UI_Close(this.Name);
            GameSystem.scene_system.SceneJumpTo(GameSystem.root_node.StartScene); 
        }
    }
}