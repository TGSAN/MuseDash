﻿using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

namespace com.ootii.Helpers
{
    public class EditorHelper
    {
        // *****************************************************************************
        // Handle Support
        // *****************************************************************************

        public static void DrawWireSphere(Vector3 rPosition, float rRadius, Color rColor)
        {
            Color lHandlesColor = Handles.color;
            Handles.color = rColor;

            Handles.DrawWireArc(rPosition, Vector3.forward, Vector3.up, 360f, rRadius);
            Handles.DrawWireArc(rPosition, Vector3.up, Vector3.forward, 360f, rRadius);
            Handles.DrawWireArc(rPosition, Vector3.right, Vector3.up, 360f, rRadius);

            Handles.color = lHandlesColor;
        }

        /// <summary>
        /// Draws a circle that always faces the scene camera
        /// </summary>
        /// <param name="rPosition"></param>
        /// <param name="rRadius"></param>
        /// <param name="rColor"></param>
        public static void DrawCircle(Vector3 rPosition, float rRadius, Color rColor)
        {
            Camera lCamera = SceneView.lastActiveSceneView.camera;

            Color lHandleColor = Handles.color;
            Handles.color = rColor;

            Vector3 lNormal = -lCamera.transform.forward;
            Handles.DrawSolidDisc(rPosition, lNormal, rRadius);

            Handles.color = lHandleColor;
        }

        /// <summary>
        /// Draws text on the scene
        /// </summary>
        /// <param name="rText"></param>
        /// <param name="rPosition"></param>
        /// <param name="rColor"></param>
        public static void DrawText(string rText, Vector3 rPosition, Color rColor)
        {
            Color lGUIColor = GUI.color;
            GUI.color = rColor;
            Handles.color = rColor;

            Handles.Label(rPosition, rText);

            GUI.color = lGUIColor;
            Handles.color = lGUIColor;
        }

        // *****************************************************************************
        // Field Support
        // *****************************************************************************

        /// <summary>
        /// Allows us to show an object field that supports interfaces
        /// </summary>
        /// <returns>Returns an object that is of the valid type or null</returns>
        public static GameObject InterfaceOwnerField<T>(GUIContent rLabel, GameObject rValue, bool rAllowSceneObjects, params GUILayoutOption[] rOptions)
        {
            GameObject lGameObject = EditorGUILayout.ObjectField(rLabel, rValue, typeof(GameObject), rAllowSceneObjects, rOptions) as GameObject;
            if (lGameObject != null)
            {
                if (InterfaceHelper.GetComponent<T>(lGameObject) == null)
                {
                    lGameObject = null;
                }
            }

            return lGameObject;
        }

        /// Holds the list of all the current layers
        private static string[] sLayerNames = null;
        private static int[] sLayerValues = null;

        /// <summary>
        /// Renders out the layer mask selection box for us
        /// 
        /// Props to Bunny83
        /// http://answers.unity3d.com/questions/60959/mask-field-in-the-editor.html
        /// </summary>
        /// <param name="rGUIContent"></param>
        /// <param name="rMask"></param>
        /// <param name="rOptions"></param>
        /// <returns></returns>
        public static int LayerMaskField(GUIContent rGUIContent, int rMask, params GUILayoutOption[] rOptions)
        {
            int lValue = rMask;
            int lMaskValue = 0;

            for (int i = 0; i < sLayerNames.Length; i++)
            {
                if (sLayerValues[i] != 0)
                {
                    if ((lValue & sLayerValues[i]) == sLayerValues[i])
                        lMaskValue |= 1 << i;
                }
                else if (lValue == 0)
                    lMaskValue |= 1 << i;
            }

            int lNewMaskVal = EditorGUILayout.MaskField(rGUIContent, lMaskValue, sLayerNames, rOptions);
            int lChanges = lMaskValue ^ lNewMaskVal;

            for (int i = 0; i < sLayerValues.Length; i++)
            {
                if ((lChanges & (1 << i)) != 0)            // has this list item changed?
                {
                    if ((lNewMaskVal & (1 << i)) != 0)     // has it been set?
                    {
                        if (sLayerValues[i] == 0)           // special case: if "0" is set, just set the val to 0
                        {
                            lValue = 0;
                            break;
                        }
                        else
                            lValue |= sLayerValues[i];
                    }
                    else                                  // it has been reset
                    {
                        lValue &= ~sLayerValues[i];
                    }
                }
            }

            return lValue;
        }

        /// <summary>
        /// Reload the layer list. We may need to do this every so often
        /// </summary>
        public static void RefreshLayers()
        {
            List<string> lLayerNames = new List<string>();
            List<int> lLayerValues = new List<int>();

            for (int i = 0; i < 32; i++)
            {
                try
                {
                    string lName = LayerMask.LayerToName(i);
                    if (lName != "")
                    {
                        lLayerNames.Add(lName);
                        lLayerValues.Add(1 << i);
                    }
                }
                catch { }
            }

            sLayerNames = lLayerNames.ToArray();
            sLayerValues = lLayerValues.ToArray();
        }
       
        
        // *****************************************************************************
        // Style Support
        // *****************************************************************************
        public static Texture2D InheritedBackground;
        public static Texture2D InstanceBackground;
        public static Texture2D GreenPlusButton;
        public static Texture2D BluePlusButton;

        public static GUIStyle InstanceStyle;
        public static GUIStyle InheritedStyle;
        public static GUIStyle IndexStyle;
        public static GUIStyle GreenPlusButtonStyle;
        public static GUIStyle BlueXButtonStyle;
        public static GUIStyle BluePlusButtonStyle;

        public static Color Border = new Color(84f / 255f, 84f / 255f, 84f / 255f, 1f);
        public static Color LightBorder = new Color(124f / 255f, 124f / 255f, 124f / 255f, 1f);
        public static Color LightOrange = new Color(241f / 255f, 156f / 255f, 117f / 255f, 1f);
        public static Color OverrideBlue = new Color(202f / 255f, 209f / 255f, 220f / 255f, 1f);
        public static Color InheritedGray = new Color(200f / 255f, 200f / 255f, 200f / 255f, 1f);

        /// <summary>
        /// Renders the inspector title for our asset
        /// </summary>
        public static void DrawSmallTitle(string rTitle)
        {
            EditorGUILayout.BeginHorizontal(EditorHelper.SmallBox);

            GUILayout.Space(5);

            EditorGUILayout.LabelField(rTitle, EditorHelper.SmallTitle);

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Renders the inspector title for our asset
        /// </summary>
        public static void DrawInspectorTitle(string rTitle)
        {
            EditorGUILayout.BeginHorizontal(EditorHelper.TitleBox);

            EditorGUILayout.LabelField("", ootiiIcon, GUILayout.Width(24f), GUILayout.Height(24f));

            GUILayout.Space(5);

            EditorGUILayout.LabelField(rTitle, EditorHelper.Title);

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Renders the inspector title for our asset
        /// </summary>
        public static void DrawInspectorDescription(string rDescription, MessageType rMessageType)
        {
            Color lGUIColor = GUI.color;

            GUI.color = CreateColor(242f, 164f, 130f, 0.8f);
            EditorGUILayout.HelpBox(rDescription, rMessageType);

            GUI.color = lGUIColor;
        }

        /// <summary>
        /// Renders a simple line to the inspector
        /// </summary>
        public static void DrawLine()
        {
            EditorGUILayout.BeginHorizontal(EditorHelper.Line);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Label with small text
        /// </summary>
        private static GUIStyle mReadOnlyLabel = null;
        public static GUIStyle ReadOnlyLabel
        {
            get
            {
                if (mReadOnlyLabel == null)
                {
                    Texture2D lTexture = Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Editor/ClearBox" : "Editor/ClearBox");

                    mReadOnlyLabel = new GUIStyle(GUI.skin.label);
                    mReadOnlyLabel.normal.background = lTexture;
                    mReadOnlyLabel.border = new RectOffset(3, 3, 2, 2);
                }

                return mReadOnlyLabel;
            }
        }

        /// <summary>
        /// Label with small text
        /// </summary>
        private static GUIStyle mSmallText = null;
        public static GUIStyle SmallText
        {
            get
            {
                if (mSmallText == null)
                {
                    mSmallText = new GUIStyle(GUI.skin.label);
                    mSmallText.fontSize = 9;
                }

                return mSmallText;
            }
        }

        /// <summary>
        /// Box used to group standard GUI elements
        /// </summary>
        private static GUIStyle mWindow = null;
        public static GUIStyle Window
        {
            get
            {
                if (mWindow == null)
                {
                    mWindow = new GUIStyle(GUI.skin.window);
                    mWindow.fontStyle = FontStyle.Bold;
                    mWindow.alignment = TextAnchor.UpperLeft;
                }

                return mWindow;
            }
        }

        /// <summary>
        /// Box used to group standard GUI elements
        /// </summary>
        private static GUIStyle mBox = null;
        public static GUIStyle Box
        {
            get
            {
                if (mBox == null)
                {
                    Texture2D lTexture = Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Editor/OrangeGrayBox_pro" : "Editor/OrangeGrayBox");

                    mBox = new GUIStyle(GUI.skin.box);
                    mBox.normal.background = lTexture;
                    mBox.padding = new RectOffset(2, 2, 4, 4);
                    mBox.margin = new RectOffset(0, 0, 0, 0);
                }

                return mBox;
            }
        }

        /// <summary>
        /// Label with small text
        /// </summary>
        private static GUIStyle mSmallTitle = null;
        public static GUIStyle SmallTitle
        {
            get
            {
                //if (mSmallTitle == null)
                {
                    Font lFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
                    if (lFont == null) { lFont = EditorStyles.standardFont; }

                    mSmallTitle = new GUIStyle(GUI.skin.label);
                    mSmallTitle.font = lFont;
                    mSmallTitle.fontSize = 12;
                    mSmallTitle.fontStyle = FontStyle.Bold;
                    mSmallTitle.normal.textColor = Color.white;
                    mSmallTitle.fixedHeight = 18f;
                    mSmallTitle.fixedWidth = 200f;
                    mSmallTitle.padding = new RectOffset(0, 0, 1, 0);
                }

                return mSmallTitle;
            }
        }

        /// <summary>
        /// Label with small text
        /// </summary>
        private static GUIStyle mTitle = null;
        public static GUIStyle Title
        {
            get
            {
                if (mTitle == null)
                {
                    Font lFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
                    if (lFont == null) { lFont = EditorStyles.standardFont; }

                    mTitle = new GUIStyle(GUI.skin.label);
                    mTitle.font = lFont;
                    mTitle.fontSize = 14;
                    mTitle.fontStyle = FontStyle.Bold;
                    mTitle.normal.textColor = Color.white;
                    mTitle.fixedHeight = 22f;
                    mTitle.fixedWidth = 200f;
                    mTitle.padding = new RectOffset(0, 0, 4, 0);
                }

                return mTitle;
            }
        }

        /// <summary>
        /// Box used to group standard GUI elements
        /// </summary>
        private static GUIStyle mSmallBox = null;
        public static GUIStyle SmallBox
        {
            get
            {
                //if (mSmallBox == null)
                {
                    Texture2D lTexture = Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Editor/TitleBox" : "Editor/TitleBox");

                    mSmallBox = new GUIStyle(GUI.skin.box);
                    mSmallBox.normal.background = lTexture;
                    mSmallBox.padding = new RectOffset(0, 0, 0, 0);
                }

                return mSmallBox;
            }
        }

        /// <summary>
        /// Box used to group standard GUI elements
        /// </summary>
        private static GUIStyle mTitleBox = null;
        public static GUIStyle TitleBox
        {
            get
            {
                if (mTitleBox == null)
                {
                    Texture2D lTexture = Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Editor/TitleBox" : "Editor/TitleBox");

                    mTitleBox = new GUIStyle(GUI.skin.box);
                    mTitleBox.normal.background = lTexture;
                    mTitleBox.padding = new RectOffset(3, 3, 3, 3);
                }

                return mTitleBox;
            }
        }

        /// <summary>
        /// Box used to group standard GUI elements
        /// </summary>
        private static GUIStyle mThinGroupBox = null;
        public static GUIStyle ThinGroupBox
        {
            get
            {
                if (mThinGroupBox == null)
                {
                    Texture2D lTexture = Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Editor/GroupBox_pro" : "Editor/GroupBox");

                    mThinGroupBox = new GUIStyle(GUI.skin.box);
                    mThinGroupBox.normal.background = lTexture;
                    mThinGroupBox.padding = new RectOffset(2, 2, 2, 2);
                }

                return mThinGroupBox;
            }
        }

        /// <summary>
        /// Box used to group standard GUI elements
        /// </summary>
        private static GUIStyle mGroupBox = null;
        public static GUIStyle GroupBox
        {
            get
            {
                if (mGroupBox == null)
                {
                    Texture2D lTexture = Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Editor/GroupBox_pro" : "Editor/GroupBox");

                    mGroupBox = new GUIStyle(GUI.skin.box);
                    mGroupBox.normal.background = lTexture;
                    mGroupBox.padding = new RectOffset(3, 3, 3, 3);
                }

                return mGroupBox;
            }
        }

        /// <summary>
        /// Box used to group standard GUI elements
        /// </summary>
        private static GUIStyle mRedBox = null;
        public static GUIStyle RedBox
        {
            get
            {
                if (mRedBox == null)
                {
                    Texture2D lTexture = Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Editor/RedBox_pro" : "Editor/RedBox");

                    mRedBox = new GUIStyle(GUI.skin.box);
                    mRedBox.normal.background = lTexture;
                }

                return mRedBox;
            }
        }

        /// <summary>
        /// Box used to group standard GUI elements
        /// </summary>
        private static GUIStyle mGreenBox = null;
        public static GUIStyle GreenBox
        {
            get
            {
                if (mGreenBox == null)
                {
                    Texture2D lTexture = Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Editor/GreenBox_pro" : "Editor/GreenBox");

                    mGreenBox = new GUIStyle(GUI.skin.box);
                    mGreenBox.normal.background = lTexture;
                }

                return mGreenBox;
            }
        }

        /// <summary>
        /// Draws a rectangle in the specified color
        /// </summary>
        //private static Texture2D mWhitePixel = null;
        //private static GUIStyle mRectStyle = null;
        public static void DrawRect(Rect rRect, Color rColor)
        {
            EditorGUI.DrawRect(rRect, rColor);

            //if (mWhitePixel == null) { mWhitePixel = Resources.Load<Texture2D>("Editor/WhitePixel"); }
            //if (mRectStyle == null) { mRectStyle = new GUIStyle(); }

            //Color lColor = GUI.color;

            //GUI.color = rColor;
            //mRectStyle.normal.background = mWhitePixel;
            //GUI.Box(rPosition, GUIContent.none, mRectStyle);
            //GUI.color = lColor;
        }

        /// <summary>
        /// Draws a rectangle in the specified color
        /// </summary>
        public static void DrawBox(Rect rRect, Color rFillColor, Color rBorderColor)
        {
            EditorGUI.DrawRect(rRect, rFillColor);

            Rect lTop = new Rect(rRect.x, rRect.y, rRect.width, 1f);
            EditorGUI.DrawRect(lTop, rBorderColor);

            Rect lBottom = new Rect(rRect.x, rRect.y + rRect.height - 1f, rRect.width, 1f);
            EditorGUI.DrawRect(lBottom, rBorderColor);

            Rect lLeft = new Rect(rRect.x, rRect.y, 1f, rRect.height);
            EditorGUI.DrawRect(lLeft, rBorderColor);

            Rect lRight = new Rect(rRect.x + rRect.width - 1f, rRect.y, 1f, rRect.height);
            EditorGUI.DrawRect(lRight, rBorderColor);
        }

        /// <summary>
        /// Draws a rectangle in the specified color
        /// </summary>
        public static void DrawRoundedBox(Rect rRect, Color rFillColor, Color rBorderColor)
        {
            Rect lFill = new Rect(rRect.x + 1, rRect.y + 1, rRect.width - 2, rRect.height - 2);
            EditorGUI.DrawRect(lFill, rFillColor);

            Rect lTop = new Rect(rRect.x + 1, rRect.y, rRect.width - 2, 1f);
            EditorGUI.DrawRect(lTop, rBorderColor);

            Rect lBottom = new Rect(rRect.x + 1, rRect.y + rRect.height - 1f, rRect.width - 2, 1f);
            EditorGUI.DrawRect(lBottom, rBorderColor);

            Rect lLeft = new Rect(rRect.x, rRect.y + 1, 1f, rRect.height - 2);
            EditorGUI.DrawRect(lLeft, rBorderColor);

            Rect lRight = new Rect(rRect.x + rRect.width - 1f, rRect.y + 1, 1f, rRect.height - 2);
            EditorGUI.DrawRect(lRight, rBorderColor);
        }

        /// <summary>
        /// Status bar for the bottom window
        /// </summary>
        private static GUIStyle mStatusBar = null;
        public static GUIStyle StatusBar
        {
            get
            {
                if (mStatusBar == null)
                {
                    mStatusBar = new GUIStyle(GUI.skin.FindStyle("Toolbar"));
                    mStatusBar.fixedHeight = 21;
                }

                return mStatusBar;
            }
        }

        /// <summary>
        /// Box used to draw a solid line
        /// </summary>
        private static GUIStyle mLine = null;
        public static GUIStyle Line
        {
            get
            {
                if (mLine == null)
                {
                    Texture2D lTexture = Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Editor/Line_pro" : "Editor/Line");

                    mLine = new GUIStyle(GUI.skin.box);
                    mLine.border.top = 0;
                    mLine.border.left = 0;
                    mLine.border.right = 0;
                    mLine.border.bottom = 0;
                    mLine.padding.top = 0;
                    mLine.padding.left = 0;
                    mLine.padding.right = 0;
                    mLine.padding.bottom = 0;
                    mLine.fixedHeight = 8f;
                    mLine.normal.background = lTexture;
                }

                return mLine;
            }
        }

        /// <summary>
        /// Gray select target button
        /// </summary>
        private static GUIStyle mTinyButton = null;
        public static GUIStyle TinyButton
        {
            get
            {
                if (mTinyButton == null)
                {
                    mTinyButton = new GUIStyle(EditorStyles.miniButton);
                    mTinyButton.padding = new RectOffset(0, 0, 0, 0);
                    mTinyButton.margin = new RectOffset(0, 0, 0, 0);
                    mTinyButton.fixedHeight = 14;
                }

                return mTinyButton;
            }
        }

        /// <summary>
        /// Gray select target button
        /// </summary>
        private static GUIStyle mBlueGearButton = null;
        public static GUIStyle BlueGearButton
        {
            get
            {
                if (mBlueGearButton == null)
                {
                    mBlueGearButton = new GUIStyle();
                    mBlueGearButton.normal.background = Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Editor/GearButtonBlue_pro" : "Editor/GearButtonBlue");
                    mBlueGearButton.margin = new RectOffset(0, 0, 2, 0);
                }

                return mBlueGearButton;
            }
        }

        /// <summary>
        /// Gray select target button
        /// </summary>
        private static GUIStyle mGraySelectButton = null;
        public static GUIStyle GraySelectButton
        {
            get
            {
                if (mGraySelectButton == null)
                {
                    mGraySelectButton = new GUIStyle();
                    mGraySelectButton.normal.background = Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Editor/SelectButtonGray" : "Editor/SelectButtonGray");
                    mGraySelectButton.margin = new RectOffset(0, 0, 2, 0);
                }

                return mGraySelectButton;
            }
        }

        /// <summary>
        /// Gray add button
        /// </summary>
        private static GUIStyle mGrayAddButton = null;
        public static GUIStyle GrayAddButton
        {
            get
            {
                if (mGrayAddButton == null)
                {
                    mGrayAddButton = new GUIStyle();
                    mGrayAddButton.normal.background = Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Editor/AddButtonGray" : "Editor/AddButtonGray");
                    mGrayAddButton.margin = new RectOffset(0, 0, 2, 0);
                }

                return mGrayAddButton;
            }
        }

        /// <summary>
        /// Gray delete button
        /// </summary>
        private static GUIStyle mGrayXButton = null;
        public static GUIStyle GrayXButton
        {
            get
            {
                if (mGrayXButton == null)
                {
                    mGrayXButton = new GUIStyle();
                    mGrayXButton.normal.background = Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Editor/DeleteButtonGray" : "Editor/DeleteButtonGray");
                    mGrayXButton.margin = new RectOffset(0, 0, 2, 0);
                }

                return mGrayXButton;
            }
        }

        /// <summary>
        /// Blue select target button
        /// </summary>
        private static GUIStyle mBlueSelectButton = null;
        public static GUIStyle BlueSelectButton
        {
            get
            {
                if (mBlueSelectButton == null)
                {
                    mBlueSelectButton = new GUIStyle();
                    mBlueSelectButton.normal.background = Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Editor/SelectButton" : "Editor/SelectButton");
                    mBlueSelectButton.margin = new RectOffset(0, 0, 2, 0);
                }

                return mBlueSelectButton;
            }
        }

        /// <summary>
        /// Blue add button
        /// </summary>
        private static GUIStyle mBlueAddButton = null;
        public static GUIStyle BlueAddButton
        {
            get
            {
                if (mBlueAddButton == null)
                {
                    mBlueAddButton = new GUIStyle();
                    mBlueAddButton.normal.background = Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Editor/AddButtonBlue" : "Editor/AddButtonBlue");
                    mBlueAddButton.margin = new RectOffset(0, 0, 2, 0);
                }

                return mBlueAddButton;
            }
        }

        /// <summary>
        /// Blue select target button
        /// </summary>
        private static GUIStyle mBlueXButton = null;
        public static GUIStyle BlueXButton
        {
            get
            {
                if (mBlueXButton == null)
                {
                    mBlueXButton = new GUIStyle();
                    mBlueXButton.normal.background = Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Editor/DeleteButtonBlue" : "Editor/DeleteButtonBlue");
                    mBlueXButton.margin = new RectOffset(0, 0, 2, 0);
                }

                return mBlueXButton;
            }
        }

        /// <summary>
        /// Red delete button
        /// </summary>
        public static GUIStyle mRedXButton = null;
        public static GUIStyle RedXButton
        {
            get
            {
                if (mRedXButton == null)
                {
                    mRedXButton = new GUIStyle();
                    mRedXButton.normal.background = Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Editor/DeleteButton" : "Editor/DeleteButton");
                    mRedXButton.margin = new RectOffset(0, 0, 2, 0);
                }

                return mRedXButton;
            }
        }

        /// <summary>
        /// Label
        /// </summary>
        public static GUIStyle mLabel = null;
        public static GUIStyle Label
        {
            get
            {
                if (mLabel == null)
                {
                    mLabel = new GUIStyle(GUI.skin.label);
                }

                return mLabel;
            }
        }

        /// <summary>
        /// Label
        /// </summary>
        public static GUIStyle mWrapLabel = null;
        public static GUIStyle WrapLabel
        {
            get
            {
                if (mWrapLabel == null)
                {
                    mWrapLabel = new GUIStyle(GUI.skin.label);
                    mWrapLabel.wordWrap = true;
                }

                return mWrapLabel;
            }
        }

        /// <summary>
        /// Label
        /// </summary>
        public static GUIStyle mSelectedLabel = null;
        public static GUIStyle SelectedLabel
        {
            get
            {
                if (mSelectedLabel == null)
                {
                    Texture2D lTexture = Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Editor/BlueSelected" : "Editor/BlueSelected");

                    mSelectedLabel = new GUIStyle(GUI.skin.label);
                    mSelectedLabel.normal.textColor = Color.white;
                    mSelectedLabel.normal.background = lTexture;
                }

                return mSelectedLabel;
            }
        }

        /// <summary>
        /// Disabled Label
        /// </summary>
        public static GUIStyle mDisabledLabel = null;
        public static GUIStyle DisabledLabel
        {
            get
            {
                if (mDisabledLabel == null)
                {
                    mDisabledLabel = new GUIStyle(GUI.skin.label);
                    mDisabledLabel.normal.textColor = Color.gray;
                }

                return mDisabledLabel;
            }
        }

        private static GUIStyle mValueStyle = null;
        public static GUIStyle ValueStyle
        {
            get
            {
                if (mValueStyle == null)
                {
                    mValueStyle = new GUIStyle(GUI.skin.label);
                    mValueStyle.alignment = TextAnchor.MiddleRight;
                }

                return mValueStyle;
            }
        }

        /// <summary>
        /// Scroll region
        /// </summary>
        public static GUIStyle mScrollArea = null;
        public static GUIStyle ScrollArea
        {
            get
            {
                if (mScrollArea == null)
                {
                    mScrollArea = new GUIStyle(GUI.skin.box);
                    mScrollArea.margin.left = 0;
                    mScrollArea.margin.right = 0;
                    mScrollArea.margin.top = 0;
                    mScrollArea.margin.bottom = 0;
                }

                return mScrollArea;
            }
        }

        /// <summary>
        /// Box used to group standard GUI elements
        /// </summary>
        private static GUIStyle mootiiIcon = null;
        private static GUIStyle ootiiIcon
        {
            get
            {
                if (mootiiIcon == null)
                {
                    Texture2D lTexture = Resources.Load<Texture2D>(EditorGUIUtility.isProSkin ? "Editor/ootii_Icon" : "Editor/ootii_Icon");

                    mootiiIcon = new GUIStyle(GUI.skin.box);
                    mootiiIcon.normal.background = lTexture;
                    mootiiIcon.padding = new RectOffset(0, 0, 0, 0);
                    mootiiIcon.margin = new RectOffset(0, 0, 0, 0);
                }

                return mootiiIcon;
            }
        }

        /// <summary>
        /// Initialize the styles we use
        /// </summary>
        public static void InitializeStyles()
        {
            // Load the resources
            InheritedBackground = CreateTexture(2, 2, new Color(0.5f, 0.5f, 0.5f, 0.25f));
            InstanceBackground = CreateTexture(2, 2, new Color(0.5f, 0.5f, 0.5f, 0.75f));
            GreenPlusButton = Resources.Load<Texture2D>("AddButton");
            BluePlusButton = Resources.Load<Texture2D>("AddButtonBlue");

            InstanceStyle = new GUIStyle(GUI.skin.textField);
            InstanceStyle.normal.background = InstanceBackground;

            InheritedStyle = new GUIStyle(GUI.skin.textField);
            InheritedStyle.normal.background = InheritedBackground;

            IndexStyle = new GUIStyle(GUI.skin.textField);
            IndexStyle.alignment = TextAnchor.MiddleRight;

            GreenPlusButtonStyle = new GUIStyle();
            GreenPlusButtonStyle.normal.background = GreenPlusButton;
            GreenPlusButtonStyle.margin = new RectOffset(0, 0, 2, 0);

            BluePlusButtonStyle = new GUIStyle();
            BluePlusButtonStyle.normal.background = BluePlusButton;
            BluePlusButtonStyle.margin = new RectOffset(0, 0, 2, 0);
        }

        /// <summary>
        /// Creates a texture given the specified color
        /// </summary>
        /// <param name="rWidth">Width of the texture</param>
        /// <param name="rHeight">Height of the texture</param>
        /// <param name="rColor">Color of the texture</param>
        /// <returns></returns>
        public static Texture2D CreateTexture(int rWidth, Color rColor)
        {
            int lHeight = rWidth;
            int lRow = lHeight / 2;
            Color lColor = new Color(0f, 0f, 0f, 0f);

            Color[] lPixels = new Color[rWidth * lHeight];
            for (int i = 0; i < lPixels.Length; i++)
            {
                lPixels[i] = lColor;
            }

            // Set the border (top, left and right, bottom)
            for (int x = 0; x < rWidth; x++) { lPixels[(lRow * rWidth) + x] = rColor; }

            Texture2D lResult = new Texture2D(rWidth, lHeight);
            lResult.SetPixels(lPixels);
            lResult.Apply();

            return lResult;
        }

        /// <summary>
        /// Creates a texture given the specified color
        /// </summary>
        /// <param name="rWidth">Width of the texture</param>
        /// <param name="rHeight">Height of the texture</param>
        /// <param name="rColor">Color of the texture</param>
        /// <returns></returns>
        public static Texture2D CreateTexture(int rWidth, int rHeight, Color rColor)
        {
            Color[] lPixels = new Color[rWidth * rHeight];
            for (int i = 0; i < lPixels.Length; i++)
            {
                lPixels[i] = rColor;
            }

            Texture2D lResult = new Texture2D(rWidth, rHeight);
            lResult.SetPixels(lPixels);
            lResult.Apply();

            return lResult;
        }

        /// <summary>
        /// Creates a texture given the specified color
        /// </summary>
        /// <param name="rWidth">Width of the texture</param>
        /// <param name="rHeight">Height of the texture</param>
        /// <param name="rColor">Color of the texture</param>
        /// <returns></returns>
        public static Texture2D CreateTexture(int rWidth, int rHeight, Color rColor, Color rBorderColor)
        {
            Color[] lPixels = new Color[rWidth * rHeight];
            for (int i = 0; i < lPixels.Length; i++)
            {
                lPixels[i] = rColor;
            }

            // Set the border (top, left and right, bottom)
            for (int x = 0; x < rWidth; x++) { lPixels[x] = rBorderColor; }
            for (int y = 0; y < rHeight; y++) { lPixels[y * rWidth] = rBorderColor; lPixels[(y * rWidth) + (rWidth - 1)] = rBorderColor; }
            for (int x = lPixels.Length - rWidth; x < lPixels.Length; x++) { lPixels[x] = rBorderColor; }

            Texture2D lResult = new Texture2D(rWidth, rHeight);
            lResult.SetPixels(lPixels);
            lResult.Apply();

            return lResult;
        }

        /// <summary>
        /// Create a color using 0-255 values
        /// </summary>
        /// <param name="rRed"></param>
        /// <param name="rGreen"></param>
        /// <param name="rBlue"></param>
        /// <returns></returns>
        public static Color CreateColor(float rRed, float rGreen, float rBlue)
        {
            Color lColor = new Color(rRed / 255f, rGreen / 255f, rBlue / 255f);
            return lColor.gamma;
        }

        /// <summary>
        /// Create a color using 0-255 values
        /// </summary>
        /// <param name="rRed"></param>
        /// <param name="rGreen"></param>
        /// <param name="rBlue"></param>
        /// <returns></returns>
        public static Color CreateColor(float rRed, float rGreen, float rBlue, float rAlpha)
        {
            Color lColor = new Color(rRed / 255f, rGreen / 255f, rBlue / 255f, rAlpha);
            return lColor.gamma;
        }
    }
}

#endif
