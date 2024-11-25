using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleSelector : MonoBehaviour
{
    public GameObject player;
    public float maxAngle;
    public float minAngle;
    public float speed;

    private bool isIncreasing = true;

    // Start is called before the first frame update
    void Start()
    {
        transform.RotateAround(player.transform.position, new Vector3(0, 0, 1), minAngle);
    }

    // Update is called once per frame
    void Update()
    {
        float angleCheck;
        bool changeDirections = false;
        float angleChange;

        if (isIncreasing)
        {
            angleCheck = maxAngle - transform.eulerAngles.z;
        }
        else
        {
            angleCheck = transform.eulerAngles.z - minAngle;
        }

        if (angleCheck < speed)
        {
            angleChange = angleCheck;
            changeDirections = true;
        }
        else
        {
            angleChange = speed;
        }

        transform.RotateAround(player.transform.position, new Vector3(0, 0, 1), isIncreasing ? angleChange : -angleChange);

        if (changeDirections) { isIncreasing = !isIncreasing; }
    }
}
