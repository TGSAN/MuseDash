using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Tools.Commons.Editor
{
    [Serializable]
    public class EditorNode
    {
        public int idx;
        public int wndIdx;
        public Rect rect;

        public List<CurveNode> parentCNodes = new List<CurveNode>();
        public List<CurveNode> childCNodes = new List<CurveNode>();

        public virtual void AddChild(EditorNode childNode, List<EditorNode> allEditorNodes, params object[] args)
        {
            childCNodes.Add(new CurveNode(childNode.idx, idx, childCNodes.Count));
        }

        public virtual void RemoveChild(EditorNode childNode, List<EditorNode> allEditorNodes = null, params object[] args)
        {
            childCNodes.RemoveAll(n => n.childIdx == childNode.idx);
            for (var i = 0; i < childCNodes.Count; i++)
            {
                var childIdx = childCNodes[i].childIdx;
                if (allEditorNodes != null)
                {
                    var childEditorNode = allEditorNodes.Find(n => n.idx == childIdx);
                    childEditorNode.parentCNodes.ForEach(p => p.pos = i);
                    childCNodes[i].pos = i;
                }
            }
        }

        public virtual void AddParent(EditorNode parentNode, List<EditorNode> allEditorNodes = null, params object[] args)
        {
            var parentCNode = new CurveNode(idx, parentNode.idx, parentNode.childCNodes.Count);
            if (!parentCNodes.Exists(n => n.childIdx == parentCNode.childIdx && n.parentIdx == parentCNode.parentIdx))
            {
                parentCNodes.Add(parentCNode);
            }
        }

        public virtual void RemoveParent(EditorNode parentNode, List<EditorNode> allEditorNodes = null, params object[] args)
        {
            parentCNodes.RemoveAll(n => n.parentIdx == parentNode.idx);
        }

        public virtual void SetParent(EditorNode parentNode, List<EditorNode> allEditorNodes)
        {
            if (parentCNodes.Count > 0)
            {
                var curParentNode = allEditorNodes.Find(n => n.idx == parentCNodes[0].parentIdx);
                RemoveParent(curParentNode, allEditorNodes);
            }

            if (parentNode != null)
            {
                AddParent(parentNode);
            }
        }

        public virtual void Destroy(List<EditorNode> allEditorNodes)
        {
            //删除父级关系
            this.parentCNodes.ForEach(c =>
            {
                var parentNode = allEditorNodes.Find(n => n.idx == c.parentIdx);
                parentNode.RemoveChild(this, allEditorNodes);
            });
            SetParent(null, allEditorNodes);

            //删除子级关系
            var allEChildren = allEditorNodes.Where(n => childCNodes.Exists(c => c.childIdx == n.idx)).ToList();
            allEChildren.ForEach(n =>
            {
                RemoveChild(n, allEChildren);
                n.RemoveParent(this, allEditorNodes);
            });

            //从列表中删除
            allEditorNodes.RemoveAll(n => n.idx == idx);
        }

        public List<int> AllParentIdx(List<EditorNode> allEditorNodes)
        {
            var list = new List<int>();
            Action<EditorNode> callFunc = null;
            callFunc = parentEditorNode =>
            {
                var curveNodes = parentEditorNode.parentCNodes;
                if (curveNodes == null) return;
                curveNodes.ForEach(n =>
                {
                    var parentIdx = n.parentIdx;
                    parentEditorNode = allEditorNodes.Find(p => p.idx == parentIdx);
                    if (!list.Contains(parentIdx))
                    {
                        list.Add(parentIdx);
                        callFunc(parentEditorNode);
                    }
                });
            };
            callFunc(this);
            return list;
        }

        public Rect[] GetChildPointRects(Rect nodeChildPointRect, bool isLocal = false, float xGap = 12, float yGap = 0)
        {
            var count = childCNodes.Count + 1;
            var rects = new Rect[count];
            for (var i = 0; i < rects.Length; i++)
            {
                var x = nodeChildPointRect.x + i * xGap;
                var y = nodeChildPointRect.y + i * yGap;
                var width = nodeChildPointRect.width;
                var height = nodeChildPointRect.height;

                if (!isLocal)
                {
                    x += rect.x;
                    y += rect.y;
                }
                rects[i] = new Rect(x, y, width, height);
            }
            return rects;
        }

        public Rect[] GetParentPointRects(Rect nodeParentPointRect, bool isLocal = false, float xGap = 12, float yGap = 0)
        {
            var count = parentCNodes.Count + 1;
            var rects = new Rect[count];
            for (var i = 0; i < rects.Length; i++)
            {
                var x = nodeParentPointRect.x + i * xGap;
                var y = nodeParentPointRect.y + i * yGap;
                var width = nodeParentPointRect.width;
                var height = nodeParentPointRect.height;

                if (!isLocal)
                {
                    x += rect.x;
                    y += rect.y;
                }
                rects[i] = new Rect(x, y, width, height);
            }
            return rects;
        }

        public Vector2 ParentPointPos(Rect nodeParentPointRect, int pos)
        {
            var theRect = GetParentPointRects(nodeParentPointRect)[pos];
            return new Vector2(theRect.x + theRect.width / 2, theRect.y + theRect.height / 2);
        }

        public Vector2 ChildPointPos(Rect nodeChildPointRect, int pos)
        {
            var theRect = GetChildPointRects(nodeChildPointRect)[pos];
            return new Vector2(theRect.x + theRect.width / 2, theRect.y + theRect.height / 2);
        }

        public Vector2 ChildPointPos(Rect nodeChildPointRect)
        {
            return ChildPointPos(nodeChildPointRect, childCNodes.Count);
        }
    }

    [Serializable]
    public class CurveNode
    {
        public int childIdx;
        public int parentIdx;
        public int pos;

        public CurveNode(int cIdx, int pIdx, int p)
        {
            childIdx = cIdx;
            parentIdx = pIdx;
            pos = p;
        }
    }
}