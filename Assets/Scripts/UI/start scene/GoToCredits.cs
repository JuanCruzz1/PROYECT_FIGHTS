using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToCredits : MonoBehaviour
{
    public void OpenCreditsScene()
    {
        SceneManager.LoadScene("CreditsScene");
    }
}