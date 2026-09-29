using UnityEngine;
public class EnergyManager : Singleton<EnergyManager>
{
    public float maxEnergy = 100f;
    public float drainSpeed = 3f;

    public float regenSpeed = 1.5f;
    public float timeToIdleRegen = 1.5f;

    private bool isIdle = true;
    private float idleTimer;

    private float _currentEnergy;
    private float currentEnergy
    {
        get => _currentEnergy;
        set
        {
            if (value < 0) value = 0;
            else if (value > maxEnergy) value = maxEnergy;
            _currentEnergy = value;
        }
    }
    private ValueSlider energySlider;

    private void Start()
    {
        currentEnergy = maxEnergy;
        energySlider = GetComponentInChildren<ValueSlider>();
        energySlider.Init(maxEnergy);
        energySlider.idleStatic = false;
    }
    private void Update()
    {
        CheckIdle();
        if (isIdle) RegenEnergy();
        energySlider.SetSliderValue(currentEnergy);
    }
    public void SetEnergyDrain(float newValue = 3f)
    {
        drainSpeed = newValue;
    }
    public void AddMaxEnergy(float amountToAdd)
    {
        RenewIdle();
        maxEnergy += amountToAdd;
        energySlider.SetMaxValue(maxEnergy);
        energySlider.SetSliderValue(currentEnergy);
    }
    public void AddToCurrEnergy(float addValue)
    {
        currentEnergy += addValue;
        if(currentEnergy > maxEnergy) currentEnergy = maxEnergy;
        energySlider.SetSliderValue(currentEnergy);
    }
    public float GetEnergy() => currentEnergy;
    public void DrainEnergy(float multiplier = 1f)
    {
        RenewIdle();
        currentEnergy -= (Time.deltaTime * drainSpeed) * multiplier;
    }
    public bool CheckEnergy()
    {
        return currentEnergy > 0f;
    }
    private void RenewIdle()
    {
        if (isIdle) isIdle = false;
        idleTimer = timeToIdleRegen;
    }
    private void RegenEnergy()
    {
        if (currentEnergy >= maxEnergy) return;
        currentEnergy += Time.deltaTime * regenSpeed;
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

