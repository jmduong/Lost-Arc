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
    /// This class contains functions that are not object-specific, but are used by various Scroll
    /// Snap objects.
    /// </summary>
    public static class ScrollSnapUtilities
    {
        private const string ROTATION_WARNING = "Detected rotation on transform: {0}. Margins may not"
            + " be set up as you expect.";
        private const string PARENT_EXCEPTION = "RectTransform {0} and RectTransform {1} must" +
            " have the same parent.";
        private const string PARENT_WARING = "Detected that RectTransform {0} and RectTransform" +
            " {1} have different parents. The resulting margin may not be in the units you expect.";

        /// <summary>
        /// Converts the world point into a point relative to the pivot of the rect.
        /// </summary>
        /// <param name="rect">
        /// The rect whose pivot we want the worldPosition to be relative to.
        /// </param>
        /// <param name="worldPosition">
        /// The point we want to convert.</param>
        /// <returns>
        /// The worldPosition relative to the pivot of the rect. It is in the units of the rect's
        /// children and corners. This includes position, scale, and as rotation.
        /// </returns>
        public static Vector3 WorldPointToLocalPointInRectangle(RectTransform rect,
                Vector3 worldPosition)
        {
            return rect.worldToLocalMatrix.MultiplyPoint3x4(worldPosition);
        }

        /// <summary>
        /// Converts the world point into a point relative to the anchor point of the rect.
        /// </summary>
        /// <param name="rect">
        /// The rect whose anchor point we want the worldPosition to be relative to.
        /// </param>
        /// <param name="worldPoint">The point we want to convert.</param>
        /// <returns>
        /// The worldPosition relative to the anchor point of the rect. It is in the units of the
        /// rect's parent (the same units that the rect's anchoredPosition is in). It has the
        /// rotation of the rect's parent (again the same way the rect's anchoredPosition works).
        /// </returns>
        // We calculate the anchorsPosition by first finding the displacement (in world
        // coordinates) of the rect relative to it's anchor point. We find this by taking the local
        // displacement (i.e. the anchoredPosition of the rect) and then converting the scale and
        // rotation into world coordinates (through maths!). Then we can take the world position of
        // the rect minus the displacement to get the anchorsPosition.
        public static Vector3 WorldPointToRelativeToAnchorPoint(RectTransform rect,
                Vector3 worldPoint)
        {
            // Warning: Rotation on the X & Y axes is ignored.

            // Get the position of the AnchorPoint in world coordinates.
            var displacement = rect.anchoredPosition;
            if (rect.parent)
            {
                var parent = rect.parent;
                displacement.x *= parent.lossyScale.x;
                displacement.y *= parent.lossyScale.y;
                // Convert to world-space rotation (still relative to anchor).
                var rotatedDisplacement = Vector2.zero;
                rotatedDisplacement.x =
                        Mathf.Cos(parent.eulerAngles.z * Mathf.Deg2Rad) * displacement.x
                        - Mathf.Sin(parent.eulerAngles.z * Mathf.Deg2Rad) * displacement.y;
                rotatedDisplacement.y =
                        Mathf.Sin(parent.eulerAngles.z * Mathf.Deg2Rad) * displacement.x
                        + Mathf.Cos(parent.eulerAngles.z * Mathf.Deg2Rad) * displacement.y;
                displacement = rotatedDisplacement;
            }
            var anchorsPosition = rect.position;
            anchorsPosition.x -= displacement.x;
            anchorsPosition.y -= displacement.y;

            // Get the rotation and scale.
            var rotation = Quaternion.identity;
            var scale = Vector3.one;
            if (rect.parent != null)
            {
                rotation = rect.parent.rotation;
                scale = rect.parent.lossyScale;
            }

            var matrix = new Matrix4x4();
            matrix.SetTRS(anchorsPosition, rotation, scale);
            matrix = matrix.inverse;
            return matrix.MultiplyPoint3x4(worldPoint);
        }

        /// <summary>
        /// Calculates the minimum margin between the startward and endward transforms.
        /// </summary>
        /// <param name="startwardTransform">The transform closer to the start direction.</param>
        /// <param name="endwardTransform">The transform closer to the end direction.</param>
        /// <param name="axis">X-axis is 0. Y-axis is 1.</param>
        /// <returns>
        /// The minimum margin between the two transforms in local/unscaled units.
        /// </returns>
        // The minimum margin is calculated by finding the distance (in the parent transform's
        // coordinates) from the startwardTransforms' end-most corner to the endwardTransform's
        // start-most corner.
        public static float CalculateMinimumMargin(RectTransform startwardTransform,
                RectTransform endwardTransform, int axis)
        {
            if (startwardTransform.parent != endwardTransform.parent)
            {
                throw new System.Exception(
                    string.Format(PARENT_EXCEPTION, startwardTransform.name, endwardTransform.name));
            }

            // The margins are calculated as if there no rotation is applied, similar to css.
            Vector3 startWardInitialRotation = CheckRotationForMarginCalculation(startwardTransform);
            Vector3 endwardInitialRotation = CheckRotationForMarginCalculation(endwardTransform);

            Vector3[] startWorldCorners = new Vector3[4];
            startwardTransform.GetWorldCorners(startWorldCorners);
            Vector3[] endWorldCorners = new Vector3[4];
            endwardTransform.GetWorldCorners(endWorldCorners);

            RectTransform parent = (RectTransform)startwardTransform.parent;
            Vector3 startwardEndCorner = ScrollSnapUtilities
                    .WorldPointToLocalPointInRectangle(parent, startWorldCorners[3]);
            Vector3 endwardStartCorner = ScrollSnapUtilities
                    .WorldPointToLocalPointInRectangle(parent, endWorldCorners[1]);

            var margin = Mathf.Abs(startwardEndCorner[axis] - endwardStartCorner[axis]);

            startwardTransform.localEulerAngles = startWardInitialRotation;
            endwardTransform.localEulerAngles = endwardInitialRotation;

            return margin;
        }

        /// <summary>
        /// Calculates what the distance between the startward and endward items' pivots should be
        /// based on their margins.
        /// </summary>
        /// <param name="startwardItem">The item closer to the start direction.</param>
        /// <param name="endwardItem">The item closer to the end direction.</param>
        /// <param name="axis">X-axis is 0. Y-axis is 1.</param>
        /// <returns>
        /// The calculated spacing between the two items in local/unscaled units.
        /// </returns>
        // The spacing is equal to the distance from the startwardItem's pivot to its end edge.
        // Plus whichever margin is greater between the startwardItem's end margin, and the
        // endwardItem's start margin. Plus the distance from the endwardItem's pivot to it's start
        // edge.
        public static float CalculateSpacing(Item startwardItem, Item endwardItem, int axis)
        {
            return CalculateSpacing(
                startwardItem.rectTransform,
                endwardItem.rectTransform,
                Mathf.Max(startwardItem.GetMinimumEndMargin(axis),
                    endwardItem.GetMinimumStartMargin(axis)),
                axis);
        }

        /// <summary>
        /// Calculates what the distance between the startward and endward RectTransforms' pivots
        /// should be based on the given margin.
        /// </summary>
        /// <param name="startwardTransform">The transform closer to the start direction.</param>
        /// <param name="endwardTransform">The transform closer to the end direction.</param>
        /// <param name="margin">
        /// The distance from the startwardTransform's endward edge, to the endwardTrasnform's
        /// startward edge in content coordinates.
        /// </param>
        /// <param name="axis">X-axis is 0. Y-axis is 1.</param>
        /// <returns>
        /// The calculated spacing between the two items in local/unscaled units.
        /// </returns>
        public static float CalculateSpacing(
            RectTransform startwardTransform,
            RectTransform endwardTransform,
            float margin,
            int axis)
        {
            if (startwardTransform.parent != endwardTransform.parent)
            {
                Debug.LogWarningFormat(PARENT_WARING,
                    startwardTransform.name, endwardTransform.name);
            }

            Vector3[] startwardLocalCorners = new Vector3[4];
            startwardTransform.GetLocalCorners(startwardLocalCorners);
            Vector3[] endwardLocalCorners = new Vector3[4];
            endwardTransform.GetLocalCorners(endwardLocalCorners);

            var startwardEndCorner = startwardLocalCorners[3];
            // Convert from item units to parent units.
            startwardEndCorner.x = Mathf.Abs(startwardEndCorner.x)
                    * startwardTransform.localScale.x;
            startwardEndCorner.y = Mathf.Abs(startwardEndCorner.y)
                    * startwardTransform.localScale.y;

            var endwardStartCorner = endwardLocalCorners[1];
            // Convert from item units to parent units.
            endwardStartCorner.x = Mathf.Abs(endwardStartCorner.x)
                    * endwardTransform.localScale.x;
            endwardStartCorner.y = Mathf.Abs(endwardStartCorner.y)
                    * endwardTransform.localScale.y;

            return startwardEndCorner[axis] + margin + endwardStartCorner[axis];
        }


        /// <summary>
        /// Checks if the rectTransform has rotation. Logs a warning if it does, then sets it to 0.
        /// </summary>
        /// <param name="transform">The rectTransform to check for rotation.</param>
        /// <returns>The old rotation of the rectTransform (before it was set to 0.</returns>
        private static Vector3 CheckRotationForMarginCalculation(RectTransform transform)
        {
            if (transform.localEulerAngles != Vector3.zero)
            {
                Debug.LogWarningFormat(transform, ROTATION_WARNING, transform.name);
                var initialRotation = transform.localEulerAngles;
                transform.localEulerAngles = Vector3.zero;
                return initialRotation;
            }
            return Vector3.zero;
        }
    }
}

