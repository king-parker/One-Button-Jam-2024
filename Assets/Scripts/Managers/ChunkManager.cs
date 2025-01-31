using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [Header("Basic Chunk Settings")]
    public float spawnDistance = 40f;
    public float baseChunkWidth = 10f;
    public float chunkFloorY = -4;
    // Spawn doesn't start at -10 so there a base amount of base blocks with no hazards
    public float startSpawnPosition = 20f;

    [Header("Safe Chunk Settings")]
    public GameObject baseChunk;
    public float safeProgression = 1000f;
    public float minSafeChance = 0.05f;
    public float maxSafeChance = 0.5f;

    [Header("Hazard Chunks")]
    [SerializeField] private List<HazardChunk> hazardChunks;
    private ChunkType lastChunkType = ChunkType.NoneOrSafe;

    [Header("Player Reference")]
    public Transform player;

    private float nextSpawnPositionX;
    private float currentChunkWidth;

    void Start()
    {
        foreach (HazardChunk chunk in hazardChunks)
        {
            chunk.Setup();
        }

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

        foreach (HazardChunk chunk in hazardChunks)
        {
            chunk.RecordLastChunkType(lastChunkType);
        }
    }

    private void SpawnChunk()
    {
        GameObject chunkPrefab = ChooseChunkType();

        GameObject newChunk = Instantiate(chunkPrefab, new Vector3(nextSpawnPositionX, chunkFloorY, 0), Quaternion.identity, this.transform);

        nextSpawnPositionX += currentChunkWidth;
    }

    private GameObject ChooseChunkType()
    {
        float progress = Mathf.Clamp01(player.position.x / safeProgression);

        float safeChance = Mathf.Lerp(maxSafeChance, minSafeChance, progress);
        bool isSafeChunk = Random.value < safeChance;

        if (isSafeChunk)
        {
            // TODO: Update base chunk width selection
            currentChunkWidth = baseChunkWidth;
            return baseChunk;
        }
        else
        {
            return ChooseHazardChunk();
        }
    }

    private GameObject ChooseHazardChunk()
    {

        float totalWeight = 0f;
        for (int i = 0; i < hazardChunks.Count; i++)
        {
            HazardChunk chunk = hazardChunks[i];

            chunk.ProgressionUpdate(player.position.x);

            if (chunk.IsUnlocked())
            {
                chunk.SetSelectRangeStart(totalWeight);
                chunk.RuleUpdate();
                totalWeight += chunk.GetWeight();
            }
        }

        float spawnValue = Random.Range(0f, totalWeight);

        GameObject spawnChunk = null;
        ChunkType spawnedType = ChunkType.NoneOrSafe;
        for (int i = 0; i < hazardChunks.Count; i++)
        {
            HazardChunk chunk = hazardChunks[i];

            // Make the chunk selection range inclusive if it is the last chunk in the list
            bool isTopInclusive = i == hazardChunks.Count - 1;

            if (chunk.IsChunkSelected(spawnValue, isTopInclusive))
            {
                spawnChunk = chunk.GetChunk();
                spawnedType = chunk.GetChunkType();
                currentChunkWidth = chunk.GetChunkWidth();
                break;
            }
        }

        if (spawnChunk != null)
        {
            lastChunkType = spawnedType;
            return spawnChunk;
        }
        else
        {
            // DEBUG
            UnityEngine.Debug.LogWarning($"No spawn chunk was selected. TotalWeight = {totalWeight}; SpawnValue = {spawnValue}");
            lastChunkType = ChunkType.NoneOrSafe;
            currentChunkWidth = baseChunkWidth;
            return baseChunk;
        }
    }
}
