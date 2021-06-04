using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DockManager : MonoBehaviour
{
    public enum ViewType { firstPerson, orbitCamera };
    public ViewType view;
    public enum Mode { place, delete };
    public Mode mode;
    public Ship ship;
    public GameObject blockToPlace;
    public List<GameObject> availableBlocks;
    public Camera mainCam;
    public bool focus = true;
    public GameObject inactive;

    void Awake()
    {
        mainCam = Camera.main;
        if (GameObject.FindObjectOfType<Ship>())
        {
            ship = GameObject.FindObjectOfType<Ship>();
        }
        else
        {
            ship = new GameObject("").AddComponent<Ship>();
        }

        if (GameObject.FindGameObjectWithTag("BlockToPlace"))
        {
            blockToPlace = GameObject.FindGameObjectWithTag("BlockToPlace");
            blockToPlace.transform.position = Vector3.one * 99999;
            if (blockToPlace.transform.childCount == 0)
            {
                GameObject tempBlock = Instantiate(availableBlocks[0]);
                tempBlock.transform.parent = blockToPlace.transform;
                tempBlock.transform.localPosition = Vector3.zero;
            }
        }
        else
        {
            blockToPlace = new GameObject("Block To Place");
            blockToPlace.transform.position = Vector3.one * 99999;
            blockToPlace.tag = "BlockToPlace";
            GameObject tempBlock = Instantiate(availableBlocks[0]);
            tempBlock.transform.parent = blockToPlace.transform;
            tempBlock.transform.localPosition = Vector3.zero;
        }

        blockToPlace.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;

        InstantiateShip();
    }

    void InstantiateShip()
    {
        GameObject firstBlock = Instantiate(availableBlocks[0]);
        firstBlock.transform.position = Vector3.zero;
        firstBlock.layer = 6;
        ship.AddBlock(firstBlock);
        firstBlock.transform.parent = inactive.transform;
        firstBlock.SetActive(false);
    }

    void Update()
    {
        if (focus == true)
        {
            if (mode == Mode.place)
            {
                Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;

                LayerMask objectMask = LayerMask.GetMask("PlacedBlock");

                if (Physics.Raycast(ray, out hitInfo, 150, objectMask, QueryTriggerInteraction.Ignore))
                {
                    Vector3 newPosition = new Vector3((hitInfo.transform.position.x), (hitInfo.transform.position.y), (hitInfo.transform.position.z))
                                        + new Vector3((hitInfo.normal.x), (hitInfo.normal.y), (hitInfo.normal.z));
                    blockToPlace.transform.position = newPosition;
                }
                else
                {
                    blockToPlace.transform.position = Vector3.one * 10000;
                }

                //Check mouse pressed this frame
                if (Input.GetMouseButtonDown(0))
                {
                    if (view == ViewType.orbitCamera)
                    {
                        if (blockToPlace.transform.position != Vector3.one * 10000)
                        {
                            bool intersects = CheckIntersects();
                            if (intersects == false)
                            {
                                GameObject currentObject = blockToPlace.transform.GetChild(0).gameObject;
                                GameObject newBlock = Instantiate(currentObject as GameObject) as GameObject;
                                newBlock.name = blockToPlace.transform.GetChild(0).gameObject.name + blockToPlace.transform.position;
                                newBlock.transform.position = blockToPlace.transform.position;
                                newBlock.layer = 6;
                                newBlock.tag = "ShipPart";
                                SetLayerRecursively(newBlock, 6);
                                newBlock.transform.parent = inactive.transform;
                                newBlock.transform.rotation = blockToPlace.transform.rotation;
                                newBlock.GetComponent<MeshRenderer>().enabled = false;
                                ship.AddBlock(newBlock);
                            }
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    blockToPlace.transform.Rotate(Vector3.up * 90, Space.World);
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    blockToPlace.transform.Rotate(Vector3.down * 90, Space.World);
                }
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    blockToPlace.transform.Rotate(Vector3.left * 90, Space.World);
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    blockToPlace.transform.Rotate(Vector3.right * 90, Space.World);
                }
            }
            else if (mode == Mode.delete)
            {
                if (blockToPlace.transform.position != Vector3.one * 10000)
                {
                    blockToPlace.transform.position = Vector3.one * 10000;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    if (view == ViewType.orbitCamera)
                    {
                        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hitInfo;

                        LayerMask objectMask = LayerMask.GetMask("PlacedBlock");

                        if (Physics.Raycast(ray, out hitInfo, 150, objectMask, QueryTriggerInteraction.Collide))
                        {
                            RemoveBlock(hitInfo.transform.gameObject);
                        }
                    }
                }
            }
        }
    }

        void RemoveBlock(GameObject obj)
    {

        if (obj.transform.parent.childCount <= 1)
        {
            return;
        }

        ship.Remove(obj);
        Destroy(obj);
    }

    public bool CheckIntersects()
    {
        GameObject shape = blockToPlace.transform.GetChild(0).gameObject;
        if (shape.transform.childCount > 2)
        {
            Vector3[] positions = new Vector3[shape.transform.childCount - 2];
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = blockToPlace.transform.GetChild(0).transform.GetChild(i + 2).transform.position;
            }

            List<Vector3> shipParts = GetShipVoxelPositions();
            foreach (Vector3 position in shipParts)
            {
                for (int i = 0; i < positions.Length; i++)
                {
                    if (position == positions[i])
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    List<Vector3> GetShipVoxelPositions()
    {
        List<Vector3> parts = new List<Vector3>();

        GameObject[] shipParts = GameObject.FindGameObjectsWithTag("ShipPart");
        foreach (GameObject part in shipParts)
        {
            for (int i = 1; i < part.transform.childCount; i++)
            {
                parts.Add(part.transform.GetChild(i).transform.position);
            }
        }

        return parts;
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}

