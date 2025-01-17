using Godot;
using System.Collections.Generic;

namespace CCsGDFrame
{
    public class Godot_GameVoice
    {
        public Node parent_node = null;
        public string src_path = null;
        /// <summary>
        /// // parent_node 为即将放置音效播放器的空节点，src_path 需带有末尾的 /
        /// </summary>
        /// <param name="parent_node"></param>
        /// <param name="src_path"></param>
        public Godot_GameVoice(Node parent_node, string src_path)
        {
            this.parent_node = parent_node;
            this.src_path = src_path;
            Bgm1 = new Bgm声轨(this);
            Bgm2 = new Bgm声轨(this);
            Bgm3 = new Bgm声轨(this);
        }
        /// <summary>
        /// // 根据音轨 index，从 Voice/ 节点下寻找 BGM1，BGM1+，BGM1_Anim（空动画节点），BGM1+_Anim（空动画节点） 四个节点来使用
        /// </summary>
        public class Bgm声轨
        {
            Godot_GameVoice comfrom;
            public Bgm声轨(Godot_GameVoice comfrom) { this.comfrom = comfrom; }
            public class Bgm播放器 : AudioStreamPlayer
            {
                public override void _Ready()
                {
                    this.Connect("finished", this, "WhenFinish");
                }
                public float 当前音量
                {
                    set
                    {
                        float db = (float)(10 * System.Math.Log10(value)) + 0f;
                        this.VolumeDb = db;
                        当前音量_inner = value;
                    }
                    get { return 当前音量_inner; }
                }
                float 当前音量_inner = 0f;
                float 起始音量;
                /// <summary>
                /// // 若最终音量为 0，则默认为备用音轨
                /// </summary>
                public float 最终音量;
                public void 进度条准备(float target) { 起始音量 = 当前音量; 最终音量 = target; }
                public float JinDuTiao
                {
                    set
                    {
                        if (最终音量 > 当前音量)
                        { 当前音量 = 起始音量 + (最终音量 - 起始音量) * value; }
                        else if (最终音量 < 当前音量)
                        { 当前音量 = 起始音量 - (起始音量 - 最终音量) * value; }
                        else { }
                        if (当前音量 <= 0) { this.Stop(); }
                        else if (this.Playing == false) { this.Play(); }
                    }
                }

                public Bgm声轨 comfrom;
                public bool loop = true;
                public void WhenFinish()
                {
                    if (this.loop) this.Play();
                    else comfrom.when_BgmFinish.Invoke();
                }
            }
            /// <summary>
            /// // 实例化所需节点
            /// </summary>
            /// <returns></returns>
            public void CreateNode()
            {
                Node this_node = new Node();
                bgm_a = new Bgm播放器(); bgm_a.当前音量 = 0f; bgm_a.comfrom = this; this_node.AddChild(bgm_a);
                bgm_b = new Bgm播放器(); bgm_b.当前音量 = 0f; bgm_b.comfrom = this; this_node.AddChild(bgm_b);
                anim_player_a = new AnimationPlayer(); this_node.AddChild(anim_player_a);
                anim_player_b = new AnimationPlayer(); this_node.AddChild(anim_player_b);
                comfrom.parent_node.AddChild(this_node);
                this.anim_reset(anim_player_a, bgm_a, time_len);
                this.anim_reset(anim_player_b, bgm_b, time_len);
            }
            Bgm播放器 bgm_a, bgm_b, bgm_in, bgm_out;
            AnimationPlayer anim_player_a, anim_player_b;
            /// <summary>
            /// // bgm 总音量，由系统自动配置，不要修改
            /// <br>// 0f~ 1f</br>
            /// </summary>
            public float volume = 1f;
            /// <summary>
            /// // 该声轨的音量（Play时自动设置一次，不建议修改）
            /// <br>// 0f ~1f</br>
            /// </summary>
            public float volume_this
            {
                set
                {
                    bgm_in.最终音量 = volume_to_db(this.volume);
                    volume_this_inner = value;
                }
                get { return volume_this_inner; }
            }
            float volume_this_inner = 1f;
            /// <summary>
            /// // 渐入渐出的时长（秒）
            /// </summary>
            public float time_len
            {
                set
                {
                    if (value == time_len_inner) return;
                    else
                    {
                        if (bgm_a == null/*未初始化*/) CreateNode();
                        time_len_inner = value;
                        this.anim_reset(anim_player_a, bgm_a, time_len_inner);
                        this.anim_reset(anim_player_b, bgm_b, time_len_inner);
                        return;
                    }
                }
                get { return time_len_inner; }
            }
            float time_len_inner = 0f;
            public void anim_reset/*重新设置渐入渐出动画时长*/(AnimationPlayer anim_player, Bgm播放器 aduio, float 时长)
            {
                Animation anim;
                if (anim_player.HasAnimation("0"))
                {
                    anim = anim_player.GetAnimation("0");
                    if (anim.Length == 时长) return;
                    anim_player.RemoveAnimation("0");
                }
                anim = new Animation();
                anim.Length = 时长;
                int track_id = anim.AddTrack(Animation.TrackType.Value);
                anim.ValueTrackSetUpdateMode(track_id, Animation.UpdateMode.Continuous);
                anim.TrackSetPath(track_id, aduio.GetPath() + ":JinDuTiao");
                anim.TrackInsertKey(track_id, 0, 0);
                anim.TrackInsertKey(track_id, 时长, 1f);
                anim_player.AddAnimation("0", anim);
            }
            /// <summary>
            /// <br>// 方案a: loop 为 true，该 bgm 自动循环</br>
            /// <br>// 方案c： loop 为 false，必须注册 when_BgmFinish 并在其中手动 Play 下一个 bgm</br>
            /// </summary>
            public void Play(string file_name, bool loop = true, float volume = 1f)
            {
                if (bgm_a == null/*未初始化*/) CreateNode();
                AudioStream stream;
                string file_path;
                if (ResourceLoader.Exists(comfrom.src_path + "BGM/" + file_name)) file_path = comfrom.src_path + "BGM/" + file_name;
                else if (ResourceLoader.Exists("res://" + comfrom.src_path + "BGM/" + file_name)) file_path = "res://" + comfrom.src_path + "BGM/" + file_name;
                else return;
                try { stream = ResourceLoader.Load<AudioStream>(file_path); }
                catch { return; }

                // 寻找一个空闲的 BGM播放器
                if (bgm_a.最终音量 == 0) { bgm_in = bgm_a; bgm_out = bgm_b; }
                else { bgm_in = bgm_b; bgm_out = bgm_a; }
                bgm_in.Stream = stream;
                bgm_in.Play();
                //
                volume_this = this.volume * volume;// 设置渐入渐出的最终音量
                bgm_in.进度条准备(volume_this);
                bgm_out.进度条准备(0f);
                bgm_in.loop = loop;
                bgm_out.loop = loop;
                // 播放
                anim_player_a.Stop(); anim_player_a.Play("0");
                anim_player_b.Stop(); anim_player_b.Play("0");
            }
            public void Stop()
            {
                if (bgm_a == null/*未初始化*/) return;
                bgm_a.进度条准备(0f);
                bgm_b.进度条准备(0f);
                anim_player_a.Stop(); anim_player_a.Play("0");
                anim_player_b.Stop(); anim_player_b.Play("0");
            }
            public delegate void DelegateWhenBgmFinish();
            static void DelegateWhenBgmFinish_inner() { }
            /// <summary>
            /// // bgm 结束时触发，（手动切换 Bgm 或 Stop 时不会触发，仅自动循环一次结束时）
            /// </summary>
            public DelegateWhenBgmFinish when_BgmFinish = new DelegateWhenBgmFinish(DelegateWhenBgmFinish_inner);
        }
        public Bgm声轨 Bgm1, Bgm2, Bgm3;
        /// <summary>
        /// //  bgm 的总音量（0f~1.0f）
        /// </summary>
        public float Bgm_volume
        {
            set
            {
                Bgm1.volume = value;
                Bgm2.volume = value;
                Bgm3.volume = value;
                Bgm_volume_inner = value;
            }
            get
            {
                return Bgm_volume_inner;
            }
        }
        float Bgm_volume_inner = 1f;
        /// <summary>
        /// // bgm 渐入渐隐时长
        /// </summary>
        public float Bgm_timelen
        {
            set
            {
                Bgm1.time_len = value;
                Bgm2.time_len = value;
                Bgm3.time_len = value;
                Bgm_timelen_inner = value;
            }
            get { return Bgm_timelen_inner; }
        }
        float Bgm_timelen_inner = 0f;

        static float volume_to_db(float volume) { return (float)(10 * System.Math.Log10(volume)) + 0f; }

        /// <summary>
        /// // 音效的对象池
        /// </summary>
        SortedDictionary<int, AudioStreamPlayer> Se_pool = new SortedDictionary<int, AudioStreamPlayer>();
        /// <summary>
        /// // 音效的总音量（0f~1.0f）
        /// </summary>
        public float SE_volume = 1f;
        /// <summary>
        /// // 播放音效 name= 文件名（需要后缀）
        /// <br>// volume 为单独的 se 音量，设置全局 SE音量，请修改 SE_volume</br>
        /// </summary>
        /// <param name="file_name"></param>
        public void SE_Play(string file_name, float volume = 1f)
        {
            AudioStream stream;
            string file_path;
            if (ResourceLoader.Exists(src_path + "SE/" + file_name)) file_path = src_path + "SE/" + file_name;
            else if (ResourceLoader.Exists("res://" + src_path + "SE/" + file_name)) file_path = "res://" + src_path + "SE/" + file_name;
            else return;
            try { stream = ResourceLoader.Load<AudioStream>(file_path); stream.Set("loop", false); }
            catch { return; }
            // 寻找一个空闲的 SE播放器
            AudioStreamPlayer target = null;
            foreach (AudioStreamPlayer founded in Se_pool.Values)
            {
                if (founded.Playing) continue;
                target = founded;
                break;
            }
            // 没有空闲时，创建一个新的来播放
            if (target == null)
            {
                target = new AudioStreamPlayer();
                Se_pool.Add(Se_pool.Count, target);
                parent_node.AddChild(target);
            }
            target.VolumeDb = volume_to_db(SE_volume * volume);
            target.Stream = stream;
            target.Play();
        }
        public void SE_Stop()
        {
            foreach (AudioStreamPlayer founded in Se_pool.Values)
            {
                if (founded.Playing) founded.Stop();
            }
        }
    }
}