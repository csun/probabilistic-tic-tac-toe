using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTTT
{
    public class BoardWinSequence
    {
        static readonly float[] COUNT_SCORES = new float[3] { 1f, 2f, 10f };
        const float CERTAINTY_BONUS = 1f;

        public GameSquare[] Squares = new GameSquare[3];
        public float OScore;
        public WinState State = WinState.Inconclusive;

        public BoardWinSequence()
        {
            Reset();
        }

        public void Reset()
        {
            OScore = COUNT_SCORES[0];
            State = WinState.Inconclusive;
        }

        public void UpdateScoreAndState()
        {

            int xCount = 0;
            int oCount = 0;
            int emptyCount = 0;

            OScore = 0;
            foreach (var square in Squares)
            {
                if (square.CurrentContents == SquareContents.X) { xCount++; }
                else if (square.CurrentContents == SquareContents.O) { oCount++; }
                else {
                    emptyCount++;
                    OScore += CERTAINTY_BONUS * (Mathf.Max(square.GoodChances, square.BadChances) / 20f);
                }
            }

            if (xCount == 3)
            {
                State = WinState.XWin;
            }
            else if (oCount == 3)
            {
                State = WinState.OWin;
            }
            else if (xCount > 0 && oCount > 0)
            {
                State = WinState.Stalemate;
            }
            else
            {
                State = WinState.Inconclusive;
            }

            OScore = (State == WinState.Inconclusive) ? OScore + COUNT_SCORES[Mathf.Max(xCount, oCount)] : 0;
        }
    }
}
