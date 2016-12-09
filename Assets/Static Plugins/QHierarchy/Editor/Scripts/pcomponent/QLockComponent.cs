using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using qtools.qhierarchy.pcomponent.pbase;
using qtools.qhierarchy.phierarchy;
using qtools.qhierarchy.phelper;
using qtools.qhierarchy.pdata;

namespace qtools.qhierarchy.pcomponent
{
    public class QLockComponent: QBaseComponent
    {
        // PRIVATE
        private Texture2D buttonLockOn;
        private Texture2D buttonLockOff;
        private bool showModifierWarning;
        private int targetLockState = -1;

        // CONSTRUCTOR
        public QLockComponent()
        {
            buttonLockOn = QResources.getInstance().getTexture(QTexture.QLockOnButton);
            buttonLockOff = QResources.getInstance().getTexture(QTexture.QLockOffButton);

            QSettings.getInstance().addEventListener(QSetting.ShowModifierWarning            , settingsChanged);
            QSettings.getInstance().addEventListener(QSetting.ShowLockComponent              , settingsChanged);
            QSettings.getInstance().addEventListener(QSetting.ShowLockComponentDuringPlayMode, settingsChanged);
            settingsChanged();
        }

        // PRIVATE
        private void settingsChanged()
        {
            showModifierWarning = QSettings.getInstance().get<bool>(QSetting.ShowModifierWarning);
            enabled = QSettings.getInstance().get<bool>(QSetting.ShowLockComponent);
            showComponentDuringPlayMode = QSettings.getInstance().get<bool>(QSetting.ShowLockComponentDuringPlayMode);
        }

        // DRAW
        public override void layout(GameObject gameObject, QObjectList objectList, ref Rect rect)
        {
            rect.x -= 13;
            rect.width = 13;
        }

        public override void draw(GameObject gameObject, QObjectList objectList, Rect selectionRect, Rect curRect)
        {  
            bool isLock = isGameObjectLock(gameObject, objectList);

            if (isLock == true && (gameObject.hideFlags & HideFlags.NotEditable) != HideFlags.NotEditable)
            {
                gameObject.hideFlags |= HideFlags.NotEditable;
                EditorUtility.SetDirty(gameObject);
            }
            else if (isLock == false && (gameObject.hideFlags & HideFlags.NotEditable) == HideFlags.NotEditable)
            {
                gameObject.hideFlags ^= HideFlags.NotEditable;
                EditorUtility.SetDirty(gameObject);
            }
            
            GUI.DrawTexture(curRect, isLock ? buttonLockOn : buttonLockOff);
        }

        public override void eventHandler(GameObject gameObject, QObjectList objectList, Event currentEvent, Rect curRect)
        {
            if (currentEvent.isMouse && currentEvent.button == 0 && curRect.Contains(currentEvent.mousePosition))
            {
                bool isLock = isGameObjectLock(gameObject, objectList);

                if (currentEvent.type == EventType.MouseDown)
                {
                    targetLockState = ((!isLock) == true ? 1 : 0);
                }
                else if (currentEvent.type == EventType.MouseDrag && targetLockState != -1)
                {
                    if (targetLockState == (isLock == true ? 1 : 0)) return;
                } 
                else
                {
                    targetLockState = -1;
                    return;
                }

                List<GameObject> targetGameObjects = new List<GameObject>();
                if (currentEvent.shift) 
                {
                    if (!showModifierWarning || EditorUtility.DisplayDialog("Change locking", "Are you sure you want to " + (isLock ? "unlock" : "lock") + " this GameObject and all its children? (You can disable this warning in the settings)", "Yes", "Cancel"))
                    {
                        getGameObjectListRecursive(gameObject, ref targetGameObjects);           
                    }
                }
                else if (currentEvent.alt)
                {
                    if (gameObject.transform.parent != null)
                    {
                        if (!showModifierWarning || EditorUtility.DisplayDialog("Change locking", "Are you sure you want to " + (isLock ? "unlock" : "lock") + " this GameObject and its siblings? (You can disable this warning in the settings)", "Yes", "Cancel"))
                        {
                            getGameObjectListRecursive(gameObject.transform.parent.gameObject, ref targetGameObjects, 1);
                            targetGameObjects.Remove(gameObject.transform.parent.gameObject);
                        }
                    }
                    else
                    {
                        Debug.Log("This action for root objects is supported only for Unity3d 5.3.3 and above");
                        return;
                    }
                }
                else 
                {
                    if (Selection.Contains(gameObject))
                    {
                        targetGameObjects.AddRange(Selection.gameObjects);
                    }
                    else
                    {
                        getGameObjectListRecursive(gameObject, ref targetGameObjects, 0);
                    };
                }
                
                setLock(targetGameObjects, objectList, !isLock);
                currentEvent.Use();
            }
        } 

        public override void disabledHandler(GameObject gameObject, QObjectList objectList)
        {	
            if (objectList != null && objectList.lockedObjects.Contains(gameObject))
            {
                objectList.lockedObjects.Remove(gameObject);
                gameObject.hideFlags &= ~HideFlags.NotEditable;
                EditorUtility.SetDirty(gameObject);
            }
        }

        // PRIVATE
        private bool isGameObjectLock(GameObject gameObject, QObjectList objectList)
        {
            return objectList == null ? false : objectList.lockedObjects.Contains(gameObject);
        }
        
        private void setLock(List<GameObject> gameObjects, QObjectList objectList, bool targetLock)
        {
            if (gameObjects.Count == 0) return;

            if (objectList == null) objectList = QObjectListManager.getInstance().getObjectList(gameObjects[0], true);
            Undo.RecordObject(objectList, targetLock ? "Lock" : "Unlock");   
            
            for (int i = gameObjects.Count - 1; i >= 0; i--)
            {     
                GameObject curGameObject = gameObjects[i];
                Undo.RecordObject(curGameObject, targetLock ? "Lock" : "Unlock");
                
                if (targetLock)
                {
                    curGameObject.hideFlags |= HideFlags.NotEditable;
                    if (!objectList.lockedObjects.Contains(curGameObject))
                        objectList.lockedObjects.Add(curGameObject);
                }
                else
                {
                    curGameObject.hideFlags &= ~HideFlags.NotEditable;
                    objectList.lockedObjects.Remove(curGameObject);
                }
                
                EditorUtility.SetDirty(curGameObject);
            }
        }
    }
}

