using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTrigger : MonoBehaviour
{
    public string targetSpawnId;
    public string houseSceneName;
    public FadeController fadePrefab;

    private bool isPlayerNearby;

    private void Update()
    {
        if(isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            LoadHouseScene();
            isPlayerNearby = false;
        }
    }

    //chamado uma vez ao encostar no colisor
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    //é chamado uma vez ao sair do colisor
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }

    void LoadHouseScene()
    {
        StartCoroutine(FadeScene());
    }

    IEnumerator FadeScene()
    {
        FadeController fade = Instantiate(fadePrefab);
        fade.fadeIn = false;
        Destroy(fade, 2f);
        yield return new WaitForSeconds(1f);
        SpawnManager.spawnId = targetSpawnId;
        SceneManager.LoadScene(houseSceneName);
    }

}
