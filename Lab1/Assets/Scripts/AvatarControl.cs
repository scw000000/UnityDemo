using UnityEngine;
using System.Collections;

public class AvatarControl : MonoBehaviour
   {
   public float m_MoveSpeed = 15.0f;
   public float m_ScalingSpeed = 3.0f;
   public float m_RotatingSpeed = 3.0f;
   private Rigidbody m_RigidBody = null;
   
      // Use this for initialization
   void Start()
      {
      Debug.Log( gameObject.name );
      m_RigidBody = gameObject.GetComponent<Rigidbody>();
      if( m_RigidBody == null )
         {
         Debug.LogError( "Cannot find rigid body of the avatar" );
         }
      }

   // Update is called once per frame
   void Update()
      {
      if( Input.GetKey( KeyCode.A ) )
         {
         gameObject.transform.Translate( new Vector3( -1.0f * Time.deltaTime * m_MoveSpeed, 0.0f, 0.0f ) );
         }

      if( Input.GetKey( KeyCode.D ) )
         {
         gameObject.transform.Translate( new Vector3( Time.deltaTime * m_MoveSpeed, 0.0f, 0.0f ) );
         }

      if( Input.GetKey( KeyCode.W ) )
         {
         gameObject.transform.Translate( new Vector3( 0.0f, Time.deltaTime * m_MoveSpeed, 0.0f ) );
         }

      if( Input.GetKey( KeyCode.S ) )
         {
         gameObject.transform.Translate( new Vector3( 0.0f, -1.0f * Time.deltaTime * m_MoveSpeed, 0.0f ) );
         }

      if( Input.GetKey( KeyCode.Q ) )
         {
         gameObject.transform.Translate( new Vector3( 0.0f, 0.0f, Time.deltaTime * m_MoveSpeed ) );
         }

      if( Input.GetKey( KeyCode.E ) )
         {
         gameObject.transform.Translate( new Vector3( 0.0f, 0.0f, -1.0f * Time.deltaTime * m_MoveSpeed ) );
         }

      if( Input.GetKeyDown( KeyCode.R ) )
         {
         gameObject.transform.position = new Vector3( 0.0f, 1.0f, 0.0f );
         gameObject.transform.rotation = new Quaternion();
         gameObject.transform.localScale = new Vector3( 1.0f, 1.0f, 1.0f );

         m_RigidBody.velocity = new Vector3();
         m_RigidBody.angularVelocity = new Vector3();
         }

      if( Input.GetKey( KeyCode.Minus ) )
         {
         float scaleSize = -1.0f * Time.deltaTime * m_ScalingSpeed;
         if( gameObject.transform.localScale.x + scaleSize > 0.1 )
            {
            gameObject.transform.localScale += new Vector3( scaleSize, scaleSize, scaleSize );
            }
         }

      if( Input.GetKey( KeyCode.Equals ) )
         {
         float scaleSize = Time.deltaTime * m_ScalingSpeed;
         if( gameObject.transform.localScale.x + scaleSize > 0.1 )
            {
            gameObject.transform.localScale += new Vector3( scaleSize, scaleSize, scaleSize );
            }
         }

      if( Input.GetKey( KeyCode.Y ) )
         {
         gameObject.transform.Rotate( new Vector3( Time.deltaTime * m_RotatingSpeed, 0.0f ) );
         }

      if( Input.GetKey( KeyCode.H ) )
         {
         gameObject.transform.Rotate( new Vector3( -1.0f * Time.deltaTime * m_RotatingSpeed, 0.0f ) );
         }

      if( Input.GetKey( KeyCode.G ) )
         {
         gameObject.transform.Rotate( new Vector3( 0.0f, Time.deltaTime * m_RotatingSpeed ) );
         }

      if( Input.GetKey( KeyCode.J ) )
         {
         gameObject.transform.Rotate( new Vector3( 0.0f, -1.0f * Time.deltaTime * m_RotatingSpeed ) );
         }

      if( Input.GetKey( KeyCode.T ) )
         {
         gameObject.transform.Rotate( new Vector3( 0.0f, 0.0f, Time.deltaTime * m_RotatingSpeed ) );
         }

      if( Input.GetKey( KeyCode.U ) )
         {
         gameObject.transform.Rotate( new Vector3( 0.0f, 0.0f, -1.0f * Time.deltaTime * m_RotatingSpeed ) );
         }

      }
   }
