using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JoinPanel : MonoBehaviour {
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField ipAddressInput;

    private Panel _panel;

    private void Awake() {
        _panel = GetComponent<Panel>();

        _panel.OnSubmit += () => {
            string[] split = ipAddressInput.text.Split(":");
            if (split.Length != 2) return;

            //UIManager.Instance.LoadingPanel.OpenJoinLoading($"{split[0]}:{split[1]}");
            UIManager.Instance.Username = usernameInput.text;
            GameManager.Instance.SetConnectionData(split[0], ushort.Parse(split[1]));
            WWNetworkManager.Instance.Client();
        };
    }
}
