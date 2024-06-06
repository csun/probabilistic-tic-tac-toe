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
            Placing
        }

        public Die Die;
        public List<GameSquare> Squares;
        public bool CurrentlyX;
        public State CurrentState { get; private set; } = State.Selecting;

        private bool xStartNextGame = true;
        private GameSquare selectedSquare;


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

                square.GoodChances = Random.Range(1, 19);  // Int overload from [1, 18]
                square.BadChances = Random.Range(1, 19 - square.GoodChances);  // Should guarantee at least 1/20 chance of neither good or bad

                square.Reset();
            }
        }

        public void OnSquareSelect(GameSquare selected)
        {
            CurrentState = State.Rolling;
            selectedSquare = selected;

            var xFaces = CurrentlyX ? selected.GoodChances : selected.BadChances;
            var oFaces = CurrentlyX ? selected.BadChances : selected.GoodChances;
            Die.AssignFaces(xFaces, oFaces);

            StartCoroutine(Die.Roll(OnRollComplete));
        }

        private void OnRollComplete(SquareContents result)
        {
            if (result != SquareContents.Empty)
            {
                selectedSquare.HandlePlacement(result == SquareContents.X);
            }
            SetCurrentPlayer(!CurrentlyX);

            CurrentState = State.Selecting;
        }

        void SetCurrentPlayer(bool playerX)
        {
            CurrentlyX = playerX;
            
            foreach(var square in Squares)
            {
                square.HandlePlayerChange(CurrentlyX);
            }
        }
    }
}
