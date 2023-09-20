using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Michsky.UI.ModernUIPack
{
    public class WindowManager : MonoBehaviour
    {
        // Content
        public List<WindowItem> windows = new List<WindowItem>();

        // Settings
        public int currentWindowIndex = 0;
        int currentButtonIndex = 0, newWindowIndex;
        public string windowFadeIn = "Panel In", windowFadeOut = "Panel Out";
        public string buttonFadeIn = "Normal to Pressed", buttonFadeOut = "Pressed to Dissolve";
        bool isFirstTime = true, cooldown = false;

        GameObject currentWindow, nextWindow, currentButton, nextButton;
        Animator currentWindowAnimator, nextWindowAnimator, currentButtonAnimator, nextButtonAnimator;

        [System.Serializable]
        public class WindowItem
        {
            public string windowName = "My Window";
            public GameObject windowObject, buttonObject;
        }

        void Start()
        {
            try
            {
                currentButton = windows[currentWindowIndex].buttonObject;
                currentButtonAnimator = currentButton.GetComponent<Animator>();
                if (currentButtonAnimator != null) currentButtonAnimator.Play(buttonFadeIn);
            }

            catch { }

            currentWindow = windows[currentWindowIndex].windowObject;
            currentWindowAnimator = currentWindow.GetComponent<Animator>();
            if(currentWindowAnimator != null) currentWindowAnimator.Play(windowFadeIn);
            isFirstTime = false;
        }

        void OnEnable()
        {
            if (isFirstTime == false && nextWindowAnimator == null)
            {
                if (currentWindowAnimator != null) currentWindowAnimator.Play(windowFadeIn);
                if (currentButtonAnimator != null) currentButtonAnimator.Play(buttonFadeIn);
            }

            else if (isFirstTime == false && nextWindowAnimator != null)
            {
                if (nextWindowAnimator != null) nextWindowAnimator.Play(windowFadeIn);
                if (nextButtonAnimator != null) nextButtonAnimator.Play(buttonFadeIn);
            }
        }

        public void OpenFirstTab()
        {
            if (currentWindowIndex != 0)
            {
                currentWindow = windows[currentWindowIndex].windowObject;
                currentWindowAnimator = currentWindow.GetComponent<Animator>();
                if (currentWindowAnimator != null) currentWindowAnimator.Play(windowFadeOut);

                try
                {
                    currentButton = windows[currentWindowIndex].buttonObject;
                    currentButtonAnimator = currentButton.GetComponent<Animator>();
                    if (currentButtonAnimator != null) currentButtonAnimator.Play(buttonFadeOut);
                }

                catch { }

                currentWindowIndex = 0;
                currentButtonIndex = 0;
                currentWindow = windows[currentWindowIndex].windowObject;
                currentWindowAnimator = currentWindow.GetComponent<Animator>();
                if (currentWindowAnimator != null) currentWindowAnimator.Play(windowFadeIn);

                try
                {
                    currentButton = windows[currentButtonIndex].buttonObject;
                    currentButtonAnimator = currentButton.GetComponent<Animator>();
                    if (currentButtonAnimator != null) currentButtonAnimator.Play(buttonFadeIn);
                }

                catch { }
            }

            else if (currentWindowIndex == 0)
            {
                currentWindow = windows[currentWindowIndex].windowObject;
                currentWindowAnimator = currentWindow.GetComponent<Animator>();
                if (currentWindowAnimator != null) currentWindowAnimator.Play(windowFadeIn);

                try
                {
                    currentButton = windows[currentButtonIndex].buttonObject;
                    currentButtonAnimator = currentButton.GetComponent<Animator>();
                    if (currentButtonAnimator != null) currentButtonAnimator.Play(buttonFadeIn);
                }

                catch { }
            }
        }

        public void OpenPanel(string newPanel)
        {
            if(!cooldown)
            {
                cooldown = true;
                for (int i = 0; i < windows.Count; i++)
                    if (windows[i].windowName == newPanel)
                        newWindowIndex = i;

                if (newWindowIndex != currentWindowIndex)
                {
                    currentWindow = windows[currentWindowIndex].windowObject;

                    try
                    {
                        currentButton = windows[currentWindowIndex].buttonObject;
                    }

                    catch { }

                    currentWindowIndex = newWindowIndex;
                    nextWindow = windows[currentWindowIndex].windowObject;
                    currentWindowAnimator = currentWindow.GetComponent<Animator>();
                    nextWindowAnimator = nextWindow.GetComponent<Animator>();
                    if (currentWindowAnimator != null) currentWindowAnimator.Play(windowFadeOut);
                    if (nextWindowAnimator != null) nextWindowAnimator.Play(windowFadeIn);

                    try
                    {
                        currentButtonIndex = newWindowIndex;
                        nextButton = windows[currentButtonIndex].buttonObject;
                        currentButtonAnimator = currentButton.GetComponent<Animator>();
                        nextButtonAnimator = nextButton.GetComponent<Animator>();
                        if (currentButtonAnimator != null) currentButtonAnimator.Play(buttonFadeOut);
                        if (nextButtonAnimator != null) nextButtonAnimator.Play(buttonFadeIn);
                    }

                    catch { }
                }
            }
            StartCoroutine(WaitCooldown(0.1f));
        }

        public void OpenPanelWithDelay(string newPanel) =>  StartCoroutine(WaitUntilCompletion(1, newPanel));

        public void NextPage()
        {
            if (currentWindowIndex <= windows.Count - 2)
            {
                currentWindow = windows[currentWindowIndex].windowObject;

                try
                {
                    currentButton = windows[currentButtonIndex].buttonObject;
                    nextButton = windows[currentButtonIndex + 1].buttonObject;
                    currentButtonAnimator = currentButton.GetComponent<Animator>();
                    if (currentButtonAnimator != null) currentButtonAnimator.Play(buttonFadeOut);
                }

                catch { }

                currentWindowAnimator = currentWindow.GetComponent<Animator>();
                if (currentWindowAnimator != null) currentWindowAnimator.Play(windowFadeOut);
                currentWindowIndex += 1;
                currentButtonIndex += 1;
                nextWindow = windows[currentWindowIndex].windowObject;
                nextWindowAnimator = nextWindow.GetComponent<Animator>();
                if (nextWindowAnimator != null) nextWindowAnimator.Play(windowFadeIn);

                try
                {
                    nextButtonAnimator = nextButton.GetComponent<Animator>();
                    if (nextButtonAnimator != null) nextButtonAnimator.Play(buttonFadeIn);
                }

                catch { }
            }
        }

        public void PrevPage()
        {
            if (currentWindowIndex >= 1)
            {
                currentWindow = windows[currentWindowIndex].windowObject;

                try
                {
                    currentButton = windows[currentButtonIndex].buttonObject;
                    nextButton = windows[currentButtonIndex - 1].buttonObject;
                    currentButtonAnimator = currentButton.GetComponent<Animator>();
                    if (currentButtonAnimator != null) currentButtonAnimator.Play(buttonFadeOut);
                }

                catch { }

                currentWindowAnimator = currentWindow.GetComponent<Animator>();
                if (currentWindowAnimator != null) currentWindowAnimator.Play(windowFadeOut);
                currentWindowIndex -= 1;
                currentButtonIndex -= 1;
                nextWindow = windows[currentWindowIndex].windowObject;
                nextWindowAnimator = nextWindow.GetComponent<Animator>();
                if (nextWindowAnimator != null) nextWindowAnimator.Play(windowFadeIn);

                try
                {
                    nextButtonAnimator = nextButton.GetComponent<Animator>();
                    if (nextButtonAnimator != null) nextButtonAnimator.Play(buttonFadeIn);
                }

                catch { }
            }
        }

        public void AddNewItem()
        {
            WindowItem window = new WindowItem();
            windows.Add(window);
        }

        IEnumerator WaitUntilCompletion(float seconds, string newPanel)
        {
            yield return new WaitForSeconds(seconds);
            OpenPanel(newPanel);
        }
        IEnumerator WaitCooldown(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            cooldown = false;
        }
    }
}