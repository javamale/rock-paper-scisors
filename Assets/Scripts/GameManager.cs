using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour
{
    private const int GAME_SCENE_BUILD_INDEX = 2;

    public static GameManager instance;

    public GameSceneUIManager gameSceneUIManager;

    public GameObject rockPrefab;
    public GameObject paperPrefab;
    public GameObject scissorsPrefab;

    public GameObject sessionObjects;

    public int totalObjectCount = 1000;
    public float speed = 9f;

    public int RockCount { get => rockCount; }
    private int rockCount = 0;

    public int PaperCount { get => paperCount; }
    private int paperCount = 0;

    public int ScissorsCount { get => scissorsCount; }
    private int scissorsCount = 0;

    // 0 rock 1 paper 2 scissors
    private List<int> pickList = new List<int>();

    public Vector2 GameViewBoundaries { get { return GetGameViewBoundaries(); } }

    private Vector2 gameViewBoundaries;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        Bounds ortographicCameraBounds = Camera.main.OrthographicBounds();

        float ortographicCameraBounds_X_Half = ortographicCameraBounds.size.x / 2;
        float ortographicCameraBounds_Y_Half = ortographicCameraBounds.size.y / 2;

        gameViewBoundaries = new Vector2(ortographicCameraBounds_X_Half, ortographicCameraBounds_Y_Half);

        CreatePickList();
    }

    public void PopulateGameScene()
    {
        // instantiate using a scattering formation
        foreach (int pick in pickList)
        {
            switch (pick)
            {
                case 0: // rock
                    InstantiateRockPrefab(FindNonOverlappingPosition());
                    break;
                case 1: // paper
                    InstantiatePaperPrefab(FindNonOverlappingPosition());
                    break;
                case 2: // scissors
                    InstantiateScissorsPrefab(FindNonOverlappingPosition());
                    break;
                default:
                    break;
            }
        }
    }

    private void CreatePickList()
    {
        pickList.Clear();

        AssignObjectCountsEach();

        PopulatePickListSorted();

        pickList.Shuffle();
    }
    private Vector3 FindNonOverlappingPosition()
    {
        Bounds ortographicCameraBounds = Camera.main.OrthographicBounds();

        float ortographicCameraBounds_X_Half = ortographicCameraBounds.size.x / 2;
        ortographicCameraBounds_X_Half -= 1; // margin from object center

        float ortographicCameraBounds_Y_Half = ortographicCameraBounds.size.y / 2;
        ortographicCameraBounds_Y_Half -= 1; // margin from object center

        float minDistance = 1f;

        Collider2D[] neighbours;
        Vector2 pos;

        do
        {
            pos = new Vector2(
                Random.Range(-1 * ortographicCameraBounds_X_Half, ortographicCameraBounds_X_Half),
                Random.Range(-1 * ortographicCameraBounds_Y_Half, ortographicCameraBounds_Y_Half));
            neighbours = Physics2D.OverlapCircleAll(pos, minDistance);
        } while (neighbours.Length > 0);

        return new Vector3(pos.x, pos.y, 0f);
    }

    // Generates uniformly distributed random numbers for each object that add up to the totalObjectCount.
    private void AssignObjectCountsEach()
    {
        List<int> fields = new List<int> { 0, 0, 0 };
        int sum = 0;
        for (int i = 0; i < fields.Count - 1; i++)
        {
            fields[i] = Random.Range(1, totalObjectCount);
            sum += fields[i];
        }
        int actualSum = sum * fields.Count / (fields.Count - 1);
        sum = 0;
        for (int i = 0; i < fields.Count - 1; i++)
        {
            fields[i] = fields[i] * totalObjectCount / actualSum;
            sum += fields[i];
        }
        fields[fields.Count - 1] = totalObjectCount - sum;

        fields.Shuffle();

        rockCount = fields[0];
        paperCount = fields[1];
        scissorsCount = fields[2];
    }

    private void PopulatePickListSorted()
    {
        for (int i = 0; i < rockCount; i++)
        {
            pickList.Add(0);
        }

        for (int i = 0; i < paperCount; i++)
        {
            pickList.Add(1);
        }

        for (int i = 0; i < scissorsCount; i++)
        {
            pickList.Add(2);
        }
    }

    public void InstantiateRockPrefab(Vector3 pos)
    {
        Instantiate(rockPrefab, pos, Quaternion.identity, sessionObjects.transform);
    }

    public void InstantiatePaperPrefab(Vector3 pos)
    {
        Instantiate(paperPrefab, pos, Quaternion.identity, sessionObjects.transform);
    }

    public void InstantiateScissorsPrefab(Vector3 pos)
    {
        Instantiate(scissorsPrefab, pos, Quaternion.identity, sessionObjects.transform);
    }

    public Vector2 GetGameViewBoundaries()
    {
        return gameViewBoundaries;
    }

    public void AddToRockCount(int amount)
    {
        rockCount += amount;
    }
    public void AddToPaperCount(int amount)
    {
        paperCount += amount;
    }
    public void AddToScissorsCount(int amount)
    {
        scissorsCount += amount;
    }
}
