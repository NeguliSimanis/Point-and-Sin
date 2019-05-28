using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EatItemButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector]
    public bool isHighlighted = false;
    bool isItemSelected = false;
    bool isButtonClicked = false;

    Color highlightColor = new Color(0.620f, 0.020f, 0.051f, 1.000f);
    Color defaultColor = Color.black;
    Color fangDefaultColor;
    Color fangHighlightColor = Color.white;

    Image buttonImage;
    [SerializeField]
    Image fangImage;
    [SerializeField]
    Text buttonText;

    private void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(HandleButtonClick);
        fangDefaultColor = fangImage.color;
        buttonImage = gameObject.GetComponent<Image>();
    }

     
    void HandleButtonClick()
    {
        if (isItemSelected)
        {
            EatItem();
            HighlightButton(false);
        }
        else
        {
            if (isHighlighted && isButtonClicked)
            {
                HighlightButton(false);
            }
            else if (!isButtonClicked)
            {
                isButtonClicked = true;
                HighlightButton(true);
            }
        }
    }

    void EatItem()
    {

    }

    /// <summary>
    /// This method is used instead of onMouseEnter, because it cannot be used for UI elements
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isHighlighted)
        {
            HighlightButton();
        }
    }

    /// <summary>
    /// This method is used instead of onMouseExit, because it cannot be used for UI elements
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isHighlighted && !isItemSelected && !isButtonClicked)
        {
            HighlightButton(false);
        }
    }

    private void HighlightButton(bool highlight = true)
    {
        // enable highlight
        if (highlight)
        {
            // set colors
            buttonText.color = highlightColor;
            buttonImage.color = highlightColor;
            fangImage.color = fangHighlightColor;

            isHighlighted = true;
        }
        // disable highlight
        else
        {
            // set colors
            buttonText.color = defaultColor;
            buttonImage.color = defaultColor;
            fangImage.color = fangDefaultColor;

            isHighlighted = false;
            isButtonClicked = false;
        }
    }

    private void OnDisable()
    {
        Debug.Log("man na");
        HighlightButton(false);
    }
}
