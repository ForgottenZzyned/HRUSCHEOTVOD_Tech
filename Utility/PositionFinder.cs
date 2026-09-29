using System.Collections.Generic;
using UnityEngine;

public static class PositionFinder
{ 
    public static WindowPoint FindBestWindowPoint(List<WindowPoint> points,Transform player, float minDist)
    {
        if (points == null) return null;
        if(points.Count == 0) return null;
        Vector3 bestWindow = Vector3.zero;
        WindowPoint bestPoint = null;
        foreach (var p in points)
        {
            if(p.isOccupied) continue;
            if (IsGoodSpot(player, p.transform))
            {
                if (bestWindow == Vector3.zero)
                {
                    bestWindow = p.transform.position;
                    bestPoint = p;
                    continue;
                }
                if (Vector3.Distance(player.position, p.transform.position) <
                    Vector3.Distance(player.position, bestWindow))
                {
                    bestWindow = p.transform.position;
                    bestPoint = p;
                }
            }
        }
        if (minDist < PlayerManager.Instance.GetDist(bestWindow)) return null;
        return bestPoint;
    }
    public static Vector3 FindBestCorner(List<Corner> points, Transform player, Transform entity)
    {
        Vector3 bestCorner = Vector3.zero;
        Corner bestCornerPoint = null;
        foreach (var p in points)
        {
            if (IsGoodSpot(player, p.peekPosition) && !p.isOccupied)
            {
                if(bestCorner == Vector3.zero)
                {
                    bestCorner = p.peekPosition.position;
                    bestCornerPoint = p;
                    continue;
                }
                if(Vector3.Distance(player.position, p.peekPosition.position) <
                    Vector3.Distance(player.position, bestCorner))
                {
                    bestCorner = p.peekPosition.position;
                    bestCornerPoint = p;
                }
            }
        }

        if(bestCorner != Vector3.zero) bestCornerPoint.isOccupied = true;
        return bestCorner;
    }
    public static bool IsVisibleToPlayerDot(Transform player, Transform entity,float dotMin = 0.6f)
    {
        Vector3 dir = (entity.position - player.position).normalized;
        float dot = Vector3.Dot(player.forward, dir);
        Vector3 test = new Vector3(player.position.x, player.position.y + 1f, player.position.z);
        return dot > dotMin;
    }
    public static bool IsVisibleToPlayer(Transform player, Transform entity, float fovModificator = 0)
    {
        float fovAngle = PlayerManager.Instance.playerObject.GetComponentInChildren<Camera>().fieldOfView;
        fovAngle += fovModificator;
        float maxDistance = 100f;
        int rayCount = 33;
        int mask = ~LayerMask.GetMask("IgnoreRaycast");
        Vector3 origin = new Vector3(player.position.x, entity.position.y, player.position.z);
        float halfFOV = fovAngle / 2f;
        
        for (int i = 0; i < rayCount; i++)
        {
            float t = (float)i / (rayCount - 1);
            float angle = Mathf.Lerp(-halfFOV, halfFOV, t);

            Vector3 dir = Quaternion.Euler(0, angle, 0) * player.forward;

            if (Physics.Raycast(origin, dir, out RaycastHit hit, maxDistance, mask, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.CompareTag("Entity"))
                    return true; 
            }
        }
        return false;
    }
    public static Vector3 GetPosInfrontPlayer(Transform player, float dist)
    {
        return player.position + player.forward * dist;
    }
    public static Vector3 GetPosInbackPlayer(Transform player, float dist)
    {
        Vector3 pos = player.position - player.forward * dist;
        return new Vector3(pos.x, player.position.y, pos.z);
    }
    public static bool CanBeSeenIfPlayerTurns(Vector3 pos, Transform player)
    {
        Vector3 dir = (pos - player.position).normalized;
        float angle = Vector3.Angle(player.forward, dir);

        return angle > 80f;
    }
    public static Vector3 FindHiddenSpotDummy(Transform player,float maxDot = 0.3f,float minDist = 20f,float maxDist = 100f)
    {
        Vector3 playerPos = player.position;

        for (int i = 0; i < 30; i++)
        {
            Vector3 dir = Random.onUnitSphere;
            dir.y = 0f;
            dir.Normalize();

            float dist = Random.Range(minDist, maxDist);

            Vector3 randomPos = playerPos + dir * dist;

            Vector3 toPoint = (randomPos - playerPos).normalized;

            float dot = Vector3.Dot(player.forward, toPoint);

            if (dot < maxDot)
                return randomPos;
        }

        return player.position - player.forward * Random.Range(minDist, maxDist);
    }
    public static bool IsGoodSpot(Transform player, Transform entity)
    {
        return !IsVisibleToPlayer(player, entity)
            && CanBeSeenIfPlayerTurns(entity.position,player)
            /*&& HasCover(pos,player)*/;
    }
    public static bool HasCover(Vector3 pos, Transform player)
    {
        Vector3 dirToPlayer = (player.position - pos).normalized;
        if (Physics.Raycast(pos, dirToPlayer, out RaycastHit hit, 2f))
        {
            return true;
        }

        return false;
    }
}
