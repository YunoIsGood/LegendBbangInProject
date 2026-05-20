using UnityEngine;

public class HealPack : MonoBehaviour
{
    public int minRange = 5;
    public int maxRange = 20;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            GameManager.instance.currentHealth += Random.Range(minRange,maxRange);
            Destroy(gameObject);
        }
    }
}
