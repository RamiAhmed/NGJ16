namespace Game
{
    using System;
    using UnityEngine;

    [Serializable]
    public class TankSeverity
    {
        [Range(1, 20)]
        public int hits = 1;

        [Range(0.1f, 30f)]
        public float leakRatePerSecond = 2f;
    }
}