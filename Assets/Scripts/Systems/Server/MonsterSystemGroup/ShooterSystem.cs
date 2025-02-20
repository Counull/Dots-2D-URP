using Component;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Server.MonsterBehavior {
    public partial struct ShooterSystem : ISystem {
        BufferLookup<ProjectileShootingEvent> _projectileShootingEventBuffer;

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<ShooterComponent>();
            _projectileShootingEventBuffer = state.GetBufferLookup<ProjectileShootingEvent>();
        }

        public void OnUpdate(ref SystemState state) {
            _projectileShootingEventBuffer.Update(ref state);
            foreach (var (shooter, localTransform, monster, entity)
                     in SystemAPI.Query<RefRW<ShooterComponent>, RefRO<LocalTransform>, RefRO<MonsterComponent>>()
                         .WithEntityAccess()) {
                ref var cd = ref shooter.ValueRW.coolDownData;
                if (!cd.IsCoolDownReady(SystemAPI.Time.ElapsedTime))
                    continue;
                if (cd.TriggerTime == 0) {
                    cd.TriggerCoolDown(SystemAPI.Time.ElapsedTime);
                    continue;
                }

                var targetDir = monster.ValueRO.targetPlayerPos - localTransform.ValueRO.Position;

                if (math.lengthsq(targetDir) > math.square(shooter.ValueRO.triggerRange))
                    continue;
                targetDir = math.normalize(targetDir);
                cd.TriggerCoolDown(SystemAPI.Time.ElapsedTime);
                var buffer = _projectileShootingEventBuffer[entity];
                var projectileData = shooter.ValueRO.projectileData;
                projectileData.startPosition = localTransform.ValueRO.Position;
                projectileData.Shooter = entity;
                var radPerBullet = shooter.ValueRO.spreadAngleRad / shooter.ValueRO.count;

                for (var i = 0; i < shooter.ValueRO.count; i++) {
                    var angle = radPerBullet * (i - (shooter.ValueRO.count - 1) / 2.0f);
                    var rotatedDir = math.mul(quaternion.AxisAngle(math.forward(), angle), targetDir);
                    projectileData.direction = rotatedDir;
                    buffer.Add(new ProjectileShootingEvent() {
                        ProjectileData = projectileData,
                        ProjectilePrefab = shooter.ValueRO.ProjectilePrefab,
                    });
                }
            }
        }
    }
}