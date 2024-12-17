using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [Header("Chunk Prefabs")]
    public GameObject baseChunk;
    public GameObject spikeHalfRight;
    public GameObject spikeHalfLeft;
    public GameObject spikeEdges;
    public GameObject movingPlatformShort;
    public GameObject gapSmall;
    public GameObject gapLarge;
    public GameObject wall;
    public GameObject spikeCeiling;

    [Header("Basic Chunk Settings")]
    public float chunkWidth = 10f;
    public float spawnDistance = 40f;
    public float chunkFloorY = -4;
    // Spawn doesn't start at -10 so there a base amount of base blocks with no hazards
    public float startSpawnPosition = 20f;

    [Header("Progression Settings")]
    public float safeProgression = 1000f;
    public float hazardProgression = 1500f;
    public float minSafeChance = 0.05f;
    public float maxSafeChance = 0.5f;
    public float minHalfSpikeWeight = 0.3f;
    public float maxHalfSpikeWeight = 0.7f;
    public float minEdgeSpikeWeight = 0.3f;
    public float maxEdgeSpikeWeight = 0.7f;
    public float minMovingPlatformShortWeight = 0.3f;
    public float maxMovingPlatformShortWeight = 0.7f;
    public float minSmallGapWeight = 0.3f;
    public float maxSmallGapWeight = 0.7f;
    public float minLargeGapWeight = 0.3f;
    public float maxLargeGapWeight = 0.7f;
    public float minWallWeight = 0.3f;
    public float maxWallWeight = 0.7f;
    public float minCeilingSpikeWeight = 0.3f;
    public float maxCeilingSpikeWeight = 0.7f;

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
        float progress = Mathf.Clamp01(player.position.x / safeProgression);

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
        float sideSpikesWeight = Mathf.Lerp(maxHalfSpikeWeight, minHalfSpikeWeight, hazardProgress);
        float spikeEdgesWeight = Mathf.Lerp(maxEdgeSpikeWeight, minEdgeSpikeWeight, hazardProgress);
        float movingPlatformShortWeight = Mathf.Lerp(maxMovingPlatformShortWeight, minMovingPlatformShortWeight, hazardProgress);
        float gapSmallWeight = Mathf.Lerp(maxSmallGapWeight, minSmallGapWeight, hazardProgress);
        float gapLargeWeight = Mathf.Lerp(maxLargeGapWeight, minLargeGapWeight, hazardProgress);
        float wallWeight = Mathf.Lerp(maxWallWeight, minWallWeight, hazardProgress);
        float spikeCeilingWeight = Mathf.Lerp(maxCeilingSpikeWeight, minCeilingSpikeWeight, hazardProgress);

        float totalWeight = sideSpikesWeight + spikeEdgesWeight + movingPlatformShortWeight + gapSmallWeight + gapLargeWeight + wallWeight + spikeCeilingWeight;

        float spawnValue = Random.Range(0, totalWeight);
        if (spawnValue < sideSpikesWeight)
        {
            return ChooseSpikeSide();
        }
        else if (spawnValue < sideSpikesWeight + spikeEdgesWeight)
        {
            return spikeEdges;
        }
        else if (spawnValue < sideSpikesWeight + spikeEdgesWeight + movingPlatformShortWeight)
        {
            return movingPlatformShort;
        }
        else if (spawnValue < sideSpikesWeight + spikeEdgesWeight + movingPlatformShortWeight + gapSmallWeight)
        {
            return gapSmall;
        }
        else if (spawnValue < sideSpikesWeight + spikeEdgesWeight + movingPlatformShortWeight + gapSmallWeight + gapLargeWeight)
        {
            return gapLarge;
        }
        else if (spawnValue < sideSpikesWeight + spikeEdgesWeight + movingPlatformShortWeight + gapSmallWeight + gapLargeWeight + wallWeight)
        {
            return wall;
        }
        else
        {
            return spikeCeiling;
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
