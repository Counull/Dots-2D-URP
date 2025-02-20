using System;
using UnityEngine.Serialization;

namespace Data {
    [Serializable]
    public struct CoolDownData {
        public float totalTime;
        public double TriggerTime { get; private set; }


        public void TriggerCoolDown(double triggerTime) {
            TriggerTime = triggerTime;
        }

        public void ResetCoolDown() {
            TriggerTime = 0;
        }

        public readonly bool IsCoolDownReady(double currentTime) {
            return currentTime - TriggerTime >= totalTime;
        }
    }
}