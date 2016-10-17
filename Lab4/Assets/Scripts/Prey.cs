using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Prey : MonoBehaviour {
   private List< GameObject > m_VisiblePredators = new List<GameObject>();
   private bool m_IsBeingTraced = false;
   private float m_Velocity = 0.5f;
   public float m_ViewDistance = 20.0f;
   private Vector3 m_RoamingGoal = new Vector3();
   private float m_EscapeTestLength = 3.0f;
   public int m_Fov = 55;
   // Use this for initialization
   void Start ()
      {
      SetRoamingGoal();
      }
	
	// Update is called once per frame
	void Update ()
      {
      List<GameObject> newVisiblePredators = new List<GameObject>();
      var preyDir = gameObject.transform.forward;
      RaycastHit hitInfo;

      for( int i = 0; i < m_Fov + 1; ++i )
         {
         if( Physics.SphereCast( gameObject.transform.position, 0.5f, Quaternion.AngleAxis( i, Vector3.up ) * preyDir, out hitInfo, m_ViewDistance ) )
            {
            if( hitInfo.collider.gameObject.tag.Equals( "Predator" ) && !newVisiblePredators.Contains( hitInfo.collider.gameObject ) )
               {
               newVisiblePredators.Add( hitInfo.collider.gameObject );
               }
            }
         if( Physics.SphereCast( gameObject.transform.position, 0.5f, Quaternion.AngleAxis( -i, Vector3.up ) * preyDir, out hitInfo, m_ViewDistance ) )
            {
            if( hitInfo.collider.gameObject.tag.Equals( "Predator" ) && !newVisiblePredators.Contains( hitInfo.collider.gameObject ) )
               {
               newVisiblePredators.Add( hitInfo.collider.gameObject );
               }
            }
         }
      if( newVisiblePredators.Count > 0 )
         {
         Vector3 bestEscapeDir = new Vector3( 0, 0, 0 );
         Vector3 initialEscapeDir = new Vector3( 0, 0, 0 );
         m_VisiblePredators = newVisiblePredators;

         foreach( var predator in m_VisiblePredators )
            {
            initialEscapeDir += gameObject.transform.position - predator.transform.position;
            }
         initialEscapeDir.Normalize();
         for( int i = 0; i < 180; ++i )
            {
            if( !Physics.SphereCast( gameObject.transform.position, 0.5f, Quaternion.AngleAxis( i, Vector3.up ) * initialEscapeDir, out hitInfo, m_EscapeTestLength )
               || hitInfo.collider.gameObject.tag.Equals( "Prey" ) )
               {
               bestEscapeDir = Quaternion.AngleAxis( i, Vector3.up ) * initialEscapeDir;
               break;
               }
            if( !Physics.SphereCast( gameObject.transform.position, 0.5f, Quaternion.AngleAxis( -i, Vector3.up ) * initialEscapeDir, out hitInfo, m_EscapeTestLength )
               || hitInfo.collider.gameObject.tag.Equals( "Prey" ) )
               {
               bestEscapeDir = Quaternion.AngleAxis( -i, Vector3.up ) * initialEscapeDir;
               break;
               }
            }

         Quaternion newRotation = new Quaternion();
         newRotation.SetLookRotation( -initialEscapeDir, Vector3.up );
         gameObject.transform.rotation = newRotation;
         gameObject.transform.position += m_Velocity * Time.deltaTime * bestEscapeDir;
         }
      else
         {
         // Escape success, return to roaming stagte
         if( m_VisiblePredators.Count > 0 || Vector3.SqrMagnitude( gameObject.transform.position - m_RoamingGoal ) <= 0.001 )
            {
            m_VisiblePredators.Clear();
            SetRoamingGoal();
            }
         gameObject.transform.position += m_Velocity * Time.deltaTime * gameObject.transform.forward;
         }

      //if( m_IsBeingTraced )
      //   {
      //   Vector3 bestEscapeDir = new Vector3( 0, 0, 0 );
      //   Vector3 initialEscapeDir = new Vector3( 0, 0, 0 );
      //   foreach( var predator in m_VisiblePredators )
      //      {
      //      initialEscapeDir += gameObject.transform.position - predator.transform.position;
      //      }
      //   initialEscapeDir.Normalize();
      //   for( int i = 0; i < 180; ++i )
      //      {
      //      if( !Physics.SphereCast( gameObject.transform.position, 0.5f, Quaternion.AngleAxis( i, Vector3.up ) * initialEscapeDir, out hitInfo, m_EscapeTestLength )
      //         || hitInfo.collider.gameObject.tag.Equals( "Prey" ) )
      //         {
      //         bestEscapeDir = Quaternion.AngleAxis( i, Vector3.up ) * initialEscapeDir;
      //         break;
      //         }
      //      if( !Physics.SphereCast( gameObject.transform.position, 0.5f, Quaternion.AngleAxis( -i, Vector3.up ) * initialEscapeDir, out hitInfo, m_EscapeTestLength )
      //         || hitInfo.collider.gameObject.tag.Equals( "Prey" ) )
      //         {
      //         bestEscapeDir = Quaternion.AngleAxis( -i, Vector3.up ) * initialEscapeDir;
      //         break;
      //         }
      //      }

      //   Quaternion newRotation = new Quaternion();
      //   newRotation.SetLookRotation( bestEscapeDir, Vector3.up );
      //   gameObject.transform.rotation = newRotation;
      //   }
      //else
      //   {
      //   // If has reached roaming posisiton, set a random point and start roaming
      //   if( Vector3.SqrMagnitude( gameObject.transform.position - m_RoamingGoal ) <= 0.001 )
      //      {
      //      SetRoamingGoal();
      //      }
      //   }

      

      Debug.DrawRay( gameObject.transform.position, preyDir, Color.blue );
      Debug.DrawRay( gameObject.transform.position, Quaternion.AngleAxis( m_Fov, Vector3.up ) * preyDir, Color.blue );
      Debug.DrawRay( gameObject.transform.position, Quaternion.AngleAxis( -m_Fov, Vector3.up ) * preyDir, Color.blue );
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

   //public void StartChasingSelf( GameObject predator )
   //   {
   //   m_IsBeingTraced = true;
   //   m_VisiblePredators.Add( predator );
   //   }

   //public void StopChasingSelf( GameObject predator )
   //   {
   //   m_VisiblePredators.Remove( predator );
   //   if( m_VisiblePredators.Count == 0 )
   //      {
   //      m_IsBeingTraced = false;
   //      SetRoamingGoal();
   //      }
   //   }


}
