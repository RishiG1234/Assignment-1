using System.Collections.Generic;
using UnityEngine;

public class SolarSystem : MonoBehaviour
{
    [SerializeField] private DataCSV dataCSV;
    [SerializeField] private Material planetMaterial;
    [SerializeField] private Material sunMaterial;

    private const float PLANET_SIZE = 0.3f;
    private const float SUN_SIZE = 10f;

    // Scaling factors
    private const float DIST_SCALE = 1e-9f;   // scale down distances (AU → Unity units)
    private const float TIME_SCALE = 1000f;   // speed up time

    public class Body
    {
        public GameObject obj;
        public float radius;      // scaled orbit radius
        public float angle;       // current angle in radians
        public float speed;       
    }

    private readonly List<Body> bodies = new List<Body>();

    void Awake()
    {
        if (dataCSV == null)
        {
            dataCSV = Object.FindFirstObjectByType<DataCSV>();
        }
    }

    void Start()
    {
        if (dataCSV == null || dataCSV.bp == null || dataCSV.bp.Length == 0)
        {
            Debug.LogError("No CSV data found.");
            return;
        }

        // --- Sun ---
        GameObject sun = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sun.transform.position = Vector3.zero;
        sun.transform.localScale = Vector3.one * SUN_SIZE;
        sun.GetComponent<Renderer>().material = sunMaterial;

        float GM = 1f;

        // --- Planets ---
        for (int i = 1; i < dataCSV.bp.Length; i++)
        {
            BodyProperty bp = dataCSV.bp[i];

            float radius = bp.distance * DIST_SCALE;

            float speed = Mathf.Sqrt(GM / radius) * TIME_SCALE;

            GameObject planet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            planet.transform.position = new Vector3(radius, 0f, 0f);
            planet.transform.localScale = Vector3.one * PLANET_SIZE;
            planet.GetComponent<Renderer>().material = planetMaterial;
            planet.GetComponent<Renderer>().material.color = Random.ColorHSV();

            // Trails
            var trail = planet.AddComponent<TrailRenderer>();
            trail.time = 50f;
            trail.startWidth = 0.05f;
            trail.endWidth = 0.01f;
            trail.material = new Material(Shader.Find("Sprites/Default"));
            trail.startColor = planet.GetComponent<Renderer>().material.color;
            trail.endColor = Color.clear;

            bodies.Add(new Body
            {
                obj = planet,
                radius = radius,
                angle = 0f,
                speed = speed
            });
        }
    }

    void Update()
    {
        foreach (var body in bodies)
        {
            body.angle += body.speed * Time.deltaTime;

            float x = body.radius * Mathf.Cos(body.angle);
            float z = body.radius * Mathf.Sin(body.angle);

            body.obj.transform.position = new Vector3(x, 0f, z);
        }
    }
}
