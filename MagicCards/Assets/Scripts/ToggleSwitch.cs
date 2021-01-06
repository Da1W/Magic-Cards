using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
public class ToggleSwitch : MonoBehaviour
{
    private int switchState = 1;
    public GameObject switchButton;

    public void OnSwitchClick()
    {
        switchButton.transform.DOLocalMoveX(-switchButton.transform.localPosition.x,0.2f);
        switchState = Math.Sign(-switchButton.transform.localPosition.x);
    }
}
