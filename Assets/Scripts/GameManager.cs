﻿using UnityEngine;
using System.Collections;

namespace Drill
{
	using System.Collections.Generic;       //Allows us to use Lists. 
	using UnityEngine.UI;

	public class GameManager : MonoBehaviour
	{
		
		public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
		private BoardManager boardScript;                       //Store a reference to our BoardManager which will set up the level.
		//level and score, constant trough reset
		private int level = 1;
		private int score = 0;
		private bool isGameOver = false;
		//player
		private GameObject playerRef;
		private PlayerController playerController;
		private int prePlayerLife;
		//canvas
		public GameObject canvas;
		private GameObject levelImage;
		private GameObject faderScreen;
		private Image redSplash;
		private Color redSplashColor;
		private Text playerLifeText;
		private Text playerScoreText;
        private Text levelNumber;
		private GameObject allBlocks;
		private GameObject mainCamera;

		//Awake is always called before any Start functions
		void Awake()
		{
			//Check if instance already exists
			if (instance == null)
				instance = this;
			
			//If instance already exists and it's not this:
			else if (instance != this)
				Destroy(gameObject);    

			DontDestroyOnLoad(gameObject);
			boardScript = GetComponent<BoardManager>();

			//IMPORTANT! This is just for development, on production delete!
			InitGame();
		}
		//To restart or level change
		void OnLevelWasLoaded()
		{
			//Call InitGame to initialize our level.
			InitGame ();
		}
		
		//Initializes the game for each level.
		public void InitGame()
		{
			Instantiate (canvas);
			mainCamera = GameObject.Find ("Main Camera 2");
			//set Level number
			levelNumber = GameObject.Find ("LevelNumber").GetComponent<Text> ();
			levelNumber.text = "Level "+ level;
			//player reference and lifeText setup
			playerRef = GameObject.Find ("Player");
			playerController = playerRef.GetComponent<PlayerController>();
			playerController.score = score;
			prePlayerLife = playerController.life;
			levelImage = GameObject.Find ("LevelImage");
			faderScreen = GameObject.Find ("FaderScreen");
			Color.TryParseHexString ("#870000AA", out redSplashColor);
			redSplash = faderScreen.GetComponent<Image> ();//#870000AA
			redSplash.color = redSplashColor;
			levelImage.SetActive (false);
			faderScreen.SetActive (false);
			playerLifeText = GameObject.Find ("LifePlayer").GetComponent<Text>();
			playerScoreText = GameObject.Find ("ScorePlayer").GetComponent<Text>();
			//Call the SetupScene function of the BoardManager script, pass it current level number.
			boardScript.SetupScene(level);
			allBlocks = GameObject.Find ("Blocks");
			isGameOver=false;

            InitLights();
        }

		public void Restart()
		{
			Application.LoadLevel (Application.loadedLevel);
		}

		public void GoMenu()
		{
			Application.LoadLevel ("MainMenu");
			Destroy (instance);
		}

		private void CleanBlocks()
		{
			Destroy (allBlocks);
		}
		public void GameOver()
		{
			levelImage.SetActive(true);
			isGameOver = true;
		}

        public void InitLights()
        {
            var mainLight = GameObject.Find("MainLight").GetComponent<Light>();
            mainLight.intensity -= 0.5f * level; // The camera gets darker, the deeper de drill goes

            var spotLight = GameObject.Find("Spotlight").GetComponent<Light>();
            spotLight.intensity += 0.4f * level; // Spotlight gets brighter, the deeper de drill goes
			//max spotlight intensity 2f
			if (spotLight.intensity > 2f)
				spotLight.intensity = 2f;
        }

		//RedScreen for Players Hits
		void HitWarning()
		{
			faderScreen.SetActive (true);
			redSplash.color = Color.Lerp(redSplash.color, Color.clear, 5f * Time.deltaTime);
			Debug.Log ("Transition:" + 4f * Time.deltaTime);
			if (redSplash.color.a <= 0.05f) 
			{
				prePlayerLife = playerController.life;
				redSplash.color = redSplashColor;
				faderScreen.SetActive(false);
			}
		}

        void Update()
		{
			playerLifeText.text = "Life " + playerController.life;
			playerScoreText.text = "Score " + playerController.score;

			if (playerController.life <= 0)
				CleanBlocks ();

            if (!isGameOver && !playerController.isAlive)
				Invoke ("GameOver", 1.5f);
			if (playerController.life != prePlayerLife)
				HitWarning ();
			//GameOver ();
			if (isGameOver && Input.GetKey (KeyCode.Backspace)) 
			{
				Invoke("Restart",1f);
			}

			if (playerController.levelWin) 
			{
				playerController.levelWin = false;
				mainCamera.GetComponent<SmoothCamera2D>().enabled = false;
				level++;
				score = playerController.score;
				Invoke("Restart",2f);
			}
		}
	}
}