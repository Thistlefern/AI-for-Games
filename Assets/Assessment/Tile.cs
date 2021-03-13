using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int tilePos { get; set; }
    public Tile prevTile;
    public Tile[] connectedTiles;

    public int gScore;
    public int hScore;
    public int fScore { get { return gScore + hScore; } }

    public bool traversible = true;

    private void OnDrawGizmosSelected()
    {
        if (connectedTiles == null) { return; }

        Gizmos.color = Color.magenta;

        Vector3 drawOffset = new Vector3(0, 1.5f, 0.0f);

        for (int i = 0; i < connectedTiles.Length; ++i)
        {
            Gizmos.DrawLine(transform.position + drawOffset,
                           connectedTiles[i].transform.position + drawOffset);
        }
    }
}
