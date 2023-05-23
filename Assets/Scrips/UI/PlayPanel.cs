using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayPanel : MonoBehaviour {
    [SerializeField] private CustomButton hostButton;
    [SerializeField] private CustomButton joinButton;

    private void Awake() {
        hostButton.OnClick += () => UIManager.Instance.HostPanel.Open();
        joinButton.OnClick += () => UIManager.Instance.JoinPanel.Open();
    }
}
