using UnityEngine;

public class WeaponSpawn : MonoBehaviour
{
    
    [SerializeField]private Monster3Weapon _monsterWeapon;
    [SerializeField]private GameObject prefabWeapon;
    [SerializeField]private Monster3 _monster3;
    public void SpawnWeapon(Vector3 value)
    {
        GameObject mW = Instantiate(prefabWeapon, transform);
        _monsterWeapon = mW.GetComponent<Monster3Weapon>();
        _monster3._isAttacking = true;
        //던지는 애니메이션
        _monsterWeapon.WeaponToss(value);
        StartCoroutine(_monster3.CoolTimeRountine());
    }
}


