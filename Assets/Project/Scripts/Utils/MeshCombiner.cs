// https://gist.github.com/yigiteren/551f693e62b5f39baaba7536fa2c4680
// https://github.com/pharan/Unity-MeshSaver/blob/master/MeshSaver/Editor/MeshSaverEditor.cs

#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public class MeshCombiner : MonoBehaviour {
	[SerializeField]
	private string meshName;
    [SerializeField] private List<MeshFilter> sourceMeshFilters;
    [SerializeField] private MeshFilter targetMeshFilter;

    [ContextMenu("Combine Meshes")]
    private void CombineMeshes()
    {
        var combine = new CombineInstance[sourceMeshFilters.Count];

        for (var i = 0; i < sourceMeshFilters.Count; i++)
        {
            combine[i].mesh = sourceMeshFilters[i].sharedMesh;
            combine[i].transform = sourceMeshFilters[i].transform.localToWorldMatrix;
        }

        var mesh = new Mesh();
        mesh.CombineMeshes(combine);
        mesh.name = meshName;
        targetMeshFilter.mesh = mesh;

        string mapName = Path.GetFileNameWithoutExtension(mesh.name);
        string localPath = Path.Combine("Assets", "Project", "Models", "Meshes", mapName + ".mesh");

        AssetDatabase.CreateAsset( mesh, localPath );
        AssetDatabase.SaveAssets();

    }
}

#endif // UNITY_EDITOR