using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomizeManager : MonoBehaviour
{
    private void Start()
    {
        Player.Instance.playerHUD.ActiveCanvas(false);
    }

    public void Confirm()
    {
        SceneManager.LoadScene("Gameplay");
    }
}

public enum CategoryType
{
    head,
    hair,
    body,
    hat,
    eye,
    mustache,
    mouth,
    accessory,
    gender
}
