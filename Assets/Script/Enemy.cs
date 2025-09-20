using System;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int health;
    private Animator anim;
    [SerializeField] Text healthText;

    void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("aw");
        UpdateText();
    }
    void UpdateText()
    {
        if (healthText != null)
            healthText.text = health.ToString();
    }
}
