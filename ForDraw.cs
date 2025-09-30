using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ForDraw : MonoBehaviour
{
   public class Body
   {
       public GameObject obj;
       public Vector3 velocity;
       public float mass;
   }
  
   public int numberOfBodies = 100;
   public float gravitationalConstant = 0.5f;
   public float bodyMass = 1f;
   public float initialVelocityScale = 0.5f;


   private List<Body> bodies = new List<Body>();


   void Start()
   {
      Material trailMaterial = new Material(Shader.Find("Sprites/Default"));


       for (int i = 0; i < numberOfBodies; i++)
       {
           // Directly create sphere
           GameObject b = GameObject.CreatePrimitive(PrimitiveType.Sphere);
           b.transform.position = Random.insideUnitSphere * 30f;
           b.transform.localScale = Vector3.one * 0.4f;


           var trail = b.AddComponent<TrailRenderer>();
           trail.time = 5f;
           trail.startWidth = 0.04f;
           trail.endWidth = 0.01f;
           trail.material = trailMaterial;
           trail.startColor = new Color(Random.value, Random.value, Random.value);
           trail.endColor = Color.clear;


           bodies.Add(new Body
           {
               obj = b,
               velocity = Random.insideUnitSphere * initialVelocityScale,
               mass = bodyMass
           });
       }
   }


   void Update()
   {
      float dt = Time.fixedDeltaTime;


       Vector3[] accelerations = new Vector3[bodies.Count];


       for (int i = 0; i < bodies.Count; i++)
       {
           Vector3 acc = Vector3.zero;
           Vector3 posI = bodies[i].obj.transform.position;


           for (int j = 0; j < bodies.Count; j++)
           {
               if (i == j) continue;
               Vector3 diff = bodies[j].obj.transform.position - posI;
               float distSqr = diff.sqrMagnitude + 0.1f;
               acc += gravitationalConstant * bodies[j].mass * diff / (distSqr * Mathf.Sqrt(distSqr));
           }
           accelerations[i] = acc;
       }


       for (int i = 0; i < bodies.Count; i++)
       {
           bodies[i].velocity += accelerations[i] * dt;
           bodies[i].obj.transform.position += bodies[i].velocity * dt;
       }
   }
}







