using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneButton : MonoBehaviour
{
    [SerializeField] private float delayBeforeSceneChange = 0.40f;

    // Función para cambiar de escena
    public void GoToSelectCharacter()
    {
        StartCoroutine(ChangeSceneAfterDelay());
    }

    private IEnumerator ChangeSceneAfterDelay()
    {
        yield return new WaitForSecondsRealtime(delayBeforeSceneChange);

        SceneManager.LoadScene("CharacterSelectionScene");
    }
}