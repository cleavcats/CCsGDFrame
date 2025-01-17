using Godot;
using System.Collections.Generic;


namespace CCsGDFrame
{
    /// <summary>
    /// // 重载过后的，适用于Godot 的 键盘 控制器
    /// </summary>
    public class KeyBoardControler : Cleavcats.Controler
    {
        public virtual void Update() { }
        // down
        public virtual void OnWDown() { }
        public virtual void OnSDown() { }
        public virtual void OnADown() { }
        public virtual void OnDDown() { }
        public virtual void OnQDown() { }
        public virtual void OnEDown() { }
        public virtual void OnFDown() { }
        public virtual void OnJDown() { }
        public virtual void OnKDown() { }
        public virtual void OnUDown() { }
        public virtual void OnIDown() { }
        public virtual void OnLDown() { }
        public virtual void OnSpaceDown() { }
        public virtual void OnEnterDown() { }
        public virtual void OnShiftDown() { }
        public virtual void OnTabDown() { }
        public virtual void OnEscDown() { }
        public virtual void OnMouseLeftDown() { }
        public virtual void OnMouseRightDown() { }
        public virtual void OnMouseMiddleDown() { }
        public virtual void OnMouse4Down() { }
        public virtual void OnMouse5Down() { }
        // up
        public virtual void OnWUp() { }
        public virtual void OnSUp() { }
        public virtual void OnAUp() { }
        public virtual void OnDUp() { }
        public virtual void OnQUp() { }
        public virtual void OnEUp() { }
        public virtual void OnFUp() { }
        public virtual void OnJUp() { }
        public virtual void OnKUp() { }
        public virtual void OnUUp() { }
        public virtual void OnIUp() { }
        public virtual void OnLUp() { }
        public virtual void OnSpaceUp() { }
        public virtual void OnEnterUp() { }
        public virtual void OnShiftUp() { }
        public virtual void OnTabUp() { }
        public virtual void OnEscUp() { }
        public virtual void OnMouseLeftUp() { }
        public virtual void OnMouseRightUp() { }
        public virtual void OnMouseMiddleUp() { }
        public virtual void OnMouse4Up() { }
        public virtual void OnMouse5Up() { }
        // hold
        public virtual void OnWHold() { }
        public virtual void OnSHold() { }
        public virtual void OnAHold() { }
        public virtual void OnDHold() { }
        public virtual void OnQHold() { }
        public virtual void OnEHold() { }
        public virtual void OnFHold() { }
        public virtual void OnJHold() { }
        public virtual void OnKHold() { }
        public virtual void OnUHold() { }
        public virtual void OnIHold() { }
        public virtual void OnLHold() { }
        public virtual void OnSpaceHold() { }
        public virtual void OnEnterHold() { }
        public virtual void OnShiftHold() { }
        public virtual void OnTabHold() { }
        public virtual void OnEscHold() { }
        public virtual void OnMouseLeftHold() { }
        public virtual void OnMouseRightHold() { }
        public virtual void OnMouseMiddleHold() { }
        public virtual void OnMouse4Hold() { }
        public virtual void OnMouse5Hold() { }
        //鼠标滚动
        public virtual void OnMouseScrollUp() { }
        public virtual void OnMouseScrollDown() { }
    }
    /// <summary>
    /// // 继承此节点来监听键盘输入
    /// <br>// 重载了 _Input 与 _Process，继承时需注意保留之</br>
    /// </summary>
    public class KeyBoardControler_Listener : Node
    {
        public Cleavcats.GameControlerManager controler = new Cleavcats.GameControlerManager();
        bool w, s, a, d, q, e, f, j, k, space, enter, shift, tab, esc, mouse_left, mouse_right, mouse_middle, mouse_4, mouse_5;
        public override void _Input(InputEvent _data)
        {
            if (controler == null) { KeyDownStateClear(); return; }
            if (_data is InputEventKey)
            {
                InputEventKey data = (InputEventKey)_data;
                if (data.Pressed)
                {
                    if (data.Echo) return;
                    if (data.Scancode == (int)KeyList.W) { ((KeyBoardControler)controler.Get()).OnWDown(); w = true; }
                    else if (data.Scancode == (int)KeyList.S) { ((KeyBoardControler)controler.Get()).OnSDown(); s = true; }
                    else if (data.Scancode == (int)KeyList.A) { ((KeyBoardControler)controler.Get()).OnADown(); a = true; }
                    else if (data.Scancode == (int)KeyList.D) { ((KeyBoardControler)controler.Get()).OnDDown(); d = true; }
                    else if (data.Scancode == (int)KeyList.Q) { ((KeyBoardControler)controler.Get()).OnQDown(); q = true; }
                    else if (data.Scancode == (int)KeyList.E) { ((KeyBoardControler)controler.Get()).OnEDown(); e = true; }
                    else if (data.Scancode == (int)KeyList.F) { ((KeyBoardControler)controler.Get()).OnFDown(); f = true; }
                    else if (data.Scancode == (int)KeyList.J) { ((KeyBoardControler)controler.Get()).OnJDown(); j = true; }
                    else if (data.Scancode == (int)KeyList.K) { ((KeyBoardControler)controler.Get()).OnKDown(); k = true; }
                    else if (data.Scancode == (int)KeyList.Space) { ((KeyBoardControler)controler.Get()).OnSpaceDown(); space = true; }
                    else if (data.Scancode == (int)KeyList.Enter) { ((KeyBoardControler)controler.Get()).OnEnterDown(); enter = true; }
                    else if (data.Scancode == (int)KeyList.KpEnter) { ((KeyBoardControler)controler.Get()).OnEnterDown(); enter = true; }
                    else if (data.Scancode == (int)KeyList.Shift) { ((KeyBoardControler)controler.Get()).OnShiftDown(); shift = true; }
                    else if (data.Scancode == (int)KeyList.Tab) { ((KeyBoardControler)controler.Get()).OnTabDown(); tab = true; }
                    else if (data.Scancode == (int)KeyList.Escape) { ((KeyBoardControler)controler.Get()).OnEscDown(); esc = true; }
                    else return;
                }
                else
                {
                    if (data.Scancode == (int)KeyList.W) { ((KeyBoardControler)controler.Get()).OnWUp(); w = false; }
                    else if (data.Scancode == (int)KeyList.S) { ((KeyBoardControler)controler.Get()).OnSUp(); s = false; }
                    else if (data.Scancode == (int)KeyList.A) { ((KeyBoardControler)controler.Get()).OnAUp(); a = false; }
                    else if (data.Scancode == (int)KeyList.D) { ((KeyBoardControler)controler.Get()).OnDUp(); d = false; }
                    else if (data.Scancode == (int)KeyList.Q) { ((KeyBoardControler)controler.Get()).OnQUp(); q = false; }
                    else if (data.Scancode == (int)KeyList.E) { ((KeyBoardControler)controler.Get()).OnEUp(); e = false; }
                    else if (data.Scancode == (int)KeyList.F) { ((KeyBoardControler)controler.Get()).OnFUp(); f = false; }
                    else if (data.Scancode == (int)KeyList.J) { ((KeyBoardControler)controler.Get()).OnJUp(); j = false; }
                    else if (data.Scancode == (int)KeyList.K) { ((KeyBoardControler)controler.Get()).OnKUp(); k = false; }
                    else if (data.Scancode == (int)KeyList.Space) { ((KeyBoardControler)controler.Get()).OnSpaceUp(); space = false; }
                    else if (data.Scancode == (int)KeyList.Enter) { ((KeyBoardControler)controler.Get()).OnEnterUp(); enter = false; }
                    else if (data.Scancode == (int)KeyList.KpEnter) { ((KeyBoardControler)controler.Get()).OnEnterUp(); enter = false; }
                    else if (data.Scancode == (int)KeyList.Shift) { ((KeyBoardControler)controler.Get()).OnShiftUp(); shift = false; }
                    else if (data.Scancode == (int)KeyList.Tab) { ((KeyBoardControler)controler.Get()).OnTabUp(); tab = false; }
                    else if (data.Scancode == (int)KeyList.Escape) { ((KeyBoardControler)controler.Get()).OnEscUp(); esc = false; }
                    else return;
                }
            }
            if (_data is InputEventMouse)
            {
                if (controler == null) { KeyDownStateClear(); return; }
                int mouse_mask = ((InputEventMouse)_data).ButtonMask;
                if ((mouse_mask & (int)ButtonList.MaskLeft) > 0)
                { if (mouse_left == false) { ((KeyBoardControler)controler.Get()).OnMouseLeftDown(); mouse_left = true; } }
                else
                { if (mouse_left) { ((KeyBoardControler)controler.Get()).OnMouseLeftUp(); mouse_left = false; } }
                if ((mouse_mask & (int)ButtonList.MaskRight) > 0)
                { if (mouse_right == false) { ((KeyBoardControler)controler.Get()).OnMouseRightDown(); mouse_right = true; } }
                else
                { if (mouse_right) { ((KeyBoardControler)controler.Get()).OnMouseRightUp(); mouse_right = false; } }
                if ((mouse_mask & (int)ButtonList.MaskMiddle) > 0)
                { if (mouse_middle == false) { ((KeyBoardControler)controler.Get()).OnMouseMiddleDown(); mouse_middle = true; } }
                else
                { if (mouse_middle) { ((KeyBoardControler)controler.Get()).OnMouseMiddleUp(); mouse_middle = false; } }
                if ((mouse_mask & (int)ButtonList.MaskXbutton1) > 0)
                { if (mouse_4 == false) { ((KeyBoardControler)controler.Get()).OnMouse4Down(); mouse_4 = true; } }
                else
                { if (mouse_4) { ((KeyBoardControler)controler.Get()).OnMouse4Up(); mouse_4 = false; } }
                if ((mouse_mask & (int)ButtonList.MaskXbutton2) > 0)
                { if (mouse_5 == false) { ((KeyBoardControler)controler.Get()).OnMouse5Down(); mouse_5 = true; } }
                else
                { if (mouse_5) { ((KeyBoardControler)controler.Get()).OnMouse5Up(); mouse_5 = false; } }
            }
            else
            {
                return;
            }
        }
        public override void _Process/* 是否持续 Hold */(float delta)
        {
            if (controler == null) { return; }
            if (OS.IsWindowFocused() == false) { _HoldBreak(); return; }
            KeyBoardControler p = (KeyBoardControler)((KeyBoardControler)controler.Get());
            if (p == null) { KeyDownStateClear(); return; }
            if (w) { p.OnWHold(); }
            if (s) { p.OnSHold(); }
            if (a) { p.OnAHold(); }
            if (d) { p.OnDHold(); }
            if (q) { p.OnQHold(); }
            if (e) { p.OnEHold(); }
            if (f) { p.OnFHold(); }
            if (j) { p.OnJHold(); }
            if (k) { p.OnKHold(); }
            if (space) { p.OnSpaceHold(); }
            if (enter) { p.OnEnterHold(); }
            if (shift) { p.OnShiftHold(); }
            if (tab) { p.OnTabHold(); }
            if (esc) { p.OnEscHold(); }
            if (mouse_left) { p.OnMouseLeftHold(); }
            if (mouse_right) { p.OnMouseRightHold(); }
            if (mouse_middle) { p.OnMouseMiddleHold(); }
            if (mouse_4) { p.OnMouse4Hold(); }
            if (mouse_5) { p.OnMouse5Hold(); }
            //
            p.Update();
        }
        public void _HoldBreak/* 主动使得该场景的持续按下事件中断 */()
        {
            KeyBoardControler p = (KeyBoardControler)((KeyBoardControler)controler.Get());
            if (p == null) { KeyDownStateClear(); return; }
            if (w) { p.OnWUp(); w = false; }
            if (s) { p.OnSUp(); s = false; }
            if (a) { p.OnAUp(); a = false; }
            if (d) { p.OnDUp(); d = false; }
            if (q) { p.OnQUp(); q = false; }
            if (e) { p.OnEUp(); e = false; }
            if (f) { p.OnFUp(); f = false; }
            if (j) { p.OnJUp(); j = false; }
            if (k) { p.OnKUp(); k = false; }
            if (space) { p.OnSpaceUp(); space = false; }
            if (enter) { p.OnEnterUp(); enter = false; }
            if (shift) { p.OnShiftUp(); shift = false; }
            if (tab) { p.OnTabUp(); tab = false; }
            if (esc) { p.OnEscUp(); esc = false; }
            if (mouse_left) { p.OnEscUp(); mouse_left = false; }
            if (mouse_right) { p.OnEscUp(); mouse_right = false; }
            if (mouse_middle) { p.OnEscUp(); mouse_middle = false; }
            if (mouse_4) { p.OnEscUp(); mouse_4 = false; }
            if (mouse_5) { p.OnEscUp(); mouse_5 = false; }
        }
        public void KeyDownStateClear()
        {
            w = false; s = false; a = false; d = false; q = false; e = false; f = false; j = false; k = false; space = false; enter = false; shift = false; tab = false; esc = false;
            mouse_left = false; mouse_right = false; mouse_middle = false; mouse_4 = false; mouse_5 = false;
        }
    }
}