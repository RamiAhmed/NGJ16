namespace Game
{
    using UnityEngine;

    public class SoundManager : MonoBehaviour
    {
        public static SoundManager instance { get; private set; }

        public SoundFx[] soundFx;
        public AudioClip[] backgroundMusic = new AudioClip[0];
        public AudioSource backgroundPlayer;
        public AudioSource fxPlayer;

        [Range(0f, 1f)]
        public float backgroundVolume = .5f;

        [Range(0f, 1f)]
        public float fxVolume = 1f;

        private AudioClip _nextTrack;

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
        }

        public void PlayFx(SoundFxType fx)
        {
            AudioClip[] clips = null;
            // Currently only supports the first match. Don't be dumb ;-)
            foreach (SoundFx sfx in soundFx)
            {
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
            if (_nextTrack == null)
            {
                _nextTrack = GetRandomClip(backgroundMusic);
            }
        }
    }
}