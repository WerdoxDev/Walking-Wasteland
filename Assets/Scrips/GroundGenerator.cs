using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

[ExecuteInEditMode]
public class GroundGenerator : MonoBehaviour {
    [SerializeField] private Sprite[] groundSprites;
    [SerializeField] private Vector2 gridSize;
    [SerializeField] private float unitSize;

    public void GenerateGround() {
        // Destroy old tiles
        int childCount = transform.childCount;
        for (int i = childCount - 1; i >= 0; i--) DestroyImmediate(transform.GetChild(i).gameObject);

        //transform.localPosition = new Vector2(-(gridSize.x * unitSize) / 2, -(gridSize.y * unitSize) / 2);

        // Generate new tiles
        for (int x = 0; x < gridSize.x; x++) {
            for (int y = 0; y < gridSize.y; y++) {
                int index = Random.Range(0, groundSprites.Length);

                GameObject obj = new($"Ground Tile #{x}_{y}");
                obj.transform.parent = transform;
                obj.transform.localPosition = new Vector2(unitSize * x, unitSize * y);

                SpriteRenderer spriteRenderer = obj.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = groundSprites[index];
            }
        }
    }
}
