using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapGenerator : NetworkBehaviour {
    public NetworkVariable<int> Seed = new();

    [SerializeField] private Vector2Int mapSize;
    [SerializeField] private Vector2Int allowedDistanceToBorders;
    [SerializeField] private float allowedDistanceToBuildings;
    [SerializeField] private int minBuildings;
    [SerializeField] private int maxBuildings;
    [SerializeField] private int iterationFailSafe;
    [SerializeField] private Transform[] buildingPrefabs;

    private List<Transform> spawnedBuildings = new();

    public override void OnNetworkSpawn() {
        if (IsServer) Seed.Value = Random.Range(0, 1000000);
        Random.InitState(Seed.Value);

        GeneratorMap(Seed.Value);
    }

    private void Update() {
        if (Keyboard.current.kKey.wasPressedThisFrame) GeneratorMap(Seed.Value);
    }

    public void GeneratorMap(int seed) {
        Debug.Log(seed);

        RemoveAllBuildings();

        int numOfBuildings = Random.Range(minBuildings, maxBuildings);

        for (int i = 0; i < numOfBuildings; i++) {
            Vector2Int position = GetNewBuildingPosition();

            if (position == Vector2Int.zero) continue;

            int buildingIndex = Random.Range(0, buildingPrefabs.Length);
            Transform newBuilding = Instantiate(buildingPrefabs[buildingIndex], transform);
            newBuilding.position = new Vector3(position.x, position.y, 0);
            spawnedBuildings.Add(newBuilding);
        }
    }

    public Vector2Int GetNewBuildingPosition() {
        int iterations = 0;
        while (iterations < iterationFailSafe) {
            Vector2Int position = new(Random.Range(0, mapSize.x), Random.Range(0, mapSize.y));

            if (IsWithinBorders(position) && IsFarFromBuildings(position)) return position;

            iterations++;
        }

        return Vector2Int.zero;
    }

    public void RemoveAllBuildings() {
        for (int i = 0; i < spawnedBuildings.Count; i++)
            Destroy(spawnedBuildings[i].gameObject);

        spawnedBuildings.Clear();
    }

    public bool IsWithinBorders(Vector2Int position) {
        bool xWithin = position.x - allowedDistanceToBorders.x >= 0 && position.x + allowedDistanceToBorders.x <= mapSize.x;
        bool yWithin = position.y - allowedDistanceToBorders.y >= 0 && position.y + allowedDistanceToBorders.y <= mapSize.y;
        return xWithin && yWithin;
    }

    public bool IsFarFromBuildings(Vector2Int position) {
        if (spawnedBuildings.Count == 0) return true;

        bool isFar = true;
        for (int i = 0; i < spawnedBuildings.Count; i++) {
            if (Vector2Int.Distance(position, Vector3To2Int(spawnedBuildings[i].position)) < allowedDistanceToBuildings)
                isFar = false;
        }

        return isFar;
    }

    public Vector2Int Vector3To2Int(Vector3 vector3) => new((int)vector3.x, (int)vector3.y);
}
