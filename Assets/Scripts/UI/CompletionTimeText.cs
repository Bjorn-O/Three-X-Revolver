using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CompletionTimeText : MonoBehaviour
{
    private TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();

        text.text = TimeFormatter(GameManager.instance.completionTime);
    }

    private string TimeFormatter(float time)
    {
        var intTime = (int)time;
        var minutes = intTime / 60;
        var seconds = intTime % 60;
        var fraction = time * 1000;
        fraction -= 0.01f;
        fraction = fraction % 1000;
        return $"{minutes:00} : {seconds:00} : {fraction / 10:00}";
    }
}
