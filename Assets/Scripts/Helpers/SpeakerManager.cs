namespace Game
{
	using System.Collections;
	using UnityEngine;

	public class SpeakerManager : MonoBehaviour
	{
		public static SpeakerManager instance { get; private set; }

		private bool firstLeak = false;

		public void Announce(Announcement announcement) {
			switch(announcement) {
			case Announcement.TankHit:
				if(!firstLeak) {
					firstLeak = true;
					SoundManager.instance.PlayFx(SoundFxType.SpeakerFirstLeak);
					SoundManager.instance.ToggleMusic();
				} else {
					SoundManager.instance.PlayFx(SoundFxType.SpeakerTankLeaking);
				}

				break;
			case Announcement.TankRepaired:
				SoundManager.instance.PlayFx(SoundFxType.SpeakerTankRepaired);
				break;
			case Announcement.TankEmpty:
				SoundManager.instance.PlayFx(SoundFxType.SpeakerTankEmpty);
				break;
			case Announcement.PlayerHit:
				SoundManager.instance.PlayFx(SoundFxType.SpeakerPlayerHit);
				break;
			case Announcement.PlayerKilled:
				SoundManager.instance.PlayFx(SoundFxType.SpeakerPlayerKilled);
				break;
			}
		}

		void Awake()
		{
			if (instance != null)
			{
				Debug.LogWarning(this.ToString() + " another SpeakerManager has already been registered, destroying this one");
				GameObject.Destroy(instance);
				return;
			}

			instance = this;

			DontDestroyOnLoad(this);
		}
	}

	public enum Announcement {
		TankHit,
		TankRepaired,
		TankEmpty,
		PlayerHit,
		PlayerKilled
	}
}

