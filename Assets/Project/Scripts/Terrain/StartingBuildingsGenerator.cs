using System.Collections.Generic;
using UnityEngine;

public class StartingBuildingsGenerator {
    private readonly World _world;

    private GameObject _gameObject;

    public StartingBuildingsGenerator(World world) {
	    _world = world;
    }

    private List<Chunk> findStartingChunks(int numberOfPlayers) {
	    var startingChunks = new List<Chunk>();

	    int xChunkPos = Random.Range(0, ChunkData.worldSizeInChunks);
	    int yChunkPos = 0;

	    startingChunks.Add(_world.getChunk(new ChunkCoord(xChunkPos, yChunkPos)));

	    xChunkPos = Random.Range(0, ChunkData.worldSizeInChunks);
	    yChunkPos = ChunkData.worldSizeInChunks - 1;

	    startingChunks.Add(_world.getChunk(new ChunkCoord(xChunkPos, yChunkPos)));

	    return startingChunks;
    }

    private List<Vector3Int> findStartingPositions(int numberOfPlayers) {

	    var startingPositions = new List<Vector3Int>();

	    var startingChunks = findStartingChunks(numberOfPlayers);

	    int freeHeight;

	    Vector2Int startPos;

	    int generateAttempts = 0;
	    int maximumAttempts = ChunkData.chunkWidth * ChunkData.chunkWidth;

	    foreach (var startingChunk in startingChunks) {

		    // This could be problem, if there is no available space on the chosen chunk
		    do {
			    ++generateAttempts;

			    startPos = new Vector2Int(
				    Random.Range(0, ChunkData.chunkWidth),
				    Random.Range(0, ChunkData.chunkWidth)
			    );

			    freeHeight = startingChunk.getFirstHeightWithNonAirBlock(startPos, ignoreStructures:true) + 1;

		    } while (freeHeight < 0);

		    startPos += new Vector2Int(startingChunk.coords.x, startingChunk.coords.z) * ChunkData.chunkWidth;

		    startingPositions.Add(new Vector3Int(startPos.x, freeHeight, startPos.y));

	    }

	    return startingPositions;
    }

    public void generateBuildings(List<Player> players) {

	    Random.InitState(SeedProcessor.getNextByteValue());

	    var startingPositions = findStartingPositions(players.Count);

	    foreach (var player in players) {

		    var startPosIdx = Random.Range(0, startingPositions.Count);
		    var startPos = startingPositions[startPosIdx];
		    startingPositions.RemoveAt(startPosIdx);

		    _gameObject = Object.Instantiate(Resources.Load("Prefabs/Structures/Warehouse"), startPos, Quaternion.identity, _world.transform) as GameObject;
		    _gameObject.GetComponentInChildren<EntityCmp>().owner = player;
	    }


    }

}
