using Microsoft.Xna.Framework.Audio;

namespace LandDefense
{
    public class MySound
    {
        private static Cue mainMusic;
// static  Cue circle;
        private static Cue bgm;
        public AudioEngine ae;
        public SoundBank sb;
        public WaveBank wb;

        /// <summary>
        ///     根据输入数字播放声音，1是主菜单，2是关卡选择，3是游戏主程序
        ///     4是敌人通过，5是游戏失败，6是敌人死亡，7是游戏中音乐循环播放
        /// </summary>
        /// <param name="type"></param>
        public void PLaySound(int type)
        {
            bgm = sb.GetCue("levelchoose");
            switch (type)
            {
                case 1:
                    sb.PlayCue("level begain");
                    mainMusic = sb.GetCue("level begain");
                    //if (!mainMusic.IsPlaying)
                    //   mainMusic.Play();
                    break;
            }
        }

        public void PlaySound(string name)
        {
            sb.PlayCue(name);
        }

        public void PauseBGM()
        {
            if (bgm.IsPlaying)
                bgm.Pause();
        }

        public void ResumeBGM()
        {
            if (bgm.IsPaused)
                bgm.Resume();
        }

        public void Playbgm()
        {
            if (!bgm.IsPlaying)
            {
                bgm = sb.GetCue(bgm.Name);
                bgm.Play();
            }
        }

        public bool IsBGMPause()
        {
            return bgm.IsPaused;
        }

        public void Update()
        {
            if (bgm.IsStopped)
            {
                Playbgm();
                ae.Update();
            }
        }
    }
}