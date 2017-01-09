using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EasyEditor
{
    /// <summary>
    /// Example class showing the use of the attribute [Message] to display a message under the ui element.
    /// Message comes with 3 flavours : MessageType.Info, MessageType.Warning, MessageType.Error.
    /// Message can be always displayed, or displayed based on some conditions.
    /// The parameter 'method' allows to specify a method returning bool. If this method return true,
    /// the message is displayed. If it returns false, the message is hidden.
    /// The parameters 'id' and 'value' allow to specify another field and indicate which value the field
    /// needs to be set to in order to show the message.
    /// </summary>
    public class ConditionalMessagesEnnemy : MonoBehaviour
    {
        [Image]
        public string easyEditorImage = "Assets/EasyEditor/Examples/icon.png";

        [Space(20f)]
        [Message(text = "This option is not optimized for mobiles.", messageType = MessageType.Info, id = "usePhysic", value = true)]
        public bool usePhysic = true;
        [Message(text = "Initial position of the ennemy.")]
        public Vector3 initialPosition;

        [Message(text = "The camera cannot be null.", messageType = MessageType.Error, method = "IsCameraNull")]
        public Camera ennemyCamera;

        [Inspector]
    	[Message(text = "The animation plays only in Play mode.", messageType = MessageType.Warning, method = "IsNotPlayMode")]
        public void PlayIntoFuryAnimation()
        {
            GetComponent<Animation>().Play();
        }

        private bool IsNotPlayMode()
        {
            return !Application.isPlaying;
        }

        private bool IsCameraNull()
        {
            return ennemyCamera == null;
        }
    }
}