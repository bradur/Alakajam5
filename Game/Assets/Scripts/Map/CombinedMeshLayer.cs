// Date   : 18.02.2019 22:06
// Project: VillageCaveGame
// Author : bradur

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CombinedMeshLayer : MonoBehaviour {

    private List<MeshFilter> meshFilters;
    private CombineInstance[] combineInstances;

    private MeshFilter meshFilter;

    public void Initialize(string layerName) {
        meshFilters = new List<MeshFilter>();
        meshFilter = GetComponent<MeshFilter>();
        name = layerName;
    }

    void Start () {
    
    }

    void Update () {
    
    }
    public void Build() {
        combineInstances = new CombineInstance[meshFilters.Count];
        for (int index = 0; index < meshFilters.Count; index += 1 ) {
            combineInstances[index].mesh = meshFilters[index].sharedMesh;
            combineInstances[index].transform = meshFilters[index].transform.localToWorldMatrix;
            meshFilters[index].gameObject.SetActive(false);
        }
        meshFilter.mesh = new Mesh();
        meshFilter.mesh.CombineMeshes(combineInstances);
        name = string.Format("{0} ({1} meshes)", name, meshFilters.Count);
    }

    public void Add(MeshFilter filter) {
        meshFilters.Add(filter);
    }
}
