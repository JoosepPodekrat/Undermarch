# TASKS

Ground rules
- Do not reference UnityEngine in Game.Simulation.
- Always iterate entities/projectiles in ascending numeric Id order to keep replays stable.
- Lists and dictionaries are used as follows: IReadOnlyList<IEntity>/IProjectile provides stable iteration ordering for phases; Dictionary<int, IEntity>/IProjectile provides O(1) lookup by Id for collision checks and command dispatch.
- Pending changes are staged in IList<T> buffers on ISimContext and applied centrally.

T1 Simulation Orchestrator and Context
Implement the ISimulation orchestrator class (file suggested: Assets/Scripts/Simulation/Core/Simulation.cs). It must own world state (Board, registries for entities/projectiles/interactables, IEffectSystem, IInteractionMatrix, IDamageResolver, IRandomSource) and expose ISimQuery. Implement AdvanceOneTick to execute phases in the fixed order documented on ISimulation. Use ISimContext to pass dependencies and staging buffers to subsystems. At the end of Cleanup, rebuild a fresh snapshot object that implements ISimQuery. Acceptance: calling AdvanceOneTick increments CurrentTick, executes empty phases without error, and produces a non-null Query with correct Width/Height.

T2 Board read/write adapters
Make Board implement IBoardRead and IBoardWrite (Assets/Scripts/Simulation/Grid/Board.cs vs IBoard.cs). On TryMoveEntity validate bounds, wall blocking, and entity occupancy (no stacking). Returns true only when the move is legal and completed; the caller then updates IEntity.SetPosition. Accept placing one interactable per tile and an arbitrary list of tile effects (stored by IEffectSystem, Board only needs to report presence via HasAnyTileEffects). Acceptance: unit test that places a wall blocks TryMoveEntity, open tile allows it, and entity occupancy prevents stacking.

T3 InteractionMatrix baseline
Implement IInteractionMatrix (Assets/Scripts/Simulation/Rules/IInteractionMatrix.cs). Start with: Defender traps trigger on Hero, not on Defender; ProjectileHero does not hit Hero (no friendly fire), but hits Defender; projectiles hit walls. Acceptance: unit tests verify boolean outcomes for explicit pairs.

T4 Damage resolution
Implement IDamageResolver (Assets/Scripts/Simulation/Combat/IDamageResolver.cs). Use percent resistances 0..100 from IResistanceProvider.Resistances; clamp to [0,100]. Policy: final = sum over types of floor(raw * (100 - resist) / 100). Acceptance: 10 Physical vs 20% returns 8; mixed packets process per type.

T5 Effect system skeleton
Implement IEffectSystem (Assets/Scripts/Simulation/Effects/IEffects.cs). Maintain entity-attached and tile-attached effect lists. Provide ApplyEntityEffect, ApplyTileEffect, TickTileEffects, TickEntityEffects, ExpireEffects with deterministic iteration. Use string definition Ids as keys for quick lookup and removal. Acceptance: apply Freeze to an entity for N ticks sets IsFrozen true for N ticks and false after ExpireEffects; TileHasEffect returns true while a SlowZone exists.

T6 Interactables auto-activation and on-enter
Implement a simple SpikeTrap that implements IInteractable (Assets/Scripts/Simulation/Interactables/SpikeTrap.cs). AutoActivates false; OnEntityEnter checks IInteractionMatrix.TrapTriggersOn and applies DamagePacket(Physical) via IDamageResolver to the entering entity, then a short Freeze via IEffectSystem. Acceptance: an entering Hero loses expected HP and is frozen; a Defender is ignored.

T7 Entities phase movement
Implement an EntitiesPhase system class (Assets/Scripts/Simulation/Entities/EntitiesPhase.cs) that iterates ISimContext.Entities in ascending Id order and, for each, extracts moves = Accumulator.AddAndExtractMoves(Speed.Numerator) if not IsFrozen. For each single-tile step request, ask IEntityController.DecideNextStep when available, compute target tile as Pos + Dir4 offset, validate via BoardRead/InteractionMatrix (walls block; entities block), then call BoardWrite.TryMoveEntity and IEntity.SetPosition on success. Call IInteractable.OnEntityEnter/Leave appropriately when crossing tiles. Acceptance: unit tests verify 3/4 speed results in 3 moves across 4 ticks and respects walls and entity occupancy.

T8 Projectiles phase micro-steps
Implement a ProjectilesPhase system class (Assets/Scripts/Simulation/Projectiles/ProjectilesPhase.cs). Iterate ISimContext.Projectiles in ascending Id order. Per projectile, call AddBudgetForTick then while budget allows steps, compute the next 8-way step using a Bresenham-like integrator derived from Dir8, consume budget using MicroStepCost, and attempt the step. Collision priority per micro-step: wall then entity then projectile. Use IInteractionMatrix to decide if a hit applies. On entity hit: resolve damage via IDamageResolver then call entity.ApplyDamageResolved; despawn projectile. On wall hit: despawn. On projectile hit (if enabled): despawn both or apply rules. Acceptance: 6 tiles/tick projectile moves 6 cardinals or 4 diag + 1 card per tick equivalent; collisions behave as configured and despawns are staged.

T9 Visibility skeleton
Implement a minimal LOS service (file suggested: Assets/Scripts/Simulation/Grid/Visibility.cs) using Bresenham to check if two positions have line-of-sight given walls and fog tile effects (query via IEffectSystem.TileHasEffect). This wires into future AI and Presentation debug overlays. Acceptance: straight and diagonal LOS pass with no walls and fail when a wall is between.

T10 Read model snapshot
Implement a Snapshot builder (Assets/Scripts/Simulation/ReadModel/SimSnapshot.cs) that gathers EntityState and ProjectileState from registries after Cleanup. Store and expose it via ISimulation.Query. Acceptance: Presentation can iterate Entities() and Projectiles() and see positions consistent with the last tick.

Ownership and division
Simulation orchestrator and context (T1, T2) must be owned by a single dev this sprint. Entities movement (T7) and Projectiles micro-steps (T8) can be parallelized once the context is available. Interactables and Effects (T5, T6) can be developed in parallel; both require InteractionMatrix (T3) and DamageResolver (T4). Read model snapshot (T10) ties everything for the Presentation to render.

Testing guidance
Place EditMode tests under Assets/Scripts/Tests/EditMode. Use only Game.Simulation references. Prefer constructing tiny 8x8 boards and explicit placements. Assert on TilePos equality and counts. For determinism-sensitive behavior, seed IRandomSource to a fixed value and assert sequences match across runs.

Presentation note
Presentation reads ISimulation.Query each Update after Simulation.AdvanceOneTick is driven by TickDriver. Do not cache references to mutable Simulation objects; cache only values copied from ISimQuery (ids, positions, hp) each frame and update or pool visuals accordingly.

---

If you want, I can add minimal skeleton classes for EntitiesPhase and ProjectilesPhase that compile and call through these interfaces, so your teammates can fill in internals without worrying about wiring.
