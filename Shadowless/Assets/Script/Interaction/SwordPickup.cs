using UnityEngine;

public class SwordPickup : MonoBehaviour, IInteractable
{
    public GameObject promptUI; // Assign your Canvas/UI in the inspector

    void Start()
    {
        // Make sure UI is hidden at start
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Show UI when player is near
            if (promptUI != null)
                promptUI.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Hide UI when player leaves
            if (promptUI != null)
                promptUI.SetActive(false);
        }
    }

    public void Interact()
    {
        // Add sword to player inventory or equip it
        Debug.Log("Picked up: " + gameObject.name);
        
        // Enable player attack
        if (PlayerController2D.Instance != null && PlayerController2D.Instance.playerAttack != null)
        {
            PlayerController2D.Instance.playerAttack.hasWeapon = true;
            Debug.Log("Player can now attack!");
        }

        // Hide UI after pickup
        if (promptUI != null)
            promptUI.SetActive(false);
        
        // Trigger sword tutorial
        SwordTutorial tutorial = FindAnyObjectByType<SwordTutorial>();
        if (tutorial != null)
        {
            tutorial.StartTutorial();
        }
        
        // Disable the sword (you can change this to Destroy(gameObject) if you prefer)
        gameObject.SetActive(false);
    }
}
