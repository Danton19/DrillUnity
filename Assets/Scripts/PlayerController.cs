﻿using UnityEngine;
using System.Collections;

namespace Drill
{

	public class PlayerController : MonoBehaviour
	{
		public float speed;
		public int life = 100;
		[HideInInspector]public int score;
        private Animator playerAnimator;
		[HideInInspector]public bool isAlive = true;
		[HideInInspector]public bool levelWin = false;
		private Light stateLight;
		private Color newColor;
		private Color redLight= new Color(255, 0, 0);
		private Color greenLight= new Color(0, 255, 0);
		private Vector2 fingerPos;
		private Animator blockAnimator;
		private bool canMove = true;

        // SOUND
        private AudioSource audioSourcePickupCoin;
        private AudioSource backgroundMusic;
        private AudioSource audioSourceSmallExplosion;
        private AudioSource audioSourceBigExplosion;

        void Awake()
		{
			playerAnimator = GetComponent<Animator> ();
			stateLight = GameObject.Find ("StateLight").GetComponent<Light>();
			//Color.TryParseHexString (greenLight, out newColor);
			stateLight.color = greenLight;

            // SOUND
            audioSourcePickupCoin = GameObject.Find("SoundPickupDiamond").GetComponent<AudioSource>();
            backgroundMusic = GameObject.Find("BackgroundMusic").GetComponent<AudioSource>();
            audioSourceSmallExplosion = GameObject.Find("SoundSmallExplosion").GetComponent<AudioSource>();
            audioSourceBigExplosion = GameObject.Find("SoundBigExplosion").GetComponent<AudioSource>();
        }
        void OnCollisionEnter2D (Collision2D col)
		{
			if(col.gameObject.tag == "Block")
			{
				col.collider.enabled = false;
				blockAnimator = col.gameObject.GetComponent<Animator>();
				blockAnimator.SetBool ("HitDrill", true);
				//Destroy(col.gameObject);
				HitDamage();
				if(life<=40)
				{
					//Color.TryParseHexString (redLight, out newColor);
					stateLight.color = redLight;
				}

                audioSourceSmallExplosion.Play();
            }
            else if (col.gameObject.tag == "Diamonds")
            {
                Destroy(col.gameObject);
                score += 50;

                audioSourcePickupCoin.Play();
            }
			//CheckIfGameOver();
		}

		public void HitDamage()
		{
			life -= 20;
		}

		void OnTriggerEnter2D (Collider2D other)
		{
			if (other.tag == "NextLevel") 
			{
				Debug.Log("NextLevel");
				levelWin=true;
			}
		}

		private void KillMovement()
		{
			stateLight.intensity = 0f;
			stateLight.enabled = false;
			canMove = false;
			GetComponent<Collider2D> ().enabled = false;

            // SOUND
            audioSourceBigExplosion.Play();
            backgroundMusic.Stop();
		}
		private void DrillGameOver()
		{
			isAlive = false;
			enabled = false;
			Destroy (gameObject);
		}

		private void DrillMovement(float Xdirection, float ySpeed)
		{
			//float moveHorizontal = Input.GetAxis ("Horizontal");
			Vector2 movement = new Vector2 (Xdirection, ySpeed);
			GetComponent<Rigidbody2D>().velocity = movement * speed;
			DrillRotation (Xdirection);
			
			//touch movement
			if (Input.touchCount > 0) 
			{
				fingerPos= Input.GetTouch(0).position;
				if(fingerPos.x < Screen.width/2)
					movement = new Vector2(-1,-1);
				else
					movement = new Vector2(1,-1);
				
				GetComponent<Rigidbody2D> ().velocity = movement * speed;
				DrillRotation (movement.x);
			}
		}

		void DrillRotation(float movement)
		{
			transform.rotation = Quaternion.Euler (0,0,25*movement);
		}

		void Update ()
		{
			playerAnimator.SetFloat ("Life", life);
            
			if (canMove)
				DrillMovement (Input.GetAxis ("Horizontal"),-1f);
			else {
				float finalSpeed = Mathf.SmoothStep (-1f, 0f, 0.8f);
				DrillMovement(0f,finalSpeed);
			}

			//level boundaries
			GetComponent<Rigidbody2D> ().position = new Vector2 
				(
					Mathf.Clamp (GetComponent<Rigidbody2D> ().position.x, 0f, 5f), 
					GetComponent<Rigidbody2D> ().position.y
				);

			//LightPong (red light when damaged)
			if (life <= 40)
				stateLight.intensity = Mathf.PingPong (Time.time * 0.5f, 0.3f);

		}

	}
}