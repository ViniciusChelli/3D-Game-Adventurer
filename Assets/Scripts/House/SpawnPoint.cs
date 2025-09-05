using System.Collections;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public string spawnId;
    public bool assignCinemachineTarget;
    public FadeController fadePrefab;

    public Transform spawnPoint;

    private void Start()
    {        
        if(SpawnManager.spawnId == spawnId)
        {
            FadeController fade = Instantiate(fadePrefab);
            fade.fadeIn = true;
            Destroy(fade, 2f);
            Player.Instance.GetComponent<CharacterController>().enabled = false;
            SpawnManager.spawnPosition = spawnPoint.position;
            Player.Instance.transform.position = SpawnManager.spawnPosition;
            Player.Instance.GetComponent<CharacterController>().enabled = true;

            if (assignCinemachineTarget)
            {
                CinemachineController.Instance.AssignPlayer();
            }
        }       
    }
}

