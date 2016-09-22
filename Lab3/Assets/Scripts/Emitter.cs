using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Emitter : MonoBehaviour
   {
   public float m_GenerateRate;
   public float m_MaxAge;
   public float m_MinAge;
   public float m_MaxRadius;
   public float m_MinRadius;
   public float m_GenerateSphereVolumeRadius;
   public float m_MinVelocity;
   public float m_MaxVelocity;
   public Mesh m_Mesh;
   public GameObject m_Plane;
   //public Material m_Material;
   private float m_ObjectNeedToSpawn = 0.0f;
   private LinkedList< Particle > m_AliveParticles = new LinkedList<Particle>();
   private LinkedList< Particle > m_DeadParticles = new LinkedList<Particle>();

   // Use this for initialization
   void Start()
      {

      }

   private void SetParticleGameObject( GameObject particleObj )
      {
      Particle particle = particleObj.GetComponent<Particle>();
      particleObj.transform.position = gameObject.transform.position + Random.insideUnitSphere * m_GenerateSphereVolumeRadius;

      //meshRenderer.material = m_Material;
      particle.m_MaxAge = Random.Range( m_MinAge, m_MaxAge );
      particle.m_Age = 0.0f;
      particle.m_Velocity = Random.onUnitSphere * Random.Range( m_MinVelocity, m_MaxVelocity );
      particle.m_Acceleration = new Vector3( 0.0f, -9.8f, 0.0f );
      particle.m_Radius = Random.Range( m_MinRadius, m_MaxRadius );

      particleObj.transform.localScale = new Vector3( particle.m_Radius * 2.0f, particle.m_Radius * 2.0f, particle.m_Radius * 2.0f );
      }

   private void SpawnParticle( int num )
      {
      while( m_DeadParticles.Count > 0 && num >0 )
         {
         var toRespawnNode = m_DeadParticles.First;
         var toRespawnParticle = toRespawnNode.Value;
         m_DeadParticles.Remove( toRespawnNode );
         m_AliveParticles.AddLast( toRespawnParticle );
         toRespawnParticle.SetVisible( true );
         SetParticleGameObject( toRespawnParticle.gameObject );
         --num;
         }
      for( int i = 0; i < num; ++i )
         {
         GameObject particleObj = new GameObject();
         Particle particle = particleObj.AddComponent<Particle>();
         particleObj.AddComponent<MeshFilter>().mesh = m_Mesh;
         particleObj.AddComponent<MeshRenderer>();
         particle.m_Plane = m_Plane;
         m_AliveParticles.AddLast( particle );
         SetParticleGameObject( particleObj );
         }
      }

   private void RecycleParticle()
      {
      var currentNode = m_AliveParticles.First;
      while( currentNode != null )
         {
         if( currentNode.Value.ShouldRecycle() )
            {
            var toRemoveNode = currentNode;
            var toRemoveParticle = toRemoveNode.Value;
            toRemoveParticle.SetVisible( false );
            currentNode = currentNode.Next;
            m_AliveParticles.Remove( toRemoveNode );
            m_DeadParticles.AddLast( toRemoveParticle );
            }
         else
            {
            currentNode = currentNode.Next;
            }
         }
      }

   // Update is called once per frame
   void Update()
      {
      RecycleParticle();

      m_ObjectNeedToSpawn += m_GenerateRate * Time.deltaTime;
      int generateParticleNum = ( int ) m_ObjectNeedToSpawn;
      m_ObjectNeedToSpawn -= generateParticleNum;
      SpawnParticle( generateParticleNum );
      }
   }
