using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTTT
{
    public class GameManager : MonoBehaviour
    {
        public List<GameSquare> Squares;
        public bool CurrentlyX;

        private void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
            Time.fixedDeltaTime = 1.0f / Application.targetFrameRate;

            SetCurrentPlayer(true);

            foreach(var square in Squares)
            {
                square.Manager = this;

                var goodInt = Random.Range(1, 19);  // Int overload from [1, 18]
                var badInt = Random.Range(1, 19 - goodInt);  // Should guarantee at least 1/20 chance of neither good or bad

                square.GoodChance = goodInt / 20.0f;
                square.BadChance = badInt / 20.0f;
                square.UpdateChanceText();
            }
        }

        internal void SetCurrentPlayer(bool playerX)
        {
            CurrentlyX = playerX;
        }
    }
}
