using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleSelector : MonoBehaviour, ISelector
{
    public PlayerController player;
    public float maxAngle;
    public float minAngle;
    public float speed;

    private bool isIncreasing = true;
    private bool indicatorRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        StopIndicator();
        HideIndicator();
    }

    // Update is called once per frame
    void Update()
    {
        if (!indicatorRunning) { return; }

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

    [ContextMenu("Start Angle Indicator")]
    public void StartIndicator()
    {
        transform.RotateAround(player.transform.position, new Vector3(0, 0, 1), minAngle);
        indicatorRunning = true;
        this.gameObject.SetActive(true);
    }

    [ContextMenu("Stop Angle Indicator")]
    public void StopIndicator()
    {
        player.SetAngle(transform.eulerAngles.z);
        indicatorRunning = false;
    }

    [ContextMenu("Hide Angle Indicator")]
    public void HideIndicator()
    {
        this.gameObject.SetActive(false);
    }
}
