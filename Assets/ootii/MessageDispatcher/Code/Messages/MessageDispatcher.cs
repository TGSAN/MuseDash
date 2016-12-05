using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ootii.Messages
{
    /// <summary>
    /// Static class that allows messages to be dispatched
    /// from one object to another. These messages can be
    /// sent immediately, the next frame, or set for a delay.
    /// </summary>
    public class MessageDispatcher
    {
        /// <summary>
        /// Determines if the message recipient is determined by
        /// the listening object's name or listening object's tags.
        /// </summary>
        private static int mRecipientType = EnumMessageRecipientType.NAME;
        public static int RecipientType
        {
            get { return mRecipientType; }
            set { mRecipientType = value; }
        }

        /// <summary>
        /// Allows the caller to assign a hander for when messages have no recipient
        /// </summary>
        public static MessageHandler MessageNotHandled = null;

        /// <summary>
        /// Used to ensure we send messages the 'next update'
        /// </summary>
        public static int FrameIndex = 0;

        /// <summary>
        /// Create the MessengerStub at startup and tie it into the Unity update path
        /// </summary>
#pragma warning disable 0414
        private static MessageDispatcherStub sStub = (new GameObject("MessageDispatcherStub")).AddComponent<MessageDispatcherStub>();
#pragma warning restore 0414

        /// <summary>
        /// List of messages that are being held until it's time for them to be dispatched
        /// </summary>
        private static List<IMessage> mMessages = new List<IMessage>();

        /// <summary>
        /// Dictionary to hold the different message types and the handlers that
        /// are tied to them. We layer this as "MessageType" and then "tag".
        /// </summary>
        private static Dictionary<string, Dictionary<string, MessageHandler>> mMessageHandlers = new Dictionary<string, Dictionary<string, MessageHandler>>();

        /// <summary>
        /// Queues listeners to be added after processing of the current 
        /// list of listeners has finished
        /// </summary>
        private static List<MessageListenerDefinition> mListenerAdds = new List<MessageListenerDefinition>();

        /// <summary>
        /// Queues listeners to be removed after the processing of the current
        /// list of listeners has finished
        /// </summary>
        private static List<MessageListenerDefinition> mListenerRemoves = new List<MessageListenerDefinition>();

        /// <summary>
        /// Clears all messages from the queue
        /// </summary>
        public static void ClearMessages()
        {
            mMessages.Clear();
        }

        /// <summary>
        /// Clears all listeners from the messenger
        /// </summary>
        public static void ClearListeners()
        {
            foreach (string lType in mMessageHandlers.Keys)
            {
                mMessageHandlers[lType].Clear();
            }

            mMessageHandlers.Clear();

            mListenerAdds.Clear();
            mListenerRemoves.Clear();
        }

        /// <summary>
        /// Tie the handler to the specified message type. 
        /// This way it will be raised when a message of this type comes in. This
        /// method allows us to listen for anyone's messages
        /// </summary>
        /// <param name="rMessageType">Message we want to listen for</param>
        /// <param name="rHandler">Hander to handle the message</param>
        public static void AddListener(string rMessageType, MessageHandler rHandler)
        {
            AddListener(rMessageType, "", rHandler, false);
        }

        /// <summary>
        /// Tie the handler to the specified message type. 
        /// This way it will be raised when a message of this type comes in. This
        /// method allows us to listen for anyone's messages
        /// </summary>
        /// <param name="rMessageType">Message we want to listen for</param>
        /// <param name="rHandler">Hander to handle the message</param>
        /// <param name="rImmediate">Determines if the function ignores the cache and forces the listener into the list now</param>
        public static void AddListener(string rMessageType, MessageHandler rHandler, bool rImmediate)
        {
            AddListener(rMessageType, "", rHandler, rImmediate);
        }

        /// <summary>
        /// Tie the handler to the specified message type. 
        /// This way it will be raised when a message of this type comes in. This
        /// method allows us to listen for a specific recipient
        /// </summary>
        /// <param name="rMessageType">Message we want to listen for</param>
        /// <param name="rHandler">Hander to handle the message</param>
        public static void AddListener(UnityEngine.Object rOwner, string rMessageType, MessageHandler rHandler)
        {
            AddListener(rOwner, rMessageType, rHandler, false);
        }

        /// <summary>
        /// Tie the handler to the specified message type. 
        /// This way it will be raised when a message of this type comes in. This
        /// method allows us to listen for a specific recipient
        /// </summary>
        /// <param name="rMessageType">Message we want to listen for</param>
        /// <param name="rHandler">Hander to handle the message</param>
        /// <param name="rImmediate">Determines if the function ignores the cache and forces the listener into the list now</param>
        public static void AddListener(UnityEngine.Object rOwner, string rMessageType, MessageHandler rHandler, bool rImmediate)
        {
            // If there is no owner, setup a general listener
            if (rOwner == null)
            {
                AddListener(rMessageType, "", rHandler, rImmediate);
            }
            // Depending on how the dispatcher is setup, use the name
            else if (mRecipientType == EnumMessageRecipientType.NAME)
            {
                if (rOwner is UnityEngine.Object)
                {
                    AddListener(rMessageType, ((UnityEngine.Object)rOwner).name, rHandler, rImmediate);
                }
            }
            // Depending on how the dispatcher is setup, use the tag
            else if (mRecipientType == EnumMessageRecipientType.TAG)
            {
                if (rOwner is GameObject)
                {
                    AddListener(rMessageType, ((GameObject)rOwner).tag, rHandler, rImmediate);
                }
            }
            // Use a general listener
            else
            {
                AddListener(rMessageType, "", rHandler, rImmediate);
            }
        }

        /// <summary>
        /// Tie the handler to the specified recipient and message type. 
        /// This way it will be raised when a message of this type comes in.
        /// </summary>
        /// <param name="rMessageType">Message we want to listen for</param>
        /// <param name="rFilter">Filter tag to determine who gets the message (recipient name or a tag)</param>
        /// <param name="rHandler">Hander to handle the message</param>
        public static void AddListener(string rMessageType, string rFilter, MessageHandler rHandler)
        {
            AddListener(rMessageType, rFilter, rHandler, false);
        }

        /// <summary>
        /// Tie the handler to the specified recipient and message type. 
        /// This way it will be raised when a message of this type comes in.
        /// </summary>
        /// <param name="rMessageType">Message we want to listen for</param>
        /// <param name="rFilter">Filter tag to determine who gets the message (recipient name or a tag)</param>
        /// <param name="rHandler">Hander to handle the message</param>
        /// <param name="rImmediate">Determines if the function ignores the cache and forces the listener into the list now</param>
        public static void AddListener(string rMessageType, string rFilter, MessageHandler rHandler, bool rImmediate)
        {
            MessageListenerDefinition lListener = MessageListenerDefinition.Allocate();
            lListener.MessageType = rMessageType;
            lListener.Filter = rFilter;
            lListener.Handler = rHandler;

            if (rImmediate)
            {
                AddListener(lListener);
                MessageListenerDefinition.Release(lListener);
            }
            else
            {
                mListenerAdds.Add(lListener);
            }
        }

        /// <summary>
        /// Internal function for removing listeners outside of the core
        /// processing logic. We do this to cleanly remove listeners and keep
        /// performance up.
        /// </summary>
        /// <param name="rListener"></param>
        private static void AddListener(MessageListenerDefinition rListener)
        {
            Dictionary<string, MessageHandler> lRecipientDictionary = null;

            // First check if we know about the message type
            if (mMessageHandlers.ContainsKey(rListener.MessageType))
            {
                lRecipientDictionary = mMessageHandlers[rListener.MessageType];
            }
            // If we don't know about the message type, add it
            else if (!mMessageHandlers.ContainsKey(rListener.MessageType))
            {
                lRecipientDictionary = new Dictionary<string, MessageHandler>();
                mMessageHandlers.Add(rListener.MessageType, lRecipientDictionary);
            }

            // Check if we know about the owner, then add the handler
            if (!lRecipientDictionary.ContainsKey(rListener.Filter)) { lRecipientDictionary.Add(rListener.Filter, null); }
            lRecipientDictionary[rListener.Filter] += rListener.Handler;

            // Release the definition
            MessageListenerDefinition.Release(rListener);
        }

        /// <summary>
        /// Stop listening for messages for the specified type.
        /// </summary>
        /// <param name="rMessageType">Message we want to listen for</param>
        /// <param name="rHandler">Hander to handle the message</param>
        public static void RemoveListener(string rMessageType, MessageHandler rHandler)
        {
            RemoveListener(rMessageType, "", rHandler, false);
        }

        /// <summary>
        /// Stop listening for messages for the specified type.
        /// </summary>
        /// <param name="rMessageType">Message we want to listen for</param>
        /// <param name="rHandler">Hander to handle the message</param>
        /// <param name="rImmediate">Determines if the function ignores the cache and forcibly removes the listener from the list now</param>
        public static void RemoveListener(string rMessageType, MessageHandler rHandler, bool rImmediate)
        {
            RemoveListener(rMessageType, "", rHandler, rImmediate);
        }

        /// <summary>
        /// Stop listening to the specified message type. 
        /// This way it won't be raised when a message of this type comes in. This
        /// method stops us from listen for a specific recipient
        /// </summary>
        /// <param name="rMessageType">Message we want to listen for</param>
        /// <param name="rHandler">Hander to handle the message</param>
        public static void RemoveListener(UnityEngine.Object rOwner, string rMessageType, MessageHandler rHandler)
        {
            RemoveListener(rOwner, rMessageType, rHandler, false);
        }

        /// <summary>
        /// Stop listening to the specified message type. 
        /// This way it won't be raised when a message of this type comes in. This
        /// method stops us from listen for a specific recipient
        /// </summary>
        /// <param name="rMessageType">Message we want to listen for</param>
        /// <param name="rHandler">Hander to handle the message</param>
        /// <param name="rImmediate">Determines if the function ignores the cache and forcibly removes the listener from the list now</param>
        public static void RemoveListener(UnityEngine.Object rOwner, string rMessageType, MessageHandler rHandler, bool rImmediate)
        {
            // If there is no owner, setup a general listener
            if (rOwner == null)
            {
                RemoveListener(rMessageType, "", rHandler, rImmediate);
            }
            // Depending on how the dispatcher is setup, use the name
            else if (mRecipientType == EnumMessageRecipientType.NAME)
            {
                if (rOwner is UnityEngine.Object)
                {
                    RemoveListener(rMessageType, ((UnityEngine.Object)rOwner).name, rHandler, rImmediate);
                }
            }
            // Depending on how the dispatcher is setup, use the tag
            else if (mRecipientType == EnumMessageRecipientType.TAG)
            {
                if (rOwner is GameObject)
                {
                    RemoveListener(rMessageType, ((GameObject)rOwner).tag, rHandler, rImmediate);
                }
            }
            // Use a general listener
            else
            {
                RemoveListener(rMessageType, "", rHandler, rImmediate);
            }
        }

        /// <summary>
        /// Stop listening for messages for the specified type and to the specified recipient.
        /// </summary>
        /// <param name="rMessageType">Message we want to listen for</param>
        /// <param name="rFilter">Filter used to determine who gets the message (recipient name or tag)</param>
        /// <param name="rHandler">Hander to handle the message</param>
        public static void RemoveListener(string rMessageType, string rFilter, MessageHandler rHandler)
        {
            RemoveListener(rMessageType, rFilter, rHandler, false);
        }

        /// <summary>
        /// Stop listening for messages for the specified type and to the specified recipient.
        /// </summary>
        /// <param name="rMessageType">Message we want to listen for</param>
        /// <param name="rFilter">Filter used to determine who gets the message (recipient name or tag)</param>
        /// <param name="rHandler">Hander to handle the message</param>
        /// <param name="rImmediate">Determines if the function ignores the cache and forcibly removes the listener from the list now</param>
        public static void RemoveListener(string rMessageType, string rFilter, MessageHandler rHandler, bool rImmediate)
        {
            // Post for clean up outside of any processing loops
            MessageListenerDefinition lListener = MessageListenerDefinition.Allocate();
            lListener.MessageType = rMessageType;
            lListener.Filter = rFilter;
            lListener.Handler = rHandler;

            if (rImmediate)
            {
                RemoveListener(lListener);
                MessageListenerDefinition.Release(lListener);
            }
            else
            {
                mListenerRemoves.Add(lListener);
            }
        }

        /// <summary>
        /// Internal function for removing listeners outside of the core
        /// processing logic. We do this to cleanly remove listeners and keep
        /// performance up.
        /// </summary>
        /// <param name="rListener"></param>
        private static void RemoveListener(MessageListenerDefinition rListener)
        {
            if (mMessageHandlers.ContainsKey(rListener.MessageType))
            {
                if (mMessageHandlers[rListener.MessageType].ContainsKey(rListener.Filter))
                {
                    // Test if we have listeners assigned
                    if (mMessageHandlers[rListener.MessageType][rListener.Filter] != null && rListener.Handler != null)
                    {
                        mMessageHandlers[rListener.MessageType][rListener.Filter] -= rListener.Handler;
                    }

                    // TT 2/18 - If removing the handler leaves no handlers left, remove the filter
                    if (mMessageHandlers[rListener.MessageType][rListener.Filter] == null)
                    {
                        mMessageHandlers[rListener.MessageType].Remove(rListener.Filter);
                    }

                    // TT 2/18 - If removing the filter leaves no filters left, remove the message type
                    if (mMessageHandlers[rListener.MessageType].Count == 0)
                    {
                        mMessageHandlers.Remove(rListener.MessageType);
                    }
                }
            }

            // Release the definition
            MessageListenerDefinition.Release(rListener);
        }

        /// <summary>
        /// Create and send a message object
        /// </summary>
        /// <param name="rType">Type of message to send</param>
        public static void SendMessage(string rType)
        {
            // Create the message
            Message lMessage = Message.Allocate();
            lMessage.Sender = null;
            lMessage.Recipient = "";
            lMessage.Type = rType;
            lMessage.Data = null;
            lMessage.Delay = EnumMessageDelay.IMMEDIATE;

            // Send it or store it
            SendMessage(lMessage);

            // Free up the message since we created it
            Message.Release(lMessage);
        }

        /// <summary>
        /// Create and send a message object
        /// </summary>
        /// <param name="rType">Type of message to send</param>
        /// <param name="rFilter">Filter to send only to those listening to the filter</param>
        public static void SendMessage(string rType, string rFilter)
        {
            // Create the message
            Message lMessage = Message.Allocate();
            lMessage.Sender = null;
            lMessage.Recipient = rFilter;
            lMessage.Type = rType;
            lMessage.Data = null;
            lMessage.Delay = EnumMessageDelay.IMMEDIATE;

            // Send it or store it
            SendMessage(lMessage);

            // Free up the message since we created it
            Message.Release(lMessage);
        }

        /// <summary>
        /// Create and send a message object
        /// </summary>
        /// <param name="rType">Type of message to send</param>
        /// <param name="rDelay">Delay (in seconds) before sending</param>
        public static void SendMessage(string rType, float rDelay)
        {
            // Create the message
            Message lMessage = Message.Allocate();
            lMessage.Sender = null;
            lMessage.Recipient = "";
            lMessage.Type = rType;
            lMessage.Data = null;
            lMessage.Delay = rDelay;

            // Send it or store it
            SendMessage(lMessage);

            // Free up the message since we created it
            if (rDelay == EnumMessageDelay.IMMEDIATE) { Message.Release(lMessage); }
        }

        /// <summary>
        /// Create and send a message object
        /// </summary>
        /// <param name="rType">Type of message to send</param>
        /// <param name="rFilter">Filter to send only to those listening to the filter</param>
        /// <param name="rDelay">Delay (in seconds) before sending</param>
        public static void SendMessage(string rType, string rFilter, float rDelay)
        {
            // Create the message
            Message lMessage = Message.Allocate();
            lMessage.Sender = null;
            lMessage.Recipient = rFilter;
            lMessage.Type = rType;
            lMessage.Data = null;
            lMessage.Delay = rDelay;

            // Send it or store it
            SendMessage(lMessage);

            // Free up the message since we created it
            if (rDelay == EnumMessageDelay.IMMEDIATE) { Message.Release(lMessage); }
        }

        /// <summary>
        /// Create and send a message object
        /// </summary>
        /// <param name="rSender">Sender</param>
        /// <param name="rType">Type of message to send</param>
        /// <param name="rData">Data to send</param>
        /// <param name="rDelay">Seconds to delay</param>
        public static void SendMessage(object rSender, string rType, object rData, float rDelay)
        {
            // Create the message
            Message lMessage = Message.Allocate();
            lMessage.Sender = rSender;
            lMessage.Recipient = "";
            lMessage.Type = rType;
            lMessage.Data = rData;
            lMessage.Delay = rDelay;

            // Send it or store it
            SendMessage(lMessage);

            // Free up the message since we created it
            if (rDelay == EnumMessageDelay.IMMEDIATE) { Message.Release(lMessage); }
        }

        /// <summary>
        /// Create and send a message object
        /// </summary>
        /// <param name="rSender">Sender</param>
        /// <param name="rRecipient">Recipient to send to</param>
        /// <param name="rType">Type of message to send</param>
        /// <param name="rData">Data to send</param>
        /// <param name="rDelay">Seconds to delay</param>
        public static void SendMessage(object rSender, object rRecipient, string rType, object rData, float rDelay)
        {
            // Create the message
            Message lMessage = Message.Allocate();
            lMessage.Sender = rSender;
            lMessage.Recipient = (rRecipient != null ? rRecipient : "");
            lMessage.Type = rType;
            lMessage.Data = rData;
            lMessage.Delay = rDelay;

            // Send it or store it
            SendMessage(lMessage);

            // Free up the message since we created it
            if (rDelay == EnumMessageDelay.IMMEDIATE) { Message.Release(lMessage); }
        }

        /// <summary>
        /// Create and send a message object
        /// </summary>
        /// <param name="rSender">Sender</param>
        /// <param name="rRecipient">Recipient name to send to</param>
        /// <param name="rType">Type of message to send</param>
        /// <param name="rData">Data to send</param>
        /// <param name="rDelay">Seconds to delay</param>
        public static void SendMessage(object rSender, string rRecipient, string rType, object rData, float rDelay)
        {
            // Create the message
            Message lMessage = Message.Allocate();
            lMessage.Sender = rSender;
            lMessage.Recipient = rRecipient;
            lMessage.Type = rType;
            lMessage.Data = rData;
            lMessage.Delay = rDelay;

            // Send it or store it
            SendMessage(lMessage);

            // Free up the message since we created it
            if (rDelay == EnumMessageDelay.IMMEDIATE) { Message.Release(lMessage); }
        }

        /// <summary>
        /// Send the message object as needed. In this instance, the caller needs to
        /// release the message.
        /// </summary>
        /// <param name="rMessage"></param>
        public static void SendMessage(IMessage rMessage)
        {
            bool lReportMissingRecipient = true;

            // Hold the message for the delay or the next frame (< 0)
            if (rMessage.Delay > 0 || rMessage.Delay < 0)
            {
                if (!mMessages.Contains(rMessage))
                {
                    rMessage.FrameIndex = FrameIndex;
                    mMessages.Add(rMessage);
                }

                lReportMissingRecipient = false;
            }
            // Send the message now if there are handlers
            else if (mMessageHandlers.ContainsKey(rMessage.Type))
            {
                // Get handlers for the message type
                Dictionary<string, MessageHandler> lHandlers = mMessageHandlers[rMessage.Type];
                foreach (string lFilter in lHandlers.Keys)
                {
                    // If there are no listeners left, flag the entries for removal
                    if (lHandlers[lFilter] == null)
                    {
                        RemoveListener(rMessage.Type, lFilter, null);
                        continue;
                    }

                    // Send to anyone listening to all filters
                    if (lFilter == "")
                    {
                        rMessage.IsSent = true;
                        lHandlers[lFilter](rMessage);

                        lReportMissingRecipient = false;
                    }
                    // Check if we are filtering by name (with an object)
                    else if (mRecipientType == EnumMessageRecipientType.NAME && rMessage.Recipient is UnityEngine.Object)
                    {
                        if (lFilter.ToLower() == ((UnityEngine.Object)rMessage.Recipient).name.ToLower())
                        {
                            rMessage.IsSent = true;
                            lHandlers[lFilter](rMessage);

                            lReportMissingRecipient = false;
                        }
                    }
                    // Check if we are filtering by tag (with an object)
                    else if (mRecipientType == EnumMessageRecipientType.TAG && rMessage.Recipient is UnityEngine.GameObject)
                    {
                        if (lFilter.ToLower() == ((UnityEngine.GameObject)rMessage.Recipient).tag.ToLower())
                        {
                            rMessage.IsSent = true;
                            lHandlers[lFilter](rMessage);

                            lReportMissingRecipient = false;
                        }
                    }
                    // If we have a string as the recipient, just test it
                    else if (rMessage.Recipient is string)
                    {
                        if (lFilter.ToLower() == ((string)rMessage.Recipient).ToLower())
                        {
                            rMessage.IsSent = true;
                            lHandlers[lFilter](rMessage);

                            lReportMissingRecipient = false;
                        }
                    }
                }
            }

            // If we were unable to send the message, we may need to report it
            if (lReportMissingRecipient)
            {
                if (MessageNotHandled == null)
                {
                    Debug.LogWarning("MessageDispatcher: Unhandled Message of type " + rMessage.Type);
                }
                else
                {
                    MessageNotHandled(rMessage);
                }

                // Flag the message as handled so we can remove it
                rMessage.IsHandled = true;
            }
        }

        /// <summary>
        /// Raised each tick so we can determine if it's time to send delayed messages
        /// </summary>
        public static void Update()
        {
            // Process the messages and determine if it's time to send them
            for (int i = 0; i < mMessages.Count; i++)
            {
                IMessage lMessage = mMessages[i];

                // Check if we're sending based on the next update
                if (lMessage.Delay == EnumMessageDelay.NEXT_UPDATE)
                {
                    if (lMessage.FrameIndex < FrameIndex)
                    {
                        lMessage.Delay = EnumMessageDelay.IMMEDIATE;
                    }
                }
                // Otherwise, we may be time based
                else
                {
                    // Reduce the delay
                    lMessage.Delay -= Time.deltaTime;
                    if (lMessage.Delay < 0)
                    {
                        lMessage.Delay = EnumMessageDelay.IMMEDIATE;
                    }
                }

                // If it's time, send the message and flag for removal
                if (!lMessage.IsSent && lMessage.Delay == EnumMessageDelay.IMMEDIATE)
                {
                    SendMessage(lMessage);
                }
            }

            // Remove sent messages
            for (int i = mMessages.Count - 1; i >= 0; i--)
            {
                IMessage lMessage = mMessages[i];
                if (lMessage.IsSent || lMessage.IsHandled)
                {
                    mMessages.RemoveAt(i);

                    // If a message is handled (done being processed),
                    // we'll release it for reuse.
                    if (lMessage.IsHandled)
                    {
                        Message.Release(lMessage);
                    }
                }
            }

            // Remove listeners
            for (int i = mListenerRemoves.Count - 1; i >= 0; i--)
            {
                RemoveListener(mListenerRemoves[i]);
            }

            mListenerRemoves.Clear();

            // Add Listeners
            for (int i = mListenerAdds.Count - 1; i >= 0; i--)
            {
                AddListener(mListenerAdds[i]);
            }

            mListenerAdds.Clear();
        }
    }

    /// <summary>
    /// Used by the messenger to hook into the unity update process. This allows us
    /// to delay messages instead of sending them right away. We'll release messages
    /// if a new level is loaded.
    /// </summary>
    public sealed class MessageDispatcherStub : MonoBehaviour
    {
        /// <summary>
        /// Raised first when the object comes into existance. Called
        /// even if script is not enabled.
        /// </summary>
        void Awake()
        {
            // Don't destroyed automatically when loading a new scene
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Called after the Awake() and before any update is called.
        /// </summary>
        public IEnumerator Start()
        {
            // Create the coroutine here so we don't re-create over and over
            WaitForEndOfFrame lWaitForEndOfFrame = new WaitForEndOfFrame();

            // Loop endlessly so we can flag when we're done with the frame
            while (true)
            {
                yield return lWaitForEndOfFrame;

                // Update the frame index for messages that need to be sent next update.
                // Since the max value of an int is 2,147,483,647, we can run 60FPS
                // for 414 days straight before we hit it. So... we won't worry about extra logic.
                MessageDispatcher.FrameIndex++;
            }
        }

        /// <summary>
        /// Update is called every frame. We pass this to our messenger
        /// </summary>
        void Update()
        {
            MessageDispatcher.Update();
        }

        /// <summary>
        /// Called when the dispatcher is disabled. We use this to
        /// clean up the event tables everytime a new level loads.
        /// </summary>
        public void OnDisable()
        {
            MessageDispatcher.ClearMessages();
            MessageDispatcher.ClearListeners();
        }
    }
}
