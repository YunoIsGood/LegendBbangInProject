using UnityEngine;

public class Next : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        NextMap();
    }

    public void NextMap()
    {
        Debug.Log("Next");
    }
}
