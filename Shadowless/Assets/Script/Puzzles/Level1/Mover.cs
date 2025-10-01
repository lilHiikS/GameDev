using UnityEngine;

public class Mover : MonoBehaviour
{
    public float speed = 5f;
    private bool move = false;
    public float moveDistance = 5f;

    void Update()
    {
        if (move)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.right * moveDistance, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, transform.position + transform.right * moveDistance) < 0.1f)
            {
                move = false;
            }
        }
    }

    public void TriggerMove()
    {
        move = true;
    }
}
