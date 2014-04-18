using UnityEngine;
using System.Collections;


public class FloatingDamageText : MonoBehaviour {


    public Color textColor = new Color(0.8f,0.0f,0.0f,1.0f); //red
    public float startVelocity = 3.0f; 
    public float duration = 1.5f;
    public Transform parent;
    private bool started;
    private float damage;

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
            //Vector3 pos = transform.position;
            //pos.y += scrollSpeed * Time.deltaTime;
            //transform.position = pos;
            alpha -= Time.deltaTime/duration;
            textColor.a = alpha;
            guiText.material.color = textColor;
        }
        else
        {
            started = false;
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

    public void startDamage(float newDamage)
    {
        if (!started)
        {
            Vector3 pos = Camera.main.WorldToViewportPoint(parent.position);
            alpha = 1;
            textColor.a = 1;
            guiText.material.color = textColor;
            damage = 0;
            transform.position = pos;
            gameObject.SetActive(true);
            transform.rigidbody2D.AddForce(Vector3.up * startVelocity + Vector3.right * Random.Range(-0.5f,0.5f) * startVelocity);
            started = true;
        }
        damage += newDamage;
        guiText.text = damage.ToString();
    }
}
