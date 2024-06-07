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
        public int MaxNeutralChances;
        public int MinGoodChances;
        public bool CurrentlyX { get; private set; }
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

            CurrentState = State.Placing;
            StartCoroutine(Die.Retract(OnPlaceComplete));
        }

        private void OnPlaceComplete()
        {
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
