using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapSize : MonoBehaviour {
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        Tilemap tilemap = GetComponent<Tilemap>();
        if (tilemap != null) {
            BoundsInt bounds = tilemap.cellBounds;
            Vector3Int minPoint = bounds.min;
            Vector3Int maxPoint = bounds.max;

            Debug.Log("Tilemap Min Point: " + minPoint);
            Debug.Log("Tilemap Max Point: " + maxPoint);
        } else {
            Debug.LogError("Tilemap is not assigned.");
        }
    }

    // Update is called once per frame
    void Update() { }
}