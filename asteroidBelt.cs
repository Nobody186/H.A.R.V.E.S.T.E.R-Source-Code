using UnityEngine;
using System.Collections.Generic;

public class AsteroidBelt : MonoBehaviour
{
    [SerializeField] Material asteroidMat;
    [SerializeField] Mesh mesh;
    [SerializeField] float xScale = 1f, yScale = 1f, zScale = 1f;
    [SerializeField] float spawnRadius = 500f;
    [SerializeField] int spawnAmount = 10000;
    [SerializeField] float cellSize = 100f;

    const int MaxBatchSize = 1023;

    class Cell
    {
        public List<Matrix4x4> matrices = new();
        public Vector3 center;
        public Matrix4x4[] matrixArray = null;
        public Bounds bounds;
    }

    List<Cell> cells = new();

    void Start()
    {
        Dictionary<Vector3Int, Cell> cellMap = new();

        for (int i = 0; i < spawnAmount; i++)
        {
            // Position + Rotation + Scale
            Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
            Quaternion rot = Random.rotation;
            Vector3 scale = new Vector3(xScale, yScale, zScale);
            Matrix4x4 matrix = Matrix4x4.TRS(pos, rot, scale);

            // Determine which cell it's in
            Vector3Int cellID = new Vector3Int(
                Mathf.FloorToInt(pos.x / cellSize),
                Mathf.FloorToInt(pos.y / cellSize),
                Mathf.FloorToInt(pos.z / cellSize)
            );

            if (!cellMap.TryGetValue(cellID, out Cell cell))
            {
                cell = new Cell();
                cell.center = new Vector3(cellID.x * cellSize + cellSize / 2f, cellID.y * cellSize + cellSize / 2f, cellID.z * cellSize + cellSize / 2f);
                cell.bounds = new Bounds(cell.center, Vector3.one * cellSize * 1.5f);
                cellMap[cellID] = cell;
                cells.Add(cell);
            }

            cell.matrices.Add(matrix);
        }

        // Pre-bake matrix arrays for performance
        foreach (var cell in cells)
        {
            cell.matrixArray = cell.matrices.ToArray();
        }
    }

    void Update()
    {
        Plane[] camFrustum = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        foreach (var cell in cells)
        {
            if (!GeometryUtility.TestPlanesAABB(camFrustum, cell.bounds))
                continue;

            var matrixArray = cell.matrixArray;
            int total = matrixArray.Length;

            for (int i = 0; i < total; i += MaxBatchSize)
            {
                int count = Mathf.Min(MaxBatchSize, total - i);
                Graphics.DrawMeshInstanced(mesh, 0, asteroidMat, matrixArray, count, null, UnityEngine.Rendering.ShadowCastingMode.On, true, 0, null);
            }
        }
    }
}