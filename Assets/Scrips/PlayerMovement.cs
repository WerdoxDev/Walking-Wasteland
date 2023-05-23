using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour {
    [Header("Movement")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float smoothTime;
    [SerializeField] private float rotationOffset;

    private Player _player;
    private Rigidbody2D _rb;
    private Vector2 _movementInput;
    private Vector2 _smoothedInput;
    private Vector2 _smoothedCurrentVelocity;

    private void Awake() {
        _player = GetComponent<Player>();
        _rb = _player.Rigidbody;
    }

    public override void OnNetworkSpawn() {
        if (!IsOwner) enabled = false;
        else SetInputEvents(true);
    }

    private void Update() {
        Vector2 mousePosition = _player.Camera.ScreenToWorldPoint(Mouse.current.position.value);
        float AngleRad = Mathf.Atan2(mousePosition.y - transform.position.y, mousePosition.x - transform.position.x);
        float AngleDeg = (180 / Mathf.PI) * AngleRad;
        _player.SpriteHolder.rotation = Quaternion.Euler(0, 0, AngleDeg + rotationOffset);

        //_player.CurrentSprite.transform.up = mousePosition - new Vector2(transform.position.x, transform.position.y);
    }

    private void FixedUpdate() {
        _smoothedInput = Vector2.SmoothDamp(_smoothedInput, _movementInput, ref _smoothedCurrentVelocity, smoothTime);
        _rb.velocity = _smoothedInput * walkSpeed;
    }

    private void OnDisable() => SetInputEvents(false);

    private void SetInputEvents(bool enabled) {
        void OnMove(Vector2 input) => _movementInput = input;

        if (enabled) _player.InputReader.MoveEvent += OnMove;
        else _player.InputReader.MoveEvent -= OnMove;
    }
}
