using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerVisual : MonoBehaviour {
    [Header("General")]
    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private GameObject readyCheckmark;

    [Header("Team")]
    [SerializeField] private TMP_Text teamText;
    [SerializeField] private GameObject teamWrapper;
    [SerializeField] private CustomButton nextTeamButton;
    [SerializeField] private CustomButton prevTeamButton;

    [Header("Character")]
    [SerializeField] private Image playerSpriteImage;
    [SerializeField] private CustomButton nextCharButton;
    [SerializeField] private CustomButton prevCharButton;

    public LobbyPlayer Player;

    public byte CurrentTeam { get; private set; }
    public byte SpriteIndex { get; private set; }
    public string Username { get; private set; }

    private void Awake() {
        nextTeamButton.OnClick += () => Player.ChangeTeam(1);
        prevTeamButton.OnClick += () => Player.ChangeTeam(-1);

        nextCharButton.OnClick += () => Player.ChangeSprite(1);
        prevCharButton.OnClick += () => Player.ChangeSprite(-1);
    }

    public void SetTeam(byte team) {        
        CurrentTeam = team;
        teamText.text = $"Team {team + 1}";
    }

    public void SetSprite(Sprite sprite, byte index) {
        playerSpriteImage.sprite = sprite;
        SpriteIndex = index;
    }

    public void SetSprite(Sprite sprite, PlayerSpriteType type) {
        SetSprite(sprite, (byte)GameManager.Instance.GetPlayerSpriteIndexByType(type));
    }

    public void SetUsername(string username) {
        Username = username;
        usernameText.text = username;
    }

    public void SetReady(bool isReady) {
        readyCheckmark.SetActive(isReady);
    }

    public void SetTeamButtonsActive(bool isActive) {
        nextTeamButton.gameObject.SetActive(isActive);
        prevTeamButton.gameObject.SetActive(isActive);
    }

    public void SetTeamActive(bool isActive) {
        teamWrapper.SetActive(isActive);
    }

    public void SetCharButtonsActive(bool isActive) {
        nextCharButton.gameObject.SetActive(isActive);
        prevCharButton.gameObject.SetActive(isActive);
    }

    public void SetColorScheme(Color color) {
        backgroundImage.color = color;
        nextCharButton.Graphic.color = color;
        prevCharButton.Graphic.color = color;
    }
}
