using System.Collections;
using UnityEngine;

public class UICornerClip : MonoBehaviour
{
    private Material m_Mtrl;
    private UIBasicSprite m_Sprite;
    public float angle;
    public float percent;
    public Vector2 offset;

    // Use this for initialization
    private void Awake()
    {
        m_Sprite = GetComponent<UIBasicSprite>();
        var shader = Resources.Load("shaders/UICornerClip") as Shader;
        m_Mtrl = new Material(shader);
        m_Mtrl.SetTexture("_MainTex", m_Sprite.mainTexture);
        m_Sprite.material = m_Mtrl;
    }

    private void Update()
    {
        m_Mtrl.SetFloat("_Percent", percent);
        m_Mtrl.SetFloat("_TanValue", Mathf.Tan((90.0f - angle) * Mathf.Deg2Rad));
        m_Mtrl.SetVector("_Offset", new Vector4(offset.x, offset.y, 0.0f, 0.0f));
    }
}