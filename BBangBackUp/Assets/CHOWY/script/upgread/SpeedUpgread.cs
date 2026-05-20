using UnityEngine;
using UnityEngine.InputSystem;

public class SpeedUpgread : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    public float speedUpgread=2f;
    public float speedUpMax = 15f;
    


    private void Start()
    {
        playerManager = GetComponent<PlayerManager>(); 
    }
    private void Update()
    {
        if(Keyboard.current.uKey.wasPressedThisFrame)
        {
            float newSpeed = playerManager.moveSpeed + speedUpgread;
            playerManager.moveSpeed = Mathf.Min(newSpeed, speedUpMax);

            Debug.Log(playerManager.moveSpeed);
        }
    }
}
