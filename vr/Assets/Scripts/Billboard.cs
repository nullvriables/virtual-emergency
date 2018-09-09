using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera m_Camera;
    void Start()
    {
        m_Camera = Camera.main;
    }
    void Update()
	{
		transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,m_Camera.transform.rotation * Vector3.up);
	}
}