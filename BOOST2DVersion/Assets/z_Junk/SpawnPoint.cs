// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour {
// This script must be attached to a game object to tell Unity where the player starts in the level.

// We'll draw a gizmo in the scene view, so it can be found....
void  OnDrawGizmos (){
	Gizmos.DrawIcon(transform.position, "Player Icon.tif");
}


}