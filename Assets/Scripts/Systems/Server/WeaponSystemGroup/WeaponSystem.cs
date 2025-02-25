using Component;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Server.WeaponSystemGroup {
    [UpdateInGroup(typeof(WeaponSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct WeaponSystem : ISystem {
        ComponentLookup<WeaponProjectile> _weaponProjectileLookup;
        BufferLookup<ProjectileShootingEvent> _projectileShootingEventLookup;
        EntityQuery _monsterQuery;

        public void OnCreate(ref SystemState state) {
            _weaponProjectileLookup = state.GetComponentLookup<WeaponProjectile>(true);
            _projectileShootingEventLookup = state.GetBufferLookup<ProjectileShootingEvent>(true);
            _monsterQuery = SystemAPI.QueryBuilder().WithAll<MonsterComponent, LocalTransform>().Build();
            state.RequireForUpdate(_monsterQuery);
            state.RequireForUpdate<WeaponComponent>();
            state.RequireForUpdate<RoundData>();
        }


        public void OnUpdate(ref SystemState state) {
            var monsterLocalTrans = _monsterQuery.ToComponentDataListAsync<LocalTransform>(state.WorldUpdateAllocator,
                out var monsterQueryHandle);
            monsterQueryHandle.Complete();
            var sortHandle = monsterLocalTrans.SortJob(new Utils.Utils.LocalTransformXComparer())
                .Schedule(monsterQueryHandle);
            _weaponProjectileLookup.Update(ref state);
            _projectileShootingEventLookup.Update(ref state);
            var ecbSystem = state.World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
            var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();


            var weaponTriggerJob = new WeaponTriggerJob() {
                WeaponProjectileLookup = _weaponProjectileLookup,
                ProjectileShootingEventLookup = _projectileShootingEventLookup,
                MonsterLocalTrans = monsterLocalTrans,
                Ecb = ecb,
                ElapsedTime = SystemAPI.Time.ElapsedTime,
            };
            var combineHandle = JobHandle.CombineDependencies(sortHandle, state.Dependency);
            state.Dependency = weaponTriggerJob.ScheduleParallel(combineHandle);

            //其实应该在spawn系统中等待这里的ECB被执行 这里这样写其实发射子弹的行为都会被推到下一帧
            //TODO system顺序 互相依赖 相关的东西写得都不是很好
        }
    }

    public partial struct WeaponTriggerJob : IJobEntity {
        [ReadOnly] public ComponentLookup<WeaponProjectile> WeaponProjectileLookup;
        [ReadOnly] public BufferLookup<ProjectileShootingEvent> ProjectileShootingEventLookup;
        [ReadOnly] public NativeList<LocalTransform> MonsterLocalTrans;
        public EntityCommandBuffer.ParallelWriter Ecb;
        public double ElapsedTime;

        public void Execute(Entity entity, [EntityIndexInQuery] int index, ref WeaponComponent weaponComponent,
            in LocalTransform localTransform, in LocalToWorld localToWorld, in FactionComponent factionComponent) {
            if (!weaponComponent.coolDownData.IsCoolDownReady(ElapsedTime)) return;
            weaponComponent.coolDownData.TriggerCoolDown(ElapsedTime);
            //local to world
            var weaponWorldPos = localToWorld.Position;
            var (nearestTargetPos, nearestDistSq) = FindNearestTarget(weaponWorldPos);
            if (nearestDistSq > math.square(weaponComponent.range)) return; //超出射程

            if (WeaponProjectileLookup.TryGetComponent(entity, out var weaponProjectile) &&
                ProjectileShootingEventLookup.HasBuffer(entity)) {
                var bulletData = weaponProjectile.projectileData;
                bulletData.direction = math.normalize(nearestTargetPos - weaponWorldPos);
                bulletData.startPosition = weaponWorldPos;
                bulletData.spawnTime = ElapsedTime;
                bulletData.maxDistance = weaponComponent.range;
                var projectileEvent = new ProjectileShootingEvent() {
                    ProjectileData = bulletData,
                    DmgSrcComponent = weaponComponent.dmgSrcComponent,
                    FactionComponent = factionComponent,
                    HealthComponent = weaponProjectile.projectileHealth,
                    ProjectilePrefab = weaponProjectile.ProjectilePrefab
                };
                Ecb.AppendToBuffer(index, entity, projectileEvent);
            }
        }


        //找到最近的目标
        public (float3, float) FindNearestTarget(float3 weaponPos) {
            //找到x轴最近的localPos                             犯罪的写法
            int startIdx = MonsterLocalTrans.BinarySearch(new LocalTransform() {Position = weaponPos},
                new Utils.Utils.LocalTransformXComparer() { });

            // When no precise match is found, BinarySearch returns the bitwise negation of the last-searched offset.
            // So when startIdx is negative, we flip the bits again, but we then must ensure the index is within bounds.
            if (startIdx < 0) startIdx = ~startIdx;
            if (startIdx >= MonsterLocalTrans.Length) startIdx = MonsterLocalTrans.Length - 1;

            float3 nearestTargetPos = MonsterLocalTrans[startIdx].Position;
            float nearestDistSq = math.distancesq(weaponPos, nearestTargetPos);
            // Searching upwards through the array for a closer target.
            Search(weaponPos, startIdx + 1, MonsterLocalTrans.Length, +1, ref nearestTargetPos, ref nearestDistSq);

            // Search downwards through the array for a closer target.
            Search(weaponPos, startIdx - 1, -1, -1, ref nearestTargetPos, ref nearestDistSq);
            return (nearestTargetPos, nearestDistSq);
        }

        void Search(float3 seekerPos, int startIdx, int endIdx, int step,
            ref float3 nearestTargetPos, ref float nearestDistSq) {
            for (int i = startIdx; i != endIdx; i += step) {
                float3 targetPos = MonsterLocalTrans[i].Position;
                float xdiff = seekerPos.x - targetPos.x;

                // If the square of the x distance is greater than the current nearest, we can stop searching.
                if ((xdiff * xdiff) > nearestDistSq) break;

                float distSq = math.distancesq(targetPos, seekerPos);

                if (distSq < nearestDistSq) {
                    nearestDistSq = distSq;
                    nearestTargetPos = targetPos;
                }
            }
        }
    }
}