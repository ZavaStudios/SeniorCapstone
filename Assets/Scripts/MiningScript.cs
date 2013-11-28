using UnityEngine;
using System.Collections;

public class MiningScript : MonoBehaviour
{
		public int _damage = 1;
		public float _distance;
		public int _oreQuantity = 0;

		void Update ()
		{
				if (Input.GetButtonDown ("Fire1"))
				{
						RaycastHit hit;
	
						if (Physics.Raycast (transform.position, transform.TransformDirection (Vector3.forward), out hit))
						{
								_distance = hit.distance;

								if (/*_distance <= 1 && */ hit.transform.CompareTag("Mineable"));
								{
										hit.transform.SendMessage ("MineAction", _damage, SendMessageOptions.DontRequireReceiver);
								}
						}
				}
		}

}
