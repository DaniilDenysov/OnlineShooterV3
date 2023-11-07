using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.Events;

public class SynchronizedCustomSlider : NetworkBehaviour
{
    [SerializeField] private Image currentValueImage, oldValueImage;
    [Range(0, float.MaxValue)]
    [SerializeField] private float maxValue = 100f, speed = 5f,regenerationSpeed=5f;
    [SyncVar(hook = nameof(OnCurrentValueChange))]
    private float currentValue, oldValue;
    [SyncVar]
    private float time;
    [SerializeField] private UnityEvent OnSliderValueChanged;
    [SerializeField] private bool regenerate;

    [Server]
    void Start()
    {
        currentValue = maxValue;
        oldValue = currentValue;
    }

    public float GetCurrentValue() => currentValue;

    [Server]
    public void SetCurrentValue(float value)
    {
        currentValue = Mathf.Clamp(value, 0, maxValue);
        time = 0f;
        OnSliderValueChanged?.Invoke();
    }

    [Server]
    private void Update()
    {
        if (currentValue != oldValue && isServer)
        {
            oldValue = Mathf.Lerp(oldValue, currentValue, time);
            time += speed * Time.deltaTime;
        }
        else
        {
            if (currentValue == maxValue) return;
            if (!regenerate) return;
            currentValue = Mathf.Lerp(currentValue, Mathf.Clamp(currentValue+regenerationSpeed, 0, maxValue), time);
            oldValue = currentValue;
            time += speed * Time.deltaTime;
        }
        UpdateVisuals();
    }

    [Client]
    private void OnCurrentValueChange(float oldValue, float newValue)
    {
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        currentValueImage.fillAmount = currentValue / maxValue;
        oldValueImage.fillAmount = oldValue / maxValue;
    }
}
