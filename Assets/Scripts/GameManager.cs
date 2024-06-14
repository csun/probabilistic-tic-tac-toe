using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PTTT
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public enum State
        {
            Selecting,
            Rolling,
            Retracting,
            DisplayingWinner,
            InMenu
        }

        public Die Die;
        public List<GameSquare> Squares;

        public MenuManager MenuManager;

        public int MaxNeutralChances;
        public int MinGoodChances;
        public bool IsOptimalDifficulty { get; private set; }
        public bool ShowWinProbabilities { get; private set; }
        public bool IsSingleplayer { get; private set; }
        public bool CurrentlyX { get; private set; }
        public State CurrentState { get; private set; } = State.Selecting;

        public ScoreIndicator XScore;
        public ScoreIndicator TieScore;
        public ScoreIndicator OScore;

        public DieBoundsResizer DieBoundsResizer;

        public UnityEvent OnGUIRefresh = new();

        private bool xStartNextGame = true;
        private GameSquare selectedSquare;
        private SquareContents lastRollResult;
        private BoardAnalyzer analyzer;

// Define SIMMODE to have the cpu play against completely random moves
#if SIMMODE
        private bool shouldResetNextTurn;

        private void Update()
        {
            if (shouldResetNextTurn)
            {
                shouldResetNextTurn = false;
                ResetBoard();
            }
        }
#endif

        void Start()
        {
            Instance = this;

            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;

            Time.fixedDeltaTime = 1.0f / Application.targetFrameRate;

            XScore.DefaultHighlightStateChecker = () => CurrentlyX;
            OScore.DefaultHighlightStateChecker = () => !CurrentlyX;

            analyzer = new(Squares);

            UpdateSettings(singleplayer: true, optimalDifficulty: false, showWinProbabilities: false);
            OnGUIRefresh.Invoke();
            DieBoundsResizer.Recalculate();
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public void UpdateSettings(bool singleplayer, bool optimalDifficulty, bool showWinProbabilities)
        {
            var shouldReset = singleplayer != IsSingleplayer || optimalDifficulty != IsOptimalDifficulty;

            IsSingleplayer = singleplayer;
            IsOptimalDifficulty = optimalDifficulty;
            ShowWinProbabilities = showWinProbabilities;

            OScore.HeaderText.text = singleplayer ? "CPU - O" : "Player - O";

            if (shouldReset)
            {
                xStartNextGame = true;
                ResetScore();
                ResetBoard();
            }
            else
            {
                OnGUIRefresh.Invoke();
            }
        }

        public void OpenMenu()
        {
            CurrentState = State.InMenu;
            StartCoroutine(MenuManager.OpenMenu(() => { }));
            OnGUIRefresh.Invoke();
        }

        public void CloseMenu()
        {
            CurrentState = State.Selecting;
            StartCoroutine(MenuManager.CloseMenu(() => { }));
            OnGUIRefresh.Invoke();
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

            foreach(var square in Squares)
            {
                square.Manager = this;

                var neutralChances = Random.Range(1, MaxNeutralChances + 1);  // Note that this is using the int overload of random range
                square.GoodChances = Random.Range(MinGoodChances, 20 - neutralChances);  // Ensure that there's at least 1 bad chance
                square.BadChances = 20 - (square.GoodChances + neutralChances);

                square.Reset();
            }

            analyzer.Reset();

            SetCurrentPlayer(xStartNextGame);
            xStartNextGame = !xStartNextGame;

            OnGUIRefresh.Invoke();
        }

        public void OnSquareSelect(GameSquare selected)
        {
            CurrentState = State.Rolling;
            selectedSquare = selected;
#if SIMMODE
            var rand = Random.Range(0, 20);
            if (rand < selected.GoodChances)
            {
                lastRollResult = CurrentlyX ? SquareContents.X : SquareContents.O;
            }
            else if (rand < selected.GoodChances + selected.BadChances)
            {
                lastRollResult = CurrentlyX ? SquareContents.O : SquareContents.X;
            }
            else
            {
                lastRollResult = SquareContents.Empty;
            }
            selected.HandlePlacement(lastRollResult, OnPlacementComplete);
#else
            selected.DefaultHighlightStateChecker = () => true;
            DieBoundsResizer.Recalculate();
            Die.AssignFaces(selected.GoodChances, selected.BadChances);

            StartCoroutine(Die.Roll(CurrentlyX, OnRollComplete));
#endif
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
            var winstate = analyzer.GetWinState(selectedSquare);

            ScoreIndicator winner;
            switch (winstate)
            {
                case WinState.XWin:
                    winner = XScore;
                    break;
                case WinState.OWin:
                    winner = OScore;
                    break;
                case WinState.Stalemate:
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
                OnGUIRefresh.Invoke();
            }
            else
            {
#if SIMMODE
                winner.Count++;
                shouldResetNextTurn = true;
#else
                CurrentState = State.DisplayingWinner;
                StartCoroutine(DisplayWinner(winner));
#endif
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

            if (!CurrentlyX && IsSingleplayer)
            {
                DoCPUMove();
            }
#if SIMMODE
            else
            {
                OnSquareSelect(analyzer.RandomValidMove());
            }
#endif
        }

        void DoCPUMove()
        {
            foreach (var square in Squares) { square.UnHighlight(); }
            OnSquareSelect(analyzer.BestMoveForO(IsOptimalDifficulty));
        }
    }
}
