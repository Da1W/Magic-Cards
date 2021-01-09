using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Blur : MonoBehaviour
{
    private Volume volume;

    public void Start()
    {
        SceneManager.sceneLoaded += OnLoadScene;
    }
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

    public void OnLoadScene(Scene scene,LoadSceneMode mode)
    {
        volume = gameObject.GetComponent<Volume>();
    }
}
