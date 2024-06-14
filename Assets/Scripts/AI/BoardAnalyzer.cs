using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Profiling;

namespace PTTT
{
    public partial class BoardAnalyzer
    {
        const float INACTION_PENALTY = 1.2f;

        private List<BoardWinSequence> winSequences = new();
        private Dictionary<GameSquare, List<BoardWinSequence>> sequencesForSquare = new();

        private OptimalSolver solver;


        private int Idx(int row, int col) => row * 3 + col;

        public BoardAnalyzer(List<GameSquare> squares)
        {
            void AddSeq(BoardWinSequence sequence)
            {
                winSequences.Add(sequence);
                foreach (var square in sequence.Squares)
                {
                    if (!sequencesForSquare.ContainsKey(square))
                    {
                        sequencesForSquare[square] = new();
                    }
                    sequencesForSquare[square].Add(sequence);
                }
            }

            // Add row sequences
            for (var row = 0; row < 3; row++)
            {
                var seq = new BoardWinSequence();
                for (var col = 0; col < 3; col++)
                {
                    seq.Squares[col] = squares[Idx(row, col)];
                }
                AddSeq(seq);
            }

            // Add col sequences
            for (var col = 0; col < 3; col++)
            {
                var seq = new BoardWinSequence();
                for (var row = 0; row < 3; row++)
                {
                    seq.Squares[row] = squares[Idx(row, col)];
                }
                AddSeq(seq);
            }

            // Add diagonal sequences
            var lrDiag = new GameSquare[3]
            {
                squares[Idx(0, 0)],
                squares[Idx(1, 1)],
                squares[Idx(2, 2)]
            };
            AddSeq(new BoardWinSequence { Squares = lrDiag });
            var rlDiag = new GameSquare[3]
            {
                squares[Idx(0, 2)],
                squares[Idx(1, 1)],
                squares[Idx(2, 0)]
            };
            AddSeq(new BoardWinSequence { Squares = rlDiag });

            solver = new OptimalSolver(squares);

        }

        public void Reset()
        {
            foreach (var seq in winSequences)
            {
                seq.Reset();
            }
            solver.Reset();
        }

#if SIMMODE
        public GameSquare RandomValidMove()
        {
            var validMoves = new List<GameSquare>();
            foreach (var square in sequencesForSquare.Keys)
            {
               if (square.CurrentContents == SquareContents.Empty) { validMoves.Add(square); }
            }

            return validMoves[Random.Range(0, validMoves.Count)];
        }
#endif

        public void UpdateBoardWinChances(bool currentlyX)
        {
            solver.UpdateBoardWinChances(currentlyX);
        }

        public GameSquare BestMoveForO(bool optimal=true)
        {
            ProfilerMarker profiler = new ProfilerMarker("PTTT.BestMoveForO");

            using (profiler.Auto())
            {
                if (optimal)
                {
                    return solver.BestMoveForO();
                }

                var bestScore = float.MinValue;
                GameSquare bestSquare = null;

                foreach (var square in sequencesForSquare.Keys)
                {
                    if (square.CurrentContents != SquareContents.Empty) { continue; }
                    var score = 0f;
                    foreach (var seq in sequencesForSquare[square])
                    {
                        score += seq.OScore;
                    }

                    var ratio = (square.GoodChances - square.BadChances) / 20f;
                    score *= ratio;
                    score -= square.NeutralChances * INACTION_PENALTY;

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestSquare = square;
                    }
                }

                return bestSquare;
            }
        }

        public WinState GetWinState(GameSquare lastMove)
        {
            foreach (var seq in sequencesForSquare[lastMove])
            {
                seq.UpdateScoreAndState();
                if (seq.State != WinState.Inconclusive && seq.State != WinState.Stalemate)
                {
                    return seq.State;
                }
            }

            foreach (var seq in winSequences)
            {
                if (seq.State == WinState.Inconclusive) { return WinState.Inconclusive; }
            }

            return WinState.Stalemate;
        }
    }
}
