using UnityEngine;
using System.Collections;
using com.ootii.Messages;

namespace com.ootii.Demos.MD
{
    public class ObjectEx : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            // Everyone is listenging for this message
            MessageDispatcher.AddListener("EVERYONE", OnEveryoneMessageReceived);

            // Same message handler function used, but filter is used to limit who gets it
            if (gameObject.tag == "Sphere") { MessageDispatcher.AddListener("FILTER", "Sphere", OnFilterMessageReceived); }
            if (gameObject.tag == "Cube") { MessageDispatcher.AddListener("FILTER", "Cube", OnFilterMessageReceived); }
            //if (gameObject.tag == "Cylinder") { MessageDispatcher.AddListener("FILTER", "Cylinder", OnFilterMessageReceived); }

            // Objects listening for messages sent to them
            if (gameObject.name == "Sphere_1") { MessageDispatcher.AddListener(gameObject, "TARGET", OnTargetedMessageReceived); }
            if (gameObject.name == "Cube_1") { MessageDispatcher.AddListener(gameObject, "TARGET", OnTargetedMessageReceived); }
            if (gameObject.name == "Cylinder_1") { MessageDispatcher.AddListener(gameObject, "TARGET", OnTargetedMessageReceived); }


        }

        // Update is called once per frame
        void Update()
        {
        }

        /// <summary>
        /// Raised when the object is clicked on
        /// </summary>
        void OnMouseDown()
        {
            if (gameObject.name == "Plane_1")
            {
                // Send the message to everyone listening
                MessageDispatcher.SendMessage("EVERYONE");
            }
            else if (gameObject.name == "Capsule_1")
            {
                // Send the message to a specific sphere (by name)
                MessageDispatcher.SendMessage(gameObject, "Sphere_1", "TARGET", null, EnumMessageDelay.IMMEDIATE);
            }
            else if (gameObject.name == "Capsule_2")
            {
                // Send the message to a specific cube (by name)
                MessageDispatcher.SendMessage(gameObject, "Cube_1", "TARGET", null, EnumMessageDelay.IMMEDIATE);
            }
            else if (gameObject.name == "Sphere_1")
            {
                // Send the message, but filter it
                MessageDispatcher.SendMessage("FILTER", "Sphere");
            }
            else if (gameObject.name == "Cube_1")
            {
                // Send the message, but filter it
                MessageDispatcher.SendMessage("FILTER", "Cube");
            }
            else if (gameObject.name == "Cylinder_1")
            {
                MessageDispatcher.RemoveListener("EVERYONE", OnEveryoneMessageReceived);
                MessageDispatcher.RemoveListener(gameObject, "TARGET", OnTargetedMessageReceived);
                //MessageDispatcher.ClearListeners();
                GameObject.Destroy(gameObject);
            }
        }

        /// <summary>
        /// Raised when the object gets a message that was sent to everyone
        /// </summary>
        /// <param name='rMessage'>Message that was sent </param>
        public void OnEveryoneMessageReceived(IMessage rMessage)
        {
            // See if the message passes in color data
            Color lColor = (Color)(rMessage.Data is Color ? rMessage.Data : Color.white);

            // Set the color on the object listening
            gameObject.GetComponent<Renderer>().material.color = lColor;

            // While not required, this is a good way to be tidy
            // and let others know that the message has been handled
            rMessage.IsHandled = true;
        }

        /// <summary>
        /// Raised when the object gets a message that was sent to everyone
        /// </summary>
        /// <param name='rMessage'>Message that was sent </param>
        public void OnFilterMessageReceived(IMessage rMessage)
        {
            // See if the message passes in color data
            Color lColor = (Color)(rMessage.Data is Color ? rMessage.Data : Color.blue);
            gameObject.GetComponent<Renderer>().material.color = lColor;

            // While not required, this is a good way to be tidy
            // and let others know that the message has been handled
            rMessage.IsHandled = true;
        }

        /// <summary>
        /// Raised when the object gets a message that was sent to everyone
        /// </summary>
        /// <param name='rMessage'>Message that was sent </param>
        public void OnTargetedMessageReceived(IMessage rMessage)
        {
            // See if the message passes in color data
            Color lColor = (Color)(rMessage.Data is Color ? rMessage.Data : Color.red);
            gameObject.GetComponent<Renderer>().material.color = lColor;

            // While not required, this is a good way to be tidy
            // and let others know that the message has been handled
            rMessage.IsHandled = true;
        }
    }
}
