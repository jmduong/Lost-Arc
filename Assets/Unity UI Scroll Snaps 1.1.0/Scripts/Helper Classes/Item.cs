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

using UnityEngine;

namespace ScrollSnaps
{
    /// <summary>
    /// A class that contains information about a RectTransform that belongs to the Scroll Snap.
    /// </summary>
    public class Item
    {
        /// <summary>
        /// The RectTransform of the item.
        /// </summary>
        public RectTransform rectTransform;
        /// <summary>
        /// The position of the content where this item's rectTransform's pivot will overlap the
        /// pivot of the scroll snap.
        /// </summary>
        public Vector2 snapPosition;

        // The margins are called "minimum margins" because different items' margins are meant
        // to overlap eachother (like collapsible margins in css). For example: if the item to the
        // left of this item has a right margin of 100, and this item has a left margin of 50, the
        // margin between them will be 100.
        // ***The margins are defined in the local space of the items i.e. they are unscaled.***
        private float m_MinLeftMargin;
        private float m_MinTopMargin;
        private float m_MinRightMargin;
        private float m_MinBottomMargin;

        public string name { get { return rectTransform.name; } }

        /// <summary>
        /// Create an item with the given RectTransform.
        /// </summary>
        /// <param name="rectTransform">The RectTransform of the item.</param>
        public Item(RectTransform rectTransform)
        {
            this.rectTransform = rectTransform;
        }

        /// <summary>
        /// Create an item with the given RectTransform and snap position.
        /// </summary>
        /// <param name="rectTransform">The RectTransform of the item.</param>
        /// <param name="snapPosition">
        /// The position the content (parent of item) would need to be in for the item to be
        /// centered on the pivot of the parent of the content (grandparent of item)
        /// </param>
        public Item(RectTransform rectTransform, Vector2 snapPosition)
        {
            this.rectTransform = rectTransform;
            this.snapPosition = snapPosition;
        }

        /// <summary>
        /// Calculates the the distance between the end (right/bottom) edge of the startward item,
        /// and the start (left/top) edge of this item. Then applies it to the correct (left/top)
        /// margin.
        /// </summary>
        /// <param name="startwardTransform">The transform directly start-adjacent to this item</param>
        /// <param name="axis">The x-axis is 0 (left). The y-axis is 1 (top).</param>
        public void SetMinimumStartMargin(RectTransform startwardTransform, int axis)
        {
            var margin = ScrollSnapUtilities.CalculateMinimumMargin(startwardTransform,
                    rectTransform, axis);
            SetMinimumStartMargin(margin, axis);
        }

        /// <summary>
        /// Sets the minimum start (left/top) margin to the given value.
        /// </summary>
        /// <param name="margin">
        /// The minimum distance bteween the end (right/bottom) edge of a startward item, and the
        /// start (left/top) edge of this item.
        /// </param>
        /// <param name="axis">The x-axis is 0 (left). The y-axis is 1 (top).</param>
        public void SetMinimumStartMargin(float margin, int axis)
        {
            // X-Axis
            if (axis == 0)
            {
                m_MinLeftMargin = margin;
            }
            else
            {
                m_MinTopMargin = margin;
            }
        }

        /// <summary>
        /// Calculates the distance between the end (right/bottom) edge of this item, and the start
        /// (left/top) edge of the endward item. Then applies it to the correct (right/bottom)
        /// margin.
        /// </summary>
        /// <param name="endwardTransform">The transform directly end-adjacent to this item.</param>
        /// <param name="axis">The x-axis is 0 (right). The y-axis is 1 (bottom).</param>
        public void SetMinimumEndMargin(RectTransform endwardTransform, int axis)
        {
            var margin = ScrollSnapUtilities.CalculateMinimumMargin(rectTransform,
                    endwardTransform, axis);
            SetMinimumEndMargin(margin, axis);
        }
        
        /// <summary>
        /// Sets the minimum end (right/bottom) margin to the given value.
        /// </summary>
        /// <param name="margin">
        /// The minimum distance bteween the start (left/top) edge of a endward item, and the
        /// end (right/bottom) edge of this item.
        /// </param>
        /// <param name="axis">The x-axis is 0 (left). The y-axis is 1 (top).</param>
        public void SetMinimumEndMargin(float margin, int axis)
        {
            // X-Axis
            if (axis == 0)
            {
                m_MinRightMargin = margin;
            }
            else
            {
                m_MinBottomMargin = margin;
            }
        }
        
        /// <summary>
        /// Gets the minimum start margin for the given axis.
        /// </summary>
        /// <param name="axis">The x-axis is 0 (left). The y-axis is 1 (top).</param>
        /// <returns>The start margin (left/top)</returns>
        public float GetMinimumStartMargin(int axis)
        {
            // X-Axis
            if (axis == 0)
            {
                return m_MinLeftMargin;
            }
            else
            {
                return m_MinTopMargin;
            }
        }

        /// <summary>
        /// Gets the minimum end margin for the given axis.
        /// </summary>
        /// <param name="axis">The x-axis is 0 (right). The y-axis is 1 (bottom).</param>
        /// <returns>The end margin (right/bottom)</returns>
        public float GetMinimumEndMargin(int axis)
        {
            // X-Axis
            if (axis == 0)
            {
                return m_MinRightMargin;
            }
            else
            {
                return m_MinBottomMargin;
            }
        }

        /// <summary>
        /// Calculates the what the distance between the two items' pivots should be.
        /// </summary>
        /// <param name="startwardItem">The item directly start-adjacent to this one.</param>
        /// <param name="axis">The x-axis is 0 (left). The y-axis is 1 (top).</param>
        /// <returns>
        /// The distance between the two items' pivots (in local/unscaled units) for the
        /// left/top direction.
        /// </returns>
        public float GetStartSpacing(Item startwardItem, int axis)
        {
            return ScrollSnapUtilities.CalculateSpacing(startwardItem, this, axis);
        }

        /// <summary>
        /// Calculates what the distance between the two items' pivots should be.
        /// </summary>
        /// <param name="endwardItem">The item directly end-adjacent to this one.</param>
        /// <param name="axis">The x-axis is 0 (left). The y-axis is 1 (top).</param>
        /// <returns>
        /// The distance between the two items' pivots (in local/unscaled units) for the
        /// right/bottom direction.
        /// </returns>
        public float GetEndSpacing(Item endwardItem, int axis)
        {
            return ScrollSnapUtilities.CalculateSpacing(this, endwardItem, axis);
        }
    }
}
