using UnityEngine;

public class NPCPerception3D : MonoBehaviour
{
    public float visionRadius = 6f;
    public LayerMask interactablesMask;

    public IInteractable FindBestTarget(Vector3 origin, NPCInventory inv)
    {
        Collider[] hits = Physics.OverlapSphere(origin, visionRadius, interactablesMask);
        IInteractable best = null; float bestScore = float.NegativeInfinity;

        foreach (var h in hits)
        {
            var it = h.GetComponent<IInteractable>();
            if (it == null || !it.IsValid()) continue;

            float score = 0f;
            if (it is ItemPickup3D) score = 100f;
            else if (it is ResourceNode3D) score = 60f;
            else score = 30f;

            float d = Vector3.Distance(origin, it.GetWorldPosition());
            score -= d * 5f;

            if (score > bestScore) { bestScore = score; best = it; }
        }
        return best;
    }

    public StorageChest3D FindClosestStorage(Vector3 origin)
    {
        Collider[] hits = Physics.OverlapSphere(origin, visionRadius * 2f, interactablesMask);
        StorageChest3D best = null; float bestD = float.PositiveInfinity;

        foreach (var h in hits)
        {
            var sc = h.GetComponent<StorageChest3D>();
            if (sc == null || !sc.IsValid()) continue;
            float d = Vector3.Distance(origin, sc.GetWorldPosition());
            if (d < bestD) { bestD = d; best = sc; }
        }
        return best;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0,1,0,0.25f);
        Gizmos.DrawWireSphere(transform.position, visionRadius);
        Gizmos.color = new Color(0,0.5f,1,0.15f);
        Gizmos.DrawWireSphere(transform.position, visionRadius*2f);
    }
}
