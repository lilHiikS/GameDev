using UnityEngine;

public class GorgonManager : MonoBehaviour
{
    public static GorgonManager Instance { get; private set; } 

    public GameObject exitPortal; 
    
    public int totalGorgons = 3; 
    private int gorgonsDefeated = 0;
    public void Awake()
    {
        // 1. Core Singleton Check
        if (Instance != null && Instance != this) 
        { 
            // A duplicate exists. Destroy it immediately.
            Destroy(this.gameObject); 
        } 
        else 
        { 
            // This is the one and only manager.
            Instance = this; 
            
            // 2. Initial Setup: MUST run only on the official instance.
            gorgonsDefeated = 0; // Explicitly reset the count for the new scene
            
            if (exitPortal != null)
            {
                exitPortal.SetActive(false); // Close the portal
            }
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

    // New Function to reset the state every time the scene loads (or game starts)
    public void ResetLevelState()
    {
        gorgonsDefeated = 0;
        if (exitPortal != null)
        {
            exitPortal.SetActive(false);
        }
    }

    // Clean up the static reference when the application quits
    private void OnApplicationQuit()
    {
        Instance = null;
    }
}