using UnityEngine;
using System.Collections;
using System;

public class MyHPSlider : MonoBehaviour {
    public GameObject owner;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (owner != null)
        {
            gameObject.SetActive(true);
            var hp = owner.GetComponent<Bomberman>().HP;
            var maxHp = owner.GetComponent<Bomberman>().GetMaxHP();
            GetComponent<UISlider>().value = hp / maxHp;
            GetComponentInChildren<UILabel>().text = hp.ToString();
            var ownerPos = owner.transform.position;
            var hpBarPos = ownerPos + new Vector3(0,2,0);
            var pos = Camera.main.WorldToScreenPoint(hpBarPos);
            try
            {
                var ff = UICamera.currentCamera.ScreenToWorldPoint(pos);
                ff.z = 0;
                transform.position = ff;
            }
            catch(Exception ex)
            {
                Debug.Log(ex.Message);
            }
            
            //transform.LookAt(Camera.main.transform.position);

        }
        else
        {
            gameObject.SetActive(false);
        }
	}

}
