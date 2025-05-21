using UnityEngine;

namespace _Script.Utils
{
    public class EnduranceSystem
    {
        private float maxValue;
        private float minValue;
        private float rateOfDecrease;
        private float rateOfIncrease;
        public float CurrentEndurance { get; private set; }
        
        public EnduranceSystem(float pMaxValue, float pMinValue, float pRateOfDecrease, float pRateOfIncrease)
        {
            maxValue = pMaxValue;
            minValue = pMinValue;
            rateOfDecrease = pRateOfDecrease;
            rateOfIncrease = pRateOfIncrease;

            CurrentEndurance = maxValue;
        }
        
        public EnduranceSystem(float pMaxValue, float pMinValue, float pRateOfDecrease, float pRateOfIncrease, float pStartValue)
        {
            maxValue = pMaxValue;
            minValue = pMinValue;
            rateOfDecrease = pRateOfDecrease;
            rateOfIncrease = pRateOfIncrease;

            CurrentEndurance = pStartValue;
        }

        public void AddEndurance()
        {
            CurrentEndurance = Mathf.Clamp(CurrentEndurance + rateOfIncrease * Time.deltaTime, minValue, maxValue);
        }
        
        public void ReduceEndurance()
        {
            CurrentEndurance = Mathf.Clamp(CurrentEndurance - rateOfDecrease * Time.deltaTime, minValue, maxValue);
        }

        public void AddValue(float Value)
        {
            CurrentEndurance = Mathf.Clamp(CurrentEndurance + Value, minValue, maxValue);
        }
        
        public void RemoveValue(float Value)
        {
            CurrentEndurance = Mathf.Clamp(CurrentEndurance - Value, minValue, maxValue);
        }
    }
}