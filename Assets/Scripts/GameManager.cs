using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int coinScore;
    public Transform initialSpawnPosition;

    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (Player.Instance != null)
        {
            Player.Instance.GetComponent<CharacterController>().enabled = false;
            Player.Instance.transform.position = initialSpawnPosition.position;
            Player.Instance.GetComponent<CharacterController>().enabled = true;

            Player.Instance.playerHUD.ActiveCanvas(true);
            Player.Instance.isPaused = false;
            CinemachineController.Instance.AssignPlayer();
        }
    }

    public void AddCoin()
    {
        coinScore++;
        Player.Instance.playerHUD.coinText.text = coinScore.ToString();
    }
}
