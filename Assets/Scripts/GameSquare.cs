using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PTTT
{
    public class GameSquare : MonoBehaviour,
        IPointerClickHandler,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        public GameManager Manager;

        public SquareContents CurrentContents;
        public int GoodChances;
        public int BadChances;

        public Color StatUnselectedColor;
        public Color StatSelectedColor;
        public Color BackgroundUnselectedColor;
        public Color BackgroundSelectedColor;

        public Image Background;
        public StatBar GoodBar;
        public StatBar NeutralBar;
        public StatBar BadBar;
        public TMPro.TMP_Text PlacedText;

        public void Reset()
        {
            CurrentContents = SquareContents.Empty;
            GoodBar.gameObject.SetActive(true);
            BadBar.gameObject.SetActive(true);
            NeutralBar.gameObject.SetActive(true);
            Background.gameObject.SetActive(true);
            PlacedText.gameObject.SetActive(false);

            GoodBar.UpdateProbability(GoodChances / 20.0f);
            BadBar.UpdateProbability(BadChances / 20.0f);
            NeutralBar.UpdateProbability(1 - ((GoodChances + BadChances) / 20.0f));
        }
        public void HandlePlacement(bool placedX)
        {
            PlacedText.text = placedX ? "X" : "O";
            PlacedText.gameObject.SetActive(true);
            CurrentContents = placedX ? SquareContents.X : SquareContents.O;

            GoodBar.gameObject.SetActive(false);
            BadBar.gameObject.SetActive(false);
            NeutralBar.gameObject.SetActive(false);
            Background.gameObject.SetActive(false);
        }

        public void HandlePlayerChange(bool playerIsX)
        {
            GoodBar.PlayerText.text = playerIsX ? "X" : "O";
            BadBar.PlayerText.text = playerIsX ? "O" : "X";
            UnHighlight();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (CurrentContents != SquareContents.Empty || Manager.CurrentState != GameManager.State.Selecting) { return; }
            Manager.OnSquareSelect(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (CurrentContents != SquareContents.Empty || Manager.CurrentState != GameManager.State.Selecting) { return; }
            Highlight();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (CurrentContents != SquareContents.Empty || Manager.CurrentState != GameManager.State.Selecting) { return; }
            UnHighlight();
        }

        private void Highlight()
        {
            GoodBar.UpdateColor(StatSelectedColor);
            BadBar.UpdateColor(StatSelectedColor);
            NeutralBar.UpdateColor(StatSelectedColor);
            Background.color = BackgroundSelectedColor;
        }

        private void UnHighlight()
        {
            GoodBar.UpdateColor(StatUnselectedColor);
            BadBar.UpdateColor(StatUnselectedColor);
            NeutralBar.UpdateColor(StatUnselectedColor);
            Background.color = BackgroundUnselectedColor;
        }
    }
}
