using System;
using UnityEngine;
using com.ootii.Collections;

namespace com.ootii.Messages
{
    /// <summary>
    /// Represents a basic message that can be passed around
    /// between objects.
    /// </summary>
    public class Message : IMessage
    {
        /// <summary>
        /// Enumeration for the message type. We use strings so they can
        /// be any value.
        /// </summary>
        protected string mType = "";
        public string Type
        {
            get { return mType; }
            set { mType = value; }
        }

        /// <summary>
        /// Sender of the message
        /// </summary>
        protected object mSender = null;
        public object Sender
        {
            get { return mSender; }
            set { mSender = value; }
        }

        /// <summary>
        /// Receiver of the message
        /// </summary>
        protected object mRecipient = null;
        public object Recipient
        {
            get { return mRecipient; }
            set { mRecipient = value; }
        }

        /// <summary>
        /// Time in seconds to delay the processing of the message
        /// </summary>
        protected float mDelay = 0;
        public float Delay
        {
            get { return mDelay; }
            set { mDelay = value; }
        }

        /// <summary>
        /// Core data of the message
        /// </summary>
        protected object mData = null;
        public object Data
        {
            get { return mData; }
            set { mData = value; }
        }

        /// <summary>
        /// Determines if the message was sent
        /// </summary>
        protected bool mIsSent = false;
        public bool IsSent
        {
            get { return mIsSent; }
            set { mIsSent = value; }
        }

        /// <summary>
        /// Determines if the message was handled
        /// </summary>
        protected bool mIsHandled = false;
        public bool IsHandled
        {
            get { return mIsHandled; }
            set { mIsHandled = value; }
        }

        /// <summary>
        /// Used to ensure frames are sent the next frame (when needed)
        /// </summary>
        protected int mFrameIndex = 0;
        public int FrameIndex
        {
            get { return mFrameIndex; }
            set { mFrameIndex = value; }
        }

        /// <summary>
        /// Clear this instance.
        /// </summary>
        public virtual void Clear()
        {
            mType = "";
            mSender = null;
            mRecipient = null;
            mData = null;
            mIsSent = false;
            mIsHandled = false;
            mDelay = 0.0f;
        }

        // ******************************** OBJECT POOL ********************************

        /// <summary>
        /// Allows us to reuse objects without having to reallocate them over and over
        /// </summary>
        private static ObjectPool<Message> sPool = new ObjectPool<Message>(40, 10);

        /// <summary>
        /// Pulls an object from the pool.
        /// </summary>
        /// <returns></returns>
        public static Message Allocate()
        {
            // Grab the next available object
            Message lInstance = sPool.Allocate();

            // Reset the sent flags. We do this so messages are flagged as 'completed'
            // by default.
            lInstance.IsSent = false;
            lInstance.IsHandled = false;

            // For this type, guarentee we have something
            // to hand back tot he caller
            if (lInstance == null) { lInstance = new Message(); }
            return lInstance;
        }

        /// <summary>
        /// Returns an element back to the pool.
        /// </summary>
        /// <param name="rEdge"></param>
        public static void Release(Message rInstance)
        {
            if (rInstance == null) { return; }

            // Reset the sent flags. We do this so messages are flagged as 'completed'
            // and removed by default.
            rInstance.IsSent = true;
            rInstance.IsHandled = true;

            // Make it available to others.
            sPool.Release(rInstance);
        }

        /// <summary>
        /// Returns an element back to the pool.
        /// </summary>
        /// <param name="rEdge"></param>
        public static void Release(IMessage rInstance)
        {
            if (rInstance == null) { return; }

            // We should never release an instance unless we're
            // sure we're done with it. So clearing here is fine
            rInstance.Clear();

            // Reset the sent flags. We do this so messages are flagged as 'completed'
            // and removed by default.
            rInstance.IsSent = true;
            rInstance.IsHandled = true;

            // Make it available to others.
            if (rInstance is Message)
            {
                sPool.Release((Message)rInstance);
            }
        }
    }
}
