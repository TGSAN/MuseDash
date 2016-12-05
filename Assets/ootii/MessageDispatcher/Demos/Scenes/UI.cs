using UnityEngine;
using System.Collections;
using System.Diagnostics;
using com.ootii.Messages;
using com.ootii.Utilities;

namespace com.ootii.Demos.MD
{
    public class UI : MonoBehaviour
    {
        private int mCounter = 0;
        private int mInstances = 0;
        private bool mIsTesting = false;
        private long mTicksPerMS = 0;
        private float mPhase1Time = 0;
        private float mPhase2Time = 0;
        private float mPhase3Time = 0;
        private int mPhase = 0;

        private com.ootii.Utilities.Profiler mProfiler = new com.ootii.Utilities.Profiler("Test");

        private float mBtnWidth = 150;
        private float mBtnHeight = 30;

        private Rect mPerformance;

        private Rect mClear;
        private Rect mLeadSphereRed;
        private Rect mSpheresBlue;
        private Rect mLeadCubeRed;
        private Rect mCubesBlue;
        private Rect mAllYellow;
        private Rect mAllGreen;
        private Rect mDelayBlue;
        private Rect mCustomMessage;
        private Rect mBadMessage;
        private Rect mTest;

        private string mCustomMessageResult = "";

        /// <summary>
        /// Build the button rectangles
        /// </summary>
        void Awake()
        {
            mTicksPerMS = System.TimeSpan.TicksPerMillisecond;

            mPerformance = new Rect((Screen.width / 2) - 200, 10, 400, 100);
            mClear = new Rect(10, 10 + ((mBtnHeight + 10) * 0), mBtnWidth, mBtnHeight);
            mLeadSphereRed = new Rect(10, 10 + ((mBtnHeight + 10) * 1), mBtnWidth, mBtnHeight);
            mSpheresBlue = new Rect(10, 10 + ((mBtnHeight + 10) * 2), mBtnWidth, mBtnHeight);
            mLeadCubeRed = new Rect(10, 10 + ((mBtnHeight + 10) * 3), mBtnWidth, mBtnHeight);
            mCubesBlue = new Rect(10, 10 + ((mBtnHeight + 10) * 4), mBtnWidth, mBtnHeight);
            mAllYellow = new Rect(10, 10 + ((mBtnHeight + 10) * 5), mBtnWidth, mBtnHeight);
            mAllGreen = new Rect(10, 10 + ((mBtnHeight + 10) * 6), mBtnWidth, mBtnHeight);
            mDelayBlue = new Rect(10, 10 + ((mBtnHeight + 10) * 7), mBtnWidth, mBtnHeight);
            mCustomMessage = new Rect(10, 10 + ((mBtnHeight + 10) * 8), mBtnWidth, mBtnHeight);
            mBadMessage = new Rect(10, 10 + ((mBtnHeight + 10) * 9), mBtnWidth, mBtnHeight);

            mTest = new Rect(10, 10 + ((mBtnHeight + 10) * 11), mBtnWidth, mBtnHeight);

            // Listen for counter update
            MessageDispatcher.AddListener("COUNTER", OnCounterMessageReceived);
            MessageDispatcher.AddListener("CUSTOM", OnCustomMessageReceived);

            MessageDispatcher.MessageNotHandled = OnMessageNotHandled;
        }

        /// <summary>
        /// Place the buttons and send messages when clicked
        /// </summary>
        void OnGUI()
        {
            // Show the buttons
            if (GUI.Button(mClear, "Clear"))
            {
                // Send the message to everyone listening
                MessageDispatcher.SendMessage("EVERYONE");
            }

            if (GUI.Button(mSpheresBlue, "Spheres Blue"))
            {
                // Send the message, but only the listeners filtering on "Sphere" will react
                MessageDispatcher.SendMessage("FILTER", "Sphere");
            }

            if (GUI.Button(mCubesBlue, "Cubes Blue"))
            {
                // Send the message, but only the listeners filtering on "Cube" will react
                MessageDispatcher.SendMessage("FILTER", "Cube");
            }

            if (GUI.Button(mLeadSphereRed, "Lead Sphere Red"))
            {
                // Send the message to a specific sphere (by name)
                MessageDispatcher.SendMessage(gameObject, "Sphere_1", "TARGET", null, EnumMessageDelay.IMMEDIATE);
            }

            if (GUI.Button(mLeadCubeRed, "Lead Cube Red"))
            {
                // Send the message to a specific cube (by name)
                MessageDispatcher.SendMessage(gameObject, "Cube_1", "TARGET", null, EnumMessageDelay.IMMEDIATE);
            }

            if (GUI.Button(mAllYellow, "All Yellow"))
            {
                // Send a custom message to everyone
                Message lMessage = new Message();
                lMessage.Type = "EVERYONE";
                lMessage.Sender = this;
                lMessage.Data = Color.yellow;
                lMessage.Delay = EnumMessageDelay.IMMEDIATE;
                MessageDispatcher.SendMessage(lMessage);
            }

            if (GUI.Button(mAllGreen, "All Green - Next Update"))
            {
                // Send a custom message to everyone
                Message lMessage = new Message();
                lMessage.Type = "EVERYONE";
                lMessage.Sender = this;
                lMessage.Data = Color.green;
                lMessage.Delay = EnumMessageDelay.NEXT_UPDATE;
                MessageDispatcher.SendMessage(lMessage);
            }

            if (GUI.Button(mDelayBlue, "Delay - All Blue"))
            {
                // Send a custom message to everyone (either approach works)

                //Message lMessage = new Message();
                //lMessage.Type = "EVERYONE";
                //lMessage.Sender = this;
                //lMessage.Data = Color.blue;
                //lMessage.Delay = 1.0f;
                //MessageDispatcher.SendMessage(lMessage);

                MessageDispatcher.SendMessage(null, "", "EVERYONE", Color.blue, 1f); 
            }

            if (GUI.Button(mCustomMessage, "Delay - Custom Msg"))
            {
                mCustomMessageResult = "";

                // Send a custom message to everyone
                MyCustomMessage lMessage = MyCustomMessage.Allocate();
                lMessage.Type = "CUSTOM";
                lMessage.MaxHealth = 100;
                lMessage.CurrentHealth = 50;
                lMessage.Sender = this;
                lMessage.Data = Color.blue;
                lMessage.Delay = 1.0f;
                MessageDispatcher.SendMessage(lMessage);
            }

            if (GUI.Button(mBadMessage, "Unhandled Message"))
            {
                // Send a custom message to everyone
                Message lMessage = new Message();
                lMessage.Type = "UNHANDLED";
                lMessage.Sender = this;
                MessageDispatcher.SendMessage(lMessage);
            }

            if (GUI.Button(mTest, "Perf. Test"))
            {
                mCustomMessageResult = "";

                mPhase = 0;
                mPhase1Time = 0;
                mPhase2Time = 0;
                mIsTesting = true;
                mInstances = 500;
                mCounter = 1;
            }

            // Update the testing
            if (mIsTesting)
            {
                if (mInstances > 0)
                {
                    if (mPhase == 0)
                    {
                        // Send a custom message to update the counter
                        mProfiler.Start();
                        MessageDispatcher.SendMessage("COUNTER");
                        mProfiler.Stop();

                        if (mCounter > 10)
                        {
                            mPhase1Time += mProfiler.ElapsedTime;
                        }

                        // Stop if we've finished the test
                        if (mCounter >= mInstances)
                        {
                            mPhase = 1;
                            mCounter = 0;
                        }
                    }
                    else if (mPhase == 1)
                    {
                        // Send a custom message to update the counter
                        mProfiler.Start();
                        gameObject.SendMessage("UpdateCounter");
                        mProfiler.Stop();

                        if (mCounter > 10)
                        {
                            mPhase2Time += mProfiler.ElapsedTime;
                        }

                        // Stop if we've finished the test
                        if (mCounter >= mInstances)
                        {
                            mPhase = 2;
                            mCounter = 0;
                        }
                    }
                    else
                    {
                        // Send a custom message to update the counter
                        mProfiler.Start();
                        UpdateCounter();
                        mPhase3Time += mProfiler.Stop();

                        if (mCounter > 10)
                        {
                            mPhase3Time += mProfiler.ElapsedTime;
                        }

                        // Stop if we've finished the test
                        if (mCounter >= mInstances)
                        {
                            mInstances = 0;
                        }
                    }
                }
            }

            // Show the label
            string lText = "Message Dispatcher";
            if (mCustomMessageResult.Length > 0) { lText += "\r\n" + mCustomMessageResult; }

            if (mIsTesting)
            {
                lText += "\r\n" + "" + mCounter + " msgs => MD: " + (mPhase1Time * mTicksPerMS).ToString("0") + " ticks   SMess: " + (mPhase2Time * mTicksPerMS).ToString("0") + " ticks   Direct: " + (mPhase3Time * mTicksPerMS).ToString("0") + " ticks";

                if (mInstances == 0)
                {
                    lText += "\r\n" + " MD: " + mPhase1Time.ToString("0.0000") + " ms   SMess: " + mPhase2Time.ToString("0.0000") + " ms   Direct: " + mPhase3Time.ToString("0.0000") + " ms";
                }
            }

            GUIStyle lStyle = new GUIStyle();
            lStyle.alignment = TextAnchor.MiddleCenter;
            lStyle.normal.textColor = Color.grey;
            lStyle.fontSize = 24;

            GUI.color = Color.white;
            GUI.contentColor = Color.white;

            GUI.Label(mPerformance, lText, lStyle);
        }

        /// <summary>
        /// Raised when the object gets a message that was sent to it
        /// </summary>
        /// <param name='rMessage'>Message that was sent </param>
        public void OnCounterMessageReceived(IMessage rMessage)
        {
            mCounter++;
        }

        /// <summary>
        /// Raised when we get the custom message
        /// </summary>
        /// <param name="rMessage">Customized message</param>
        public void OnCustomMessageReceived(IMessage rMessage)
        {
            int lMaxHealth = ((MyCustomMessage)rMessage).MaxHealth;
            int lCurrentHealth = ((MyCustomMessage)rMessage).CurrentHealth;

            mCustomMessageResult = lCurrentHealth + " of " + lMaxHealth + " health";

            MyCustomMessage.Release(rMessage);
        }

        /// <summary>
        /// Raised when there are no listeners for the message
        /// </summary>
        /// <param name="rMessage"></param>
        public void OnMessageNotHandled(IMessage rMessage)
        {
            UnityEngine.Debug.Log("Nope, no listener for '" + rMessage.Type + '"');
        }

        /// <summary>
        /// Updates the counter for testing.
        /// </summary>
        public void UpdateCounter()
        {
            mCounter++;
        }

        /// <summary>
        /// Update this instance.
        /// </summary>
        public void Update()
        {
        }
    }
}