using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("LEVEL LOADER STARTED");
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            Debug.Log("L WAS PRESSED");

            if (NetworkManager.Singleton == null)
            {
                Debug.LogError("No NetworkManager found.");
                return;
            }

            if (!NetworkManager.Singleton.IsServer)
            {
                Debug.LogWarning("Only the host can load the level.");
                return;
            }

            Debug.Log("LOADING LEVEL 1");

            NetworkManager.Singleton.SceneManager.LoadScene(
                "Level1",
                LoadSceneMode.Single
            );
        }
    }
}