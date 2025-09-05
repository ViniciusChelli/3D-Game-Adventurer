using System.Collections.Generic;
using UnityEngine;

public class PlayerCustomizeBody : MonoBehaviour
{
    [System.Serializable]
    public class Body
    {
        public CategoryType category;
        public List<GameObject> parts;
    }

    public List<Body> bodyParts;
}
