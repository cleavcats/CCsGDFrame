using CCsGDFrame;
using Godot;
using System;

namespace Demo
{
    public class MainScene : Godot_SceneNode
    {
        public override void _Ready()
        {
            this.FindNode("LoadGame").Connect("pressed", this, "When_LoadGame");
            this.FindNode("NewGame").Connect("pressed", this, "When_NewGame");
            this.FindNode("ExitGame").Connect("pressed", this, "When_ExitGame");

            this.FindNode("language_zh_cn").Connect("pressed", this, "When_language_zh_cn");
            this.FindNode("language_en_us").Connect("pressed", this, "When_language_en_us");
            GameSystem.translate.factory.LoadAllSpace(GameSystem.root_node.LanguageDir + "en-us", "en-us");
            Translate();

            GameSystem.voice_controler.Bgm1.Play("frozen_winter.mp3");
        }
        public override void _ExitTree()
        {
            GameSystem.voice_controler.Bgm1.Stop();
        }
        void Translate()
        {
            GameSystem.translate.Translate(this, "LoadGame", "ui", "LoadGame");
            GameSystem.translate.Translate(this, "NewGame", "ui", "NewGame");
            GameSystem.translate.Translate(this, "ExitGame", "ui", "ExitGame");
        }

        void When_LoadGame() { GameSystem.GameRead("save_0"); }
        void When_NewGame() { GameSystem.GameCreate(); GameSystem.scene_system.SceneJumpTo("Gaming"); }
        void When_ExitGame() { GameSystem.GameQuit(); }
        void When_language_zh_cn() { GameSystem.translate.factory.language_using = "zh-cn"; Translate(); }
        void When_language_en_us() { GameSystem.translate.factory.language_using = "en-us"; Translate(); }

    }
}