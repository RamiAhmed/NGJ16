using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {
	
	public AudioClip[] BackgroundMusic;
	public AudioClip PlayerHit;
	public AudioClip PlayerDash;
	public AudioSource backgroundPlayer;

	private AudioClip nextTrack;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		QueueNextTrack();
		PlayTrack();
	}

	void PlayTrack() {
		if(!backgroundPlayer.isPlaying) {
			backgroundPlayer.clip = nextTrack;
			backgroundPlayer.Play();
			nextTrack = null;
		}
	}

	AudioClip GetTrack() {
		//TODO: Crazy logic here.
		if(BackgroundMusic.Length > 0) {
			
			int idx = Random.Range(0, BackgroundMusic.Length);
			print("Playing " + idx);
			return BackgroundMusic[idx];
		}
		return null;
	}

	void QueueNextTrack() {
		if(nextTrack == null) {
			nextTrack = GetTrack();
		}
	}
}
