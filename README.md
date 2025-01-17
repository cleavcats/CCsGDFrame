# CCsGDFrame
<br> 一个应用于 Godot 的 C# 小型游戏框架，提供了 场景、UI、音频、存档、多语言翻译等功能的支持。
附带一个简单的拼图小游戏 Demo。
### 注意事项
<br>必须创建一个与项目同名的 _Runtime文件夹，例：若项目路径为 D:/CCsGDFrame_Demo/，则需要创建一个 D:/CCsGDFrame_Demo_Runtime/ 文件夹，Debug时游戏的运行路径将会重新设置到此处，此举是为了模拟导出后的exe的运行环境，来尽可能的消除导出前后的运行环境差别，以及为开发过程中的调试提供真实运行环境。
### 快速开始
<br>1.新建一个空白项目，将 CCsGDFrame 目录下的内容复制到项目的根目录中，并创建与项目同名的Runtime文件夹。
<br>2.将项目的起始场景设置为 res://CCsGDFrame/_GameSystem/Scene/root.tscn
<br>3.在godot编辑器中随便创建一个C#脚本以生成 C#解决方案。
<br>4.在 res://Scene/ 下创建你的游戏登录界面（场景名需要和子文件夹名称相同，如 res://Scene/MainScene/MainsScene.tscn），并将 root.tscn 的 StartScene 属性设置为 登录界面 的场景名（如：MainScene）（C#解决方案需要成功编译一次才能使用脚本中的 Export 属性）
<br>5.运行项目，场景将会自动跳转到 MainScene

### 静态全局对象：CCsGDFrame.GameSystem
<br> 默认扫描整个项目中 继承自 Cleavcats.DataGroup 的类型，为他们各自创建一个实例并存入 GameSystem.data 中去，若想改变这一行为需要修改 GameSystem.Init() 方法。
<br>GameSave 将存档保存到运行路径的 Save/目录下的指定文件中
<br>GameRead从运行路径的 Save/目录下的指定文件中读取存档，并跳转到保存时所处的场景中去。
<br>GameCreate 执行所有存档数据 的 Clear 方法
<br>GameRestore 回到 root 场景中指定的那个游戏登录界面，并 Clear 存档。
<br>GameQuit 游戏退出。
<br> GameSystem.config 从运行路径下的 config.csv 自动读取的配置文件，用来存储与存档无关的数据如游戏的图形配置参数等，需要手动调用 GameSystem.ConfigSave 来保存。

### 场景模块：CCsGDFrame.GameSystem.scene_system
<br>调用 SceneJumpTo 来切换场景，场景只会同时存在一个，加载另一个场景前自动卸载原有的。
<br>场景的根节点所挂载的脚本必须继承自 CCsGDFrame.Godot_SceneNode。

### UI模块：CCsGDFrame.GameSystem.ui_controler
<br>UI_Add 将一个 Control 节点从节点树中摘除并存入此模块中
<br>UI_Open 来显示UI，可选参数 layer 可以指定此 UI 的显示图层，layer更大的图层显示在上方，两个UI的 layer相同时，后打开的那个会显示在更上方
<br> UI_Close 关闭一个UI并将它从节点树中摘除

### 音频模块：CCsGDFrame.GameSystem.voice_controler
<br>提供了3条bgm音轨：bgm1, bgm2, bgm3，每一条都可以单独设置音量，也可以通过 Bgm_volume 设置bgm总音量。
<br>SE_Play 播放音效，可以单独设置音量，也可以通过 SE_volume 设置全部音效的 总音量。

### 存档模块：CCsGDFrame.GameSystem.data
<br>Get() 取得指定的 Cleavcats.DataGroup 的实例，可以继承该类型来扩展存档的内容，框架将会扫描所有继承自 DataGroup的类型并在存档中创建一个实例，自定义的 DataGroup 需要继承 Clear，SaveTo，ReadFrom 方法来管理自己数据的存取。
<br>Clear() 初始化所有 DataGrop

### 多语言翻译模块：CCsGDFrame.GameSystem.translate
<br>模块自动将 root.tscn 上填入的 LanguageDir 与 LanguageDefault 作为默认语言导入（LanguageDir下的其他语言不会自动导入，这是为了节约内存空间，如有需要手动调用 LoadAllSpace 加载之）。
<br>LanguageDir/Language_name/下的所有 csv文件均视为翻译数据，读取时使用 书名(文件名去掉后缀的csv)+键名+行号（同一个键存在多行时），来提取对应的某条数据，提供多语言翻译时应该复制默认语言文件夹并重命名文件夹为新语言名称，再修改其下的csv文件内容即可。
<br>---------------------------------------------------------------------
<br>GameSystem.translate.factory.LoadAllSpace(string dir_path, string language) 扫描目标目录下的所有 csv 文件，将其作为目标语言导入。
<br>GameSystem.translate.factory.language_using 设置此项来更改当前所使用的语言，如果在该语言中没有找到 对应的键，则从默认语言中寻找。
<br>GameSystem.translate.Translate 寻找目标节点，并尝试将其 Text属性设置为 翻译数据中的某个键所记录的字符串。

### 外部资源模块：CCsGDFrame.GameSystem.game_io
<br>读取资源时优先考虑外部资源，如果不存在，才考虑pck的嵌入资源，编辑器内Debug 与 Debug导出时才会工作，Release导出时此模块只考虑pck的嵌入资源。此举是为了在团队协作中为合作者提供完整的导出版程序，使非程序合作者无需安装Godot工作流即可进行素材的编辑与替换，Release导出时开发者将对应素材迁徙到项目内部即可。
<br>GetResource 等同于 Godot.ResourceLoader.Load，但是会首先考虑读取外部路径中的素材，如果没有找到才考虑读取 res:// 中对应路径的资源。

### 单元测试模块
<br>框架自带的一个ui，通过点击窗口右下角显示的 debug 按钮来展开，此按钮在 release导出时自动隐藏。
<br>扩展CCsGDFrame下的 public partial class UI_Debug 来编写你的单元测试代码，其中定义的所有以 When_ 开头的方法均会作为一个按钮加入到该 ui 中，同时提供了 LogLine() 来显示输出的字符串。

<br>Voice Credit:
<br>https://cyrex-studios.itch.io/universal-ui-soundpack
<br>https://potat0master.itch.io/free-background-music-for-visual-novels-bgm-pack-1
