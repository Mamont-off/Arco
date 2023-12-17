using UnityEngine;

namespace Scriptable
{
    public class BulletSOData : ScriptableObject
    {
        public GameObject Prefab;
        public int Damage;
        public float BulletSpeed;
        public float ShootSpeed;
        public int DestroyDistance;
    }
}