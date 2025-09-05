using System.Collections.Generic;
using UnityEngine;

public class CameraObstacleFader : MonoBehaviour
{
    public LayerMask obstacleLayer;
    public Material transparencyMat;
    public float checkInterval = 0.2f; // Intervalo em segundos entre verificações

    private Transform player;
    private float checkTimer;

    private Transform lastHitObject;
    private Dictionary<MeshRenderer, Material[]> originalMats = new();

    void Start()
    {
        player = Player.Instance.transform;
    }

    void Update()
    {
        checkTimer -= Time.deltaTime;
        if (checkTimer > 0f) return;

        checkTimer = checkInterval;

        Vector3 direction = player.position - transform.position;
        float sqrDistance = direction.sqrMagnitude;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction.normalized, out hit, Mathf.Sqrt(sqrDistance), obstacleLayer))
        {
            Transform currentHit = hit.transform.parent;

            if (currentHit != lastHitObject)
            {
                RestoreOriginalMaterials();

                originalMats.Clear();

                foreach (var renderer in currentHit.GetComponentsInChildren<MeshRenderer>())
                {
                    originalMats[renderer] = renderer.materials;

                    Material[] newMats = new Material[renderer.materials.Length];
                    for (int i = 0; i < newMats.Length; i++)
                        newMats[i] = transparencyMat;

                    renderer.materials = newMats;
                }

                lastHitObject = currentHit;
            }
        }
        else
        {
            RestoreOriginalMaterials();
        }
    }

    private void RestoreOriginalMaterials()
    {
        if (lastHitObject == null) return;

        foreach (var pair in originalMats)
        {
            if (pair.Key != null)
                pair.Key.materials = pair.Value;
        }

        originalMats.Clear();
        lastHitObject = null;
    }

    private void OnDrawGizmos()
    {
        if (player == null) return;

        Gizmos.color = Color.red;
        Vector3 direction = player.position - transform.position;
        Gizmos.DrawRay(transform.position, direction.normalized * direction.magnitude);
    }
}
