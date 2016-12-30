using DG.Tweening;
using HutongGames.PlayMaker.Actions;
using Smart.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
        public Vector2 delayRange;
        private ParticleSystem m_ParticleSystem;
        private GameObject[] m_Gos;
        private UIGrid m_Grid;
        private Vector3 m_OrginStartPos, m_OriginEndPos;
        private List<Vector3> m_StartPoss = new List<Vector3>();

        private void Awake()
        {
            m_ParticleSystem = GetComponent<ParticleSystem>();
            m_Grid = startPos.GetComponent<UIGrid>();
            m_OrginStartPos = startPos.position;
            m_OriginEndPos = endPos.position;
        }

        public void SetPos(Vector3 start, Vector3 end)
        {
            m_StartPoss.Add(start);
            endPos.position = end == Vector3.zero ? m_OriginEndPos : end;
        }

        public GameObject[] SetAllItem(Texture[] texs)
        {
            count = texs.Length;
            m_Gos = new GameObject[count];
            var pool = FastPoolManager.GetPool(go);
            for (int i = 0; i < m_Gos.Length; i++)
            {
                var g = pool.FastInstantiate();
                g.transform.SetParent(m_Grid.transform, false);
                m_Grid.enabled = true;
                var uiTexture = g.GetComponent<UITexture>();
                uiTexture.mainTexture = texs[i];
                uiTexture.enabled = false;
                g.transform.localScale = Vector3.one * scale;
                m_Gos[i] = g;
                g.SetActive(false);
                uiTexture.enabled = true;
            }
            return m_Gos;
        }

        public void FlyAllItem(List<int> inVisibleIdx = null)
        {
            m_Grid.gameObject.SetActive(true);
            foreach (var g in m_Gos)
            {
                g.SetActive(true);
                startPos = g.transform;
                var isVisible = inVisibleIdx != null && !inVisibleIdx.Contains(m_Gos.IndexOf(g));
                Fly(g, Vector3.zero, isVisible);
            }
            m_Gos = null;
        }

        public void FlyAll()
        {
            m_Gos = new GameObject[count];
            var pool = FastPoolManager.GetPool(go);
            var sPos = m_OrginStartPos;
            if (m_StartPoss.Count > 0)
            {
                sPos = m_StartPoss[m_StartPoss.Count - 1];
                m_StartPoss.RemoveAt(m_StartPoss.Count - 1);
            }
            for (int i = 0; i < m_Gos.Length; i++)
            {
                var g = pool.FastInstantiate();
                m_Gos[i] = g;
                Fly(g, sPos);
            }
            m_Gos = null;
        }

        private void Fly(GameObject g, Vector3 sPos, bool isVisble = true, Callback finishFunc = null)
        {
            var start = startPos.InverseTransformDirection(sPos == Vector3.zero ? startPos.position : sPos);
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
            //g.SetActive(false);
            var path = new[] { start, midPos, end };
            var dt = Random.Range(delayRange.x, delayRange.y);
            DOTweenUtils.Delay(() =>
            {
                g.SetActive(isVisble);
            }, dt);
            g.transform.DOPath(path, time, PathType.CatmullRom).SetEase(speedCurve).OnComplete(() =>
            {
                g.transform.SetParent(null);
                var pool = FastPoolManager.GetPool(go);
                pool.FastDestroy(g);
                if (finishFunc != null) finishFunc();
            }).SetDelay(dt);

            var rotateValue = rotateX ? Vector3.right : (rotateY ? Vector3.up : Vector3.forward) * rotateSpeed * time;
            g.transform.DOLocalRotate(rotateValue, time, RotateMode.LocalAxisAdd).SetEase(rotateCurve);
        }
    }
}