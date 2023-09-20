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
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.EventSystems;

namespace ScrollSnaps.Editors
{
    public class DirectionalScrollSnapMenuOption
    {
        /// <summary>
        /// Adds a menu option on the Hierarchy > Create button for creating a Directional Scroll
        /// Snap object.
        /// </summary>
        [MenuItem("GameObject/UI/ScrollSnaps/DirectionalScrollSnap", false, 10)]
        private static void CreateDirectionalScrollSnap(MenuCommand menuCommand)
        {
            GameObject parent = menuCommand.context as GameObject;

            if (parent == null || parent.GetComponentInParent<Canvas>() == null)
            {
                Canvas canvas = Object.FindObjectOfType<Canvas>();
                if (canvas == null || !canvas.gameObject.activeInHierarchy)
                {
                    parent = new GameObject("Canvas");
                    parent.layer = LayerMask.NameToLayer("UI");
                    canvas = parent.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    parent.AddComponent<CanvasScaler>();
                    parent.AddComponent<GraphicRaycaster>();
                    Undo.RegisterCreatedObjectUndo(parent, "Create " + parent.name);

                    EventSystem eventSystem = Object.FindObjectOfType<EventSystem>();
                    if (eventSystem == null || !eventSystem.gameObject.activeInHierarchy)
                    {
                        GameObject eventSystemGameObject = new GameObject("EventSystem");
                        eventSystemGameObject.AddComponent<EventSystem>();
                        eventSystemGameObject.AddComponent<StandaloneInputModule>();

                        Undo.RegisterCreatedObjectUndo(eventSystemGameObject,
                            "Create " + eventSystemGameObject.name);
                    }
                }
                else
                {
                    parent = canvas.gameObject;
                }
            }
            
            GameObject directionalScrollSnapGameObject = new GameObject("Directional Scroll Snap");
            RectTransform scrollSnapRect = directionalScrollSnapGameObject.AddComponent<RectTransform>();
            Image image = directionalScrollSnapGameObject.AddComponent<Image>();
            directionalScrollSnapGameObject.AddComponent<DirectionalScrollSnap>();

            scrollSnapRect.SetParent(parent.transform, false);
            scrollSnapRect.sizeDelta = new Vector2(200, 200);
            image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            image.type = Image.Type.Sliced;
            image.color = Color.red;

            GameObject contentGameObject = new GameObject("Content");
            RectTransform contentRectTransform = contentGameObject.AddComponent<RectTransform>();
            Image contentImage = contentGameObject.AddComponent<Image>();

            contentRectTransform.SetParent(directionalScrollSnapGameObject.transform, false);
            contentRectTransform.sizeDelta = new Vector2(350, 200);
            contentImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            contentImage.type = Image.Type.Sliced;
            contentImage.color = new Color(0, 0, 1, .5f);
            
            GameObject itemGameObject1 = new GameObject("Item (1)");
            RectTransform itemRectTransform1 = itemGameObject1.AddComponent<RectTransform>();
            itemGameObject1.AddComponent<Image>();

            itemRectTransform1.SetParent(contentGameObject.transform, false);
            // It isn't neccessary to set the anchors this way, it just makes layouting easier.
            itemRectTransform1.anchorMin = new Vector2(0, 1);
            itemRectTransform1.anchorMax = new Vector2(0, 1);
            itemRectTransform1.sizeDelta = new Vector2(100, 100);
            itemRectTransform1.anchoredPosition = new Vector2(100, -100);

            GameObject itemGameObject2 = new GameObject("Item (2)");
            RectTransform itemRectTransform2 = itemGameObject2.AddComponent<RectTransform>();
            itemGameObject2.AddComponent<Image>();

            itemRectTransform2.SetParent(contentGameObject.transform, false);
            // It isn't neccessary to set the anchors this way, it just makes layouting easier.
            itemRectTransform2.anchorMin = new Vector2(0, 1);
            itemRectTransform2.anchorMax = new Vector2(0, 1);
            itemRectTransform2.sizeDelta = new Vector2(100, 100);
            itemRectTransform2.anchoredPosition = new Vector2(250, -100);

            GameObjectUtility.SetParentAndAlign(directionalScrollSnapGameObject, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(directionalScrollSnapGameObject, "Create " + directionalScrollSnapGameObject.name);
            Selection.activeObject = directionalScrollSnapGameObject;


            // Find the best scene view
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView == null && SceneView.sceneViews.Count > 0)
            {
                sceneView = SceneView.sceneViews[0] as SceneView;
            }

            // Couldn't find a SceneView. Don't set position.
            if (sceneView == null || sceneView.camera == null)
            {
                return;
            }

            // Create world space Plane from canvas position.
            RectTransform canvasRTransform = parent.GetComponent<RectTransform>();
            Vector2 localPlanePosition;
            Camera camera = sceneView.camera;
            Vector3 position = Vector3.zero;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRTransform, new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2), camera, out localPlanePosition))
            {
                // Adjust for canvas pivot
                localPlanePosition.x = localPlanePosition.x + canvasRTransform.sizeDelta.x * canvasRTransform.pivot.x;
                localPlanePosition.y = localPlanePosition.y + canvasRTransform.sizeDelta.y * canvasRTransform.pivot.y;

                localPlanePosition.x = Mathf.Clamp(localPlanePosition.x, 0, canvasRTransform.sizeDelta.x);
                localPlanePosition.y = Mathf.Clamp(localPlanePosition.y, 0, canvasRTransform.sizeDelta.y);

                // Adjust for anchoring
                position.x = localPlanePosition.x - canvasRTransform.sizeDelta.x * scrollSnapRect.anchorMin.x;
                position.y = localPlanePosition.y - canvasRTransform.sizeDelta.y * scrollSnapRect.anchorMin.y;

                Vector3 minLocalPosition;
                minLocalPosition.x = canvasRTransform.sizeDelta.x * (0 - canvasRTransform.pivot.x) + scrollSnapRect.sizeDelta.x * scrollSnapRect.pivot.x;
                minLocalPosition.y = canvasRTransform.sizeDelta.y * (0 - canvasRTransform.pivot.y) + scrollSnapRect.sizeDelta.y * scrollSnapRect.pivot.y;

                Vector3 maxLocalPosition;
                maxLocalPosition.x = canvasRTransform.sizeDelta.x * (1 - canvasRTransform.pivot.x) - scrollSnapRect.sizeDelta.x * scrollSnapRect.pivot.x;
                maxLocalPosition.y = canvasRTransform.sizeDelta.y * (1 - canvasRTransform.pivot.y) - scrollSnapRect.sizeDelta.y * scrollSnapRect.pivot.y;

                position.x = Mathf.Clamp(position.x, minLocalPosition.x, maxLocalPosition.x);
                position.y = Mathf.Clamp(position.y, minLocalPosition.y, maxLocalPosition.y);
            }

            scrollSnapRect.anchoredPosition = position;
            scrollSnapRect.localRotation = Quaternion.identity;
            scrollSnapRect.localScale = Vector3.one;
        }
    }
}
