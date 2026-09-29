using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class HouseRebuildingAmbient : Singleton<HouseRebuildingAmbient>
{
    public List<GameObject> buildingPrefabs = new();
    public SFXBank ambientSounds;
    [Range(0, 1)]
    public float ambientChance;
    public float minWaitTime;
    public float maxWaitTime;
    [SerializeField] private float lastAmbientTime = 0f;

    private void Start()
    {
        StartCoroutine(RebuildingRoutine());
    }

    private IEnumerator RebuildingRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (CheckChance())
            {
                RebuildHouse();
                lastAmbientTime = Time.time;
            }
            yield return null;
        }
    }

    public void RebuildHouse()
    {
        GameObject randPref = buildingPrefabs[Random.Range(0, buildingPrefabs.Count)];
        Transform playerT = PlayerManager.Instance.playerObject.transform;
        int vDirDist = Random.Range(0, 2) == 0
            ? 20
            : -20;
        Vector3 startPos = playerT.position + Vector3.up * vDirDist;
        Vector3 dir = Random.Range(0, 2) == 0
            ? Vector3.forward
            : -Vector3.forward;
        Transform obj = Instantiate(randPref, startPos, Quaternion.identity).transform;
        Vector3 pos2 = startPos + dir * 20f;
        Vector3 pos3 = pos2 + Vector3.down * (vDirDist * 2);
        Vector3 pos4 = pos3 - dir * 20f;
        Sequence seq = DOTween.Sequence();
        Vector3[] path =
        {
            startPos,
            pos2,
            pos3,
            pos4
        };
        obj.DOPath(path, 60f, PathType.CatmullRom).SetEase(Ease.InOutSine);
        obj.DOBlendableLocalMoveBy(Vector3.up * 0.5f, 1f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
        obj.DORotate(
            new Vector3(Random.Range(-5f, 6f), Random.Range(-5f, 6f), 0f),
            4f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
        obj.localRotation = Quaternion.Euler(0, 0, -3);

        obj.DOLocalRotate(
            new Vector3(0, 0, 3),
            6f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        obj.DOScale(1.03f, 2f)
            .SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
        Destroy(obj.gameObject, 60f);
    }

    private bool CheckChance()
    {
        float normalized = 0f;
        if (Time.time > minWaitTime + lastAmbientTime)
        {
            normalized = Mathf.InverseLerp(minWaitTime + lastAmbientTime, maxWaitTime + lastAmbientTime, Time.time);
            if ((ambientChance * normalized) > Random.value) return true;
        }
        return false;
    }
}
