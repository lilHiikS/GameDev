using UnityEngine;

public class GorgonManager : MonoBehaviour
{
    // 1. Static Instance Field
    public static GorgonManager Instance { get; private set; } // Accessible by other scripts

    // Assign the Portal GameObject in the Inspector
    public GameObject exitPortal; 
    
    // Set this number to the total count of Gorgons (e.g., 3)
    public int totalGorgons = 3; 
    private int gorgonsDefeated = 0;

    void Awake()
    {
        // 2. Singleton Initialization
        if (Instance != null && Instance != this) 
        { 
            // Ensures only one manager exists
            Destroy(this.gameObject); 
        } 
        else 
        { 
            Instance = this; 
        } 
        if (exitPortal != null)
        {
            exitPortal.SetActive(false);
        }
    }

    void Start()
    {
        // Ensure the portal is closed at the start of the level
        if (exitPortal != null)
        {
            exitPortal.SetActive(false);
        }
    }

    // Public method called by a Gorgon's script when it dies
    public void ReportGorgonDefeated()
    {
        gorgonsDefeated++;
        Debug.Log($"[GorgonManager] Gorgon defeated. Total defeated: {gorgonsDefeated}");

        if (gorgonsDefeated >= totalGorgons)
        {
            OpenPortal();
        }
    }

    private void OpenPortal()
    {
        if (exitPortal != null)
        {
            Debug.Log("[GorgonManager] ALL GORGONS DEFEATED! Opening Exit Portal.");
            exitPortal.SetActive(true);
        }
    }
}