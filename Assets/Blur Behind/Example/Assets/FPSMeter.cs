using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FPSMeter : MonoBehaviour {

    Text m_text;
    Text p_text
    {
        get
        {
            if (m_text == null)
            {
                m_text = GetComponent<Text>();
            }
            return m_text;
        }
    }

    const float updatePeriod = 0.5f;
    float m_cycleStartTime = 0f;
    int m_cycleStartFrame = 0;

	void Update () {
        float time = Time.unscaledTime;
        float elapsedTime = time - m_cycleStartTime;
        if (elapsedTime >= updatePeriod)
        {
            int frameIndex = Time.frameCount;
            p_text.text = Mathf.RoundToInt((frameIndex - m_cycleStartFrame) / elapsedTime).ToString() + " fps";
            m_cycleStartFrame = frameIndex;
            m_cycleStartTime = time;
        }
    }
}
