using UnityEngine;

public class MazeConstructor : MonoBehaviour
{
    /// <summary>
    /// 1 These fields are all available in the inspector.
    /// showDebug will toggle debug displays, while the various Material references are materials for generated 
    /// models. The SerializeField attribute displays a field in the Inspector even though private.
    /// </summary>
    public bool showDebug = true;
    
    [SerializeField] private Material mazeMat1;
    [SerializeField] private Material mazeMat2;
    [SerializeField] private Material startMat;
    [SerializeField] private Material treasureMat;

    private MazeDataGenerator dataGenerator;
    private MazeMeshGenerator meshGenerator;

    //2 Read only outside this class
    // Maze data can't be modified from outside of this class
    // 1 represents open or blocked for every space
    public int[,] data
    {
        get; private set;
    }

    //3
    // Inititlizes data with a 3x3 array of 1s surrounding a zero 
    // 1 means wall while 0 means empty
    void Awake()
    {
        dataGenerator = new MazeDataGenerator();
        meshGenerator = new MazeMeshGenerator();

        // default to walls surrounding a single empty cell
        data = new int[,]
        {
            {1, 1, 1},
            {1, 0, 1},
            {1, 1, 1}
        };
    }
    
    public float hallWidth
    {
        get; private set;
    }
    public float hallHeight
    {
        get; private set;
    }

    public int startRow
    {
        get; private set;
    }
    public int startCol
    {
        get; private set;
    }

    public int goalRow
    {
        get; private set;
    }
    public int goalCol
    {
        get; private set;
    }

    public void GenerateNewMaze(int sizeRows, int sizeCols, TriggerEventHandler startCallback=null, TriggerEventHandler goalCallback=null)
    {
        if (sizeRows % 2 == 0 && sizeCols % 2 == 0)
        {
            Debug.LogError("Odd numbers work better for dungeon size.");
        }

        DisposeOldMaze();

        data = dataGenerator.FromDimensions(sizeRows, sizeCols);

        FindStartPosition();
        FindGoalPosition();

        // store values used to generate this mesh
        hallWidth = meshGenerator.width;
        hallHeight = meshGenerator.height;

        DisplayMaze();

        PlaceStartTrigger(startCallback);
        PlaceGoalTrigger(goalCallback);
    }


    public void DisposeOldMaze()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Generated");
        foreach (GameObject go in objects)
        {
            Destroy(go);
        }
    }

    /// <summary>
    /// This code starts at 0,0 and iterates through the maze
    /// until it finds an open space. These coordinates are stored as the maze's start position.
    /// </summary>
    private void FindStartPosition()
    {
        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        for (int i = 0; i <= rMax; i++)
        {
            for (int j = 0; j <= cMax; j++)
            {
                if (maze[i, j] == 0)
                {
                    startRow = i;
                    startCol = j;
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Starts with max values to find an open space. Will be used to find the 
    /// maze exit 
    /// </summary>
    private void FindGoalPosition()
    {
        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        // loop top to bottom, right to left
        for (int i = rMax; i >= 0; i--)
        {
            for (int j = cMax; j >= 0; j--)
            {
                if (maze[i, j] == 0)
                {
                    goalRow = i;
                    goalCol = j;
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Place the start trigger
    /// </summary>
    /// <param name="callback"></param>
    private void PlaceStartTrigger(TriggerEventHandler callback)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = new Vector3(startCol * hallWidth, .5f, startRow * hallWidth);
        go.name = "Start Trigger";
        go.tag = "Generated";

        go.GetComponent<BoxCollider>().isTrigger = true;
        go.GetComponent<MeshRenderer>().sharedMaterial = startMat;

        TriggerEventRouter tc = go.AddComponent<TriggerEventRouter>();
        tc.callback = callback;
    }

    /// <summary>
    /// Places goal position
    /// </summary>
    /// <param name="callback"></param>
    private void PlaceGoalTrigger(TriggerEventHandler callback)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = new Vector3(goalCol * hallWidth, .5f, goalRow * hallWidth);
        go.name = "Treasure";
        go.tag = "Generated";

        go.GetComponent<BoxCollider>().isTrigger = true;
        go.GetComponent<MeshRenderer>().sharedMaterial = treasureMat;

        TriggerEventRouter tc = go.AddComponent<TriggerEventRouter>();
        tc.callback = callback;
    }

    private void DisplayMaze()
    {
        GameObject go = new GameObject();
        go.transform.position = Vector3.zero;
        go.name = "Procedural Maze";
        go.tag = "Generated";

        MeshFilter mf = go.AddComponent<MeshFilter>();
        mf.mesh = meshGenerator.FromData(data);
        
        MeshCollider mc = go.AddComponent<MeshCollider>();
        mc.sharedMesh = mf.mesh;

        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        mr.materials = new Material[2] {mazeMat1, mazeMat2};
    }

    /// <summary>
    /// Displays the maze data
    /// </summary>
    void OnGUI()
    {
        //1 checks if debug displays are enabled
        
        if (!showDebug)
        {
            return;
        }

        //2 initialize some local variables
        int[,] maze = data; // local copy of the maze
        int rMax = maze.GetUpperBound(0); // maximum rows and columns
        int cMax = maze.GetUpperBound(1); 

        string msg = ""; // a string to buildup

        //3 Iterates over the rows and columns of the 2d array.
        // The code checks the stored value and appends eitehr .... or == depening
        // on if the value is zero or not
        for (int i = rMax; i >= 0; i--)
        {
            for (int j = 0; j <= cMax; j++)
            {
                if (maze[i, j] == 0)
                {
                    msg += "....";
                }
                else
                {
                    msg += "==";
                }
            }
            msg += "\n";
        }

        //4 prints out the built-up string
        GUI.Label(new Rect(20, 20, 500, 500), msg);
    }

}
