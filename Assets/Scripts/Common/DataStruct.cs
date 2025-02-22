using System;

namespace Common {
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

        public bool IsCoolDownReadyWithBaseCd(double currentTime) {
            if (TriggerTime != 0) return currentTime - TriggerTime >= totalTime;
            //未触发过冷则初始化冷却时间
            TriggerCoolDown(currentTime);
            return false;
        }

        public readonly bool IsCoolDownReady(double currentTime) {
            return currentTime - TriggerTime >= totalTime;
        }
    }
}