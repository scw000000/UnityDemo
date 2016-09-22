using UnityEngine;
using System.Collections;

public class RotateControl : MonoBehaviour
   {
   public float m_RotateSpeed = 3.0f;
   // Use this for initialization
   void Start()
      {

      }

   // Update is called once per frame
   void Update()
      {
      if( Input.GetKey( KeyCode.Alpha0 ) )
         {
         gameObject.transform.RotateAround( gameObject.transform.position, gameObject.transform.up, Time.deltaTime * m_RotateSpeed );
         }
      }
   }