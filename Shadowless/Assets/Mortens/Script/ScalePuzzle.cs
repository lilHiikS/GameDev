using UnityEngine;
using System.Collections.Generic;

public class ScalePuzzle : MonoBehaviour
{
    public Transform bar, leftHandle, rightHandle;
    public Collider2D leftTrigger;
    public float rotationSpeed = 2f;
    public float maxAngle = 30f;
    public float rightWeight = 20f;
    public float leftWeight = 0f;
    public Mover doorMover;
    public float weightSensitivity = 0.2f;

    [SerializeField]
    private List<Rigidbody2D> leftObjects = new List<Rigidbody2D>();


    void Update()
    {
        leftWeight = 0f;
        foreach (var obj in leftObjects)
        {
            leftWeight += obj.mass;
        }

        if (rightWeight == leftWeight)
        {
            doorMover.TriggerMove();
        }

        float weightDiff = leftWeight - rightWeight;
        float normalizedDiff = Mathf.Atan(weightDiff * weightSensitivity) / (Mathf.PI / 2);
        float targetAngle = normalizedDiff * maxAngle;

        float currentAngle = bar.localEulerAngles.z;
        if (currentAngle > 180f) currentAngle -= 360f;

        float newAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * rotationSpeed);

        bar.localEulerAngles = new Vector3(0, 0, newAngle);

        leftHandle.rotation = Quaternion.Euler(0, 0, 0);
        rightHandle.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void AddLeftObject(Rigidbody2D rb)
    {
        if (!leftObjects.Contains(rb))
            leftObjects.Add(rb);
    }

    public void RemoveLeftObject(Rigidbody2D rb)
    {
        leftObjects.Remove(rb);
    }
}