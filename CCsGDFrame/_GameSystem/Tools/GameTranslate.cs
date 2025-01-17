using Godot;
using Cleavcats;

namespace CCsGDFrame
{
    public static partial class GameSystem
    {
        public class GameTranslate
        {
            public translate_Factory factory = new translate_Factory();
            public GameTranslate(string language_dir, string language_default)
            {
                factory.QuickInit(language_dir, language_default);
            }

            /// <summary>
            /// // 在目标节点下寻找子节点，将该子节点 的 Text 替换为目标 翻译条目
            /// <br>// 支持 Label、RichTextLabel、 Button、或者其他含有 Text 属性的节点</br>
            /// </summary>
            public void Translate(Node parent, string node_name, string book_name, string key, int index = 0, string language_name = null)
            {
                Node founded = parent.FindNode(node_name, true, false);
                if (founded == null) return;
                string data = factory.DataGet(book_name, key, index, language_name);
                if (founded is Label) ((Label)founded).Text = data;
                else if (founded is RichTextLabel)
                {
                    if (((RichTextLabel)founded).BbcodeEnabled)
                        ((RichTextLabel)founded).BbcodeText = data;
                    else ((RichTextLabel)founded).Text = data;
                }
                else if (founded is Button) ((Button)founded).Text = data;
                else if (founded is LinkButton) ((LinkButton)founded).Text = data;
                else if (founded is CheckButton) ((CheckButton)founded).Text = data;
                else if (founded is OptionButton) ((OptionButton)founded).Text = data;
                else if (founded is MenuButton) ((MenuButton)founded).Text = data;
                else if (founded is ToolButton) ((ToolButton)founded).Text = data;
                else founded.Set("Text", data);
            }
        }
    }
}