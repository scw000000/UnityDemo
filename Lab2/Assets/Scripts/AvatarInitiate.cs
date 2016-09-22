using UnityEngine;
using System.Collections;

public class AvatarInitiate : MonoBehaviour
   {
   public Material m_Material;
   public float m_CubeSize;
   // Use this for initialization
   void Start ()
      {
      float halfCubeSize = m_CubeSize / 2.0f;
      Vector3[] m_Vertices = new Vector3[] {
         new Vector3( -halfCubeSize, -halfCubeSize, -halfCubeSize ),
        new Vector3( -halfCubeSize, halfCubeSize, -halfCubeSize ),
        new Vector3( halfCubeSize, halfCubeSize, -halfCubeSize ),
        new Vector3( halfCubeSize, -halfCubeSize, -halfCubeSize ),
        new Vector3( halfCubeSize, -halfCubeSize, halfCubeSize ),
        new Vector3( halfCubeSize, halfCubeSize, halfCubeSize ),
        new Vector3( -halfCubeSize, halfCubeSize, halfCubeSize ),
        new Vector3( -halfCubeSize, -halfCubeSize, halfCubeSize ) };

      Vector2[] m_UVs = new Vector2[] {
         new Vector2( 0.0f, 1.0f ),
         new Vector2( 0.0f, 0.0f ),
         new Vector2( 1.0f, 0.0f ),
         new Vector2( 1.0f, 1.0f ),
         new Vector2( 0.0f, 1.0f ),
         new Vector2( 0.0f, 0.0f ),
         new Vector2( 1.0f, 0.0f ),
         new Vector2( 1.0f, 1.0f ) };

      int[] m_Triangles = new int[] {
         0, 1, 3,
        1, 2, 3,
        3, 2, 5,
        3, 5, 4,
        5, 2, 1,
        5, 1, 6,
        3, 4, 7,
        3, 7, 0,
        0, 7, 6,
        0, 6, 1,
        4, 5, 6,
        4, 6, 7  };
      Mesh mesh = new Mesh();
      gameObject.AddComponent<MeshFilter>().mesh = mesh;
      mesh.vertices = m_Vertices;
      mesh.triangles = m_Triangles;
      mesh.uv = m_UVs;
      mesh.RecalculateNormals();
      MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
      meshRenderer.material = m_Material;
      var boxCollider = gameObject.AddComponent<BoxCollider>();
      boxCollider.size = new Vector3( m_CubeSize, m_CubeSize, m_CubeSize );
      //Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>(); 
      //rigidbody.mass = 5;
      }
	
	// Update is called once per frame
	void Update ()
      {
	
	   }
   }
