using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpawn : MonoBehaviour
{
    public GameObject wallSection;
    public int maxWallHeight = 4;
    public float blockHeight = 2;

    void Start()
    {
        int layersToAdd = Random.Range(0, maxWallHeight);
        float chunkX = this.transform.position.x;
        float chunkY = this.transform.position.y;

        float spawnHeight = chunkY + 6f;

        for (int i = 0; i < layersToAdd; i++)
        {
            Instantiate(wallSection, new Vector3(chunkX, spawnHeight, 0), Quaternion.identity, this.transform);
            spawnHeight += blockHeight;
        }
    }
}
