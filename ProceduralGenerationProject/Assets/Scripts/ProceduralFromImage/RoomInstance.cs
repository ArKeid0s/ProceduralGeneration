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
    private Vector2 roomSizeInTiles = new Vector2(9, 17);

    public void Setup(Texture2D _texture, Vector2 _gridPos, int _type, bool _doorTop, bool _doorBot, bool _doorLeft, bool _doorRight){
		texture = _texture;
		gridPos = _gridPos;
		type = _type;
		doorTop = _doorTop;
		doorBot = _doorBot;
		doorLeft = _doorLeft;
		doorRight = _doorRight;
		MakeDoors();
		GenerateRoomTiles();
	}
	void MakeDoors(){
		//top door, get position then spawn
		Vector3 spawnPos = transform.position + Vector3.up*(roomSizeInTiles.y/4 * tileSize) - Vector3.up*(tileSize/4);
		PlaceDoor(spawnPos, doorTop, doorU);
		//bottom door
		spawnPos = transform.position + Vector3.down*(roomSizeInTiles.y/4 * tileSize) - Vector3.down*(tileSize/4);
		PlaceDoor(spawnPos, doorBot, doorD);
		//right door
		spawnPos = transform.position + Vector3.right*(roomSizeInTiles.x * tileSize) - Vector3.right*(tileSize);
		PlaceDoor(spawnPos, doorRight, doorR);
		//left door
		spawnPos = transform.position + Vector3.left*(roomSizeInTiles.x * tileSize) - Vector3.left*(tileSize);
		PlaceDoor(spawnPos, doorLeft, doorL);
	}
	void PlaceDoor(Vector3 spawnPos, bool door, GameObject doorSpawn){
		// check whether its a door or wall, then spawn
		if (door){
			Instantiate(doorSpawn, spawnPos, Quaternion.identity).transform.parent = transform;
		}else{
			Instantiate(doorWall, spawnPos, Quaternion.identity).transform.parent = transform;
		}
	}
	void GenerateRoomTiles(){
		//loop through every pixel of the textureture
		for(int x = 0; x < texture.width; x++){
			for (int y = 0; y < texture.height; y++){
				GenerateTile(x,y);
			}
		}
	}
	void GenerateTile(int x, int y){
		Color pixelColor = texture.GetPixel(x,y);
		//skip clear spaces in textureture
		if (pixelColor.a == 0){
			return;
		}
		//find the color to math the pixel
		foreach (ColorToGameObject mapping in mappings){
			if (mapping.color.Equals(pixelColor)){
				Vector3 spawnPos = positionFromTileGrid(x,y);
				Instantiate(mapping.prefab, spawnPos, Quaternion.identity).transform.parent = this.transform;
			}else{
				//forgot to remove the old print for the tutorial lol so I'll leave it here too
				//print(mapping.color + ", " + pixelColor);
			}
		}
	}
	Vector3 positionFromTileGrid(int x, int y){
		Vector3 ret;
		//find difference between the corner of the textureture and the center of this object
		Vector3 offset = new Vector3((-roomSizeInTiles.x + 1)*tileSize, (roomSizeInTiles.y/4)*tileSize - (tileSize/4), 0);
		//find scaled up position at the offset
		ret = new Vector3(tileSize * (float) x, -tileSize * (float) y, 0) + offset + transform.position;
		return ret;
	}
}
