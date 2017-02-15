using System;
using com.ootii.Collections;

namespace com.ootii.Messages
{
    /// <summary>
    /// Defines a specific listener so we can add/remove listeners
    /// outside of the core processing loop
    /// </summary>
    public class MessageListenerDefinition
    {
        /// <summary>
        /// Type of message to listen for
        /// </summary>
        public string MessageType;

        /// <summary>
        /// Filter for the messages
        /// </summary>
        public string Filter;

        /// <summary>
        /// Handler for the listener
        /// </summary>
        public MessageHandler Handler;

        // ******************************** OBJECT POOL ********************************

        /// <summary>
        /// Allows us to reuse objects without having to reallocate them over and over
        /// </summary>
        private static ObjectPool<MessageListenerDefinition> sPool = new ObjectPool<MessageListenerDefinition>(40, 10);

        /// <summary>
        /// Pulls an object from the pool.
        /// </summary>
        /// <returns></returns>
        public static MessageListenerDefinition Allocate()
        {
            // Grab the next available object
            MessageListenerDefinition lInstance = sPool.Allocate();
            lInstance.MessageType = "";
            lInstance.Filter = "";
            lInstance.Handler = null;

            // For this type, guarentee we have something
            // to hand back tot he caller
            if (lInstance == null) { lInstance = new MessageListenerDefinition(); }
            return lInstance;
        }

        /// <summary>
        /// Returns an element back to the pool.
        /// </summary>
        /// <param name="rEdge"></param>
        public static void Release(MessageListenerDefinition rInstance)
        {
            if (rInstance == null) { return; }

            // We should never release an instance unless we're
            // sure we're done with it. So clearing here is fine
            rInstance.MessageType = "";
            rInstance.Filter = "";
            rInstance.Handler = null;

            // Make it available to others.
            sPool.Release(rInstance);
        }
    }
}