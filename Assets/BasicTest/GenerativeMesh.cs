using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//[ExecuteInEditMode]
public class GenerativeMesh : MarchTables
{
    [SerializeField] private GameObject redererObj;
    [SerializeField] private int pointsPerFace = 30;
    [SerializeField] private float surfaceLevel = 1f;
    [SerializeField] private bool autoUpdateInEditor = true;
    [SerializeField] private bool autoUpdateInGame = true;
    [SerializeField] private Material DotMaterial;
    [SerializeField] private Material triangleMaterial;
    private static Vector3[] CP ={
        new Vector3(0.5f,-0.5f,0.5f),
        new Vector3(0.5f,-0.5f,-0.5f),
        new Vector3(-0.5f,-0.5f,-0.5f),
        new Vector3(-0.5f,-0.5f,0.5f),
        new Vector3(0.5f,0.5f,0.5f),
        new Vector3(0.5f,0.5f,-0.5f),
        new Vector3(-0.5f,0.5f,-0.5f),
        new Vector3(-0.5f,0.5f,0.5f)
    };
    private static Vector3[] EP ={
        new Vector3(0.5f,-0.5f,0),
        new Vector3(0,-0.5f,-0.5f),
        new Vector3(-0.5f,-0.5f,0),
        new Vector3(0,-0.5f,0.5f),
        new Vector3(0.5f,0.5f,0),
        new Vector3(0,0.5f,-0.5f),
        new Vector3(-0.5f,0.5f,0),
        new Vector3(0,0.5f,0.5f),
        new Vector3(0.5f,0,0.5f),
        new Vector3(0.5f,0,-0.5f),
        new Vector3(-0.5f,0,-0.5f),
        new Vector3(-0.5f,0,0.5f)
    };
    public struct Triangle {
        public Vector3[] IndivTriangle;
        public Triangle(Vector3 _a, Vector3 _b,Vector3 _c) : this(){
            IndivTriangle = new Vector3[3];
            IndivTriangle[0] = _a;
            IndivTriangle[1] = _b;
            IndivTriangle[2] = _c;
        }
    };
    
    public struct dotStruct
    {
        public float x;
        public float y;
        public float z;
        public Vector4[] Corners;

        public Vector3[] Edges;

        public dotStruct(float _x, float _y, float _z) : this() {
            x = _x;
            y = _y;
            z = _z;
            Corners = new Vector4[8];
            Edges = new Vector3[12];

            Corners[0] = new Vector4(_x + CP[0].x ,_y + CP[0].y,_z + CP[0].z, Random.Range(0f, 1f));
            Corners[1] = new Vector4(_x + CP[1].x ,_y + CP[1].y,_z + CP[1].z, Random.Range(0f, 1f));
            Corners[2] = new Vector4(_x + CP[2].x ,_y + CP[2].y,_z + CP[2].z, Random.Range(0f, 1f));
            Corners[3] = new Vector4(_x + CP[3].x ,_y + CP[3].y,_z + CP[3].z, Random.Range(0f, 1f));
            Corners[4] = new Vector4(_x + CP[4].x ,_y + CP[4].y,_z + CP[4].z, Random.Range(0f, 1f));
            Corners[5] = new Vector4(_x + CP[5].x ,_y + CP[5].y,_z + CP[5].z, Random.Range(0f, 1f));
            Corners[6] = new Vector4(_x + CP[6].x ,_y + CP[6].y,_z + CP[6].z, Random.Range(0f, 1f));
            Corners[7] = new Vector4(_x + CP[7].x ,_y + CP[7].y,_z + CP[7].z, Random.Range(0f, 1f));

            Edges[0] = new Vector3(_x + EP[0].x ,_y + EP[0].y,_z + EP[0].z);
            Edges[1] = new Vector3(_x + EP[1].x ,_y + EP[1].y,_z + EP[1].z);
            Edges[2] = new Vector3(_x + EP[2].x ,_y + EP[2].y,_z + EP[2].z);
            Edges[3] = new Vector3(_x + EP[3].x ,_y + EP[3].y,_z + EP[3].z);
            Edges[4] = new Vector3(_x + EP[4].x ,_y + EP[4].y,_z + EP[4].z);
            Edges[5] = new Vector3(_x + EP[5].x ,_y + EP[5].y,_z + EP[5].z);
            Edges[6] = new Vector3(_x + EP[6].x ,_y + EP[6].y,_z + EP[6].z);
            Edges[7] = new Vector3(_x + EP[7].x ,_y + EP[7].y,_z + EP[7].z);
            Edges[8] = new Vector3(_x + EP[8].x ,_y + EP[8].y,_z + EP[8].z);
            Edges[9] = new Vector3(_x + EP[9].x ,_y + EP[9].y,_z + EP[9].z);
            Edges[10] = new Vector3(_x + EP[10].x ,_y + EP[10].y,_z + EP[10].z);
            Edges[11] = new Vector3(_x + EP[11].x ,_y + EP[11].y,_z + EP[11].z);


        }
        
    }


    private dotStruct[,,] poinstMatrix;
    private bool settingsUpdated;
    private GameObject[] allDots;



    private void Awake() {
        poinstMatrix = new dotStruct[pointsPerFace, pointsPerFace, pointsPerFace];
        allDots = new GameObject[pointsPerFace*pointsPerFace*pointsPerFace];
    }
    private void Start() {
        Run();
    }

    /*void OnValidate() {
        settingsUpdated = true;
    }*/

    private void Run(){
        createMarchingArea();
        makePointsVisibles(poinstMatrix.Length);
        createTriangle(poinstMatrix.Length);
    }
    private void clearRun(){
        foreach (var item in allDots)
        {
            Destroy(item);
        }
    }

    private void createTriangle(int _iteration){
        //createSimpleTriangle();        
        int numTris = 0;
        int index = 0;
        //makePointsVisibles(_iteration);
        List<Triangle> AllTriangles = new List<Triangle>();

        

        foreach (var item in poinstMatrix)
        {
            if (index >= _iteration) break;
            
            int cubeIndex = 0;
            for (int i = 0; i < 8; i++)
            {
                if (item.Corners[i].w < surfaceLevel)
                {
                    cubeIndex |= 1<<i;
                    //Debug.Log($"({item.Corners[i].x}, {item.Corners[i].y}, {item.Corners[i].z})");
                }
            }
            //Debug.Log($"cubeIndex: {cubeIndex}" );
            int n = 16;
            int numVetices = 0;
            int[] chosen = new int[n] ;
            
            
            
            for (int i = 0; i < n; i++)
            {
                chosen[i] = triangulation[cubeIndex, i];
                if (triangulation[cubeIndex, i] != -1)
                {
                    numVetices++;
                }
            }   
            ///Nuevo intento
            
            for (int i = 0; chosen[i] != -1; i+=3)
            {
                //Debug.Log(chosen[i]);
                numTris++;
                int a0 = cornerIndexAFromEdge[chosen[i]];
                int b0 = cornerIndexBFromEdge[chosen[i]];

                int a1 = cornerIndexAFromEdge[chosen[i+1]];
                int b1 = cornerIndexBFromEdge[chosen[i+1]];

                int a2 = cornerIndexAFromEdge[chosen[i+2]];
                int b2 = cornerIndexBFromEdge[chosen[i+2]];

                Triangle tri;
                tri = new Triangle(interpolateVerts(item.Corners[a0], item.Corners[b0]), interpolateVerts(item.Corners[a1], item.Corners[b1]), interpolateVerts(item.Corners[a2], item.Corners[b2]));
                AllTriangles.Add(tri);
            }


            /*Vector3[] vertices = new Vector3[12];
            vertices = item.Edges;
            int[] triangles = new int[numVetices];
            string arrtri = "";
            for (int i = 0; i < numVetices; i++)
            {
                triangles[i] = chosen[i];
                arrtri += " " + triangles[i];

            }

            Debug.Log(arrtri);
            

            Mesh newMesh = new Mesh();
            newMesh.vertices = vertices;
            newMesh.triangles = triangles;
            
            GameObject meshSector = GameObject.CreatePrimitive(PrimitiveType.Plane);
            Destroy(meshSector.GetComponent<MeshCollider>());
            meshSector.transform.parent = redererObj.transform;
            meshSector.GetComponent<MeshFilter>().mesh = newMesh;
            meshSector.GetComponent<MeshRenderer>().material = triangleMaterial;
            
            foreach (var singleTriangle in AllTriangles)
            {
                Debug.Log($"TriangleA: {singleTriangle.vertexA} TriangleB: {singleTriangle.vertexB} TriangleC: {singleTriangle.vertexC}");
            }*/

            index++;
        }
        Mesh newMesh = new Mesh();
        var vertices = new Vector3[numTris * 3];
        var meshTriangles = new int[numTris * 3];
        for (int i = 0; i < numTris; i++) {
            for (int j = 0; j < 3; j++) {
                meshTriangles[i * 3 + j] = i * 3 + j;
                vertices[i * 3 + j] = AllTriangles[i].IndivTriangle[j];

            }
            
        }
        newMesh.vertices = vertices;
        newMesh.triangles = meshTriangles;
        newMesh.RecalculateNormals ();

        GameObject meshSector = GameObject.CreatePrimitive(PrimitiveType.Plane);
        Destroy(meshSector.GetComponent<MeshCollider>());
        meshSector.transform.parent = redererObj.transform;
        meshSector.GetComponent<MeshFilter>().mesh = newMesh;
        meshSector.GetComponent<MeshRenderer>().material = triangleMaterial;
    }

    Vector3 interpolateVerts(Vector4 v1, Vector4 v2) {
        float t = (surfaceLevel - v1.w) / (v2.w - v1.w);
        return v1 + t * (v2-v1);
    }

    /*private void Update() {
        if (settingsUpdated) {
            if ((Application.isPlaying && autoUpdateInGame) || (!Application.isPlaying && autoUpdateInEditor)) {
                clearRun();
                Run ();
            }
            settingsUpdated = false;
        }
    }*/


    private void createMarchingArea(){
        for (int i = 0; i < pointsPerFace; i++)
        {
            for (int j = 0; j < pointsPerFace; j++)
            {
                for (int k = 0; k < pointsPerFace; k++)
                {
                    poinstMatrix[i,j,k] = new dotStruct(i,j,k);
                    
                }
            }
        }
    }

    private void makePointsVisibles(int _iteration){
        Vector3 sizeDot = new Vector3(1f,1f,1f);
        Vector3 sizeCorner = new Vector3(0.1f,0.1f,0.1f);

        int indexPoints = 0;
        foreach (var item in poinstMatrix)
        {
            if (indexPoints >= _iteration) break;
            
            allDots[indexPoints] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            allDots[indexPoints].transform.localScale = sizeDot;
            allDots[indexPoints].transform.position = new Vector3(item.x, item.y, item.z);
            allDots[indexPoints].GetComponent<MeshRenderer>().material = DotMaterial;
            //allDots[indexPoints].GetComponent<MeshRenderer>().enabled = false;
            //Material newMaterial = new Material(DotMaterial);
            //newMaterial.color = new Color(item.w,item.w,item.w,0.5f);
            //allDots[indexPoints].GetComponent<MeshRenderer>().material = newMaterial;
            float opacityCorner = 0.8f;
            GameObject[] cornersOfCube = new GameObject[8];
            for (int i = 0; i < 8; i++)
            {
                if (item.Corners[i].w < surfaceLevel)
                {
                    cornersOfCube[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cornersOfCube[i].transform.parent = allDots[indexPoints].transform;
                    cornersOfCube[i].transform.localScale = sizeCorner;
                    cornersOfCube[i].transform.position = item.Corners[i];
                    Material newMaterial = new Material(DotMaterial);
                    newMaterial.color = new Color(item.Corners[i].w,item.Corners[i].w,item.Corners[i].w, opacityCorner);
                    cornersOfCube[i].GetComponent<MeshRenderer>().material = newMaterial;  
                }
                   
            }


            indexPoints++;
        }
    }

    private void createSimpleTriangle(){
        Mesh newMesh = new Mesh();

        Vector3[] verticesArr = { new Vector3(0,0,0), new Vector3(1,0,0), new Vector3(0,1,0.5f), new Vector3(0,0,0), new Vector3(0,1,0.5f), new Vector3(0,1,1f),};
        newMesh.vertices  = verticesArr;

        int[] trianglesArr = {0,1,2,3,4,5};
        newMesh.triangles = trianglesArr;

        redererObj.GetComponent<MeshFilter>().mesh = newMesh;
    }
}
