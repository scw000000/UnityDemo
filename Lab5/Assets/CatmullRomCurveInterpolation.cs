using UnityEngine;
using System.Collections;

public class UTable
   {
   public int m_PointIdx;
   public int m_SampleIdx;
   public float m_U;
   public float m_Distance;
   public UTable( int pointIdx, int sampleIdx, float u, float distance )
      {
      m_PointIdx = pointIdx;
      m_SampleIdx = sampleIdx;
      m_U = u;
      m_Distance = distance;
      }
   }

public class CatmullRomCurveInterpolation : MonoBehaviour
   {
	
	const int NumberOfPoints = 8;
	Vector3[] controlPoints;
	
	const int MinX = -5;
	const int MinY = -5;
	const int MinZ = 0;

	const int MaxX = 5;
	const int MaxY = 5;
	const int MaxZ = 5;
	
	double time = 0;
	const double DT = 0.01;
   public float m_Tao = 0.5f;
   int m_CurrIdx = 0;
   int m_DistanceSampleNum = 10;

   
   System.Collections.Generic.List<UTable> m_UTableList;
   /* Returns a point on a cubic Catmull-Rom/Blended Parabolas curve
	 * u is a scalar value from 0 to 1
	 * segment_number indicates which 4 points to use for interpolation
	 */
   Vector3 ComputePointOnCatmullRomCurve( float u, int curIndex )
      {
      Vector3 ret = new Vector3();
      Vector3 ref_MinusOne = controlPoints[ ( curIndex + controlPoints.Length - 1 ) % controlPoints.Length ];
      Vector3 ref_MinusTwo = controlPoints[ ( curIndex + controlPoints.Length - 2 ) % controlPoints.Length ];
      Vector3 ref_Zero = controlPoints[ curIndex ];
      Vector3 ref_PlusOne = controlPoints[ ( curIndex + 1 ) % controlPoints.Length ];

      Vector3 c3 = -m_Tao * ref_MinusTwo + ( 2 - m_Tao ) * ref_MinusOne + ( m_Tao - 2 ) * ref_Zero + m_Tao * ref_PlusOne;
      Vector3 c2 = 2 * m_Tao * ref_MinusTwo + ( m_Tao - 3 ) * ref_MinusOne + ( 3 - 2 * m_Tao ) * ref_Zero - m_Tao * ref_PlusOne;
      Vector3 c1 = -m_Tao * ref_MinusTwo + m_Tao * ref_Zero;
      Vector3 c0 = ref_MinusOne;

      ret = Mathf.Pow( (float) u, 3.0f ) * c3 + Mathf.Pow( (float) u, 2.0f ) * c2 + ( float )u * c1 + c0;
      return ret;
	   }
	
	void GenerateControlPointGeometry()
	   {
		for(int i = 0; i < NumberOfPoints; i++)
		   {
			GameObject tempcube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			tempcube.transform.localScale -= new Vector3(0.8f,0.8f,0.8f);
			tempcube.transform.position = controlPoints[i];
		   }	
	   }
	
	// Use this for initialization
	void Start ()
      {

		controlPoints = new Vector3[NumberOfPoints];
      
      // set points randomly...
      controlPoints[0] = new Vector3(0,0,0);
		for(int i = 1; i < NumberOfPoints; i++)
		   {
			controlPoints[i] = new Vector3(Random.Range(MinX,MaxX),Random.Range(MinY,MaxY),Random.Range(MinZ,MaxZ));
		   }
		
		GenerateControlPointGeometry();

      m_UTableList = new System.Collections.Generic.List<UTable>();

      UpdateUToDistanceFunction();
      }

   void UpdateUToDistanceFunction()
      {
      var distanceSum = 0.0f;
      
      for( int i = 0; i < NumberOfPoints; ++i )
         {
         float deltaU = 1.0f / ( m_DistanceSampleNum );
         float currU = 0.0f;
         var prevPoint = controlPoints[ i ];
    //     m_DistanceToUFunct[ i, 0 ] = new System.Collections.Generic.KeyValuePair<float, float>( 0.0f, 0.0f );
         m_UTableList.Add( new UTable( i, 0, 0f, 0f ) );
         for( int j = 0; j < m_DistanceSampleNum; ++j )
            {
            currU += deltaU;
            var currPoint = ComputePointOnCatmullRomCurve( currU, ( i + 1 ) % controlPoints.Length );
            var distance = Vector3.Distance( prevPoint, currPoint );
            distanceSum += distance;
         //   m_DistanceToUFunct[ i, j + 1 ] = new System.Collections.Generic.KeyValuePair<float, float>( currU, distanceSum );
            m_UTableList.Add( new UTable( i, j + 1, currU, distanceSum ) );
            prevPoint = currPoint;
            }
         if( i > 0 )
            {
            //m_DistanceToUFunct[ i, 0 ] = new System.Collections.Generic.KeyValuePair<float, float>( 0.0f, m_DistanceToUFunct[ i - 1, m_DistanceSampleNum ].Value );
            m_UTableList[ i * m_DistanceSampleNum + i ].m_Distance = m_UTableList[ i * m_DistanceSampleNum + i - 1 ].m_Distance;
            }
         }

      for( int i = 0; i < NumberOfPoints; ++i )
         {
         for( int j = 0; j < m_DistanceSampleNum + 1; ++j )
            {
            //  m_DistanceToUFunct[ i, j ] = new System.Collections.Generic.KeyValuePair<float, float>( m_DistanceToUFunct[ i, j ].Key, m_DistanceToUFunct[ i, j ].Value / distanceSum );
            m_UTableList[ i * m_DistanceSampleNum + i + j ].m_Distance /= distanceSum;
            }
         // m_DistanceToUFunct[ i, m_DistanceSampleNum ] = new System.Collections.Generic.KeyValuePair<float, float>( 1f, m_DistanceToUFunct[ i, m_DistanceSampleNum ].Value );
         m_UTableList[ i * m_DistanceSampleNum + i + m_DistanceSampleNum ].m_U = 1f;
         }
      foreach( var sample in m_UTableList )
         {
       //  Debug.Log( sample.m_PointIdx + " " + sample.m_SampleIdx + " " + sample.m_U + " " + sample.m_Distance );
         }
      //m_DistanceToUFunct[ 0, 0 ] = new System.Collections.Generic.KeyValuePair<float, float>( 0.0f, 0.0f );
  //    m_UTableList[ 0 ].
      }

   void MapDistanceToU( ref int pointIdx, ref float u, float s )
      {
      s %= 1.0f;
      int leftBound = 0;
      int rightBound = m_UTableList.Count - 1;
      while( leftBound < rightBound )
         {
         var midIdx = ( leftBound + rightBound ) / 2;
         if( m_UTableList[ midIdx ].m_Distance > s )
            {
            rightBound = midIdx - 1;
            }
         // left <= s, test right
         else if( m_UTableList[ midIdx + 1 ].m_Distance < s )
            {
            leftBound = midIdx + 1;
            }
         else
            {
            leftBound = midIdx;
            break;
            }
         }
      var leftPoint = m_UTableList[ leftBound ];
      var rightPoint = m_UTableList[ leftBound + 1 ];
      float factor = ( s - leftPoint.m_Distance ) / ( leftPoint.m_Distance - rightPoint.m_Distance );
      u = Mathf.Lerp( leftPoint.m_U, rightPoint.m_U, factor );
      Debug.Log( u );
      pointIdx = leftPoint.m_PointIdx;
      // Debug.Log( s + " : " + ret.m_PointIdx + " " + ret.m_SampleIdx + " " + ret.m_U + " " + ret.m_Distance );
      //   int tableIdx = 0;
      //  // pointIdx = 0; 
      //   while( pointIdx < NumberOfPoints - 1 && s > m_DistanceToUFunct[ pointIdx, m_DistanceSampleNum ].Value )
      //      {
      //      ++pointIdx;
      //      }

      //   while( tableIdx < m_DistanceSampleNum && s > m_DistanceToUFunct[ pointIdx, tableIdx + 1 ].Value )
      //      {
      //      ++tableIdx;
      //      }

      //   float factor = ( s - m_DistanceToUFunct[ pointIdx, tableIdx ].Value ) / ( m_DistanceToUFunct[ pointIdx, tableIdx ].Value - m_DistanceToUFunct[ pointIdx, tableIdx + 1 ].Value );
      //   u = Mathf.Lerp( m_DistanceToUFunct[ pointIdx, tableIdx ].Key, m_DistanceToUFunct[ pointIdx, tableIdx + 1 ].Key, factor );
      ////   Debug.Log( s + " " + pointIdx + " " + tableIdx + " " + u );
      }
   // Update is called once per frame
   void Update ()
      { 
		time += ( 0.001f  );
      time %= 1f;
      //    if( time >= 1.0f )
      //       {
      //       time %= 1.0f;
      //       m_CurrIdx = ( m_CurrIdx + 1 ) % controlPoints.Length;
      //       m_ShouldUpdateSpeedFunct = true;
      //       }
      float tempU = 0f;
      MapDistanceToU( ref m_CurrIdx, ref tempU, (float) time );

      Vector3 temp = ComputePointOnCatmullRomCurve( tempU, ( m_CurrIdx + 1 ) % controlPoints.Length );
		transform.position = temp;
	   }
   }
