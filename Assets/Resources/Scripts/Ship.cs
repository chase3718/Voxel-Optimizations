using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour {
    public string alias;
    public List<GameObject> blocks = new List<GameObject>();
    public float horsePower;
    public float estimatedSpeed;
    public float maximumFireRange;
    public float maxHealth;
    public float armor;
    public Vector3 centerOfMass;
    public float displacement;
    public float density;
    public float volume;
    public float length;
    public float beam;
    public float height;
    public float estimatedDraft;
    public float sight;
    public float cost;
    public float mass {
        get => volume * density * 1000;
    }
    public GameObject shipRenderer;
    public ShipMesh shipMesh;

    void Awake() {
        if (this.alias == "") {
            this.alias = this.gameObject.name;
        } else {
            this.gameObject.name = this.alias;
        }
        if (GameObject.FindGameObjectWithTag("ShipMesh")) {
            shipRenderer = GameObject.FindGameObjectWithTag("shipRenderer");
            if (shipRenderer.GetComponent<MeshRenderer>() == null) shipRenderer.AddComponent<MeshRenderer>();
            if (shipRenderer.GetComponent<MeshFilter>() == null) shipRenderer.AddComponent<MeshFilter>();
            if (shipRenderer.GetComponent<MeshCollider>() == null) shipRenderer.AddComponent<MeshCollider>();
            if (shipRenderer.GetComponent<ShipMesh>() == null) shipRenderer.AddComponent<ShipMesh>();
        } else {
            shipRenderer = new GameObject("Ship Mesh");
            shipRenderer.tag = "ShipMesh";
            shipRenderer.AddComponent<MeshRenderer>();
            shipRenderer.AddComponent<MeshFilter>();
            shipRenderer.AddComponent<MeshCollider>();
            shipRenderer.AddComponent<ShipMesh>();
        }

        shipRenderer.transform.parent = transform;

        shipMesh = shipRenderer.GetComponent<ShipMesh>();
    }

    public Ship() {
        this.alias = "Unnamed Ship";
        
    }

    public Ship(string _alias) {
        this.alias = _alias;
    }

    public void AddBlock(GameObject _block) {
        blocks.Add(_block);
        shipMesh.AddBlock(_block);
        GameObject newBlock = Instantiate(_block);
        newBlock.transform.parent = shipMesh.transform;
        newBlock.layer = 6;
        RecalculateShip();
    }

    public void Remove(GameObject _obj) {
        blocks.Remove(_obj);
        shipMesh.RemoveBlock(_obj);
    }

    public void RecalculateShip() {
        horsePower = 0f;
        // estimatedSpeed = 0f;
        maximumFireRange = 0f;
        maxHealth = 0f;
        armor = 0f;
        centerOfMass = Vector3.zero;
        // displacement = 0f;
        density = 0f;
        volume = 0f;
        length = 0f;
        beam = 0f;
        height = 0f;
        // estimatedDraft = 0f;
        // sight = 0f;
        cost = 0f;
        int numBlocksWithDens = 0;
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = int.MaxValue;
        float maxY = int.MinValue;
        float minZ = int.MaxValue;
        float maxZ = int.MinValue;
        foreach (GameObject _obj in blocks) {
            if (_obj.GetComponent<Block>()) {
                Block block = _obj.GetComponent<Block>();
                horsePower += block.horsePower;
                maxHealth += block.health;
                armor += block.armor;
                centerOfMass += _obj.transform.position * block.mass;
                density += block.density;
                numBlocksWithDens ++;
                volume += block.volume;
                cost += block.cost;
            }

            if (_obj.transform.position.x < minX) {
                minX = _obj.transform.position.x;
            } else if (_obj.transform.position.x > maxX) {
                maxX = _obj.transform.position.x;
            }

            if (_obj.transform.position.y < minY) {
                minY = _obj.transform.position.y;
            } else if (_obj.transform.position.y > maxY) {
                maxY = _obj.transform.position.y;
            }

            if (_obj.transform.position.z < minZ) {
                minZ = _obj.transform.position.z;
            } else if (_obj.transform.position.z > maxZ) {
                maxZ = _obj.transform.position.z;
            }
        }
        centerOfMass /= this.mass;
        density /= numBlocksWithDens;
        length = Mathf.Abs(maxZ - minZ) + 1;
        beam = Mathf.Abs(maxX - minX) + 1;
        height = Mathf.Abs(maxY - minY) + 1;
    }
}

public struct VoxelData {
    public GameObject block;
    
}