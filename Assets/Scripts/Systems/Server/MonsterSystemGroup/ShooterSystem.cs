using Aspect;
using Common;
using Component;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems.Server.MonsterSystemGroup {
    [UpdateInGroup(typeof(MonsterBehaviorGroup))]
    public partial struct ShooterSystem : ISystem {
        private BufferLookup<ProjectileShootingEvent> _projectileShootingEventBuffer;

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<RoundData>();
            state.RequireForUpdate<ShooterComponent>();
            _projectileShootingEventBuffer = state.GetBufferLookup<ProjectileShootingEvent>();
        }

        public void OnUpdate(ref SystemState state) {
            var roundData = SystemAPI.GetSingleton<RoundData>();
            if (roundData.Phase != RoundPhase.Combat) return;
            _projectileShootingEventBuffer.Update(ref state);
            foreach (var (shooter, monsterAspect, entity)
                     in SystemAPI.Query<RefRW<ShooterComponent>, MonsterAspect>()
                         .WithEntityAccess()) {
                ref var cd = ref shooter.ValueRW.coolDownData;
                if (!cd.IsCoolDownReadyWithBaseCd(SystemAPI.Time.ElapsedTime)) continue; //如果冷却时间未到
                var shooterData = shooter.ValueRO;
                //计算目标方向

                var targetDistanceSq = monsterAspect.Monster.ValueRO.targetDistanceSq;
                if (targetDistanceSq > math.square(shooterData.triggerRange)) continue; //超出范围
                cd.TriggerCoolDown(SystemAPI.Time.ElapsedTime); //触发冷却

                var buffer = _projectileShootingEventBuffer[entity];

                var dmgSrc = shooterData.dmgSrcComponent;


                //赋值子弹的起始属性
                var projectileData = shooterData.projectileData;
                projectileData.spawnTime = SystemAPI.Time.ElapsedTime;

                var startPos = monsterAspect.LocalTransform.ValueRO.Position;
                startPos.z = 0;
                projectileData.startPosition = startPos;


                var radPerBullet = shooterData.spreadAngleRad / shooterData.count;
                var targetDir = monsterAspect.Monster.ValueRO.targetPlayerDirNormalized;

                for (var i = 0; i < shooterData.count; i++) {
                    //计算子弹的方向
                    var angle = radPerBullet * (i - (shooterData.count - 1) / 2.0f);
                    projectileData.direction = math.mul(quaternion.AxisAngle(math.forward(), angle), targetDir);

                    //子弹生成事件将会在ProjectileSpawnSystem中处理
                    buffer.Add(new ProjectileShootingEvent {
                        ProjectileData = projectileData,
                        DmgSrcComponent = dmgSrc,
                        HealthComponent = shooter.ValueRO.projectileHealth,
                        FactionComponent = FactionComponent.Monster,
                        ProjectilePrefab = shooter.ValueRO.ProjectilePrefab,
                    });
                }
            }
        }
    }
}