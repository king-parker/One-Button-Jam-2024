using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayer
{
    private float angle = 0;
    private float power = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetAngle(float angleValue)
    {
        angle = angleValue;
    }

    public void SetPower(float powerValue)
    {
        power = powerValue;
    }
}
