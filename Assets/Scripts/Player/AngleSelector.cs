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
    private float initialX;
    private float initialY;

    // Start is called before the first frame update
    void Start()
    {
        transform.RotateAround(player.transform.position, new Vector3(0, 0, 1), minAngle);
        initialX = transform.position.x;
        initialY = transform.position.y;

        //StopIndicator();
        if (player.GetState() != PlayerController.PlayerState.AngleSelect)  { HideIndicator(); }
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
        transform.position = new Vector3(initialX + player.transform.position.x, initialY + player.transform.position.y, 0);
        transform.rotation = Quaternion.AngleAxis(minAngle, Vector3.forward);
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
