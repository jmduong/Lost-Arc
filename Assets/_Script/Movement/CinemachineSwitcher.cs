using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CinemachineSwitcher : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera vcam;
    [SerializeField]
    private CinemachineFreeLook fcam;

    public void SwitchPriority(bool option)
    {
        if(option)
        {
            vcam.Priority = 1;
            fcam.Priority = 0;
        }
        else
        {
            vcam.Priority = 0;
            fcam.Priority = 1;
        }
    }

    IEnumerator WaitUntilCompletion(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}
