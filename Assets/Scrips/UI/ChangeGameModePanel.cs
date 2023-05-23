using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChangeGameModePanel : MonoBehaviour {
    [SerializeField] private Dropdown gameModeDropdown;

    public Panel Panel;

    private void OnEnable() {
        gameModeDropdown.SelectItemByValue(LobbyManager.Instance.CurrentGameMode.Value.ToString());
    }

    private void Awake() {
        Panel.OnSubmit += () => {
            LobbyManager.Instance.SetGameModeServerRpc(LobbyManager.Instance.GetGameModeByName(gameModeDropdown.SelectedValue));
            Panel.Close();
        };
    }
}
