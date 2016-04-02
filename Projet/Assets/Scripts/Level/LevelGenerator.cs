using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

// Ça c'est pour différencier les types de tuiles.
public enum tileType
{
    EMPTY,     // Rien
    START,     // Début de la piste
    END,       // Fin de la piste
    STRAIGHT,  // Ligne droite
    CORNER_E,  // Coin qui sort vers l'est
    CORNER_W,  // Coin qui sort vers l'ouest
    CORNER_EN, // Coin qui part de l'est et qui sort vers le nord
    CORNER_WN  // Coin qui part de l'ouest et qui sort vers le nord
}

public class LevelGenerator : MonoBehaviour
{
    #region Paramètres
    /* C'est toute la map, mais avant qu'elle soit rendue visible.
     * Le premier et la position Y, et le deuxième, la position X.
     */
    tileType[,] levelGrid;

    // C'est le GameObject de murs qui seront instanciés selon levelGrid.
    GameObject wall, wallFlip, ground, endLine, endBlock;

    // Ce sont les GameObjects des obstacles qui seront instanciés après les murs.
    GameObject gravel, ice, tree, jump;

    // Taille de chaque dimension de la grille
    public int gridSize = 50;

    // Taille de cahque dimension d'une tuile
    const int tileSize = 10;

    // Facteur d'échelle de toute la map
    public float scaleFactor = 10.0f;

    GameObject player, avalanche, skyBox;

    #endregion

    #region Méthodes non-MonoBehaviour

    void cleanLevelGrid()
    {
        int startingPosition = 0;
        int endingPosition = gridSize;
        bool foundOne = false;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (levelGrid[y, x] == tileType.EMPTY)
                    continue;
                else
                {
                    foundOne = true;
                    break;
                }
            }

            if (foundOne) break;

            startingPosition++;
        }

        foundOne = false;

        for (int x = gridSize-1; x >= 0; x--)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (levelGrid[y, x] == tileType.EMPTY)
                    continue;
                else
                {
                    foundOne = true;
                    break;
                }
            }

            if (foundOne) break;

            endingPosition--;
        }

        int newXSize = endingPosition - startingPosition;

        if (newXSize == gridSize) return;

        tileType[,] newGrid = new tileType[gridSize, newXSize];

        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < newXSize; x++)
                newGrid[y, x] = levelGrid[y, x + startingPosition];
        }

        levelGrid = newGrid;
    }

    // Pour placer les tuiles de façon aléatoire, fonction récursive.
    void setTiles(int y, int x)
    {
        if ((x >= gridSize || x < 0) || (y >= gridSize || y < 0))
            throw new System.IndexOutOfRangeException("On est allé trop loin dans setTiles.");

        if (y == 0)
        {
            levelGrid[y, x] = tileType.END;
            cleanLevelGrid();
            return;
        }

        while (true)
        {
            int randomPct = Random.Range(0, 100);

            if (randomPct >= 0 && randomPct < 84)
            {
                levelGrid[y, x] = tileType.STRAIGHT;
                setTiles(y - 1, x);
            }
            else if (randomPct >= 84 && randomPct < 92)
            {
                if (x < gridSize - 2)
                {
                    levelGrid[y, x] = tileType.CORNER_E;
                    levelGrid[y, x + 1] = tileType.CORNER_EN;
                    setTiles(y - 1, x + 1);
                }
                else
                    continue;
            }
            else if (randomPct >= 92 && randomPct < 100)
            {
                if (x >= 2)
                {
                    levelGrid[y, x] = tileType.CORNER_W;
                    levelGrid[y, x - 1] = tileType.CORNER_WN;
                    setTiles(y - 1, x - 1);
                }
                else
                    continue;
            }

            break;
        }
    }

    // Pour écrire la map dans un fichier texte.
    /*void writeFile()
    {
        if (File.Exists("testMap.txt")) File.Delete("testMap.txt");

        using (StreamWriter sw = new StreamWriter("testMap.txt"))
        {
            for (int y = 0; y < levelGrid.GetLength(0); y++)
            {
                string ligne = "";

                for (int x = 0; x < levelGrid.GetLength(1); x++)
                {
                    switch (levelGrid[y,x])
                    {
                        case tileType.EMPTY:
                            ligne += "   ";
                            break;
                        case tileType.START:
                            ligne += "|S|";
                            break;
                        case tileType.END:
                            ligne += "|E|";
                            break;
                        case tileType.STRAIGHT:
                            ligne += "| |";
                            break;
                        case tileType.CORNER_E:
                            ligne += "|‾‾";
                            break;
                        case tileType.CORNER_W:
                            ligne += "‾‾|";
                            break;
                        case tileType.CORNER_WN:
                            ligne += "|__";
                            break;
                        case tileType.CORNER_EN:
                            ligne += "__|";
                            break;
                    }
                }

                sw.WriteLine(ligne);
            }
        }
    }*/

    // Pour placer un wall à la bonne place
    void placeWall(bool vertical, bool flip, float posX, float posY, float posZ, ref GameObject root)
    {
        GameObject newWall = GameObject.Instantiate(flip ? wallFlip : wall) as GameObject;
        newWall.name = "Wall";
        newWall.transform.parent = root.transform;
        newWall.transform.localScale = new Vector3(newWall.transform.localScale.x * scaleFactor,
                                                   newWall.transform.localScale.y,
                                                   newWall.transform.localScale.z);

        newWall.transform.position = new Vector3(posX, posY, posZ);

        if (vertical)
        {
            float halfTileSize = ((float)tileSize / 2.0f) * scaleFactor;
            newWall.transform.Rotate(0, 90, 0);
            newWall.transform.Translate(-halfTileSize, 0, -halfTileSize);
        }
    }

    bool proximityCheck(Vector2 toCheck, List<Vector2> placedPositions)
    {
        foreach (Vector2 aVector in placedPositions)
        {
            if (Vector2.Distance(aVector, toCheck) < 10)
                return true;
        }

        return false;
    }

    // Pour placer des obstacles dans une zone... ou pas.
    void instantiateObstacles(float minX, float maxX, float minZ, float maxZ, float posY, ref GameObject parent)
    {
        int amntObstacles = Random.Range(0, 4);
        if (amntObstacles == 0) return;

        List<Vector2> placedPositions = new List<Vector2>();

        for (int i = 0; i < amntObstacles; i++)
        {
            Vector2 newPlace = new Vector2();
            do
            {
                newPlace.x = Random.Range(minX, maxX);
                newPlace.y = Random.Range(minZ, maxZ);
            }
            while (proximityCheck(newPlace, placedPositions));

            GameObject newObstacle;

            switch (Random.Range(0,4))
            {
                case 0:
                    newObstacle = GameObject.Instantiate(gravel) as GameObject;
                    break;
                case 1:
                    newObstacle = GameObject.Instantiate(ice) as GameObject;
                    break;
                case 2:
                    newObstacle = GameObject.Instantiate(tree) as GameObject;
                    break;
                case 3:
                    newObstacle = GameObject.Instantiate(jump) as GameObject;
                    break;
                default:
                    throw new System.Exception("newObstacle pls");
            }

            newObstacle.transform.position = new Vector3(newPlace.x,
                                                         newObstacle.name.Contains("Tree") ? 17.5f : posY,
                                                         newPlace.y);

            newObstacle.transform.localScale = newObstacle.transform.localScale * (scaleFactor/2.0f);

            newObstacle.transform.parent = parent.transform;
        }
    }

    // Pour instancier les murs selon la map générée
    void instantiateWalls()
    {
        float posX, posY = 18.0f, posZ;
        int numTile = -1;

        float extremeLeft = 0;
        float extremeRight = 0;

        GameObject root = new GameObject();
        root.name = "Map";

        for (int y = 0; y < levelGrid.GetLength(0); y++)
        {
            posZ = (gridSize - y) * tileSize * scaleFactor;
            for (int x = 0; x < levelGrid.GetLength(1); x++)
            {
                if (levelGrid[y, x] == tileType.EMPTY) continue;
                numTile++;

                posX = ((x * tileSize + ((float)tileSize / 2.0f)) * scaleFactor);
                GameObject tile = new GameObject();
                tile.name = "Tile #" + numTile + " (" + levelGrid[y, x].ToString() + ")";
                tile.transform.parent = root.transform;

                switch (levelGrid[y,x])
                {
                    case tileType.START:
                        placeWall(true, false, posX, posY, posZ, ref tile);
                        placeWall(true, true, posX + tileSize * scaleFactor, posY, posZ, ref tile);
                        player.transform.position = new Vector3(posX,
                                                                player.transform.position.y,
                                                                player.transform.position.z);
                        break;
                    case tileType.END:
                        placeWall(true, false, posX, posY, posZ, ref tile);
                        placeWall(true, true, posX + tileSize * scaleFactor, posY, posZ, ref tile);

                        GameObject newEndLine = GameObject.Instantiate(endLine) as GameObject;
                        newEndLine.name = "EndLine";
                        newEndLine.transform.parent = tile.transform;
                        newEndLine.transform.position = new Vector3(posX, 3.0f, posZ + tileSize * scaleFactor);
                        newEndLine.transform.localScale *= scaleFactor;

                        GameObject newEndBlock = GameObject.Instantiate(endBlock) as GameObject;
                        newEndBlock.name = "END BLOCK";
                        newEndBlock.transform.parent = root.transform;
                        newEndBlock.transform.position = new Vector3(posX, 8.0f, posZ + (2 * tileSize * scaleFactor));
                        newEndBlock.transform.localScale *= scaleFactor;
                        break;
                    case tileType.STRAIGHT:
                        placeWall(true, false, posX, posY, posZ, ref tile);
                        placeWall(true, true, posX + tileSize * scaleFactor, posY, posZ, ref tile);
                        break;
                    case tileType.CORNER_E:
                        placeWall(false, false, posX, posY, posZ + tileSize * scaleFactor, ref tile);
                        placeWall(true, false, posX, posY, posZ, ref tile);
                        break;
                    case tileType.CORNER_W:
                        placeWall(false, true, posX, posY, posZ + tileSize * scaleFactor, ref tile);
                        placeWall(true, true, posX + tileSize * scaleFactor, posY, posZ, ref tile);
                        break;
                    case tileType.CORNER_EN:
                        placeWall(false, false, posX, posY, posZ, ref tile);
                        placeWall(true, true, posX + tileSize * scaleFactor, posY, posZ, ref tile);
                        break;
                    case tileType.CORNER_WN:
                        placeWall(false, true, posX, posY, posZ, ref tile);
                        placeWall(true, false, posX, posY, posZ, ref tile);
                        break;
                }

                if (levelGrid[y, x] != tileType.EMPTY)
                {
                    if (posX > extremeRight || extremeRight == 0)
                        extremeRight = posX;

                    if (posX < extremeLeft || extremeLeft == 0)
                        extremeLeft = posX;

                    GameObject tileGround = GameObject.Instantiate(ground) as GameObject;
                    tileGround.name = "Ground";
                    tileGround.transform.parent = tile.transform;
                    tileGround.transform.position = new Vector3(posX, -1f, posZ);
                    tileGround.transform.localScale *= 2 * scaleFactor;
                }

                if (levelGrid[y,x] != tileType.START && levelGrid[y,x] != tileType.END)
                    instantiateObstacles(posX + scaleFactor - (((float)tileSize / 2.0f) * scaleFactor),
                                         posX + (10 * scaleFactor) - scaleFactor - (((float)tileSize / 2.0f) * scaleFactor),
                                         posZ + scaleFactor, posZ + (10 * scaleFactor) - scaleFactor,
                                         0f,
                                         ref tile);

                
            }
        }

        avalanche.transform.position = new Vector3((extremeLeft + extremeRight) / 2,
                                                   avalanche.transform.position.y,
                                                   avalanche.transform.position.z);
    }

    #endregion

    #region Méthodes MonoBehaviour

    void Awake () {
        // Initialisation des paramètres
        levelGrid = new tileType[gridSize, gridSize];
        wall = Resources.Load<GameObject>("Prefabs/Level/Mur");
        wallFlip = Resources.Load<GameObject>("Prefabs/Level/MurDroit");
        endLine = Resources.Load<GameObject>("Prefabs/EndLine");
        ground = Resources.Load<GameObject>("Prefabs/Sol");
        gravel = Resources.Load<GameObject>("Prefabs/Obstacles/Gravier");
        ice = Resources.Load<GameObject>("Prefabs/Obstacles/Ice");
        tree = Resources.Load<GameObject>("Prefabs/Obstacles/Tree");
        jump = Resources.Load<GameObject>("Prefabs/Obstacles/Jump");
        player = GameObject.FindGameObjectWithTag("Player");
        avalanche = GameObject.FindGameObjectWithTag("Avalanche");
        skyBox = Resources.Load<GameObject>("Prefabs/skybox");
        endBlock = Resources.Load<GameObject>("Prefabs/Level/End");

        // On met une tuile de départ
        levelGrid[gridSize - 1, gridSize/2 - 1] = tileType.START;

        // On met les autres tuiles
        setTiles(gridSize - 2, gridSize / 2 - 1);

        // Avanlanche
        avalanche.transform.localScale = new Vector3(avalanche.transform.localScale.x*levelGrid.GetLength(1),
                                                     avalanche.transform.localScale.y,
                                                     avalanche.transform.localScale.z);

        // Pour tester ça, j'avais fait écrire la map dans un fichier texte.
        //writeFile();

        // Mais là on ne veut pas créer de fichiers texte, on veut instantier des murs!
        instantiateWalls();
    }

    #endregion
}