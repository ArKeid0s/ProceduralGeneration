using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeGenerator : MonoBehaviour
{
    // Define the biome radius on the x and the y (biome will be 2x bigger)
    public Vector2 biomeSize = new Vector2(8, 8);

    // Store the biome rooms
    Room[,] rooms;

    // TEMP: Is it the best solution ?
    List<Vector2> takenPositions = new List<Vector2>();

    int gridSizeX, gridSizeY;

    public int numberOfRooms = 48;

    public GameObject minimapGo;

    private void Start()
    {
        // Verify that there is no more rooms than cells in the grid
        if (numberOfRooms >= (biomeSize.x * 2) * (biomeSize.y * 2))
        {
            numberOfRooms = Mathf.RoundToInt((biomeSize.x * 2) * (biomeSize.y * 2));
        }

        gridSizeX = Mathf.RoundToInt(biomeSize.x);
        gridSizeY = Mathf.RoundToInt(biomeSize.y);

        CreateRooms();
        SetRoomDoors();
        DrawMap();
        GetComponent<SheetAssigner>().Assign(rooms);
    }

    /// <summary>
    /// Create the different rooms at random positions and store the created ones inside the rooms list and fill the takenPositions
    /// </summary>
    private void CreateRooms()
    {
        // Setup
        rooms = new Room[gridSizeX * 2, gridSizeY * 2];
        rooms[gridSizeX, gridSizeY] = new Room(Vector2.zero, 1);
        takenPositions.Insert(0, Vector2.zero);
        Vector2 checkPos = Vector2.zero;

        // Math values
        float randomCompare = 0.2f, randomCompareStart = 0.2f, randomCompareEnd = 0.01f;

        /*
         * Add Rooms
         */
        for (int i = 0; i < numberOfRooms - 1; i++)
        {
            float randomPerc = ((float)i) / (((float)numberOfRooms - 1));
            // The furthure you loop the less there is a chance to branch out
            randomCompare = Mathf.Lerp(randomCompareStart, randomCompareEnd, randomPerc);

            // Grab new position to spawn a room
            checkPos = NewPosition();

            // Test new position
            /*
             * This if statement make it so the rooms are not so clamped and branch out
             */
            if (NumberOfNeighbors(checkPos, takenPositions) > 1 && Random.value > randomCompare)
            {
                int iterations = 0;
                do
                {
                    checkPos = SelectiveNewPosition();
                    iterations++;
                } while (NumberOfNeighbors(checkPos, takenPositions) > 1 && iterations < 100);

                if (iterations >= 50)
                {
                    print("error: could not create with fewer neighbors than: " + NumberOfNeighbors(checkPos, takenPositions));
                }
            }

            // Finalize position
            rooms[(int)checkPos.x + gridSizeX, (int)checkPos.y + gridSizeY] = new Room(checkPos, 0);
            takenPositions.Insert(0, checkPos);
        }

    }

    /// <summary>
    /// Set the door booleans and the typeId of each rooms
    /// </summary>
    private void SetRoomDoors()
    {
        for (int x = 0; x < (gridSizeX * 2); x++)
        {
            for (int y = 0; y < (gridSizeY * 2); y++)
            {
                if (rooms[x, y] == null)
                {
                    continue;
                }

                Vector2 gridPosition = new Vector2(x, y);

                if (y - 1 < 0) // Check above
                {
                    rooms[x, y].doorBot = false;
                }
                else
                {
                    rooms[x, y].doorBot = (rooms[x, y - 1] != null);
                    if (rooms[x, y].doorBot) rooms[x, y].typeId += 1;
                }

                if (y + 1 >= gridSizeY * 2) // Check below
                {
                    rooms[x, y].doorTop = false;
                }
                else
                {
                    rooms[x, y].doorTop = (rooms[x, y + 1] != null);
                    if (rooms[x, y].doorTop) rooms[x, y].typeId += 4;
                }

                if (x - 1 < 0) // Check left
                {
                    rooms[x, y].doorLeft = false;
                }
                else
                {
                    rooms[x, y].doorLeft = (rooms[x - 1, y] != null);
                    if (rooms[x, y].doorLeft) rooms[x, y].typeId += 2;
                }

                if (x + 1 >= gridSizeX * 2) // Check right
                {
                    rooms[x, y].doorRight = false;
                }
                else
                {
                    rooms[x, y].doorRight = (rooms[x + 1, y] != null);
                    if (rooms[x, y].doorRight) rooms[x, y].typeId += 8;
                }
            }
        }
    }


    /// <summary>
    /// Find a new position in the array based on the taken positions to spawn a new room BUT with only 1 neighbor
    /// </summary>
    /// <returns>Vector2 checkingPos</returns>
    private Vector2 SelectiveNewPosition()
    {
        int index = 0, inc = 0;
        int x = 0, y = 0;
        Vector2 checkingPos = Vector2.zero;
        do
        {
            inc = 0;
            do
            {
                // Pick random taken position in the array
                index = Mathf.RoundToInt(Random.value * (takenPositions.Count - 1));
                inc++;
            } while (NumberOfNeighbors(takenPositions[index], takenPositions) > 1 && inc < 100);

            x = (int)takenPositions[index].x;
            y = (int)takenPositions[index].y;
            bool UpDown = (Random.value < 0.5f);
            bool LeftRight = (Random.value < 0.5f);

            if (UpDown)
            {
                if (LeftRight)
                {
                    y += 1;
                }
                else
                {
                    y -= 1;
                }
            }
            else
            {
                if (LeftRight)
                {
                    x += 1;
                }
                else
                {
                    x -= 1;
                }
            }

            checkingPos = new Vector2(x, y);
        } while (takenPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY);
        {
            if (inc >= 100)
            {
                print("error: could not find position with onely one neighbor");
            }
            return checkingPos;
        }
    }


    /// <summary>
    /// Find a new position in the array based on the taken positions to spawn a new room
    /// </summary>
    /// <returns>Vector2 checkingPos</returns>
    private Vector2 NewPosition()
    {
        int x = 0, y = 0;
        Vector2 checkingPos = Vector2.zero;
        do
        {
            // Pick random taken position in the array
            int index = Mathf.RoundToInt(Random.value * (takenPositions.Count - 1));
            x = (int)takenPositions[index].x;
            y = (int)takenPositions[index].y;
            bool UpDown = (Random.value < 0.5f);
            bool LeftRight = (Random.value < 0.5f);

            if (UpDown)
            {
                if (LeftRight)
                {
                    y += 1;
                }
                else
                {
                    y -= 1;
                }
            }
            else
            {
                if (LeftRight)
                {
                    x += 1;
                }
                else
                {
                    x -= 1;
                }
            }

            checkingPos = new Vector2(x, y);
        } while (takenPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY);
        {
            return checkingPos;
        }
    }

    /// <summary>
    /// Look into the takenPositions if the room at the checkingPos has neighbors
    /// </summary>
    /// <param name="checkingPos"></param>
    /// <param name="takenPositions"></param>
    /// <returns>int amount</returns>
    private int NumberOfNeighbors(Vector2 checkingPos, List<Vector2> takenPositions)
    {
        int amount = 0;

        if (takenPositions.Contains(checkingPos + Vector2.right))
        {
            amount++;
        }

        if (takenPositions.Contains(checkingPos + Vector2.left))
        {
            amount++;
        }

        if (takenPositions.Contains(checkingPos + Vector2.up))
        {
            amount++;
        }

        if (takenPositions.Contains(checkingPos + Vector2.down))
        {
            amount++;
        }

        return amount;
    }
    private void DrawMap()
    {
        foreach (Room room in rooms)
        {
            if (room == null)
            {
                continue;
            }

            Vector2 drawPos = room.gridPos;
            drawPos.x *= 12;
            drawPos.y *= 12;
            MapSpriteSelector mapper = Object.Instantiate(minimapGo, drawPos, Quaternion.identity).GetComponent<MapSpriteSelector>();
            mapper.type = room.type;
            mapper.typeId = room.typeId;
            print("id:" + room.gridPos + "doors: \n" + "bot " + room.doorBot + "\ntop " + room.doorTop + "\nleft " + room.doorLeft + "\nRight " + room.doorRight);
        }
    }

}

