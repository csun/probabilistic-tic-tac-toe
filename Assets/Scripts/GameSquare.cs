using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTTT
{
    public class GameSquare : MonoBehaviour
    {
        public GameManager Manager;

        public SquareContents CurrentContents;
        public float GoodChance;
        public float BadChance;

        public StatBar GoodBar;
        public StatBar NeutralBar;
        public StatBar BadBar;
        public TMPro.TMP_Text PlacedText;

        public void UpdateChanceText()
        {
            GoodBar.UpdateProbability(GoodChance);
            BadBar.UpdateProbability(BadChance);
            NeutralBar.UpdateProbability(1 - (GoodChance + BadChance));
        }

        public void OnMouseDown()
        {
            if (CurrentContents != SquareContents.Empty)
            {
                return;
            }

            var rand = Random.value;
            if (rand <= GoodChance)
            {
                HandlePlayerChange(Manager.CurrentlyX);
            }
            else if (1 - rand <= BadChance)
            {
                HandlePlayerChange(!Manager.CurrentlyX);
            }

            Manager.SetCurrentPlayer(!Manager.CurrentlyX);
        }

        private void HandlePlayerChange(bool playerIsX)
        {
            PlacedText.text = playerIsX ? "X" : "O";
            PlacedText.gameObject.SetActive(true);
            CurrentContents = playerIsX ? SquareContents.X : SquareContents.O;

            GoodBar.gameObject.SetActive(false);
            BadBar.gameObject.SetActive(false);
            NeutralBar.gameObject.SetActive(false);
        }
    }
}
