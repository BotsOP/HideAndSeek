using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSnap : MonoBehaviour
{
    [SerializeField] private Vector3 gridSize = default;

    private void OnDrawGizmos()
    {
        if(!Application.isPlaying && this.transform.hasChanged)
            SnapToGrid();
    }

    private void SnapToGrid()
    {
        Vector3 position = new Vector3(
            Mathf.Round(this.transform.position.x / this.gridSize.x) * this.gridSize.x,
            Mathf.Round(this.transform.position.y / this.gridSize.y) * this.gridSize.y,
            Mathf.Round(this.transform.position.z / this.gridSize.z) * this.gridSize.z
        );

        this.transform.position = position;
    }
}
