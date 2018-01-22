using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GridPosition
{
    public int row { get; set; }
    public int col { get; set; }

    public GridPosition(int r, int c)
    {
        row = r;
        col = c;
    }
}

public class ShipAttack : MonoBehaviour {
    public bool isSelected = false;
    public GridPosition gridPosition;
}
