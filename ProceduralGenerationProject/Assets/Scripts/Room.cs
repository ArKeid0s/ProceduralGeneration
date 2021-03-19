using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public Vector2 gridPos;

    // Room type change the structure is needed (enum ?)
    public int type;
    // Room id based on the doors
    public int typeId;

    public bool doorTop, doorBot, doorLeft, doorRight;

    public Room(Vector2 _gridPos, int _type)
    {
        gridPos = _gridPos;
        type = _type;
    }
}