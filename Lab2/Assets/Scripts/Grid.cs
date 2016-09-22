using UnityEngine;
using System.Collections;

public class Grid 
   {
   public int m_RowCount;
   public int m_ColCount;
   public Cell[,] m_Cells;

	public Grid( int row, int col )
	   {
      m_RowCount = row;
      m_ColCount = col;

      m_Cells = new Cell[ m_RowCount, m_ColCount ];

      for( int i = 0; i < m_RowCount; ++i )
         {
         for( int j = 0; j < m_ColCount; ++j )
            {
            m_Cells[ i, j ] = new Cell( i, j );
            }
         }
	   }

   public Cell FindAdjacent( Cell currNode, Cell.s_Directions dir )
      {
      int targetRow = currNode.m_RowIdx + Cell.s_DirectionMapping[ (int) dir, 0 ];
      int targetCol = currNode.m_ColIdx + Cell.s_DirectionMapping[ (int) dir, 1 ];
      if( targetRow < 0 || targetRow >= m_RowCount || targetCol < 0 || targetCol >= m_ColCount )
         {
         return null;
         }  
      return m_Cells[ targetRow, targetCol ];
      }
   }
