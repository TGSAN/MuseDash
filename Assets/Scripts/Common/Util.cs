using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

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
                if (completeFunc != null)
                {
                    completeFunc();
                }
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

    public static T[] RandomSort(T[] array)
    {
        Random random = new Random();
        List<T> newList = new List<T>();
        foreach (T item in array)
        {
            newList.Insert(random.Next(newList.Count + 1), item);
        }
        return newList.ToArray();
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

    public static string FirstToLower(string str)
    {
        return str.Substring(0, 1).ToLower() + str.Substring(1);
    }

    public static string FirstToUpper(string str)
    {
        return str.Substring(0, 1).ToUpper() + str.Substring(1);
    }
}

public class RandomUtils
{
    public static void RandomEvent(float[] probabilities, Action[] events)
    {
        const int baseNum = 10000;
        var random = UnityEngine.Random.Range(0, baseNum);
        var startR = 0f;
        for (var i = 0; i < probabilities.Length; i++)
        {
            var probability = probabilities[i];
            var callFuncEvent = events[i];
            var radomNum = startR + probability * baseNum;
            if (random >= startR && random < radomNum)
            {
                if (events.Length <= i) continue;
                callFuncEvent();
                break;
            }
            else
            {
                startR = radomNum;
            }
        }
    }
}

public class GradientUtil
{
    public static Gradient BlendGradient(Gradient l, Gradient r, float percent)
    {
        var gradient = new Gradient();
        var list = new List<GradientColorKey>();
        for (int i = 0; i < l.colorKeys.Length; i++)
        {
            var c = Color.Lerp(l.colorKeys[i].color, r.colorKeys[i].color, percent);
            var t = l.colorKeys[i].time;
            list.Add(new GradientColorKey(c, t));
        }
        gradient.colorKeys = list.ToArray();
        return gradient;
    }
}