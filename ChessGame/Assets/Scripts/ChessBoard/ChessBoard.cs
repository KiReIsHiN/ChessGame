namespace ChessBoardModel
{
    using System;
    using UnityEngine;

    public class ChessBoard : MonoBehaviour
    {
        [Header("Art Stuff")]
        [SerializeField] private Material _tileMaterial;
        [SerializeField] private float _tileSize = 1.0f;
        [SerializeField] private float _yOffset = 0.2f;
        [SerializeField] private Vector3 _boardCenter = Vector3.zero;

        [Header("Prefabs && Materials")]
        [SerializeField] private GameObject[] _prefabs;
        [SerializeField] private Material[] _teamMaterials;

        //LOGIC
        private ChessPiece[,] _chessPieces;
        private const int TILE_COUNT_X = 8;
        private const int TILE_COUNT_Y = 8;
        private GameObject[,] _tiles;

        private Camera _currentCamera;

        private Vector2Int _currentHover;
        private Vector3 _bounds;

        private void Awake()
        {
            GenerateAllTiles(_tileSize, TILE_COUNT_X, TILE_COUNT_Y);

            SpawnAllPieces();

            PositionAllPiece();
        }

        private void Update()
        {
            OnMousePressed();
        }

        private void OnMousePressed()
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (!_currentCamera)
                {
                    _currentCamera = Camera.main;
                    return;
                }

                RaycastHit info;
                Ray ray = _currentCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile")))
                {
                    //Get indexes of the tile we hit
                    Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);

                    //If we are hovering a tile after not hovering any tiles
                    if (_currentHover == -Vector2Int.one)
                    {
                        _currentHover = hitPosition;
                        _tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                    }

                    //If we were already hovering a tile, change the previous one
                    if (_currentHover != hitPosition)
                    {
                        _tiles[_currentHover.x, _currentHover.y].layer = LayerMask.NameToLayer("Tile");
                        _currentHover = hitPosition;
                        _tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                    }
                }
                else
                {
                    if (_currentHover != -Vector2Int.one)
                    {
                        _tiles[_currentHover.x, _currentHover.y].layer = LayerMask.NameToLayer("Tile");
                        _currentHover = -Vector2Int.one;
                    }
                }
            }
        }

        //Generate the board
        private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
        {
            _yOffset += transform.position.y;
            _bounds = new Vector3((tileCountX / 2) * tileSize, 0, (tileCountX / 2) * tileSize) + _boardCenter;

            _tiles = new GameObject[tileCountX, tileCountY];
            for (int x = 0; x < tileCountX; x++)
                for (int y = 0; y < tileCountY; y++)
                    _tiles[x, y] = GenerateSingleTile(tileSize, x, y);
        }

        private GameObject GenerateSingleTile(float tileSize, int x, int y)
        {
            GameObject tileObject = new GameObject(string.Format("X:{0},Y:{1}", x, y));
            tileObject.transform.parent = transform;

            //Here we create mesh for cells
            Mesh mesh = new Mesh();
            tileObject.AddComponent<MeshFilter>().mesh = mesh;
            tileObject.AddComponent<MeshRenderer>().material = _tileMaterial;

            //Here we create geometry
            Vector3[] vertices = new Vector3[4];
            vertices[0] = new Vector3(x * tileSize, _yOffset, y * tileSize) - _bounds;
            vertices[1] = new Vector3(x * tileSize, _yOffset, (y + 1) * tileSize) - _bounds;
            vertices[2] = new Vector3((x + 1) * tileSize, _yOffset, y * tileSize) - _bounds;
            vertices[3] = new Vector3((x + 1) * tileSize, _yOffset, (y + 1) * tileSize) - _bounds;

            int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };

            //Here we assign vertices
            mesh.vertices = vertices;
            mesh.triangles = tris;

            //Recalculate normals for tile material
            mesh.RecalculateNormals();

            //Here we assign layer mask
            tileObject.layer = LayerMask.NameToLayer("Tile");

            tileObject.AddComponent<BoxCollider>();

            return tileObject;
        }

        //Spawning of the pieces
        private void SpawnAllPieces()
        {
            _chessPieces = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];

            int whiteTeam = 0, blackTeam = 1;

            //White team
            _chessPieces[0, 0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);
            _chessPieces[1, 0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
            _chessPieces[2, 0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
            _chessPieces[3, 0] = SpawnSinglePiece(ChessPieceType.King, whiteTeam);
            _chessPieces[4, 0] = SpawnSinglePiece(ChessPieceType.Queen, whiteTeam);
            _chessPieces[5, 0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
            _chessPieces[6, 0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
            _chessPieces[7, 0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);
            for (int i = 0; i < TILE_COUNT_X; i++)
                _chessPieces[i,1] = SpawnSinglePiece(ChessPieceType.Pawn, whiteTeam);

            //White team
            _chessPieces[0, 7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);
            _chessPieces[1, 7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam);
            _chessPieces[2, 7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
            _chessPieces[3, 7] = SpawnSinglePiece(ChessPieceType.King, blackTeam);
            _chessPieces[4, 7] = SpawnSinglePiece(ChessPieceType.Queen, blackTeam);
            _chessPieces[5, 7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
            _chessPieces[6, 7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam);
            _chessPieces[7, 7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);
            for (int i = 0; i < TILE_COUNT_X; i++)
                _chessPieces[i, 6] = SpawnSinglePiece(ChessPieceType.Pawn, blackTeam);
        }

        private ChessPiece SpawnSinglePiece(ChessPieceType type, int team)
        {
            ChessPiece cp = Instantiate(_prefabs[(int)type - 1], transform).GetComponent<ChessPiece>();

            cp.chessPieceType = type;
            cp.team = team;
            cp.GetComponent<MeshRenderer>().material = _teamMaterials[team];

            return cp;
        }

        //Positioning
        private void PositionAllPiece()
        {
            for (int x = 0; x < TILE_COUNT_X; x++)
                for (int y = 0; y < TILE_COUNT_Y; y++)
                    if (_chessPieces[x, y] != null)
                        PositioningSinglePiece(x, y, true);           
        }

        private void PositioningSinglePiece(int x, int y, bool force = false)
        {
            _chessPieces[x, y].currentX = x;
            _chessPieces[x, y].currentY = y;
            _chessPieces[x, y].transform.position = GetTileCenter(x,y);
        }

        private Vector3 GetTileCenter(int x, int y)
        {
            return new Vector3(x * _tileSize, _yOffset, y * _tileSize) - _bounds + new Vector3(_tileSize/2,0,_tileSize/2);
        }

        //Operations
        private Vector2Int LookupTileIndex(GameObject hitInfo)
        {
            for (int x = 0; x < TILE_COUNT_X; x++)
                for (int y = 0; y < TILE_COUNT_Y; y++)
                    if (_tiles[x, y] == hitInfo)
                        return new Vector2Int(x, y);

            return -Vector2Int.one; //Invalid
        }

    }//End class
}//End namespace
