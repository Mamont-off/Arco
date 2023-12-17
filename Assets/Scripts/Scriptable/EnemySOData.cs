using System;
using Unity.Mathematics;
using UnityEngine;

namespace Scriptable
{
    [Serializable]
    public class EnemySOData : ScriptableObject
    {
        public GameObject Prefab;
        public int Hp;
        public float BaseMoveSpeed;
        public float IncreaseSpeedStep;
        [Min(1)]
        public int RowCount;

        public float MoveBorder;
        public float MaxHeight;
        [Min(1)]
        public float EnemyXOffset;
        [Min(1)]
        public float EnemyYOffset;
    }
}