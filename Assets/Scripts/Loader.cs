﻿using UnityEngine;
using System.Collections;

namespace Drill
{   
	public class Loader : MonoBehaviour 
	{
		public GameObject gameManager;          //GameManager prefab to instantiate.

		public void Awake ()
		{
			if (GameManager.instance == null)
				Instantiate(gameManager);
		}
	}
}