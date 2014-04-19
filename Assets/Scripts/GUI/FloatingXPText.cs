using UnityEngine;
using System.Collections;


public class FloatingXPText : MonoBehaviour {


    public Color textColor = new Color(0.0f,0.8f,0.2f,1.0f); //green
    public float startVelocity = 3.0f; 
    public float duration = 1.5f;
    public Transform parent2;
    private bool started;

    private float alpha;
	// Use this for initialization
	public void Start () 
    {
	    guiText.material.color = textColor;
        guiText.fontSize = 24;
        guiText.fontStyle = FontStyle.Bold;
        alpha = 1;           
        gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () 
    {
	    if(alpha > 0)
        {
            alpha -= Time.deltaTime/duration;
            textColor.a = alpha;
            guiText.material.color = textColor;
            print(transform.position);
        }
        else
        {
            started = false;
            if(parent2)
            {
                gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
            }
        }
	}

    public void displayText(string textToDisplay)
    {

        if (!started)
        {
            print("displaying" + textToDisplay);
            Vector3 pos = Camera.main.WorldToViewportPoint(parent2.position);
            alpha = 1;
            textColor.a = 1;
            guiText.material.color = textColor;
            transform.position = pos;
            gameObject.SetActive(true);
            transform.rigidbody2D.AddForce(Vector3.up * startVelocity + Vector3.right * Random.Range(-0.5f, 0.5f) * startVelocity);
            started = true;
        }
        guiText.text = textToDisplay;
    }
   
}
