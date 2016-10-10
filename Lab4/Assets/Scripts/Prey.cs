using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Prey : MonoBehaviour {
   private List< GameObject > m_ChasingPredators = new List<GameObject>();
   private bool m_IsBeingTraced = false;
   private float m_Velocity = 0.5f;
   private Vector3 m_RoamingGoal = new Vector3();
   private float m_EscapeViewLength = 3.0f;
   // Use this for initialization
   void Start ()
      {
      SetRoamingGoal();
      }
	
	// Update is called once per frame
	void Update ()
      {
      if( m_IsBeingTraced )
         {
         Vector3 bestEscapeDir = new Vector3( 0, 0, 0 );
         Vector3 initialEscapeDir = new Vector3( 0, 0, 0 );
         foreach( var predator in m_ChasingPredators )
            {
            initialEscapeDir += gameObject.transform.position - predator.transform.position;
            }
         initialEscapeDir.Normalize();
         RaycastHit hitInfo;
         for( int i = 0; i < 180; ++i )
            {
            if( !Physics.SphereCast( gameObject.transform.position, 0.5f, Quaternion.AngleAxis( i, Vector3.up ) * initialEscapeDir, out hitInfo, m_EscapeViewLength )
               || hitInfo.collider.gameObject.tag.Equals( "Prey" ) )
               {
               bestEscapeDir = Quaternion.AngleAxis( i, Vector3.up ) * initialEscapeDir;
               break;
               }
            if( !Physics.SphereCast( gameObject.transform.position, 0.5f, Quaternion.AngleAxis( -i, Vector3.up ) * initialEscapeDir, out hitInfo, m_EscapeViewLength )
               || hitInfo.collider.gameObject.tag.Equals( "Prey" ) )
               {
               bestEscapeDir = Quaternion.AngleAxis( -i, Vector3.up ) * initialEscapeDir;
               break;
               }
            }

         Quaternion newRotation = new Quaternion();
         newRotation.SetLookRotation( bestEscapeDir, Vector3.up );
         gameObject.transform.rotation = newRotation;
         }
      else
         {
         // If has reached roaming posisiton, set a random point and start roaming
         if( Vector3.SqrMagnitude( gameObject.transform.position - m_RoamingGoal ) <= 0.001 )
            {
            SetRoamingGoal();
            }
         }

      gameObject.transform.position += m_Velocity * Time.deltaTime * gameObject.transform.forward;

      Debug.DrawRay( gameObject.transform.position, gameObject.transform.forward, Color.blue );
      }

   private void SetRoamingGoal()
      {
      Vector3 random3DCirleVec;
      RaycastHit hitInfo;
      do
         {
         var random2DCircleVec = Random.insideUnitCircle.normalized;
         random3DCirleVec = new Vector3( random2DCircleVec.x, 0, random2DCircleVec.y );
         }
      while( Physics.SphereCast( gameObject.transform.position, 0.5f, random3DCirleVec, out hitInfo, 3.0f ) );
      m_RoamingGoal = transform.position + 3.0f * random3DCirleVec;
      Quaternion newRotation = new Quaternion();
      newRotation.SetLookRotation( m_RoamingGoal - gameObject.transform.position, Vector3.up );
      gameObject.transform.rotation = newRotation;
      }

   public void StartChasingSelf( GameObject predator )
      {
      m_IsBeingTraced = true;
      m_ChasingPredators.Add( predator );
      }

   public void StopChasingSelf( GameObject predator )
      {
      m_ChasingPredators.Remove( predator );
      if( m_ChasingPredators.Count == 0 )
         {
         m_IsBeingTraced = false;
         SetRoamingGoal();
         }
      }


}
