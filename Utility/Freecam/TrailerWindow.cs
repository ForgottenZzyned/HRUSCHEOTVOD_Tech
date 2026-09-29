using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
public class TrailerWindow : MonoBehaviour
{
    [SerializeField] private Material litMaterial;
    public List<Renderer> windowGlassRend;
    public List<Material> mats;
    private Color baseEmission;
    private void Start()
    {
        if (windowGlassRend[0].sharedMaterial != litMaterial)
        {
            return;
        }
        FreecamManager.Instance.AddWindow(this);
        SerializeMat();
    }
    private void SerializeMat()
    {
        foreach (Renderer r in windowGlassRend)
        {
            mats.Add(r.material);
        }
        baseEmission = mats[0].GetColor("_EmissiveColor");
    }
    private void SetEmission(float value)
    {
        foreach(Material m in mats)
        {
            m.SetColor("_EmissiveColor", baseEmission * value);
        }
    }
    public void TriggerDeactivation()
    {
        StartCoroutine(DeactivateWindow());
    }
    private IEnumerator DeactivateWindow()
    {
        float intensity = 0;
        for (int i = 0; i < 10; i++)
        {
            intensity = Random.Range(0f, 3f);
            SetEmission(intensity);
            yield return new WaitForSeconds(Random.Range(0.03f, 0.07f));
        }   
        float y = 1f;
        DOTween.To(() => y, x => x = y, 0f, 0.25f)
            .OnUpdate(() =>
            {
                SetEmission(y);
            }).OnComplete(() =>
            {
                SetEmission(0f);
            });
    }
}
