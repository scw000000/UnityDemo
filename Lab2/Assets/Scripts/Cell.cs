using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Cell
   {
   public static readonly int[,] s_DirectionMapping = new int[5,2] { { -1, 0 }, { 0, 1 }, { 0, -1 }, { 1, 0 }, { 0, 0 } };
   public enum s_Directions { N, E, W, S, NOT_VALID };
   public static readonly Dictionary<s_Directions, s_Directions> s_InverseDirection = new Dictionary<s_Directions, s_Directions>()
      {
         { s_Directions.N, s_Directions.S },
         { s_Directions.E, s_Directions.W },
         { s_Directions.W, s_Directions.E },
         { s_Directions.S, s_Directions.N },
         { s_Directions.NOT_VALID, s_Directions.NOT_VALID },
      };

   public int m_RowIdx;
	public int m_ColIdx;

	public Dictionary< s_Directions, Cell > m_Neighbors;

	public Cell( int rowIdx, int colIdx )
      {
		m_RowIdx = rowIdx;
		m_ColIdx = colIdx;
      m_Neighbors = new Dictionary<s_Directions, Cell>();
      }

   public s_Directions GetDirection( Cell other )
      {
      int diffRowIdx = other.m_RowIdx - m_RowIdx;
      int diffColIdx = other.m_ColIdx - m_ColIdx;
      if( Math.Abs( diffRowIdx ) > 1 || Math.Abs( diffColIdx ) > 1 )
         {
         return s_Directions.NOT_VALID;
         }
      foreach( s_Directions dir in Enum.GetValues( typeof( s_Directions ) ) )
         {
         if( s_DirectionMapping[ (int) dir, 0 ] == diffRowIdx && s_DirectionMapping[ (int) dir, 1 ] == diffColIdx )
            {
            return dir;
            }
         }
      return s_Directions.NOT_VALID;
      }

   public bool IsAdjacent( Cell other )
	  {
      return GetDirection( other ) != s_Directions.NOT_VALID;
	  }

   public bool IsNeighbor( Cell other )
      {
      if( !IsAdjacent( other ) )
         {
         return false;
         }
      foreach( var cell in m_Neighbors )
         {
         if( cell.Value == other )
            {
            return true;
            }
         }
      return false;
      }

	public void Link( Cell other )
	   {
      // if its not next to it or already added to the neighbor, return
      if( !IsAdjacent( other ) || IsNeighbor( other ) )
         {
         if( !IsAdjacent( other ) )
            {
            Debug.LogError( "Invalid link!" );
            }
         return;
         }
      var thisToOtherDir = GetDirection( other );
      m_Neighbors.Add( thisToOtherDir, other );
      other.m_Neighbors.Add( s_InverseDirection[ thisToOtherDir ], this );
	   }

   public void UnLink( Cell other )
      {
      // if its not next to it or not added to the neighbor, return
      if( !IsAdjacent( other ) || !IsNeighbor( other ) )
         {
         return;
         }
      var thisToOtherDir = GetDirection( other );
      m_Neighbors.Remove( thisToOtherDir );
      other.m_Neighbors.Remove( s_InverseDirection[ thisToOtherDir ] );
      }
   }
