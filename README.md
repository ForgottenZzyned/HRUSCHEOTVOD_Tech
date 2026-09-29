# HRUSCHEOTVOD — Technical Overview

Technical showcase of the core systems used in **HRUSCHEOTVOD**, a procedural liminal horror game built with Unity and C#.

The repository focuses on the game's gameplay architecture, entity state machines, procedural room generation and supporting systems.

## Architecture

The project is organized around independent gameplay systems and managers rather than a single monolithic controller.

Core systems communicate through dedicated managers and lightweight event-driven communication where appropriate.

```text
Gameplay
??? Entity System
?   ??? Entity
?   ??? Entity State Machine
?   ??? Entity Behaviours
?   ??? EntityData
?
??? Procedural Generation
?   ??? RoomChainManager
?   ??? Room
?   ??? RoomData
?   ??? Door / DoorSocket
?   ??? RoomDeletionManager
?
??? Managers
?   ??? EntitiesManager
?   ??? PlayerManager
?   ??? AtmosphereManager
?   ??? AnalyticsManager
?
??? Utility
    ??? GameEvents
    ??? Singleton
    ??? PositionFinder
```

The architecture separates **gameplay logic, entity behaviour, state transitions and procedural generation**, making individual systems easier to extend without modifying unrelated parts of the game.

---

## Entity State Machine

Entities use a dedicated **State Machine architecture**.

The `Entity` component owns the currently active `IEntityState` and is responsible for state transitions:

```text
Entity
   ?
   ??? currentState : IEntityState
   ?
   ??? SetState()
   ?
   ??? Update()
          ?
          ?
    IEntityState
          ?
    ?????????????????????
    ?     ?             ?
 Walker  Watcher      Fracture
 State   State         State
    ?
    ??? ...additional states
```

Every state follows the same lifecycle:

```csharp
Enter(Entity entity)
Update()
Exit()
```

This keeps entity-specific behaviour out of the main `Entity` class and allows different enemies to reuse the same state-machine infrastructure.

The state machine is located under:

`Scripts/Entity/`

with the main entry point:

`Scripts/Entity/Entity.cs`

and the state contract:

`Scripts/Entity/IEntityState.cs`

States are grouped under:

`Scripts/Entity/States/`

The project also separates **behaviour** from **state**. `EntityBehaviour` implementations contain entity-specific behaviour/setup, while states control the entity's current gameplay state.

Entity configuration is stored separately through `EntityData` ScriptableObjects:

`Scripts/Entity/EntityData.cs`

This allows prefab, damage, spawn conditions, weights and behaviour references to be configured independently from the entity's runtime logic.

---

## Procedural Generation

The game's environment is generated dynamically as a chain of room prefabs.

The main system responsible for this is:

`Scripts/RoomsGeneration/RoomChainManager.cs`

`RoomChainManager` maintains a linked list of currently active rooms and generates new rooms as the player progresses.

```text
Start Room
    ?
    ?
???????????
? Room 01 ?
???????????
     ?
     ?
???????????
? Room 02 ?
???????????
     ?
     ?
???????????
? Room 03 ?
???????????
     ?
    ...
```

Rooms are connected through `DoorSocket` objects. During generation the system:

1. Selects a compatible room prefab.
2. Finds a socket with the required direction.
3. Calculates the required rotation and position.
4. Aligns the new room with the previous room.
5. Connects the two rooms with a generated door.
6. Adds the room to the active room chain.

Room selection uses several parameters:

* **Socket direction** — only rooms with a compatible entry socket can be selected.
* **Weight** — controls the probability of selecting a room.
* **Room threshold** — prevents certain rooms from appearing before the required progression point.
* **Room type** — supports `Basic`, `Unique` and `Ending` rooms.

This creates a weighted procedural generation system while still allowing specific rooms and progression events to be controlled.

---

## Special Generation Chains

The generation system also supports special room sequences.

`RoomChainManager` uses a generation state:

```text
Normal
   ?
   ?
Unique Room
   ?
   ?
WaitingForUniqueDecision
   ?
   ??? Correct Door ??? Normal
   ?
   ??? Wrong Door ????? SpecialChain
                           ?
                           ?
                     Special Rooms
                           ?
                           ?
                         Normal
```

This allows procedural generation to coexist with authored gameplay sequences.

For example, a `Unique` room can temporarily take control of the generation flow. The player's door choice can then trigger a special chain of rooms before returning to normal procedural generation.

---

## Room Lifecycle & Memory Management

The procedural system does not keep the entire generated level in memory.

`RoomChainManager` maintains a forward and backward buffer of active rooms.

`RoomDeletionManager` removes rooms behind the player once they are no longer required.

This allows the game to create the impression of a much larger continuously generated environment while keeping the number of active rooms limited.

Rooms also have their own lifecycle:

```text
Instantiate
    ?
ActivateRoom()
    ?
Player enters
    ?
Gameplay
    ?
CloseRoom()
    ?
Destroy
```

Room activation is also responsible for room-specific initialization such as lighting and randomized interior elements.

---

## Event-Driven Communication

Several systems communicate through a lightweight event layer instead of directly referencing each other.

For example:

```csharp
GameEvents.OnDoorOpened
```

is used by both the procedural generation system and the entity spawning system.

This allows one gameplay action — opening a door — to trigger multiple independent systems:

```text
             Door Opened
                  ?
        ?????????????????????
        ?                   ?
RoomChainManager      EntitiesManager
        ?                   ?
Generate next room    Try spawn entity
```

This reduces direct coupling between gameplay systems.

---

## Entity Spawning

`EntitiesManager` controls enemy spawning independently from the entity state machine.

Entity spawning can be triggered by:

* opening doors;
* timers;
* configured spawn thresholds;
* weighted random selection;
* enemy count limits;
* safe-room restrictions.

Entity configuration is stored in `EntityData` ScriptableObjects, allowing different entity types to share the same spawning infrastructure.

The spawning pipeline is approximately:

```text
EntitiesManager
      ?
      ?
Select EntityData
      ?
      ?
Find hidden spawn position
      ?
      ?
Instantiate prefab
      ?
      ?
Entity.Initialize()
      ?
      ?
Behaviour.Initialize()
      ?
      ?
Entity State Machine
```

---

## Key Technical Concepts

The project demonstrates several gameplay-programming concepts:

* **State Machine Pattern** for enemy behaviour.
* **Procedural Generation** using weighted room selection.
* **ScriptableObject-based data configuration**.
* **Event-driven communication** between gameplay systems.
* **Dynamic room instantiation and deletion**.
* **Object lifecycle management** for procedurally generated content.
* **Separation of runtime logic and configuration data**.
* **Reusable Singleton-based managers** for global gameplay systems.
* **Modular entity behaviours and states**.
* **Progression-based procedural content** through room thresholds and unique rooms.

The repository intentionally focuses on the **technical implementation of the game's core systems** rather than the complete Unity project and its assets.
