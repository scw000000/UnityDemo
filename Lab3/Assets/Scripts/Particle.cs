using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Particle : MonoBehaviour
   {
   public Vector3 m_Velocity;
   public Vector3 m_Acceleration;
   public float m_MaxAge;
   public float m_Age = 0.0f;
   public float m_Mass;
   public float m_Radius;
   public SpriteRenderer m_SpriteRenderer;
   public GameObject m_MainCamera;
   private Vector3 m_PlaneNormal;
   private Vector3 m_PlanePos;
   public GameObject m_Plane
      {
      set
         {
         m_PlaneNormal = value.transform.up.normalized;
         m_PlanePos = value.transform.position;
         }
      }
   // Use this for initialization
   void Start()
      {
      }

   bool IsAbovePlane( Vector3 nxtPosition )
      {
      return Vector3.Dot( m_PlaneNormal, nxtPosition - m_PlanePos ) >= m_Radius;
      }

   private void UpdateColor( float deltaSecond )
      {
      float halfMaxAge = m_MaxAge / 2.0f;
      if( m_Age <= halfMaxAge )
         {
         m_SpriteRenderer.color = Color.Lerp( Color.red, Color.green, m_Age / halfMaxAge );
         }
      else
         {
         m_SpriteRenderer.color = Color.Lerp( Color.green, Color.blue, ( m_Age - halfMaxAge ) / halfMaxAge );
         }
      }

   private void UpdateMotion( float deltaSecond )
      {
      float midPointDeltaTime = deltaSecond / 2.0f;
      Vector3 midPointVelocity = m_Velocity + m_Acceleration * midPointDeltaTime;

      if( !IsAbovePlane( gameObject.transform.position + midPointVelocity * deltaSecond ) )
         {
         float timeToPlane = ( m_Radius - Vector3.Dot( m_PlaneNormal, gameObject.transform.position - m_PlanePos ) ) / Vector3.Dot( m_PlaneNormal, midPointVelocity );
         gameObject.transform.position += midPointVelocity * timeToPlane;
         m_Velocity -= 2 * Vector3.Dot( m_Velocity, m_PlaneNormal ) * m_PlaneNormal;
         UpdateMotion( deltaSecond - timeToPlane );
         return;
         }
      else
         {
         gameObject.transform.position += midPointVelocity * deltaSecond;
         // This would be wrong if acceleration is not remain constant
         m_Velocity += m_Acceleration * deltaSecond;
         }

      }

   // Update is called once per frame
   void Update()
      {
      if( m_Age < m_MaxAge )
         {
         m_Age += Time.deltaTime;
         UpdateMotion( Time.deltaTime );
         UpdateColor( Time.deltaTime );
         gameObject.transform.LookAt( m_MainCamera.transform, gameObject.transform.up );
         }
      }

   public bool ShouldRecycle()
      {
      return m_Age >= m_MaxAge;
      }

   public void SetVisible( bool isVisible )
      {
      m_SpriteRenderer.enabled = isVisible;
      }
   }
