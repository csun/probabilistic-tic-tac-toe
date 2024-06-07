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
            Retracting,
            DisplayingWinner
        }

        public Die Die;
        public List<GameSquare> Squares;
        public PlayerSelectButton PlayerSelectButton;

        public int MaxNeutralChances;
        public int MinGoodChances;
        public bool IsSingleplayer { get; private set; }
        public bool CurrentlyX { get; private set; }
        public State CurrentState { get; private set; } = State.Selecting;

        public ScoreIndicator XScore;
        public ScoreIndicator TieScore;
        public ScoreIndicator OScore;

        private bool xStartNextGame = true;
        private GameSquare selectedSquare;
        private SquareContents lastRollResult;
        private BoardAnalyzer analyzer;


        void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
            Time.fixedDeltaTime = 1.0f / Application.targetFrameRate;

            analyzer = new(Squares);

            SetPlayerMode(false);
        }

        public void SetPlayerMode(bool singleplayer)
        {
            IsSingleplayer = singleplayer;
            xStartNextGame = true;
            ResetScore();
            ResetBoard();
            PlayerSelectButton.Refresh();
        }

        private void ResetScore()
        {
            XScore.Count = 0;
            OScore.Count = 0;
            TieScore.Count = 0;
        }

        private void ResetBoard()
        {
            CurrentState = State.Selecting;
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
            var winstate = analyzer.GetWinState();

            ScoreIndicator winner;
            switch (winstate)
            {
                case BoardAnalyzer.WinState.XWin:
                    winner = XScore;
                    break;
                case BoardAnalyzer.WinState.OWin:
                    winner = OScore;
                    break;
                case BoardAnalyzer.WinState.Stalemate:
                    winner = TieScore;
                    break;
                default:
                    winner = null;
                    break;
            }

            if (winner is null)
            {
                CurrentState = State.Selecting;
                SetCurrentPlayer(!CurrentlyX);
                foreach (var square in Squares) { square.Refresh(); }
            }
            else
            {
                CurrentState = State.DisplayingWinner;
                StartCoroutine(DisplayWinner(winner));
            }

        }

        private IEnumerator DisplayWinner(ScoreIndicator winner)
        {
            XScore.UnHighlight();
            TieScore.UnHighlight();
            OScore.UnHighlight();

            winner.Count++;

            yield return winner.Blink(ResetBoard);
        }

        void SetCurrentPlayer(bool playerX)
        {
            CurrentlyX = playerX;
            (CurrentlyX ? XScore : OScore).Highlight();
            (CurrentlyX ? OScore : XScore).UnHighlight();
            TieScore.UnHighlight();
        }
    }
}
