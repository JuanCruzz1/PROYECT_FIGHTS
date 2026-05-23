using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToStartScene : MonoBehaviour
{
    public void GoToStartScene()
    {
        SceneManager.LoadScene("Start_Scene");
    }
}