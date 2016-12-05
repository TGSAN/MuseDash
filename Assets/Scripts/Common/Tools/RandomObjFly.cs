using DG.Tweening;
using Smart.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Common.Tools
{
    [RequireComponent(typeof(ParticleSystem))]
    public class RandomObjFly : MonoBehaviour
    {
        public Transform startPos, endPos;
        public GameObject go;
        public float maxY, maxZ;
        public float time;
        public AnimationCurve speedCurve;
        public int count = 20;
        private ParticleSystem m_ParticleSystem;
        private GameObject[] m_Gos;

        private void Awake()
        {
            m_ParticleSystem = GetComponent<ParticleSystem>();
        }

        public void FlyAll()
        {
            m_Gos = new GameObject[count];
            var pool = FastPoolManager.GetPool(go);
            for (int i = 0; i < m_Gos.Length; i++)
            {
                m_Gos[i] = pool.FastInstantiate();
                Fly(m_Gos[i]);
            }
        }

        private void Fly(GameObject g)
        {
            var distance = Vector3.Distance(startPos.position, endPos.position);
            var xDirection = Vector3.Normalize(endPos.position - startPos.position);
            var zDirection = Vector3.forward;
            var yDirection = Vector3.Cross(xDirection, zDirection);
            var x = m_ParticleSystem.velocityOverLifetime.x.Evaluate(Random.Range(0f, 1f), Random.Range(0f, 1f));
            var y = m_ParticleSystem.velocityOverLifetime.y.Evaluate(x, Random.Range(0f, 1f));
            var z = m_ParticleSystem.velocityOverLifetime.z.Evaluate(x, Random.Range(0f, 1f));
            var midPos = startPos.TransformPoint(x * distance * xDirection + y * maxY * yDirection + z * maxZ * zDirection);
            g.transform.localScale = Vector3.one;
            g.transform.position = startPos.position;
            var path = new[] { startPos.position, midPos, endPos.position };
            g.transform.DOPath(path, time, PathType.CatmullRom).SetEase(speedCurve).OnComplete(() =>
            {
                var pool = FastPoolManager.GetPool(go);
                pool.FastDestroy(g);
            });
        }
    }
}