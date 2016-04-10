namespace Game
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;
    using UnityEngine.UI;

    public class ScoreboardManager : MonoBehaviour
    {
        public static ScoreboardManager instance { get; private set; }

        public Text scoreText;

        private Dictionary<PlayerController, PlayerScore> _dict = new Dictionary<PlayerController, PlayerScore>(3);

        private void OnEnable()
        {
            if (this.scoreText == null)
            {
                throw new ArgumentNullException("scoreText");
            }

            if (instance != null)
            {
                Debug.LogWarning(this.ToString() + " another ScoreboardManager has already been registered, destroying this one");
                Destroy(this);
                return;
            }

            instance = this;
        }

        public PlayerScore Get(PlayerController player)
        {
            PlayerScore score;
            if (!_dict.TryGetValue(player, out score))
            {
                score = new PlayerScore();
                _dict.Add(player, score);
            }

            return score;
        }

        public void SetScoreText()
        {
            var sb = new StringBuilder(_dict.Count * 14); // TODO: Could do with better preallocation
            foreach (var pair in _dict)
            {
                var player = pair.Key;
                var score = pair.Value;
                var color = GetPlayerColor(player.playerIndex);

                sb.AppendFormat("<color={0}>", color);
                sb.AppendFormat("Player {0}</color> Scores - ", player.playerIndex);
                sb.AppendFormat("Enemy hits: {0}, ", score.enemyHits);
                sb.AppendFormat("Tank hits: {0}, ", score.tankHits);
                sb.AppendFormat("Wall bounces: {0}, ", score.wallBounces);
                sb.AppendLine();
            }

            this.scoreText.text = sb.ToString();
        }

        private string GetPlayerColor(int index)
        {
            switch (index)
            {
                case 1:
                {
                    return "red";
                }

                case 2:
                {
                    return "green";
                }

                case 3:
                {
                    return "blue";
                }
            }

            return string.Empty;
        }
    }
}