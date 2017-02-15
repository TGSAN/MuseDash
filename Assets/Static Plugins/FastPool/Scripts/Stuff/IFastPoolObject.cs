using UnityEngine;
using System.Collections;

public interface IFastPoolItem
{
    void OnFastInstantiate();
    void OnFastDestroy();
}
