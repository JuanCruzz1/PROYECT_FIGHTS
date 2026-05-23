using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneButton : MonoBehaviour
{
    // Función para cambiar de escena
    public void GoToSelectCharacter()
    {
        SceneManager.LoadScene("Select_Character");
    }
}
