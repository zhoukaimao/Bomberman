using UnityEngine;
using System.Collections;

public class createdObject : MonoBehaviour {
    private bool m_canCreate;
	// Use this for initialization
	void Start () {
        m_canCreate = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public bool get_canCreate()
    {
        return m_canCreate;
    }
    public void set_canCreate(bool set)
    {
        m_canCreate = set;
    }
}
