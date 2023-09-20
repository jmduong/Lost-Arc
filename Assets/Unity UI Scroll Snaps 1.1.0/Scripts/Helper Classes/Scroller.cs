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
    /// This class encapsulates scrolling. It tracks scroll offsets over time for you, but it is
    /// your job to get and apply new coordinates.
    /// </summary>
    public class Scroller
    {
        /// <summary>
        /// Has the animation reached its end. Velocity = 0 in the case of a fling.
        /// </summary>
        private bool m_Finished = true;
        /// <summary>
        /// The Time.time of the last time ComputeScrollPosition was called.
        /// </summary>
        private float m_LastComputeTime;
        /// <summary>
        /// The percentage of the initial velocity that should remain after one second.
        /// </summary>
        private float m_DecelerationRate;
        /// <summary>
        /// The latest velocity of the animation.
        /// </summary>
        private Vector2 m_CurrentVelocity;
        /// <summary>
        /// The initial position of the object being animated (space-less, because it is applied
        /// by other classes/components).
        /// </summary>
        private Vector2 m_InitialPosition;
        /// <summary>
        /// The position to set the currentPosition to at the end of the animation, aka the
        /// position the object should reach by the end of the animation.
        /// </summary>
        private Vector2 m_FinalPosition;
        private Vector2 m_CurrentPosition;
        /// <summary>
        /// The currentPosition of the object being animated. Used by other classes/components
        /// to apply the animation.
        /// </summary>
        public Vector2 currentPosition { get { return m_CurrentPosition; } }
        /// <summary>
        /// The time since the last time ComputeScrollPosition was called.
        /// </summary>
        private float m_DeltaTime { get { return Time.time - m_LastComputeTime; } }
        /// <summary>
        /// Is the animation completed?
        /// </summary>
        public bool isFinished { get { return m_Finished; } }

        /// <summary>
        /// Calculates the amount of movement for a given initial velocity and deceleration rate.
        /// </summary>
        /// <param name="initialVelocity">The initial velocity in units per second.</param>
        /// <param name="decelerationRate">
        /// The percentage of the velocity that remains after one second.
        /// </param>
        /// <returns>The amount of units that an object with these parameters would move.</returns>
        public float CalculateMovementDelta(float initialVelocity, float decelerationRate)
        {
            if (initialVelocity == 0)
            {
                return 0;
            }
            return (1 - initialVelocity) / Mathf.Log(decelerationRate);
        }

        /// <summary>
        /// Calculates the deceleration rate you would need to reach the movementDelta given the
        /// initialVelocity.
        /// </summary>
        /// <param name="initialVelocity">The initial velocity in units per second.</param>
        /// <param name="movementDelta">The amount in units that you want the object to move.</param>
        /// <returns>The precentage of the velocity that would remain after one second.</returns>
        public float CalculateDecelerationRate(float initialVelocity, float movementDelta)
        {
            if (initialVelocity == 0)
            {
                return 0;
            }
            return Mathf.Exp((1 - initialVelocity) / movementDelta);
        }

        /// <summary>
        /// Calculates the duration of movement for a given initial velocity and deceleration rate.
        /// </summary>
        /// <param name="initialVelocity">The initial velocity in units per second.</param>
        /// <param name="decelerationRate">
        /// The percentage of the velocity that remains after one second.
        /// </param>
        /// <returns>
        /// The time in seconds an object with these parameters would move before stopping.
        /// </returns>
        public float CalculateDuration(float initialVelocity, float decelerationRate)
        {
            if (initialVelocity == 0)
            {
                return 0;
            }
            return Mathf.Log(1 / initialVelocity) / Mathf.Log(decelerationRate);
        }

        /// <summary>
        /// Starts a fling animation.
        /// </summary>
        /// <param name="initialVelocity">The initial velocity in units per second.</param>
        /// <param name="currentPosition">The initial position of the object being flung.</param>
        /// <param name="decelerationRate">
        /// The percentage of the velocity that remains after one second.
        /// </param>
        public void StartFling(Vector2 initialVelocity, Vector2 currentPosition,
                float decelerationRate)
        {
            if (initialVelocity.magnitude == 0)
            {
                // No movement.
                return;
            }

            m_LastComputeTime = Time.time;
            m_CurrentVelocity = initialVelocity;
            m_InitialPosition = currentPosition;
            m_CurrentPosition = currentPosition;
            m_FinalPosition = new Vector2(
                    currentPosition.x + CalculateMovementDelta(initialVelocity.x, decelerationRate),
                    currentPosition.y + CalculateMovementDelta(initialVelocity.y, decelerationRate));
            m_DecelerationRate = decelerationRate;
            m_Finished = false;
        }

        /// <summary>
        /// Stops any running animation.
        /// </summary>
        public void StopAnimation()
        {
            m_Finished = true;
        }

        /// <summary>
        /// Shifts the animation by the given offset. Used when the object being animated moves,
        /// but the animation should remain smooth.
        /// </summary>
        /// <param name="offset"></param>
        public void ShiftAnimation(Vector2 offset)
        {
            m_InitialPosition += offset;
            m_CurrentPosition += offset;
            m_FinalPosition += offset;
        }

        /// <summary>
        /// Calculates the current position of the object, based on the time since the animation
        /// was started.
        /// </summary>
        /// <returns>The current position of the animation.</returns>
        public bool ComputeScrollPosition()
        {
            if (m_Finished)
            {
                return false;
            }
            Vector2 previousVelocity = m_CurrentVelocity;
            m_CurrentVelocity *= Mathf.Pow(m_DecelerationRate, m_DeltaTime);
            Vector2 averageVelocity = m_CurrentVelocity + (previousVelocity - m_CurrentVelocity) / 2;

            if (Mathf.Abs(m_CurrentVelocity.x) < 1)
            {
                m_CurrentVelocity.x = 0;
            }
            if (Mathf.Abs(m_CurrentVelocity.y) < 1)
            {
                m_CurrentVelocity.y = 0;
            }
            
            m_CurrentPosition += averageVelocity * m_DeltaTime;
            ClampToFinalPosition();
            if (m_CurrentVelocity == Vector2.zero)
            {
                // Because negative velocities tend to undershoot, we'll set it just in case.
                m_CurrentPosition = m_FinalPosition;
                m_Finished = true;
            }
            m_LastComputeTime = Time.time;
            return true;
        }

        /// <summary>
        /// Keeps the current position from overshooting the final position. This can happen if
        /// there is a large delta time between calls to ComputeScrollPosition.
        /// </summary>
        private void ClampToFinalPosition()
        {
            // If the current position has passed beyond the final position.
            if (m_InitialPosition.x < m_FinalPosition.x
                    && m_FinalPosition.x < m_CurrentPosition.x
                    || m_InitialPosition.x > m_FinalPosition.x
                    && m_FinalPosition.x > m_CurrentPosition.x)
            {
                m_CurrentPosition.x = m_FinalPosition.x;
            }

            if (m_InitialPosition.y < m_FinalPosition.y
                    && m_FinalPosition.y < m_CurrentPosition.y
                    || m_InitialPosition.y > m_FinalPosition.y
                    && m_FinalPosition.y > m_CurrentPosition.y)
            {
                m_CurrentPosition.y = m_FinalPosition.y;
            }

            if (m_CurrentPosition == m_FinalPosition)
            {
                m_Finished = true;
            }
        }
    }
}
