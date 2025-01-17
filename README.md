# CCsGDFrame
 一个应用于 Godot 的 C# 小型游戏框架，提供了 场景、UI、音频、存档、多语言翻译等功能的支持。
附带一个简单的拼图小游戏 Demo。
### 快速开始
<p>1.新建一个空白项目，将 CCsGDFrame 目录下的内容复制到项目的根目录中。
<p>2.将项目的起始场景设置为 res://CCsGDFrame/_GameSystem/Scene/root.tscn
<p>3.在godot编辑器中随便创建一个C#脚本以生成 C#解决方案。
<p>4.在 res://Scene/ 下创建你的游戏登录界面（场景名需要和子文件夹名称相同，如 res://Scene/MainScene/MainsScene.tscn），并将 root.tscn 的 StartScene 属性设置为 登录界面 的场景名（如：MainScene）（C#解决方案需要成功编译一次才能使用脚本中的 Export 属性）
<p>5.运行项目，场景将会自动跳转到 MainScene

### 场景系统：GameSystem.scene_system
...
