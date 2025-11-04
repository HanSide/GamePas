using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UIElements;

public class HealingItem : MonoBehaviour
{
    [SerializeField] private int healAmount = 2;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();

            if (playerHealth != null)
            {
               
                if (playerHealth.IsFullHealth())
                {
                    Debug.Log("Player already at full health!");
                    return;
                }

                playerHealth.Heal(healAmount);

                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("Player doesn't have Health component!");
            }
        }
    }
}