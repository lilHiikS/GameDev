using UnityEngine;

public class WeightItem : MonoBehaviour
{
    public bool isGrounded = false;
    public ScalePuzzle scalePuzzle;

    private Transform startParent;
    private bool isQuitting = false;

    void Start()
    {
        startParent = transform.parent;
    }

    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Plate"))
        {
            SetGrounded(true);
        }
        else if (collider.CompareTag("Weight"))
        {
            var otherWeight = collider.GetComponent<WeightItem>();
            if (otherWeight != null && otherWeight.isGrounded)
            {
                SetGrounded(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        SetGrounded(false);
    }

    void SetGrounded(bool grounded)
    {
        if (isQuitting) return;

        if (isGrounded != grounded)
        {
            isGrounded = grounded;
            if (isGrounded)
            {
                if (scalePuzzle.leftHandle.gameObject.activeInHierarchy)
                    transform.SetParent(scalePuzzle.leftHandle);
            }
            else
            {
                if (startParent.gameObject.activeInHierarchy)
                    transform.SetParent(startParent);
            }

            if (scalePuzzle != null)
            {
                if (isGrounded)
                    scalePuzzle.AddLeftObject(GetComponent<Rigidbody2D>());
                else
                    scalePuzzle.RemoveLeftObject(GetComponent<Rigidbody2D>());
            }
        }
    }
}
