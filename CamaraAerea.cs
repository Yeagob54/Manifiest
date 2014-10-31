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
	private float velocidadMovimientoActual;
	//Determina si la camara se esta moviendo
	private bool moviendose = false;
	//Switch control del movimiento de camara on/off
	private bool move = true;

	//Controles para tablet
	private bool versionTablet = false;

	//Relacion de incremento del zoom In/Out 
	public float zoomRate = 10f;

	//Posicion inicial de la camara en la escena
	public GameObject puntoInicial;

	//Limites de desplazamiento y del miniMapa.
	public float[] limitesMapa = new float[4];
	//Margenes de la pantalla
	private float xMin, xMax, yMin, yMax, lxMin, lxMax, lyMin, lyMax;

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
		moviendose = false;


		//Con esta linea controlamos la rotacion de camara con el boton derecho. No tablets.
		if (!versionTablet)
			GetComponent<MouseLook>().enabled = Input.GetMouseButton(1);

		//Controlamos si la camara se mueve por el mouse en margen lateral o si se esta haciendo zoom
		//El control de la camara con WASD, es mediante un FirstPersonControler
		MovimientoCamara();
	
	}
	
  /***********************************
  *  ESTABLECER LOS LIMITES DEL MAPA
  * *********************************/
  public void EstablecerLimites(float xMin, float xMax, float yMin, float yMax)
	{
		this.xMin = xMin;
		this.xMax = xMax;
		this.yMin = yMin;
		this.yMax = yMax;
	}

	/***********************************
   *  CONTROL CAMARA DENTRO DE LIMITES
   * *********************************/
	//Si se sale de los limites, vuelve dentro de ellos.
	public void comprobarPosicion() {

		Ray r1 = Camera.main.ViewportPointToRay (new Vector3(0,1,0));
		Ray r2 = Camera.main.ScreenPointToRay (new Vector3(Screen.width-Gui.temp.mainMenuWidth,Screen.height-1,0));
		Ray r3 = Camera.main.ViewportPointToRay (new Vector3(0,0,0));
		
		float left, right, top, bottom;
		
		RaycastHit h1;
		
		Physics.Raycast (r1, out h1, Mathf.Infinity, 1<< 16);		
		left = h1.point.x;
		top = h1.point.z;
		
		Physics.Raycast (r2, out h1, Mathf.Infinity, 1<< 16);
		right = h1.point.x;
		
		Physics.Raycast (r3, out h1, Mathf.Infinity, 1<< 16);
		bottom = h1.point.z;
		
		if (left < xMin)
		{
			Camera.main.transform.Translate (new Vector3(xMin-left,0,0), Space.World);
		}
		else if (right > xMax)
		{
			Camera.main.transform.Translate (new Vector3(xMax-right,0,0), Space.World);
		}
		
		if (bottom < yMin)
		{
			Camera.main.transform.Translate (new Vector3(0,0,yMin-bottom), Space.World);
		}
		else if (top > yMax)
		{
			Camera.main.transform.Translate (new Vector3(0,0,yMax-top), Space.World);
		}
	}
	/************************************************
	 *  DIBUJAMOS LAS LINEAS DE DEPLAZAMIENTO Y ATAQUE
	 * ************************************************/
	void OnPostRender()
	{

		//Dubuja una linea con el objetivo a interactuar o al destino
		foreach (GameObject g in selectedManager.temp.objects)
		{
			Vector3 startPos;
			startPos = this.camera.WorldToScreenPoint (g.transform.position);
			startPos.z = 0;
			Vector3 endPos;
			GL.PushMatrix ();			
			GL.LoadPixelMatrix();
			mat.SetPass (0);				
			GL.Begin(GL.LINES);
			
			if (g.GetComponent<UnitManager>().estaAtacando)
			{
				GL.Color (Color.red);
				try
				{
					endPos = this.camera.WorldToScreenPoint (g.GetComponent<UnitManager>().objetivoInteractuar.transform.position);
					endPos.z = 0;
				}
				catch
				{
					endPos = startPos;
				}
			}
			else if (g.GetComponent<ComportamientoHumano>().moviendose)
			{
				GL.Color (Color.blue);					
				endPos = this.camera.WorldToScreenPoint (g.GetComponent<ComportamientoHumano>().destinoTemp.position);					
				endPos.z = 0;					
			}
			//Esta pintada podemos aprobecharla para mostrar algo de info?
			else {
				GL.Color (Color.green);					
				endPos = this.camera.WorldToScreenPoint (g.transform.position);					
				endPos.z = 0;					

			}
			
			GL.Vertex (startPos);GL.Vertex (endPos);
			
			GL.End ();			
			
			//Square at end of movement line
			GL.Begin(GL.QUADS);
			GL.Vertex3 (endPos.x-3, endPos.y-3, 0);
			GL.Vertex3 (endPos.x-3, endPos.y+3, 0);
			GL.Vertex3 (endPos.x+3, endPos.y+3, 0);
			GL.Vertex3 (endPos.x+3, endPos.y-3, 0);
			GL.End ();
			GL.PopMatrix ();
			
			//Debug.Log ("draw line!");
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

		/* PARA PONER LIMITES AL MAPA, 
		//Buscamos los extremos de la prespectiva de camara*/
		Ray mRay = Camera.main.ScreenPointToRay(new Vector3(0, Screen.height, 0));
		Ray mRay2 = Camera.main.ScreenPointToRay(new Vector3(Screen.width-Gui.temp.mainMenuWidth, Screen.height, 0));
		Ray mRay3 = Camera.main.ScreenPointToRay (new Vector3(Screen.width/2, 0, 0));
		RaycastHit mHit;

		//Margenes inferior e izquierdo de la pantalla
		if (Physics.Raycast (mRay, out mHit))
		{
			lxMin = mHit.point.x;
			lyMax = mHit.point.z;
		}

		//Margen por la derecha, de la pantalla
		if (Physics.Raycast (mRay2, out mHit))
		{
			lxMax = mHit.point.x;
		}

		//Limite por arriba
		if (Physics.Raycast (mRay3, out mHit))
		{
			lyMin = mHit.point.z;
		}

		//Control de movimiento de camara por mouse, teniendo en cuenta los limites
		if (!versionTablet) {
			if (Input.mousePosition.x < 5 && move && lxMin > xMin )
			{
				transform.Translate (new Vector3(-velocidadMovimientoActual * Time.deltaTime, 0, 0), Space.World);
				moviendose = true;
			}	
			else if (Input.mousePosition.x > Screen.width-5 && move && lxMax < xMax)
			{
				transform.Translate(new Vector3(velocidadMovimientoActual*Time.deltaTime, 0, 0), Space.World);
				moviendose = true;
			}
			
			if (Input.mousePosition.y < 5 && move)// && lyMin > yMin && move)
			{
				transform.Translate(new Vector3(0, 0, -velocidadMovimientoActual*Time.deltaTime),Space.World);
				moviendose = true;
			}
			else if (Input.mousePosition.y > Screen.height-5 && move)// && lyMax < yMax && move)
			{
				transform.Translate(new Vector3(0, 0, velocidadMovimientoActual*Time.deltaTime),Space.World);
				moviendose = true;
			}
			
			//La velocidad se incrementa cuanto mas tiempo nos estamos moviento
			if (moviendose)	{
				if (velocidadMovimientoActual <= 20) {
					velocidadMovimientoActual *= 1.05f;
				}
				comprobarPosicion ();
			}
			else  {
				velocidadMovimientoActual = velocidadMovimiento;
			}

			//Zoom in/out con la rueda del raton o con las teclas + y -
			if (Input.GetAxis ("Mouse ScrollWheel") != 0 || Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.Less)) {
				//Zoom In
				if ((Input.GetAxis ("Mouse ScrollWheel") > 0 || Input.GetKey(KeyCode.Plus)) && Camera.main.fieldOfView >= 10)
					Camera.main.fieldOfView -= zoomRate*2;
				//Zoom out
				else
					if (Camera.main.fieldOfView <= 500) Camera.main.fieldOfView += zoomRate*2;
				
				comprobarPosicion ();
			}
		}
		//Si es version tablet, dibujar botones de + y - para el zoom y controlar el movimiento por minimapa
		else {
		}
	}

	//Movimiento desde los bordes activado/desactivado
	public void movimientoBordes(bool b)
	{
		move = b;
	}

}
