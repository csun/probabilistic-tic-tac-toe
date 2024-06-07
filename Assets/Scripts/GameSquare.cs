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

        public int TotalBlinks;
        public float FirstBlinksOnTime;
        public float FirstBlinksOffTime;
        public float FinalBlinkHoldTime;

        public Color StatUnselectedColor;
        public Color StatSelectedColor;
        public Color BackgroundUnselectedColor;
        public Color BackgroundSelectedColor;
        public Color ContentsUnselectedColor;
        public Color ContentsSelectedColor;

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
            PlacedText.gameObject.SetActive(false);

            GoodBar.UpdateProbability(GoodChances / 20.0f);
            BadBar.UpdateProbability(BadChances / 20.0f);
            NeutralBar.UpdateProbability(1 - ((GoodChances + BadChances) / 20.0f));
        }
        public void HandlePlacement(SquareContents contents, System.Action onPlacementComplete)
        {
            if (contents != SquareContents.Empty)
            {
                PlacedText.text = contents == SquareContents.X ? "X" : "O";
                PlacedText.gameObject.SetActive(true);
                CurrentContents = contents;

                GoodBar.gameObject.SetActive(false);
                BadBar.gameObject.SetActive(false);
                NeutralBar.gameObject.SetActive(false);
            }

            StartCoroutine(Blink(onPlacementComplete));
        }

        private IEnumerator Blink(System.Action onPlacementComplete)
        {
            for (var i = 0; i < TotalBlinks - 1; i++)
            {
                UnHighlight();
                yield return new WaitForSeconds(FirstBlinksOffTime);
                Highlight();
                yield return new WaitForSeconds(FirstBlinksOnTime);
            }
            UnHighlight();
            yield return new WaitForSeconds(FirstBlinksOffTime);
            Highlight();
            yield return new WaitForSeconds(FinalBlinkHoldTime);
            UnHighlight();
            onPlacementComplete();
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
            PlacedText.color = ContentsSelectedColor;
            Background.color = BackgroundSelectedColor;
        }

        private void UnHighlight()
        {
            GoodBar.UpdateColor(StatUnselectedColor);
            BadBar.UpdateColor(StatUnselectedColor);
            NeutralBar.UpdateColor(StatUnselectedColor);
            PlacedText.color = ContentsUnselectedColor;
            Background.color = BackgroundUnselectedColor;
        }
    }
}
