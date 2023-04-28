using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public ActivePiece activePiece { get; private set; }
    public TetrominoData[] tetrominos;
    public Vector3Int spawnPosition;
    public Vector2Int boardSize = new Vector2Int(10, 20);

    public RectInt boardBounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<ActivePiece>();

        for (int i = 0; i < tetrominos.Length; i++)
        {
            tetrominos[i].Initialize();
        }
    }

    private void Start()
    {
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        int random = Random.Range(0, this.tetrominos.Length); 
        TetrominoData data = this.tetrominos[random];

        activePiece.Initialize(this, data, spawnPosition);
        Set(activePiece);
    }

    // set piece onto the board
    public void Set(ActivePiece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            // tile position is piece cells local/shape relative position + world relative position
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    // remove piece from the board
    public void Clear(ActivePiece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            // tile position is piece cells local/shape relative position + world relative position
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    // check if location is valid for active piece
    public bool IsValidPosition(ActivePiece piece, Vector3Int position)
    {
        RectInt bounds = boardBounds;

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            // if out bounds of gameboard
            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }

            // check if space is already occupied
            if (tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }
}
