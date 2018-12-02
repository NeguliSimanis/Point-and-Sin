using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSpriteOnHover : MonoBehaviour
{

    [SerializeField]
    Sprite spriteOnMouseOver;

    SpriteRenderer spriteRenderer;
    Sprite defaultSprite;

    private void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        defaultSprite = spriteRenderer.sprite;
    }

    private void OnMouseEnter()
    {
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
