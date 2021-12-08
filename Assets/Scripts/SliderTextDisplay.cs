using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderTextDisplay : MonoBehaviour
{
    private TMP_Text text;

    public Slider slider;
    // get text component
    void Start()
    {
        text = this.GetComponent<TMP_Text>();
    }

    //called when slider value changes
    public void SliderChange()
    {
        text.text = slider.value.ToString();
    }
}
