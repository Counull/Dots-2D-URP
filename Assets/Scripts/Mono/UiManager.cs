using Component;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mono {
    public class UiManager : MonoBehaviour {
        [FormerlySerializedAs("HpText")] [SerializeField]
        private TMP_Text hpText;

        [SerializeField] private TMP_Text timeText;

        public void RefreshHp(in HealthComponent healthComponent) {
            hpText.text = $"{healthComponent.CurrentHealth}/{healthComponent.maxHealth}";
        }

        public void RefreshRoundData(in RoundData roundData) {
            /*if (roundData.RoundDefeated) {
                timeText.text = "Defeated";
            }
            else if (roundData.CombatTimeOut) {
                timeText.text = "Succeed";
            }
            else {
                timeText.text = ((int) roundData.CombatTimeCountingDown).ToString();
            }*/
        }
    }
}