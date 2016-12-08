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
        public float scale = 1.0f;
        public float time;
        public AnimationCurve speedCurve;
        public int count = 10;
        public bool rotateX, rotateY, rotateZ;
        public float rotateSpeed = 400f;
        public AnimationCurve rotateCurve;
        private ParticleSystem m_ParticleSystem;
        private GameObject[] m_Gos;
        private UIGrid m_Grid;

        private void Awake()
        {
            m_ParticleSystem = GetComponent<ParticleSystem>();
            m_Grid = startPos.GetComponent<UIGrid>();
        }

        public void SetAllItem(Texture[] texs)
        {
            count = texs.Length;
            m_Gos = new GameObject[count];
            var pool = FastPoolManager.GetPool(go);
            for (int i = 0; i < m_Gos.Length; i++)
            {
                var g = pool.FastInstantiate();
                g.transform.SetParent(m_Grid.transform, false);
                m_Grid.enabled = true;
                g.GetComponent<UITexture>().mainTexture = texs[i];
                g.transform.localScale = Vector3.one * scale;
                m_Gos[i] = g;
                g.SetActive(false);
            }
        }

        public void FlyAllItem()
        {
            m_Grid.gameObject.SetActive(true);
            foreach (var g in m_Gos)
            {
                startPos = g.transform;
                g.SetActive(true);
                Fly(g);
            }
        }

        public void FlyAll()
        {
            m_Gos = new GameObject[count];
            var pool = FastPoolManager.GetPool(go);
            for (int i = 0; i < m_Gos.Length; i++)
            {
                var g = pool.FastInstantiate();
                Fly(g);
                m_Gos[i] = g;
            }
        }

        private void Fly(GameObject g)
        {
            var start = startPos.InverseTransformDirection(startPos.position);
            var end = endPos.InverseTransformDirection(endPos.position);
            var distance = Vector3.Distance(start, end);
            var xDirection = Vector3.Normalize(end - start);
            var zDirection = Vector3.forward;
            var yDirection = Vector3.Cross(xDirection, zDirection);
            var x = m_ParticleSystem.velocityOverLifetime.x.Evaluate(Random.Range(0f, 1f), Random.Range(0f, 1f));
            var y = m_ParticleSystem.velocityOverLifetime.y.Evaluate(x, Random.Range(0f, 1f));
            var z = m_ParticleSystem.velocityOverLifetime.z.Evaluate(x, Random.Range(0f, 1f));
            var midPos = startPos.TransformPoint(x * distance * xDirection + y * maxY * yDirection + z * maxZ * zDirection);
            g.transform.localScale = Vector3.one * scale;
            g.transform.position = start;
            var path = new[] { start, midPos, end };
            g.transform.DOPath(path, time, PathType.CatmullRom).SetEase(speedCurve).OnComplete(() =>
            {
                var pool = FastPoolManager.GetPool(go);
                pool.FastDestroy(g);
            });

            var rotateValue = rotateX ? Vector3.right : (rotateY ? Vector3.up : Vector3.forward) * rotateSpeed * time;
            g.transform.DOLocalRotate(rotateValue, time, RotateMode.LocalAxisAdd).SetEase(rotateCurve);
        }
    }
}