using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLogic : MonoBehaviour
{
    public float freeDistance = -16;
    public PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = transform.position.x - player.transform.position.x;
        if (distance <= freeDistance)
        {
            Destroy(gameObject);
        }
    }
}
