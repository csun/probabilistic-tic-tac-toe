using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTTT
{
    public class BoardAnalyzer
    {
        public enum WinState
        {
            Inconclusive,
            Stalemate,
            XWin,
            OWin
        }

        private List<GameSquare[]> winSequences = new();
        private int Idx(int row, int col) => row * 3 + col;

        public BoardAnalyzer(List<GameSquare> squares)
        {
            // Add row sequences
            for (var row = 0; row < 3; row++)
            {
                var seq = new GameSquare[3];
                for (var col = 0; col < 3; col++)
                {
                    seq[col] = squares[Idx(row, col)];
                }
                winSequences.Add(seq);
            }

            // Add col sequences
            for (var col = 0; col < 3; col++)
            {
                var seq = new GameSquare[3];
                for (var row = 0; row < 3; row++)
                {
                    seq[row] = squares[Idx(row, col)];
                }
                winSequences.Add(seq);
            }

            // Add diagonal sequences
            var lrDiag = new GameSquare[3]
            {
                squares[Idx(0, 0)],
                squares[Idx(1, 1)],
                squares[Idx(2, 2)]
            };
            winSequences.Add(lrDiag);
            var rlDiag = new GameSquare[3]
            {
                squares[Idx(0, 2)],
                squares[Idx(1, 1)],
                squares[Idx(2, 0)]
            };
            winSequences.Add(rlDiag);
        }

        public WinState GetWinState()
        {
            var inconclusiveFound = false;
            foreach (var seq in winSequences)
            {
                var state = SequenceState(seq);
                if (state != WinState.Inconclusive && state != WinState.Stalemate)
                {
                    return state;
                }
                else if (state == WinState.Inconclusive)
                {
                    inconclusiveFound = true;
                }
            }

            return inconclusiveFound ? WinState.Inconclusive : WinState.Stalemate;
        }

        private WinState SequenceState(GameSquare[] sequence)
        {
            int xCount = 0;
            int oCount = 0;
            int emptyCount = 0;

            foreach (var square in sequence)
            {
                if (square.CurrentContents == SquareContents.X) { xCount++; }
                else if (square.CurrentContents == SquareContents.O) { oCount++; }
                else { emptyCount++; }
            }

            if (xCount == 3)
            {
                return WinState.XWin;
            }
            else if (oCount == 3)
            {
                return WinState.OWin;
            }
            else if (xCount > 0 && oCount > 0)
            {
                return WinState.Stalemate;
            }
            else
            {
                return WinState.Inconclusive;
            }
        }
    }
}
