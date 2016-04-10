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
					SoundManager.instance.PlayAnnouncement(SoundFxType.SpeakerFirstLeak);
					SoundManager.instance.ToggleMusic();
				} else {
					SoundManager.instance.PlayAnnouncement(SoundFxType.SpeakerTankLeaking);
				}

				break;
			case Announcement.TankRepaired:
				SoundManager.instance.PlayAnnouncement(SoundFxType.SpeakerTankRepaired);
				break;
			case Announcement.TankEmpty:
				SoundManager.instance.PlayAnnouncement(SoundFxType.SpeakerTankEmpty);
				break;
			case Announcement.PlayerHit:
				SoundManager.instance.PlayAnnouncement(SoundFxType.SpeakerPlayerHit);
				break;
			case Announcement.PlayerKilled:
				SoundManager.instance.PlayAnnouncement(SoundFxType.SpeakerPlayerKilled);
				break;
			case Announcement.Begin:
				SoundManager.instance.PlayAnnouncement(SoundFxType.SpeakerBegin);
				break;
			}
		}

		void Awake()
		{
			if (instance != null)
			{
				Debug.LogWarning(this.ToString() + " another SpeakerManager has already been registered, destroying this one");
				GameObject.Destroy(this);
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
		PlayerKilled,
		Begin
	}
}

