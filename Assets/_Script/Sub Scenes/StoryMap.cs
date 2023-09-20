using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryMap : MonoBehaviour
{
    private static string[] _book1 = new string[10]
    {
        "Prologue",
        // Story begins with the MC, a soldier in Europe, getting ready for a new baby. Flees w/ baby after wife and baby found to be witches due to hair color by the govt.
        "Chapter 1 - A New Beginning", 
        // Timeskip. MC finds new life in the new world. Living out the daily life activities while hiding son in home for fear of witchhunt. Ends with impending trouble when returning from hunt.
        "Chapter 2 - A Journey Beyond the Heart",
        // Finds that son was found by villagers and was reported to higher ups. Confronted by soldiers in the forest. Runs back and reunites with commander who killed wife during escape.
        // Loses fight and on verge of death, thrown into the deep cold area that was corrupted bc its treated as disposal area and nothing escapes.
        "Chapter 3 - A Father's Path",
        // In fear of losing son, MC tries to escape area, but fails. Discovers the area is a living construct and tries to solve its puzzle to escape.
        "Chapter 4 - Finding Resolve",
        // Arrives at the center of the area to find a large crystal and a little girl trapped inside. MC tries to get the girl out, but must get through ice soldiers.
        // Awakens dormant powers (weak fire for now) and burns through crystal. Warmth from fire slowly awakens girl and crystal shatters. MC gains new resolve as adoptive Father and they leave.
        "Chapter 5 - Traversing Unpaved Roads",
        // MC and girl discuss their plans on what to do. Unable to go east back to village, so they go west, despite dangers with the wild and potential fights with natives. Travel around an/
        // forage, and then ends with MC and girl confronted by battle with enraged natives. MC and girl win.
        "Chapter 6 - Fallen Youth",
        // Natives accept their fate for losing, but MC and girl spares them since they're looking for refuge. After finding out they have a common enemy, they go to native village to discuss 
        // their current predicament. MC finds out the village chief son was taken after chief was killed, and natives are trying to get the son back, tho they don't know if still alive.
        // Shortly, the invaders come and demand surrender, but natives refuse. Invaders use a super weapon and shoots it at them, causing a large area to manifest and corrupt the area.
        // Wild life gets crazy and the area becomes a super forest. 
        "Chapter 7 - WildLyfe",
        // You find yourself in the corrupted area. You then talk to the natives and player learns more about these corruption areas. 
        "Chapter 8 - Living Amongst Ourself",
        "Chapter 9 - Finding Bonds Past Resolution"
    };
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UnloadScene(string scene) => SceneManager.UnloadSceneAsync(scene);
}
