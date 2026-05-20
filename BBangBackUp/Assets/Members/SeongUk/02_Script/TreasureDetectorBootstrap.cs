using UnityEngine;
using UnityEngine.SceneManagement;

public class TreasureDetectorBootstrap : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void StartBootstrap()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        InstallDetector();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InstallDetector();
    }

    private static void InstallDetector()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            player = GameObject.Find("Player");
        }

        if (player == null)
        {
            return;
        }

        if (player.GetComponent<TreasureDetector>() == null)
        {
            player.AddComponent<TreasureDetector>();
        }
    }
}