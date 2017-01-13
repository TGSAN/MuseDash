using System;
using UnityEngine;

namespace Assets.Scripts.UI
{
    [Serializable]
    public class GradientKVP
    {
        public Gradient key;
        public Gradient value;

        public GradientKVP(Gradient k, Gradient v)
        {
            key = k;
            value = v;
        }
    }

    public class UIGradient : MonoBehaviour
    {
        public float radius;
        public Vector2 uv;
        public Gradient color;
        private Material m_Mtrl;
        private UIBasicSprite m_Sprite;

        private void Awake()
        {
            var sprite = GetComponent<UIBasicSprite>();
            var shader = Resources.Load("shaders/Gradient") as Shader;
            var material = new Material(shader);
            material.SetTexture("_MainTex", sprite.mainTexture);
            material.SetVector("_UVStartRamp", new Vector4(uv.x, uv.y, 0.0f, 0.0f));
            material.SetFloat("_Radius", radius);
            var length = Mathf.Min(color.colorKeys.Length, 4);
            for (int i = 0; i < length; i++)
            {
                material.SetColor("_Color" + i, color.colorKeys[i].color);
                material.SetFloat("_X" + i, color.colorKeys[i].time * radius);
            }
            sprite.material = material;
            m_Mtrl = material;
            m_Sprite = sprite;
        }

        private void Update()
        {
            m_Mtrl.SetVector("_UVStartRamp", new Vector4(uv.x, uv.y, 0.0f, 0.0f));
            m_Mtrl.SetFloat("_Radius", radius);
            var length = Mathf.Min(color.colorKeys.Length, 4);
            for (int i = 0; i < length; i++)
            {
                m_Mtrl.SetColor("_Color" + i, color.colorKeys[i].color);
                m_Mtrl.SetFloat("_X" + i, color.colorKeys[i].time * radius);
            }
            m_Sprite.enabled = false;
            m_Sprite.enabled = true;
        }
    }
}