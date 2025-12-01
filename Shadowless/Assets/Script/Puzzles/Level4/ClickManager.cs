using UnityEngine;

public class TorchClickHandler : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                Torch torch = hit.collider.GetComponent<Torch>();
                if (torch != null)
                {
                    Debug.Log("Clicked torch " + torch.torchID);
                    torch.LightTorch();
                }
            }
        }
    }
}
