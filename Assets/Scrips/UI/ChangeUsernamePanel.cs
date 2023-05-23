using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChangeUsernamePanel : MonoBehaviour {
    [SerializeField] private TMP_InputField usernameInput;

    public Panel Panel;

    private void OnEnable() {
        usernameInput.text = UIManager.Instance.Username;
    }

    private void Awake() {
        Panel.OnSubmit += () => {
            UIManager.Instance.Username = usernameInput.text;
            LobbyManager.Instance.GetLobbyPlayer(UIManager.Instance.LobbyPanel.LocalPlayerVisualId).SetUsername(usernameInput.text);
            Panel.Close();
        };
    }
}
