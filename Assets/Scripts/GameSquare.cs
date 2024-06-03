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

        public SpriteRenderer ContentsRenderer;

        public TMPro.TMP_Text GoodText;
        public TMPro.TMP_Text BadText;
        public TMPro.TMP_Text NeutralText;

        public void UpdateChanceText()
        {
            GoodText.text = (GoodChance * 100).ToString("0.#");
            BadText.text = (BadChance * 100).ToString("0.#");
            NeutralText.text = ((1 - (GoodChance + BadChance)) * 100).ToString("0.#");
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
            ContentsRenderer.sprite = Manager.SpriteForPlayer(playerIsX);
            CurrentContents = playerIsX ? SquareContents.X : SquareContents.O;

            GoodText.gameObject.SetActive(false);
            BadText.gameObject.SetActive(false);
            NeutralText.gameObject.SetActive(false);
        }
    }
}
