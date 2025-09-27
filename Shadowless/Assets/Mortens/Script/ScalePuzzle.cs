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

    [SerializeField]
    private List<Rigidbody2D> leftObjects = new List<Rigidbody2D>();

    void Update()
    {
        float weightDiff = leftWeight - rightWeight;
        float normalizedDiff = Mathf.Atan(weightDiff * 0.2f) / (Mathf.PI / 2);
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

        leftWeight = 0f;
        foreach (var b in leftObjects)
            if (b != null) leftWeight += b.mass;
    }

    public void RemoveLeftObject(Rigidbody2D rb)
    {
        leftObjects.Remove(rb);

        leftWeight = 0f;
        foreach (var b in leftObjects)
            if (b != null) leftWeight += b.mass;
    }
}