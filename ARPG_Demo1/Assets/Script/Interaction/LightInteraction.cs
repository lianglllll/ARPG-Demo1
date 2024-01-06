using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightInteraction : InteractionBehaviour
{
    private Light _light;

    private void Awake()
    {
        _light = GetComponentInChildren<Light>();
        _light.enabled = false;
    }

    protected override void Interaction()
    {
        if (_light == null) return;
        _light.enabled = !_light.enabled;
    }
}
