using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct Tile
{
    public enum Type { Grass, Water, Mountain, Coal, Iron };
    public Sprite center, left, right, top, bottom, topLeft, topRight, bottomLeft, bottomRight;
    public int food;
    public int production;
    public int gold;
    public int water;
    public int air;
}

[System.Serializable]
public struct Layer
{
    public enum Type { Additive, Subtractive, Multiplication, Division, Modulo, Exponentiation, Logarithm };

    public Type type;
    public float frequency;
    public float minimum;
    public float maximum;
}

public class World : MonoBehaviour
{
    public int width = 64;
    public int height = 64;

    public int seed = 0;
    public float frequency = 0.1f;
    [Range(0f, 1f)]
    public float falloff =  0.5f;
    [Range(0f, 1f)]
    public float seaLevel = 0.1f;
    public float mountLevel = 0.5f;

    public Layer[] layers;

    public Tile grass;
    public Tile water;
    public Tile mountain;
    public Tile coal;

    private Tile.Type[,] tiles;
    public GameObject[,] gameObjects;

    void Awake()
    {
        Generate();
    }

    void OnValidate()
    {
        if(Application.isPlaying)
            Generate();
    }

    void Generate()
    {
        if (tiles == null)
            tiles = new Tile.Type[width, height];
        Vector2 center = new Vector2(width * 0.5f, height * 0.5f) - new Vector2(0.5f, 0.5f);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 point = new Vector2(x, y);
                float distance = Vector2.Distance(center, point);
                float value = 0f;
                foreach(Layer layer in layers)
                {
                    float noise = layer.minimum + Mathf.PerlinNoise(seed + ((float)x / width) * layer.frequency,
                        seed + ((float)y / height) * layer.frequency) * Mathf.Clamp((layer.maximum - layer.minimum), 0f, float.MaxValue);
                    switch (layer.type)
                    {
                        case Layer.Type.Additive:
                            value += noise;
                            break;
                        case Layer.Type.Subtractive:
                            value -= noise;
                            break;
                        case Layer.Type.Multiplication:
                            value *= noise;
                            break;
                        case Layer.Type.Division:
                            value /= noise;
                            break;
                        case Layer.Type.Modulo:
                            value %= noise;
                            break;
                        case Layer.Type.Exponentiation:
                            value = Mathf.Pow(value, noise);
                            break;
                        case Layer.Type.Logarithm:
                            value = Mathf.Log(value, noise);
                            break;
                    }
                }
                value *= (1f - distance / (Mathf.Min(width, height) * falloff));
                if (value >= seaLevel)
                {
                    if (value >= mountLevel)
                    {

                        tiles[x, y] = Tile.Type.Mountain;
                        float mresourcen = Random.Range(1, 101) / 100;
                        Debug.Log(mresourcen);
                        if (mresourcen > 0.5f)
                        {
                            if (mresourcen > 0.8f)
                            {
                                tiles[x, y] = Tile.Type.Coal;
                            }
                        }
                    }
                    else
                    {
                    
                        tiles[x, y] = Tile.Type.Grass;
                    }
                }
                else
                {
                    tiles[x, y] = Tile.Type.Water;
                }
            }
        }

        if (gameObjects == null)
            gameObjects = new GameObject[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject gameObject = gameObjects[x, y];
                if (!gameObject)
                {
                    gameObject = new GameObject("(" + x + ", " + y + ")");
                    gameObject.hideFlags = HideFlags.None;
                }
                SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                if (!spriteRenderer)
                    spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

                if (tiles[x, y] == Tile.Type.Grass)
                {
                    if (tiles[x - 1, y] == Tile.Type.Water)
                        if (tiles[x, y + 1] == Tile.Type.Water)
                            spriteRenderer.sprite = grass.topLeft;
                        else if (tiles[x, y - 1] == Tile.Type.Water)
                            spriteRenderer.sprite = grass.bottomLeft;
                        else
                            spriteRenderer.sprite = grass.left;
                    else if (tiles[x + 1, y] == Tile.Type.Water)
                        if (tiles[x, y + 1] == Tile.Type.Water)
                            spriteRenderer.sprite = grass.topRight;
                        else if (tiles[x, y - 1] == Tile.Type.Water)
                            spriteRenderer.sprite = grass.bottomRight;
                        else
                            spriteRenderer.sprite = grass.right;
                    else if (tiles[x, y + 1] == Tile.Type.Water)
                        spriteRenderer.sprite = grass.top;
                    else if (tiles[x, y - 1] == Tile.Type.Water)
                        spriteRenderer.sprite = grass.bottom;
                    else
                        spriteRenderer.sprite = grass.center;
                }
                else if (tiles[x, y] == Tile.Type.Water)
                    spriteRenderer.sprite = water.center;
                else if (tiles[x, y] == Tile.Type.Mountain)
                    spriteRenderer.sprite = mountain.center;
                else if (tiles[x, y] == Tile.Type.Coal)
                    spriteRenderer.sprite = coal.center;
                //spriteRenderer.sprite = (tiles[x, y] == Tile.Type.Grass) ? grass.sprite : water.sprite;
                Vector2 size = spriteRenderer.sprite.bounds.size;
                gameObject.transform.parent = transform;
                gameObject.transform.position = new Vector3(size.x * x, size.y * y) - new Vector3(size.x * width, size.y * height) * 0.5f;
                gameObjects[x, y] = gameObject;
            }
        }
    }
}
