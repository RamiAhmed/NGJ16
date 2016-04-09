namespace Game
{
    using System;
    using UnityEngine;

    [Serializable]
    public class PlayerSetup
    {
        public GameObject playerPrefab;
        public GameObject tankPrefab;

        public Transform playerStartPos;
        public Transform tankStartPos;
    }
}