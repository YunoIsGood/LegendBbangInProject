using UnityEngine;
using UnityEngine.UI;

public class Bed : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D collision)
    {
        GameManager.instance.Sleep();
    }

}
