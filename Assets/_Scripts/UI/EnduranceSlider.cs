using System;
using _Script.Utils;
using UnityEngine;
using UnityEngine.UI;

public class EnduranceSlider : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    private EnduranceSystem _enduranceSystem;

    public void Initialization(EnduranceSystem enduranceSystem)
    {
        _enduranceSystem = enduranceSystem;
        _slider.maxValue = _enduranceSystem.MaxValue;
        _slider.value = _enduranceSystem.CurrentEndurance;
        _enduranceSystem.OnValueChanged += HandleValueChanged;
    }

    private void OnEnable()
    {
        if(_enduranceSystem != null)
            _enduranceSystem.OnValueChanged += HandleValueChanged;
    }

    private void OnDisable()
    {
        if(_enduranceSystem != null)
            _enduranceSystem.OnValueChanged -= HandleValueChanged;
    }

    private void HandleValueChanged()
    {
        _slider.value = _enduranceSystem.CurrentEndurance;
    }
}
