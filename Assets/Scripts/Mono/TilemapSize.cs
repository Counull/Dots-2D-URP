using UnityEngine;
using UnityEngine.Tilemaps;

namespace Mono {
    public class TilemapSize : MonoBehaviour {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start() {
            var tilemap = GetComponent<Tilemap>();
            if (tilemap != null) {
                var bounds = tilemap.cellBounds;
                var minPoint = bounds.min;
                var maxPoint = bounds.max;

                Debug.Log("Tilemap Min Point: " + minPoint);
                Debug.Log("Tilemap Max Point: " + maxPoint);
            }
            else {
                Debug.LogError("Tilemap is not assigned.");
            }
        }

        // Update is called once per frame
        private void Update() { }
    }
}