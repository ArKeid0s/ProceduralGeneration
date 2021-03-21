using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInstance : MonoBehaviour
{
    public Texture2D texture;

    public Color doorTopColor, doorBotColor, doorLeftColor, doorRightColor;

    [HideInInspector] public Vector2 gridPos;
    public int type;
    [HideInInspector] public bool doorTop, doorBot, doorLeft, doorRight;
    [SerializeField] private GameObject doorU, doorD, doorL, doorR, doorWall;
    //TODO: Automate this by looking into a specific folder
    [SerializeField] private ColorToGameObject[] mappings;
    private float tileSize = 16;
    private Vector2 roomSizeInTiles = new Vector2(45, 23);

    public void Setup(Texture2D _texture, Vector2 _gridPos, int _type, bool _doorTop, bool _doorBot, bool _doorLeft,
        bool _doorRight)
    {
        texture = _texture;
        gridPos = _gridPos;
        type = _type;
        doorTop = _doorTop;
        doorBot = _doorBot;
        doorLeft = _doorLeft;
        doorRight = _doorRight;
        print("setup");
        GenerateRoomTiles();
    }

    /*
     * TODO: THIS IS LOOPING INFINITELY CHECK TEXTURE WIDTH AND HEIGHT
     */
    private void GenerateRoomTiles()
    {
        print("Width: " + texture.width + "\n Height: " + texture.height);
        /*
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                print("generate");
                //MakeDoors(x, y);
                //GenerateTile(x, y);
            }
        }
        */
    }

    private void GenerateTile(int x, int y)
    {
        Color pixelColor = texture.GetPixel(x, y);
        if (pixelColor.a == 0 || pixelColor.Equals(doorTopColor) || pixelColor.Equals(doorBotColor) || pixelColor.Equals(doorLeftColor) || pixelColor.Equals(doorRightColor))
        {
            return;
        }

        foreach (ColorToGameObject mapping in mappings)
        {
            if (mapping.color.Equals(pixelColor))
            {
                Vector3 spawnPos = PositionFromTileGrid(x, y);
                Instantiate(mapping.prefab, spawnPos, Quaternion.identity).transform.parent = this.transform;
            }
            else
            {
                print(mapping.color + ", " + pixelColor);
            }
        }
    }

    private Vector2 PositionFromTileGrid(int x, int y)
    {
        Vector3 ret;
        Vector3 offset = new Vector3((-roomSizeInTiles.x + 1) * tileSize,
            (roomSizeInTiles.y / 4) * tileSize - (tileSize / 4), 0);
        ret = new Vector3(tileSize * (float) x, -tileSize * (float) y, 0) + offset + transform.position;
        return ret;
    }

    private void MakeDoors(int x, int y)
    {
        /*
         * TODO: If false replace by walls
         */

        Color pixelColor = texture.GetPixel(x, y);
        if (pixelColor.a == 0 || !pixelColor.Equals(doorTopColor) || !pixelColor.Equals(doorBotColor) || !pixelColor.Equals(doorLeftColor) || !pixelColor.Equals(doorRightColor))
        {
            return;
        }

        if (doorTopColor.Equals(pixelColor) && doorTop)
        {
            Vector3 spawnPos = PositionFromTileGrid(x, y);
            PlaceDoor(spawnPos, doorTop, doorU);
        }

        if (doorBotColor.Equals(pixelColor) && doorBot)
        {
            Vector3 spawnPos = PositionFromTileGrid(x, y);
            PlaceDoor(spawnPos, doorBot, doorD);
        }

        if (doorLeftColor.Equals(pixelColor) && doorLeft)
        {
            Vector3 spawnPos = PositionFromTileGrid(x, y);
            PlaceDoor(spawnPos, doorLeft, doorL);
        }

        if (doorRightColor.Equals(pixelColor) && doorRight)
        {
            Vector3 spawnPos = PositionFromTileGrid(x, y);
            PlaceDoor(spawnPos, doorRight, doorR);
        }
    }

    private void PlaceDoor(Vector3 spawnPos, bool door, GameObject doorPrefab)
    {
        Instantiate(doorPrefab, spawnPos, Quaternion.identity).transform.parent = transform;
    }
}
