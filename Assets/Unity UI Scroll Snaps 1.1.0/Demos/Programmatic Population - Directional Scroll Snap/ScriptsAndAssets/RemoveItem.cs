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

using System.Collections;
using System.Linq;
using UnityEngine;
using ScrollSnaps;

public class RemoveItem : MonoBehaviour
{
    public DirectionalScrollSnap m_ScrollSnap;
    public GameObject m_ItemPrefab;

    private const string ITEM_NAME = "Item ({0})";
    private const float MARGIN = 50;

    // This is called by the button.
    public void Remove()
    {
        InternalRemove();
    }

    // This is called by the other button.
    public void RemoveDelayed()
    {
        StartCoroutine(DelayRemoving());
    }

    // This allows us to wait three seconds before calling insert.
    private IEnumerator DelayRemoving()
    {
        yield return new WaitForSeconds(3);
        InternalRemove();
    }

    private void InternalRemove()
    {
        // Note that we have to include the "System.Linq" using directive to count the items.
        // Only remove the end item if it exists.
        if (m_ScrollSnap.items.Count() > 0)
        {
            // Remove the endmost item.
            m_ScrollSnap.RemoveItemAtEnd();
        }
    }
}
