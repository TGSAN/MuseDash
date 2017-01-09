using UnityEngine;
using System.Collections;
using EasyEditor;

namespace EasyEditor
{
    /// <summary>
    /// Sound manager demonstrates how it is easy to create a functional editor interface with EE.
    /// </summary>
    public class SoundManager : MonoBehaviour {

        [Image]
        public string easyEditorImage = "Assets/EasyEditor/Examples/icon.png";
        [Space(20f)]

    	[Comment("Ensure you input a new sound name before to click on Add Sound Handler")]
    	public string newSoundName = "";

    	[Inspector]
    	private void AddSoundHandler()
    	{
    		GameObject gameObject = new GameObject (newSoundName, typeof(SoundHandler));
    		gameObject.transform.position = this.transform.position;
    		gameObject.transform.parent = this.transform;
            gameObject.GetComponent<SoundHandler>().ID = newSoundName;
    	}
    }
}
