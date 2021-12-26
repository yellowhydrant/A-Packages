using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

[AddComponentMenu("A/Building/OpenWorld/Structure Builder")]
public class AStructureBuilder : MonoBehaviour
{
    [SerializeField] float range = 4;
    [SerializeField] LayerMask mask;

    [SerializeField] float cellSize = 3;
    [SerializeField] Vector3Int activeCell;
    [SerializeField] Vector3 activeCellCenter => (Vector3)activeCell * cellSize - new Vector3(cellSize / 2, 0, cellSize / 2);

    [SerializeField] bool checkForConnections = true;
    [SerializeField] bool clampHeight = true;
    [SerializeField] float minHeight = 0;
    [SerializeField] float maxHeight = 300;

    [SerializeField] AStructure[] structures;
    int selectedIndex = 0;
    int prevSelectedIndex = int.MaxValue;

    Camera cam;
    Color color;

    AStructure decoyStructure;

    bool buildInput;

    private void Start()
    {
        cam = Camera.main;
        for (int i = 0; i < structures.Length; i++)
        {
            structures[i].transform.localScale = Vector3.Scale(structures[i].scaleFactor, Vector3.one * cellSize);
        }
        SpawnDecoyStructure();
    }

    void OnFire(InputValue value) => buildInput = value.isPressed;

    void OnScroll(InputValue value)
    {
        var val = (int)Mathf.Sign(value.Get<float>());
        if(val == 1)
        {
            if (selectedIndex < structures.Length - 1)
                selectedIndex++;
            else
                selectedIndex = 0;
            SpawnDecoyStructure();
        }
        else if(val == -1)
        {
            if (selectedIndex > 0)
                selectedIndex--;
            else
                selectedIndex = structures.Length - 1;
            SpawnDecoyStructure();
        }
    }

    public void Update()
    {
        var pos = transform.position + (cam.transform.forward * range);
        if (clampHeight)
            pos.y = Mathf.Clamp(pos.y, minHeight, maxHeight);
        activeCell = GetCellFromPos(pos);
        decoyStructure.transform.position = activeCellCenter + structures[selectedIndex].offset;
        // -> //FIXME: Doesn't ignore player collider // possible solution: place player on ignoreraycast layer and use it for masking
        //TODO: Add recursive checks for connections and destroy any disconnected tiles
        // maybe keep track of the connections and just destroy everything connected to the destroyed structure
        //TODO: Add ability to destroy structures
        //TODO: Add ability to rotate structures
        //TODO: Add ability to modify structures
        if (decoyStructure.CanPlace(mask) && (!checkForConnections || decoyStructure.IsConnected()))
        {
            color = Color.green;
            if (buildInput)
            {
                Instantiate(structures[selectedIndex], activeCellCenter + structures[selectedIndex].offset, Quaternion.identity);
            }
        }
        else
        {
            color = Color.red;
        }
    }

    Vector3Int GetCellFromPos(Vector3 pos) => new Vector3Int(Mathf.CeilToInt(pos.x / cellSize), Mathf.CeilToInt((pos.y - cellSize * .8f) / cellSize), Mathf.CeilToInt(pos.z / cellSize));

    void SpawnDecoyStructure()
    {
        if (prevSelectedIndex != selectedIndex)
        {
            if (decoyStructure != null)
                Destroy(decoyStructure);
            decoyStructure = Instantiate(structures[selectedIndex]);
            decoyStructure.structureCollider.isTrigger = true;
            prevSelectedIndex = selectedIndex;
        }
    }
}


