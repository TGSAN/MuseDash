using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using qtools.qhierarchy.pdata;
using System;

namespace qtools.qhierarchy.phierarchy
{
	public class QHierarchySettingsWindow : EditorWindow 
	{	
        // STATIC
		[MenuItem ("Tools/QHierarchy")]	
		public static void ShowWindow () 
		{ 
			EditorWindow window = EditorWindow.GetWindow(typeof(QHierarchySettingsWindow));           
            window.minSize = new Vector2(375, 400);

            #if UNITY_4_6 || UNITY_4_7 || UNITY_5_0
                window.title = "QHierarchy";
            #else
                window.titleContent = new GUIContent("QHierarchy");
            #endif
		}

        // PRIVATE
        private bool inited = false;
        private Rect lastRect;
        private bool isProSkin;
        private int indentLevel;
        private Texture2D checkBoxChecked;
        private Texture2D checkBoxUnchecked;
        private Texture2D orderUp;
        private Texture2D orderDown;
        private Vector2 scrollPosition = new Vector2();
        private Color separatorColor;
        private Color yellowColor;
        private float totalWidth;

        // INIT
        private void init() 
        { 
            inited            = true;
            isProSkin         = EditorGUIUtility.isProSkin;
            separatorColor    = isProSkin ? new Color(0.18f, 0.18f, 0.18f) : new Color(0.59f, 0.59f, 0.59f);
            yellowColor       = isProSkin ? new Color(1.00f, 0.90f, 0.40f) : new Color(0.31f, 0.31f, 0.31f);
            checkBoxChecked   = QResources.getInstance().getTexture(QTexture.QCheckBoxChecked);
            checkBoxUnchecked = QResources.getInstance().getTexture(QTexture.QCheckBoxUnchecked);
            orderUp           = QResources.getInstance().getTexture(QTexture.QArrowUpButton);
            orderDown         = QResources.getInstance().getTexture(QTexture.QArrowDownButton);
        }
         
        // GUI
		void OnGUI()
		{
            if (!inited || isProSkin != EditorGUIUtility.isProSkin)  
                init();

            indentLevel = 8;
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            {
                Rect targetRect = EditorGUILayout.GetControlRect(GUILayout.Height(lastRect.y + lastRect.height));
                if (Event.current.type == EventType.Repaint) totalWidth = targetRect.width + 8;

                this.lastRect = new Rect(0, 1, 0, 0);

                // COMPONENTS
                drawSection("COMPONENTS SETTINGS");
                float sectionStartY = lastRect.y + lastRect.height;
                drawSpace(5);  

                drawTreeMapComponentSettings();
                drawSeparator();
                drawMonoBehaviourIconComponentSettings();
                drawSeparator();
                drawSeparatorComponentSettings();
                drawSeparator();
                drawVisibilityComponentSettings();
                drawSeparator();
                drawLockComponentSettings();
                drawSeparator();
                drawStaticComponentSettings();
                drawSeparator();
                drawErrorComponentSettings();
                drawSeparator();
                drawRendererComponentSettings();
                drawSeparator();
                drawPrefabComponentSettings();
                drawSeparator();
                drawTagLayerComponentSettings();
                drawSeparator();
                drawColorComponentSettings();
                drawSeparator();
                drawGameObjectIconComponentSettings();
                drawSeparator();
                drawTagIconComponentSettings();
                drawSeparator();
                drawChildrenCountComponentSettings();
                drawSeparator();
                drawComponentsComponentSettings();

                drawSpace(3);  
                drawLeftLine(sectionStartY, lastRect.y + lastRect.height, separatorColor);

                // ORDER
                drawSection("ORDER OF COMPONENTS");         
                sectionStartY = lastRect.y + lastRect.height;
                drawSpace(8);  
                drawOrderSettings();
                drawSpace(6);      
                drawLeftLine(sectionStartY, lastRect.y + lastRect.height, separatorColor);

                // ADDITIONAL
                drawSection("ADDITIONAL SETTINGS");             
                sectionStartY = lastRect.y + lastRect.height;
                drawSpace(3);  

                drawAdditionalSettings();

                drawLeftLine(sectionStartY, lastRect.y + lastRect.height + 4, separatorColor);

                indentLevel -= 1;
            }

            EditorGUILayout.EndScrollView();
        }

        // ORDER
        private void drawOrderSettings()
        {
            indentLevel += 8;

            string componentOrder = QSettings.getInstance().get<string>(QSetting.ComponentOrder);
            string[] componentIds = componentOrder.Split(';');

            for (int i = 0; i < componentIds.Length; i++)
            {
                QHierarchyComponentEnum type = (QHierarchyComponentEnum)int.Parse(componentIds[i]);

                Rect rect = getControlRect(14, 17);

                if (i > 0)
                {
                    if (GUI.Button(rect, orderUp, GUIStyle.none))
                    {
                        string newIconOrder = "";
                        for (int j = 0; j < componentIds.Length; j++)
                        {
                            if (j == i - 1) newIconOrder += componentIds[i] + ";";
                            else if (j == i) newIconOrder += componentIds[i-1] + ";";
                            else newIconOrder += componentIds[j] + ";";
                        }
                        newIconOrder = newIconOrder.TrimEnd(';');
                        QSettings.getInstance().set(QSetting.ComponentOrder, newIconOrder);
                    }
                }
                
                rect.x += 17; 
                
                if (i < componentIds.Length - 1)
                {
                    if (GUI.Button(rect, orderDown, GUIStyle.none))
                    {
                        string newIconOrder = "";
                        for (int j = 0; j < componentIds.Length; j++)
                        {
                            if (j == i) newIconOrder += componentIds[i+1] + ";";
                            else if (j == i + 1) newIconOrder += componentIds[i] + ";";
                            else newIconOrder += componentIds[j] + ";";
                        }
                        newIconOrder = newIconOrder.TrimEnd(';');
                        QSettings.getInstance().set(QSetting.ComponentOrder, newIconOrder);
                    }
                }
                
                rect.x += 19;
                rect.y -= 1;
                rect.width = 200; 
                rect.height = 22;
                GUI.Label(rect, getTextWithSpaces(type.ToString()));
            }

            indentLevel -= 8;
        }             
        
        // COMPONENTS
        private void drawTreeMapComponentSettings() 
        {
            drawComponentCheckBox(QSetting.ShowTreeMapComponent, "Hierarchy Tree");
        }
        
        private void drawMonoBehaviourIconComponentSettings() 
        {
            if (drawComponentCheckBox(QSetting.ShowMonoBehaviourIconComponent, "MonoBehaviour Icon"))
            {
                Rect rect = getControlRect(0, 18);
                drawBackground(rect.x, rect.y, rect.width, 18 * 2 + 5);
                rect.x += 31;
                rect.width -= 31 + 6;

                drawCheckBoxRight(rect, QSetting.ShowMonoBehaviourIconComponentDuringPlayMode, "Show Component During Play Mode");
                drawCheckBoxRight(getControlRect(0, 18, 31, 6), QSetting.IgnoreUnityMonobehaviour, "Ignore Unity MonoBehaviour");
                drawSpace(5);
            }
        }

        private void drawBackground(float x, float y, float width, float height)
        {
            EditorGUI.DrawRect(new Rect(x, y, width, height), separatorColor);
        }

        private void drawSeparatorComponentSettings() 
        {
            if (drawComponentCheckBox(QSetting.ShowSeparatorComponent, "Separator"))
            {
                Rect rect = getControlRect(0, 18);
                drawBackground(rect.x, rect.y, rect.width, 18 + 5);
                rect.x += 31;
                rect.width -= 31 + 6;

                drawCheckBoxRight(rect, QSetting.ShowRowShading, "Row Shading");
                drawSpace(5);
            }
        }

        private void drawVisibilityComponentSettings() 
        {
            if (drawComponentCheckBox(QSetting.ShowVisibilityComponent, "Visibility"))
            {
                Rect rect = getControlRect(0, 18);
                drawBackground(rect.x, rect.y, rect.width, 18 + 5);
                rect.x += 31;
                rect.width -= 31 + 6;

                drawCheckBoxRight(rect, QSetting.ShowVisibilityComponentDuringPlayMode, "Show Component During Play Mode");
                drawSpace(5);
            }
        }

        private void drawLockComponentSettings() 
        {
            if (drawComponentCheckBox(QSetting.ShowLockComponent, "Lock"))
            {   
                Rect rect = getControlRect(0, 18);
                drawBackground(rect.x, rect.y, rect.width, 18 * 2 + 5);
                rect.x += 31;
                rect.width -= 31 + 6;

                drawCheckBoxRight(rect, QSetting.ShowLockComponentDuringPlayMode, "Show Component During Play Mode");
                drawCheckBoxRight(getControlRect(0, 18, 31, 6), QSetting.PreventSelectionOfLockedObjects, "Prevent Selection Of Locked Objects");
                drawSpace(5);
            }
        }

        private void drawStaticComponentSettings() 
        {
            if (drawComponentCheckBox(QSetting.ShowStaticComponent, "Static"))
            {
                Rect rect = getControlRect(0, 18);
                drawBackground(rect.x, rect.y, rect.width, 18 + 5);
                rect.x += 31;
                rect.width -= 31 + 6;

                drawCheckBoxRight(rect, QSetting.ShowStaticComponentDuringPlayMode, "Show Component During Play Mode");
                drawSpace(5);
            }        
        }

        private void drawErrorComponentSettings() 
        {
            if (drawComponentCheckBox(QSetting.ShowErrorComponent, "Error"))
            {
                Rect rect = getControlRect(0, 18);
                drawBackground(rect.x, rect.y, rect.width, 18 * 9 + 7);
                rect.x += 31;
                rect.width -= 31 + 6;

                drawCheckBoxRight(rect, QSetting.ShowErrorComponentDuringPlayMode, "Show Component During Play Mode");
                drawCheckBoxRight(getControlRect(0, 18, 31, 6), QSetting.ShowErrorIconParent, "Show Error Icon Up Of Hierarchy");
                drawCheckBoxRight(getControlRect(0, 18, 31, 6), QSetting.ShowErrorForDisabledComponents , "Show Error Icon For Disabled Components");
                drawCheckBoxRight(getControlRect(0, 18, 31, 6), QSetting.ShowErrorIconScriptIsMissing   , "Show Error Icon When Script Is Missing");
                drawCheckBoxRight(getControlRect(0, 18, 31, 6), QSetting.ShowErrorIconReferenceIsNull   , "Show Error Icon When Reference Is Null");
                drawCheckBoxRight(getControlRect(0, 18, 31, 6), QSetting.ShowErrorIconStringIsEmpty     , "Show Error Icon When String Is Empty");
                drawCheckBoxRight(getControlRect(0, 18, 31, 6), QSetting.ShowErrorIconMissingEventMethod, "Show Error Icon When Callback Of Event Is Missing");
                drawCheckBoxRight(getControlRect(0, 18, 31, 6), QSetting.ShowErrorIconWhenTagOrLayerIsUndefined, "Show Error Icon When Tag Or Layer Is Undefined");

                drawSpace(4);
                string ignore = QSettings.getInstance().get<string>(QSetting.IgnoreErrorOfMonoBehaviours);
                string newIgnore = EditorGUI.TextField(getControlRect(0, 16, 31, 6), "Ignore Classes Pattern", ignore);
                if (!ignore.Equals(newIgnore)) QSettings.getInstance().set(QSetting.IgnoreErrorOfMonoBehaviours, newIgnore);

                drawSpace(5);
            }
        }

        private void drawRendererComponentSettings() 
        {
            if (drawComponentCheckBox(QSetting.ShowRendererComponent, "Renderer"))
            {
                Rect rect = getControlRect(0, 18);
                drawBackground(rect.x, rect.y, rect.width, 18 + 5);
                rect.x += 31;
                rect.width -= 31 + 6;

                drawCheckBoxRight(rect, QSetting.ShowRendererComponentDuringPlayMode, "Show Component During Play Mode");
                drawSpace(5);
            }
        }

        private void drawPrefabComponentSettings() 
        {
            if (drawComponentCheckBox(QSetting.ShowPrefabComponent, "Prefab"))
            {
                Rect rect = getControlRect(0, 18);
                drawBackground(rect.x, rect.y, rect.width, 18 + 5);
                rect.x += 31;
                rect.width -= 31 + 6;

                drawCheckBoxRight(rect, QSetting.ShowBreakedPrefabsOnly, "Show Icon For Broken Prefabs Only");
                drawSpace(5);
            }
        }

        private void drawTagLayerComponentSettings()
        {
            if (drawComponentCheckBox(QSetting.ShowTagLayerComponent, "Tag And Layer"))
            {
                Rect rect = getControlRect(0, 16);
                drawBackground(rect.x, rect.y, rect.width, 18 * 6 + 5);
                rect.x += 31;
                rect.width -= 31 + 6;

                drawCheckBoxRight(rect, QSetting.ShowTagLayerComponentDuringPlayMode, "Show Component During Play Mode");
                drawSpace(5);

                // SHOW
                QHierarchyTagAndLayerType tagAndLayerType = (QHierarchyTagAndLayerType)QSettings.getInstance().get<int>(QSetting.TagAndLayerType);
                QHierarchyTagAndLayerType newTagAndLayerType;
                if ((newTagAndLayerType = (QHierarchyTagAndLayerType)EditorGUI.EnumPopup(getControlRect(0, 16, 31, 6), "Show Tag And Layer", tagAndLayerType)) != tagAndLayerType)                
                    QSettings.getInstance().set(QSetting.TagAndLayerType, (int)newTagAndLayerType);                  
                drawSpace(2);

                // LABEL SIZE
                QHierarchyTagAndLayerLabelSize tagAndLayerLabelSize = (QHierarchyTagAndLayerLabelSize)QSettings.getInstance().get<int>(QSetting.TagAndLayerLabelSize);
                QHierarchyTagAndLayerLabelSize newTagAndLayerLabelSize;
                if ((newTagAndLayerLabelSize = (QHierarchyTagAndLayerLabelSize)EditorGUI.EnumPopup(getControlRect(0, 16, 31, 6), "Label Size", tagAndLayerLabelSize)) != tagAndLayerLabelSize)                
                    QSettings.getInstance().set(QSetting.TagAndLayerLabelSize, (int)newTagAndLayerLabelSize);  
                drawSpace(2);

                // SIZE TYPE
                QHierarchyTagAndLayerSizeType tagAndLayerSizeValueType = (QHierarchyTagAndLayerSizeType)QSettings.getInstance().get<int>(QSetting.TagAndLayerSizeValueType);
                QHierarchyTagAndLayerSizeType newTagAndLayerSizeValueType;
                if ((newTagAndLayerSizeValueType = (QHierarchyTagAndLayerSizeType)EditorGUI.EnumPopup(getControlRect(0, 16, 31, 6), "Unit Of Width", tagAndLayerSizeValueType)) != tagAndLayerSizeValueType)                
                    QSettings.getInstance().set(QSetting.TagAndLayerSizeValueType, (int)newTagAndLayerSizeValueType);  
                drawSpace(2);

                // SIZE
                if (newTagAndLayerSizeValueType == QHierarchyTagAndLayerSizeType.Pixel)
                {
                    int tagAndLayerSizeValue = QSettings.getInstance().get<int>(QSetting.TagAndLayerSizeValuePixel);
                    int newLayerSizeValue = EditorGUI.IntSlider(getControlRect(0, 16, 31, 6), "Width In Pixels", tagAndLayerSizeValue, 1, 250);
                    if (newLayerSizeValue != tagAndLayerSizeValue)                    
                        QSettings.getInstance().set(QSetting.TagAndLayerSizeValuePixel, newLayerSizeValue);   
                }
                else
                {
                    float tagAndLayerSizeValuePercent = QSettings.getInstance().get<float>(QSetting.TagAndLayerSizeValuePercent);
                    float newtagAndLayerSizeValuePercent = EditorGUI.Slider(getControlRect(0, 16, 31, 6), "Percentage Width", tagAndLayerSizeValuePercent, 0, 0.5f);
                    if (tagAndLayerSizeValuePercent != newtagAndLayerSizeValuePercent)                    
                        QSettings.getInstance().set(QSetting.TagAndLayerSizeValuePercent, newtagAndLayerSizeValuePercent);   
                }
                drawSpace(2);

                // ALIGNMENT
                QHierarchyTagAndLayerAligment tagAndLayerAligment = (QHierarchyTagAndLayerAligment)QSettings.getInstance().get<int>(QSetting.TagAndLayerAligment);
                QHierarchyTagAndLayerAligment newTagAndLayerAligment;
                if ((newTagAndLayerAligment = (QHierarchyTagAndLayerAligment)EditorGUI.EnumPopup(getControlRect(0, 16, 31, 6), "Alignment", tagAndLayerAligment)) != tagAndLayerAligment)                
                    QSettings.getInstance().set(QSetting.TagAndLayerAligment, (int)newTagAndLayerAligment);  
                drawSpace(3);
            }
        }

        private void drawColorComponentSettings() 
        {
            if (drawComponentCheckBox(QSetting.ShowColorComponent, "Color"))
            {
                Rect rect = getControlRect(0, 18);
                drawBackground(rect.x, rect.y, rect.width, 18 + 5);
                rect.x += 31;
                rect.width -= 31 + 6;

                drawCheckBoxRight(rect, QSetting.ShowColorComponentDuringPlayMode, "Show Component During Play Mode");
                drawSpace(5);
            }
        }

        private void drawGameObjectIconComponentSettings() 
        {
            if (drawComponentCheckBox(QSetting.ShowGameObjectIconComponent, "GameObject Icon"))
            {
                Rect rect = getControlRect(0, 18);
                drawBackground(rect.x, rect.y, rect.width, 18 + 5);
                rect.x += 31;
                rect.width -= 31 + 6;
                
                drawCheckBoxRight(rect, QSetting.ShowGameObjectIconComponentDuringPlayMode, "Show Component During Play Mode");
                drawSpace(5);
            }
        }

        private void drawTagIconComponentSettings() 
        {
            if (drawComponentCheckBox(QSetting.ShowTagIconComponent, "Tag Icon"))
            {     
                bool changed = false;
                List<QTagTexture> tagTextureList = QSettings.getInstance().get<List<QTagTexture>>(QSetting.CustomTagIcon);
                string[] tags = UnityEditorInternal.InternalEditorUtility.tags;

                Rect rect = getControlRect(0, 18);
                drawBackground(rect.x, rect.y, rect.width, 18 + 18 * tags.Length + 5);
                rect.x += 31;
                rect.width -= 31 + 6;      

                drawCheckBoxRight(rect, QSetting.ShowTagIconComponentDuringPlayMode, "Show Component During Play Mode");

                drawSpace(4);

                for (int i = 0; i < tags.Length; i++) 
                {
                    string tag = tags[i];
                    QTagTexture tagTexture = tagTextureList.Find(t => t.tag == tag);
                    Texture2D newTexture = (Texture2D)EditorGUI.ObjectField(getControlRect(0, 16, 31, 6), 
                                                                            tag, tagTexture == null ? null : tagTexture.texture, typeof(Texture2D), false);
                    if (newTexture != null && tagTexture == null)
                    {
                        QTagTexture newTagTexture = new QTagTexture(tag, newTexture);
                        tagTextureList.Add(newTagTexture);
                        
                        changed = true;
                    }
                    else if (newTexture == null && tagTexture != null)
                    {
                        tagTextureList.Remove(tagTexture);
                        
                        changed = true;
                    }
                    else if (tagTexture != null && tagTexture.texture != newTexture)
                    {
                        tagTexture.texture = newTexture;
                        changed = true;
                    }

                    drawSpace(i == tags.Length - 1 ? 2 : 2);
                }
                
                if (changed) 
                {     
                    QSettings.getInstance().set(QSetting.CustomTagIcon, tagTextureList);
                    EditorApplication.RepaintHierarchyWindow();
                }
            }
        }

        private void drawChildrenCountComponentSettings() 
        {
            if (drawComponentCheckBox(QSetting.ShowChildrenCountComponent, "Children Count"))
            {
                Rect rect = getControlRect(0, 18);
                drawBackground(rect.x, rect.y, rect.width, 18 + 5);
                rect.x += 31;
                rect.width -= 31 + 6;
                
                drawCheckBoxRight(rect, QSetting.ShowChildrenCountComponentDuringPlayMode, "Show Component During Play Mode");
                drawSpace(5);
            }
        }

        private void drawComponentsComponentSettings() 
        {
            if (drawComponentCheckBox(QSetting.ShowComponentsComponent, "Components"))
            {
                Rect rect = getControlRect(0, 18);
                drawBackground(rect.x, rect.y, rect.width, 18 + 5);
                rect.x += 31;
                rect.width -= 31 + 6;
                
                drawCheckBoxRight(rect, QSetting.ShowComponentsComponentDuringPlayMode, "Show Component During Play Mode");
                drawSpace(5);
            }
        }

        // ADDITIONAL SETTINGS
        private void drawAdditionalSettings()
        {
            drawCheckBoxRight(getControlRect(0, 18, 5, 8), QSetting.ShowHiddenQHierarchyObjectList, "Show QHierarchyObjectList GameObject");
            drawCheckBoxRight(getControlRect(0, 18, 5, 8), QSetting.HideIconsIfNotFit, "Hide Icons If Not Fit");
            drawSpace(6);

            int identation = QSettings.getInstance().get<int>(QSetting.Identation); 
            int newIdentation = EditorGUI.IntSlider(getControlRect(0, 16, 5, 8), "Right Indent", identation, 0, 500);
            if (newIdentation != identation) QSettings.getInstance().set(QSetting.Identation, newIdentation);       
            drawCheckBoxRight(getControlRect(0, 18, 5, 8), QSetting.ShowModifierWarning, "Show Warning When Using Modifiers + Click");

            drawSpace(6);
        }

        // PRIVATE
        private void drawSection(string title)
        {
            Rect rect = getControlRect(0, 24, -3);
            rect.width *= 2;
            rect.x = 0;
            GUI.Box(rect, "");             
            
            drawLeftLine(rect.y, rect.y + 24, yellowColor);
            
            rect.x = lastRect.x + 8;
            rect.y += 4;
            GUI.Label(rect, title);
        }

        private string getTextWithSpaces(string text)
        {
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ')                
                    newText.Append(' ');                
                newText.Append(text[i]);                
            }
            newText.Replace(" Component", "");
            return newText.ToString();
        }

        private void drawSeparator(int spaceBefore = 0, int spaceAfter = 0, int height = 1)
        {
            if (spaceBefore > 0) drawSpace(spaceBefore);
            Rect rect = getControlRect(0, height);
            rect.width += 8;
            EditorGUI.DrawRect(rect, separatorColor);
            if (spaceAfter > 0) drawSpace(spaceAfter);
        }

        private bool drawComponentCheckBox(QSetting setting, string label)
        {
            indentLevel += 8;

            Rect rect = getControlRect(0, 28);

            float rectWidth = rect.width;
            bool isChecked = QSettings.getInstance().get<bool>(setting);

            rect.x -= 1;
            rect.y += 7;
            rect.width  = 14;
            rect.height = 14;

            if (GUI.Button(rect, isChecked ? checkBoxChecked : checkBoxUnchecked, GUIStyle.none))
            {
                QSettings.getInstance().set(setting, !isChecked);
            }

            rect.x += 14 + 10;
            rect.width = rectWidth - 14 - 8;
            rect.y -= 1;
            rect.height = 20;

            GUI.Label(rect, label);

            indentLevel -= 8;

            return isChecked;
        }

        private bool drawCheckBoxRight(Rect rect, QSetting setting, string label)
        {
            bool isChecked = QSettings.getInstance().get<bool>(setting);

            float tempX = rect.x;
            rect.x += rect.width - 14;
            rect.y += 5;
            rect.width  = 14;
            rect.height = 14;
            
            if (GUI.Button(rect, isChecked ? checkBoxChecked : checkBoxUnchecked, GUIStyle.none))
            {
                QSettings.getInstance().set(setting, !isChecked);
            }

            rect.width = rect.x - tempX - 4;
            rect.x = tempX;
            rect.y -= 1;
            rect.height = 20;
            
            GUI.Label(rect, label);
            
            return isChecked;
        }

        private void drawSpace(int value)
        {
            getControlRect(0, value);
        }
        
        private void drawLeftLine(float fromY, float toY, Color color, float width = 0)
        {
            EditorGUI.DrawRect(new Rect(0, fromY, width == 0 ? indentLevel : width, toY - fromY), color);
        }
        
        private Rect getControlRect(float width, float height, float addIndent = 0, float remWidth = 0)
        { 
            Rect rect = new Rect(indentLevel + addIndent, lastRect.y + lastRect.height, (width == 0 ? totalWidth - indentLevel - addIndent - remWidth: width), height);
            lastRect = rect;
            return rect;
        }
	}
}