using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLogic : MonoBehaviour
{
    public float freeDistance = 16;
    public PlayerController player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        float distance = player.transform.position.x - transform.position.x;
        if (distance >= freeDistance)
        {
            print("Name: " + this.name + "\nDistance: " + distance + "\nPosition: " + transform.position);
            Destroy(gameObject);
        }
    }
}
