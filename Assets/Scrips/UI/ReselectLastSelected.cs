using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class ReselectLastSelected : MonoBehaviour {
    public static ReselectLastSelected Instance;

    public GameObject LastSelectedObject { get; private set; }

    private InputSystemUIInputModule _inputModule;

    private void Awake() {
        if (Instance == null) Instance = this;
        else {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
            return;
        }

        _inputModule = GetComponent<InputSystemUIInputModule>();
    }

    private void Update() {
        if (!GameManager.Instance.IsPaused && GameManager.Instance.GameState == GameState.InGame) return;

        if (CacheLastSelectedObject()) return;

        if (_inputModule.move.action.WasPressedThisFrame()) {

            ReselectLastObject();
            return;
        }
    }

    public void ReselectLastObject() {
        if (!isActiveAndEnabled || !gameObject.activeInHierarchy)
            return;

        EventSystem.current.SetSelectedGameObject(LastSelectedObject);
    }

    private bool CacheLastSelectedObject() {
        if (EventSystem.current.currentSelectedGameObject == null) return false;
        LastSelectedObject = EventSystem.current.currentSelectedGameObject.gameObject;
        return true;
    }
}

