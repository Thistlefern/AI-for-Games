using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public int gridWidth;
    public int gridHeight;

    public GameObject prefab;

    public Tile[] tiles;

    private void Start()
    {
        tiles = new Tile[gridWidth * gridHeight];

        Vector3 offset = Vector3.zero;

        // spawn the tiles
        for (int i = 0; i < gridHeight; ++i)
        {
            for (int j = 0; j < gridWidth; ++j)
            {
                GameObject newTile = Instantiate(prefab, transform.position + offset, transform.rotation);
                tiles[i * gridWidth + j] = newTile.GetComponent<Tile>();
                tiles[i * gridWidth + j].tilePos = new Vector2Int(j, i);

                newTile.name = string.Format("{0}, {1}", j, i);
                offset.x += 1.0f;
            }

            offset.x = 0.0f;
            offset.z += 1.0f;
        }

        // setup connections
        for (int i = 0; i < tiles.Length; ++i)
        {
            List<Tile> connectedTiles = new List<Tile>();

            // if we're not at the left-edge
            if (i % gridWidth != 0)
            {
                connectedTiles.Add(tiles[i - 1]);
            }

            // if we're not at the right-edge
            if ((i + 1) % gridWidth != 0)
            {
                connectedTiles.Add(tiles[i + 1]);
            }

            // if we're not at the upper-edge
            if (i < gridWidth * gridHeight - gridWidth)
            {
                connectedTiles.Add(tiles[i + gridWidth]);
            }

            // if we're not at the bottom-edge
            if (i > gridWidth - 1)
            {
                connectedTiles.Add(tiles[i - gridWidth]);
            }

            tiles[i].connectedTiles = connectedTiles.ToArray();
        }
    }

    private Tile GetCheapestTile(Tile[] arr)
    {
        float bestScore = float.MaxValue;
        Tile bestTile = null;

        for (int i = 0; i < arr.Length; ++i)
        {
            if (arr[i].fScore < bestScore)
            {
                bestTile = arr[i];
                bestScore = arr[i].fScore;
            }
        }

        return bestTile;
    }

    private int GetHScoreDjikstra(Tile selectedTile, Tile destinationTile)
    {
        return 0;
    }

    private int GetHScoreManhattan(Tile selectedTile, Tile destinationTile)
    {
        Vector2Int tileOffset = selectedTile.tilePos - destinationTile.tilePos;
        return System.Math.Abs(tileOffset.x) + System.Math.Abs(tileOffset.y);
    }

    private void ResetNodes()
    {
        for (int i = 0; i < tiles.Length; ++i)
        {
            tiles[i].gScore = 0;
            tiles[i].prevTile = null;
        }
    }

    public Tile[] CalculatePath(Tile origin, Tile destination)
    {
        // clear data on nodes from last calculation
        ResetNodes();

        // for tiles to be processed
        List<Tile> openList = new List<Tile>();
        // for tiles that have or are currently being procssed
        List<Tile> closedList = new List<Tile>();

        openList.Add(origin);

        while (openList.Count != 0 &&        // still stuff left to explore
        !closedList.Contains(destination))   // AND we haven't reached the destination yet
        {
            // TODO: replace this with a proper sorted array implementation
            Tile current = GetCheapestTile(openList.ToArray());
            openList.Remove(current);
            closedList.Add(current);

            // iterate through all connected tiles
            for (int i = 0; i < current.connectedTiles.Length; ++i)
            {
                // create variable for to current connected tile for readability
                Tile adjTile = current.connectedTiles[i];

                // skip tiles that were already processed or not traversible
                if (closedList.Contains(adjTile) || !adjTile.traversible) { continue; }

                // NOTE: hard-coded cost of 1
                int estGScore = current.gScore + 1;

                if (adjTile.prevTile == null ||     // there is no score (no previous tile) OR
                    estGScore < adjTile.gScore)         // this is a cheaper route...
                {
                    adjTile.prevTile = current;
                    adjTile.gScore = estGScore;
                    adjTile.hScore = GetHScoreManhattan(adjTile, destination);
                }

                if (!closedList.Contains(adjTile) && // if a connected tile is not in the closed list AND
                    !openList.Contains(adjTile))    // is not *already* in the open list
                {
                    openList.Add(adjTile);
                }
            }
        }

        List<Tile> path = new List<Tile>();

        if (closedList.Contains(destination))
        {
            Tile prevTile = destination;
            while (prevTile != origin)
            {
                path.Add(prevTile);
                prevTile = prevTile.prevTile;
            }
            path.Add(prevTile);
        }

        // reverse order from "destination to start" to "start to destination"
        path.Reverse();

        return path.ToArray();
    }
}
