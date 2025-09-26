using UnityEngine;
using UnityEngine.UI;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private Text healthText;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        UpdateText();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. HP: {health}");
        UpdateText();

        if (health <= 0)
        {
            Die();
        }
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
