using Unity.Cinemachine;
using UnityEngine;

public class CinemachineController : MonoBehaviour
{
    private CinemachineCamera cam; 

    public static CinemachineController Instance;

    private void Awake()
    {
        Instance = this;
        cam = GetComponent<CinemachineCamera>();
    }

    public void AssignPlayer()
    {
        cam.Follow = Player.Instance.transform;
    }
}
