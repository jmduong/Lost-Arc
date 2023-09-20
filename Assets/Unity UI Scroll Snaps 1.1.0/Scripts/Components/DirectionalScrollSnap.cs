/*
 * Copyright(c) 2019 Unity UI Scroll Snaps
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ScrollSnaps
{
    [AddComponentMenu("UI/Scroll Snaps/Directional Scroll Snap")]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class DirectionalScrollSnap : MonoBehaviour, IBeginDragHandler, IDragHandler,
            IEndDragHandler
    {
        /// <summary>
        /// An enum describing the layout of the scroll snap. Used to calculate the movement axis.
        /// </summary>
        public enum Layout { Horizontal, Vertical }

        /// <summary>
        /// The layout of the scroll snap. Tells the scroll snap how the items will be
        /// arranged, and how the scroll snap will move.
        /// </summary>
        [SerializeField]
        private Layout m_Layout;
        /// <summary>
        /// A modifier used to change the deceleration of fling animations.
        /// </summary>
        [SerializeField]
        [Range(0.01f, .99f)]
        private float m_Friction = 0.865f;
        /// <summary>
        /// Tells the scroll snap whether items should appear to be in a loop.
        /// </summary>
        [SerializeField]
        private bool m_ShouldLoop;
        /// <summary>
        /// Only used if the scroll snap is loopable. Tells the scroll snap the margin between the
        /// endmost and startmost items after either has looped.
        /// </summary>
        [SerializeField]
        private float m_EndMargin;

        /// <summary>
        /// The item whose rectTransform's pivot is closet to the pivot of the scroll snap.
        /// </summary>
        public Item closestItem { get { return GetClosestItem(); } }
        /// <summary>
        /// Used to iterate through all of the items "owned" by the scroll snap.
        /// </summary>
        public IEnumerable<Item> items { get { return m_Items; } }
        private RectTransform m_RectTransform;
        /// <summary>
        /// The root RectTransform of the scroll snap (the one attached to the same GameObject
        /// as the DirectionalScrollSnap component).
        /// </summary>
        public RectTransform rectTransform
        {
            get
            {
                if (m_RectTransform == null)
                {
                    m_RectTransform = (RectTransform)transform;
                }
                return m_RectTransform;
            }
        }
        private RectTransform m_Content;
        /// <summary>
        /// The content RectTransform of the scroll snap. Is the parent of all of the
        /// Item RectTransforms.
        /// </summary>
        public RectTransform content
        {
            get
            {
                if (m_Content == null)
                {
                    try
                    {
                        m_Content = (RectTransform)transform.GetChild(0);
                    }
                    catch
                    {
                        throw new System.Exception(CONTENT_CHILD_EXCEPTION);
                    }
                }
                return m_Content;
            }
        }

        /// <summary>
        /// A linked list containing all of the items "owned" by the scroll snap. If the
        /// scroll snap is loopable this is always updated so the first item is the startmost item.
        /// </summary>
        private LinkedList<Item> m_Items = new LinkedList<Item>();
        /// <summary>
        /// Used for drag animations. This is the position of the mouse cursor/touch (in
        /// the space of the scroll snap's rectTransform) when the drag begins.
        /// </summary>
        private Vector2 m_CursorInitialPosition;
        /// <summary>
        /// Used for drag animations. This is the position of the content (content's
        /// anchoredPosition) when the drag begins.
        /// </summary>
        private Vector2 m_ContentInitialPosition;
        /// <summary>
        /// Used for animating the scroll snap after a drag has ended. Contains useful functions
        /// for calculating flings.
        /// </summary>
        private Scroller m_Scroller = new Scroller();
        /// <summary>
        /// The estimated current velocity of the scroll snap. The velocity is based on the
        /// movement of the content RectTransform not the movement of the items.
        /// </summary>
        private Vector2 m_Velocity;
        /// <summary>
        /// Used for calculating the velocity as well as movement direction of the scroll snap.
        /// It is the anchoredPosition of the content from the previous frame.
        /// </summary>
        private Vector2 m_PreviousPosition;
        /// <summary>
        /// The cached value of the maximum spacing between items. Returned by GetMaxItemSpacing
        /// when this value is up-to-date. Used for calculating the size of the content if the
        /// scroll snap is loopable.
        /// </summary>
        private float m_CachedMaxItemSpacing;
        /// <summary>
        /// Tells the GetMaxItemSpacing function if it needs to recalculate the maximum spacing.
        /// True if the spacing of the items may have changed (e.g. items added, items removed,
        /// etc) false otherwise.
        /// </summary>
        private bool m_ItemSpacingDirty = true;
        /// <summary>
        /// Used to tell if the velocity should be updated this frame or not. If the scroll has
        /// been updated it means the content has been moved and will give an incorrect velocity
        /// value, so we should skip velocity calculation this frame.
        /// </summary>
        private bool m_UpdatedScrollThisFrame = false;
        /// <summary>
        /// The camera from the latest OnBeginDrag call. Used to UpdateScroll to update
        /// the m_CursorInitialPosition.
        /// </summary>
        private Camera m_LastPressedCamera;

        /// <summary>
        /// The axis the scroll snap moves along. 0 (x) for a horizontal layout, and 1 (y) for
        /// a vertical layout.
        /// </summary>
        private int m_Axis { get { return (int)m_Layout; } }
        /// <summary>
        /// The axis the scroll snap does not move along. 1 (y) for a horizontal layout, and 0 (x)
        /// for a vertical layout.
        /// </summary>
        private int m_InverseAxis { get { return 1 - (int)m_Layout; } }
        /// <summary>
        /// Used by the scroller. The scroller accepts a deceleration rate rather than friction.
        /// The scroller uses the deceleration rate as the percentage of the velocity left after
        /// a second has passed. This value should get smaller as the friction gets larger,
        /// so it is the reciprocal of the friction.
        /// </summary>
        private float m_DecelerationRate { get { return 1 - m_Friction; } }

        private const string CONTENT_ROTATION_EXCEPTION = "Content must not have rotation.";
        private const string CONTENT_CHILD_EXCEPTION = "The Scroll Snap must have a child " +
            "Content GameObject.";
        private const string DOES_NOT_CONTAIN_ITEM_EXCEPTION = "The Scroll Snap does not " +
            "contain the provided {0} item.";
        private const string SCROLL_SNAP_IS_EMPTY_EXCEPTION = "Cannot remove item because the " +
            "Scroll Snap is empty.";

        /// <summary>
        /// Called just after the component is enabled. If it is enabled when the scene loads it is
        /// called before start on the first frame. Used to initialize the Scroll Snap.
        /// </summary>
        private void OnEnable()
        {
            // Disable Layout Groups.
            var layoutGroup = content.GetComponent<LayoutGroup>();
            if (layoutGroup && layoutGroup.enabled)
            {
                layoutGroup.CalculateLayoutInputHorizontal();
                layoutGroup.CalculateLayoutInputVertical();
                layoutGroup.SetLayoutHorizontal();
                layoutGroup.SetLayoutVertical();
                layoutGroup.enabled = false;
            }

            // Setup the Scroll Snap.
            if (m_Items.Count == 0)
            {
                // This means the scroll snap was not programmatically populated, so we need to
                // set up the items.
                CollectItems();
                if (m_Items.Count != 0)
                {
                    SortItems();
                    SetItemMargins();
                    ArrangeItems();
                }
            }
            else
            {
                // This means the scroll snap was programmatically populated, so we don't need
                // to set up the items, just arrange them.
                ArrangeItems();
            }

            m_PreviousPosition = content.anchoredPosition;
        }

        /// <summary>
        /// Called when the component becomes disabled. It is used to reset the component to a
        /// state where it can be re-enabled cleanly.
        /// </summary>
        private void OnDisable()
        {
            m_Scroller.StopAnimation();
            m_Velocity = Vector2.zero;
            m_EndMargin = m_Items.Last.Value.GetMinimumEndMargin(m_Axis);
            m_Items.Clear();
        }

        /// <summary>
        /// Creates a list of Items that are children of the Scroll Snap.
        /// </summary>
        private void CollectItems()
        {
            for (var i = 0; i < content.childCount; i++)
            {
                var itemTransform = (RectTransform)content.GetChild(i);
                m_Items.AddLast(new Item(itemTransform, CalculateSnapPosition(itemTransform)));
            }
        }

        /// <summary>
        /// Sorts the items in order from start to end.
        /// </summary>
        private void SortItems()
        {
            if (m_Layout == Layout.Horizontal)
            {
                m_Items = new LinkedList<Item>(m_Items.OrderByDescending(i => i.snapPosition.x));
            }
            else  // Layout vertical.
            {
                m_Items = new LinkedList<Item>(m_Items.OrderBy(i => i.snapPosition.y));
            }
        }

        /// <summary>
        /// Sets the items' margins based on their current positions.
        /// </summary>
        private void SetItemMargins()
        {
            var startwardItem = m_Items.First;
            var currentItem = startwardItem.Next;
            while (true)
            {
                if (currentItem == null)
                {
                    if (m_ShouldLoop)
                    {
                        startwardItem.Value.SetMinimumEndMargin(m_EndMargin, m_Axis);
                    }
                    break;
                }

                startwardItem.Value.SetMinimumEndMargin(currentItem.Value.rectTransform, m_Axis);
                currentItem.Value.SetMinimumStartMargin(startwardItem.Value.rectTransform, m_Axis);

                currentItem = currentItem.Next;
                startwardItem = startwardItem.Next;  // The old current item.
            }
        }

        /// <summary>
        /// Scales and moves the content and items so that all of the items are snappable.
        /// </summary>
        private void ArrangeItems()
        {
            if (m_Items.Count == 0)
            {
                // No items to arrange.
                return;
            }

            var oldContentSize = content.sizeDelta;
            var startmostItemInitialWorldPosition = m_Items.First.Value.rectTransform.position;

            ResizeContent();
            ResizeItems(oldContentSize);
            ShiftItemsForSnappability();
            ShiftContent(startmostItemInitialWorldPosition);
            UpdateSnapPositions();
}

        /// <summary>
        /// Resizes the content so that it has the full range of motion needed.
        /// </summary>
        // The amountOfScrollSnap is the amount we need to add to the distanceBetweenEndPoints so that
        // the content has full range of motion. If the anchorMin & Max are together (e.g .5, .5) then
        // we need to add the full size of the scrollsnap. If the anchorMin & Max are apart (eg. 0, 0
        // and 1, 1) we need to add the portion of the size that the anchors /don't/ cover (in this
        // case none because they cover the whole thing). The amount covered is equal to the anchorMax
        // minus the anchorMin (times the size), so the amount they /don't/ cover is the reciprocal of
        // that i.e. Vector2.one - (m_Content.anchorMax - m_Content.anchorMin).
        private void ResizeContent()
        {
            if (content.localEulerAngles != Vector3.zero)
            {
                throw new System.Exception(CONTENT_ROTATION_EXCEPTION);
            }

            float extraLoopSpacing = 0;
            if (m_ShouldLoop)
            {
                extraLoopSpacing = GetMaxItemSpacing();
            }

            // We can't use the pre-calculated snap position because this function may be called
            // from loop, when these values would be incorrect.
            Vector2 distanceBetweenEndPoints =
                CalculateSnapPosition(m_Items.First.Value.rectTransform) -
                CalculateSnapPosition(m_Items.Last.Value.rectTransform);
            distanceBetweenEndPoints.x = Mathf.Abs(distanceBetweenEndPoints.x);
            distanceBetweenEndPoints.y = Mathf.Abs(distanceBetweenEndPoints.y);

            // .sizeDelta is not accurate if the rect is scaled relative to its parent (w/ anchors).
            Vector2 amountOfScrollSnap = (Vector2.one - (content.anchorMax - content.anchorMin))
                    * rectTransform.rect.size;

            // Divide by local scale to convert from scroll snap units to content units.
            Vector2 newContentSize = (distanceBetweenEndPoints + amountOfScrollSnap)
                    / content.localScale;

            // Loop spacing is already in content units, because item spacing is in content units.
            newContentSize[m_Axis] += extraLoopSpacing;

            // Don't resize along inverse axis.
            newContentSize[m_InverseAxis] = content.sizeDelta[m_InverseAxis];

            content.sizeDelta = newContentSize;
        }

        /// <summary>
        /// Resizes the items so they are the same size they were before the content was resized.
        /// Only affects items that resize themselves based on the size of the content
        /// </summary>
        /// <param name="oldContentSize">The size of the content before it was resized.</param>
        // The newDelta is calculated by taking the new size of the content minus the old size
        // of the item (which is the size we want the item to be). This gives us the difference
        // between the size of the content and the size of the item, which (when the anchor points
        // are not together) is exactly what the sizeDelta is. For example:
        // oldContentSize = 100, 100;
        // newContentSize = 150, 150; (bigger)
        // itemSizeDelta = -50, -50; This means that:
        // oldItemSize = 50, 50; So this is what we want the item's size to be.
        // differenceBetweenContentAndItem = 100, 100;
        // if we negate this the new item's size delta will be -100, -100 so its size will be
        // 50, 50 which was the goal.
        private void ResizeItems(Vector2 oldContentSize)
        {
            foreach(Item item in m_Items)
            {
                var itemRectTransform = item.rectTransform;
                // Only resize items that stretch.
                if (itemRectTransform.anchorMin[m_Axis] != itemRectTransform.anchorMax[m_Axis])
                {
                    var newDelta = -(content.sizeDelta
                            - (oldContentSize + itemRectTransform.sizeDelta));
                    itemRectTransform.sizeDelta = newDelta;
                }
            }
        }

        /// <summary>
        /// Moves the items so they are snappable.
        /// </summary>
        // The just snappable position is the position that the scroll snap's pivot would be at (in
        // content space) if the content's top-left corner overlapped the scroll snap's top-left
        // corner. We calculate this by taking the top-left corner of the of the content and
        // subtracting the distance between the top-left corner of the scroll snap and the scroll
        // snap's pivot. Because the corners of the scroll snap are already in local space (pivot
        // is 0,0) the distance is simply the location of the top-left corner. We subtract because
        // if (for example) the top-left of the scroll snap is to the left of the pivot, we want
        // the snappable position to be to the right of the top-left of the content.
        // Divide distanceToPivot by local scale to convert from content units to scroll snap units.
        // Do Not use WorldPointToLocalPointInRectangle because we want the distance to be relative
        // to the scroll Snap pivot, and if we convert, it will be relative to the content pivot.
        private void ShiftItemsForSnappability()
        {
            if (content.localEulerAngles != Vector3.zero)
            {
                throw new System.Exception(CONTENT_ROTATION_EXCEPTION);
            }

            var contentLocalCorners = new Vector3[4];
            content.GetLocalCorners(contentLocalCorners);
            float loopSpacingAdjustment = m_ShouldLoop ? GetMaxItemSpacing() / 2 : 0;
            Vector3 adjustedContentCorner = contentLocalCorners[1];
            if (m_Layout == Layout.Horizontal)
            {
                adjustedContentCorner[m_Axis] += loopSpacingAdjustment;
            } else
            {
                adjustedContentCorner[m_Axis] -= loopSpacingAdjustment;
            }

            var scrollSnapLocalCorners = new Vector3[4];
            rectTransform.GetLocalCorners(scrollSnapLocalCorners);
            Vector3 distanceToPivot = scrollSnapLocalCorners[1];
            // See above comment.
            distanceToPivot.x /= content.localScale.x;
            distanceToPivot.y /= content.localScale.y;

            Vector2 justSnappablePosition = adjustedContentCorner - distanceToPivot;
            Vector2 relativeToContentPosition =
                ScrollSnapUtilities.WorldPointToLocalPointInRectangle(
                    content, m_Items.First.Value.rectTransform.position);

            Vector2 shiftAmount = justSnappablePosition - relativeToContentPosition;
            // Don't shift along inverse axis.
            shiftAmount[m_InverseAxis] = 0;
            ShiftItems(m_Items.First, shiftAmount);
        }
        
        /// <summary>
        /// Shifts all the items including and endward-from the given startwardItem by the
        /// given shiftAmount.
        /// </summary>
        /// <param name="startmostItem">The startward most item to shift.</param>
        /// <param name="shiftAmount">The amount to shift all of the items by.</param>
        private void ShiftItems(LinkedListNode<Item> startmostItem, Vector2 shiftAmount)
        {
            // Note: It would be an efficiency improvement to not shift the items if the
            // shiftAmount were ~= to Vector2.zero. But in certain cases the items need to be
            // re-spaced even if the startward item is within the bounds of the content. E.g.
            // if the item's have different anchors this can happen.

            startmostItem.Value.rectTransform.anchoredPosition += shiftAmount;
            if (m_Items.Count == 1)
            {
                return;
            }

            var startwardItemNode = startmostItem;
            var currentItemNode = startwardItemNode.Next;
            Item startwardItem;
            Item currentItem;
            while (currentItemNode != null)
            {
                startwardItem = startwardItemNode.Value;
                currentItem = currentItemNode.Value;

                // The position of the startward item, relative to the current item's anchor,
                // plus (x-axis) or minus (y-axis) the space between them.
                Vector3 newPosition = currentItem.rectTransform.anchoredPosition;
                newPosition[m_Axis] = ScrollSnapUtilities.WorldPointToRelativeToAnchorPoint(
                        currentItem.rectTransform, startwardItem.rectTransform.position)[m_Axis];
                float spacing = currentItem.GetStartSpacing(startwardItem, m_Axis);
                newPosition[m_Axis] += (m_Layout == Layout.Horizontal) ? spacing : -spacing;
                currentItem.rectTransform.anchoredPosition = newPosition;

                currentItemNode = currentItemNode.Next;
                startwardItemNode = startwardItemNode.Next;  // The old current item.
            }
        }

        /// <summary>
        /// Moves the content so it appears the items did not move.
        /// </summary>
        /// <param name="startmostItemInitialWorldPosition">
        /// The world position of the startmost item before it was moved in ShiftITems. This is
        /// used to reset the content back to a position where the startmost item has the same
        /// world position it originally had.
        /// </param>
        private void ShiftContent(Vector3 startmostItemInitialWorldPosition)
        {
            // Convert to scroll snap units.
            var initialPosition = ScrollSnapUtilities.WorldPointToLocalPointInRectangle(
                    rectTransform, startmostItemInitialWorldPosition);
            var finalPosition = ScrollSnapUtilities.WorldPointToLocalPointInRectangle(
                    rectTransform, m_Items.First.Value.rectTransform.position);
            Vector2 delta = finalPosition - initialPosition;

            content.anchoredPosition -= delta;
        }

        /// <summary>
        /// Updates the snap positions of all of the items.
        /// </summary>
        private void UpdateSnapPositions()
        {
            foreach (Item item in m_Items)
            {
                item.snapPosition = CalculateSnapPosition(item.rectTransform);
            }
        }

        /// <summary>
        /// Modifies the content and/or items so that it appears the items have looped around
        /// to the other side.
        /// </summary>
        private bool LoopItems()
        {
            if (content.anchoredPosition[m_Axis] == m_PreviousPosition[m_Axis])
            {
                // No movement.
                return false;
            }
            var initialContentPosition = content.anchoredPosition;
            var movingTowardEndItems = IsMovingTowardEndwardItems(
                m_PreviousPosition, content.anchoredPosition);
            var distance = GetLoopDistance();

            /* Do the Looping */
            bool didLooping = false;
            if (m_Items.Count == 1)
            {
                if (movingTowardEndItems)
                {
                    didLooping = LoopStartwardSingle(distance);
                }
                else
                {
                    didLooping = LoopEndwardSingle(distance);
                }
            }
            else
            {
                if (movingTowardEndItems)
                {
                    didLooping = LoopStartwardMultiple(distance);
                }
                else
                {
                    didLooping = LoopEndwardMultiple(distance);
                }

                /* Update Content Size */
                if (didLooping)
                {
                    ArrangeItems();
                }
            }
            
            if (didLooping)
            {
                UpdateScroll(initialContentPosition);
            }
            return didLooping;
        }

        /// <summary>
        /// Logic for looping a startward item when it is the only item in the scroll snap.
        /// </summary>
        /// <param name="distance">
        /// The distance in scroll snap units the item needs to be from the pivot of the
        /// scroll snap before it can be looped.
        /// </param>
        /// <returns>True if the item was looped, false otherwise.</returns>
        private bool LoopStartwardSingle(float distance)
        {
            var item = m_Items.First.Value;
            if (!StartwardItemShouldLoop(item, distance))
            {
                return false;
            }
            var newContentPosition = content.anchoredPosition;
            var spacing = item.GetStartSpacing(item, m_Axis);
            newContentPosition[m_Axis] += (m_Layout == Layout.Horizontal) ? spacing : -spacing;
            content.anchoredPosition = newContentPosition;

            return true;
        }

        /// <summary>
        /// Logic for looping startward items when there is more than one item.
        /// </summary>
        /// <param name="distance">
        /// The distance in scroll snap units the item needs to be from the pivot of the
        /// scroll snap before it can be looped.
        /// </param>
        /// <returns>True if at least one item was looped, false otherwise</returns>
        private bool LoopStartwardMultiple(float distance)
        {
            // If we're moving toward the end items we need to move the start items to the end.
            var itemNode = m_Items.First;
            bool didLooping = false;
            for (int i = 0; i < m_Items.Count; i++)
            {
                var item = itemNode.Value;
                // If we don't need to loop this item no need to continue.
                if (!StartwardItemShouldLoop(item, distance))
                {
                    break;
                }

                var startwardItem = m_Items.Last.Value;
                float startSpacing = item.GetStartSpacing(startwardItem, m_Axis);
                // The position of the startward item relative to the current item's anchor point.
                Vector2 startwardItemPosition = ScrollSnapUtilities
                    .WorldPointToRelativeToAnchorPoint(item.rectTransform,
                        startwardItem.rectTransform.position);

                Vector2 newPosition = startwardItemPosition;
                newPosition[m_Axis] +=
                    (m_Layout == Layout.Horizontal) ? startSpacing : -startSpacing;
                // Don't move along the inverse axis;
                newPosition[m_InverseAxis] = item.rectTransform.anchoredPosition[m_InverseAxis];

                item.rectTransform.anchoredPosition = newPosition;

                // Move the item to the back of the list.
                m_Items.Remove(itemNode);
                m_Items.AddLast(itemNode);

                itemNode = m_Items.First;

                didLooping = true;
            }

            return didLooping;
        }

        /// <summary>
        /// Logic for looping an endward item when it is the only item in the scroll snap.
        /// </summary>
        /// <param name="distance">
        /// The distance in scroll snap units the item needs to be from the pivot of the
        /// scroll snap before it can be looped.
        /// </param>
        /// <returns>True if the item was looped, false otherwise.</returns>
        private bool LoopEndwardSingle(float distance)
        {
            var item = m_Items.Last.Value;
            if (!EndwardItemShouldLoop(item, distance))
            {
                return false;
            }
            var newContentPosition = content.anchoredPosition;
            var spacing = item.GetEndSpacing(item, m_Axis);
            newContentPosition[m_Axis] += (m_Layout == Layout.Horizontal) ? -spacing : spacing;
            content.anchoredPosition = newContentPosition;

            return true;
        }

        /// <summary>
        /// Logic for looping endward items when there is more than one item.
        /// </summary>
        /// <param name="distance">
        /// The distance in scroll snap units the item needs to be from the pivot of the
        /// scroll snap before it can be looped.
        /// </param>
        /// <returns>True if at least one item was looped, false otherwise.</returns>
        private bool LoopEndwardMultiple(float distance)
        {
            var itemNode = m_Items.Last;
            bool didLooping = false;
            for (int i = 0; i < m_Items.Count; i++)
            {
                var item = itemNode.Value;
                // If we don't need to loop this item no need to continue.
                if (!EndwardItemShouldLoop(item, distance))
                {
                    break;
                }

                var endwardItem = m_Items.First.Value;
                float endSpacing = item.GetEndSpacing(endwardItem, m_Axis);
                // The position of the endward item relative to the current item's anchor point.
                Vector2 endwardItemPosition = ScrollSnapUtilities
                    .WorldPointToRelativeToAnchorPoint(item.rectTransform,
                        endwardItem.rectTransform.position);

                Vector2 newPosition = endwardItemPosition;
                newPosition[m_Axis] +=
                    (m_Layout == Layout.Horizontal) ? -endSpacing : endSpacing;
                // Don't move along the inverse axis;
                newPosition[m_InverseAxis] = item.rectTransform.anchoredPosition[m_InverseAxis];

                item.rectTransform.anchoredPosition = newPosition;

                // Move the item to the back of the list.
                m_Items.Remove(itemNode);
                m_Items.AddFirst(itemNode);

                itemNode = m_Items.Last;

                didLooping = true;
            }
            return didLooping;
        }

        /// <summary>
        /// Should the startward item be looped to the end?
        /// </summary>
        /// <param name="item">The item to check for whether it should be looped.</param>
        /// <param name="distance">
        /// The distance in scroll snap units the item needs to be from the pivot of the
        /// scroll snap before it can be looped.
        /// </param>
        /// <returns>True if the item should be looped, false otherwise.</returns>
        private bool StartwardItemShouldLoop(Item item, float distance)
        {
            var itemPosition = ScrollSnapUtilities.WorldPointToLocalPointInRectangle(
                    rectTransform, item.rectTransform.position);
            if (m_Layout == Layout.Horizontal)
            {
                return itemPosition[m_Axis] <= -distance;
            }
            else
            {
                return itemPosition[m_Axis] >= distance;
            }
        }

        /// <summary>
        /// Should the endward item be looped to the start?
        /// </summary>
        /// <param name="item">The item to chck for whether it should be looped.</param>
        /// <param name="distance">
        /// The distance in scroll snap units the item needs to be from the pivot of the
        /// scroll snap before it can be looped.
        /// </param>
        /// <returns>True if the item should be looped, false otherwise.</returns>
        private bool EndwardItemShouldLoop(Item item, float distance)
        {
            var itemPosition = ScrollSnapUtilities.WorldPointToLocalPointInRectangle(
                    rectTransform, item.rectTransform.position);
            if (m_Layout == Layout.Horizontal)
            {
                return itemPosition[m_Axis] >= distance;
            }
            else
            {
                return itemPosition[m_Axis] <= -distance;
            }
        }

        /// <summary>
        /// Get the distance in scroll snap units an item needs to be from the scroll snap's
        /// pivot before it should be looped.
        /// </summary>
        /// <returns>
        /// The distance in scroll snap units an item needs to be from the scroll snap's pivot
        /// before it should be looped.
        /// </returns>
        private float GetLoopDistance()
        {
            // NOTE: This algorithm isn't the best at keeping the scroll snap optimally centered.
            // If we wanted to do that we'd have to calculate loop distances for each item
            // individually, and the loss in efficiency isn't worth the gain.
            // See Picture: https://goo.gl/ynU6bL
            Vector3[] contentWorldCorners = new Vector3[4];
            content.GetWorldCorners(contentWorldCorners);
            Vector3[] scrollSnapLocalCorners = new Vector3[4];
            rectTransform.GetLocalCorners(scrollSnapLocalCorners);

            // Convert content world corners into the scroll snap's space (scroll snap units).
            var contentTopLeftInScrollSnap = ScrollSnapUtilities
                .WorldPointToLocalPointInRectangle(rectTransform, contentWorldCorners[1]);
            var contentBottomRightInScrollSnap = ScrollSnapUtilities
                .WorldPointToLocalPointInRectangle(rectTransform, contentWorldCorners[3]);

            float contentSize = Mathf.Abs(contentTopLeftInScrollSnap[m_Axis]
                - contentBottomRightInScrollSnap[m_Axis]);
            float scrollSnapSize = Mathf.Abs(scrollSnapLocalCorners[1][m_Axis]
                - scrollSnapLocalCorners[3][m_Axis]);

            // Distance is in scroll snap units.
            return (contentSize - scrollSnapSize) / 2;
        }

        /// <summary>
        /// Is the second position closer to the end than the first position?
        /// </summary>
        /// <param name="firstPosition">The first position to compare.</param>
        /// <param name="secondPosition">The second position to compare.</param>
        /// <returns>
        /// If the second position is closer to the end (end items closer to the scroll
        /// snap's pivot) than the first position.
        /// </returns>
        private bool IsMovingTowardEndwardItems(Vector3 firstPosition, Vector3 secondPosition)
        {
            var secondPositionMorePositive = secondPosition[m_Axis] > firstPosition[m_Axis];
            return (m_Layout == Layout.Vertical)
                ? secondPositionMorePositive : !secondPositionMorePositive;
        }

        /// <summary>
        /// Calculates the position the content needs to be in for the item to be centered on the
        /// rectTransform's pivot.
        /// </summary>
        /// <param name="rect">
        /// The RectTransform of the item we are finding the snap position of.
        /// </param>
        /// <returns>The snap position of the rect.</returns>
        // The relative position tells us the position of the item's pivot relative to the scroll
        // snap's pivot. Which is how far we need to move the content from it's current position to
        // "snap" the item. But if the item is +200x from the snap point (right) we actually want to
        // move the content -200x (left). So we take the content's current position minus the item's
        // relative position to find the snap position.
        private Vector2 CalculateSnapPosition(RectTransform rect)
        {
            var relativeToScrollSnapPosition = ScrollSnapUtilities.
                        WorldPointToLocalPointInRectangle(rectTransform, rect.position);
            var snapPosition = content.anchoredPosition;
            snapPosition.x -= relativeToScrollSnapPosition.x;
            snapPosition.y -= relativeToScrollSnapPosition.y;
            return snapPosition;
        }

        /// <summary>
        /// Called after update is called, but before the frame is rendered. Used to update the
        /// position and velocity of the scroll snap.
        /// </summary>
        private void LateUpdate()
        {
            if (m_Scroller.ComputeScrollPosition())
            {
                content.anchoredPosition = m_Scroller.currentPosition;
            }

            if (m_ShouldLoop)
            {
                LoopItems();
            }

            // TODO: Add better velocity tracking ala Android. 
            // Skip the frames where we update the scroll because on these frames the content
            // moves, if we tracked these our velocity would be incorrect.
            if (!m_UpdatedScrollThisFrame)
            {
                Vector3 newVelocity = (content.anchoredPosition - m_PreviousPosition)
                        / Time.unscaledDeltaTime;
                m_Velocity = Vector3.Lerp(m_Velocity, newVelocity, Time.unscaledDeltaTime * 10);
            }

            m_UpdatedScrollThisFrame = false;
            m_PreviousPosition = content.anchoredPosition;
        }

        /// <summary>
        /// Called when unity detects a drag gesture beginning. Stops any current animations and
        /// initializes the scroll snap for dragging and flinging.
        /// </summary>
        public virtual void OnBeginDrag(PointerEventData ped)
        {
            m_CursorInitialPosition = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, ped.position, ped.pressEventCamera, out m_CursorInitialPosition);
            m_ContentInitialPosition = content.anchoredPosition;
            m_LastPressedCamera = ped.pressEventCamera;
            m_Scroller.StopAnimation();
        }

        /// <summary>
        /// Called when unity detects a drag gesture. Changes the position of the content so that
        /// it appears to be dragged.
        /// </summary>
        public virtual void OnDrag(PointerEventData ped)
        {
            Vector2 cursorPosition;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, ped.position, ped.pressEventCamera, out cursorPosition))
                return;
            
            var pointerDelta = cursorPosition - m_CursorInitialPosition;
            Vector2 position = m_ContentInitialPosition + pointerDelta;
            Vector2 offset = CalculateOffset(position - content.anchoredPosition);
            position += offset;
            // Don't move along the inverse axis.
            position[m_InverseAxis] = content.anchoredPosition[m_InverseAxis];
            content.anchoredPosition = position;
        }

        /// <summary>
        /// Called when unity detects a drag gesture ending. Used to start a fling gesture.
        /// </summary>
        public virtual void OnEndDrag(PointerEventData ped)
        {
            if (!m_ShouldLoop && m_Items.Count < 2)
            {
                // If there is 1 or 0 items and we do not loop, we have nothing to fling to.
                return;
            }

            var finalPosition = new Vector2(
                content.anchoredPosition.x + m_Scroller.CalculateMovementDelta(
                    m_Velocity.x, m_DecelerationRate),
                content.anchoredPosition.y + m_Scroller.CalculateMovementDelta(
                    m_Velocity.y, m_DecelerationRate));
            var snapPosition = GetSnapPosition(finalPosition);
            float decelerationRate = m_Scroller.CalculateDecelerationRate(m_Velocity[m_Axis],
                    snapPosition[m_Axis] - content.anchoredPosition[m_Axis]);
            m_Scroller.StartFling(m_Velocity, content.anchoredPosition, decelerationRate);
        }

        /// <summary>
        /// Chooses the optimal position to snap to, based on where the default animation would
        /// have stopped.
        /// </summary>
        /// <param name="finalPosition">The position the content would have stopped at.</param>
        /// <returns>The optimal position to snap to.</returns>
        protected virtual Vector2 GetSnapPosition(Vector2 finalPosition)
        {
            if (!m_ShouldLoop && m_Items.Count < 2)
            {
                // If there is 1 or 0 items and we do not loop, there is only 1 snap position, so
                // no need to do all of the logic.
                return content.anchoredPosition;
            }

            // These make it so our linked list is circular if this scroll snap should loop.
            Func<LinkedListNode<Item>, LinkedListNode<Item>> getNextItem =
                (LinkedListNode<Item> item) => m_ShouldLoop ?
                (item.Next ?? item.List.First) : item.Next;
            Func<LinkedListNode<Item>, LinkedListNode<Item>> getPreviousItem =
                (LinkedListNode<Item> item) => m_ShouldLoop ?
                (item.Previous ?? item.List.Last) : item.Previous;

            var initialPosition = content.anchoredPosition;

            var finalPositionIsMorePositive = finalPosition[m_Axis] > initialPosition[m_Axis];
            var movingTowardEnd = IsMovingTowardEndwardItems(initialPosition, finalPosition);

            var itemNode = (movingTowardEnd ? m_Items.First.Next : m_Items.Last.Previous)
                ?? m_Items.First;  // Only a single item.
            var currentSnapPosition = movingTowardEnd
                ? m_Items.First.Value.snapPosition : m_Items.Last.Value.snapPosition;
            var minDistance = Mathf.Abs(finalPosition[m_Axis] - currentSnapPosition[m_Axis]);

            // The default behavior is that we don't move. See Issue #7.
            var selectedSnapPosition = content.anchoredPosition;
            
            while (itemNode != null)
            {
                 var spacing = movingTowardEnd
                    ? itemNode.Value.GetStartSpacing(getPreviousItem(itemNode).Value, m_Axis)
                    : itemNode.Value.GetEndSpacing(getNextItem(itemNode).Value, m_Axis);
                spacing *= content.localScale[m_Axis];
                currentSnapPosition[m_Axis] += finalPositionIsMorePositive ? spacing : -spacing;
                itemNode = movingTowardEnd ? getNextItem(itemNode) : getPreviousItem(itemNode);

                var currentPositionIsMorePositive =
                    currentSnapPosition[m_Axis] > initialPosition[m_Axis];
                if (finalPositionIsMorePositive && !currentPositionIsMorePositive
                    || !finalPositionIsMorePositive && currentPositionIsMorePositive)
                {
                    // The current position is behind the initial position.
                    continue;
                }

                var distance = Mathf.Abs(finalPosition[m_Axis] - currentSnapPosition[m_Axis]);
                if (distance > minDistance)
                {
                    var selectedPositionIsMorePositive =
                        selectedSnapPosition[m_Axis] > initialPosition[m_Axis];
                    if (finalPositionIsMorePositive && !selectedPositionIsMorePositive
                        || !finalPositionIsMorePositive && selectedPositionIsMorePositive
                        || selectedSnapPosition == content.anchoredPosition)
                    {
                        // The selected position is behind the initial position.
                        selectedSnapPosition = currentSnapPosition;
                    }
                    return selectedSnapPosition;
                }
                else  // Found a closer position;
                {
                    minDistance = distance;
                    selectedSnapPosition = currentSnapPosition;
                }
            }
            return selectedSnapPosition;
        }

        /// <summary>
        /// Gets the item closest to the scroll snap's pivot. Note: This does not use caching,
        /// every time you call it it will iterate over all of the items.
        /// </summary>
        /// <returns>The item closest to the scroll snap's pivot.</returns>
        private Item GetClosestItem()
        {
            var minDistance = float.MaxValue;
            Item closestItem = m_Items.First.Value;
            foreach (var item in m_Items)
            {
                Vector2 position = ScrollSnapUtilities.WorldPointToLocalPointInRectangle(
                    rectTransform, item.rectTransform.position);
                float distance = Mathf.Abs(position[m_Axis]);
                if (distance > minDistance)
                {
                    return closestItem;
                }
                else  // Found closer item.
                {
                    minDistance = distance;
                    closestItem = item;
                }
            }
            return closestItem;
        }

        /// <summary>
        /// Adds the given RectTransform as an item after /current/ endmost item. If the
        /// scroll snap has looped the /current/ endmost item may not be the same as the
        /// /original/ endmost item.
        /// </summary>
        /// <param name="itemTransform">The RectTransform of the item to add.</param>
        /// <param name="startMargin">
        /// The minimum margin between this item and the item in the startward direction from it.
        /// </param>
        /// <param name="endMargin">
        /// The minimum margin between this item and the item in the endward direction from it.
        /// </param>
        /// <returns>The newly inserted item.</returns>
        public Item AddItemAtEnd(RectTransform itemTransform, float startMargin, float endMargin)
        {
            return InternalAddItem(m_Items.Last, itemTransform, startMargin, endMargin);
        }

        /// <summary>
        /// Adds the given RectTransform as an item before the /current/ startmost item. If the
        /// scroll snap has looped the /current/ startmost item may not be the same as the
        /// /original/ startmost item.
        /// </summary>
        /// <param name="itemTransform">The RectTransform of the item to add.</param>
        /// <param name="startMargin">
        /// The minimum margin between this item and the item in the startward direction from it.
        /// </param>
        /// <param name="endMargin">
        /// The minimum margin between this item and the item in the endward direction from it.
        /// </param>
        /// <returns>The newly inserted item.</returns>
        public Item AddItemAtStart(RectTransform itemTransform, float startMargin, float endMargin)
        {
            return InternalAddItem(null, itemTransform, startMargin, endMargin);
        }

        /// <summary>
        /// Adds the given RectTransform as an item after the given startward item.
        /// </summary>
        /// <param name="startwardItem">The item to insert the new item after.</param>
        /// <param name="itemTransform">The RectTransform of the item to add.</param>
        /// <param name="startMargin">
        /// The minimum margin between this item and the item in the startward direction from it.
        /// </param>
        /// <param name="endMargin">
        /// The minimum margin between this item and the item in the endward direction from it.
        /// </param>
        /// <returns>The newly inserted item.</returns>
        public Item AddItemAfter(Item startwardItem, RectTransform itemTransform,
            float startMargin, float endMargin)
        {
            var startwardItemNode = m_Items.Find(startwardItem);
            if (startwardItemNode == null)
            {
                throw new InvalidOperationException(
                    string.Format(DOES_NOT_CONTAIN_ITEM_EXCEPTION, "startward"));
            }
            return InternalAddItem(startwardItemNode, itemTransform, startMargin, endMargin);
        }

        /// <summary>
        /// Adds the given RectTransform as an item before the given endward item.
        /// </summary>
        /// <param name="endwardItem">The item to insert the new item before</param>
        /// <param name="itemTransform">The RectTransform of the item to add.</param>
        /// <param name="startMargin">
        /// The minimum margin between this item and the item in the startward direction from it.
        /// </param>
        /// <param name="endMargin">
        /// The minimum margin between this item and the item in the endward direction from it.
        /// </param>
        /// <returns>The newly inserted item.</returns>
        public Item AddItemBefore(Item endwardItem, RectTransform itemTransform,
            float startMargin, float endMargin)
        {
            var endwardItemNode = m_Items.Find(endwardItem);
            if (endwardItemNode == null)
            {
                throw new InvalidOperationException(
                    string.Format(DOES_NOT_CONTAIN_ITEM_EXCEPTION, "endward"));
            }
            return InternalAddItem(endwardItemNode.Previous, itemTransform,
                startMargin, endMargin);
        }

        /// <summary>
        /// Adds the given RectTransform as an item after the given startward item. If the
        /// startward item is null the item is inserted at the startward end of the scroll snap.
        /// If this item is the first item to be inserted, its pivot will be aligned with the
        /// pivot of the scroll snap (along the movement axis) upon insertion.
        /// </summary>
        /// <param name="startwardItemNode">The item to insert the new item after, or null.</param>
        /// <param name="itemTransform">The RectTransform of the item to be inserted.</param>
        /// <param name="startMargin">
        /// The minimum margin between this item and the item in the startward direction from it.
        /// </param>
        /// <param name="endMargin">
        /// The minimum margin between this item and the item in the endward direction from it.
        /// </param>
        private Item InternalAddItem(
            LinkedListNode<Item> startwardItemNode, RectTransform itemTransform,
            float startMargin, float endMargin)
        {
            if (itemTransform == null)
            {
                throw new System.ArgumentNullException("itemTransform");
            }

            // Create the item.
            itemTransform.SetParent(content, /* worldPositionStays */ true);
            var item = new Item(itemTransform);
            item.SetMinimumStartMargin(startMargin, m_Axis);
            item.SetMinimumEndMargin(endMargin, m_Axis);

            // Insert the item into the linked list.
            LinkedListNode<Item> itemNode = null;
            if (startwardItemNode != null)
            {
                itemNode = m_Items.AddAfter(startwardItemNode, item);
            }
            else
            {
                itemNode = m_Items.AddFirst(item);
            }
            LinkedListNode<Item> endwardItemNode = itemNode.Next;

            if (startwardItemNode != null)
            {
                // Position the item relative to the startward item.
                var startSpacing = ScrollSnapUtilities.CalculateSpacing(
                    startwardItemNode.Value, item, m_Axis);
                // We convert to relative to anchor point so that the code works even 
                // if they ahve different anchors.
                var newPosition = ScrollSnapUtilities.WorldPointToRelativeToAnchorPoint(
                    item.rectTransform, startwardItemNode.Value.rectTransform.position);
                newPosition[m_Axis] +=
                    (m_Layout == Layout.Horizontal) ? startSpacing : -startSpacing;
                // Don't move along the inverse axis.
                newPosition[m_InverseAxis] = item.rectTransform.anchoredPosition[m_InverseAxis];
                item.rectTransform.anchoredPosition = newPosition;

                if (endwardItemNode != null)
                {
                    // Shift the endward items so they are the correct distance from the new item.

                    // Convert the positions into the same coordinate system.
                    var endwardPosition = ScrollSnapUtilities.WorldPointToLocalPointInRectangle(
                        m_RectTransform, endwardItemNode.Value.rectTransform.position);
                    var itemPosition = ScrollSnapUtilities.WorldPointToLocalPointInRectangle(
                        m_RectTransform, item.rectTransform.position);

                    float desiredSpacing = ScrollSnapUtilities.CalculateSpacing(
                        item, endwardItemNode.Value, m_Axis);
                    float currentSpacing = endwardPosition[m_Axis] - itemPosition[m_Axis];
                    float delta = desiredSpacing - currentSpacing;

                    Vector2 shiftAmount = Vector2.zero;
                    shiftAmount[m_Axis] = (m_Layout == Layout.Horizontal) ? delta : -delta;
                    ShiftItems(endwardItemNode, shiftAmount);
                }
            }
            else if (endwardItemNode != null)
            {
                var endSpacing = ScrollSnapUtilities.CalculateSpacing(
                    item, endwardItemNode.Value, m_Axis);
                // We convert to relative to anchor point so that the code
                // works even if they have different anchors.
                var newPosition = ScrollSnapUtilities.WorldPointToRelativeToAnchorPoint(
                    item.rectTransform, endwardItemNode.Value.rectTransform.position);
                newPosition[m_Axis] +=
                    (m_Layout == Layout.Horizontal) ? -endSpacing : endSpacing;
                // Don't move along the inverse axis.
                newPosition[m_InverseAxis] =
                    item.rectTransform.anchoredPosition[m_InverseAxis];
                item.rectTransform.anchoredPosition = newPosition;
            }
            else
            {
                // We're modifying in global space so we don't have to worry about anchors.
                var newPosition = item.rectTransform.position;
                newPosition[m_Axis] = rectTransform.position[m_Axis];
                item.rectTransform.position = newPosition;
            }

            // If the monobehavior has not been enabled yet don't call arrange items. This can
            // improve performance when populating a scroll snap programmatically on start up.
            if (isActiveAndEnabled)
            {
                var initialContentPosition = content.anchoredPosition;
                ArrangeItems();
                UpdateScroll(initialContentPosition);
            }
            return item;
        }

        /// <summary>
        /// Removes the given item from the scroll snap. The margin between the two items on
        /// either side of it (if they exist) will be collapsed to the minimum margin between them.
        /// </summary>
        /// <param name="item">The item to remove from the scroll snap.</param>
        /// <returns>True if the item was successfully found and removed, false otherwise.</returns>
        public bool RemoveItem(Item item)
        {
            var itemNode = m_Items.Find(item);
            if (itemNode != null)
            {
                InternalRemoveItem(itemNode);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes the startmost item from the scroll snap.
        /// </summary>
        public void RemoveItemAtStart()
        {
            if (m_Items.First == null)
            {
                throw new InvalidOperationException(SCROLL_SNAP_IS_EMPTY_EXCEPTION);
            }
            InternalRemoveItem(m_Items.First);
        }

        /// <summary>
        /// Removes the endmost item from the scroll snap.
        /// </summary>
        public void RemoveItemAtEnd()
        {
            if (m_Items.Last == null)
            {
                throw new InvalidOperationException(SCROLL_SNAP_IS_EMPTY_EXCEPTION);
            }
            InternalRemoveItem(m_Items.Last);
        }

        /// <summary>
        /// Removes the given itemNode from the scroll snap, then updates the arrangement of items
        /// and the current scroll.
        /// </summary>
        /// <param name="itemNode">The item to remove.</param>
        public void InternalRemoveItem(LinkedListNode<Item> itemNode)
        {
            m_Items.Remove(itemNode);
            Destroy(itemNode.Value.rectTransform.gameObject);
            if (isActiveAndEnabled)
            {
                var initialContentPosition = content.anchoredPosition;
                ArrangeItems();
                UpdateScroll(initialContentPosition);
            }
        }

        /// <summary>
        /// Updates the current drag/scroller animation for the new content positioned. Called
        /// when the content is moved for non-scroll reasons (i.e. looping, inserting items, etc).
        /// </summary>
        /// <param name="initialContentPosition">
        /// The anchoredPosition of the content before it was moved.
        /// </param>
        private void UpdateScroll(Vector2 initialContentPosition)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform,
                Input.mousePosition, m_LastPressedCamera, out m_CursorInitialPosition);
            m_ContentInitialPosition = content.anchoredPosition;

            if (!m_Scroller.isFinished)
            {
                m_Scroller.ShiftAnimation(content.anchoredPosition - initialContentPosition);
            }

            m_UpdatedScrollThisFrame = true;
        }

        /// <summary>
        /// Get the current largest spacing between two items in the scroll snap.
        /// </summary>
        /// <returns>The current largest spacing between two items in the scroll snap.</returns>
        private float GetMaxItemSpacing()
        {
            if (!m_ItemSpacingDirty)
            {
                return m_CachedMaxItemSpacing;
            }
            m_ItemSpacingDirty = false;

            float maxSpacing = 0;
            var itemNode = m_Items.First;
            for (int i = 0; i < m_Items.Count; i++)
            {
                var endwardItemNode = itemNode.Next ?? (m_ShouldLoop ? m_Items.First : null);
                if (endwardItemNode == null)
                {
                    break;
                }
                float currentSpacing = itemNode.Value.GetEndSpacing(endwardItemNode.Value, m_Axis);
                maxSpacing = Mathf.Max(maxSpacing, currentSpacing);
                itemNode = itemNode.Next;
            }
            m_CachedMaxItemSpacing = maxSpacing;
            return maxSpacing;
        }

        /// <summary>
        /// Calculates the offset from the rectTransform the content would have if the given delta
        /// were applied. Used to keep the content from going inside the bounds of the rectTransform.
        /// </summary>
        /// <param name="delta">The movement delta that would be applied.</param>
        /// <returns>The offset from the rectTransform.</returns>
        private Vector2 CalculateOffset(Vector2 delta)
        {
            if (content.localEulerAngles != Vector3.zero)
            {
                throw new System.Exception(CONTENT_ROTATION_EXCEPTION);
            }

            // Everything needs to be in the space of the content's anchoredPosition (b/c that is
            // what the offset is applied to) which is the space of the scroll snap.
            Vector3[] contentWorldCorners = new Vector3[4];
            content.GetWorldCorners(contentWorldCorners);
            Vector3[] scrollSnapLocalCorners = new Vector3[4];
            rectTransform.GetLocalCorners(scrollSnapLocalCorners);

            var contentMin = ScrollSnapUtilities.WorldPointToLocalPointInRectangle(
                    rectTransform, contentWorldCorners[0]);
            var contentMax = ScrollSnapUtilities.WorldPointToLocalPointInRectangle(
                    rectTransform, contentWorldCorners[2]);
            contentMin.x += delta.x;
            contentMax.x += delta.x;
            contentMin.y += delta.y;
            contentMax.y += delta.y;

            var scrollSnapMin = scrollSnapLocalCorners[0];
            var scrollSnapMax = scrollSnapLocalCorners[2];

            Vector2 offset = Vector2.zero;

            if (contentMin.x > scrollSnapMin.x)
            {
                offset.x = scrollSnapMin.x - contentMin.x;
            }
            else if (contentMax.x < scrollSnapMax.x)
            {
                offset.x = scrollSnapMax.x - contentMax.x;
            }
            
            if (contentMax.y < scrollSnapMax.y)
            {
                offset.y = scrollSnapMax.y - contentMax.y;
            }
            else if (contentMin.y > scrollSnapMin.y)
            {
                offset.y = scrollSnapMin.y - contentMin.y;
            }

            return offset;
        }

        /// <summary>
        /// Draws helper gizmos when the scroll snap is selected.
        /// </summary>
        public void OnDrawGizmosSelected()
        {
            /* Find Scroll Snap Pivot Line */
            var scrollSnapLocalCorners = new Vector3[4];
            rectTransform.GetLocalCorners(scrollSnapLocalCorners);
            // Top left and bottom right corners.
            var firstPoint = scrollSnapLocalCorners[1];
            var secondPoint = scrollSnapLocalCorners[3];
            // The pivot in local coordinates is (0, 0) so this makes it align with the pivot.
            firstPoint[m_Axis] = 0;
            secondPoint[m_Axis] = 0;

            /* Draw the Gizmos */
            Gizmos.color = Color.blue;
            DrawLineScrollSnapSpace(firstPoint, secondPoint);
            Gizmos.color = Color.cyan;
            DrawItemPivots(firstPoint, secondPoint);
            Gizmos.color = Color.green;
            DrawGizmosLoopedItem();
        }

        /// <summary>
        /// Draws lines to indicate the content's childrens' pivots.
        /// </summary>
        /// <param name="firstPoint">
        /// The first point defining a line that the pivot lines need to be parallel to.
        /// </param>
        /// <param name="secondPoint">
        /// The second point defining a line that the pivot lines need to be parallel to.
        /// </param>
        private void DrawItemPivots(Vector3 firstPoint, Vector3 secondPoint)
        {
            foreach (RectTransform child in content)
            {
                var relativePoint = ScrollSnapUtilities.WorldPointToLocalPointInRectangle(
                    rectTransform, child.position);
                firstPoint[m_Axis] = relativePoint[m_Axis];
                secondPoint[m_Axis] = relativePoint[m_Axis];

                DrawLineScrollSnapSpace(firstPoint, secondPoint);
            }
        }

        /// <summary>
        /// Draws a box to indicate where the first child will be after it loops.
        /// </summary>
        private void DrawGizmosLoopedItem()
        {
            if (!m_ShouldLoop || content.childCount == 0)
            {
                // Nothing to draw.
                return;
            }

            // Note: these are not necessarily the startmost and endmost items, but under most
            // circumstances they /should/ be. It just depends on how you set up your scroll snap.
            var firstChild = (RectTransform)content.GetChild(0);
            var lastChild = (RectTransform)content.GetChild(content.childCount - 1);

            var firstChildWorldCorners = new Vector3[4];
            firstChild.GetWorldCorners(firstChildWorldCorners);

            // Converts the corners into scroll snap space so that if the scroll snap is rotated
            // we handle it correctly.
            var firstChildScrollSnapCorners = new Vector3[4];
            firstChildScrollSnapCorners[0] = ScrollSnapUtilities.WorldPointToLocalPointInRectangle(
                rectTransform, firstChildWorldCorners[0]);
            firstChildScrollSnapCorners[1] = ScrollSnapUtilities.WorldPointToLocalPointInRectangle(
                rectTransform, firstChildWorldCorners[1]);
            firstChildScrollSnapCorners[2] = ScrollSnapUtilities.WorldPointToLocalPointInRectangle(
                rectTransform, firstChildWorldCorners[2]);
            firstChildScrollSnapCorners[3] = ScrollSnapUtilities.WorldPointToLocalPointInRectangle(
                rectTransform, firstChildWorldCorners[3]);

            // First we need to move the corners to that the children's pivots align.
            var firstChildScrollSnapPosition = ScrollSnapUtilities.WorldPointToLocalPointInRectangle(
                rectTransform, firstChild.position);
            var lastChildScrollSnapPosition = ScrollSnapUtilities.WorldPointToLocalPointInRectangle(
                rectTransform, lastChild.position);
            var delta = lastChildScrollSnapPosition[m_Axis] - firstChildScrollSnapPosition[m_Axis];

            // Then we need to move the corners by the spacing between their pivots.
            var spacing = ScrollSnapUtilities.CalculateSpacing(
                lastChild, firstChild, m_EndMargin, m_Axis);
            // Convert from content units to scroll snap units.
            spacing *= content.localScale[m_Axis];

            delta += (m_Layout == Layout.Horizontal) ? spacing : -spacing;
            firstChildScrollSnapCorners[0][m_Axis] += delta;
            firstChildScrollSnapCorners[1][m_Axis] += delta;
            firstChildScrollSnapCorners[2][m_Axis] += delta;
            firstChildScrollSnapCorners[3][m_Axis] += delta;

            DrawLineScrollSnapSpace(firstChildScrollSnapCorners[0],
                firstChildScrollSnapCorners[1]);
            DrawLineScrollSnapSpace(firstChildScrollSnapCorners[1],
                firstChildScrollSnapCorners[2]);
            DrawLineScrollSnapSpace(firstChildScrollSnapCorners[2],
                firstChildScrollSnapCorners[3]);
            DrawLineScrollSnapSpace(firstChildScrollSnapCorners[3],
                firstChildScrollSnapCorners[0]);
        }

        /// <summary>
        /// Draws a line defined in the space of the scroll snap.
        /// </summary>
        /// <param name="firstPointLocal">
        /// The first point defining the line. Must be in scroll snap space.
        /// </param>
        /// <param name="secondPointLocal">
        /// The second point defining the line. Must be in scroll snap space.
        /// </param>
        private void DrawLineScrollSnapSpace(Vector3 firstPointLocal, Vector3 secondPointLocal)
        {
            var firstPointWorld = rectTransform.localToWorldMatrix
                .MultiplyPoint3x4(firstPointLocal);
            var secondPointWorld = rectTransform.localToWorldMatrix
                .MultiplyPoint3x4(secondPointLocal);

            Gizmos.DrawLine(firstPointWorld, secondPointWorld);
        }
    }
}

