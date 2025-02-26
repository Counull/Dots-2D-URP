using System.Collections.Generic;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;

namespace Utils {
    public class Utils {
        [BurstCompile]
        public struct AxisXComparer : IComparer<float3> {
            public int Compare(float3 a, float3 b) {
                return a.x.CompareTo(b.x);
            }
        }

        [BurstCompile]
        public struct LocalTransformXComparer : IComparer<LocalTransform> {
            public int Compare(LocalTransform lhs, LocalTransform rhs) {
                return lhs.Position.x.CompareTo(rhs.Position.x);
            }
        }
    }
}