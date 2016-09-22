using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeBuilder : MonoBehaviour {
   public int m_RowCount = 3;
   public int m_ColCount = 3;
   public GameObject m_WallPrefab;
   public GameObject m_Avatar;
   public float m_MazeScale = 1.0f;
   public enum s_GenerateType { NEWEST, OLDEST, RANDOM };
   public s_GenerateType m_GenerateType = s_GenerateType.RANDOM;
   private Grid m_Grid;
   private MazeGerneration m_Generation;
   private GameObject InstantiateAndAttach( Object prefab, Vector3 pos, Quaternion quat )
      {
      GameObject obj = Instantiate( prefab, pos, quat ) as GameObject;
      obj.transform.parent = gameObject.transform;
      return obj;
      }

   // Use this for initialization
   void Start()  
      {
      m_Grid = new Grid( m_RowCount, m_ColCount );
      MazeGerneration mazeGeneration = new MazeGerneration( m_GenerateType );
      mazeGeneration.GenerateMaze( m_Grid );

      int outerWallRowSize = m_RowCount * 2 + 1;
      int outerWallColSize = m_ColCount * 2 + 1;

      float xShift = -1.0f * outerWallRowSize / 2;
      float zShift = -1.0f * outerWallColSize / 2;

      GameObject tempObj = Instantiate( m_WallPrefab, Vector3.zero, Quaternion.identity ) as GameObject;
      tempObj.transform.localScale *= m_MazeScale;
      GameObject startWallObj = null;
      GameObject exitWallObj = null;

      float wallHeightShift = m_MazeScale / 2.0f - 0.5f;
      for( int i = 0; i < outerWallRowSize; ++i )
         {
         if( i == 1 )
            {
            startWallObj = InstantiateAndAttach( tempObj, new Vector3( ( i - 1 + xShift ) * m_MazeScale, wallHeightShift, ( -1 + zShift ) * m_MazeScale ), Quaternion.identity );
            }
         else
            {
            InstantiateAndAttach( tempObj, new Vector3( ( i - 1 + xShift ) * m_MazeScale, wallHeightShift, ( -1 + zShift ) * m_MazeScale ), Quaternion.identity );
            }
         if( i == outerWallRowSize - 2 )
            {
            exitWallObj = InstantiateAndAttach( tempObj, new Vector3( ( i - 1 + xShift ) * m_MazeScale, wallHeightShift, ( m_ColCount * 2 - 1 + zShift ) * m_MazeScale ), Quaternion.identity );
            }
         else
            {
            InstantiateAndAttach( tempObj, new Vector3( ( i - 1 + xShift ) * m_MazeScale, wallHeightShift, ( m_ColCount * 2 - 1 + zShift ) * m_MazeScale ), Quaternion.identity );
            }
         }
      m_Avatar.transform.position = new Vector3( xShift * m_MazeScale, wallHeightShift, ( -1 + zShift ) * m_MazeScale );
      Destroy( startWallObj );
      Destroy( exitWallObj );
      for( int i = 0; i < outerWallColSize - 2; ++i )
         {
         InstantiateAndAttach( tempObj, new Vector3( ( -1 + xShift ) * m_MazeScale, wallHeightShift, ( i + zShift ) * m_MazeScale ), Quaternion.identity );
         InstantiateAndAttach( tempObj, new Vector3( ( m_RowCount * 2 - 1 + xShift ) * m_MazeScale, wallHeightShift, ( i + zShift ) * m_MazeScale ), Quaternion.identity );
         }

      for( int i = 0; i < m_RowCount; ++i )
         {
         for( int j = 0; j < m_ColCount; ++j )
            {
            // no link in W or N, build wall
            if( j != 0 && !m_Grid.m_Cells[ i, j ].m_Neighbors.ContainsKey( Cell.s_Directions.W ) )
               {
               InstantiateAndAttach( tempObj, new Vector3( ( i * 2 + xShift ) * m_MazeScale, wallHeightShift, ( j * 2 - 1 + zShift ) * m_MazeScale ), Quaternion.identity );
               }
            if( i != 0 && !m_Grid.m_Cells[ i, j ].m_Neighbors.ContainsKey( Cell.s_Directions.N ) )
               {
               InstantiateAndAttach( tempObj, new Vector3( ( i * 2 - 1 + xShift ) * m_MazeScale, wallHeightShift, ( j * 2 + zShift ) * m_MazeScale ), Quaternion.identity );
               }

            if( i < m_RowCount - 1 && j < m_ColCount - 1 )
               {
               InstantiateAndAttach( tempObj, new Vector3( ( i * 2 + 1 + xShift ) * m_MazeScale, wallHeightShift, ( j * 2 + 1 + zShift ) * m_MazeScale ), Quaternion.identity );
               }
            }
         }
      Destroy( tempObj );
      }
   // Update is called once per frame
   void Update () {
	
	}
}

public class MazeGerneration
   {
   private static System.Random s_Random;
   private MazeBuilder.s_GenerateType m_GenerateType;

   public MazeGerneration( MazeBuilder.s_GenerateType genType )
      {
      m_GenerateType = genType;
      }
   
   public void GenerateMaze( Grid grid )
      {
      List< Cell> mySet = new List<Cell>();
      Dictionary<Cell, Cell> visitedSet = new Dictionary<Cell, Cell>();
      s_Random = new System.Random( System.Guid.NewGuid().GetHashCode() );
      int row = s_Random.Next( 0, grid.m_RowCount );
      int col = s_Random.Next( 0, grid.m_ColCount );
      mySet.Add( grid.m_Cells[ row, col ] );
      Cell.s_Directions[] randomPickDir = new Cell.s_Directions[ 4 ] { Cell.s_Directions.N, Cell.s_Directions.E, Cell.s_Directions.W, Cell.s_Directions.S };

      while( mySet.Count > 0 )
         {
         Cell currCell = GetNextCell( mySet );
         Shuffle( randomPickDir, 0 );
         bool findNextNode = false;
         foreach( var dir in randomPickDir )
            {
            var randomNeighbor = grid.FindAdjacent( currCell, dir );
            if( randomNeighbor != null && !visitedSet.ContainsKey( randomNeighbor ) )
               {
               currCell.Link( randomNeighbor );
               mySet.Add( randomNeighbor );
               visitedSet.Add( randomNeighbor, randomNeighbor );
               findNextNode = true;
               }
            }
         if( !findNextNode )
            {
            mySet.Remove( currCell );
            }
         }
      }

   private Cell GetNextCell( List<Cell> set )
      {
      int idx = 0;
      switch( m_GenerateType )
         {
         case MazeBuilder.s_GenerateType.OLDEST:
            idx = 0;
            break;
         case MazeBuilder.s_GenerateType.NEWEST:
            idx = set.Count - 1;
            break;
         case MazeBuilder.s_GenerateType.RANDOM:
            idx = s_Random.Next( 0, set.Count );
            break;
         };
      
      Cell ret = set[ idx ];
      set.Remove( ret );
      return ret;
      }

   private static void Shuffle( Cell.s_Directions[] array, int startIdx )
      {
      if( startIdx >= 3 )
         {
         return;
         }
      int swapIdx = s_Random.Next( startIdx, 4 );
      var temp = array[ swapIdx ];
      array[ swapIdx ] = array[ startIdx ];
      array[ startIdx ] = temp;
      Shuffle( array, ++startIdx );
      }
   }
