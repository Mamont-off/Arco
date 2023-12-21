using Scriptable;
using UnityEngine;

namespace Other
{
    /// <summary> Do it simple, we don't have good ECS sound </summary>
    public class AudioManager : MonoBehaviour//TODO: ECS-based Audio
    {
        [SerializeField] private AudioSOData _audioData;
        public static AudioManager Instance { get; private set; }

        private AudioSource _musicSource;
        private AudioSource _sfxSource;

        private void Awake()
        {
            Init();
            _musicSource.Play();
        }

        private void Init()
        {
            Instance = this;
            
            var audioSourceGO = new GameObject("AudioObject")
            {
                isStatic = true
            };

            _musicSource = audioSourceGO.AddComponent<AudioSource>();
            _musicSource.clip = _audioData.Music;
            _musicSource.loop = true;
            
            _sfxSource = audioSourceGO.AddComponent<AudioSource>();
        }

        public void PlayShoot()
        {
            _sfxSource.PlayOneShot(_audioData.Shoot);
        }

        public void PlayExplosion()
        {
            _sfxSource.PlayOneShot(_audioData.Explosion);
        }
    }
}