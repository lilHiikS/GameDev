using System;
using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    public float Health, MaxHealth, Width, Height;
    public Transform player;
    public Vector3 offset = new Vector3(0, 2f, 0);

    [SerializeField]
    private RectTransform healthBar;

    public void SetMaxHealth(float maxHealth)
    {
        MaxHealth = maxHealth;
    }

    public void SetHealth(float health)
    {
        Health = health;
        float newWidth = Health / MaxHealth * Width;
        healthBar.sizeDelta = new Vector2(newWidth, Height);
    }

    void LateUpdate()
    {
        if (player == null) return;
        transform.position = player.position + offset;
        transform.forward = Camera.main.transform.forward;
    }
}
