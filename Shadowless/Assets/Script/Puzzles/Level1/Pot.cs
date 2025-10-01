using System.Collections.Generic;
using UnityEngine;

public class Pot : MonoBehaviour
{
    public PotPuzzle potPuzzle;
    private List<GameObject> items = new List<GameObject>();

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item") && potPuzzle.waitingForIngredient)
        {
            Debug.Log("Item submitted: " + collision.gameObject.name);
            items.Add(collision.gameObject);
            collision.gameObject.SetActive(false);
            potPuzzle.SubmitIngredient(collision.gameObject.name);
        }
    }

    public void SpitItems()
    {
        foreach (var item in items)
        {
            item.SetActive(true);
            var rb = item.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                float angle = Random.Range(30f, 150f) * Mathf.Deg2Rad;
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                rb.AddForce(direction * 5f, ForceMode2D.Impulse);
            }
        }
        items.Clear();
    }
}
