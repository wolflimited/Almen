using UnityEngine;
using System.Collections;

public class World : MonoBehaviour {
    public int width;
    public int height;
    public float frequency;
    public float waterl;
    public float falloff;
    public Sprite grass;
    public Sprite water;
    private TileType[,] tiles;
    public enum TileType { Grass, Water };
	// Use this for initialization
	void Start () {
        tiles = new TileType[width, height];
        Vector2 center = new Vector2(width * 0.5f, height * 0.5f);
	    for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                float n = Mathf.PerlinNoise((float)x * frequency, (float)y * frequency);
                Vector2 point = new Vector2(x, y);
                float distance = Vector2.Distance(center, point);
                n *= (1f - (distance / (Mathf.Min(width,height) * 0.5f)) * falloff);
                //GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
                //gameObject.transform.position = new Vector3(x, y);
                //MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
                GameObject gameObject = new GameObject();
                SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = (n > waterl) ? grass : water;
                gameObject.transform.position = new Vector3(spriteRenderer.sprite.bounds.size.x * x, spriteRenderer.sprite.bounds.size.y * y);
                tiles[x,y] = (n > waterl) ? TileType.Grass : TileType.Water;
                /* 
                if (n > waterl)
                {
                    meshRenderer.material.color = Color.green;
                }
                else
                {
                    meshRenderer.material.color = Color.blue;
                } 
                */
            }
        }    
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
