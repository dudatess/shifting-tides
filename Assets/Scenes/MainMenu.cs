using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public void HostGame()
    {
        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("[MainMenu] Host iniciado, carregando Lobby...");
            NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        }
        else
        {
            Debug.LogError("[MainMenu] Falha ao iniciar o Host");
        }
    }

    public void JoinGame()
    {
        if (NetworkManager.Singleton.StartClient())
        {
            Debug.Log("[MainMenu] Cliente tentando conectar...");
            // Cliente espera o Host carregar a cena.
        }
        else
        {
            Debug.LogError("[MainMenu] Falha ao iniciar o Client");
        }
    }
}
