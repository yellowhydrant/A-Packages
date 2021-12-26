//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.InputSystem;

//[AddComponentMenu("A/Building/Builder")]
//public class ABuilder : MonoBehaviour
//{
//    [SerializeField] float range = 4;

//    [SerializeField] float cellSize = 3;
//    [SerializeField] Vector3Int activeCell;
//    [SerializeField] Vector3 activeCellCenter => (Vector3)activeCell * cellSize - new Vector3(cellSize / 2, 0, cellSize / 2);

//    [SerializeField] LayerMask mask;

//    [SerializeField] bool checkForConnections = false;
//    [SerializeField] bool clampHeight = true;
//    [SerializeField] float minHeight = 0;
//    [SerializeField] float maxHeight = float.MaxValue;

//    Dictionary<Vector3Int, Positions> gridCells = new Dictionary<Vector3Int, Positions>();

//    Camera cam;
//    Color color = Color.yellow;

//    bool buildInput;
//    int scrollInput;

//    Vector3Int[] offsets = new Vector3Int[] { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right, Vector3Int.forward, Vector3Int.back };

//    [System.Serializable, System.Flags]
//    public enum Positions
//    {
//        None = 0,
//        BottomForward = 1 << 0,
//        BottomBack = 1 << 1,
//        BottomLeft = 1 << 2,
//        BottomRight = 1 << 3,
//        TopForward = 1 << 4,
//        TopBack = 1 << 5,
//        TopLeft = 1 << 6,
//        TopRight = 1 << 7
//    }

//    private void Start()
//    {
//        cam = Camera.main;
//    }

//    void OnFire(InputValue value) => buildInput = value.isPressed;

//    void OnScroll(InputValue value) => scrollInput = (int)Mathf.Sign(value.Get<float>());

//    public void Update()
//    {
//        var pos = transform.position + (cam.transform.forward * range);
//        if (clampHeight)
//            pos.y = Mathf.Clamp(pos.y, minHeight, maxHeight);

//        activeCell = GetCellFromPos(pos);
//            //isGrounded = Physics.OverlapBox(activeCellCenter + Vector3.up * (cellSize / 2), (Vector3.one * (cellSize / 2)), Quaternion.identity, mask).Length < 0;
//        if (checkForConnections || IsCellConnected(activeCell))
//        {
//            color = Color.yellow;
//        }
//        else
//        {
//            color = Color.magenta;
//        }
//    }

//    public void SetPositions(Vector3Int cell, Positions positions)
//    {
//        if (!gridCells.ContainsKey(cell) && positions != Positions.None)
//        {
//            gridCells.Add(cell, positions);
//        }
//        else
//        {
//            if (positions == Positions.None)
//                gridCells.Remove(cell);
//            else
//                gridCells[cell] = positions;
//        }
//    }

//    public bool CanBuild()
//    {
//        return false;
//    }

//    public bool IsCellConnected(Vector3Int cell, Positions cellPositions = Positions.None)
//    {
//        cellPositions = cellPositions == Positions.None ? GetPositions(cell) : cellPositions;
//        var isConnected = false;
        
//        return isConnected;
//    }

//    public Positions GetPositions(Vector3Int cell) => gridCells.ContainsKey(cell) ? gridCells[cell] : Positions.None;

//    Vector3Int GetCellFromPos(Vector3 pos) => new Vector3Int(Mathf.CeilToInt(pos.x / cellSize), Mathf.CeilToInt((pos.y - cellSize * .8f) / cellSize), Mathf.CeilToInt(pos.z / cellSize));

//    private void OnDrawGizmos()
//    {
//        Gizmos.color = color;
//        Gizmos.DrawWireCube(activeCellCenter + Vector3.up * (cellSize / 2), Vector3.one * cellSize);
//        Gizmos.DrawCube(activeCellCenter, new Vector3(cellSize / 4, cellSize/20, cellSize /4));
//        Gizmos.DrawCube(activeCellCenter + (Vector3.left + Vector3.up) * (cellSize / 2), new Vector3(cellSize / 20, cellSize / 4, cellSize / 4));
//        Gizmos.DrawCube(activeCellCenter + (Vector3.right + Vector3.up) * (cellSize / 2), new Vector3(cellSize / 20, cellSize / 4, cellSize / 4));
//        Gizmos.DrawCube(activeCellCenter + (Vector3.forward + Vector3.up) * (cellSize / 2), new Vector3(cellSize / 4, cellSize / 4, cellSize / 20));
//        Gizmos.DrawCube(activeCellCenter + (Vector3.back + Vector3.up) * (cellSize / 2), new Vector3(cellSize / 4, cellSize / 4, cellSize / 20));
//        Gizmos.DrawCube(activeCellCenter + (Vector3.up) * (cellSize / 2), new Vector3(cellSize / 8, cellSize / 8, cellSize / 8));
//    }
//}

