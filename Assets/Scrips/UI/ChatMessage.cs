using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatMessage : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private TMP_Text messageText;

    public void SetMessage(ChatMessageInfo messageInfo) {
        messageText.text = $"<b>{messageInfo.SenderName}:</b> {messageInfo.Message}";
        messageText.color = messageInfo.Color;
    }
}
