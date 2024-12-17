using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Target Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Platform Properties")]
    public float speed = 2f;
    public float directionChangeTolerance = 0.01f;

    private Vector3 targetPos;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        targetPos = pointB.position;
    }

    void FixedUpdate()
    {
        Vector3 newPos = Vector3.MoveTowards(transform.position, targetPos, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        if (Vector3.Distance(transform.position, targetPos) < directionChangeTolerance)
        {
            targetPos = (targetPos == pointA.position) ? pointB.position : pointA.position;
        }
    }

    public Vector3 GetVelocity()
    {

        return (targetPos - transform.position).normalized * speed;
    }
}
