using UnityEngine;

public class ActivePiece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }

    // tilemaps use vector3Ints so these can't be vector2's
    public Vector3Int position { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public int rotationIndex { get; private set; }

    // move time delay vars
    [SerializeField] private float moveRate = 10f;
    private float nextMoveTime = 0f;

    private void Update()
    {
        board.Clear(this);
        CheckMoveInput();
        board.Set(this);
    }

    public void Initialize(Board board, TetrominoData data, Vector3Int position)
    {
        this.board = board;
        this.position = position;
        this.data = data;
        rotationIndex = 0;

        if (cells == null)
        {
            cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < data.cells.Length; i++)
        {
            cells[i] = (Vector3Int)data.cells[i];
        }
    }    

    private void CheckMoveInput()
    {
        // check attack speed timer to see if you can attack again
        // each move will add 1f to the timer divided by move rate
        if (Time.time >= nextMoveTime)
        {
            if (Input.GetButton("Horizontal") && Input.GetAxisRaw("Horizontal") > 0)
            {
                // Move the piece right
                Move(Vector2Int.right);
                nextMoveTime = Time.time + 1f / moveRate;
            }
            else if (Input.GetButton("Horizontal") && Input.GetAxisRaw("Horizontal") < 0)
            {
                // Move the piece left
                Move(Vector2Int.left);
                nextMoveTime = Time.time + 1f / moveRate;
            }
            else if (Input.GetButton("Vertical") && Input.GetAxisRaw("Vertical") > 0)
            {
                // Move the piece up
                Move(Vector2Int.up);
                nextMoveTime = Time.time + 1f / moveRate;
            }
            else if (Input.GetButton("Vertical") && Input.GetAxisRaw("Vertical") < 0)
            {
                // Move the piece down
                Move(Vector2Int.down);
                nextMoveTime = Time.time + 1f / moveRate;
            }

            if (Input.GetButtonDown("Fire1"))
            {
                Rotate(-1);
            }
            else if (Input.GetButtonDown("Fire2"))
            {
                Rotate(1);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Lock();
            }
        }
    }

    private void Rotate(int direction)
    {
        int originalRotation = rotationIndex;
        rotationIndex += Wrap((rotationIndex + direction), 0, 4);

        ApplyRotationMatrix(direction);

        // if rotation would push piece off the board rotate the other way instead
        if (!TestWallKicks(rotationIndex, direction))
        {
            rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

    private void ApplyRotationMatrix(int direction)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            int x, y;

            Vector3 cell = cells[i];

            switch (data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
            }

            cells[i] = new Vector3Int(x, y, 0);
        }
    }

    // checks if active piece rotation would place a block outside gameboard
    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickidx = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = data.wallKicks[wallKickidx, i];

            if (Move(translation))
            {
                return true;
            }
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIdx = rotationIndex * 2;

        if (rotationDirection < 0 )
        {
            wallKickIdx--;
        }

        return Wrap(wallKickIdx, 0, data.wallKicks.GetLength(0));
    }

    // rotate between positions 1 - 4
    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        {
            return min + (input - min) % (max - min);
        }
    }

    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = board.IsValidPosition(this, newPosition);

        if (valid)
        {
            position = newPosition;
        }

        return valid;
    }

    // lock piece into place on the gameboard
    private void Lock()
    {
        board.Set(this);
        board.SpawnPiece();
    }
}
