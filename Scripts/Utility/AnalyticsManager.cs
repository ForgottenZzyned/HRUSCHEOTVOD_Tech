using System.Collections.Generic;
using Unity.Services.Core.Environments;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class AnalyticsManager : Singleton<AnalyticsManager>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private async void Start()
    {
        var options = new InitializationOptions();

        options.SetEnvironmentName("production");

        await UnityServices.InitializeAsync(options);

        AnalyticsService.Instance.StartDataCollection();
    }

    public static void StartGame()
    {
        AnalyticsService.Instance.CustomData("started_game");
    }

    public static void RoomEntered(int roomNumber, string roomType)
    {
        var data = new Dictionary<string, object>
        {
            { "room_number", roomNumber },
            { "room_type", roomType },
            { "is_hard", HardmodeManager.isHard }
        };
        AnalyticsService.Instance.CustomData("room_entered",data);
    }

    public static void PlayerDied(int roomNumber, string deathCause)
    {
        var data = new Dictionary<string, object>
        {
            { "room_number", roomNumber },
            { "death_cause", deathCause },
            { "is_hard", HardmodeManager.isHard }
        };
        AnalyticsService.Instance.CustomData("player_died",data);
    }

    public static void ItemUsed(string itemType)
    {
        var data = new Dictionary<string, object>
        {
            { "item_type", itemType }
        };
        AnalyticsService.Instance.CustomData("item_used",data);
    }

    public static void RunStarted()
    {
        var data = new Dictionary<string, object>
        {
            { "is_hard", HardmodeManager.isHard }
        };
        AnalyticsService.Instance.CustomData("run_started", data);
    }

    public static void RunCompleted()
    {
        var data = new Dictionary<string, object>
        {
            { "is_hard", HardmodeManager.isHard }
        };
        AnalyticsService.Instance.CustomData("run_completed", data);
    }
    public static void FirstLaunch()
    {
        AnalyticsService.Instance.CustomData("first_launch");
    }
}
