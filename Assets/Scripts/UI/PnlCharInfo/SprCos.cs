using System.Collections;
using UnityEngine;

public class SprCos : MonoBehaviour
{
    public UIToggle selectedTgl, inGroupTgl;
    public UISprite sprOff;

    public bool isSelected
    {
        get
        {
            return selectedTgl.value;
        }
        set
        {
            selectedTgl.value = value;
        }
    }

    public bool isInGroup
    {
        get
        {
            return inGroupTgl.value;
        }
        set
        {
            inGroupTgl.value = value;
        }
    }

    public bool isLock
    {
        get
        {
            return sprOff.spriteName.Contains("false");
        }
        set
        {
            if (value)
            {
                sprOff.spriteName = sprOff.spriteName.Replace("true", "false");
            }
            else
            {
                sprOff.spriteName = sprOff.spriteName.Replace("false", "true");
            }
            GetComponent<Collider>().enabled = !value;
        }
    }
}