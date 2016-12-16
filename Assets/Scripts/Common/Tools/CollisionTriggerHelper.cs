using Smart.Types;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Common.Tools
{
    [RequireComponent(typeof(Collider))]
    public class CollisionTriggerHelper : MonoBehaviour
    {
        public UnityEventGameObject onCollisionEnter, onCollisionExit, onCollisionStay;
        public UnityEventGameObject onTriggerEnter, onTriggerExit, onTriggerStay;

        private void OnTriggerEnter(Collider col)
        {
            onTriggerEnter.Invoke(col.gameObject);
        }

        private void OnTriggerExit(Collider col)
        {
            onTriggerExit.Invoke(col.gameObject);
        }

        private void OnTriggerStay(Collider col)
        {
            onTriggerStay.Invoke(col.gameObject);
        }

        private void OnCollisionEnter(Collision col)
        {
            onCollisionEnter.Invoke(col.gameObject);
        }

        private void OnCollisionExit(Collision col)
        {
            onCollisionExit.Invoke(col.gameObject);
        }

        private void OnCollisionStay(Collision col)
        {
            onCollisionStay.Invoke(col.gameObject);
        }
    }
}