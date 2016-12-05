using UnityEngine;
using System.Collections;
using com.ootii.Messages;

namespace com.ootii.Demos.MD
{
    public class Sample : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            // Here an object registers a function to listen for the "STARTED" message.
            // When the "STARTED" message is sent (by any object), the OnStarted function
            // is called.
            MessageDispatcher.AddListener("STARTED", OnStarted, true);

            // This is a goofy example since we don't typically need to send a message
            // to ourselves. However, imagine we were calling this from a totally different
            // object that didn't know about this one.
            //
            // Here we send the "STARTED" Message.
            MessageDispatcher.SendMessage(this, "STARTED", "Whoo Hoo!", 0);
        }

        /// <summary>
        /// Raised by the MessageDispatcher when the "STARTED" message is recieved.
        /// </summary>
        void OnStarted(IMessage rMessage)
        {
            Debug.Log((string)rMessage.Data);
        }
    }
}
