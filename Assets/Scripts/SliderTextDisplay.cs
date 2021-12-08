using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderTextDisplay : MonoBehaviour
{
    private TMP_Text text;

    public Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        text = this.GetComponent<TMP_Text>();
    }

    public void SliderChange()
    {
        text.text = slider.value.ToString();
    }
}
