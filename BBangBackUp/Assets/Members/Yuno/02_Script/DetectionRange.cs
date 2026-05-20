using UnityEngine;

public class DetectionRange : MonoBehaviour
{
    // 부모의 Monster1 스크립트를 인스펙터에서 배정해야 함
    public Monster1 owner;

    private void Awake()
    {
        // 혹은 자동으로 부모에서 찾아옴
        if (owner == null) owner = GetComponentInParent<Monster1>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            owner.SetChasing(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            owner.SetChasing(false);
        }
    }
}