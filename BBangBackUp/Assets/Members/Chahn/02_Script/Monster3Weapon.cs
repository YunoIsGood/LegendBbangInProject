using System;
using UnityEngine;

public class Monster3Weapon : MonoBehaviour
{
    private Rigidbody2D _rb;
    private int damageStat = 3;

    
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
    }
    public void WeaponToss(Vector3 dir)
    {
        transform.SetParent(null);
        _rb.AddForce(new Vector3(dir.x, dir.y * 10, dir.z)); 
        Debug.Log("throw!");
           
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.instance.TakeDamage(damageStat);
            Debug.Log("3" + "데미지");
        }
    }
}
