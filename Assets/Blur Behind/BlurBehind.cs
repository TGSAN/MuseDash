using UnityEngine;

[AddComponentMenu("Image Effects/Blur Behind")]
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class BlurBehind : MonoBehaviour
{
    // STATIC
    static RenderTexture storedTexture = null;
    static int count = 0;
    static Rect storedRect = new Rect(0, 0, 1, 1);

    public static void SetViewport()
    {
        Rect cameraRect = Camera.current.rect;
        if (cameraRect != new Rect(0f, 0f, 1f, 1f))
        {
            Vector2 cameraTargetSize;
            if (Camera.current.targetTexture == null)
            {
                cameraTargetSize = new Vector2(Screen.width, Screen.height);
            }
            else
            {
                cameraTargetSize = new Vector2(Camera.current.targetTexture.width, Camera.current.targetTexture.height);
            }
            cameraRect.width = Mathf.Round(Mathf.Clamp01(cameraRect.width + cameraRect.x) * cameraTargetSize.x) / cameraTargetSize.x;
            cameraRect.height = Mathf.Round(Mathf.Clamp01(cameraRect.height + cameraRect.y) * cameraTargetSize.y) / cameraTargetSize.y;
            cameraRect.x = Mathf.Round(Mathf.Clamp01(cameraRect.x) * cameraTargetSize.x) / cameraTargetSize.x;
            cameraRect.y = Mathf.Round(Mathf.Clamp01(cameraRect.y) * cameraTargetSize.y) / cameraTargetSize.y;
            cameraRect.width -= cameraRect.x;
            cameraRect.height -= cameraRect.y;

            Shader.SetGlobalVector("_BlurBehindRect", new Vector4((storedRect.x - cameraRect.x) / cameraRect.width, storedRect.y / cameraRect.height + cameraRect.y, storedRect.width / cameraRect.width, storedRect.height / cameraRect.height));
        }
    }

    public static void ResetViewport()
    {
        Shader.SetGlobalVector("_BlurBehindRect", new Vector4(storedRect.x, storedRect.y, storedRect.width, storedRect.height));
    }



    // INSTANCE
    public Shader blurShader = null;
    Material blurMaterial = null;

    public enum Mode { Absolute = 0, Relative = 1 };
    public Mode mode = Mode.Relative;

    public float radius = 2.5f;

    public enum Settings { Standard = 0, Smooth = 1, Manual = 2 };
    public Settings settings = Settings.Standard;

    public float downsample = 1;

    public int iterations = 1;

    public Rect cropRect = new Rect(0f, 0f, 1f, 1f);
    public Rect pixelOffset = new Rect(0f, 0f, 0f, 0f);



    void CheckSettings(int sourceSize)
    {
        if (radius < 0f)
        {
            radius = 0f;
        }
        if (downsample < 1f)
        {
            downsample = 1f;
        }
        if (iterations < 0)
        {
            iterations = 0;
        }

        if (settings != Settings.Manual)
        {
            float iterationRate = settings == Settings.Standard ? 36f : 6f;
            if (mode == Mode.Absolute)
            {
                if (radius > 0f)
                {
                    if (radius < iterationRate)
                    {
                        iterations = 1;
                    }
                    else
                    {
                        iterations = Mathf.FloorToInt(Mathf.Log(radius, iterationRate)) + 1;
                    }
                    downsample = (radius / Mathf.Pow(3f, iterations));
                    if (downsample < 1f)
                    {
                        downsample = 1f;
                    }
                }
                else
                {
                    downsample = 1f;
                    iterations = 0;
                }
            }
            else
            {
                if (radius > 0f)
                {
                    float pixelRadius = radius / 100f * sourceSize;
                    if (pixelRadius < iterationRate)
                    {
                        iterations = 1;
                    }
                    else
                    {
                        iterations = Mathf.FloorToInt(Mathf.Log(pixelRadius, iterationRate)) + 1;
                    }
                    downsample = sourceSize / (pixelRadius / Mathf.Pow(3f, iterations));
                }
                else
                {
                    downsample = float.PositiveInfinity;
                    iterations = 0;
                }
            }
        }
    }

    void CheckOutput(int rtW, int rtH, RenderTextureFormat format)
    {
        if (storedTexture == null)
        {
            CreateOutput(rtW, rtH, format);
        }
        else if (storedTexture.width != rtW || storedTexture.height != rtH || storedTexture.format != format)
        {
            storedTexture.Release();
            DestroyImmediate(storedTexture);
            CreateOutput(rtW, rtH, format);
        }
        else
        {
            storedTexture.DiscardContents();
        }
    }

    bool CheckResources()
    {
        if (blurMaterial == null)
        {
            if (blurShader != null)
            {
                if (blurShader.isSupported)
                {
                    blurMaterial = new Material(blurShader);
                    blurMaterial.hideFlags = HideFlags.DontSave;
                }
                else
                {
                    Debug.Log("Blur Behind UI: Shader not supported");
                    return false;
                }
            }
            else
            {
                Debug.Log("Blur Behind UI: Shader reference missing");
                return false;
            }
        }
        return true;
    }

    bool CheckSupport()
    {
        if (!SystemInfo.supportsImageEffects)
        {
            Debug.Log("Blur Behind UI: Image effects not supported");
            return false;
        }
        if (!SystemInfo.supportsRenderTextures)
        {
            Debug.Log("Blur Behind UI: Render textures not supported");
            return false;
        }
        return true;
    }

    void CreateOutput(int width, int height, RenderTextureFormat format)
    {
        storedTexture = new RenderTexture(width, height, 0, format);
        storedTexture.filterMode = FilterMode.Bilinear;
        storedTexture.hideFlags = HideFlags.DontSave;
        Shader.SetGlobalTexture("_BlurBehindTex", storedTexture);
        Shader.EnableKeyword("BLUR_BEHIND_SET");
    }

    RenderTexture CropSource(RenderTexture source)
    {
        Rect effectRect = new Rect(cropRect.x * source.width + pixelOffset.x, cropRect.y * source.height + pixelOffset.y, cropRect.width * source.width + pixelOffset.width, cropRect.height * source.height + pixelOffset.height);
        effectRect.width = Mathf.Clamp01(Mathf.Round(effectRect.width + effectRect.x) / source.width);
        effectRect.height = Mathf.Clamp01(Mathf.Round(effectRect.height + effectRect.y) / source.height);
        effectRect.x = Mathf.Clamp01(Mathf.Round(effectRect.x) / source.width);
        effectRect.y = Mathf.Clamp01(Mathf.Round(effectRect.y) / source.height);
        effectRect.width -= effectRect.x;
        effectRect.height -= effectRect.y;

        RenderTexture croppedSource;

        if (effectRect != new Rect(0f, 0f, 1f, 1f))
        {
            croppedSource = RenderTexture.GetTemporary(Mathf.RoundToInt(effectRect.width * source.width), Mathf.RoundToInt(effectRect.height * source.height), 0, source.format);
            blurMaterial.SetVector("_Parameter", new Vector4(effectRect.x, effectRect.y, effectRect.width, effectRect.height));
            Graphics.Blit(source, croppedSource, blurMaterial, 2);
            storedRect = effectRect;
        }
        else
        {
            croppedSource = source;
            storedRect = new Rect(0f, 0f, 1f, 1f);
        }

        Rect cameraRect = Camera.current.rect;
        if (cameraRect != new Rect(0f, 0f, 1f, 1f))
        {
            Vector2 cameraTargetSize;
            if (Camera.current.targetTexture == null)
            {
                cameraTargetSize = new Vector2(Screen.width, Screen.height);
            }
            else
            {
                cameraTargetSize = new Vector2(Camera.current.targetTexture.width, Camera.current.targetTexture.height);
            }
            cameraRect.width = Mathf.Round(Mathf.Clamp01(cameraRect.width + cameraRect.x) * cameraTargetSize.x) / cameraTargetSize.x;
            cameraRect.height = Mathf.Round(Mathf.Clamp01(cameraRect.height + cameraRect.y) * cameraTargetSize.y) / cameraTargetSize.y;
            cameraRect.x = Mathf.Round(Mathf.Clamp01(cameraRect.x) * cameraTargetSize.x) / cameraTargetSize.x;
            cameraRect.y = Mathf.Round(Mathf.Clamp01(cameraRect.y) * cameraTargetSize.y) / cameraTargetSize.y;
            cameraRect.width -= cameraRect.x;
            cameraRect.height -= cameraRect.y;

            storedRect = new Rect(cameraRect.x + storedRect.x * cameraRect.width, cameraRect.y + storedRect.y * cameraRect.height, cameraRect.width * storedRect.width, cameraRect.height * storedRect.height);
        }

        Shader.SetGlobalVector("_BlurBehindRect", new Vector4(storedRect.x, storedRect.y, storedRect.width, storedRect.height));

        return croppedSource;
    }

    void Downsample(RenderTexture source, RenderTexture dest)
    {
        int downsampleLeft = 0;
        if (source.width > source.height)
        {
            int testLength = source.width;
            while (testLength > dest.width)
            {
                downsampleLeft++;
                testLength = testLength >> 1;
            }
        }
        else
        {
            int testLength = source.height;
            while (testLength > dest.height)
            {
                downsampleLeft++;
                testLength = testLength >> 1;
            }
        }

        if (downsampleLeft > 1)
        {
            RenderTexture downsampleRT = source;
            while (downsampleLeft > 2)
            {
                int rtWidth = downsampleRT.width >> 2;
                if (rtWidth < 1) { rtWidth = 1; }
                int rtHeight = downsampleRT.height >> 2;
                if (rtHeight < 1) { rtHeight = 1; }
                downsampleRT.filterMode = FilterMode.Bilinear;
                RenderTexture tempRT = RenderTexture.GetTemporary(rtWidth, rtHeight, 0, downsampleRT.format);
                blurMaterial.SetVector("_Parameter", new Vector4(downsampleRT.texelSize.x, downsampleRT.texelSize.y, -downsampleRT.texelSize.x, -downsampleRT.texelSize.y));
                Graphics.Blit(downsampleRT, tempRT, blurMaterial, 1);
                if (downsampleRT != source)
                {
                    RenderTexture.ReleaseTemporary(downsampleRT);
                }
                downsampleRT = tempRT;
                downsampleLeft -= 2;
            }
            if (downsampleLeft > 1)
            {
                blurMaterial.SetVector("_Parameter", new Vector4(downsampleRT.texelSize.x, downsampleRT.texelSize.y, -downsampleRT.texelSize.x, -downsampleRT.texelSize.y));
                Graphics.Blit(downsampleRT, dest, blurMaterial, 1);
            }
            else
            {
                Graphics.Blit(downsampleRT, dest);
            }
            if (downsampleRT != source)
            {
                RenderTexture.ReleaseTemporary(downsampleRT);
            }
        }
        else
        {
            Graphics.Blit(source, dest);
        }
    }

    void OnDisable()
    {
        if (blurMaterial)
        {
#if UNITY_EDITOR
            DestroyImmediate(blurMaterial);
#else
            Destroy(blurMaterial);
#endif
            blurMaterial = null;
        }
        count--;
        if (count == 0)
        {
            if (storedTexture)
            {
                storedTexture.Release();
#if UNITY_EDITOR
                DestroyImmediate(storedTexture);
#else
                Destroy(storedTexture);
#endif
                storedTexture = null;
                Shader.SetGlobalTexture("_BlurBehindTex", null);
                Shader.DisableKeyword("BLUR_BEHIND_SET");
            }
        }
    }

    void OnEnable()
    {
        count++;
    }

    void OnPreRender()
    {
        SetViewport();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!CheckSupport() || !CheckResources())
        {
            enabled = false;
            Graphics.Blit(source, destination);
            return;
        }

        RenderTexture croppedSource = CropSource(source);

        int sourceSize = croppedSource.width > croppedSource.height ? croppedSource.width : croppedSource.height;

        CheckSettings(sourceSize);

        int outputWidth;
        int outputHeight;
        SetOutputSize(source, croppedSource, out outputWidth, out outputHeight);

        CheckOutput(outputWidth, outputHeight, croppedSource.format);

        Downsample(croppedSource, storedTexture);

        if (croppedSource != source)
        {
            RenderTexture.ReleaseTemporary(croppedSource);
        }

        // BLUR
        if (iterations > 0 && radius > 0f)
        {
            RenderTexture blurRT = RenderTexture.GetTemporary(outputWidth, outputHeight, 0, croppedSource.format);
            blurRT.filterMode = FilterMode.Bilinear;

            for (int i = 0; i < iterations; i++)
            {
                float iterationRadius = radius / 300f * Mathf.Pow(3f, i) / Mathf.Pow(3f, iterations - 1);
                if (mode == Mode.Absolute)
                {
                    iterationRadius *= 100f / sourceSize;
                }
                else if (croppedSource != source)
                {
                    iterationRadius *= (float)(source.width > source.height ? source.width : source.height) / sourceSize;
                }
                float angle = i * 0.7853982f / iterations; // Iteration rotation
                Vector2 vector = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * iterationRadius;
                Vector2 viewMultiplier = outputWidth > outputHeight ?
                                         new Vector2(1f, 1f / outputHeight * outputWidth) :
                                         new Vector2(1f / outputWidth * outputHeight, 1f);

                // VERTICAL
                Vector4 parameter = new Vector4(vector.x * viewMultiplier.x, vector.y * viewMultiplier.y, 0f, 0f);
                parameter.z = -parameter.x;
                parameter.w = -parameter.y;
                blurMaterial.SetVector("_Parameter", parameter);
                Graphics.Blit(storedTexture, blurRT, blurMaterial, 0);
                storedTexture.DiscardContents();

                // HORIZONTAL
                parameter = new Vector4(vector.y * viewMultiplier.x, -vector.x * viewMultiplier.y, 0f, 0f);
                parameter.z = -parameter.x;
                parameter.w = -parameter.y;
                blurMaterial.SetVector("_Parameter", parameter);
                Graphics.Blit(blurRT, storedTexture, blurMaterial, 0);
                blurRT.DiscardContents();
            }

            RenderTexture.ReleaseTemporary(blurRT);
        }

        Graphics.Blit(source, destination);
    }

    void SetOutputSize(RenderTexture source, RenderTexture croppedSource, out int width, out int height)
    {
        float pixelSize;
        if (mode == Mode.Absolute)
        {
            pixelSize = 1f / downsample;
        }
        else
        {
            if (source.width > source.height)
            {
                if (source.width > downsample)
                {
                    pixelSize = downsample / source.width;
                }
                else
                {
                    pixelSize = 1f;
                }
            }
            else
            {
                if (source.height > downsample)
                {
                    pixelSize = downsample / source.height;
                }
                else
                {
                    pixelSize = 1f;
                }
            }
        }
        width = Mathf.RoundToInt(croppedSource.width * pixelSize);
        height = Mathf.RoundToInt(croppedSource.height * pixelSize);
        if (width < 1)
        {
            width = 1;
        }
        else if (width > croppedSource.width)
        {
            width = croppedSource.width;
        }
        if (height < 1)
        {
            height = 1;
        }
        else if (height > croppedSource.height)
        {
            height = croppedSource.height;
        }
    }
}