using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ValueSlider : MonoBehaviour
{
    public bool idleStatic = true;

    [SerializeField] private float timeToIdleInSec = 3f;
    [Header("Unlimited Settings")]
    public bool isUnlimited;
    private bool IsUnlimited
    {
        get => isUnlimited;
        set
        {
            isUnlimited = value;
            if (value == false)
            {
                SetUnlimitedVisibility(value,3f);
                return;
            }
            SetUnlimitedVisibility(value);
        }
    }
    [SerializeField] private TMP_Text unlimitedText;
    private CanvasGroup unlimitedCanvGroup;
    [Header("Slider Settings")]
    private Slider slider;
    private CanvasGroup sliderCanvGroup;
    public bool isIdle;
    private bool IsIdle
    {
        get => isIdle;
        set
        {
            isIdle = value;
            SetSliderVisibility(value);
        }
    }

    private float idleTimer;
    private void Awake()
    {
        slider = GetComponent<Slider>();
        sliderCanvGroup = GetComponent<CanvasGroup>();
        unlimitedCanvGroup = unlimitedText.GetComponent<CanvasGroup>();
        SetSliderVisibility(true, 0f);
        SetUnlimitedVisibility(false, 0f);
    }
    private void Start()
    {
        SetSliderVisibility(true, 0f);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) RenewIdle();
        CheckIdle();
    }
    public void Init(float maxValue)
    {
        SetMaxValue(maxValue);
        slider.value = maxValue;
    }
    public void SetSliderValue(float value)
    {
        if (value >= slider.maxValue) return;
        if (value == slider.value && idleStatic) return;
        RenewIdle();
        slider.DOValue(value, 0.05f).SetEase(Ease.InOutQuad);
    }
    public void SetMaxValue(float value)
    {
        slider.maxValue = value;
    }
    public void SetUnlimitedText(float unlimitedTime)
    {
        if(!IsUnlimited)
        {
            RenewIdle();
            IsUnlimited = true;
        };
        if (unlimitedTime > 0) unlimitedText.text = unlimitedTime.ToString("0.#");
        else
        {
            unlimitedText.text = "0";
            IsUnlimited = false;
        }
    }
    private void SetSliderVisibility(bool active, float dur = 0.2f)
    {
        sliderCanvGroup.DOKill();
        if (!PlayerManager.Instance.IsPlayerAlive())
        {
            sliderCanvGroup.DOFade(0f, dur);
            return;
        }
        sliderCanvGroup.DOFade(active ? 0f : 1f, dur);
    }
    private void SetUnlimitedVisibility(bool active, float dur = 0.2f)
    {
        unlimitedCanvGroup.DOKill();
        if (!PlayerManager.Instance.IsPlayerAlive())
        {
            unlimitedCanvGroup.DOFade(0f, dur);
            return;
        }
        unlimitedCanvGroup.DOFade(active ? 1f : 0f, dur).SetEase(Ease.OutQuad);
    }
    private void RenewIdle()
    {
        IsIdle = false;
        idleTimer = timeToIdleInSec;
    }
    private void CheckIdle()
    {
        if (IsIdle) return;

        idleTimer -= Time.deltaTime;

        if (idleTimer <= 0f && !IsUnlimited)
        {
            IsIdle = true;
        }
    }
}
