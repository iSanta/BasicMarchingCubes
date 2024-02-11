using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modifyterrain : GenerativeMeshSpehere
{

    [SerializeField] private float ModifyRadius;
    private Vector3 rayPos;
    private int rayType;


    /// <summary>
    /// This recibe position and tipe of ray from the camera and update all value field on each corner of the marching cube algoritm
    /// </summary>
    /// <param name="_position">position vector where the ray impact with the mesh</param>
    /// <param name="_type">type of ray 1 = add  |  2 = remove</param>
    public void rayTerrainModify(Vector3 _position, int _type){
        rayPos = _position;
        rayType = _type;
        
        for (int i = 0; i < pointsPerFace; i++)
        {
            for (int j = 0; j < pointsPerFace; j++)
            {
                for (int k = 0; k < pointsPerFace; k++)
                {
                    for (int corner = 0; corner < 8; corner++)
                    {
                        poinstMatrix[i,j,k].Corners[corner].w = calcDistance(poinstMatrix[i,j,k].Corners[corner]);
                    }
                }
            }
        }

        createTriangles(poinstMatrix.Length);
    }

    /// <summary>
    /// this calcule distance between each corner and the ray hit point if the distance is less than the radio of an imaginare sphere then change the value of the corner depending of the type of ray
    /// </summary>
    /// <param name="_corner">indiviudal corner of the marching cube algoritm</param>
    /// <returns></returns>
    private float calcDistance(Vector4 _corner){
        Vector3 newVec = new Vector3(_corner.x, _corner.y, _corner.z) - rayPos;
        float magnitude = Vector3.Magnitude(newVec);
        switch (rayType)
        {
            case 1:
                if (magnitude <= ModifyRadius) return 1;
                else return _corner.w;

            case 2:
                if (magnitude <= ModifyRadius) return 0;
                else return _corner.w;
            default:
                return _corner.w;
        }
        
        
    }
}
