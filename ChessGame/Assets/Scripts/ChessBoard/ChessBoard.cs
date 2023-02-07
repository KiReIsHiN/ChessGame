namespace ChessBoardModel
{
    using System;
    using UnityEngine;

    public class ChessBoard : MonoBehaviour
    {
        [Header("Art Stuff")]
        [SerializeField] private Material tileMaterial;

        //LOGIC
        private const int TILE_COUNT_X = 8;
        private const int TILE_COUNT_Y = 8;
        private GameObject[,] tiles;

        private Camera currentCamera;

        private Vector2Int currentHover;

        private void Awake()
        {
            GenerateAllTiles(1, TILE_COUNT_X, TILE_COUNT_Y);
        }

        private void Update()
        {

            if (!currentCamera)
            {
                currentCamera = Camera.main;
                return;
            }

            RaycastHit info;
            Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile")))
            {
                //Get indexes of the tile we hit
                Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);

                //If we are hovering a tile after not hovering any tiles
                if (currentHover == -Vector2Int.one)
                {
                    currentHover = hitPosition;
                    tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                }

                //If we were already hovering a tile, change the previous one
                if (currentHover != hitPosition)
                {
                    tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                    currentHover = hitPosition;
                    tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                }
            }
            else
            {
                if (currentHover != -Vector2Int.one)
                {
                    tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                    currentHover = -Vector2Int.one;
                }
            }
        }

        //Generate the board
        private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
        {
            tiles = new GameObject[tileCountX, tileCountY];
            for (int x = 0; x < tileCountX; x++)
                for (int y = 0; y < tileCountY; y++)
                    tiles[x, y] = GenerateSingleTile(tileSize, x, y);
        }
        private GameObject GenerateSingleTile(float tileSize, int x, int y)
        {
            GameObject tileObject = new GameObject(string.Format("X:{0},Y:{1}", x, y));
            tileObject.transform.parent = transform;

            //Here we create mesh for cells
            Mesh mesh = new Mesh();
            tileObject.AddComponent<MeshFilter>().mesh = mesh;
            tileObject.AddComponent<MeshRenderer>().material = tileMaterial;

            //Here we create geometry
            Vector3[] vertices = new Vector3[4];
            vertices[0] = new Vector3(x * tileSize, 0, y * tileSize);
            vertices[1] = new Vector3(x * tileSize, 0, (y + 1) * tileSize);
            vertices[2] = new Vector3((x + 1) * tileSize, 0, y * tileSize);
            vertices[3] = new Vector3((x + 1) * tileSize, 0, (y + 1) * tileSize);

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

        //Operations
        private Vector2Int LookupTileIndex(GameObject hitInfo)
        {
            for (int x = 0; x < TILE_COUNT_X; x++)
                for (int y = 0; y < TILE_COUNT_Y; y++)
                    if (tiles[x, y] == hitInfo)
                        return new Vector2Int(x, y);

            return -Vector2Int.one; //Invalid
        }

    }//End class
}//End namespace
