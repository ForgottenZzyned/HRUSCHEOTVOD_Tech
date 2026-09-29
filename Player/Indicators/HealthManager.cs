using DG.Tweening;
using System;
using UnityEngine;
public class HealthManager : Singleton<HealthManager>
{
    public float maxHealth = 100f;

    public float regenSpeed = 1.5f;
    public float timeToIdleRegen = 1.5f;

    private bool isIdle = true;
    private float idleTimer;

    private float currentHealth;
    private ValueSlider healthSlider;

    private void Start()
    {
        currentHealth = maxHealth;
        healthSlider = GetComponentInChildren<ValueSlider>();
        healthSlider.Init(maxHealth);
        healthSlider.idleStatic = false;
    }

    private void Update()
    {
        CheckIdle();
        if (isIdle) RegenHealth();
        healthSlider.SetSliderValue(currentHealth);
    }

    public void AddHealth(float amountToAdd)
    {
        if (amountToAdd < 0) 
        {
            PlayerManager.Instance.HitPlayer(amountToAdd * -1, "Died to unsuspected reasons");
            return;
        };
        float newNum = currentHealth + amountToAdd;
        if(newNum > maxHealth) newNum = maxHealth;
        DOTween.To(
                () => currentHealth,
                x => currentHealth = x,
                newNum,
                0.15f
            );
        if(currentHealth > maxHealth) currentHealth = maxHealth;
    }

    public void SetCurrHealth(float value)
    {
        DOTween.To(
                () => currentHealth,
                x => currentHealth = x,
                value,
                0.15f
            );
        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    public void AddMaxHealth(float amountToAdd)
    {
        RenewIdle();
        maxHealth += amountToAdd;
        healthSlider.SetMaxValue(maxHealth);
        healthSlider.SetSliderValue(currentHealth);
    }

    public void AddRegenSpeed(float amountToAdd)
    {
        RenewIdle();
        regenSpeed += amountToAdd;
    }

    public void DecreaseHealth(float amount = 0f, string cause = "null")
    {
        RenewIdle();
        currentHealth -= amount;
        if (currentHealth <= 0) PlayerManager.Instance.KillPlayer(cause);
    }

    public float GetCurrHealth() => currentHealth;

    private void RenewIdle()
    {
        if (isIdle) isIdle = false;
        idleTimer = timeToIdleRegen;
    }

    private void RegenHealth()
    {
        if (currentHealth >= maxHealth) return;
        currentHealth += Time.deltaTime * regenSpeed;
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
