using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTTT
{
    public class GameManager : MonoBehaviour
    {
        public enum State
        {
            Selecting,
            Rolling,
            Retracting
        }

        public Die Die;
        public List<GameSquare> Squares;
        public int MaxNeutralChances;
        public int MinGoodChances;
        public bool CurrentlyX { get; private set; }
        public State CurrentState { get; private set; } = State.Selecting;

        public ScoreIndicator XScore;
        public ScoreIndicator TieScore;
        public ScoreIndicator OScore;

        private bool xStartNextGame = true;
        private GameSquare selectedSquare;
        private SquareContents lastRollResult;


        void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
            Time.fixedDeltaTime = 1.0f / Application.targetFrameRate;

            ResetBoard();
        }

        public void ResetBoard()
        {
            SetCurrentPlayer(xStartNextGame);
            xStartNextGame = !xStartNextGame;

            foreach(var square in Squares)
            {
                square.Manager = this;

                var neutralChances = Random.Range(1, MaxNeutralChances + 1);  // Note that this is using the int overload of random range
                square.GoodChances = Random.Range(MinGoodChances, 20 - neutralChances);  // Ensure that there's at least 1 bad chance
                square.BadChances = 20 - (square.GoodChances + neutralChances);

                square.Reset();
            }
        }

        public void OnSquareSelect(GameSquare selected)
        {
            CurrentState = State.Rolling;
            selectedSquare = selected;

            Die.AssignFaces(selected.GoodChances, selected.BadChances);

            StartCoroutine(Die.Roll(CurrentlyX, OnRollComplete));
        }

        private void OnRollComplete(SquareContents result)
        {
            CurrentState = State.Retracting;
            lastRollResult = result;
            StartCoroutine(Die.Retract(OnWinningFaceShown));
        }

        private void OnWinningFaceShown()
        {
            selectedSquare.HandlePlacement(lastRollResult, OnPlacementComplete);
        }

        private void OnPlacementComplete()
        {
            CurrentState = State.Selecting;
            SetCurrentPlayer(!CurrentlyX);
        }

        void SetCurrentPlayer(bool playerX)
        {
            CurrentlyX = playerX;
            (CurrentlyX ? XScore : OScore).Highlight();
            (CurrentlyX ? OScore : XScore).UnHighlight();
        }
    }
}
