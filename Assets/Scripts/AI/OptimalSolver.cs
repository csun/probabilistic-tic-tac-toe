using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Profiling;

// See https://louisabraham.github.io/articles/probabilistic-tic-tac-toe for an explanation of this algorithm

namespace PTTT
{
    public class OptimalSolver
    {

        public static (double, int)[] HullIntersection(List<(double, double)> f, List<(double, double)> g)
        {
            double a = 0, b = 1;
            while (b - a > 1e-9)
            {
                double x = (a + b) / 2;
                double y = f.Max(t => t.Item1 * x + t.Item2);
                double x1 = g.Min(t => t.Item1 * y + t.Item2);
                if (x1 < x)
                    b = x;
                else
                    a = x;
            }
            double finalX = (a + b) / 2;
            var (finalY, i) = f.Select((t, idx) => (Value: t.Item1 * finalX + t.Item2, Index: idx)).Max();
            var (minX, j) = g.Select((t, idx) => (Value: t.Item1 * finalY + t.Item2, Index: idx)).Min();
            return new[] { (finalY, i), (minX, j) };
        }

        public static T[] Apply<T>(T[] state, int cell, T player)
        {
            T[] newState = (T[])state.Clone();
            newState[cell] = player;
            return newState;
        }

        public static char? Winner(char?[] state)
        {
            for (int i = 0; i < 3; i++)
            {
                if (state[i] == state[i + 3] && state[i] == state[i + 6] && state[i] != null)
                    return state[i];
                if (state[3 * i] == state[3 * i + 1] && state[3 * i] == state[3 * i + 2] && state[3 * i] != null)
                    return state[3 * i];
            }
            if (state[0] == state[4] && state[0] == state[8] && state[0] != null)
                return state[0];
            if (state[2] == state[4] && state[2] == state[6] && state[2] != null)
                return state[2];
            return null;
        }

        public static List<int> AvailableCells(char?[] state)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < state.Length; i++)
            {
                if (state[i] == null)
                    result.Add(i);
            }
            return result;
        }

        private static readonly Dictionary<string, ((double, int?), (double, int?))> ValueCache = new Dictionary<string, ((double, int?), (double, int?))>();
        private readonly GameSquare[] squares;
        public OptimalSolver(List<GameSquare> squares)
        {
            this.squares = squares.ToArray();
        }

        public ((double, int?), (double, int?)) Value(char?[] state = null)
        {
            if (state == null)
                state = new char?[9];

            var key = string.Join(',', state.Select(s => s ?? ' '));
            if (ValueCache.TryGetValue(key, out var cachedValue))
                return cachedValue;

            var w = Winner(state);
            if (w == 'x')
                return ((1, (int?)null), (1, (int?)null));
            if (w == 'o')
                return ((0, (int?)null), (0, (int?)null));

            var cells = AvailableCells(state);
            if (!cells.Any())
                return ((0.5, (int?)null), (0.5, (int?)null));

            var f = new List<(double, double)>();
            var g = new List<(double, double)>();
            foreach (var cell in cells)
            {
                double success = (double)squares[cell].GoodChances / 20;
                double neutral = (double)squares[cell].NeutralChances / 20;
                double failure = (double)squares[cell].BadChances / 20;
                var s1 = Apply(state, cell, 'x');
                var ((v1, _), (vp1, _)) = Value(s1);
                var s2 = Apply(state, cell, 'o');
                var ((v2, _), (vp2, _)) = Value(s2);
                double x_c = success * vp1 + failure * vp2;
                double xp_c = success * v2 + failure * v1;
                f.Add((neutral, x_c));
                g.Add((neutral, xp_c));
            }

            var sol = HullIntersection(f, g);
            var (v, i) = sol[0];
            var (vp, ip) = sol[1];
            var result = ((v, cells[i]), (vp, cells[ip]));
            ValueCache[key] = result;
            return result;
        }

        public void UpdateBoardWinChances(bool currentlyX)
        {
            var state = StateFromBoard();
            var cells = AvailableCells(state);

            foreach (var cell in cells)
            {
                ((var currentXVal, _), (var currentOVal, _)) = Value(state);
                var opponentNeutralVal = currentlyX ? currentOVal : currentXVal;

                var goodState = Apply(state, cell, currentlyX ? 'x' : 'o');
                ((var goodXVal, _), (var goodOVal, _)) = Value(goodState);
                var opponentGoodVal = currentlyX ? goodOVal : goodXVal;

                var badState = Apply(state, cell, currentlyX ? 'o' : 'x');
                ((var badXVal, _), (var badOVal, _)) = Value(badState);
                var opponentBadVal = currentlyX ? badOVal : badXVal;

                double goodChance = (double)squares[cell].GoodChances / 20;
                double neutralChance = (double)squares[cell].NeutralChances / 20;
                double badChance = (double)squares[cell].BadChances / 20;

                var combinedVal = 
                    (float)((goodChance * opponentGoodVal) + (neutralChance * opponentNeutralVal) + (badChance * opponentBadVal));
                // Val represents X's chance to win, so we must invert it if O is playing
                if (!currentlyX) { combinedVal = 1 - combinedVal; }

                squares[cell].WinChance = combinedVal;
            }
        }

        public GameSquare BestMoveForO()
        {
            (_, (double _, int? bestMove)) = Value(StateFromBoard());
            if (bestMove == null)
                throw new Exception("Best move is null");
            return squares[(int)bestMove];
        }

        private char?[] StateFromBoard()
        {
            char?[] state = new char?[9];
            for (int i = 0; i < 9; i++)
            {
                state[i] = squares[i].CurrentContents switch
                {
                    SquareContents.X => 'x',
                    SquareContents.O => 'o',
                    SquareContents.Empty => null,
                    _ => throw new Exception("Invalid square content")
                };
            }
            return state;
        }

        public void Reset()
        {
            ValueCache.Clear();
        }
    }
}