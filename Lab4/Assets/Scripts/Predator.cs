using UnityEngine;
using System.Collections;

public class Predator : MonoBehaviour {
   public int m_Fov = 55;
   private float m_Velocity = 1.0f;
   public float m_ViewDistance = 20.0f;
   private GameObject m_CurrentTarget = null;
   private Vector3 m_RoamingGoal = new Vector3();
   //private float m_UpdateRoamDir
	// Use this for initialization
	void Start ()
      {
      }
	
	// Update is called once per frame
	void Update ()
      {
      GameObject newBestTarget = null;
      float besDistance = 0.0f;
      var bestHuntDir = new Vector3();
      var predatorDir = gameObject.transform.forward;
      RaycastHit hitInfo;

      for( int i =  0; i < m_Fov + 1; ++i )
         {
         if( Physics.SphereCast( gameObject.transform.position, 0.5f, Quaternion.AngleAxis( i, Vector3.up ) * predatorDir, out hitInfo, m_ViewDistance ) )
            {
            if( hitInfo.collider.gameObject.tag.Equals( "Prey" ) &&
               ( newBestTarget == null || ( hitInfo.distance < besDistance ) ) )
               {
               newBestTarget = hitInfo.collider.gameObject;
               besDistance = hitInfo.distance;
               bestHuntDir = Quaternion.AngleAxis( i, Vector3.up ) * predatorDir;
               }
            }
         if( Physics.SphereCast( gameObject.transform.position, 0.5f, Quaternion.AngleAxis( -i, Vector3.up ) * predatorDir, out hitInfo, m_ViewDistance ) )
            {
            if( hitInfo.collider.gameObject.tag.Equals( "Prey" ) &&
               ( newBestTarget == null || ( hitInfo.distance < besDistance ) ) )
               {
               newBestTarget = hitInfo.collider.gameObject;
               besDistance = hitInfo.distance;
               bestHuntDir = Quaternion.AngleAxis( -i, Vector3.up ) * predatorDir;
               }
            }
         }
      if( newBestTarget != null )
         {
         m_CurrentTarget = newBestTarget;
         Quaternion newRotation = new Quaternion();
         bestHuntDir.Normalize();
         newRotation.SetLookRotation( bestHuntDir, Vector3.up );
         gameObject.transform.rotation = newRotation;
         }
      else
         {
         // Tracing failed or has reached roaming posisiton, set a random point and start roaming
         if( m_CurrentTarget != null || ( Vector3.SqrMagnitude( gameObject.transform.position - m_RoamingGoal ) <= 0.001 ) )
            {
            m_CurrentTarget = null;
            SetRoamingGoal();
            }
         Quaternion newRotation = new Quaternion();
         newRotation.SetLookRotation( m_RoamingGoal - gameObject.transform.position, Vector3.up );
         gameObject.transform.rotation = newRotation;
         }

       gameObject.transform.position += m_Velocity * Time.deltaTime * gameObject.transform.forward;
     // gameObject.transform.Translate( gameObject.transform.forward * m_Velocity * Time.deltaTime );

      Debug.DrawRay( gameObject.transform.position, gameObject.transform.forward, Color.red );
      Debug.DrawRay( gameObject.transform.position, Quaternion.AngleAxis( m_Fov, Vector3.up ) * predatorDir, Color.red );
      Debug.DrawRay( gameObject.transform.position, Quaternion.AngleAxis( -m_Fov, Vector3.up ) * predatorDir, Color.red );
      }

   void SetRoamingGoal()
      {
      m_CurrentTarget = null;
      RaycastHit hitInfo;
      Vector3 random3DCirleVec;
      do
         {
         var random2DCircleVec = Random.insideUnitCircle.normalized;
         random3DCirleVec = new Vector3( random2DCircleVec.x, 0, random2DCircleVec.y );
         }
      while( Physics.SphereCast( gameObject.transform.position, 0.5f, random3DCirleVec, out hitInfo, 3.0f ) );
      m_RoamingGoal = transform.position + 3.0f * random3DCirleVec;
      }

   void StopChasingPrey()
      {
      if( m_CurrentTarget != null )
         {
         m_CurrentTarget = null;
         SetRoamingGoal();
         }
      }

   void OnTriggerEnter( Collider collider)
      {
      Predator predatorComp;
      if( collider.gameObject.tag.Equals( "Prey" ) )
         {
         Debug.Log( "Prey eaten!" );
         var predators = GameObject.FindGameObjectsWithTag( "Predator" );
         // tell all predators who are chasing same prey to stop
         foreach( var predator in predators )
            {
            predatorComp = predator.GetComponent<Predator>();
            if( predatorComp.m_CurrentTarget != null && predatorComp.m_CurrentTarget.GetInstanceID() == collider.gameObject.GetInstanceID() )
               {
               predatorComp.StopChasingPrey();
               }
            }
         Destroy( collider.gameObject );
         }

      }
}
