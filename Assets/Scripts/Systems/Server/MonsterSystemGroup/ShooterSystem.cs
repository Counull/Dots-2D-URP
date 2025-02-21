using Aspect;
using Common;
using Component;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems.Server.MonsterSystemGroup {
    [UpdateInGroup(typeof(MonsterBehaviorGroup))]
    public partial struct ShooterSystem : ISystem {
        private BufferLookup<ProjectileShootingEvent> projectileShootingEventBuffer;

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<ShooterComponent>();
            projectileShootingEventBuffer = state.GetBufferLookup<ProjectileShootingEvent>();
        }

        public void OnUpdate(ref SystemState state) {
            projectileShootingEventBuffer.Update(ref state);
            foreach (var (shooter, monsterAspect, entity)
                     in SystemAPI.Query<RefRW<ShooterComponent>, MonsterAspect>()
                         .WithEntityAccess()) {
                ref var cd = ref shooter.ValueRW.coolDownData;
                if (!cd.IsCoolDownReady(SystemAPI.Time.ElapsedTime)) continue; //如果冷却时间未到
                if (cd.TriggerTime == 0) {
                    //如果未触发过冷却则初始化冷却时间
                    cd.TriggerCoolDown(SystemAPI.Time.ElapsedTime);
                    continue;
                }

                var shooterData = shooter.ValueRO;

                //计算目标方向
                var targetDir = monsterAspect.Monster.ValueRO.targetPlayerPos -
                                monsterAspect.LocalTransform.ValueRO.Position;
                if (math.lengthsq(targetDir) > math.square(shooterData.triggerRange)) continue; //超出范围
                cd.TriggerCoolDown(SystemAPI.Time.ElapsedTime); //触发冷却

                targetDir = math.normalize(targetDir);
                var buffer = projectileShootingEventBuffer[entity];

                //赋值子弹的起始属性
                var projectileData = shooterData.projectileData;
                projectileData.startPosition = monsterAspect.LocalTransform.ValueRO.Position;
                projectileData.spawnTime = SystemAPI.Time.ElapsedTime;

                var radPerBullet = shooterData.spreadAngleRad / shooterData.count;
                var dmgSrc = shooterData.dmgSrcComponent;
                dmgSrc.ownerFaction = Faction.Monster;
                for (var i = 0; i < shooterData.count; i++) {
                    //计算子弹的方向
                    var angle = radPerBullet * (i - (shooterData.count - 1) / 2.0f);
                    projectileData.direction = math.mul(quaternion.AxisAngle(math.forward(), angle), targetDir);
                    //子弹生成事件将会在ProjectileSpawnSystem中处理
                    buffer.Add(new ProjectileShootingEvent {
                        ProjectileData = projectileData,
                        ProjectilePrefab = shooter.ValueRO.ProjectilePrefab,
                        DmgSrcComponent = dmgSrc
                    });
                }
            }
        }
    }
}