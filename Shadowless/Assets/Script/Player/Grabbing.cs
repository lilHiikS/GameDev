using UnityEngine;
using UnityEngine.InputSystem;

public class Grabbing : MonoBehaviour
{
    public float grabRange = 2f;
    public LayerMask grabbableLayer;
    public Rigidbody2D playerRb;
    public float grabSpeed = 15f;

    [Header("Audio")]
    [SerializeField] private AudioSource grabSound;

    private Transform grabbedObject;
    private Rigidbody2D grabbedRb;
    private Collider2D grabbedCollider;
    private Collider2D playerCollider;
    private int originalLayer;
    public Transform hoveredObject;

    void Awake()
    {
        playerCollider = playerRb.GetComponent<Collider2D>();
    }

    void Update()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, Camera.main.nearClipPlane));
        Vector2 mousePos2D = new Vector2(mouseWorld.x, mouseWorld.y);

        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, 0f, grabbableLayer);
        Transform newHovered = null;

        if (hit.collider != null)
        {
            float dist = Vector2.Distance(transform.position, hit.collider.transform.position);
            if (dist <= grabRange)
            {
                newHovered = hit.collider.transform;
            }
        }

        if (hoveredObject != null && hoveredObject != newHovered)
        {
            var sr = hoveredObject.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.material.DisableKeyword("_USE_OUTLINE");
            hoveredObject = null;
        }

        if (newHovered != null && newHovered != hoveredObject)
        {
            var sr = newHovered.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.material.EnableKeyword("_USE_OUTLINE");
            hoveredObject = newHovered;
        }
    }

    void FixedUpdate()
    {
        if (grabbedObject != null)
        {
            FollowCursor();
        }
    }

    public void TryGrab(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 mouseScreen = Mouse.current.position.ReadValue();
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, Camera.main.nearClipPlane));
            Vector2 mousePos2D = new Vector2(mouseWorld.x, mouseWorld.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, 0f, grabbableLayer);
            if (hit.collider != null)
            {
                float dist = Vector2.Distance(transform.position, hit.collider.transform.position);
                if (dist <= grabRange)
                {
                    grabbedObject = hit.collider.transform;
                    grabbedRb = grabbedObject.GetComponent<Rigidbody2D>();
                    grabbedCollider = hit.collider;

                    originalLayer = grabbedObject.gameObject.layer;
                    grabbedObject.gameObject.layer = LayerMask.NameToLayer("Default");

                    if (grabbedCollider != null && playerCollider != null)
                    {
                        Physics2D.IgnoreCollision(grabbedCollider, playerCollider, true);
                    }

                    // Play grab sound
                    if (grabSound != null)
                    {
                        grabSound.Play();
                    }
                }
            }
        }
        else if (context.canceled)
        {
            if (grabbedObject != null)
            {
                Physics2D.IgnoreCollision(grabbedCollider, playerCollider, false);

                grabbedObject.gameObject.layer = originalLayer;

                grabbedCollider = null;
                grabbedObject = null;
                grabbedRb = null;
            }
        }
    }

    private void FollowCursor()
    {
        if (grabbedObject == null) return;

        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, Camera.main.nearClipPlane));
        Vector2 playerPos = transform.position;
        Vector2 targetPos = new Vector2(mouseWorld.x, mouseWorld.y);

        Vector2 clampedPos = playerPos + Vector2.ClampMagnitude(targetPos - playerPos, grabRange);

        if (grabbedRb == null)
            grabbedRb = grabbedObject.GetComponent<Rigidbody2D>();

        Vector2 currentPos = grabbedObject.position;
        Vector2 lerpedPos = Vector2.Lerp(currentPos, clampedPos, Time.deltaTime * grabSpeed);

        if (grabbedRb != null)
            grabbedRb.MovePosition(lerpedPos);
    }
}
