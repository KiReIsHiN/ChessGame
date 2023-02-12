using UnityEngine;

public enum ChessPieceType
{
    None = 0,
    Pawn = 1,
    Rook = 2,
    Knight = 3,
    Bishop = 4,
    Queen = 5,
    King = 6
}

public class ChessPiece : MonoBehaviour
{
    public float speed;

    public int team;
    public int currentX;
    public int currentY;
    public ChessPieceType chessPieceType;

    private Vector3 _desiredPosition;
    private Vector3 _desiredScale = Vector3.one;

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, _desiredPosition, Time.deltaTime * speed);
        transform.localScale = Vector3.Lerp(transform.localScale, _desiredScale, Time.deltaTime * 10);
    }

    public virtual void SetPosition(Vector3 position, bool force = false)
    {
        _desiredPosition = position;
        if (force)
        {
            transform.position = _desiredPosition;
        }
    }

    public virtual void SetScale(Vector3 scale, bool force = false)
    {
        _desiredScale = scale;
        if (force)
            transform.localScale = _desiredScale;
    }
}
