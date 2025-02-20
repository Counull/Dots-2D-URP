using Unity.Burst;
using Unity.Cinemachine;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;


namespace Systems.Client {
    public struct GoInGameRequest : IRpcCommand { }


    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    public partial struct GoInGameClientSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkId>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            //查找所有带着networkId(跟着返回他们的entity)
            //且他们暂时还没带有NetworkStreamInGame的标签
            foreach (var (id, entity) in
                     SystemAPI.Query<RefRO<NetworkId>>().WithNone<NetworkStreamInGame>()
                         .WithEntityAccess()) {
                //为entity添加NetworkStreamInGame标签 
                ecb.AddComponent<NetworkStreamInGame>(entity);
                var req = ecb.CreateEntity();
                ecb.AddComponent<GoInGameRequest>(req);
                ecb.AddComponent(req, new SendRpcCommandRequest() {
                    TargetConnection = entity
                });
            }

            ecb.Playback(state.EntityManager);
        }
    }
}