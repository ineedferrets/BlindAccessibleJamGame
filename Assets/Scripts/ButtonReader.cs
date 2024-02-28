using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonReader : Button
{
    override public void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);

        TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
        Debug.Log(text);
        if (text)
        {
            //ScreenReader.StaticReadText(text.text);
        }
    }
}
