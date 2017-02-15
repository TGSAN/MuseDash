using UnityEngine;
using UnityEngine.UI;

public class BlurBehindExampleUI : MonoBehaviour {
	
	public BlurBehind effect0;
	public BlurBehind effect1;

	public Slider layersSlider;
	public Text layersValue;

    public Slider radiusSlider;
    public Text radiusValue;

    public Button standardButton;
    public Button smoothButton;
    public Button manualButton;

    public CanvasGroup group;

    public Slider downsampleSlider;
    public Text downsampleValue;

    public Slider iterationsSlider;
	public Text iterationsValue;



	public void OnValueChanged() {
		int activeLayers = Mathf.RoundToInt(layersSlider.value);
		effect0.enabled = activeLayers > 0;
		effect1.enabled = activeLayers > 1;

        effect0.radius = Mathf.Pow(radiusSlider.value, 2f);
        effect1.radius = effect0.radius;

        if (downsampleSlider.value > 12)
        {
            effect0.downsample = float.PositiveInfinity;
        }
        else
        {
            effect0.downsample = Mathf.Pow(2, downsampleSlider.value);
        }
        effect1.downsample = effect0.downsample;

        effect0.iterations = Mathf.RoundToInt(iterationsSlider.value);
        effect1.iterations = effect0.iterations;
    }



    void Redraw()
    {
        if (effect1.enabled)
        {
            layersSlider.value = 2;
            layersValue.text = "2";
        }
        else if (effect0.enabled)
        {
            layersSlider.value = 1;
            layersValue.text = "1";
        }
        else
        {
            layersSlider.value = 0;
            layersValue.text = "0";
        }

        radiusSlider.value = Mathf.Sqrt(effect0.radius);
        radiusValue.text = Mathf.RoundToInt(effect0.radius).ToString();

        standardButton.interactable = effect0.settings != BlurBehind.Settings.Standard;
        smoothButton.interactable = effect0.settings != BlurBehind.Settings.Smooth;
        manualButton.interactable = effect0.settings != BlurBehind.Settings.Manual;

        if (effect0.settings == BlurBehind.Settings.Manual)
        {
            group.interactable = true;
            group.alpha = 1f;
        }
        else
        {
            group.interactable = false;
            group.alpha = 0.35f;
        }

        if (effect0.downsample > 4096)
        {
            downsampleSlider.value = 13;
            downsampleValue.text = "Inf";
        }
        else {
            downsampleSlider.value = Mathf.Log(effect0.downsample, 2);
            downsampleValue.text = Mathf.RoundToInt(effect0.downsample).ToString();
        }

        iterationsSlider.value = effect0.iterations;
        iterationsValue.text = effect0.iterations.ToString();
    }



    public void SetSettings(int state)
    {
        effect0.settings = (BlurBehind.Settings)state;
        effect1.settings = effect0.settings;
    }



    void Start()
    {
        Redraw();
    }



    void Update()
    {
        Redraw();
    }
}
