using Unity.Netcode;
using UnityEngine;

public class NetworkTestUI : MonoBehaviour
{
    private void OnGUI()
    {
        if (NetworkManager.Singleton == null) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            if (GUILayout.Button("Host (Server + Client)", GUILayout.Height(40)))
            {
                NetworkManager.Singleton.StartHost();
            }

            if (GUILayout.Button("Join as Client", GUILayout.Height(40)))
            {
                NetworkManager.Singleton.StartClient();
            }

            if (GUILayout.Button("Start Server Only", GUILayout.Height(40)))
            {
                NetworkManager.Singleton.StartServer();
            }
        }
        else
        {
            GUILayout.Label($"Mode: {(NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client")}");
        }

        GUILayout.EndArea();
    }
}