using UnityEngine;

namespace WhiteKNight
{
    public class SoundManager : MonoBehaviour
    {
        private static SoundManager instance;

        public static SoundManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<SoundManager>();
                }

                return instance;
            }
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            // 全てのAudioSourceコンポーネントを追加する

            // BGM AudioSource
            BGMsource = gameObject.AddComponent<AudioSource>();
            // BGMはループを有効にする
            BGMsource.loop = true;

            // SE AudioSource
            for (int i = 0; i < SEsources.Length; i++)
            {
                SEsources[i] = gameObject.AddComponent<AudioSource>();
                SEsources[i].loop = false;
                SEsources[i].playOnAwake = false;
            }
        }

        public SoundVolume volume = new SoundVolume();

        // === AudioSource ===
        // BGM
        private AudioSource BGMsource;
        // SE
        private AudioSource[] SEsources = new AudioSource[10];
       
        // === AudioClip ===
        // BGM
        public AudioClip[] BGM;
        [SerializeField] private BGMAudioDatabase _BGMdatabase;

        // SE
        public AudioClip[] SE;
        [SerializeField] private SoundEffectAudioDatabase _SEdatabase;

      
        public void SetBGMMute()
        {
            BGMsource.mute = true;
        }

        public void SetSEMute()
        {
            foreach (AudioSource source in SEsources)
            {
                source.mute = true;
            }
        }

        public void ResetBGMMute()
        {
            BGMsource.mute = false;
        }

        public void ResetSEMute()
        {
            foreach (AudioSource source in SEsources)
            {
                source.mute = true;
            }
        }

        public void SetMute()
        {
            BGMsource.mute = volume.Mute;
            foreach (AudioSource source in SEsources)
            {
                source.mute = volume.Mute;
            }
        }

        public void SetSEVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);
            foreach (AudioSource source in SEsources)
            {
                source.volume = volume;
            }
        }

        public void SetBGMVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);
            BGMsource.volume = volume;
        }

        // ***** BGM再生 *****
        // BGM再生
        public void PlayBGM(int index)
        {
            if (0 > index || BGM.Length <= index)
            {
                return;
            }
            // 同じBGMの場合は何もしない
            if (BGMsource.clip == BGM[index])
            {
                print("same bgm");
                return;
            }

            BGMsource.Stop();
            BGMsource.clip = BGM[index];
            BGMsource.Play();
        }

        public void PlayBGM(BGMId bgmID)
        {
            var data = _BGMdatabase.Get(bgmID);
            if(data == null)
            {
                Debug.LogWarning($"BGM '{bgmID}' was not found.");
                return;
            }

            BGMsource.clip = data.Clip;
            BGMsource.volume = data.Volume;
            BGMsource.pitch = data.Pitch;
            BGMsource.loop = data.Loop;
            BGMsource.outputAudioMixerGroup = data.Mixer;
            BGMsource.Play();
        }

        // BGM停止
        public void StopBGM()
        {
            BGMsource.Stop();
            BGMsource.clip = null;
        }


        // ***** SE再生 *****
        // SE再生
        public void PlaySE(int index)
        {
            if (SE == null || index < 0 || SE.Length <= index || SE[index] == null)
                return;

            foreach (AudioSource source in SEsources)
            {
                if (!source.isPlaying)
                {
                    source.PlayOneShot(SE[index], volume.SE);
                    return;
                }
            }

            // 全部埋まってたら先頭で鳴らす
            SEsources[0].PlayOneShot(SE[index], volume.SE);
        }
        
        public void PlaySE(SoundEffectId seID)
        {
            var data = _SEdatabase.Get(seID);
            if (data == null)
            {
                Debug.LogWarning($"SE '{seID}' was not found.");
                return;
            }

            foreach (AudioSource source in SEsources)
            {
                if (!source.isPlaying)
                {
                    source.clip = data.Clip;
                    source.volume = data.Volume;
                    source.pitch = data.Pitch;
                    source.loop = data.Loop;
                    source.outputAudioMixerGroup = data.Mixer;
                    source.PlayOneShot(source.clip);
                    return;
                }
            }

            // 全部埋まってたら先頭で鳴らす
            SEsources[0].clip = data.Clip;
            SEsources[0].volume = data.Volume;
            SEsources[0].pitch = data.Pitch;
            SEsources[0].loop = data.Loop;
            SEsources[0].outputAudioMixerGroup = data.Mixer;
            SEsources[0].Play();
        }

        public SoundEffectEntry GetPlayFootStep(SoundEffectId seID, float pitch, float volume)
        {
            var data = _SEdatabase.Get(seID);
            if (data == null)
            {
                Debug.LogWarning($"SE '{seID}' was not found.");
                return null;
            }
            return data;
        }

        // SE停止
        public void StopSE()
        {
            // 全てのSE用のAudioSouceを停止する
            foreach (AudioSource source in SEsources)
            {
                source.Stop();
                source.clip = null;
            }
        }

        public bool IsSameAudioPlaying(int index)
        {
            if (SE == null || index < 0 || SE.Length <= index || SE[index] == null)
                return false;

            foreach (AudioSource source in SEsources)
            {
                if (source.isPlaying && source.clip == SE[index])
                    return true;
            }

            return false;
        }
    }
}
