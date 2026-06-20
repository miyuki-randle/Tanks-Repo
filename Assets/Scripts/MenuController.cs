using Unity.Netcode;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] private string LevelName = "Level";

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene(LevelName, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
        NetworkManager.Singleton.SceneManager.LoadScene(LevelName, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}