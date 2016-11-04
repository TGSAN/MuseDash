using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class JsonUtil
{
}

public class DOTweenUtils
{
    public static Sequence Delay(Action callFunc, float dt)
    {
        var seq = DOTween.Sequence();
        seq.AppendInterval(dt);
        seq.AppendCallback(() =>
        {
            callFunc();
        });
        seq.Play();
        return seq;
    }

    public static Sequence Update(Action completeFunc, Func<bool> stopFunc)
    {
        var seq = DOTween.Sequence();
        seq.AppendInterval(float.MaxValue);
        seq.OnUpdate(() =>
        {
            if (stopFunc())
            {
                completeFunc();
                seq.Kill();
                return;
            }
        });
        seq.Play();
        return seq;
    }

    public static Tweener[] TweenAllAlphaTo(GameObject go, float alpha, float dt, float near)
    {
        var childTexs = go.GetComponentsInChildren<UIWidget>();
        return (from uiWidget in childTexs where !(Mathf.Abs(uiWidget.alpha - alpha) <= near) select DOTween.To(() => uiWidget.alpha, x => uiWidget.alpha = x, alpha, dt)).Cast<Tweener>().ToArray();
    }
}

public class ArrayUtils<T>
{
    public static bool Contains(T[] array, T value)
    {
        return new List<T>(array).Contains(value);
    }

    public static T[] Add(T[] array, T value)
    {
        return new List<T>(array) { value }.ToArray();
    }

    public static T[] Remove(T[] array, T value)
    {
        var list = new List<T>(array);
        list.Remove(value);
        return list.ToArray();
    }
}

public class StringUtils
{
    public static string LastAfter(string str, char split)
    {
        var strArray = str.Split(split);
        return strArray[strArray.Length - 1];
    }

    public static string BeginBefore(string str, char split)
    {
        return str.Split(split)[0];
    }
}

public class RandomUtils
{
    public static void RandomEvent(float[] probalities, Action[] events)
    {
    }
}