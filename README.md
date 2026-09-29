# HRUSCHEOTVOD - Technical Overview

This repository contains selected technical systems from HRUSCHEOTVOD, a procedural liminal horror game built with Unity.

The project focuses on procedural room generation, entity AI, state machines, event-driven communication, and modular gameplay systems.

## Architecture

The project is divided into several independent systems responsible for different gameplay concerns:

* Room generation
* Entity AI
* Entity spawning
* Room lifecycle
* Gameplay events
* Shared managers and utilities

The systems communicate through managers, interfaces, ScriptableObjects, and C# events instead of relying on a single centralized gameplay script.

---

## Entity State Machine

The entity system is built around a state machine.

Main entry point:

`Scripts/Entity/Entity.cs`

State interface:

`Scripts/Entity/IEntityState.cs`

States:

`Scripts/Entity/States/`

The `Entity` component owns the current state and controls state transitions through:

```csharp
SetState(IEntityState newState)
```

Each state implements:

```csharp
Enter(Entity entity)
Update()
Exit()
```

This separates entity behavior into independent states instead of putting all AI logic into one large class.

Example structure:

```text
Entity
  |
  +-- IEntityState
        |
        +-- Core
        +-- Ephemer
        +-- Fracture
        +-- Glitch
        +-- Walker
        +-- Watcher
```

Entity-specific behavior is also separated from the state machine:

`Scripts/Entity/Behaviours/`

Entity configuration is stored in ScriptableObjects:

`Scripts/Entity/EntityData.cs`

This allows entity data, behavior, prefab, damage, spawn conditions, and other parameters to be configured independently from the runtime state machine.

---

## Procedural Generation

The main procedural generation system is:

`Scripts/RoomsGeneration/RoomChainManager.cs`

`RoomChainManager` is responsible for building the room chain and selecting rooms during gameplay.

The generation system uses room metadata rather than selecting completely random prefabs.

Room data is defined in:

`Scripts/RoomsGeneration/Room/Room.cs`

Important parameters include:

* Room weight
* Room threshold
* Room type
* Available door sockets
* Light sources

Room selection therefore depends on the current generation context, available connections, and progression.

Simplified structure:

```text
RoomChainManager
  |
  +-- Select room
  |
  +-- Check generation conditions
  |
  +-- Match available sockets
  |
  +-- Instantiate room
  |
  +-- Connect room to the chain
  |
  +-- Continue generation
```

The system also supports special room types and controlled generation sequences, allowing authored gameplay situations to exist inside an otherwise procedural environment.

---

## Room Lifecycle

Each generated room is represented by:

`Scripts/RoomsGeneration/Room/Room.cs`

A room is responsible for its own runtime lifecycle.

Examples of responsibilities:

* Initializing interactable objects
* Managing door sockets
* Activating the room when the player enters
* Updating room-related systems
* Managing lights
* Tracking whether the player is inside
* Closing and removing rooms when they are no longer needed

This allows the generation manager to focus on creating the room chain while the individual room controls its own runtime behavior.

---

## Entity Spawning

Entity spawning is handled separately from the entity state machine.

Main manager:

`Scripts/Entity/EntitiesManager.cs`

The manager controls:

* Spawn timing
* Spawn chances
* Enemy limits
* Spawn conditions
* Weighted entity selection
* Room progression requirements
* Spawn positions

Entity configuration comes from `EntityData` ScriptableObjects.

This keeps the responsibilities separated:

```text
EntitiesManager
  |
  +-- Decides WHEN and WHAT to spawn
  |
  +-- Entity
        |
        +-- Controls HOW the entity behaves
```

---

## Event-Driven Communication

The project also uses C# events for communication between systems.

Main event container:

`Scripts/Utility/GameEvents.cs`

For example:

```csharp
GameEvents.OnDoorOpened
```

Door-related events can be consumed by different systems without creating direct dependencies between them.

This is used by systems such as room generation and entity spawning.

The approach helps reduce coupling between gameplay systems and makes it easier to add new listeners without modifying the original event source.

---

## Key Technical Concepts

The main technical concepts demonstrated in this repository are:

* State Machine
* Procedural Generation
* Weighted Random Selection
* ScriptableObject-based configuration
* Event-driven architecture
* Modular entity behavior
* Runtime room lifecycle management
* Singleton-based managers
* Separation of data, spawning, and runtime behavior

The repository intentionally contains selected technical systems rather than the complete game project.
