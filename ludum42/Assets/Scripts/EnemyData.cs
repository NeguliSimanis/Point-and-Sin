using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData
{
    public static EnemyData current;

    int lastEnemyID = 0;

    public int GetEnemyID()
    {
        lastEnemyID++;
        return lastEnemyID;
    }
}
