using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class World : MonoBehaviour {

	public static World current;

	public bool isInitialized;

	public readonly List<Player> players = new ();

	private List<GameObject> _allResources;
	private List<GameObject> _allBuildings;

	public bool enableBuildingsGeneration;

	private TerrainGenerator _terrainGenerator;
	private StartingBuildingsGenerator _startingBuildingsGenerator;
	private ResourceGenerator _resourceGenerator;
	private ForestGenerator _forestGenerator;

	public BlockPlacingChannelSO blockPlacingChanel;

	private bool _gameEnded;

	private Vector3 _centerOfWorld;
	public Vector3 centerOfWorld => _centerOfWorld;

	[FormerlySerializedAs("seed")]
	[SerializeField]
	private string _seed;

	private My2DArray<Chunk> _chunks;

	[NonSerialized]
	public static BlockTypes blockTypes;

	[FormerlySerializedAs("biome")]
	[SerializeField]
	private BiomeAttributes _biome;

	private void Awake() {
		current = this;
		SeedProcessor.initSeed(_seed);
		_forestGenerator = new ForestGenerator(this, _biome);
		_terrainGenerator = new TerrainGenerator(this, _biome);
		_resourceGenerator = new ResourceGenerator();


		_startingBuildingsGenerator = new StartingBuildingsGenerator(this);

		blockPlacingChanel.onBlockIdPlaced += placeBlock;
		blockPlacingChanel.onBlockDestroyed += destroyBlock;

	}

	private IEnumerator Start() {
		var centerOfWorldCoord = Mathf.FloorToInt(ChunkData.worldSizeInChunks / 2f);
		_centerOfWorld = new Vector3(centerOfWorldCoord, 0, centerOfWorldCoord);

		blockTypes = BlockTypes.current;

		_terrainGenerator.init();

		Application.targetFrameRate = 500;

		// Generate chunks
		_chunks = _terrainGenerator.createChunks(ChunkData.worldSizeInChunks, ChunkData.worldSizeInChunks);


		// Generate chunk.s render data
		for (var x = 0; x < ChunkData.worldSizeInChunks; ++x) {
			for (var z = 0; z < ChunkData.worldSizeInChunks; ++z) {
				_forestGenerator.generateForest(_chunks[x, z]);
				_resourceGenerator.generateResources(_chunks[x, z]);
				_chunks[x, z].updateMesh();
			}
		}

		// Generate chunk's navmesh data
		for (var x = 0; x < ChunkData.worldSizeInChunks; ++x) {
			for (var z = 0; z < ChunkData.worldSizeInChunks; ++z) {
				_chunks[x, z].updateNavMesh();
			}
		}

		// Wait for CoroutineStarter to initialize.
		while (CoroutineStarter.current == null) {
			yield return null;
		}

		// For now limited to 2 players. First one has to be the HumanPlayer
		players.Add(new HumanPlayer(PlayerColor.Blue));
		players.Add(new ComputerPlayer(PlayerColor.Red));

		foreach (var player in players) {
			player.initPlayerSystems();
		}

		// generate starting buildings
		if (enableBuildingsGeneration)
			_startingBuildingsGenerator.generateBuildings(players);

		_allResources = GameObject.FindGameObjectsWithTag("Resource").ToList();
		_allBuildings = GameObject.FindGameObjectsWithTag("Building").ToList();

		// Load resources
		for (var x = 0; x < ChunkData.worldSizeInChunks; ++x) {
			for (var z = 0; z < ChunkData.worldSizeInChunks; ++z) {
				_chunks[x, z].loadResources(_allResources);
			}
		}

		// Initialize player systems and base buildings buildings.
		foreach (var player in players) {
			player.loadBuildings(_allBuildings);
			player.startPlayerSystems();
		}

		isInitialized = true;
	}

	private void Update() {
		if (_gameEnded && Input.anyKeyDown) {
			StartCoroutine(waitToQuitProcess());
			return;
		}

		if (!isInitialized) {
			return;
		}

		// Check, if any base building has been destroyed
		foreach (var player in players) {
			if (player._buildings.Count < 1) {
				getPrimaryPlayer().inputSystem.uiChannel.endGame(player);
				_gameEnded = true;
				isInitialized = false;
			}
		}

	}

	/**
	 * Waits for 3 second before quiting a game.
	 * Created, so the player has some time realizing that the game has ended and not crashed.
	 */
	private IEnumerator waitToQuitProcess() {
		yield return new WaitForSeconds(3f);
		Application.Quit();
	}

	private void OnDestroy() {
		blockPlacingChanel.onBlockIdPlaced -= placeBlock;
		blockPlacingChanel.onBlockDestroyed -= destroyBlock;
		StopAllCoroutines();
	}

	/**
	 * Place block by its id on target global position.
	 */
	public void placeBlock(Vector3Int targetPos, BlockId blockId) {
		placeBlock(targetPos, BlockTypes.getBlock(targetPos, blockId));
	}

	/**
	 * Place block on target global position.
	 */
	public void placeBlock(Vector3Int targetPos, Block block) {

		var chunkCoord = getChunkCoords(targetPos);

		try {
			_chunks[chunkCoord.x, chunkCoord.z].editChunk(targetPos, block);
			updateNeighbours(targetPos);
		}
		catch (IndexOutOfRangeException) {
			// Block is outside the world. No need to check it. Empty catch block is therefore correct.
		}
	}

	/**
	 * Destroy block on target global position. Block is destroyed by replacing it with block of air.
	 */
	public void destroyBlock(Vector3Int targetPos) {
		placeBlock(targetPos, BlockTypes.getBlock(targetPos, BlockId.AIR));
	}

	/**
	 * Updated all "perpendicular" neighbours of the block, that sits at given global position absPos.
	 */
	public void updateNeighbours(Vector3Int absPos) {
		if (!gameObject.activeSelf) {
			return;
		}
		StartCoroutine(updateNeighboursCoroutine(absPos));
	}

	private IEnumerator updateNeighboursCoroutine(Vector3Int absPos) {
		foreach (Face face in Enum.GetValues(typeof(Face))) {
			var checkedPosition = absPos + ChunkData.getFaceNormal(face);
			if (getBlockAt(checkedPosition) is IUpdatableBlock updatableBlock) {
				updatableBlock.update();
			}

			yield return null;
		}
	}

	public BlockId getBlockIdAt(Vector3Int pos) {
		var block = getBlockAt(pos);
		return block?.blockId ?? BlockId.AIR;
	}

	public Block getBlockAt(Vector3Int pos) {
		if (!isBlockInsideWorld(pos))
			return new Void();

		var chunkCoords = new ChunkCoord(pos);
		var chunk = _chunks[chunkCoords.x, chunkCoords.z];
		return chunk.getBlockFromAbsolutePos(pos);
	}

	/**
	 *	Checks, if the chunk is inside the world
	 */
	public static bool isChunkInWorld(ChunkCoord coord) {
		return !(coord.x < 0 || coord.x > ChunkData.worldSizeInChunks - 1 ||
		         coord.z < 0 || coord.z > ChunkData.worldSizeInChunks - 1);
	}

	/**
	 * Checks, if the block is inside the world
	 */
	public static bool isBlockInsideWorld(Vector3Int pos) {
		return ! (pos.x < 0 || pos.x > ChunkData.worldSizeInVoxels - 1 ||
		          pos.y < 0 || pos.y > ChunkData.chunkHeight - 1 ||
		          pos.z < 0 || pos.z > ChunkData.worldSizeInVoxels - 1);
	}

	public static ChunkCoord getChunkCoords(Vector3Int pos) {
		var x = Mathf.FloorToInt(pos.x * 1f / ChunkData.chunkWidth );
		var z = Mathf.FloorToInt(pos.z * 1f / ChunkData.chunkWidth );
		return new ChunkCoord(x, z);
	}

	public Chunk getChunk(Vector3Int pos) {
		ChunkCoord coord = getChunkCoords(pos);
		return getChunk(coord);
	}

	public Chunk getChunk(ChunkCoord coord) {
		if (isChunkInWorld(coord))
			return _chunks[coord.x, coord.z];
		return null;
	}

	public Chunk getChunk(Vector3 pos) {
		return getChunk(Vector3Int.FloorToInt(pos));
	}

	/**
	 * Return the player, that was created first. Human player should be created first for this to work.
	 * Method is used by presenters, which are set in Unity editor. It is placeholder for better communication with UI layer
	 */
	public Player getPrimaryPlayer() {
		if (players.Count == 0) {
			return null;
		}
		return players[0];
	}

}
