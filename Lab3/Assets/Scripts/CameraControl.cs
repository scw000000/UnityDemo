using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
   {
   public GameObject m_Player;
   public float m_ViewRotationSpeed;
   private Vector3 m_Offset;
   private Quaternion m_InitialRotation;
   private Vector3 prevMousePos;
   private Vector3 currMousePos;
   // Use this for initialization
   void Start()
      {
      m_Offset = transform.position - m_Player.transform.position;
      m_ViewRotationSpeed = 25.0f;
      m_InitialRotation = gameObject.transform.rotation;
      prevMousePos = Input.mousePosition;
      }

   
   // Update is called once per frame
   void Update()
      {

    //  gameObject.transform.position = m_Player.transform.position + m_Offset;

      currMousePos = Input.mousePosition;
      if( Input.GetKey( KeyCode.Mouse0 ) )
         {
         Vector3 deltaMousePos = currMousePos - prevMousePos;
         
         gameObject.transform.RotateAround( gameObject.transform.position, gameObject.transform.right, -1.0f * deltaMousePos.y * Time.deltaTime * m_ViewRotationSpeed );
         gameObject.transform.RotateAround( gameObject.transform.position, new Vector3( 0.0f, 1.0f, 0.0f ), deltaMousePos.x * Time.deltaTime * m_ViewRotationSpeed );
         }

      prevMousePos = currMousePos;

      if( Input.GetKeyDown( KeyCode.R ) )
         {
         gameObject.transform.rotation = m_InitialRotation;
         }
      }
   }
