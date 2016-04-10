﻿namespace Game
{
	using System;
	using UnityEngine;

	[Serializable]
	public class SoundFx
	{
		public SoundFxType type = SoundFxType.PlayerHit;
		public AudioClip[] clips = new AudioClip[0];
	}

	public enum SoundFxType {
		PlayerHit,
		PlayerDash,
		PlayerSwing,
		TankHit,
		SpeakerFirstLeak,
		SpeakerTankRepaired,
		SpeakerTankEmpty,
		SpeakerPlayerHit,
		SpeakerPlayerKilled,
		SpeakerTankLeaking
	}

}