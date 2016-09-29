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

public class DOTweenUtil
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

    public static Tweener[] TweenAllAlphaTo(GameObject go, float alpha, float dt, float near)
    {
        var childTexs = go.GetComponentsInChildren<UIWidget>();
        return (from uiWidget in childTexs where !(Mathf.Abs(uiWidget.alpha - alpha) <= near) select DOTween.To(() => uiWidget.alpha, x => uiWidget.alpha = x, alpha, dt)).Cast<Tweener>().ToArray();
    }
}