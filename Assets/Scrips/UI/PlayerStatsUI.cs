using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour {
    [SerializeField] private Image healthBarImage;
    [SerializeField] private Player player;

    private void Awake() {
        player.Stats.OnHealthChanged += (newHealth) => {
            Vector2 size = healthBarImage.rectTransform.sizeDelta;
            healthBarImage.rectTransform.sizeDelta = new Vector2(Utils.Remap(size.x, 0, player.Stats.MaxHealth, 0, 800), size.y);
        };
    }
}
