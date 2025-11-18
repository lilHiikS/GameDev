using UnityEngine;

public class TorchDropTarget : MonoBehaviour
{
    public TorchManager torchManager;

    void Start()
    {
        // Auto-assign the manager
        if (torchManager == null)
        {
            torchManager = FindObjectOfType<TorchManager>();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Torch torch = collision.GetComponent<Torch>(); 

        if (torch != null)
        {
            if (!torch.isLit)
            {
                collision.gameObject.SetActive(false); 
                
                torchManager.TorchActivated(torch); 
        }
    }
}
}


