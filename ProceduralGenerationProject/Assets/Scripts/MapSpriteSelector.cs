using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpriteSelector : MonoBehaviour
{
    public Sprite[] sprites;

    public int type; // 0: normal 1: enter
    public int typeId;

    public Color enterColor;

    SpriteRenderer sRenderer;

    private void Start()
    {
        sRenderer = GetComponent<SpriteRenderer>();

        PickSprite();
        PickColor();
    }

    private void PickSprite()
    {
        // Set the sprite based on the index in the sprites List
        sRenderer.sprite = sprites[Mathf.Clamp(typeId - 1, 0, sprites.Length)];
    }

    private void PickColor()
    {
        if (type == 1)
        {
            sRenderer.color = enterColor;

        }
    }
}