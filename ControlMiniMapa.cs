/********************************************************************************************
* ControlMiniMapa.cs
*
* Control de la camara de Mini Mapa, recuadro indicador de camara,
*  control del desplazamiento y marcado de las unidades.
* (Policia en rojo, Manifestantes en blanco y peatones en azul)
*
* (cc) 2014 Santiago Dopazo 
*********************************************************************************************/
using UnityEngine;
using System.Collections;

public class ControlMiniMapa : MonoBehaviour {

	//Comprobacion para version tablet
	private bool versionTablet;

	//Referencia estatica al mini mapa
	public static ControlMiniMapa temp;
	
	//Material del recuadro
	public Material mat;

	//Color de las unidades sobre el mapa
	Texture2D colorUnidades;

	//Texturas para marcar las unidades en el map
	private Texture2D whiteTexture;
	private Texture2D redTexture;
	private Texture2D blueTexture;

	//Referencia al 'transform' del Main Camera
	private Transform camaraTransform;
	
	//Referencia a la camara del mini mapa
	private Camera camaraMiniMapa;
	
	//Dimensiones y margenes del mundo 
	private float anchoReal;
	private float zMargenes = 20;
	
	//Coordenadas en pantalla del mini mapa
	private float mapaPosicionIz, mapaPosicionTop, mapaPosicionDer, mapaPosicionBottom;
	private Vector3 miniMapaInferiorIz;
	private	Vector3 miniMapaSuperiorIz;
	private	Vector3 miniMapaSuperiorDer;
	private	Vector3 miniMapaInferiorDer;

	/************************
	//START inicializaciones
	/***********************/
	void Start () 
	{
		//Comprobamos si estamos en version para tablet
		versionTablet = Manager.temp.versionTablet;
			
		//Instancia de esta clase 
		temp = this;

		//Asignamos el transform de la main camera y el componente camera de la mini-map-camera
		camaraTransform = Camera.main.transform;
		camaraMiniMapa = GetComponent<Camera>();
		
		//Calculamos el ratio de visualizacion
		float ratioVisualizacion = (float)Screen.width/(float)Screen.height;
		
		//Calculamos el tamaño en funcion del tamaño de la pantalla.
		float viewPortWidth = 1.0f/(4.5f*ratioVisualizacion);
		float viewPortX = 1-(0.25f/ratioVisualizacion);
		
		//Tamaño del mini-mapa
		camaraMiniMapa.rect = new Rect(viewPortX, 3f/4f, viewPortWidth, 1.0f/4.5f);
		
		//Sabiendo el tamaño del mini mapa calculamos el tamaño del menu del GUI
		float mapaX = Camera.main.ViewportToScreenPoint(new Vector3(viewPortX, 3f/4f, 0)).x;
		float mapaX2 = Camera.main.ViewportToScreenPoint(new Vector3(viewPortX+viewPortWidth, 3f/4f, 0)).x;
		float ancho = mapaX2 - mapaX;

		//Ajustamos el ancho del GUI Menu Principal
		Gui.temp.anchoMenuPrincipal = ancho+(2*(Screen.width-mapaX2));
		
		//Calculamos las coordenadas del mini mapa en pantalla
		mapaPosicionIz = camaraMiniMapa.ViewportToScreenPoint(new Vector3(0,0,0)).x;
		mapaPosicionBottom = camaraMiniMapa.ViewportToScreenPoint(new Vector3(0,0,0)).y;	
		mapaPosicionDer = camaraMiniMapa.ViewportToScreenPoint(new Vector3(1,1,0)).x;
		mapaPosicionTop = camaraMiniMapa.ViewportToScreenPoint(new Vector3(1,1,0)).y;

		//Limites del minimapa, para que el GUI construlla los bordes
		Gui.temp.miniMapBounds = getLimitesMapa ();

		//Definimos los colores de las texturas para marcar las unidades
		whiteTexture = makeColor (1,1,1,1);
		redTexture = makeColor (1,0,0,1);
		blueTexture = makeColor (0,0,1,1);
		
	}
	
	/**************************************
	 * UPDATE, es llamdao una vez por frame
	 * ***********************************/
	void Update () 	{
		//Click sobre el mini mapa
		if (Input.GetMouseButton (0) 
		    && Input.mousePosition.x > mapaPosicionIz 
		    && Input.mousePosition.x < mapaPosicionDer 
		    && Input.mousePosition.y > mapaPosicionBottom 
		    && Input.mousePosition.y < mapaPosicionTop)	{
				//Capturamos la posicion del click
				Vector3 mousePos = Input.mousePosition;
				Ray ray;
				RaycastHit hit;			
				//Lanzamos un ray para averiguar la posicion en la escena del click 
				ray = camaraMiniMapa.ScreenPointToRay(mousePos);	
				Physics.Raycast (ray, out hit, Mathf.Infinity, 1 << 19);
				//Movemos la camara principal a la nueva posicion
				camaraTransform.position = new Vector3(hit.point.x, camaraTransform.position.y, hit.point.z-zMargenes);
				//Comprobamos que no nos hayamos salido de los margenes				
				CamaraAerea.temp.comprobarPosicion();
		}
		else if (versionTablet && Input.touchCount > 0) {
			Touch touch = Input.GetTouch(0);
			if (touch.position.x > mapaPosicionIz 
			    && touch.position.x < mapaPosicionDer 
			    && touch.position.y > mapaPosicionBottom 
			    && touch.position.y < mapaPosicionTop)	{
					//Capturamos la posicion del click
					Vector3 touchPos = touch.position;
					Ray ray;
					RaycastHit hit;			
					//Lanzamos un ray para averiguar la posicion en la escena del touch 
					ray = camaraMiniMapa.ScreenPointToRay(touchPos);	
					Physics.Raycast (ray, out hit, Mathf.Infinity, 1 << 19);
					//Movemos la camara principal a la nueva posicion
					camaraTransform.position = new Vector3(hit.point.x, camaraTransform.position.y, hit.point.z-zMargenes);
					//Comprobamos que no nos hayamos salido de los margenes
					CamaraAerea.temp.comprobarPosicion();
			}
		}			
	}

	/**********************************************************
	//Dibujamos el recuadro de la main camera sobre el minimapa
	***********************************************************/
	public Rect getRecuadro()
	{
		//Buscamos el centro de la camara principal, para poscionar el indicador en el minimapa
		Ray ray = Camera.main.ScreenPointToRay(new Vector3((Screen.width/2)-(Gui.temp.anchoMenuPrincipal/2), Screen.height/2, 0));
		RaycastHit hit;
		float zoom = Camera.main.fieldOfView/2;

		//Lanzamos el ray y obtenemos la posicion dentro de la mini map camera
		Physics.Raycast (ray, out hit);
		Vector3 center = camaraMiniMapa.WorldToScreenPoint(hit.point);		

		//Construimos el cuadrado de visualizacion de la main camara en el mini mapa
		Rect r = new Rect(center.x-zoom/2, Screen.height - center.y - zoom, zoom, zoom);

		return r;
	}

	/**********************************************************************************
	// Dibujamos las posiciones de las unidades y el recuadro del minimapa, en pantalla
	/**********************************************************************************/
	void OnGUI()
	{	
		//Si estamos jugando...
		if (Gui.temp.estadoJuego == Gui.estadosJuego.jugando) {
			if (Event.current.type.Equals(EventType.Repaint))
			{
				//Localizamods cada unidad(policias, manifestantes y peatones) y los marcamos en el mapa
				foreach (GameObject g in Manager.temp.unidades)
				{			
					Vector3 unitPos;
					Rect marca;
					unitPos = g.transform.position;
					//Convertimos la posicion de la unidad a posicion del mapa
					unitPos = camaraMiniMapa.WorldToScreenPoint(unitPos);
					//Creamos la marca
					marca = new Rect(unitPos.x -1, Screen.height-unitPos.y-1, 3, 3);				

					if (g.tag == "Manifestantes")
						colorUnidades = whiteTexture;
					else if (g.tag == "Policias")
						colorUnidades = redTexture;
					else if (g.tag == "Peatones")
						colorUnidades = blueTexture;
					//Dibujamos la marca sobre cada unidad
					Graphics.DrawTexture (marca, colorUnidades);
				}
			}
			//Dibujamos el recuadro del minimapa
			GUI.Box (getRecuadro (), "");
		}
	}

	/**********************
	 *  Limites del mapa
	 * *******************/
	public float[] getLimitesMapa()
	{
		float[] limitesMapa = new float[4];
		limitesMapa[0] = mapaPosicionIz;
		limitesMapa[1] = mapaPosicionDer;
		limitesMapa[2] = mapaPosicionTop;
		limitesMapa[3] = mapaPosicionBottom;
		return limitesMapa;
	}

	/***********************************
	 * Creacion de texturas de un color
	 * ********************************/
	private Texture2D makeColor(float R, float G, float B, float A)
	{
		Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);   	 	
    	texture.SetPixel(0, 0, new Color(R, G, B, A));   
   		texture.Apply();
		return texture;
	}
	
}
