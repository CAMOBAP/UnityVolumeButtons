using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class VolumeText : MonoBehaviour
{
    private Text volumeLevel;

    void Start()
    {
        _UpdateText(gameObject.GetComponent<VolumeButtons>());
    }

    public void OnVolumeUpdated(VolumeButtonsEventType e, VolumeButtons ctrl)
    {
        _UpdateText(ctrl);
    }

    private void _UpdateText(VolumeButtons ctrl)
    {
        Text volumeLevel = gameObject.GetComponent<UnityEngine.UI.Text>();
        string value = ctrl.SystemVolumeLevel.ToString("N2");

        Debug.LogFormat("Volume updated {0} of 1.0", value);

        volumeLevel.text = value;
    }
}
