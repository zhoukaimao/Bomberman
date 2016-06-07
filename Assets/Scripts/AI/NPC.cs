using UnityEngine;
using System.Collections;
/// <summary>
/// store npc information
/// npc action method
/// </summary>
public class NPC : MonoBehaviour
{
    private float speed = 2f;
    private float hp = 10f;
    private int bombLeft = 1;
    private int flameDist = 1;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    /// current position is safe or not
    /// </summary>
    /// <returns>safe or not</returns>
    public bool IsSafe()
    {
        //
        return true;
    }
    public float GetHP()
    {
        return hp;
    }
    public int GetFlameDist()
    {
        return flameDist;
    }
    public int GetBombLeft()
    {
        return bombLeft;
    }
    public Vector2 GetMapPos()
    {
        return new Vector2(1,1);
    }
    public void MoveTo(Vector2 pos)
    {

        Vector3 dir = Util.Map2World(pos) - transform.position;
        if (Vector3.Magnitude(dir) < 0.1)
        {
            return;
        }
        transform.position += dir.normalized * speed * Time.deltaTime;
        
    }
    public void PlaceBomb()
    {

    }
}
