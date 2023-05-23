using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOrder : MonoBehaviour {
    [SerializeField] private int order;
    [SerializeField] private bool orderOnStart;

    private void OnEnable() {
        if (orderOnStart) Order();
    }

    public void Order() {
        if (!TryGetComponent(out GraphicRaycaster graphicRaycaster))
            gameObject.AddComponent<GraphicRaycaster>();
        if (!TryGetComponent(out Canvas canvas))
            canvas = gameObject.AddComponent<Canvas>();

        canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1;
        canvas.overrideSorting = true;
        canvas.sortingOrder = order;
    }

    public void RemoveOrder() {
        Destroy(GetComponent<GraphicRaycaster>());
        Destroy(GetComponent<Canvas>());
    }
}
