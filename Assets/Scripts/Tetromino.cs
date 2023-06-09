using UnityEngine;
using UnityEngine.Tilemaps;

public enum Tetromino
{
    I, J, L, O, S, T, Z, HH, VV, DU, DD, HHH, VVV, DUU, DDD, X
}

[System.Serializable]
public struct TetrominoData
{
    public Tile tile;
    public Tetromino tetromino;

    // x,y for individual block locations to create shapes
    // could make them a serialzed field to test/create new shapes
    //public Vector2Int[] cells { get; private set; }
    [SerializeField]
    public Vector2Int[] cells;
    public Vector2Int[,] wallKicks { get; private set; }

    public void Initialize()
    {
        cells = Data.Cells[tetromino];
        wallKicks = Data.WallKicks[tetromino];
    }

}
