/* Couple methods for data persisting between scenes.
 * https://gamedev.stackexchange.com/questions/110958/unity-5-what-is-the-proper-way-to-handle-data-between-scenes
 * -Static script (current solution)
 * -Don't destroy on load + singelton
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PersistentData
{
    public static int phase1_winner { get; set; }
}
