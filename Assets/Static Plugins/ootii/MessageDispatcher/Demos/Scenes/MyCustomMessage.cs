using System;
using com.ootii.Collections;

namespace com.ootii.Messages
{
    public class MyCustomMessage : Message
    {
        public int MaxHealth = 0;

        public int CurrentHealth = 0;

        /// <summary>
        /// Clear the message content so it can be reused
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            MaxHealth = 0;
            CurrentHealth = 0;
        }

        // ******************************** OBJECT POOL ********************************

        /// <summary>
        /// Allows us to reuse objects without having to reallocate them over and over
        /// </summary>
        private static ObjectPool<MyCustomMessage> sPool = new ObjectPool<MyCustomMessage>(40, 10);

        /// <summary>
        /// Pulls an object from the pool.
        /// </summary>
        /// <returns></returns>
        public new static MyCustomMessage Allocate()
        {
            // Grab the next available object
            MyCustomMessage lInstance = sPool.Allocate();

            // Reset the sent flags. We do this so messages are flagged as 'completed'
            // by default.
            lInstance.IsSent = false;
            lInstance.IsHandled = false;

            // For this type, guarentee we have something
            // to hand back tot he caller
            if (lInstance == null) { lInstance = new MyCustomMessage(); }
            return lInstance;
        }

        /// <summary>
        /// Returns an element back to the pool.
        /// </summary>
        /// <param name="rEdge"></param>
        public static void Release(MyCustomMessage rInstance)
        {
            if (rInstance == null) { return; }
            rInstance.Clear();

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
        public new static void Release(IMessage rInstance)
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
            if (rInstance is MyCustomMessage)
            {
                sPool.Release((MyCustomMessage)rInstance);
            }
        }
    }
}
