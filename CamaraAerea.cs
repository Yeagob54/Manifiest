	/* **********************************************************
	 * CamaraAerea.cs
	 * 
	 * Script de control de la camara principal, de manejo aereo 
	 * y control de los limites de la pantalla
	 * **********************************************************/

	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	public class CamaraAerea : MonoBehaviour {

		//Posicion inicial
		public float alturaCamara = 10.0f;
		public float anguloCamara = 10.0f;

		//configuracion inicial de la velocidad de movimiento de camara
		public float velocidadMovimiento = 3.0f;
		public float velocidadMovimientoMaxima = 50f;
		private float velocidadMovimientoActual;

		//Determina si la camara se esta moviendo
		private bool camaraMoviendose = false;

		//Switch control del movimiento de camara on/off
		private bool move = true;

		//Controles para tablet
		private bool versionTablet = false;

		//Relacion de incremento del zoom In/Out 
		public float zoomRate = 0.4f;
		private float zoom = 0f;

		//Posicion inicial de la camara en la escena
		public GameObject puntoInicial;

		//Limites de desplazamiento y del miniMapa.
		public float[] limitesMapa = new float[4];

		//Material shader, para las lineas de movimiento
		private Material mat;
		
		//Instancia de la clase
		public static CamaraAerea temp;
		
		/*****************
		 *  INICIALIZAMOS
		 * ***************/
		void Start () 
		{
			//Instanciamos la clase
			temp = this;

			velocidadMovimientoActual = velocidadMovimiento;

			//Si se definio un punto inicial, asigmanos la posicion de la camara alli. 
			if (puntoInicial != null) transform.position = new Vector3(puntoInicial.transform.position.x, alturaCamara, puntoInicial.transform.position.z-anguloCamara);		

			//Para las lineas de movimiento
			createShader ();

			//Comprobamos si estamos en version para tablet
			versionTablet = Manager.temp.versionTablet;
		}
			
		/************
		 *  UPDATE
		 * **********/
		void Update () 
		{		
			//Movimiento de la camara
			camaraMoviendose = false;

			//Con esta linea controlamos la rotacion de camara con el boton derecho. No tablets.
			//Desactivado porque pebaga un salto raro en el primer click_der
			//if (!versionTablet)
				//GetComponent<MouseLook>().enabled = Input.GetMouseButton(1);

			//Controlamos si la camara se mueve por el mouse en margen lateral o si se esta haciendo zoom
			//El control de la camara con WASD, es mediante un FirstPersonControler
			MovimientoCamara();
		
		}
			  
		/************************************************
		 *  DIBUJAMOS LAS LINEAS DE DEPLAZAMIENTO Y ATAQUE
		 * ************************************************/
		void OnPostRender()
		{
			if (selectedManager.temp.objects.Count > 0)
			//Dubuja una linea con el objetivo a interactuar o al destino
			foreach (GameObject g in selectedManager.temp.objects)
			{
				bool existe, atacando = false, moviendose = false, fichado = false;
				
				try {
					//Consultamos por los 3 supuestos en los que pintaremnos una linea
					atacando = g.GetComponent<UnitManager>().estaAtacando;
					moviendose = g.GetComponent<ComportamientoHumano>().moviendose;
					fichado = g.GetComponent<UnitManager>().estaFichado;
					existe = true;
					}
				//Objetos eliminados mientras se realizaba este proceso
				catch{existe = false;}

				if (existe && (atacando || moviendose || fichado)) {
					//Definimos el punto inicial de la linea
					Vector3 startPos;
					startPos = this.camera.WorldToScreenPoint (g.transform.position);
					startPos.z = 0;
					Vector3 endPos;
					//Preparamos para pintar pixels en pantalla
					GL.PushMatrix ();			
					GL.LoadPixelMatrix();
					mat.SetPass (0);				
					GL.Begin(GL.LINES);
					//Si esta atacando, dibujamos una linea roja entre el manifestante y el objetivo
					if (atacando)	{
						GL.Color (Color.red);
						try	{
							endPos = this.camera.WorldToScreenPoint (g.GetComponent<UnitManager>().objetivoInteractuar.transform.position);
							endPos.z = 0;
						}
						catch {
							endPos = startPos;
						}
					}
					//Si se esta moviendo, dibujamos una linea azul entre el manifestante y el destino
					else if (moviendose)	{
						GL.Color (Color.blue);					
						endPos = this.camera.WorldToScreenPoint (g.GetComponent<ComportamientoHumano>().destinoTemp.position);					
						endPos.z = 0;					
					}
					//si no esta atacando o moviense, es que esta fichado y le ponemos una marca verde
					else {
						GL.Color (Color.green);					
						endPos = startPos;
						endPos.z = 0;					

					}

					//Creamos la linea
					GL.Vertex (startPos);GL.Vertex (endPos);
					
					GL.End ();			
					
					//Dibujamos un cuadradito al final de la linea de 3x3 pixels
					GL.Begin(GL.QUADS);
					GL.Vertex3 (endPos.x-3, endPos.y-3, 0);
					GL.Vertex3 (endPos.x-3, endPos.y+3, 0);
					GL.Vertex3 (endPos.x+3, endPos.y+3, 0);
					GL.Vertex3 (endPos.x+3, endPos.y-3, 0);
					GL.End ();
					GL.PopMatrix ();
				}			
			}
		}

		/***********************************
	  *  CREACION SHADER PARA LINEAS
	  * *********************************/
		private void createShader()
		{
			string shaderText = 
			"Shader \"Lines/Colored Blended\" {" +
	            "SubShader { Pass { " +
	            "    Blend SrcAlpha OneMinusSrcAlpha " +
	            "    ZWrite Off Cull Off Fog { Mode Off } " +
	            "    BindChannels {" +
	            "      Bind \"vertex\", vertex Bind \"color\", color }" +
	            "} } }" ;
			
			mat = new Material(shaderText);
		}

		/*********************************
		 *  CONTROL MOVIMIENTO DE CAMARA
		 * *******************************/
		private void MovimientoCamara() {
			
			Transform camara = Manager.temp.controlCamara.transform;

			//Control de movimiento de camara por mouse.
			if (!versionTablet) {
				if (Input.mousePosition.x < 15 && move && camara.position.x > -776f)	{
					camara.Translate (new Vector3(-velocidadMovimientoActual * Time.deltaTime, 0, 0), Space.World);
					camaraMoviendose = true;
				}	
				else if (Input.mousePosition.x > Screen.width - 5 && move && camara.position.x < -422f) {
					camara.Translate(new Vector3(velocidadMovimientoActual*Time.deltaTime, 0, 0), Space.World);
					camaraMoviendose = true;
				}
				
				if (Input.mousePosition.y < 5 && move && camara.position.z > 1276f) {
					camara.Translate(new Vector3(0, 0, -velocidadMovimientoActual * Time.deltaTime), Space.World);
					camaraMoviendose = true;
				}
				else if (Input.mousePosition.y > Screen.height - 15 && camara.position.z < 2016f ) 	{
					camara.Translate(new Vector3(0, 0, velocidadMovimientoActual * Time.deltaTime), Space.World);
					camaraMoviendose = true;
				}

				//La velocidad se incrementa cuanto mas tiempo nos estamos moviento
				if (camaraMoviendose)	
					if (velocidadMovimientoActual <= velocidadMovimientoMaxima) 
						velocidadMovimientoActual += 10f * Time.deltaTime;
				else  
					velocidadMovimientoActual = velocidadMovimiento;

				//Zoom in/out con la rueda del raton o con las teclas + y -
				if (Input.GetAxis ("Mouse ScrollWheel") != 0 || Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.Less)) {

					//Zoom In
					if ((Input.GetAxis ("Mouse ScrollWheel") > 0 || Input.GetKey(KeyCode.Plus)) 
						&& zoom < 20) {
						Camera.main.transform.Translate (Camera.main.transform.forward * zoomRate);
						zoom += zoomRate; 
					}
					//Zoom out
					else
						if (zoom > -50) {
							Camera.main.transform.Translate (-Camera.main.transform.forward * zoomRate);
							zoom -= zoomRate;
						}
					
				}
			}
			//Si es version tablet, dibujar botones de + y - para el zoom y controlar el movimiento por arrastre. Desafio.
			else {
			}
		}

		//Movimiento desde los bordes activado/desactivado
		public void movimientoBordes(bool b)
		{
			move = b;
		}

		public float GetZoom(){
			return zoom;
		}

	}
