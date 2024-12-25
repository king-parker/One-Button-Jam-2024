using System;
using UnityEngine;

[Serializable]
public abstract class HazardChunk : ScriptableObject
{
    [SerializeField] protected GameObject[] chunks;
    [SerializeField] protected ChunkType chunkType;
    [SerializeField] protected float unlockDistance;
    [SerializeField] protected float maxProgression;
    [SerializeField] protected float minWeight;
    [SerializeField] protected float maxWeight;

    protected bool isUnlocked = false;
    protected float progressionWidth = 0f;
    protected float weight = 0f;
    protected float selectRangeStart = 0f;

    public abstract void RecordLastChunkType(ChunkType lastChunkSpawn);
    public abstract void RuleUpdate();

    public GameObject GetChunk()
    {
        return chunks[UnityEngine.Random.Range(0, chunks.Length)];
    }

    public ChunkType GetChunkType() { return chunkType; }

    public bool IsUnlocked() { return isUnlocked; }

    public float GetWeight() { return weight; }

    public void SetSelectRangeStart(float prevTotalWeight) { selectRangeStart = prevTotalWeight; }

    public void Setup()
    {
        progressionWidth = maxProgression - unlockDistance;
    }

    public void ProgressionUpdate(float playerDistance)
    {
        if (!isUnlocked && playerDistance >= unlockDistance) { isUnlocked = true; }

        if (isUnlocked)
        {
            float progression = Mathf.Clamp01(playerDistance / progressionWidth);
            weight = Mathf.Lerp(minWeight, maxWeight, progression);
        }
        else
        {
            weight = 0f;
        }
    }

    public bool IsChunkSelected(float selectValue, bool rangeTopInclusive = false)
    {
        if (!isUnlocked) { return false; }

        if (rangeTopInclusive)
        {
            if (selectValue <= (selectRangeStart + weight) && selectValue >= selectRangeStart) { return true; }
        }
        else
        {
            if (selectValue < (selectRangeStart + weight) && selectValue >= selectRangeStart) { return true; }
        }

        return false;
    }
}