using System;
using UnityEngine;

namespace Utilities
{
    public static class DebugDisplay
    {
        // Display player position on grid map
        public static void DisplayGrid(ScriptableGrid grid)
        {
            for (int j = 0; j < grid.Height; j++)
            {
                string t = String.Empty;
                for (int i = 0; i < grid.Width; i++)
                {
                    t += grid.State.Grid[j, i].ToString();
                }
        
                Debug.Log(t);
            }
        }
    }
}