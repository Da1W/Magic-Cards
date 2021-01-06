using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Blur : MonoBehaviour
{
    [SerializeField] Volume volume;
    public void BlurOn()
    {
        DepthOfField dof;
        if(volume.profile.TryGet(out dof))
        {
            dof.active = true;
        }
    }

    public void BlurOff()
    {
        DepthOfField dof;
        if (volume.profile.TryGet(out dof))
        {
            dof.active = false;
        }
    }
}
