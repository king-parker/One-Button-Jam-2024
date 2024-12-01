using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLogic : MonoBehaviour
{
    public float freeDistance = -16;
    public PlayerController player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        float distance = transform.position.x - player.transform.position.x;
        if (distance <= freeDistance)
        {
            Destroy(gameObject);
        }
    }
}
