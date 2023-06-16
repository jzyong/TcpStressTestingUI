using Core.Util;
using UnityEngine;

namespace Common
{
    /// <summary>
    /// 音频播放管理
    /// </summary>
    public class AudioManager : SingleInstance<AudioManager>
    {
        /**背景音乐*/
        private AudioSource bgmAudioSource;

        /**效果音乐*/
        private AudioSource sfxAudioSource;


        public AudioManager()
        {
            Init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            var g = new GameObject();
            bgmAudioSource = g.AddComponent<AudioSource>();
            g.name = "BGMAudioSource";
            Object.DontDestroyOnLoad(g);

            g = new GameObject();
            sfxAudioSource = g.AddComponent<AudioSource>();
            g.name = "SFXAudioSource";
            Object.DontDestroyOnLoad(g);


            bgmAudioSource.loop = true;
            bgmAudioSource.playOnAwake = true;
            sfxAudioSource.loop = false;
            sfxAudioSource.playOnAwake = true;
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void Destroy()
        {
            if (bgmAudioSource)
            {
                Object.Destroy(bgmAudioSource.gameObject);
            }

            if (sfxAudioSource)
            {
                Object.Destroy(sfxAudioSource.gameObject);
            }
        }


        /// <summary>
        /// 停止播放特效音乐
        /// </summary>
        public void StopSfx()
        {
            if (sfxAudioSource)
            {
                sfxAudioSource.Stop();
            }
        }


        /// <summary>
        /// 播放特效声音 
        /// </summary>
        /// <param name="name"></param>
        public void PlaySfx(string name)
        {
            // ResourceManager.Instance.LoadAudioClip(name, (clip =>
            // {
            //     if (sfxAudioSource != null)
            //         sfxAudioSource.PlayOneShot(clip);
            // }));
        }


        /// <summary>
        /// 停止音乐播放
        /// </summary>
        public void StopMusic()
        {
            bgmAudioSource.Stop();
        }

        // /// <summary>
        // /// 播放音乐
        // /// </summary>
        // /// <param name="name"></param>
        // /// <param name="audioSuffix"></param>
        // public void PlayMusic(string name, string audioSuffix = "mp3")
        // {
        //     if (string.IsNullOrEmpty(name))
        //     {
        //         if (bgmAudioSource)
        //         {
        //             bgmAudioSource.Stop();
        //         }
        //     }
        //
        //     ResourceManager.Instance.LoadAudioClip(name, (clip =>
        //     {
        //         if (bgmAudioSource)
        //         {
        //             bgmAudioSource.clip = clip;
        //             bgmAudioSource.Play();
        //         }
        //     }), audioSuffix);
        // }

        /// <summary>
        /// 设置音乐音量
        /// </summary>
        /// <param name="vol"></param>
        public void SetMusicVolume(float vol)
        {
            if (bgmAudioSource)
                bgmAudioSource.volume = vol * 0.2f;
        }

        /// <summary>
        /// 设置特效音量 
        /// </summary>
        /// <param name="vol"></param>
        public void SetSfxVolume(float vol)
        {
            if (sfxAudioSource)
                sfxAudioSource.volume = vol * 0.37f;
        }


        /// <summary>
        /// 设置背景音是否可用 
        /// </summary>
        /// <param name="state"></param>
        public void SetMusicEnable(bool state)
        {
            if (bgmAudioSource)
                bgmAudioSource.enabled = state;
        }


        /// <summary>
        /// 设置特效是否可用
        /// </summary>
        /// <param name="state"></param>
        public void SetSfxEnable(bool state)
        {
            if (sfxAudioSource)
                sfxAudioSource.enabled = state;
        }
    }
}