//using System.Collections;
//using System.Collections.Generic;
//using A;
//using UnityEngine;
//using UnityEngine.InputSystem;

//public class StructureBuilder : MonoBehaviour
//{
//    public int gridSize = 3;

//    [SerializeField] float rayRange = 4;
//    [SerializeField] Transform cameraRoot;
//    [SerializeField] LayerMask mask;

//    public Dictionary<Vector3, GameObject> structureCache = new Dictionary<Vector3, GameObject>();

//    [SerializeField] Structure[] allStrucutres;
//    [SerializeField] Structure selectedStructure;

//    int curIndex;
//    float timePressed;
//    Camera cam;

//    [System.Serializable]
//    public class Structure
//    {
//        public enum Position { side, corner, cap}

//        public Position posType;
//        public GameObject prefab;
//    }

//    private void Awake()
//    {
//        cam = Camera.main;
////        selectedStructure = allStrucutres[0];
//    }

//    public void OnFire(InputValue value)
//    {
//        if (value.isPressed)
//        {
//            BuildStructure();
//        }
//    }

//    //public void OnScroll(InputValue value)
//    //{
//    //    var input = value.Get<float>();
//    //    input = Mathf.Clamp(input, -1, 1);
//    //    if(input != 0)
//    //    {
//    //        if(input > 0)
//    //        {

//    //        }
//    //    }
//    //}

//    void BuildStructure()
//    {
//        var ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
//        RaycastHit hit;
//        var range = Vector3.Distance(cameraRoot.position, cam.transform.position);
//        range += rayRange;
//        if(Physics.Raycast(ray, out hit, range, mask))
//        {
//            var hitTileCenter = new Vector3Int(Mathf.CeilToInt(hit.point.x / gridSize), Mathf.CeilToInt(hit.point.y / gridSize), Mathf.CeilToInt(hit.point.z / gridSize)) * gridSize - new Vector3Int(gridSize / 2, 0, gridSize / 2);
//            var buildPos = GetBuildPos(selectedStructure.posType, hitTileCenter);
//            if(!Build())
//            {
//                if (structureCache.ContainsValue(hit.collider.gameObject))
//                {
//                    hitTileCenter += Vector3Int.up * gridSize * (cameraRoot.position.y < hit.collider.transform.position.y ? -1 : 1);
//                    Build();
//                }
//                else
//                {
//                    //Destroy(value);
//                    //structureCache.Remove(buildPos);
//                }
//            }

//            bool Build()
//            {
//                if (!structureCache.TryGetValue(buildPos, out GameObject value))
//                {
//                    var go = Instantiate(selectedStructure.prefab, buildPos, Quaternion.identity);
//                    structureCache.Add(buildPos, go);
//                    return true;
//                }
//                else
//                {
//                    return false;
//                }
//            }
//        }
//    }

//    IEnumerator DelayedDestroy(GameObject go, float delay)
//    {
//        yield return new WaitForSeconds(delay);
//        Destroy(go);
//    }

//    Vector3 GetBuildPos(Structure.Position posType, Vector3Int centerPos)
//    {
//        switch (posType)
//        {
//            case Structure.Position.side:
//                return centerPos + new Vector3Int(gridSize / 2, 0, 0);
//            case Structure.Position.corner:
//                return centerPos + new Vector3Int(gridSize / 2, 0, gridSize / 2);
//            case Structure.Position.cap:
//                return centerPos;
//            default:
//                return Vector3.zero;
//        }
//    }

//    Vector3 RotatePoint(Vector3 point, Vector3Int centerPos)
//    {
//        return AMath.RotatePointAroundPivot(point, centerPos, Vector3.forward * 90f);
//    }
//}
