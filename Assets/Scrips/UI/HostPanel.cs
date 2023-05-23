using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HostPanel : MonoBehaviour {
    [SerializeField] private Dropdown gameModeDropdown;
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField bindIpInput;

    private Panel _panel;

    private void Awake() {
        _panel = GetComponent<Panel>();

        _panel.OnSubmit += () => {
            string[] split = bindIpInput.text.Split(":");
            if (split.Length != 2) return;

            //UIManager.Instance.LoadingPanel.OpenHostLoading();
            UIManager.Instance.Username = usernameInput.text;
            GameManager.Instance.SetConnectionData(split[0], ushort.Parse(split[1]));
            WWNetworkManager.Instance.Host();
            LobbyManager.Instance.SetGameModeServerRpc(LobbyManager.Instance.GetGameModeByName(gameModeDropdown.SelectedValue));
        };
    }
}
