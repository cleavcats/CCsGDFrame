using Godot;
using System.Collections.Generic;

namespace CCsGDFrame
{
    /// <summary>
    /// // 摘除 parent_node 其下的所有 Control 节点作为 ui，并通过 Open Close 来打开与关闭
    /// <br>// 若要感知 ui 的 关闭与 开启信号，使用该 ui 的 enter_tree 与 exit_tree 信号</br>
    /// </summary>
    public class UI_Controler
    {
        SortedDictionary<string, Control> pool = new SortedDictionary<string, Control>();
        Node parent_node;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent_node">// 所有UI 的父节点</param>
        public UI_Controler(Node parent_node)
        {
            while (parent_node.GetChildCount() > 0)
            {
                Node target = parent_node.GetChild(0);
                parent_node.RemoveChild(target);
                if (target is Control) pool.Add(target.Name, (Control)target);
            }
            this.parent_node = parent_node;
        }
        ~UI_Controler() { foreach (Control control in pool.Values) control.QueueFree(); }
        /// <summary>
        /// // 取得所有 UI 的名称
        /// </summary>
        /// <returns></returns>
        public string[] UI_Names()
        {
            List<string> returns = new List<string>();
            foreach (string key in pool.Keys) returns.Add(key);
            return returns.ToArray();
        }
        /// <summary>
        /// // 将一个 UI 从节点树中摘除，并加入到此处的 UI 列表中
        /// <br>// 如果同名UI 已存在于 列表中，则什么也不会发生</br>
        /// </summary>
        public void UI_Add(Control ui)
        {
            if (pool.ContainsKey(ui.Name)) return;
            if (ui.IsInsideTree()) ui.GetParent().RemoveChild(ui);
            pool.Add(ui.Name, ui);
        }
        /// <summary>
        /// // 从 UI 列表中移除 一个UI 且销毁他，如果他处于节点树中，也从节点树中移除
        /// </summary>
        public void UI_Remove(Control ui)
        {
            if (pool.ContainsKey(ui.Name) == false) return;
            pool.Remove(ui.Name);
            if (ui.IsInsideTree()) { ui.GetParent().RemoveChild(ui); }
            ui.QueueFree();
        }
        /// <summary>
        /// // 取得一个 UI 的引用
        /// </summary>
        public Control UI_Get(string name)
        {
            Control returns;
            if (pool.TryGetValue(name, out returns)) return returns;
            return null;
        }
        /// <summary>
        /// // 打开一个UI，触发其 EnterTree，layer 更高的图片将现实在上层
        /// <br>// 如果两个UI Layer 相同，则后来的UI将显示在上方</br>
        /// <br>// 如果 一个 UI 已经处于打开状态，则对其进行重新排序（如果图层发生改变则触发其 ExitTree 与 EnterTree）</br>
        /// </summary>
        public Control UI_Open(string name, int layer = 0)
        {
            string parent_name = layer.ToString();
            Node parent;
            Control returns;
            if (pool.TryGetValue(name, out returns))
            {
                if (returns.IsInsideTree())
                {
                    parent = returns.GetParent();
                    if (parent.Name == parent_name/*浮到最上层*/)
                    {
                        parent.MoveChild(returns, parent.GetChildCount() - 1);
                        returns.Visible = true;
                        return returns;
                    }
                    // 从原节点 摘除，找到新的 UI 图层
                    else
                    {
                        parent.RemoveChild(returns);
                        parent = UI_LayerGet(layer);
                    }
                    parent.AddChild(returns);
                    returns.Visible = true;
                    return returns;
                }
                parent = UI_LayerGet(layer);
                parent.AddChild(returns);
                returns.Visible = true;
                return returns;
            }
            return null;
        }
        Control UI_LayerGet(int layer)
        {
            Control returns = parent_node.FindNode(layer.ToString(), true, false) as Control;
            if (returns == null)
            {
                returns = new Control()
                {
                    Name = layer.ToString(),
                    MouseFilter=Control.MouseFilterEnum.Ignore,
                    AnchorLeft = 0,
                    AnchorTop = 0,
                    AnchorRight = 1,
                    AnchorBottom = 1,
                    MarginLeft = 0,
                    MarginTop = 0,
                    MarginRight = 0,
                    MarginBottom = 0
                };
                parent_node.AddChild(returns);
            }
            return returns;
        }
        /// <summary>
        /// // 从节点树中 摘除 一个 UI ，不销毁，触发其 ExitTree
        /// </summary>
        /// <param name="name"></param>
        public void UI_Close(string name)
        {
            Control returns;
            if (pool.TryGetValue(name, out returns)) if (parent_node.IsAParentOf(returns)) returns.GetParent().RemoveChild(returns);
        }
    }
}