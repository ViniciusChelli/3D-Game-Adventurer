using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [System.Serializable]
    public enum ItemEffect
    {
        health,
        speed,
        damage
    }

    public string itemName;
    public string desc;
    public Sprite icon;
    public GameObject prefab;
    public float amount;

    public ItemEffect effect;

    public void Apply()
    {
        switch (effect)
        {
            case ItemEffect.health:
                Player.Instance.playerHealth.RecoveryHP(Mathf.RoundToInt(amount));
                GameObject effect = Instantiate(prefab, Player.Instance.transform.position, Quaternion.identity);
                Destroy(effect, 3f);
                break;

            case ItemEffect.speed:

                break;

            case ItemEffect.damage:

                break;
        }

        Debug.Log("Ativou o efeito: " + effect.ToString() + " em: " + amount);
    }
}
