using UnityEngine;


public class FightManager : MonoBehaviour
{
    /*
    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;

    public GameObject enemyPrefab;

    private GameObject player;
    private GameObject enemy;

    IEnumerator StartCountdown()
    {
        SetCombatActive(false);

        countdownText.text = "3";
        yield return new WaitForSeconds(1f);

        countdownText.text = "2";
        yield return new WaitForSeconds(1f);

        countdownText.text = "1";
        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false);
        SetCombatActive(true);
    }

    public void Start()
    {
        SpawnCharacters();
        StartCoroutine(StartCountdown());
    }

    private void SpawnCharacters()
    {
        player = Instantiate(
            GameManager.Instance.SelectedCharacter.characterPrefab,
            playerSpawnPoint.position,
            Quaternion.identity
        );

        enemy = Instantiate(
            enemyPrefab,
            enemySpawnPoint.position,
            Quaternion.identity
        );
    }

    public void EndFight(bool playerWon)
    {
        if (playerWon)
        {
            ShowKO();
        }
        else
        {
            ShowYouLose();
        }

        DisableCombat();
        ShowResultMenu();
    }
    */
}
