using UnityEngine;
using UnityEngine.UI;
using System;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] public int maxHealth;
    [SerializeField] public int health;
    [SerializeField] private Text healthText;
    private Animator anim;
    public event Action OnHPChanged;

    void Start()
    {
        anim = GetComponent<Animator>();
        UpdateText();
        maxHealth = health;
    }

    public void TakeDamage(int damage)
    {
        DecreaseHP(damage);
        if (health <= 0)
        {
            Die();
        }
    }
    public void IncreaseHP(int amount)
    {
        health = Mathf.Clamp(health + amount, 0, maxHealth);
        Debug.Log($"{gameObject.name} healed {amount}. HP: {health}");
        UpdateText();
        OnHPChanged?.Invoke();
    }

    public void DecreaseHP(int damage)
    {
        health = Mathf.Clamp(health - damage, 0, maxHealth);
        Debug.Log($"{gameObject.name} took {damage} damage. HP: {health}");
        UpdateText();
        OnHPChanged?.Invoke();
    }
    void UpdateText()
    {
        if (healthText != null)
            healthText.text = health.ToString();
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} Died");
        if (anim != null)
            anim.SetTrigger("Die");
        gameObject.SetActive(false);
    }
}
