using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StaminaManager : Singleton<StaminaManager>
{
    public float maxStamina = 100f; 
    public float drainSpeed = 2f;

    public float regenSpeed = 1.5f;
    public float timeToIdleRegen = 1.5f;

    public float unlimitedTimer = 0;

    private bool isIdle = true;
    private float idleTimer;

    private float currentStamina;
    private ValueSlider staminaSlider;

    private void Start()
    {
        currentStamina = maxStamina;
        staminaSlider = GetComponentInChildren<ValueSlider>();
        staminaSlider.Init(maxStamina);
    }
    private void Update()
    {
        CheckIdle();
        if(unlimitedTimer > 0)
        {
            unlimitedTimer -= Time.deltaTime;
            staminaSlider.SetUnlimitedText(unlimitedTimer);
        }
        if (isIdle) RegenStamina();
        staminaSlider.SetSliderValue(currentStamina);
    }
    public void AddMaxStamina(float amountToAdd)
    {
        RenewIdle();
        maxStamina += amountToAdd;
        staminaSlider.SetMaxValue(maxStamina);
        staminaSlider.SetSliderValue(currentStamina);
    }
    public void AddRegenSpeed(float amountToAdd)
    {
        RenewIdle();
        regenSpeed += amountToAdd;
    }
    public void AddUnlimitedStamina(float timeToAdd)
    {
        RenewIdle();
        unlimitedTimer += timeToAdd;
    }
    public void DrainStamina(float multiplier = 1f)
    {
        if (unlimitedTimer > 0) return;
        RenewIdle();
        currentStamina -= (Time.deltaTime * drainSpeed) * multiplier;
    }
    public bool CheckStamina()
    {
        return currentStamina > 0f;
    }
    private void RenewIdle()
    {
        if (isIdle) isIdle = false;
        idleTimer = timeToIdleRegen;
    }
    private void RegenStamina()
    {
        if (currentStamina >= maxStamina) return;
        currentStamina += Time.deltaTime * regenSpeed;
    }
    private void CheckIdle()
    {
        if (isIdle) return;

        idleTimer -= Time.deltaTime;

        if (idleTimer <= 0f)
        {
            isIdle = true;
        }
    }
}
