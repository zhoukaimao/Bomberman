using UnityEngine;

public class MapCell : MonoBehaviour {

    public struct MapCellPointArray
    {
        public int x;
        public int y;
    }
    private MapCellPointArray myMapCellPointArray;

    private GameObject MainController;
	// Use this for initialization
	void Start () {
        MainController = GameObject.Find("GameManager");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    //button pressed
    public void myButtonPressed()
    {
        MainController.GetComponent<CreateMapManager>().SendMessage("pressed_MapCell", myMapCellPointArray);
    }
    //sendmessage
    public void SetArrayPoint(MapCellPointArray set)
    {
        myMapCellPointArray = set;
    }
}
