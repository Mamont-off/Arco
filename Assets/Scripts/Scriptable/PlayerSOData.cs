using UnityEngine;

namespace Scriptable
{
    public class PlayerSOData : ScriptableObject
    {
        public GameObject Prefab;
        public float Speed;
        public Vector3 SpawnPosition;
        [Tooltip("Absolute value")]
        public float MoveBorder;
    }
}