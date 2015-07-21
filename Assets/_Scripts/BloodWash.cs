using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BloodWash : MonoBehaviour 
{
    private Image picture;

    private int counter = 0;
    private int destroyFlag = 0;

    public float fadeAway1float;
    public float fadeAway2float;
    private Color fadeAway1;
    private Color fadeAway2;
    

    public GameObject samurai;
    private SamuraiInput samuraiInput;

    void Start()
    {
        fadeAway1 = new Color(1, 1, 1, fadeAway1float);
        fadeAway2 = new Color(1, 1, 1, fadeAway2float);
    }
    void Awake()
    {
        picture = GetComponent<Image>();
        samuraiInput = samurai.GetComponent<SamuraiInput>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V) && samuraiInput.inputEnable == true)
        {
            counter++;

            if (counter == 1)
            {
                picture.color = fadeAway1;
                destroyFlag = 0;
            }

            else if (counter == 2)
            {
                picture.color = fadeAway2;
                destroyFlag = 0;
            }

            else
            {
                destroyFlag = 1;
            }
        }
    }

    void FixedUpdate()
    {
        if (destroyFlag == 1)
            gameObject.SetActive(false);
    
    }
}
