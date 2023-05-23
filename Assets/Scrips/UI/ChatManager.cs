using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChatManager : NetworkBehaviour {
    public static ChatManager Instance;

    [Header("Chat Settings")]
    [SerializeField] private Color[] chatColors;
    [SerializeField] private Transform messageHolder;
    [SerializeField] private TMP_InputField messageInputField;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private GameObject messagePrefab;
    [SerializeField] private float chatKeepAliveTime;
    private readonly List<ChatMessageInfo> _chatMessages = new();
    private readonly Dictionary<ulong, Color> _playersChatColor = new();
    private float _timeSinceUnselected = -1;
    private bool _messageSentWhileOpen = false;

    public bool IsOpen;

    [Header("Settings")]
    [SerializeField] private UITweener panelOpenMoveTweener;
    [SerializeField] private UITweener panelCloseMoveTweener;
    [SerializeField] private UITweener panelOpenFadeTweener;
    [SerializeField] private UITweener panelCloseFadeTweener;

    private void Awake() {
        if (Instance == null) Instance = this;
        else {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(gameObject);
            return;
        }

        System.Random random = new();

        GameManager.Instance.OnPlayerSpawned += (player, isOwner) => {
            Color[] availableColors = chatColors.ToList().FindAll(x => !_playersChatColor.Values.Contains(x)).ToArray();
            if (availableColors.Length == chatColors.Length) availableColors = chatColors;
            Debug.Log(availableColors);
            int randomIndex = random.Next(availableColors.Length);
            _playersChatColor.Add(player.OwnerClientId, availableColors[randomIndex]);
        };

        GameManager.Instance.OnPlayerDespawned += (player, isOwner) => {
            _playersChatColor.Remove(player.OwnerClientId);
        };
    }

    private void Update() {
        if (_chatMessages.Count == 0 || _timeSinceUnselected == -1) return;
        _timeSinceUnselected += Time.deltaTime;

        if (_timeSinceUnselected >= chatKeepAliveTime && IsOpen) Close();
    }

    public void Open(bool selectInput, bool openInPosition) {
        IsOpen = true;

        if (openInPosition) {
            panelOpenFadeTweener.SetOnceStartOffset(false);
            panelOpenFadeTweener.HandleTween();
            _timeSinceUnselected = -1;
        }
        else {
            panelOpenMoveTweener.HandleTween();
            panelOpenFadeTweener.HandleTween();
            _messageSentWhileOpen = false;
        }

        if (selectInput) {
            EventSystem.current.SetSelectedGameObject(messageInputField.gameObject);
            GameManager.Instance.LocalPlayer.SetKeybindsActive(false);
        }
        else _timeSinceUnselected = 0;
    }

    public void Close() {
        IsOpen = false;

        panelCloseMoveTweener.HandleTween();
        panelCloseFadeTweener.HandleTween();

        messageInputField.text = "";

        _timeSinceUnselected = -1;
        GameManager.Instance.LocalPlayer.SetKeybindsActive(true);
        GameManager.Instance.LockCursor();
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void TrySendMessage() {
        if (messageInputField.text.Trim() != "") {
            SendMessageServerRpc(new ChatMessageInfo(GameManager.Instance.LocalPlayer.MultiplayerData.Username,
                messageInputField.text.Trim(), Color.black));

            messageInputField.text = "";
            _messageSentWhileOpen = true;
            //messageInputField.ActivateInputField();
            //return;
            //EventSystem.current.SetSelectedGameObject(null);
            //GameManager.Instance.PlayerObject.EnableKeybinds();
            //GameManager.Instance.LockCursor();
            //_timeSinceUnselected = 0;
        }
        else if (!_messageSentWhileOpen) {
            Close();
            return;
        }

        if (_timeSinceUnselected == -1) {
            EventSystem.current.SetSelectedGameObject(null);
            GameManager.Instance.LocalPlayer.SetKeybindsActive(true);
            GameManager.Instance.LockCursor();
            _timeSinceUnselected = 0;
        }
        else Open(true, true);

        // Close();
    }

    public void ClearChat() {
        foreach (Transform child in messageHolder) Destroy(child.gameObject);
        _chatMessages.Clear();
        _playersChatColor.Clear();
    }

    private void AddNewMessage(ChatMessageInfo messageInfo) {
        _chatMessages.Add(messageInfo);
        GameObject newMessageGO = Instantiate(messagePrefab, Vector3.zero, Quaternion.identity, messageHolder);
        ChatMessage message = newMessageGO.GetComponent<ChatMessage>();
        message.SetMessage(messageInfo);
        scrollRect.verticalNormalizedPosition = 0;

        if (!IsOpen) {
            Open(false, false);
            _messageSentWhileOpen = true;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendMessageServerRpc(ChatMessageInfo messageInfo, ServerRpcParams rpcParams = default) {
        Color color = _playersChatColor[rpcParams.Receive.SenderClientId];
        messageInfo.Color = color;
        SendMessageClientRpc(messageInfo);
    }

    [ServerRpc]
    public void SendSystemMessageServerRpc(ChatMessageInfo messageInfo) {
        SendMessageClientRpc(messageInfo);
    }

    [ClientRpc]
    private void SendMessageClientRpc(ChatMessageInfo messageInfo) {
        AddNewMessage(messageInfo);
    }
}

public struct ChatMessageInfo : IEquatable<ChatMessageInfo>, INetworkSerializable {
    public FixedString32Bytes SenderName;
    public FixedString128Bytes Message;
    public Color Color;

    public ChatMessageInfo(FixedString32Bytes senderName, FixedString128Bytes message, Color color) {
        SenderName = senderName;
        Message = message;
        Color = color;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref SenderName);
        serializer.SerializeValue(ref Message);
        serializer.SerializeValue(ref Color);
    }

    public bool Equals(ChatMessageInfo other) => other.SenderName == SenderName && other.Message == Message && other.Color == Color;
}
