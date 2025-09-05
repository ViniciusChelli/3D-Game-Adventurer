using UnityEngine;

public class LimitCameraMinimap : MonoBehaviour
{
    private void LateUpdate()
    {
        if (Player.Instance != null)
            transform.position = new Vector3(Player.Instance.transform.position.x, 55, Player.Instance.transform.position.z);
    }
}
