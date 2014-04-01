using UnityEngine;
using System.Collections;


public class FloatingDamageText : MonoBehaviour {


    public Color textColor = new Color(0.8f,0.0f,0.0f,1.0f); //red
    public float scrollSpeed = 0.05f; 
    public float duration = 1.5f;
    public Transform parent;

    private float alpha;
	// Use this for initialization
	void Start () 
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
            Vector3 pos = transform.position;
            pos.y += scrollSpeed * Time.deltaTime;
            transform.position = pos;
            alpha -= Time.deltaTime/duration;
            textColor.a = alpha;
            guiText.material.color = textColor;
        }
        else
        {
            if(parent)
            {
                gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
            }
        }
	}

    public void startText(string text)
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(parent.position);
        alpha = 1;
        textColor.a = 1;
        guiText.material.color = textColor;
        guiText.text = text;
        transform.position = pos;
        gameObject.SetActive(true);
    }
}
