using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSpriteOnHover : MonoBehaviour
{
    public Sprite spriteOnMouseOver;

    SpriteRenderer spriteRenderer;
    public Sprite defaultSprite;

    private void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        defaultSprite = spriteRenderer.sprite;
    }

    private void OnMouseEnter()
    {
        //Debug.Log("mouse over!");
        spriteRenderer.sprite = spriteOnMouseOver;
    }

    private void OnMouseOver()
    {
        spriteRenderer.sprite = spriteOnMouseOver;
    }

    private void OnMouseExit()
    {
        spriteRenderer.sprite = defaultSprite;
    }
}
