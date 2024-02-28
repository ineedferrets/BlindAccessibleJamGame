using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderReader : Slider
{
    protected override void Start()
    {
        base.Start();

        onValueChanged.AddListener(OnSliderValueChange);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);

        TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
        if (text && Application.platform != RuntimePlatform.WebGLPlayer)
        {
            int sliderPercentage = (int)(value * 100);
            ScreenReader.StaticReadText("Slider. " + text.text + " at value " + sliderPercentage + "%.");
        }
    }

    private void OnSliderValueChange(float newValue)
    {
        TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
        if (text && Application.platform != RuntimePlatform.WebGLPlayer)
        {
            int sliderPercentage = (int)(newValue * 100);
            ScreenReader.StaticReadText("Slider. " + text.text + " changed to value " + sliderPercentage + "%.");
        }
    }
}
