using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [Header("Chunk Prefabs")]
    public GameObject baseChunk;
    public GameObject spikeHalfRight;
    public GameObject spikeHalfLeft;

    [Header("Basic Chunk Settings")]
    public float chunkWidth = 10f;
    public float spawnDistance = 40f;
    public float chunkFloorY = -4;
    // Spawn doesn't start at -10 so there a base amount of base blocks with no hazards
    public float startSpawnPosition = 20f;

    [Header("Progression Settings")]
    public float maxProgression = 1000f;
    public float hazardProgression = 1500f;
    public float minSafeChance = 0.05f;
    public float maxSafeChance = 0.5f;
    public float minHalfSpikeChance = 0.3f;
    public float maxHalfSpikeChance = 0.7f;

    [Header("Player Reference")]
    public Transform player;

    private float nextSpawnPositionX;

    void Start()
    {
        nextSpawnPositionX = startSpawnPosition;
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
        float progress = Mathf.Clamp01(player.position.x / maxProgression);

        float safeChance = Mathf.Lerp(maxSafeChance, minSafeChance, progress);
        bool isSafeChunk = Random.value < safeChance;

        if (isSafeChunk)
        {
            return baseChunk;
        }
        else
        {
            return ChooseHazardChunk();
        }
    }

    private GameObject ChooseHazardChunk()
    {
        float hazardProgress = Mathf.Clamp01(player.position.x / hazardProgression);
        float sideSpikesChance = Mathf.Lerp(maxHalfSpikeChance, minHalfSpikeChance, hazardProgress);

        float spawnValue = Random.value;
        if (spawnValue < sideSpikesChance)
        {
            return ChooseSpikeSide();
        }
        else
        {
            return ChooseSpikeSide();
        }
    }

    private GameObject ChooseSpikeSide()
    {
        int sideSelect = Random.Range(0, 2);
        if (sideSelect == 0)
        {
            return spikeHalfLeft;
        }
        else
        {
            return spikeHalfRight;
        }
    }
}
