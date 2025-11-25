using UnityEngine;

public class GorgonManager : MonoBehaviour
{
    public static GorgonManager Instance { get; private set; } 

    public GameObject exitPortal; 
    
    public int totalGorgons = 3; 
    private int gorgonsDefeated = 0;
    public void Awake()
    {
        Debug.Log($"[GorgonManager] Awake called on {gameObject.name}");
        
        if (Instance != null && Instance != this) 
        { 
            Debug.LogWarning($"[GorgonManager] Duplicate found! Destroying {gameObject.name}");
            Destroy(this.gameObject);
            return; 
        }
        
        Instance = this; 
        Debug.Log($"[GorgonManager] Instance set to {gameObject.name}");
        
        gorgonsDefeated = 0; 
        Debug.Log($"[GorgonManager] Reset gorgonsDefeated to 0. Total gorgons expected: {totalGorgons}");
        
        if (exitPortal != null)
        {
            exitPortal.SetActive(false); 
            Debug.Log("[GorgonManager] Exit portal deactivated");
        }
    }

    public void ReportGorgonDefeated()
    {
        if (Instance != this)
        {
            Debug.LogWarning("[GorgonManager] ReportGorgonDefeated called on inactive instance - ignoring");
            return;
        }
        
        gorgonsDefeated++;
        Debug.Log($"[GorgonManager] Gorgon defeated. Total defeated: {gorgonsDefeated}/{totalGorgons}");

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

    // Also clean up when this object is destroyed (scene changes, etc.)
    private void OnDestroy()
    {
        // Only clear the static reference if this is the active instance
        if (Instance == this)
        {
            Instance = null;
        }
    }
}