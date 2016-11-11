using DYUnityLib;
using FormulaBase;
using GameLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Resource loader.
/// 统一资源加载
/// </summary>
public class ResourceLoader : MonoBehaviour
{
    public delegate void ResourceLoaderHandler(UnityEngine.Object res);

    public const string RES_FROM_WWW = "www";
    public const string RES_FROM_LOCAL = "local";
    public const string RES_FROM_RESOURCE = "res";

    private static ResourceLoader instance = null;

    public static ResourceLoader Instance
    {
        get
        {
            return instance;
        }
    }

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
    }

    /*
	public T Load<T>(string path) {
		return default(T);
	}
	*/

    public Coroutine LoadItemIcon(FormulaHost host, UITexture tex)
    {
        var icon = "items/icon/" + host.GetDynamicStrByKey(SignKeys.ICON);
        return ResourceLoader.Instance.Load(icon, res => tex.mainTexture = res as Texture);
    }

    public Coroutine Load(FormulaHost host, UITexture tex)
    {
        var icon = host.GetDynamicStrByKey(SignKeys.ICON);
        return ResourceLoader.Instance.Load(icon, res => tex.mainTexture = res as Texture);
    }

    public Coroutine Load(string path, ResourceLoaderHandler handler, string resFrom = RES_FROM_RESOURCE)
    {
        if (resFrom == RES_FROM_RESOURCE)
        {
            CommonPanel.GetInstance().DebugInfo(path);
            UnityEngine.Object resObj = Resources.Load<UnityEngine.Object>(path);
            CommonPanel.GetInstance().DebugInfo(resObj.ToString());
            if (handler != null)
            {
                handler(resObj);
            }

            return null;
        }

        return this.StartCoroutine(this.__Load(path, handler, resFrom));
    }

    private IEnumerator __Load(string path, ResourceLoaderHandler handler, string resFrom)
    {
        UnityEngine.Object resObj = null;
        if (resFrom == RES_FROM_LOCAL)
        {
            ResourceRequest loadRequest = Resources.LoadAsync<UnityEngine.Object>(path);
            while (!loadRequest.isDone)
            {
                yield return null;
            }

            resObj = loadRequest.asset as UnityEngine.Object;
        }
        else if (resFrom == RES_FROM_WWW)
        {
            WWW streamRes = new WWW(AssetBundleFileMangager.STREAMINGASSETS + path);
            yield return streamRes;
            resObj = streamRes.assetBundle.LoadAsset<UnityEngine.Object>(path);
        }

        if (resObj.GetType() == typeof(AudioClip))
        {
            AudioClip ac = resObj as AudioClip;
            if (ac.loadState != AudioDataLoadState.Loaded)
            {
                yield return null;
            }

            Debug.Log("==== : Load audioclip " + path + " ok.");
        }

        if (handler != null)
        {
            handler(resObj);
        }

        yield return 1;
    }
}