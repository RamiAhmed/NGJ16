using System.Collections;

namespace Game
{
    using UnityEngine;

    public class SoundManager : MonoBehaviour
    {
        public static SoundManager instance { get; private set; }

        public SoundFx[] soundFx = new SoundFx[0];
		public AudioClip actionMusic;
		public AudioClip quietMusic;
        public AudioSource backgroundPlayer;
        public AudioSource fxPlayer;

        [Range(0f, 1f)]
        public float backgroundVolume = .5f;

        [Range(0f, 1f)]
        public float fxVolume = 1f;

        private AudioClip _nextTrack;
		private bool _actionMusic = false;
		private bool _fadingOut = false;
		private bool _fadingIn = false;

        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogWarning(this.ToString() + " another SoundManager has already been registered, destroying this one");
                GameObject.Destroy(instance);
                return;
            }

            instance = this;
        }

        private void Start()
        {
            backgroundPlayer.priority = 0;
        }

        private void Update()
        {
            QueueNextTrack();
            PlayTrack();

			ManageFading();
        }

		public void ToggleMusic() {
			_actionMusic = !_actionMusic;
			_fadingOut = true;
		}

        public void PlayFx(SoundFxType fx)
        {
            AudioClip[] clips = null;
            // Currently only supports the first match. Don't be dumb ;-)
            for (int i = 0; i < soundFx.Length; i++)
            {
                var sfx = soundFx[i];
                if (sfx.type == fx)
                {
                    clips = sfx.clips;
                    break;
                }
            }

            if (clips != null)
            {
                fxPlayer.PlayOneShot(GetRandomClip(clips), fxVolume);
            }
        }

        private void PlayTrack()
        {
            if (!backgroundPlayer.isPlaying)
            {
                backgroundPlayer.clip = _nextTrack;
                backgroundPlayer.Play();
                _nextTrack = null;
            }
        }

        private AudioClip GetRandomClip(AudioClip[] clips)
        {
            //TODO: Crazy logic here.
            if (clips.Length > 0)
            {
                int idx = Random.Range(0, clips.Length);
                return clips[idx];
            }

            return null;
        }

        private void QueueNextTrack()
        {
			if(!_actionMusic) {
				_nextTrack = quietMusic;
			} else {
				_nextTrack = actionMusic;
			}                
        }

		private void ManageFading()
		{
			if(_fadingIn || _fadingOut) {
				fadeOut();

				if (backgroundPlayer.volume <= 0.1) {
					backgroundPlayer.clip = _nextTrack;
					backgroundPlayer.Play();
					_fadingOut = false;
					_fadingIn = true;
				}

				fadeIn();
			}
		}

		void fadeIn()
		{
			if (backgroundPlayer.volume < backgroundVolume && _fadingIn) {
				backgroundPlayer.volume += 0.5f * Time.deltaTime;
			} else {
				_fadingIn = false;
			}
		}

		void fadeOut()
		{
			if(backgroundPlayer.volume > 0.1f && _fadingOut)
			{
				backgroundPlayer.volume -= 0.5f * Time.deltaTime;
			}
		}
    }
}