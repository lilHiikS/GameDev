using UnityEngine;

public class Mover : MonoBehaviour
{
    public float speed = 5f;
    public Transform targetPosition;
    private bool move = false;

    void Update()
    {
        if (move)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition.position) < 0.1f)
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
