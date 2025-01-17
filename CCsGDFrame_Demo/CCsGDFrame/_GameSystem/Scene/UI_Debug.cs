
using Godot;
using System;
using Cleavcats;
using CCsGDFrame;
using System.Reflection;
using System.Collections.Generic;

namespace CCsGDFrame
{
    /// <summary>
    /// // 如果存在 UI_OnCreate 方法，则UI Ready时 执行之
    /// <br>// 所有自定义 的 When_ 开头的方法 将会作为按钮 在 单元测试面板上显示</br>
    /// </summary>
    public partial class UI_Debug : Control
    {
        Control debug_space;
        TextEdit input;
        RichTextLabel output;
        public override void _Ready()
        {
            debug_space = (Control)this.FindNode("debug_space");
            input = (TextEdit)this.FindNode("input");
            output = (RichTextLabel)this.FindNode("output");
            this.FindNode("button_clear").Connect("pressed", this, "UI_Clear");
            this.FindNode("button_close").Connect("pressed", this, "UI_Close");
            //
            while (debug_space.GetChildCount() > 0) { Node founded = debug_space.GetChild(0); debug_space.RemoveChild(founded); founded.QueueFree(); }
            foreach (MethodInfo founded in this.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
            {
                if (founded.Name.StartsWith("When_"))
                {
                    string button_name = founded.Name.Substring(5);
                    Button button_new = new Button() { Name = button_name, Text = button_name };
                    button_new.Connect("pressed", this, founded.Name);
                    debug_space.AddChild(button_new);
                }
            }
            if (this.HasMethod("UI_OnCreate")) this.Call("UI_OnCreate");
        }

        public Button UI_debug_button;
        KeyBoardControler keyboard_controler = new KeyBoardControler();
        public override void _EnterTree()
        {
            if (GameSystem.is_ready == false) return;
            GameSystem.keybord_controler.Add(keyboard_controler);
        }
        public override void _ExitTree()
        {
            if (GameSystem.is_ready == false) return;
            GameSystem.keybord_controler.Remove(keyboard_controler);
            if (UI_debug_button != null) UI_debug_button.Visible = true;
        }
        void UI_Clear() { output.Text = ""; }
        void UI_Close() { GameSystem.ui_controler.UI_Close(this.Name); }

        System.Text.StringBuilder out_put_builder = new System.Text.StringBuilder();
        void Log(object data) { out_put_builder.Append(data.ToString()); output.Text = out_put_builder.ToString(); }
        void LogLine(object data) { out_put_builder.AppendLine(data.ToString()); output.Text = out_put_builder.ToString(); }
        void LogClear() { out_put_builder.Clear(); output.Text = ""; }

        SortedDictionary<string, string> config_data = new SortedDictionary<string, string>();
        /// <summary>
        /// // 以 ini 文件的 格式获得配置 参数
        /// </summary>
        void ConfigReadFromCode(string code, SortedDictionary<string, string> target = null)
        {
            if (target == null) target = config_data;
            System.IO.StringReader reader = new System.IO.StringReader(code);
            string line = reader.ReadLine();
            while (line != null)
            {
                string[] splits = line.Split('=');
                if (splits.Length >= 2)
                {
                    if (target.ContainsKey(splits[0])) target[splits[0]] = splits[1];
                    else target.Add(splits[0], splits[1]);
                }
                line = reader.ReadLine();
            }
        }
        void ArgsGet_Innerr<T>(string src, out T returns)
        {
            switch (typeof(T).Name)
            {
                case "Boolen": returns = (T)(object)System.Convert.ToBoolean(src); break;
                case "Int16": returns = (T)(object)System.Convert.ToInt16(src); break;
                case "Int32": returns = (T)(object)System.Convert.ToInt32(src); break;
                case "Single": returns = (T)(object)System.Convert.ToSingle(src); break;
                case "Double": returns = (T)(object)System.Convert.ToDouble(src); break;
                case "String": returns = (T)(object)System.Convert.ToString(src); break;
                default: returns = default(T); return;
            }
        }
        /// <summary>
        /// // 取出参数 的快捷 方式
        /// </summary>
        void ArgsGet<T1>(string src, char split, out T1 arg1)
        {
            string[] splite_s = src.Split(split);
            ArgsGet_Innerr<T1>(splite_s[0], out arg1);
        }
        /// <summary>
        /// // 取出参数 的快捷 方式
        /// </summary>
        void ArgsGet<T, T1>(string src, char split, out T arg0, out T1 arg1)
        {
            string[] splite_s = src.Split(split);
            ArgsGet_Innerr<T>(splite_s[0], out arg0);
            ArgsGet_Innerr<T1>(splite_s[1], out arg1);
        }
        /// <summary>
        /// // 取出参数 的快捷 方式
        /// </summary>
        void ArgsGet<T, T1, T2>(string src, char split, out T arg0, out T1 arg1, out T2 arg2)
        {
            string[] splite_s = src.Split(split);
            ArgsGet_Innerr<T>(splite_s[0], out arg0);
            ArgsGet_Innerr<T1>(splite_s[1], out arg1);
            ArgsGet_Innerr<T2>(splite_s[2], out arg2);
        }
        /// <summary>
        /// // 取出参数 的快捷 方式
        /// </summary>
        void ArgsGet<T, T1, T2, T3>(string src, char split, out T arg0, out T1 arg1, out T2 arg2, out T3 arg3)
        {
            string[] splite_s = src.Split(split);
            ArgsGet_Innerr<T>(splite_s[0], out arg0);
            ArgsGet_Innerr<T1>(splite_s[1], out arg1);
            ArgsGet_Innerr<T2>(splite_s[2], out arg2);
            ArgsGet_Innerr<T3>(splite_s[3], out arg3);
        }
        /// <summary>
        /// // 取出参数 的快捷 方式
        /// </summary>
        void ArgsGet<T, T1, T2, T3, T4>(string src, char split, out T arg0, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4)
        {
            string[] splite_s = src.Split(split);
            ArgsGet_Innerr<T>(splite_s[0], out arg0);
            ArgsGet_Innerr<T1>(splite_s[1], out arg1);
            ArgsGet_Innerr<T2>(splite_s[2], out arg2);
            ArgsGet_Innerr<T3>(splite_s[3], out arg3);
            ArgsGet_Innerr<T4>(splite_s[4], out arg4);
        }
        /// <summary>
        /// // 取出参数 的快捷 方式
        /// </summary>
        void ArgsGet<T, T1, T2, T3, T4, T5>(string src, char split, out T arg0, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5)
        {
            string[] splite_s = src.Split(split);
            ArgsGet_Innerr<T>(splite_s[0], out arg0);
            ArgsGet_Innerr<T1>(splite_s[1], out arg1);
            ArgsGet_Innerr<T2>(splite_s[2], out arg2);
            ArgsGet_Innerr<T3>(splite_s[3], out arg3);
            ArgsGet_Innerr<T4>(splite_s[4], out arg4);
            ArgsGet_Innerr<T5>(splite_s[5], out arg5);
        }
        /// <summary>
        /// // 取出参数 的快捷 方式
        /// </summary>
        void ArgsGet<T, T1, T2, T3, T4, T5, T6>(string src, char split, out T arg0, out T1 arg1, out T2 arg2, out T3 arg3, out T4 arg4, out T5 arg5, out T6 arg6)
        {
            string[] splite_s = src.Split(split);
            ArgsGet_Innerr<T>(splite_s[0], out arg0);
            ArgsGet_Innerr<T1>(splite_s[1], out arg1);
            ArgsGet_Innerr<T2>(splite_s[2], out arg2);
            ArgsGet_Innerr<T3>(splite_s[3], out arg3);
            ArgsGet_Innerr<T4>(splite_s[4], out arg4);
            ArgsGet_Innerr<T5>(splite_s[5], out arg5);
            ArgsGet_Innerr<T6>(splite_s[6], out arg6);
        }
    }
}