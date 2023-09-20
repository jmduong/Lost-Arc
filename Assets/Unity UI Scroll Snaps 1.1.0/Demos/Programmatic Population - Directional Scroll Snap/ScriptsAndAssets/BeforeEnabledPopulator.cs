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
using ScrollSnaps;

public class BeforeEnabledPopulator : MonoBehaviour
{
    public DirectionalScrollSnap m_ScrollSnap;
    public GameObject m_ItemPrefab;
    public int m_ItemsToAdd;

    private const string ITEM_NAME = "Item ({0})";
    private const float MARGIN = 50;

    // Awake is called before OnEnabled, which is what we want.
    public void Awake()
    {
        // Create as many items as specified, starting at index 0.
        for (int i = 0; i < m_ItemsToAdd; i++)
        {
            // Instantiate the item, meaning create a duplicate from the prefab.
            var itemObject = Instantiate(m_ItemPrefab);
            // Cache the RectTransform because we will be using it twice.
            var itemTransform = itemObject.GetComponent<RectTransform>();
            // Make sure the item's inverse-axis aligns with the scroll snap's inverse-axis, the
            // other axis will be changed to suit the margins.
            itemTransform.position = m_ScrollSnap.rectTransform.position;
            // Set the item's name.
            itemObject.name = string.Format(ITEM_NAME, i);
            // Set the text of the item's child text object.
            itemTransform.GetChild(0).GetComponent<Text>().text = i.ToString();
            // Add the item to the scroll snap.
            m_ScrollSnap.AddItemAtEnd(itemTransform, MARGIN, MARGIN);
        }
    }
}
