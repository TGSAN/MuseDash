//
// Copyright (c) 2016 Easy Editor 
// All Rights Reserved 
//  
//

using UnityEngine;
using System;

namespace EasyEditor
{
    /// <summary>
    /// Specify the different UI groups displayed in the inspector.
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Class)]
    public class GroupsAttribute : System.Attribute
    {
        /// <summary>
        /// UI groups displayed in the inspector. The groups order determines the order the groups are displayed.
        /// </summary>
        public string[] groups = new string[]{""};

        public GroupsAttribute(params string[] groupList)
        {
            groups = groupList;
        }
    }
}