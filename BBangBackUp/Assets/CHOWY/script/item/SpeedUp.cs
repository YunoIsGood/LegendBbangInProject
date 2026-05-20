using UnityEngine;
using UnityEngine.InputSystem;

public class SpeedUp : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    public float speedUp = 10f;
    private void Update()
    {
        if(Keyboard.current.digit3Key.wasPressedThisFrame)
        {

        }
    }
}
