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

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

namespace ScrollSnaps.Editors
{
    [CustomEditor(typeof(DirectionalScrollSnap))]
    public class DirectionalScrollSnapEditor : Editor
    {
        private SerializedProperty
            m_Layout,
            m_Friction,
            m_ShouldLoop,
            m_EndMargin;

        private AnimBool
            showEndMargin;

        // This makes it easier to modify tooltips in the future.
        private Dictionary<string, string> tooltips = new Dictionary<string, string>
        {
            {"m_Layout", "The direction of movement for the Scroll Snap." },
            {"m_Friction", "The amount of friction to apply to fling gestures.\n" +
                    "A value of 0 would make it fling forever (if this wasn't a scroll snap).\n" +
                    "A value of 1 would make it stop immediately." },
            {"m_ShouldLoop", "Should the items in the scroll snap appear to be in a loop?" },
            {"m_EndMargin", "The margin between the end edge of the endmost item and the " +
                    "start edge of the startmost item." }
        };

        /// <summary>
        /// Called when the component is enabled. Used to initialize the serialized properties of
        /// the editor.
        /// </summary>
        private void OnEnable()
        {
            m_Layout = serializedObject.FindProperty("m_Layout");
            m_Friction = serializedObject.FindProperty("m_Friction");
            m_ShouldLoop = serializedObject.FindProperty("m_ShouldLoop");
            m_EndMargin = serializedObject.FindProperty("m_EndMargin");

            showEndMargin = new AnimBool(m_ShouldLoop.boolValue);
            showEndMargin.valueChanged.AddListener(Repaint);
        }

        /// <summary>
        /// Draws the custom editor.
        /// </summary>
        public override void OnInspectorGUI()
        {
            DirectionalScrollSnap scrollSnap = (DirectionalScrollSnap)target;

            serializedObject.Update();

            EditorGUILayout.PropertyField(m_Layout,
                new GUIContent("Direction", tooltips["m_Layout"]));
            EditorGUILayout.PropertyField(m_Friction,
                new GUIContent("Friction", tooltips["m_Friction"]));
            EditorGUILayout.PropertyField(m_ShouldLoop,
                new GUIContent("Loop Items", tooltips["m_ShouldLoop"]));

            showEndMargin.target = m_ShouldLoop.boolValue;
            if (EditorGUILayout.BeginFadeGroup(showEndMargin.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_EndMargin,
                    new GUIContent("End Margin", tooltips["m_EndMargin"]));
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
