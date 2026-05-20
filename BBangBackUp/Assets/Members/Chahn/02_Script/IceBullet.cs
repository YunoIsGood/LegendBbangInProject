using System.Collections;
using UnityEngine;

public class IceBullet : MonoBehaviour
{

    public float bulletSpeed = 10f;
    private Rigidbody2D _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

    }
    public void Shoot(Vector3 dir)
    {
        _rb.linearVelocity = new Vector2(dir.x, dir.y) * bulletSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //플레이어 못움직임
            Debug.Log("플레이어 얼음");
            StartCoroutine(CanMoveRoutine());
        }
    }
    public IEnumerator CanMoveRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        //다시 움직이는 코드 
    }

}
