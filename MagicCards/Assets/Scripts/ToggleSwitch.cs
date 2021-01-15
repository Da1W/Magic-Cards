using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;
public class ToggleSwitch : MonoBehaviour
{
    private int switchState = 1;
    public GameObject switchButton;

    public void OnSwitchClick()
    {
        switchButton.transform.DOLocalMoveX(-switchButton.transform.localPosition.x,0.2f);
        StartCoroutine(OffInteractableCoroutine());
        switchState = Math.Sign(-switchButton.transform.localPosition.x);
    }

    public IEnumerator OffInteractableCoroutine()
    {
        switchButton.GetComponent<Button>().interactable = false;
        yield return new WaitForSeconds(0.2f);
        switchButton.GetComponent<Button>().interactable = true;
    }
}
