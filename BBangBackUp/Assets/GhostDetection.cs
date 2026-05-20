using UnityEngine;

public class GhostDetection : MonoBehaviour
{
    public GhostScript ghostScript;

    void Start()
    {
        ghostScript = GetComponentInParent<GhostScript>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            ghostScript.isChasing = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            ghostScript.isChasing = false;
        }
    }
}