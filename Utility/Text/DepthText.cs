using TMPro;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
public class DepthText : MonoBehaviour
{
    public TextMeshPro textPrefab;
    public int depth = 5;
    public float offset = 0.01f;
    public float colorOffset = 0.01f;

    private List<TextMeshPro> layers = new();
    public void RemoveText()
    {
        GetComponent<TextMeshPro>().DOFade(0, 0.35f).SetEase(Ease.InOutQuad);
        foreach (var layer in layers)
        {
            layer.DOFade(0,0.35f).SetEase(Ease.InOutQuad);
            Debug.Log($"removing text for {layer.name}");
        }
    }
    public void SetText(string value)
    {
        foreach (var t in layers)
            Destroy(t.gameObject);
        layers.Clear();
        GetComponent<TMP_Text>().text = value;
        for (int i = 0; i < depth; i++)
        {
            var t = Instantiate(textPrefab, transform);
            t.text = value;
            t.color = GetComponent<TMP_Text>().color;
            t.transform.localPosition = new Vector3(0, 0, i * offset);

            float shadeR = t.color.r - (i * colorOffset);
            float shadeG = t.color.g - (i * colorOffset);
            float shadeB = t.color.b - (i * colorOffset);
            t.color = new Color(shadeR, shadeG, shadeB);

            layers.Add(t);
        }
    }
}
