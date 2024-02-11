using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerativeMeshSpehere : MarchTables
{
   [SerializeField] private GameObject redererObj;
    public int pointsPerFace = 30;
    [SerializeField] private static float size = 0.5f;

    [SerializeField] private float surfaceLevel = 0.5f;
    [SerializeField] private bool ShowGrid = false;
    [SerializeField] private Material DotMaterial;
    [SerializeField] private Material triangleMaterial;
    [SerializeField] private float radius;
    [SerializeField] private Vector3 spherePos;

    private static float distance = 0.5f * size;

    private Vector3[] vertices;
    private int[] meshTriangles;
    private Mesh newMesh;
    private GameObject meshSector;
    

    /// <summary>
    /// this vector array is to store the 8 diferent corner positions in a cube based on a pivot in the center of the cube
    /// </summary>
    private static Vector3[] CP ={
        new Vector3(distance,-distance,distance),
        new Vector3(distance,-distance,-distance),
        new Vector3(-distance,-distance,-distance),
        new Vector3(-distance,-distance,distance),
        new Vector3(distance,distance,distance),
        new Vector3(distance,distance,-distance),
        new Vector3(-distance,distance,-distance),
        new Vector3(-distance,distance,distance)
    };

    /// <summary>
    /// this vector array is to store the 12 difrent edge positions in a cube based on a pivot in the center of the cube
    /// </summary>
    private static Vector3[] EP ={
        new Vector3(distance,-distance,0),
        new Vector3(0,-distance,-distance),
        new Vector3(-distance,-distance,0),
        new Vector3(0,-distance,distance),
        new Vector3(distance,distance,0),
        new Vector3(0,distance,-distance),
        new Vector3(-distance,distance,0),
        new Vector3(0,distance,distance),
        new Vector3(distance,0,distance),
        new Vector3(distance,0,-distance),
        new Vector3(-distance,0,-distance),
        new Vector3(-distance,0,distance)
    };


    /// <summary>
    /// this struct is for triangles, each triangle have 3 corners 
    /// </summary>
    public struct Triangle {
        public Vector3[] IndivTriangle;
        public Triangle(Vector3 _a, Vector3 _b,Vector3 _c) : this(){
            IndivTriangle = new Vector3[3];
            IndivTriangle[0] = _a;
            IndivTriangle[1] = _b;
            IndivTriangle[2] = _c;
        }
    };
    

    /// <summary>
    /// this strict have all necesary information for a single cube in a marching cube algoritm
    /// x,y,z variables describe the position of the cube 
    /// Corners array of type Vector4 describe the 8 difrent corners positions of a cube, the w component is for the value 0 to 1, this indicate if the corner is inside of the mesh or not
    /// a cube with 8 corners with w component at 1 means that cube is complete inside of the mesh, 
    /// a cube with 8 corners with w component at 0 means that cube is complete outside of the mesh,
    /// the rest of the cases means that cube is in the surface so it will create triangles of the mesh later
    /// 
    /// in the constructor store the position of the cube and using the arrays of CP (corner points) and EP (edge points) calcule the positions of corners and edges for this specific cube
    /// 
    /// for the w component of the cornesrs, for this case (a sphere) just calcule the distance between each corner to the center of an imaginary sphere, 
    /// if the distance is less than the radius of that sphere means that especific corner is inside of the sphere
    /// 
    /// if you need a diferent shape, for example a terrain, you will need to create a way to asign w corner component a value that you need
    /// </summary>
    public struct dotStruct
    {
        public float x;
        public float y;
        public float z;
        public Vector4[] Corners;

        public Vector3[] Edges;

        public dotStruct(float _x, float _y, float _z, Vector3 _originTarget, float _radiusTarget) : this() {
            x = _x;
            y = _y;
            z = _z;
            Corners = new Vector4[8];
            Edges = new Vector3[12];

            float calculeDistance(Vector3 _corner, Vector3 target, float _radius){
                Vector3 newVec = _corner - target;
                float magnitude = Vector3.Magnitude(newVec);
                if (magnitude <= _radius)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }

            Corners[0] = new Vector4(_x + CP[0].x ,_y + CP[0].y,_z + CP[0].z, calculeDistance(new Vector3(_x + CP[0].x ,_y + CP[0].y,_z + CP[0].z), _originTarget, _radiusTarget));
            Corners[1] = new Vector4(_x + CP[1].x ,_y + CP[1].y,_z + CP[1].z, calculeDistance(new Vector3(_x + CP[1].x ,_y + CP[1].y,_z + CP[1].z), _originTarget, _radiusTarget));
            Corners[2] = new Vector4(_x + CP[2].x ,_y + CP[2].y,_z + CP[2].z, calculeDistance(new Vector3(_x + CP[2].x ,_y + CP[2].y,_z + CP[2].z), _originTarget, _radiusTarget));
            Corners[3] = new Vector4(_x + CP[3].x ,_y + CP[3].y,_z + CP[3].z, calculeDistance(new Vector3(_x + CP[3].x ,_y + CP[3].y,_z + CP[3].z), _originTarget, _radiusTarget));
            Corners[4] = new Vector4(_x + CP[4].x ,_y + CP[4].y,_z + CP[4].z, calculeDistance(new Vector3(_x + CP[4].x ,_y + CP[4].y,_z + CP[4].z), _originTarget, _radiusTarget));
            Corners[5] = new Vector4(_x + CP[5].x ,_y + CP[5].y,_z + CP[5].z, calculeDistance(new Vector3(_x + CP[5].x ,_y + CP[5].y,_z + CP[5].z), _originTarget, _radiusTarget));
            Corners[6] = new Vector4(_x + CP[6].x ,_y + CP[6].y,_z + CP[6].z, calculeDistance(new Vector3(_x + CP[6].x ,_y + CP[6].y,_z + CP[6].z), _originTarget, _radiusTarget));
            Corners[7] = new Vector4(_x + CP[7].x ,_y + CP[7].y,_z + CP[7].z, calculeDistance(new Vector3(_x + CP[7].x ,_y + CP[7].y,_z + CP[7].z), _originTarget, _radiusTarget));

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


    public dotStruct[,,] poinstMatrix;
    private bool settingsUpdated;
    private GameObject[] allDots;



    /// <summary>
    /// asign sizes of arrays based on pointsPerFace
    /// </summary>
    private void Awake() {
        poinstMatrix = new dotStruct[pointsPerFace, pointsPerFace, pointsPerFace];
        allDots = new GameObject[pointsPerFace*pointsPerFace*pointsPerFace];
    }
    private void Start() {
        newMesh = new Mesh();
        meshSector = GameObject.CreatePrimitive(PrimitiveType.Plane);
        Destroy(meshSector.GetComponent<MeshCollider>());
        meshSector.transform.parent = redererObj.transform;

        Run();
    }

    


    /// <summary>
    /// main function
    /// </summary>
    private void Run(){
        createMarchingArea();
        if (ShowGrid)
        {
            makePointsVisibles(poinstMatrix.Length); 
        }
        createTriangles(poinstMatrix.Length);
    }


    /// <summary>
    /// this function set the initial values of each cube in the marching cube algoritm
    /// </summary>
    private void createMarchingArea(){
        for (int i = 0; i < pointsPerFace; i++)
        {
            for (int j = 0; j < pointsPerFace; j++)
            {
                for (int k = 0; k < pointsPerFace; k++)
                {
                    poinstMatrix[i,j,k] = new dotStruct(i*size,j*size,k*size, spherePos, radius);
                    
                }
            }
        }
    }





    /// <summary>
    /// each cube have corners, depending of the value of that corner (w component) the algoritm generate triangles, there are like 256 difrent posible configuration for each cube
    /// the number of triangles and positions depends on the corners values, the int cubeIndex variable store the index of the configuration for each cube, this index is for the triangulation matrix on MarchTables.cs
    /// the triangulation matrix is fill of int arrays who describe each posible configuration (256 in total)
    /// those values are just the edges of a cube, for example if a cube have the configuration 2 ({ 0, 1, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }) this means that this cube only have one triangule, and the corners of that triangle are the edges 0 , 1 and 9 of our cube
    /// each 3 values on the int arrays are difent triangles, all the -1 values are just empty space
    /// 
    /// at the end of this function vertices and meshTriangles variables get fill with all the necesary information correctly ordered to create a mesh later
    /// </summary>
    /// <param name="_iteration">the lenght of the matrix, i put it like a paramether because some times i just want to see some cubes and not all the matrix</param>

    public void createTriangles(int _iteration){
        int numTris = 0;
        int index = 0;
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


            index++;
        }
        int trianglesCounter = 0;
        
        vertices = new Vector3[numTris * 3];
        meshTriangles = new int[numTris * 3];
        for (int i = 0; i < numTris; i++) {
            for (int j = 0; j < 3; j++) {
                meshTriangles[i * 3 + j] = i * 3 + j;
                vertices[i * 3 + j] = AllTriangles[i].IndivTriangle[j];
                trianglesCounter++;
            }
            //Debug.Log(vertices[i*3]);
            
        }

        createMesh();
    }
    Vector3 interpolateVerts(Vector4 v1, Vector4 v2) {
        float t = (surfaceLevel - v1.w) / (v2.w - v1.w);
        return v1 + t * (v2-v1);
    }


    /// <summary>
    /// this function create the mesh, clear previus information, fill the mesh with the new vertices and triangles information and asign it to a game object
    /// </summary>
    public void createMesh(){
        newMesh.Clear();
        Destroy(meshSector.GetComponent<MeshCollider>());
        newMesh.vertices = vertices;
        newMesh.triangles = meshTriangles;
        newMesh.RecalculateNormals ();

        
        meshSector.GetComponent<MeshFilter>().mesh = newMesh;
        meshSector.GetComponent<MeshRenderer>().material = triangleMaterial;
        meshSector.AddComponent<MeshCollider>();
    }

 



    /// <summary>
    /// WARNING: this function execute if you mark ShowGrid in the inspecto, if you do that, make sure that the pointsPerFace variable not have a hight value, i usualy use this with a value like 10
    /// otherwise this will blow up your pc
    /// 
    /// this function is to visualy see the cubes and corners in the algoritm
    /// </summary>
    /// <param name="_iteration"></param>
    private void makePointsVisibles(int _iteration){
        Vector3 sizeDot = new Vector3(size,size,size);
        Vector3 sizeCorner = new Vector3(0.1f,0.1f,0.1f);

        int indexPoints = 0;
        foreach (var item in poinstMatrix)
        {
            if (indexPoints >= _iteration) break;
            
            allDots[indexPoints] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            allDots[indexPoints].transform.localScale = sizeDot;
            allDots[indexPoints].transform.position = new Vector3(item.x, item.y, item.z);
            allDots[indexPoints].GetComponent<MeshRenderer>().material = DotMaterial;

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


    /// <summary>
    /// this is just an example of how to create meshes on unity using vertices positions and triangles
    /// triangles is just an array of int who describe the order of each vertice
    /// in this example we have 2 triangles, the firts one with the corners 0,1,2, and the second with the corners 3,4,5 this values are the index of the verticeesArr
    /// </summary>
    private void createSimpleTriangle(){
        Mesh newMesh = new Mesh();

        Vector3[] verticesArr = { new Vector3(0,0,0), new Vector3(1,0,0), new Vector3(0,1,0.5f), new Vector3(0,0,0), new Vector3(0,1,0.5f), new Vector3(0,1,1f),};
        newMesh.vertices  = verticesArr;

        int[] trianglesArr = {0,1,2,3,4,5};
        newMesh.triangles = trianglesArr;

        redererObj.GetComponent<MeshFilter>().mesh = newMesh;
    }

    /// <summary>
    /// Gizmos to see the size of the cube matrix
    /// </summary>
    void OnDrawGizmos(){
        Gizmos.DrawWireCube(new Vector3((pointsPerFace*size/2)-(size/2),(pointsPerFace*size/2)-(size/2),(pointsPerFace*size/2)-(size/2)), Vector3.one * size * pointsPerFace );
    }
}
