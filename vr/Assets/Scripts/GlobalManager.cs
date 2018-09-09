using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
public class GlobalManager : MonoBehaviour {

    public GameObject schools;
    public GameObject incidents;
    public GameObject waternodes;
    public GameObject sportsnodes;
    public GameObject pinpoint;
    public GameObject buses;
    public GameObject buildings;
    public GameObject fire;
    public GameObject firesparks;
    public GameObject skyDome;
    public GameObject rain;

    private Vector2 _localOrigin = Vector2.zero;
    private float _LatOrigin { get { return _localOrigin.x; } }
    private float _LonOrigin { get { return _localOrigin.y; } }
    public static LayerMask tempMask;
    private float metersPerLat;
    private float metersPerLon;
    public bool isRaining;
    private ParticleSystem ps;
    private ParticleSystem sparksps;
    // Use this for initialization
    void Awake()
    {
        // Set Origin point to Parliament house instead of Africa which would be 0,0
        SetLocalOrigin(new Vector2(-35.3082f, 149.1244f));
    }
    void Start()
    {
        isRaining = false;
        ps = rain.GetComponent<ParticleSystem>();
        sparksps = firesparks.GetComponent<ParticleSystem>();
        tempMask = LayerMask.GetMask("Terrain");

        // Load Schools
        List<Dictionary<string, object>> schooldata = CSVReader.Read("ACTSchools");
        for (var i = 0; i < schooldata.Count; i++)
        {
            GameObject instance1 = Instantiate(Resources.Load("School", typeof(GameObject))) as GameObject;
            instance1.transform.Find("NodeText").gameObject.GetComponent<TextMesh>().text = schooldata[i]["school"].ToString();
            instance1.name = schooldata[i]["id"].ToString();
            instance1.transform.parent = schools.transform; // this binds it to the parent object
            instance1.transform.position = ConvertGPStoUCS(new Vector2((float)schooldata[i]["lat"], (float)schooldata[i]["long"]));
            instance1.isStatic = true;
        }
        StaticBatchingUtility.Combine(schools);

        // Load Incidents
        List<Dictionary<string, object>> incidentdata = CSVReader.Read("incidents");
        for (var i = 0; i < incidentdata.Count; i++)
        {
            GameObject instance2 = Instantiate(Resources.Load("Incident", typeof(GameObject))) as GameObject;
            instance2.transform.Find("NodeText").gameObject.GetComponent<TextMesh>().text = incidentdata[i]["type"].ToString();
            instance2.name = incidentdata[i]["id"].ToString();
            instance2.transform.parent = incidents.transform; // this binds it to the parent object
            instance2.transform.position = ConvertGPStoUCS(new Vector2((float)incidentdata[i]["lat"], (float)incidentdata[i]["long"]));
            instance2.isStatic = true;
        }
        StaticBatchingUtility.Combine(incidents);

        // Load Water
        List<Dictionary<string, object>> waterdata = CSVReader.Read("waterpoints");
        for (var i = 0; i < waterdata.Count; i++)
        {
            GameObject instance3 = Instantiate(Resources.Load("Water", typeof(GameObject))) as GameObject;
            instance3.transform.Find("NodeText").gameObject.GetComponent<TextMesh>().text = waterdata[i]["name"].ToString();
            instance3.name = waterdata[i]["id"].ToString();
            instance3.transform.parent = waternodes.transform; // this binds it to the parent object
            instance3.transform.position = ConvertGPStoUCS(new Vector2((float)waterdata[i]["lat"], (float)waterdata[i]["long"]));
            instance3.isStatic = true;
        }
        StaticBatchingUtility.Combine(waternodes);

        // Load Sports Ovals
        List<Dictionary<string, object>>sportsdata = CSVReader.Read("sport");
        for (var i = 0; i < sportsdata.Count; i++)
        {
            GameObject instance4 = Instantiate(Resources.Load("Sports", typeof(GameObject))) as GameObject;
            instance4.transform.Find("NodeText").gameObject.GetComponent<TextMesh>().text = sportsdata[i]["name"].ToString();
            instance4.name = sportsdata[i]["id"].ToString();
            instance4.transform.parent = sportsnodes.transform; // this binds it to the parent object
            instance4.transform.position = ConvertGPStoUCS(new Vector2((float)sportsdata[i]["lat"], (float)sportsdata[i]["long"]));
            instance4.isStatic = true;
        }
        StaticBatchingUtility.Combine(sportsnodes);

        // Load Route Shapes
        List<Dictionary<string, object>> shapesdata = CSVReader.Read("shapes");

        // Load Buses
        List<Dictionary<string, object>> busdata = CSVReader.Read("ESAVehicles");
        for (var i = 0; i < busdata.Count; i++)
        {
            List<Vector3> busPath = new List<Vector3>();
            bool foundFirstPoint = false;
            Vector3 firstPoint = new Vector3();
            for (var j = 0; j < shapesdata.Count; j++)
            {
                if (busdata[i]["id"].ToString() == shapesdata[j]["id"].ToString())
                {
                    Vector2 busRouteGPS = new Vector2(float.Parse(shapesdata[j]["lat"].ToString()), float.Parse(shapesdata[j]["long"].ToString()));
                    busPath.Add(ConvertGPStoUCS(busRouteGPS));
                    if (foundFirstPoint == false)
                    {
                        foundFirstPoint = true;
                        firstPoint = ConvertGPStoUCS(busRouteGPS);
                    }
                }
            } 
            GameObject instance = Instantiate(Resources.Load("Bus", typeof(GameObject))) as GameObject;
            instance.transform.Find("NodeText").gameObject.GetComponent<TextMesh>().text = busdata[i]["id"].ToString();
            instance.name = busdata[i]["id"].ToString();
            instance.transform.position = firstPoint;
            instance.transform.parent = buses.transform; // this binds it to the parent object
                                                            //instance.transform.position = ConvertGPStoUCS(new Vector2((float)busdata[i]["lat"], (float)busdata[i]["long"]));
            instance.transform.GetComponent<Bus>().setPath(busPath);
        }
        // Load Stations
        /*List<Dictionary<string, object>> crashdata = CSVReader.Read("ACT_Road_Crash_Data");
        for (var i = 0; i < crashdata.Count; i++)
        {
            //print("id " + stationdata[i]["id"] + " " + "name " + stationdata[i]["name"] + " " + "lat " + stationdata[i]["lat"] + " " + "long " + stationdata[i]["long"]);
            GameObject instance = Instantiate(Resources.Load("MediumFlames", typeof(GameObject))) as GameObject;
            instance.name = crashdata[i]["id"].ToString();
            instance.transform.position = ConvertGPStoUCS(new Vector2((float)crashdata[i]["lat"], (float)crashdata[i]["long"]));
            instance.transform.parent = crashes.transform; // this binds it to the parent object
            instance.isStatic = true;
        }
        StaticBatchingUtility.Combine(crashes);
        */
        
    }
    private void Update()
    {
        if (Input.GetKeyDown("0") || Input.GetButton("XboxY"))
        {
            print("0 was pressed - do day time");
            skyDome.GetComponent<TOD_Sky>().Cycle.Hour = 13f;
        }
        if (Input.GetKeyDown("1") || Input.GetButton("XboxA"))
        {
            print("1 was pressed - do night time");
            skyDome.GetComponent<TOD_Sky>().Cycle.Hour = 20f;
        }
        if (Input.GetKeyDown("2") || Input.GetButton("XboxX"))
        {
            print("1 was pressed - do Dawn");
            skyDome.GetComponent<TOD_Sky>().Cycle.Hour = 6.5f;
        }
        if (Input.GetKeyDown("3") || Input.GetButton("XboxB"))
        {
            print("1 was pressed - do Dusk");
            skyDome.GetComponent<TOD_Sky>().Cycle.Hour = 17.6f;
        }
        if (Input.GetKeyDown("4") || Input.GetButton("StartRain"))
        {
            print("4 was pressed - do start Rain");
            var emission = ps.emission;
            emission.enabled = true;
            isRaining = true;
         }
        if (Input.GetKeyDown("5") || Input.GetButton("StopRain"))
        {
            print("4 was pressed - do stop Rain");
            var emission = ps.emission;
            emission.enabled = false;
            isRaining = false;
        }
        if (Input.GetButton("XboxStart"))
        {
            print("Start was pressed - do Fire");
            incidents.SetActive(false);
            fire.GetComponent<MeshRenderer>().enabled = true;
            var emission = sparksps.emission;
            emission.enabled = true;
            RenderSettings.fogDensity = 0.0002f;
        }
        if (Input.GetButton("XboxBack"))
        {
            print("Back was pressed - do Drop PinPoint");
            pinpoint.GetComponent<MeshRenderer>().enabled = true;
        }
    }
    private void SetLocalOrigin(Vector2 localOrigin)
    {
        _localOrigin = localOrigin;
    }
    private void FindMetersPerLat(float lat) // Compute lengths of degrees
    {
        float m1 = 111132.92f;    // latitude calculation term 1
        float m2 = -559.82f;        // latitude calculation term 2
        float m3 = 1.175f;      // latitude calculation term 3
        float m4 = -0.0023f;        // latitude calculation term 4
        float p1 = 111412.84f;    // longitude calculation term 1
        float p2 = -93.5f;      // longitude calculation term 2
        float p3 = 0.118f;      // longitude calculation term 3

        lat = lat * Mathf.Deg2Rad;

        // Calculate the length of a degree of latitude and longitude in meters
        metersPerLat = m1 + (m2 * Mathf.Cos(2 * (float)lat)) + (m3 * Mathf.Cos(4 * (float)lat)) + (m4 * Mathf.Cos(6 * (float)lat));
        metersPerLon = (p1 * Mathf.Cos((float)lat)) + (p2 * Mathf.Cos(3 * (float)lat)) + (p3 * Mathf.Cos(5 * (float)lat));
    }

    private Vector3 ConvertGPStoUCS(Vector2 gps)
    {
        // Set Parliament house to local origin 0,0,0 because it IS the origin
        if (gps == new Vector2(-35.3082f, 149.1244f))
        {
            Vector3 tempWorldPosition = new Vector3(0, 0, 0);
            return new Vector3(0.0f, Terrain.activeTerrain.SampleHeight(tempWorldPosition), 0.0f);
        }
        else
        {
            FindMetersPerLat(_LatOrigin);
            float zPosition = metersPerLat * (gps.x - _LatOrigin); //Calc current lat
            float xPosition = metersPerLon * (gps.y - _LonOrigin); //Calc current lat
            Vector3 tempPosition = new Vector3((float)xPosition, 200.0f, (float)zPosition);
            RaycastHit hit;
            if (Physics.Raycast(tempPosition, -Vector3.up, out hit))
            {
                if (hit.collider.gameObject.layer == 9) // Terrain
                {
                    if (hit.distance > 0.1f)
                    {
                        tempPosition.y = hit.point.y + 1.0f;
                    }
                }

            }
            return tempPosition;
        }
    }
    
    private Vector2 ConvertUCStoGPS(Vector3 position)
    {
        FindMetersPerLat(_LatOrigin);
        Vector2 geoLocation = new Vector2(0, 0);
        geoLocation.x = (_LatOrigin + (position.z) / metersPerLat); //Calc current lat
        geoLocation.y = (_LonOrigin + (position.x) / metersPerLon); //Calc current lon
        return geoLocation;
    }
}
