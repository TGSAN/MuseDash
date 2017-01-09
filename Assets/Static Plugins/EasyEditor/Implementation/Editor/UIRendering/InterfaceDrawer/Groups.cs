//
// Copyright (c) 2016 Easy Editor 
// All Rights Reserved 
//  
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EasyEditor
{
    /// <summary>
    /// Group describes a group rendered in the inspector.
    /// </summary>
    public class Group
    {
        public readonly string name;
        public string description = "";
        public bool displayHeader = true;
        public bool foldable = false;
        public bool foldout = false;
        public readonly int index;
        public bool hidden;

        public Group(string groupName, int groupIndex)
        {
            name = groupName;
            index = groupIndex;
            hidden = false;
        }
    }

    /// <summary>
    /// Groups is an accessor to the groups defined for a Monobehaviour/Scriptable Object 
    /// and can operate different operation on them.
    /// </summary>
	public class Groups {

        List<Group> groups;

        public Group this[int i]
        {
            get {
                if (i < groups.Count)
                {
                    return groups[i];
                }
                else
                {
                    return null;
                }
            }
        }

        public bool Exist(string groupName)
        {
            return FindGroup(groupName) != null;
        }

        /// <summary>
        /// Determines whether the specified group is hidden.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <returns></returns>
        public bool IsHidden(string groupName)
        {
            bool isHidden = false;

            Group group = FindGroup(groupName);
            if (group != null)
            {
                isHidden = group.hidden;
            }
            else
            {
                Debug.LogWarning("The group you specified does not exists");
            }

            return isHidden;
        }

        public void HideGroup(string groupName)
        {
            Group group = FindGroup(groupName);
            if (group != null)
            {
                group.hidden = true;
            }
        }

        public void ShowGroup(string groupName)
        {
            Group group = FindGroup(groupName);
            if (group != null)
            {
                group.hidden = false;
            }
        }

        public void SetGroupDescription(string groupName, string groupDescription)
        {
            Group group = FindGroup(groupName);
            if (group != null)
            {
                group.description = groupDescription;
            }
        }

        public void SetGroupDisplayHeader(string groupName, bool displayHeader)
        {
            Group group = FindGroup(groupName);
            if (group != null)
            {
                group.displayHeader = displayHeader;
            }
        }

        public void SetGroupFoldable(string groupName, bool foldable)
        {
            Group group = FindGroup(groupName);
            if (group != null)
            {
                group.foldable = foldable;
            }
        }

        public int GetGroupIndex(string groupName)
        {
            int result = -1;

            Group group = FindGroup(groupName);
            if(group != null)
            {
                result = group.index;
            }

            return result;
        }

        public Groups(string[] groupList)
        {
            groups = new List<Group>();
            for(int i = 0; i < groupList.Length; i++)
            {
                groups.Add(new Group(groupList[i], i));
            }
        }

        private Group FindGroup(string groupName)
        {
            Group result = null;

            foreach (Group group in groups)
            {
                if (group.name == groupName)
                {
                    result = group;
                    break;
                }
            }

            return result;
        }
	}
}