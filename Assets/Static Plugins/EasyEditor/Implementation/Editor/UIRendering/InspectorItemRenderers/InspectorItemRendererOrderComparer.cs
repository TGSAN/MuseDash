using System;
using System.Collections;
using System.Collections.Generic;

namespace EasyEditor
{
	/// <summary>
	/// List element comparer to order <c>InspectorItemRenderer</c> in the inspector based on their group and order.
	/// </summary>
	public class InspectorItemRendererOrderComparer : IComparer<InspectorItemRenderer>
	{
		Groups orderedGroupList;
		InspectorItemRenderer[] rendererArray;
		
		public InspectorItemRendererOrderComparer(Groups groups, List<InspectorItemRenderer> list)
		{
			orderedGroupList = groups;
			
			rendererArray = new InspectorItemRenderer[list.Count];
			list.CopyTo(rendererArray);
		}
		
		public int Compare(InspectorItemRenderer a, InspectorItemRenderer b)
		{
			int aIndex = orderedGroupList.GetGroupIndex(a.inspectorAttribute.group);
			int bIndex = orderedGroupList.GetGroupIndex(b.inspectorAttribute.group);
			
			int result = 1;
			
            if (aIndex < bIndex)
            {
                result = -1;
            }
            if (aIndex == bIndex)
            {
                if(a.inspectorAttribute.order < b.inspectorAttribute.order)
                {
                    result = -1;
                }
                else if (a.inspectorAttribute.order == b.inspectorAttribute.order && Array.IndexOf(rendererArray,a) < Array.IndexOf(rendererArray,b))
                {
                    result = -1;
                }
            }
			
			return result;
		}

        bool DoesNotBelongToGroups(string groupName)
        {
            return string.IsNullOrEmpty(groupName) || (orderedGroupList.GetGroupIndex(groupName) == -1);
        }

        bool BelongToGroups(string groupName)
        {
            return !string.IsNullOrEmpty(groupName) && !(orderedGroupList.GetGroupIndex(groupName) == -1);
        }
	}
}