﻿namespace Game
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class StartManager : MonoBehaviour
    {
        [Range(10, 100)]
        public int fontSize = 40;

        [Range(1, 10)]
        public int maxCountdown = 5;

        public Color countdownColor = Color.red;

        [Range(2, 3)]
        public int minPlayers = 2;

        public PlayerSetup[] playerSetups = new PlayerSetup[0];

        private List<int> _list = new List<int>(3);
        private bool _starting;
        private int _countdown;

        private GUIStyle _style;

        private void OnEnable()
        {
            PauseManager.isPaused = true;
            _countdown = this.maxCountdown;
        }

        private void Update()
        {
            for (int i = 1; i <= 3; i++)
            {
                HandleControllerStart(i);
            }

            if (!_starting && _list.Count >= this.minPlayers)
            {
                _starting = true;
                StartCoroutine(Countdown());
            }
        }

        private void OnGUI()
        {
            if (_style == null)
            {
                _style = new GUIStyle(GUI.skin.label);
                _style.fontSize = this.fontSize;
                _style.alignment = TextAnchor.MiddleCenter;
            }

            GUI.Label(new Rect(5f, 5f, 100f, 50f), "Players Joined: " + _list.Count);

            if (_starting)
            {
                var height = Screen.height;
                var width = Screen.width;
                var w = 200f;
                var h = 100f;
                GUI.color = this.countdownColor;
                GUI.Label(new Rect((width * 0.5f) - (w * 0.5f), (height * 0.5f) - (h * 0.5f), w, h), _countdown.ToString(), _style);
            }
        }

        private void HandleControllerStart(int index)
        {
            if (!_list.Contains(index))
            {
                if (Input.GetButtonDown(string.Concat("Start_", index)))
                {
                    Debug.Log("Start player " + index);
                    _list.Add(index);
                    var playerSetup = this.playerSetups[index - 1];
                    var playerGO = (GameObject)Instantiate(playerSetup.playerPrefab, playerSetup.playerStartPos.position, playerSetup.playerStartPos.rotation);
                    var player = playerGO.GetComponent<PlayerController>();

                    var tankGO = (GameObject)Instantiate(playerSetup.tankPrefab, playerSetup.tankStartPos.position, playerSetup.tankStartPos.rotation);
                    tankGO.GetComponent<Tank>().player = player;
                }
            }
        }

        private IEnumerator Countdown()
        {
            while (_countdown-- > 0)
            {
                yield return new WaitForSeconds(1f);
            }

            PauseManager.isPaused = false;
            this.enabled = false;
            _starting = false;
        }
    }
}