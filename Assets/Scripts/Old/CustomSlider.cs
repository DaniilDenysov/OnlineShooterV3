using UnityEngine;
using UnityEngine.UI;

public class CustomSlider : MonoBehaviour
{
    [SerializeField] private Image currentValueImage, oldValueImage;
    [Range(0,float.MaxValue)]
    [SerializeField] private float maxValue=100f,speed = 5f;
    private float currentValue,oldValue,time;

    void Start()
    {
        currentValue = maxValue;
        oldValue = currentValue;
    }

    public float GetCurrentValue() => currentValue;

    public void SetCurrentValue (float value)
    {
        currentValue = Mathf.Clamp(value,0,maxValue);
        time = 0f;
    }

    private void Update()
    {
        if (currentValue != oldValue)
        {
            oldValue = Mathf.Lerp(oldValue, currentValue, time);
            time += speed * Time.deltaTime;
            UpdateVisuals();
        }
    }

    public void UpdateVisuals ()
    {
        currentValueImage.fillAmount = currentValue / maxValue;
        oldValueImage.fillAmount = oldValue / maxValue;
    }
}
