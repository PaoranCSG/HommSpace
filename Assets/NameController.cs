using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameController : MonoBehaviour
{
    public static NameController instance;
    public List<string> characterNames = new List<string>();
    public List<string> usedCharacterNames = new List<string>();
    public List<string> planetNames = new List<string>();
    public List<string> spaceObjectNames = new List<string>();
    public List<Sprite> characterSprites = new List<Sprite>();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(this);
        }
    }

}
