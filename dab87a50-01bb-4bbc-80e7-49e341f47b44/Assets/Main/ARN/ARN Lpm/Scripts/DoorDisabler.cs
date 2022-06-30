using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorDisabler : MonoBehaviour
{
    public GameObject block;
    public GameObject uiBlock;

    public void OnChange(bool isOn)
    {
        block.SetActive(!isOn);
        uiBlock.SetActive(!isOn);
    }
}
