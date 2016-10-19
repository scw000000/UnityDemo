using UnityEngine;
using System.Collections;

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
   bool m_ShouldUpdateSpeedFunct = true;
   int m_DistanceSampleNum = 20;
   System.Collections.Generic.KeyValuePair<float, float>[] m_DistanceToUFunct;
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
      m_DistanceToUFunct = new System.Collections.Generic.KeyValuePair<float, float>[ m_DistanceSampleNum ];
      m_DistanceToUFunct[ 0 ] = new System.Collections.Generic.KeyValuePair<float, float>( 0.0f, 0.0f );
      m_DistanceToUFunct[ m_DistanceSampleNum - 1 ] = new System.Collections.Generic.KeyValuePair<float, float>( 1.0f, 1.0f );
      // set points randomly...
      controlPoints[0] = new Vector3(0,0,0);
		for(int i = 1; i < NumberOfPoints; i++)
		{
			controlPoints[i] = new Vector3(Random.Range(MinX,MaxX),Random.Range(MinY,MaxY),Random.Range(MinZ,MaxZ));
		}
		
		GenerateControlPointGeometry();
	   }

   void UpdateUToDistanceFunction()
      {
      float deltaU = 1.0f / ( m_DistanceSampleNum - 1 );
      float currU = 0.0f;
      m_DistanceToUFunct[ 0 ] = new System.Collections.Generic.KeyValuePair<float, float>( 0.0f, 0.0f );
      var prevPoint = controlPoints[ m_CurrIdx ];
      var distanceSum = 0.0f;
      for( int i = 1; i < m_DistanceSampleNum; ++i )
         {
         currU += deltaU;
         var currPoint = ComputePointOnCatmullRomCurve( currU, ( m_CurrIdx + 1 ) % controlPoints.Length );
         var distance = Vector3.Distance( prevPoint, currPoint );
         distanceSum += distance;
         m_DistanceToUFunct[ i ] = new System.Collections.Generic.KeyValuePair<float, float>( currU, distanceSum );
         prevPoint = currPoint;
         }

      for( int i = 1; i < m_DistanceSampleNum; ++i )
         {
         m_DistanceToUFunct[ i ] = new System.Collections.Generic.KeyValuePair<float, float>( m_DistanceToUFunct[ i ].Key, m_DistanceToUFunct[ i ].Value / distanceSum );
         }
      m_DistanceToUFunct[ m_DistanceSampleNum - 1 ] = new System.Collections.Generic.KeyValuePair<float, float>( 1.0f, 1.0f );
      }

   float MapDistanceToU( float s )
      {
      int tableIdx = 0;
      while( tableIdx < m_DistanceSampleNum - 1 && s > m_DistanceToUFunct[ tableIdx + 1 ].Value )
         {
         ++tableIdx;
         }
      float factor = ( s - m_DistanceToUFunct[ tableIdx ].Value ) / ( m_DistanceToUFunct[ tableIdx + 1 ].Value - m_DistanceToUFunct[ tableIdx + 1 ].Value );
      return Mathf.Lerp( m_DistanceToUFunct[ tableIdx ].Key, m_DistanceToUFunct[ tableIdx + 1 ].Key, factor );
      }
   // Update is called once per frame
   void Update ()
      {
		time += DT;
      if( m_ShouldUpdateSpeedFunct )
         {
         UpdateUToDistanceFunction();
         m_ShouldUpdateSpeedFunct = false;
         }

      if( time >= 1.0f )
         {
         time %= 1.0f;
         m_CurrIdx = ( m_CurrIdx + 1 ) % controlPoints.Length;
         m_ShouldUpdateSpeedFunct = true;
         }
		Vector3 temp = ComputePointOnCatmullRomCurve( MapDistanceToU( ( float )time ), ( m_CurrIdx + 1 ) % controlPoints.Length );
		transform.position = temp;
	   }
   }
