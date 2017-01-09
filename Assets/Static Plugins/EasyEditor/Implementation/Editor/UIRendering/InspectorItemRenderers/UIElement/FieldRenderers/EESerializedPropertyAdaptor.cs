//
// Copyright (c) 2016 Easy Editor 
// All Rights Reserved 
//  
//

using UnityEngine;
using UnityEditor;
using UEObject = UnityEngine.Object;
using System.Collections;
using EasyEditor.ReorderableList;
using System;

namespace EasyEditor
{
    public class EESerializedPropertyAdaptor : SerializedPropertyAdaptor, IReorderableListDropTarget
    {
        public Action<int, SerializedProperty> OnItemSelected;

        private bool isReadOnly;

        public EESerializedPropertyAdaptor(SerializedProperty arrayProperty, bool isReadOnly) : base(arrayProperty)
        {
            this.isReadOnly = isReadOnly;
        }

        private Rect DropTargetPosition 
        {
            get 
            {
                // Expand size of drop target slightly so that it is easier to drop.
                Rect dropPosition = ReorderableListGUI.CurrentListPosition;
                dropPosition.y -= 10;
                dropPosition.height += 15;
                return dropPosition;
            }
        }

        public bool CanDropInsert(int insertionIndex)
        {
            bool canDrop = true;

            canDrop &= !isReadOnly;

            if(DropTargetPosition.Contains(Event.current.mousePosition))
            {
                canDrop &= true;
            }
            else
            {
                canDrop = false;
            }

            return canDrop;
        }

        public void ProcessDropInsertion(int insertionIndex)
        {
            if (Event.current.type == EventType.DragPerform) 
            {
                for(int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                {
                    _arrayProperty.InsertArrayElementAtIndex(insertionIndex + i);
                    _arrayProperty.GetArrayElementAtIndex(insertionIndex + i).objectReferenceValue = DragAndDrop.objectReferences[i];
                }
            }
        }

        public override void DrawItem(Rect position, int index)
        {
            base.DrawItem(position, index);

            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            if(Event.current.GetTypeForControl(controlID) == EventType.MouseDown) 
            {
                //Drag handle are more on the left of the drawn item.
                position.x -= 22; 
                if (Event.current.button == 0 && position.Contains(Event.current.mousePosition)) 
                {
                    if(OnItemSelected != null)
                    {
                        OnItemSelected(index, _arrayProperty);
                    }
                }
            }
        }
    }
}