using CCsGDFrame;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;

namespace Demo
{
    public class GamingData : Cleavcats.DataGroup
    {
        public static GamingData ptr;
        public GamingData() : base("GamingData") { ptr = this; }
        public bool is_created = false;
        public List<List<int>> map = new List<List<int>>() { };
        public int block_x, block_y;
        public override void Clear() { is_created = false; }
        public override void SaveTo(Stream stream)
        {
            base.SaveTo(stream);
            Cleavcats.DataFactory.TypeStream.BoolWrite(stream, is_created);
            Cleavcats.DataFactory.TypeStream.IntWrite(stream, map.Count);
            for (int x = 0; x < map.Count; x++)
            {
                Cleavcats.DataFactory.TypeStream.IntWrite(stream, map[x].Count);
                for (int y = 0; y < map[x].Count; y++) Cleavcats.DataFactory.TypeStream.IntWrite(stream, map[x][y]);
            }
            Cleavcats.DataFactory.TypeStream.IntWrite(stream, block_x);
            Cleavcats.DataFactory.TypeStream.IntWrite(stream, block_y);
        }
        public override void ReadFrom(Stream stream)
        {
            base.ReadFrom(stream);
            is_created = Cleavcats.DataFactory.TypeStream.BoolRead(stream);
            int x_count = Cleavcats.DataFactory.TypeStream.IntRead(stream);
            map.Clear();
            for (int x = 0; x < x_count; x++)
            {
                List<int> y_list = new List<int>();
                map.Add(y_list);
                int y_count = Cleavcats.DataFactory.TypeStream.IntRead(stream);
                for (int y = 0; y < y_count; y++)
                    y_list.Add(Cleavcats.DataFactory.TypeStream.IntRead(stream));
            }
            block_x = Cleavcats.DataFactory.TypeStream.IntRead(stream);
            block_y = Cleavcats.DataFactory.TypeStream.IntRead(stream);
        }
    }
    public class Gaming : Godot_SceneNode
    {
        class GamingKeybordControler : KeyBoardControler
        {
            Gaming comfrom;
            public override void Start(params object[] args) { comfrom = (Gaming)args[0]; }
            public override void OnWDown() { comfrom.Map_Move(0, -1); }
            public override void OnSDown() { comfrom.Map_Move(0, 1); }
            public override void OnADown() { comfrom.Map_Move(-1, 0); }
            public override void OnDDown() { comfrom.Map_Move(1, 0); }
        }
        GamingKeybordControler keyboard = new GamingKeybordControler();
        public override void _Ready()
        {
            this.FindNode("Save").Connect("pressed", this, "When_Save");
            GameSystem.ui_controler.UI_Add(this.FindNode("UI_Win") as Control);
            if (GamingData.ptr.is_created) Map_Redraw();
            else Map_Create();
            this.Translate();
            GameSystem.voice_controler.Bgm1.Play("hope.mp3");
            GameSystem.keybord_controler.Add(keyboard, this);
        }
        public override void _ExitTree()
        {
            GameSystem.voice_controler.Bgm1.Stop();
            GameSystem.keybord_controler.Remove(keyboard);
        }
        void Translate()
        {
            GameSystem.translate.Translate(this, "Save", "ui", "save");
            GameSystem.translate.Translate(this, "Game_info", "ui", "gaming_info");
        }
        void When_Save() { GameSystem.GameSave("save_0"); GameSystem.voice_controler.SE_Play("Modern9.mp3"); }

        void Map_Create()
        {
            GamingData.ptr.is_created = true;
           Random rand = new Random();
            List<int> img_pool = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
            GamingData.ptr.map.Clear();
            for (int x = 0; x < 3; x++)
            {
                GamingData.ptr.map.Add(new List<int>() { 0, 0, 0 });
                for (int y = 0; y < 3; y++)
                {
                    GamingData.ptr.map[x][y] = img_pool[rand.Next() % img_pool.Count];
                    img_pool.Remove(GamingData.ptr.map[x][y]);
                    if (GamingData.ptr.map[x][y] == 0) { GamingData.ptr.block_x = x; GamingData.ptr.block_y = y; }
                }
            }

            Map_Redraw();
        }
        void Map_Move(int x, int y)
        {
            bool move_enable = true;
            if (x > 0 && GamingData.ptr.block_x == 0) move_enable = false;
            if (x < 0 && GamingData.ptr.block_x == 2) move_enable = false;
            if (y > 0 && GamingData.ptr.block_y == 0) move_enable = false;
            if (y < 0 && GamingData.ptr.block_y == 2) move_enable = false;
            if (move_enable == false) { GameSystem.voice_controler.SE_Play("Modern5.mp3", 0.08f); return; }
            GameSystem.voice_controler.SE_Play("Modern5.mp3");
            //
            int target_x = GamingData.ptr.block_x - x;
            int target_y = GamingData.ptr.block_y - y;
            GamingData.ptr.map[GamingData.ptr.block_x][GamingData.ptr.block_y] =
                GamingData.ptr.map[target_x][target_y];
            GamingData.ptr.map[target_x][target_y] = 0;
            GamingData.ptr.block_x = target_x;
            GamingData.ptr.block_y = target_y;
            Map_Redraw();
            if (GameWinCheck()) { GameSystem.ui_controler.UI_Open("UI_Win"); }
        }
        Texture Map_imgGet(int img_id)
        {
            return ResourceLoader.Load<AtlasTexture>("res://Scene/Gaming/map" + img_id + ".tres");
        }
        void Map_Redraw()
        {
            for (int x = 0; x < GamingData.ptr.map.Count; x++)
            {
                for (int y = 0; y < GamingData.ptr.map[x].Count; y++)
                {
                    if (GamingData.ptr.map[x][y] == 0) { this.GetNode<TextureRect>("map/" + x + "," + y).Texture = null; }
                    else this.GetNode<TextureRect>("map/" + x + "," + y).Texture = Map_imgGet(GamingData.ptr.map[x][y]);
                }
            }
        }

        bool GameWinCheck() 
        {
            if (GamingData.ptr.map[0][0] != 0) return false;
            if (GamingData.ptr.map[1][0] != 1) return false;
            if (GamingData.ptr.map[2][0] != 2) return false;
            if (GamingData.ptr.map[0][1] != 3) return false;
            if (GamingData.ptr.map[1][1] != 4) return false;
            if (GamingData.ptr.map[2][1] != 5) return false;
            if (GamingData.ptr.map[0][2] != 6) return false;
            if (GamingData.ptr.map[1][2] != 7) return false;
            if (GamingData.ptr.map[2][2] != 8) return false;
            return true;
        }
    }
}