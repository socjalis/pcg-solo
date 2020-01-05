using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Algorithm
{
    abstract public SortedDictionary<string, string> getParameters();
    abstract public void setParameters(SortedDictionary<string, string> parameters);
    abstract public int[,] generateMap();
}
    
