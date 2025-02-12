using Authoring;
using Systems.Client;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;


namespace Systems.Server {
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct GoInGameServerSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            //当有GoInGameRequest和ReceiveRpcCommandRequest时，才会执行OnUpdate
            var query = SystemAPI.QueryBuilder().WithAll<GoInGameRequest, ReceiveRpcCommandRequest>().Build();
            state.RequireForUpdate(query);
            state.RequireForUpdate<PlayerSpawner>();
        }


        
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var playerPrefab = SystemAPI.GetSingleton<PlayerSpawner>().PlayerPrefab;
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            //当服务器World内接收到GoInGameRequest时
            //生成一个Player实体
            foreach (var (requestSource, requestEntity) in
                     SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>>()
                         .WithAll<GoInGameRequest>().WithEntityAccess()) {
                ecb.AddComponent<NetworkStreamInGame>(requestSource.ValueRO.SourceConnection);

                {
                    var networkId = SystemAPI.GetComponent<NetworkId>(requestSource.ValueRO.SourceConnection);
                    var player = ecb.Instantiate(playerPrefab);
                    ecb.SetComponent(player, new GhostOwner {NetworkId = networkId.Value});

                    // Add the player to the linked entity group so it is destroyed automatically on disconnect
                    ecb.AppendToBuffer(requestSource.ValueRO.SourceConnection, new LinkedEntityGroup {Value = player});
                }
                ecb.DestroyEntity(requestEntity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}