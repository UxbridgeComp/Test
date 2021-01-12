using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralGenTEST : MonoBehaviour
{
	[Header("Terrain Gen")]
	[SerializeField] int width;  //width and height or our procedurally generatied terrain
	[SerializeField] int height;
	[SerializeField] float smoothness; //handles the smoothness of the procedural generation
	[SerializeField] float seed;
	int[] perlinHeightList;	

	[Header("Cave Gen")]
	[Range(0, 100)]
	[SerializeField] int randomFillPercent;
	[SerializeField] int smoothAmount;

	[Header("Tiles")]
	 //will help us create generation based on a seed
	[SerializeField] TileBase[] baseTiles; //This is the place where we will put our rule tile
	[SerializeField] Tilemap[] tileMap;

	int[,] map;

	// Start is called before the first frame update
    	void Start()
    	{
		perlinHeightList = new int[width];
		Generation();
    	}

	// Update is called once per frame
    	void Update()
    	{
        	if (Input.GetKeyDown(KeyCode.Space)){
			Generation();
		}
    	}

	void Generation(){
	
		ClearMap();
		map = GenerateArray(width, height, true);
		map = TerrainGeneration(map);
		SmoothMap(smoothAmount);   
		RenderMap(map, baseTiles, tileMap);
	}

public int[,] GenerateArray(int width, int height, bool empty){
	int[,] map = new int[width, height];
	for (int x = 0; x < width; x++)
	{
		for(int y = 0; y < height; y++){
			map[x, y] = (empty) ? 0:1;
		}
	}
	return map;
}

public int[,] TerrainGeneration(int[,] map){
	System.Random pesudoRandom = new System.Random(seed.GetHashCode());//gives us a random value based on the seed.
	int perlinHeight;
	seed = Random.Range(1, 10000);	
	for (int x = 0; x < map.GetLength(0); x++)
	{
		perlinHeight = Mathf.RoundToInt(Mathf.PerlinNoise(x/smoothness, seed)*height/2);
		perlinHeight += height/2;
		perlinHeightList[x] = perlinHeight;
		for(int y = 0; y < perlinHeight; y++){
			map[x, y] = (pesudoRandom.Next(1, 100)<randomFillPercent)? 1: 2;
			
		}
	}
	return map;
}


void SmoothMap(int smoothAmount){
	for (int i = 0; i < smoothAmount; i++){
		for (int x = 0; x < width; x++)
		{		
			for(int y = 0; y < perlinHeightList[x]; y++){
				if (x == 0 || y == 0 || x == width-1 || y == perlinHeightList[x]-1){
					map[x,y] = 1;
				} else {
					int surroundingGroundCount = GetSurroundingGroundCount(x, y);
					if (surroundingGroundCount>4){
						map[x,y] = 1;
					} else if (surroundingGroundCount<4){
						map[x,y] = 2;
					}
				}
			}
		}
	}	
}

int GetSurroundingGroundCount(int gridX, int gridY){
	int groundCount = 0;
	for (int nebX = gridX-1; nebX <= gridX+1; nebX++){
		for (int nebY = gridY-1; nebY <= gridY+1; nebY++){
			if (nebX>=0 && nebX<width && nebY>=0 && nebY<height){ //if we are inside the map
				if (nebX != gridX || nebY != gridY){ //excluding the middle tile
					if (map[nebX, nebY] == 1){
						groundCount++;
					}
				}
			}
		}
	}	
	return groundCount;
}

public void RenderMap(int[,] map, TileBase[] baseTiles, Tilemap[] tileMap){
	for (int x = 0; x < width; x++)
	{
		for(int y = 0; y < height; y++){
			if (map[x,y] == 1){
				tileMap[0].SetTile(new Vector3Int(x,y,0), baseTiles[0]);
			} else if (map[x, y] ==  2){
				tileMap[1].SetTile(new Vector3Int(x,y,0), baseTiles[1]);
			}
		}
	}
}

void ClearMap(){
	for(int i = 0; i < tileMap.Length; i++){
		tileMap[i].ClearAllTiles();
	}
	//groundTilemap.ClearAllTiles();
	//caveTilemap.ClearAllTiles();
}

}
