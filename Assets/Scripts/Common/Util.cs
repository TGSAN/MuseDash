﻿using DG.Tweening;
using System;
using System.Collections;
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
}