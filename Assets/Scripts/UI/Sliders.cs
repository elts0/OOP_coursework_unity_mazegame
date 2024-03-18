using System;
using TMPro;
using UnityEngine;

public class Sliders : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _rowCountText;
    [SerializeField] private TextMeshProUGUI _columnCountText;
    public void OnSliderRowChange(float value)
    {
        int rowCnt = (int)value;
        if (rowCnt <= 0 || rowCnt >= 100)
        {
            throw new Exception("Slider value out of range");
        }

        _rowCountText.text = "Rows: " + rowCnt.ToString();
    }

    public void OnSliderColumnChange(float value)
    {
        int colCnt = (int)value;
        if (colCnt <= 0 || colCnt >= 100)
        {
            throw new Exception("Slider value out of range");
        }

        _columnCountText.text = "Columns: " + colCnt.ToString();
    }
}
