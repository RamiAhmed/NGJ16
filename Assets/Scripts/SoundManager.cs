namespace Game
{
    using UnityEngine;

    public class SoundManager : MonoBehaviour
    {
        public AudioClip[] backgroundMusic = new AudioClip[0];
        public AudioClip playerHit;
        public AudioClip playerDash;
        public AudioSource backgroundPlayer;

        private AudioClip nextTrack;

        // Use this for initialization
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            QueueNextTrack();
            PlayTrack();
        }

        private void PlayTrack()
        {
            if (!backgroundPlayer.isPlaying)
            {
                backgroundPlayer.clip = nextTrack;
                backgroundPlayer.Play();
                nextTrack = null;
            }
        }

        private AudioClip GetTrack()
        {
            //TODO: Crazy logic here.
            if (backgroundMusic.Length > 0)
            {
                int idx = Random.Range(0, backgroundMusic.Length);
                print("Playing " + idx);
                return backgroundMusic[idx];
            }

            return null;
        }

        private void QueueNextTrack()
        {
            if (nextTrack == null)
            {
                nextTrack = GetTrack();
            }
        }
    }
}