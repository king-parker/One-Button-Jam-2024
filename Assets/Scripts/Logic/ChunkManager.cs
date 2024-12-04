using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public GameObject baseChunk;
    public GameObject singleSpikeChunk;
    public GameObject spikeChunk;
    public float chunkWidth = 4f;
    public float spawnDistance = 40f;
    public float chunkFloorY = -4;
    public Transform player;

    private float nextSpawnPositionX = 20f; // Doesn't start at -4 so there a base amount of base blocks with no hazards

    void Start()
    {
        ManageChunks();
    }

    void Update()
    {
        ManageChunks();
    }

    private void ManageChunks()
    {
        while (player.position.x + spawnDistance > nextSpawnPositionX)
        {
            SpawnChunk();
        }
    }

    private void SpawnChunk()
    {
        GameObject chunkPrefab = ChooseChunkType();

        GameObject newChunk = Instantiate(chunkPrefab, new Vector3(nextSpawnPositionX, chunkFloorY, 0), Quaternion.identity, this.transform);

        nextSpawnPositionX += chunkWidth;
    }

    private GameObject ChooseChunkType()
    {
        int randomIndex = Random.Range(0, 3);

        switch (randomIndex)
        {
            case 0: return baseChunk;
            case 1: return singleSpikeChunk;
            case 2: return spikeChunk;
            default: return baseChunk;
        }
    }
}
