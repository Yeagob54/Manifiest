// (cc) 2014 SANTIAGO DOPAZO HILARIO

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class gui : MonoBehaviour {

	//Variablees de tamaños y posiciones en pantalla
	private int screenHeight;
	private int screenWidth;
	private int Margen = 20;
	private float mouseX, mouseY;
	private Vector3 menuBtnDerPos;
	public float mainMenuWidth = 150;
	private float guiSelectedSize;	
	
	//Texturas del GUI
	public Texture normalCursor;
	public Material cursorMaterial;
	public Texture hoverCursor;
	public Texture[] attackCursor;
	public Texture[] goCursor;
	public Texture invalidCursor;
	public Texture[] InteractCursor;
	public Texture accionIrA;
	public Texture accionParar;
	public Texture accionCorrer;
	public Texture accionCantar;
	public Texture accionRepartir;
	public Texture accionLanzar;
	public Texture accionQuemar;
	public Texture accionHablar;
	public Texture accionCoger;
	public Texture botonLider;
	public Texture botonFree;
	public Texture botonTerceraPersona;
	public Texture botonParar;
	public Texture botonProtestar;
	public Texture botonAtacar;
	public Texture botonAndar;
	public Texture botonBailar;
	public Texture botonMovil;
	public Texture botonActoPoetico;

	//Texturas 2D
	public Texture2D fondoGUI;
	public Texture2D constructorHover;
	public Texture2D constructorSelected;
	
	public Texture2D supportNormal;
	public Texture2D supportHover;
	public Texture2D supportSelected;

	//private Texture2D blackTexture;
	private Texture2D greenTexture;
	private Texture2D redTexture;

	//Animaciones del cursor. Revisar todos estos cursores!!!
	private float cursorTimer = 0;
		
	//Hover Cursor Variables	
	private float hoverSize = 0.8f;
	private float currentHoverSize;
	private float maxHoverSize = 1.5f;
	private float hoverRate = 1.2f;

	//attack cursor variables
	private float attackRate = 0.2f;
	private int attackCursorState = 0;
	private int attackCursorNum;
	
	//go cursor variables
	private float goRate = 0.2f;
	private int goCursorState = 0;
	private int goCursorNum;
	
	//invalid cursor variables
	private float invalidSize = 30.0f;

	//Interact cursor variables
	private float IntRate = 0.4f;
	private int IntCursorState = 0;
	private int IntCursorNum;
	private float IntSize = 40.0f;

	//Variables para el marco de seleccion
	private Vector3 selectBoxStart = new Vector3();
	private Vector3 selectBoxEnd = new Vector3();
	private Vector3[] dLoc = new Vector3[2];
	
	//Estados del cursor
	public enum CursorState
	{
		normal,
		hover,
		go,
		attack,
		invalid,
		inter
	};

	//Estados posibles
	private enum acciones
	{
		lider,
		free,
		terceraPersona
	};

	//Posibles estados del juego	
	private enum estadosJuego
	{
		mainMenu,
		jugando,
		pausa,
		prueba,
		camaraCine
	};

	//Variables de estado
	private estadosJuego estadoJuego = estadosJuego.mainMenu;
	//Estado actual del cursor
	public CursorState cursorState = CursorState.normal;
	
	//Accion Actual
	private acciones accionActual = acciones.free;

	//Variables de estilo del GUI
	private GUIStyle dragBoxStyle = new GUIStyle();
	private GUIStyle ambienteBarStyle = new GUIStyle();
	private GUIStyle concienciaBarStyle = new GUIStyle();
	private GUIStyle repercusionBarStyle = new GUIStyle();
	
	public GUIStyle manifestLabel;
	
	//public GUIStyle mainMenuBackground;
	
	public GUIStyle topButtonStyle;
	
	public GUIStyle itemButtonStyle;
	
	public GUIStyle supportButtonStyle;
	
	public GUIStyle endGameStyle;
	
	public GUIStyle infoHoverStyle;
	
	private bool disableOnScreenMouse = false;
	
	public float[] miniMapBounds = new float[4];
	
	
	//Variables para control del teclado para los botones de camara
	private bool teclaT = false;
	private bool teclaF = false;
	private bool teclaL = false;
	private bool teclaF1 = false;
	private bool teclaF2 = false;
	private bool teclaF3 = false;
	private bool teclaF4 = false;
	private bool teclaF5 = false;
	private bool teclaF6 = false;
	private bool teclaF7 = false;
	private bool teclaF8 = false;
	private bool tecla1 = false;
	private bool tecla2 = false;

	private Renderer objectRenderer;
	private Material[] objectMaterials;
	private Color[] objectColors;

	//Variables para controlar el arrastre del raton
	private Vector2 dragStart, dragEnd;
	public bool mouseDrag = false;
	
	private float cameraZOffset;
	
	private bool endGameWon = false;
	private bool endGameLost = false;

	private bool camaraMenuRotando = false;
	private bool camaraMenuVolviendo = false;

	public bool alwaysDisplayRadar = true;
	public bool radarClosed = false;

	private bool mouseSobreObjetivo = false;

	private bool menuBtnDerOn = false;

	//Rectangulo de la zona en la que estamos en el minimapa
	public Rect miniMapRect;

	//Objeto que puede ser hecho objetivo de acciones
	private Transform objetivoInteractuar;
	
	//Variable para saber a quien estamos manejando cuando estamos en 3ª persona
	private GameObject personaTerceraPersona;
	
	//Variable publica para acceder al GUI
	public static gui temp;

	void Start () 
	{
		temp = this;

		Screen.showCursor = false;
		screenHeight = Screen.height;
		screenWidth = Screen.width;

		//Estilo del cuadrado de arrastre
		dragBoxStyle.normal.background = makeColor (0.8f, 0.8f, 0.8f, 0.3f);
		dragBoxStyle.border.bottom = 1;
		dragBoxStyle.border.top = 1;
		dragBoxStyle.border.left = 1;
		dragBoxStyle.border.right = 1;

		//Estilo de las etiquetas de informacion
		infoHoverStyle.normal.background = makeColor (0.8f, 0.8f, 0.8f, 0.5f);
		infoHoverStyle.border.bottom = 1;
		infoHoverStyle.border.top = 1;
		infoHoverStyle.border.left = 1;
		infoHoverStyle.border.right = 1;
		infoHoverStyle.normal.textColor = Color.white;

		//Estilo de los mensajes en pantalla
		manifestLabel.normal.textColor = Color.white;
		manifestLabel.fontSize = 14;
		manifestLabel.fontStyle = FontStyle.Bold;

		/*
		mainMenuBackground.normal.background = makeColor (0.3f, 0.3f, 0.9f, 1.0f);
		mainMenuBackground.border.bottom = 1;
		mainMenuBackground.border.top = 1;
		mainMenuBackground.border.left = 1;
		mainMenuBackground.border.right = 1;*/

		//Estilo de las barras de Ambiente, 
		ambienteBarStyle.border.bottom = 2;
		ambienteBarStyle.border.top = 2;
		ambienteBarStyle.border.left = 2;
		ambienteBarStyle.border.right = 2;

		//Estilo de las barras de Impacto, 
		repercusionBarStyle.border.bottom = 2;
		repercusionBarStyle.border.top = 2;
		repercusionBarStyle.border.left = 2;
		repercusionBarStyle.border.right = 2;

		//Estilo de las barras de Repercusion, 
		concienciaBarStyle.border.bottom = 2;
		concienciaBarStyle.border.top = 2;
		concienciaBarStyle.border.left = 2;
		concienciaBarStyle.border.right = 2;

		//Estilo de los support buttons
		supportButtonStyle.alignment = TextAnchor.MiddleCenter;

		//Establecemos el color de las texturas definidas
		//blackTexture = makeColor (0,0,0,1);
		greenTexture = makeColor (0,1,0,1);
		redTexture = makeColor (1,0,0,1);

		//Definimos el estilo de los botones de menu
		topButtonStyle.normal.background = fondoGUI;
		topButtonStyle.hover.background = constructorHover;

		//Definimos el estilo del "fin de juego"
		endGameStyle.normal.textColor = Color.white;
		endGameStyle.fontSize = 80;
		endGameStyle.fontStyle = FontStyle.Bold;

		//Establecemos las variables para los cursores dinamicos
		currentHoverSize = maxHoverSize;
		attackCursorNum = attackCursor.Length;
		goCursorNum = goCursor.Length;	
		IntCursorNum = InteractCursor.Length;

		//mostramos el radar si en el manager esta asi indicado. 
		alwaysDisplayRadar = manager.temp.alwaysDisplayRadar;


		//Rectangulo con la zona de vision en el mini mapa
		miniMapRect = miniMapController.temp.getMapRect ();

		
		//cameraZOffset = (Camera.main.GetComponent<mainCamera>().heightAboveGround)*Mathf.Tan ((Camera.main.GetComponent<mainCamera>().angleOffset)*Mathf.Deg2Rad);
	}

	//Esta codigo se ejecuta una vez por frame
	void Update () 
	{
		//Control por teclado de los botones
		teclaT = Input.GetKey(KeyCode.T);					
		teclaF = Input.GetKey(KeyCode.F);
		teclaL = Input.GetKey(KeyCode.L);
		teclaF1 = Input.GetKey(KeyCode.F1);
		teclaF2 = Input.GetKey(KeyCode.F2);
		teclaF3 = Input.GetKey(KeyCode.F3);
		teclaF4 = Input.GetKey(KeyCode.F4);
		teclaF5 = Input.GetKey(KeyCode.F5);
		teclaF6 = Input.GetKey(KeyCode.F6);
		teclaF7 = Input.GetKey(KeyCode.F7);
		teclaF8 = Input.GetKey(KeyCode.F8);
		tecla1 = Input.GetKey (KeyCode.Alpha1);
		tecla2 = Input.GetKey (KeyCode.Alpha2);

	}

	//Cada vez que se actualiza el GUI
	void OnGUI()
	{
		//Se acabo la partida
		if (endGameWon)
		{
			GUI.Label (new Rect(0,0,screenWidth, screenHeight), "MANIFESTACION TERMINADA\n  ¡EXITO!", endGameStyle);
			Time.timeScale = 0.0f;
			//Mostrar menu con opciones: volver al menu, recomenzar o salir
		}
		else if (endGameLost)
		{
			GUI.Label (new Rect(0,0,screenWidth, screenHeight), "LA MANIFESTACION \n HA SIDO DISUELTA", endGameStyle);
			Time.timeScale = 0.0f;
			//Mostrar menu con opciones: cargar partida, recomenzar o salir
			//estadoJuego = estadosJuego.mainMenu;
		}			
		else if (estadoJuego == estadosJuego.mainMenu) {
			//El tiempo se para mientras estamos en el menu
			Time.timeScale = 0.0f;
			//Buscamos la camara Main menu
			Camera tCam = GameObject.Find("Camara Main Menu").camera;
			//Deshabilitamos el audioListener de la Main
			Camera.main.GetComponent<AudioListener>().enabled = false;
			//Habilitamos el audilistener de la camara-menu
			tCam.GetComponent<AudioListener>().enabled = true;
			//Ponemos la camara Menu como principal
			tCam.depth = 5;

			//Efecto de movmimiento de camara, para ver las instrucciones
			if (camaraMenuRotando) {
				tCam.transform.Rotate(tCam.transform.up, -4);
				if (tCam.transform.rotation.y < 0.02f)
					camaraMenuRotando = false;
			}
			else if (camaraMenuVolviendo) {
				tCam.transform.Rotate(tCam.transform.up,+5);
				if (tCam.transform.rotation.y > 0.73f)
					camaraMenuVolviendo = false;
			}

			//Detectamos si se hace click sobre alguno de los botones. 
			Ray ray = tCam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray,out hit)) {
				//Capa 17: Botones
				if (hit.collider.gameObject.layer == 17) {
					//Cambiamos la forma del cursor
					cursorState = CursorState.go;
					//Y si pulsa, miramos en que boton ha pulsado
					if (Input.GetMouseButtonUp(0)) {
						if (hit.collider.gameObject.name == "Boton Start Game") {
							//Comienza el juego
							tCam.depth = 0;
							Time.timeScale = 1.0f;
							estadoJuego = estadosJuego.jugando;
							tCam.GetComponent<AudioSource>().Stop();
							GameObject.Find("Camara Main Menu").GetComponent<AudioListener>().enabled = false;
							GameObject.Find("Main Camara").GetComponent<AudioListener>().enabled = true;
						}
						else if (hit.collider.gameObject.name == "Boton Instrucciones") 
							camaraMenuRotando = true;							
						
						else if (hit.collider.gameObject.name == "Instrucciones 1" || hit.collider.gameObject.name == "Instrucciones 2") 
							camaraMenuVolviendo = true;
					}//if left click

				}//if hit on layer 17
				//Si estamos en las Instrucciones y llevamos el raton a la derecha de la pantalla volvemos
				if (Input.mousePosition.x > screenWidth-20 && tCam.transform.rotation.y < 0.02)
					camaraMenuVolviendo = true;
			}//if hit
			else
				cursorState = CursorState.normal;

		}
		/* *******************************************
		 * 	ESTADO_JUEGO = JUGANDO
		 * *******************************************/
		else if (estadoJuego == estadosJuego.jugando){
			//Cuantos manifestantes estan seleccionados
			int cuantos = selectedManager.temp.objects.Count;

			//Si la camara no esta en tercera persona, lanzamos un raycast 
			//desde la posicion del mouse, para adaptar el puntero, dependiendo del objeto que estemos apuntando
			//Y si no estamos sobre del main menu...
			if (accionActual != acciones.terceraPersona && !menuBtnDerOn && Input.mousePosition.x < screenWidth - mainMenuWidth) {
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				//Por defecto suponemos que el raton no esta sobre un objetivo
				mouseSobreObjetivo = false;
				if (Physics.Raycast(ray,out hit)) {
					//Partimos de un cursor normal y lo modificamos si hace falta
					cursorState = CursorState.normal;
					//Preguntamos por los TAG del objeto colisionado
					string tagit = hit.collider.gameObject.tag;
					if (tagit == "Manifestantes") {
						//Cambiamos la forma del cursor: Seleccionar
						cursorState = CursorState.hover;
					}
					else if (tagit == "Policias" || tagit == "Peatones" || tagit == "Contenedores" ) {
						if (cuantos > 0) {
							//Cambiamos la forma del cursor: objetivo
							cursorState = CursorState.attack;
							//Guardamos el objeto para una posible interaccion
							objetivoInteractuar = hit.collider.gameObject.transform;
							mouseSobreObjetivo = true;
						}
					}
					else if (tagit == "Suelo" && cuantos > 0 )
						//Cambiamos la forma del cursor: Ir A...
						cursorState = CursorState.go;

				}
			}		
			else 
				//Cambiamos la forma del cursor: Normal
				cursorState = CursorState.normal;

			//Para saber si hay activistas dentro del grupo seleccionado
			bool hayActivistas = false;
			//Para saber si alguno tiene movil
			bool tienenMovil = false;

			//******************************************************************
			//DIBUJAMOS LOS RECUADROS EN TORNO A LAS UNIDADES SELECCIONADAS....
			//Y APROVECHAMOS PARA VER QUE TIPOS DE MANIFESTANTES ESTAN SELECCIONADOS, PARA DEFINIR QUE ACCIONES PODRAN HACER.
			//***************************************************************************************************************
			//Si no estamos en tercera persona
			if (accionActual != acciones.terceraPersona) {
				foreach (GameObject g in selectedManager.temp.objects)
				{
					unitManager persona = g.GetComponent<unitManager>();
					//Obtenemos el tamaño de la unidad, en funcion del zoom de la camara
					guiSelectedSize = Camera.main.GetComponent<mainCamera>().getScreenUnitSize () * 25;
					//Obtenemos la posicion en pantalla de cada manifestante seleccionado
					Vector3 pos = Camera.main.WorldToScreenPoint(g.transform.position);					
					//Creacion de grupos. Revisar. 
					int gNum = g.GetComponent<selected>().getGroupNumber ();

					//Si estamos Repintando la pantalla y el manifestante esta dentro de la zona de juego
					if (Event.current.type.Equals(EventType.Repaint) && Camera.main.WorldToScreenPoint (g.transform.position).x <screenWidth - gui.temp.mainMenuWidth)
					{
						if (persona.estaAtacando) 
							//Dibujamos la marca, en rojo, de manifestante seleccionado, que esta atacando
							Graphics.DrawTexture (new Rect(pos.x-(guiSelectedSize/2),screenHeight-(pos.y+(guiSelectedSize)),
							                               guiSelectedSize,guiSelectedSize), g.GetComponent<selected>().getRedOverlay ());
						else
							//Dibujamos la marca de manifestante seleccionado
							Graphics.DrawTexture (new Rect(pos.x-(guiSelectedSize/2),screenHeight-(pos.y+(guiSelectedSize)),
							                               guiSelectedSize,guiSelectedSize), g.GetComponent<selected>().getOverlay ());
						//Para hacer grupos.
						if (gNum != -1) GUI.Label (new Rect(pos.x-20, screenHeight-pos.y+5, 20, 20), gNum.ToString ());
					}	

					//Miramos si alguno tiene movil
					if (!tienenMovil)						
						if (persona.tieneMovil)
							tienenMovil = true;

					//Miramos si alguno es activista
					if (!hayActivistas) 
						if (persona.activismo >= 50 || persona.valor >= 70) {
							hayActivistas = true;

					}
				}
			}		

			/* ********************
			 *   El FONDO DEL GUI
			 * ********************/
			if (Event.current.type.Equals(EventType.Repaint))
			{		
				//Dibujamos el fondo lateral del menu, en negro.
				Graphics.DrawTexture(new Rect(screenWidth-mainMenuWidth, screenHeight/4, mainMenuWidth, 3*screenHeight/4), fondoGUI);			
							
				//GUI background above mini map
				Graphics.DrawTexture(new Rect(screenWidth-mainMenuWidth, 0, mainMenuWidth, 
				                              (screenHeight/4)-(miniMapBounds[2] - miniMapBounds[3])), fondoGUI);
				
				//GUI background right of mini map
				Graphics.DrawTexture(new Rect(miniMapBounds[1], 0, screenWidth-miniMapBounds[1], (screenHeight/4)), fondoGUI);
				
				//GUI background left of mini map
				Graphics.DrawTexture(new Rect(miniMapBounds[0]-(screenWidth-miniMapBounds[1]), 0, screenWidth-miniMapBounds[1], 
				                              (screenHeight/4)), fondoGUI);
			}

			//LABEL: Manifestantes x
			GUI.Label (new Rect(screenWidth-mainMenuWidth+Margen, (screenHeight/4)+Margen/2, mainMenuWidth/3, (screenHeight/8)), 
			           "Manifestantes: "+ manager.temp.getManifest ().ToString(), manifestLabel);

			//Dibujamos las tres barras de estado de la mani. Con el tiempo tal vez sean 2 o no esten visibles...
			dibujarBarrasAmbiente();


			//*****************************
			// 		EFECTOS DE CAMARA
			//*****************************
			if (tecla1) 
				iniciarEfectoCamara(2,1);
			if (tecla2) 
				iniciarEfectoCamara(3,1);


			//******************************************************************
			// BOTONES DE ACCIONES POSIBLES PARA LOS MANIFESTANTES SELECCIONADOS
			//******************************************************************
			//Posicion inicial de los botones
			float b1x = screenWidth-(mainMenuWidth-Margen*2);
			float b2x = screenWidth-(mainMenuWidth-Margen);
			float bWidth = Margen*1.5f;
			float b1y = (screenHeight/4.0f)+Margen*8;
			float b2y = b1y;

			//********************** 
			//BOTON DE ACCION: LIDER
			//*********************
			//para que se seleccione al Lider, 'en la cabeza de la mani'
			if (accionActual != acciones.terceraPersona) {
				//Dibujamos el boton y averiguamos si se ha pulsado
				if (GUI.Button (new Rect(b2x, b1y, bWidth, bWidth), botonLider, topButtonStyle) || teclaL) {
					teclaL = false;
					accionActual = acciones.lider;
					//Deseleccionamos todas la unidades
					selectedManager.temp.deselectAll();
					//Seleccionamos al Lider
					selectedManager.temp.addObject(GameObject.Find("Lider Alpha"));
				}

				//Mostramos la informacion del boton, cuando el raton este 'sobre' L:
				if (Input.mousePosition.x>b2x && screenHeight-(Input.mousePosition.y)>b1y && Input.mousePosition.x<b2x+bWidth 
				    && screenHeight-(Input.mousePosition.y)<b1y+bWidth){
					GUI.Label (new Rect(Input.mousePosition.x, screenHeight-(Input.mousePosition.y)+bWidth, 135, 30), 
					           "Lider seleccionado.\nLa camara le sigue(L)", infoHoverStyle);
				}
				//Incrementamos la posicion del boton
				b2x += bWidth;
			}

			//Lider es boton de ON/OFF
			//Si el boton L esta seleccionado, cambiamos el fondo y la camara sigue al liderd
			if (accionActual == acciones.lider){
				topButtonStyle.normal.background = constructorSelected;
				topButtonStyle.hover.background = constructorSelected;
				Transform lider = GameObject.Find("Lider Alpha").transform;
				Camera.main.transform.position = new Vector3(lider.position.x-20, Camera.main.transform.position.y, lider.position.z-30);
				Camera.main.transform.LookAt (lider.position);
				
			}
			else {
				topButtonStyle.normal.background = fondoGUI;
				topButtonStyle.hover.background = constructorHover;
			}


			//*************************************************
			//ACCIONES DE MANIFESTANTES SELECCIONADOS
			//*************************************************
			if (cuantos > 0 && accionActual != acciones.terceraPersona) {

				//Si solo hay un manifestante seleccionado
				if (cuantos == 1)
					mostrarCara(b1x+Margen/3, b1y+bWidth*2);
				else
					//Mostramos multiples caras de los manifestantes seleccionados. 
					mostrarCaras(cuantos, b1x-Margen*2, b1y+bWidth*2);

				/**************************
				//BOTON DE ACCION: CAMINAR...  (F1)
				***************************/
				if (GUI.Button (new Rect(b2x, b1y, bWidth, bWidth), botonAndar, topButtonStyle) || teclaF1)
				{
					teclaF1 = false;
					//Ponemos a caminar a las unidades, a su velocidad inicial.
					foreach (GameObject g in selectedManager.temp.objects) {
						g.GetComponent<Animator>().SetFloat("Speed", g.GetComponent<unitManager>().prisa);
						g.GetComponent<unitManager>().estaParado = false;
					}
				}
				//Info CAMINAR...:
				if (Input.mousePosition.x>b2x && screenHeight-(Input.mousePosition.y)>b1y && Input.mousePosition.x<b2x+bWidth 
				    && screenHeight-(Input.mousePosition.y)<b1y+bWidth)
				{
					GUI.Label (new Rect(Input.mousePosition.x, screenHeight-(Input.mousePosition.y)+bWidth, 85, 25), 
					           "Continuar \nla marcha(F1)", infoHoverStyle);
				}
				//Incrementamos la posicion para el siguiente boton
				b2x += bWidth;

				/******************************
				//BOTON DE ACCION: PROTESTAR   (  F4  )
				*******************************/
				//Dibujamos el boton de protesar, o con F4, los manifestantes empiezan a cantar
				if (GUI.Button (new Rect(b2x, b1y, bWidth, bWidth), botonProtestar, topButtonStyle) || teclaF4) {
					teclaF4 = false;
					//Si no estan cantando, los ponemos a cantar. 
					foreach (GameObject g in selectedManager.temp.objects) {					
						if (!g.GetComponent<unitManager>().estaCantando) {
							g.GetComponent<Animator>().SetBool("Protestando", true);
							//REVISAR. que cada animacion de protestando empiece en un lugar diferente!!
							g.GetComponent<unitManager>().estaCantando = true;
							// NO ESTA MUY CLARO COMO VOY A HACER LA RELACION CON EL SONIDO, ASI DE QUE MOMENTO CON TRY (REVISAR)
							try {
								g.GetComponent<AudioSource>().Play();
							}
							catch{}
						}
					}
				}
				//Info PROTESTAR:
				if (Input.mousePosition.x>b2x && screenHeight-(Input.mousePosition.y)>b1y && Input.mousePosition.x<b2x+bWidth 
				    && screenHeight-(Input.mousePosition.y)<b1y+bWidth)
				{
					GUI.Label (new Rect(Input.mousePosition.x-50, screenHeight-(Input.mousePosition.y)+bWidth, 135, 30), 
					           "Comenzar a cantar\n y protestar (F4)", infoHoverStyle);
				}
				//Incrementamos la posicion para el siguiente boton.
				b2x += bWidth;

				/*******************************
				//BOTON DE ACCION: PONER MUSICA ( F6 )
				********************************/
				//Si la persona seleccionada tiene una radio en la mano
				if (selectedManager.temp.objects[0].GetComponent<unitManager>().tieneMusica) {
					if (GUI.Button (new Rect(b2x, b2y, bWidth, bWidth), botonBailar , topButtonStyle) || teclaF6) {
						teclaF6 = false;
						//Hacemos que el manifestante reproduzca su musica. 
						selectedManager.temp.objects[0].gameObject.GetComponent<unitManager>().estaReproduciendoMusica = true;
					}

					//Info MUSICA:
					if (Input.mousePosition.x>b2x && screenHeight-(Input.mousePosition.y)>b2y && Input.mousePosition.x<b2x+bWidth 
					    && screenHeight-(Input.mousePosition.y)<b2y+bWidth)
					{
						GUI.Label (new Rect(Input.mousePosition.x-50, screenHeight-(Input.mousePosition.y)+bWidth,105, 15), 
						           "Poner Musica (F6)", infoHoverStyle);
					}

					//Incrementamos la posicion para el siguiente boton.
					b2x += bWidth;
					//Si los botones se acercan al borde, saltamos de linea
					if (b2x >= screenWidth-bWidth*2) { 
						b2x = screenWidth-(mainMenuWidth-Margen);
						b2y += bWidth;
					}									
				}

				/******************************
				//BOTON DE ACCION: MOVIL ( F7 )
				*******************************/
				//Si en el grupo seleccionado hay alguien con movil
				if (tienenMovil) {
					if (GUI.Button (new Rect(b2x, b2y, bWidth, bWidth), botonMovil , topButtonStyle) || teclaF7) {
						teclaF7 = false;
						GameObject interfaceMovil = GameObject.Find("Interface Movil");
						//Mostramos el movil. Revisar que haga distitnas cosas dependiendo del momento...
						interfaceMovil.GetComponent<ComportamientoMovil>().mirarMovil(!interfaceMovil.GetComponent<ComportamientoMovil>().mirandoMovil,10);
					}
					
					//Info movil:
					if (Input.mousePosition.x>b2x && screenHeight-(Input.mousePosition.y)>b2y && Input.mousePosition.x<b2x+bWidth 
					    && screenHeight-(Input.mousePosition.y)<b2y+bWidth)
					{
						GUI.Label (new Rect(Input.mousePosition.x-50, screenHeight-(Input.mousePosition.y)+bWidth,105, 15), 
						           "Difundir Mani (F7)", infoHoverStyle);
					}
					
					//Incrementamos la posicion para el siguiente boton.
					b2x += bWidth;
					//Si los botones se acercan al borde, saltamos de linea
					if (b2x >= screenWidth-bWidth*2) { 
						b2x = screenWidth-(mainMenuWidth-Margen);
						b2y += bWidth;
					}									
				}

				//Si hay activistas en el grupo
				if (hayActivistas) {
					/***************************************
					//BOTON DE ACCION: DISTURBIOS   (  F5  )
					***************************************/
					//Aceptaran la orden los que tengan un nivel de activismo suficiente
					if (GUI.Button (new Rect(b2x, b2y, bWidth, bWidth), botonAtacar, topButtonStyle) || teclaF5) {
						teclaF5 = false;
						iniciarDisturbios(null);
					}

					//Info DISTURBIOS:
					if (Input.mousePosition.x>b2x && screenHeight-(Input.mousePosition.y)>b2y && Input.mousePosition.x<b2x+bWidth 
					    && screenHeight-(Input.mousePosition.y)<b2y+bWidth)
					{
						GUI.Label (new Rect(Input.mousePosition.x-50, screenHeight-(Input.mousePosition.y)+bWidth,115, 15), 
						           "Comenzar Disturbios (F5)", infoHoverStyle);
					}
					//Incrementamos la posicion para el siguiente boton.
					b2x += bWidth;
					//Si los botones se acercan al borde, saltamos de linea
					if (b2x >= screenWidth-bWidth*2) { 
						b2x = screenWidth-(mainMenuWidth-Margen);
						b2y += bWidth;
					}
				}
			}
			//Si estamos en TERCERA PERSONA
			else if (accionActual == acciones.terceraPersona) {
				//Mostramos la cara del payo actual
				mostrarCara(b1x+Margen/3, b1y+bWidth*2);
				/**************************************
				//BOTON DE ACCION: PROTESTAR   (  F4  )  [Tercera persona]
				***************************************/
				//Dibujamos el boton de protesar, o con F4, los manifestantes empiezan a cantar
				if (GUI.Button (new Rect(b2x, b1y, bWidth, bWidth), botonProtestar, topButtonStyle) || teclaF4) {
					teclaF4 = false;
					//Si no esta cantando, lo ponemos a cantar. 
					personaTerceraPersona.GetComponent<Animator>().SetBool("Protestando", true);
					personaTerceraPersona.GetComponent<unitManager>().estaCantando = true;
					// NO ESTA MUY CLARO COMO VOY A HACER LA RELACION CON EL SONIDO, ASI DE QUE MOMENTO CON TRY (REVISAR)
					try {
						personaTerceraPersona.GetComponent<AudioSource>().Play();
					}catch{}

				}
				//Info PROTESTAR:
				if (Input.mousePosition.x>b2x && screenHeight-(Input.mousePosition.y)>b1y && Input.mousePosition.x<b2x+bWidth 
				    && screenHeight-(Input.mousePosition.y)<b1y+bWidth)
				{
					GUI.Label (new Rect(Input.mousePosition.x-50, screenHeight-(Input.mousePosition.y)+bWidth, 135, 30), 
					           "Comenzar a cantar\n y protestar (F4)", infoHoverStyle);
				}
				//Incrementamos la posicion para el siguiente boton.
				b2x += bWidth;
				//Si los botones se acercan al borde, saltamos de linea
				if (b2x >= screenWidth-bWidth*2) { 
					b2x = screenWidth-(mainMenuWidth-Margen);
					b2y += bWidth;
				}									

				/**************************************
				//BOTON DE ACCION: PONER MUSICA   (  F6  ) [Tercera persona]
				***************************************/
				//Si la persona tiene una radio en la mano
				if (personaTerceraPersona.GetComponent<unitManager>().tieneMusica) {
					if (GUI.Button (new Rect(b2x, b2y, bWidth, bWidth), botonBailar , topButtonStyle) || teclaF6) {
						teclaF6 = false;
						//Hacemos que el manifestante reproduzca su musica. 
						personaTerceraPersona.gameObject.GetComponent<unitManager>().estaReproduciendoMusica = true;
					}
					
					//Info MUSICA:
					if (Input.mousePosition.x>b2x && screenHeight-(Input.mousePosition.y)>b2y 
					    && Input.mousePosition.x<b2x+bWidth && screenHeight-(Input.mousePosition.y)<b2y+bWidth)
					{
						GUI.Label (new Rect(Input.mousePosition.x-50, screenHeight-(Input.mousePosition.y)+bWidth,105, 15), 
						           "Poner Musica (F6)", infoHoverStyle);
					}
					
					//Incrementamos la posicion para el siguiente boton.
					b2x += bWidth;
					//Si los botones se acercan al borde, saltamos de linea
					if (b2x >= screenWidth-bWidth*2) { 
						b2x = screenWidth-(mainMenuWidth-Margen);
						b2y += bWidth;
					}									
				}

			}

			/********************************
			//  BOTON DE ACCION: FREE (libre)
			********************************/
			//Dibujamos el boton de Libre, con una F(ree), este servira para que la camara este libre o en modo helicoptero.
			//Solo si estamos siguiendo al lider o en modo tercera persona
			if (accionActual == acciones.lider || accionActual == acciones.terceraPersona) {
				//Ponemos el cursor a normal
				cursorState = CursorState.normal;
				//Si hay mas de un manifestante seleccionado, tambien salimos del modo tercera persona.
				if (GUI.Button (new Rect(b2x, b2y, bWidth, bWidth), botonFree, topButtonStyle) || teclaF || cuantos > 1 )	{
					if (accionActual == acciones.terceraPersona) 				
						saliendoTerceraPersona(personaTerceraPersona);			
					accionActual = acciones.free;
					teclaF = false;
				}
				//Info ( F ):
				if (Input.mousePosition.x>b2x && screenHeight-(Input.mousePosition.y)>b2y 
				    && Input.mousePosition.x<b2x+bWidth && screenHeight-(Input.mousePosition.y)<b2y+bWidth)
				{
					GUI.Label (new Rect(Input.mousePosition.x, screenHeight-(Input.mousePosition.y)+bWidth, 105, 15), 
					           "Camara Libre/Free (F)", infoHoverStyle);
				}
				//Incrementamos la posicion para el siguiente boton.
				b2x += bWidth;
				//Si los botones se acercan al borde, saltamos de linea
				if (b2x >= screenWidth-bWidth*2) { 
					b2x = screenWidth-(mainMenuWidth-Margen);
					b2y += bWidth;
				}

			}

			/*******************************************
			//BOTON DE ACCION: TERCERA PERSONA   (  T  )		
			*******************************************/
			//Pasar a manejar al manifestante directamente con WASD, apareceran las acciones contextuales. 
			if (cuantos == 1 && accionActual != acciones.terceraPersona) {

				//Dibujamos el boton de Tercera Persona,  para que la camara siga en 3ª persona a la unidad seleccionada.
				if (GUI.Button (new Rect(b2x, b2y, bWidth, bWidth), botonTerceraPersona, topButtonStyle) || teclaT) {
					//Control por teclado de la tecla T
					teclaT = false;
					//Almacenamos en la variable 'personaTerceraPersona' al manifestante seleccionado
					personaTerceraPersona = selectedManager.temp.objects[0];
					accionActual = acciones.terceraPersona;

					//Activamos el control en primera persona del manifestante
					selectedManager.temp.objects[0].GetComponent<ComportamientoHumano>().terceraPersona = true;
					selectedManager.temp.objects[0].GetComponent<unitManager>().terceraPersona = true;

					//Encontramos la camara de seguimiento personal y la atachamos al manifestante seleccionado
					Camera cPersonal;
					cPersonal = GameObject.Find("CamaraPersonal").camera;
					//Hacemos que la musica y los efectos sean los mismos que en la camara anteior
					cPersonal.audio.clip = Camera.main.audio.clip;
					cPersonal.audio.Play();
					//Y ponemos de Main a la CamaraPersonal
					cPersonal.depth = 5;
					//La adjuntamos al manifestante para que se mueva con el
					cPersonal.gameObject.transform.parent = personaTerceraPersona.transform;
					//Ajustamos la posicion y la rotacion, con respecto al manifestante 
					cPersonal.transform.rotation = personaTerceraPersona.transform.rotation;
					cPersonal.transform.forward = personaTerceraPersona.transform.forward;
					cPersonal.gameObject.transform.position = personaTerceraPersona.transform.position + Vector3.up*4 + Vector3.right*3 - Vector3.forward*3;

					//Activamos el audio listener y se lo desactivamos a la camara principal
					cPersonal.GetComponent<AudioListener>().enabled = true;
					GameObject.Find("Main Camara").camera.GetComponent<AudioListener>().enabled = false;	

					//Eliminamos la seleccion de todo manifestante
					selectedManager.temp.deselectAll();
			
				}

				//Info T:
				if (Input.mousePosition.x>b2x+bWidth*3 && screenHeight-(Input.mousePosition.y)>b2y 
				    && Input.mousePosition.x<b2x+bWidth && screenHeight-(Input.mousePosition.y)<b2y+bWidth)
				{
					GUI.Label (new Rect(Input.mousePosition.x-50, screenHeight-(Input.mousePosition.y)+bWidth, 135, 30), 
					           "Control en tercera\npersona (T)", infoHoverStyle);
				}

				//Incrementamos la posicion para el siguiente boton.
				b2x += bWidth;
				//Si los botones se acercan al borde, saltamos de linea
				if (b2x >= screenWidth-bWidth*2) { 
					b2x = screenWidth-(mainMenuWidth-Margen);
					b2y += bWidth;
				}

			}				
		


								/******************************************************
			 					* 				MENU DEL BOTON DERECHO
			 					* ****************************************************/


			//Si al pulsar el boton derecho estamos sobre un objeto interactuable, lo indicamos
			if (Input.GetMouseButtonDown(1))
				if (cursorState == CursorState.attack) 
					mouseSobreObjetivo = true;

			//Si presionamos con el boton derecho sobre la pantalla, cambiamos el cursor, para indicar que podemos mover la camara
			if (Input.GetMouseButton (1))
				cursorState = CursorState.inter;

			//CONTROL DEL BOTON DERECHO, PARA MOSTRAR EL MENU
			if (Input.GetMouseButtonUp (1) && cuantos > 0 && accionActual!=acciones.terceraPersona) {
				//Posiciones relativas al lugar donde se pulso el boton derecho...
				menuBtnDerPos = Input.mousePosition;
				menuBtnDerOn = true;			
			}

			//Si esta activo el menu del boton derecho.
			if (menuBtnDerOn) {
				//Ponemos el cursor de laton a puntero, de nuevo.
				cursorState = CursorState.normal;

				//Posicion de los botones
				b1y = screenHeight-(menuBtnDerPos.y);
				b1x = menuBtnDerPos.x+(bWidth)+Margen/2;
			
				//Obtenemos la posicion en la que se hizo el click
				Ray ray = Camera.main.ScreenPointToRay(menuBtnDerPos);
				RaycastHit hit;
				Physics.Raycast(ray,out hit);

				/*********************************************
				//BOTON MENU DERECHO ACCION: CAMINAR...  (F2)
				*********************************************/
				if (GUI.Button (new Rect(b1x, b1y, bWidth, bWidth), accionIrA, topButtonStyle) || teclaF2)  {
					teclaF2 = false;
					menuBtnDerOn = false;
					mouseSobreObjetivo = false;

					//Detenemos los ataques??
					//g.GetComponent<unitManager>().stopAttack();

					//Posicionamos un duplicado del objeto 'DestinoTemp' en el lugar del click. 
					//Y lo asignamos como destino de los manifestantes
					GameObject destinoT = GameObject.Find("DestinoTemp");
					GameObject destinoTemp = (GameObject)UnityEngine.Object.Instantiate(destinoT, hit.point, transform.rotation);
					destinoTemp.GetComponent<tempMesh>().quitaTextura = true;

					//Ponemos a caminar a las unidades, hacia el destino indicado
					foreach (GameObject g in selectedManager.temp.objects) {
						g.GetComponent<Animator>().SetFloat("Speed", g.GetComponent<unitManager>().prisa);
						//Activamos el movimiento por orden directa, hacia el destino indicado
						g.GetComponent<unitManager>().isMoving(true);
						g.GetComponent<ComportamientoHumano>().moviendose = true;
						g.GetComponent<ComportamientoHumano>().destinoTemp = destinoTemp.transform;
					}
				}
				//Info CAMINAR...:
				if (Input.mousePosition.x>b1x && screenHeight-(Input.mousePosition.y)>b1y 
				    && Input.mousePosition.x<b1x+bWidth && screenHeight-(Input.mousePosition.y)<b1y+bWidth)
				{
					GUI.Label (new Rect(Input.mousePosition.x, screenHeight-(Input.mousePosition.y)+bWidth, 85, 15), 
					           "Caminar... (F2)", infoHoverStyle);
				}		

				//Incrementamos la posicion para el siguiente boton.
				b1x += bWidth+Margen/2;

				/*********************************************
				//BOTON MENU DERECHO ACCION: : CORRER  (F3)
				*********************************************/
				if (GUI.Button (new Rect(b1x, b1y, bWidth, bWidth), accionCorrer, topButtonStyle) || teclaF3)  {
					teclaF3 = false;
					menuBtnDerOn = false;
					mouseSobreObjetivo = false;

					//Aqui la velocidad a la que correran las unidades
					float velocidadCorriendo = 0.7f;
					
					//Posicionamos un duplicado del objeto 'DestinoTemp' en el lugar del click. 
					//Y lo asignamos como destino de los manifestantes
					GameObject destinoT = GameObject.Find("DestinoTemp");
					GameObject destinoTemp = (GameObject)UnityEngine.Object.Instantiate(destinoT, hit.point, transform.rotation);
					destinoTemp.GetComponent<tempMesh>().quitaTextura = true;

					//Hacemos correr a todas las unidades seleccionadas. 
					foreach (GameObject g in selectedManager.temp.objects) {
						g.GetComponent<Animator>().SetFloat("Speed", velocidadCorriendo);
						//Activamos el movimiento por orden directa, hacia el destino indicado
						g.GetComponent<unitManager>().isMoving(true);
						g.GetComponent<unitManager>().tiempoCorriendo += Time.deltaTime;
						g.GetComponent<ComportamientoHumano>().moviendose = true;
						g.GetComponent<ComportamientoHumano>().destinoTemp = destinoTemp.transform;
					}
				}

				//Info CORRER...:
				if (Input.mousePosition.x>b1x && screenHeight-(Input.mousePosition.y)>b1y 
					    && Input.mousePosition.x<b1x+(bWidth) && screenHeight-(Input.mousePosition.y)<b1y+bWidth)
				{
					GUI.Label (new Rect(Input.mousePosition.x, screenHeight-(Input.mousePosition.y)+bWidth, 71, 15), 
						           "Correr... (F3)", infoHoverStyle);
				}

				//Incrementamos la posicion para el siguiente boton.
				b1x += bWidth+Margen/2;
						
				//Si el mouse esta sobre un objetivo, mostramos las opciones de interaccion con dicho objetivo
			    if (mouseSobreObjetivo) {
					/***********************************************
					 * BOTON MENU DERECHO ACCION: : DISTURBIOS/ATACAR  (F5)
					 * *********************************************/
					if (GUI.Button (new Rect(b1x, b1y, bWidth, bWidth), accionLanzar, topButtonStyle) || teclaF5) {
						teclaF5 = false;
						menuBtnDerOn = false;
						mouseSobreObjetivo = false;
						iniciarDisturbios(objetivoInteractuar);
					}
					//Info ATACAR/DISTURBIOS...:
					if (Input.mousePosition.x>b1x && screenHeight-(Input.mousePosition.y)>b1y 
						    && Input.mousePosition.x<b1x+bWidth && screenHeight-(Input.mousePosition.y)<b1y+bWidth)
					{
						GUI.Label (new Rect(Input.mousePosition.x, screenHeight-(Input.mousePosition.y)+bWidth, 135, 15), 
							           "Atacar/Crear Disturbios (F5)", infoHoverStyle);
					}

					//Incrementamos la posicion para el siguiente boton.
					b1x += bWidth+Margen/2;
					//Si los botones se acercan al borde, saltamos de linea
					if (b1x >= screenWidth-bWidth*2) { 
						b1x = screenWidth-(mainMenuWidth-Margen);
						b1y += bWidth;
					}

					/***********************************************
					 * BOTON MENU DERECHO ACCION: : HABLAR  (F8)
					 * *********************************************/
					//Si el objetivo es una persona, podemos hablar con ella
					if (hit.collider.gameObject.layer == 8) {
						if (GUI.Button (new Rect(b1x, b1y, bWidth, bWidth), accionHablar, topButtonStyle) || teclaF8) {
							teclaF8 = false;
							menuBtnDerOn = false;
							mouseSobreObjetivo = false;
							//ACCION calmar arnimos/Dar discurso/ convencer, dependiendo del objetivo
							//PENDIENte. revisar. 
						}
						//Info HABLAR...:
						if (Input.mousePosition.x>b1x && screenHeight-(Input.mousePosition.y)>b1y+bWidth*2 
							    && Input.mousePosition.x<b1x+(bWidth) && screenHeight-(Input.mousePosition.y)<b1y+bWidth)
						{
							GUI.Label (new Rect(Input.mousePosition.x, screenHeight-(Input.mousePosition.y)+bWidth, 135, 15), 
								           "Calamar Animos (F8)", infoHoverStyle);
						}						
						//Incrementamos la posicion para el siguiente boton.
						b1x += bWidth+Margen/2;
						//Si los botones se acercan al borde, saltamos de linea
						if (b1x >= menuBtnDerPos.x+(bWidth*3)+Margen/2) { 
							b1x = menuBtnDerPos.x+(bWidth)+Margen/2;
							b1y += bWidth;
						}
					} 
					/***********************************************
					 * BOTON MENU DERECHO ACCION: : COGER  (F8)
					 * *********************************************/
					else if(hit.collider.gameObject.tag == "Contenedores") {
						if (GUI.Button (new Rect(b1x, b1y, bWidth, bWidth), accionCoger, topButtonStyle) || teclaF8) {
							teclaF8 = false;
							menuBtnDerOn = false;
							mouseSobreObjetivo = false;
							//ACCION COGER!!!???
						}
						//Info COGER...:
						if (Input.mousePosition.x>b1x && screenHeight-(Input.mousePosition.y)>b1y
							    && Input.mousePosition.x<b1x+(bWidth) && screenHeight-(Input.mousePosition.y)<b1y+bWidth)
						{
							GUI.Label (new Rect(Input.mousePosition.x, screenHeight-(Input.mousePosition.y)+bWidth, 135, 15), 
								           "Recoger Objetos (F8)", infoHoverStyle);
						}						
						//Incrementamos la posicion para el siguiente boton.
						b1x += bWidth+Margen/2;
						//Si los botones se acercan al borde, saltamos de linea
						if (b1x >= menuBtnDerPos.x+(bWidth*3)+Margen/2) { 
							b1x = menuBtnDerPos.x+(bWidth)+Margen/2;
							b1y += bWidth;
						}

					}
				
				}
				//Estas acciones se muestra si el mouse NO esta sobre un bjetivo. 
				else {
					/***********************************************
					 * BOTON MENU DERECHO ACCION: : CANTAR  (F4)
					 * *********************************************/
					if (GUI.Button (new Rect(b1x, b1y, bWidth, bWidth), accionCantar, topButtonStyle) || teclaF4)	{
						teclaF4 = false;
						menuBtnDerOn = false;

						//Si no estan cantando, los ponemos a cantar. 
						foreach (GameObject g in selectedManager.temp.objects) {
							if (!g.GetComponent<unitManager>().estaCantando) {
								g.GetComponent<Animator>().SetBool("Protestando", true);
								g.GetComponent<unitManager>().estaCantando = true;
								// NO ESTA MUY CLARO COMO VOY A HACER LA RELACION CON EL SONIDO, ASI DE QUE MOMENTO CON TRY
								try {
									g.GetComponent<AudioSource>().Play();
								}
								catch{}
							}
						}
					}
					//Info Cantar...:
					if (Input.mousePosition.x>b1x && screenHeight - (Input.mousePosition.y) > b1y
						    && Input.mousePosition.x<b1x+(bWidth) + Margen && screenHeight - (Input.mousePosition.y) < b1y + bWidth)
					{
						GUI.Label (new Rect(Input.mousePosition.x, screenHeight-(Input.mousePosition.y)+bWidth, 71, 15), 
							           "Cantar (F4)", infoHoverStyle);
					}

					//Incrementamos la posicion para el siguiente boton.
					b1x += bWidth+Margen/2;
					//Si los botones se acercan al borde, saltamos de linea
					if (b1x >= menuBtnDerPos.x+(bWidth*3)+Margen/2) { 
						b1x = menuBtnDerPos.x+(bWidth)+Margen/2;
						b1y += bWidth;
					}

					/***********************************************
					 * BOTON MENU DERECHO ACCION: : PARAR  (F1)
					 * *********************************************/					
					if (GUI.Button (new Rect(b1x, b1y, bWidth, bWidth), accionParar,topButtonStyle) || teclaF1)	{
						teclaF1 = false;
						menuBtnDerOn = false;

						//Detenemos a todas las unidades seleccionadas. 
						foreach (GameObject g in selectedManager.temp.objects) {
							unitManager uManifestante = g.GetComponent<unitManager>();
							//si no esta parado lo paramos
							if (!uManifestante.estaParado) {
								uManifestante.stopAttack ();
								uManifestante.isMoving(false);
							}
							//Si ya estaba parado, detenemos sus acciones
							else 
								uManifestante.stopAcciones();

						}
					}

					//Info PARAR...:
					if (Input.mousePosition.x>b1x && screenHeight-(Input.mousePosition.y)>b1y && Input.mousePosition.x<b1x 
						    && screenHeight-(Input.mousePosition.y)<b1y+bWidth)
					{
						GUI.Label (new Rect(Input.mousePosition.x, screenHeight-(Input.mousePosition.y)+bWidth, 70, 15), 
							           "Parar (F1)", infoHoverStyle);
					}
				}
			}		
		}	//End if: Estado de juego


		//**********************************************************************************************************************
		//														IMAGEN DEL CURSOR
		//**********************************************************************************************************************
		cursorTimer += Time.deltaTime;
		//Solo ejecutamos esto en los ciclos de repintado 
		if (Event.current.type.Equals(EventType.Repaint))
		{
			//Modificamos la textura del cursor, dependiento del cursorState.
			switch(cursorState)
			{
			case CursorState.normal:
				Graphics.DrawTexture(new Rect(Input.mousePosition.x, screenHeight-Input.mousePosition.y,20,20), normalCursor, cursorMaterial);	
				break;
				
			case CursorState.hover:
				currentHoverSize -= hoverRate*Time.deltaTime;
				if (currentHoverSize <= hoverSize) currentHoverSize = maxHoverSize;
				
				Graphics.DrawTexture(new Rect(Input.mousePosition.x-(10*currentHoverSize), screenHeight-Input.mousePosition.y-(10*currentHoverSize),(20*currentHoverSize),(20*currentHoverSize)), hoverCursor, cursorMaterial);
				break;
				
			case CursorState.go:
				if (cursorTimer > goRate)
				{
					cursorTimer = 0;
					goCursorState++;
					if (goCursorState >= goCursorNum) goCursorState = 0;
				}
				Graphics.DrawTexture(new Rect(Input.mousePosition.x-(20), screenHeight-Input.mousePosition.y-(20),(40),(40)), goCursor[goCursorState], cursorMaterial);
				break;
				
			case CursorState.attack:
				if (cursorTimer > attackRate)
				{
					cursorTimer = 0;
					attackCursorState++;
					if (attackCursorState >= attackCursorNum) attackCursorState = 0;					

				}
				Graphics.DrawTexture(new Rect(Input.mousePosition.x-(20), screenHeight-Input.mousePosition.y-(20),(40),(40)), attackCursor[attackCursorState], cursorMaterial);
				break;

			case CursorState.invalid:
				Graphics.DrawTexture(new Rect(Input.mousePosition.x-(invalidSize/2), screenHeight-Input.mousePosition.y-(invalidSize/2),(invalidSize),(invalidSize)), invalidCursor, cursorMaterial);
				break;

			case CursorState.inter:
				if (cursorTimer > IntRate)
				{
					cursorTimer = 0;
					IntCursorState++;
					if (IntCursorState >= IntCursorNum) IntCursorState = 0;
				}
				Graphics.DrawTexture (new Rect(Input.mousePosition.x-(IntSize/2), screenHeight-Input.mousePosition.y-(IntSize/2), IntSize, IntSize), InteractCursor[IntCursorState], cursorMaterial);
				break;
			}			
		}	
		
		//*******************************************************************************************
		//									ARRASTRE DEL RATON
		//*******************************************************************************************
		if (Input.GetMouseButtonDown(0))
		{
			//Cuando se presiona con el boton iz dentro del area de juego, 
			//iniciamos las variables de arrastre y bloqueamos el movimiento de bordes de la pantalla.
			if (Input.mousePosition.x < screenWidth-mainMenuWidth)
			{
				dragStart = Input.mousePosition;
				mainCamera.temp.edgeMovement (false);
				mouseX = Input.mousePosition.x;
				mouseY = Input.mousePosition.y;	
			}
			//Click fuera del area de juego
			else
			{
				disableOnScreenMouse = true;
			}
		}
		//Si se esta presionando el boton iz, actualizamos las variables de arrastre.
		else if (Input.GetMouseButton (0) && !disableOnScreenMouse)
		{
			dragEnd = Input.mousePosition;
			//Si nos salimos del area de juego, el limite es el ultimo punto de arrastre
			if (dragEnd.x > screenWidth-mainMenuWidth) dragEnd.x = screenWidth-mainMenuWidth;

			//Si el raton se desplaza mas de 4 pixels, con el boton pulsado, consideramos que esta arrastrando.
			if (!(Mathf.Abs(Input.mousePosition.x-mouseX) < 4 && Mathf.Abs (Input.mousePosition.y-mouseY) < 4))
			{
				mouseDrag = true;
			}			
		}
		//Al soltar el boton izquierdo finalizamos el arrastre
		else if (Input.GetMouseButtonUp (0))
		{
			mouseDrag = false;
			//Volvemos a activar el movimiento por margenes de pantalla
			mainCamera.temp.edgeMovement (true);
			disableOnScreenMouse = false;
		}


		//***********************************************
		//  SELECCION DE UNIDADES POR ARRASTRE
		//**********************************************
		if (mouseDrag)
		{
			//Si estaba el menu derecho del raton, se quita. 
			menuBtnDerOn = false;

			//Existes 4 posibles cuadrantes de arrastre, teniendo un mismo punto de partida.
			//El dragStart y el dragEnd seran las coordenadas de la esquina su perior, o inferior, 
			//del cuadro de seleccion, dependiendo de la direccion de arrastre del raton.

			//Definicion de los vertices del cuadrado de seleccion.
			if (dragStart.y < dragEnd.y && dragStart.x > dragEnd.x)
			{
				selectBoxStart = dragStart;
				selectBoxEnd = dragEnd;
			}
			else if (dragStart.y < dragEnd.y)
			{
				selectBoxStart.y = dragStart.y;
				selectBoxEnd.y = dragEnd.y;
				selectBoxStart.x = dragEnd.x;
				selectBoxEnd.x = dragStart.x;
			}			
			else if (dragStart.x > dragEnd.x)
			{
				selectBoxStart.y = dragEnd.y;
				selectBoxEnd.y = dragStart.y;
				selectBoxStart.x = dragStart.x;
				selectBoxEnd.x = dragEnd.x;
			}
			else
			{
				selectBoxStart = dragEnd;
				selectBoxEnd = dragStart;
			}		
			
			//Actualizamos en tiempo real la posicion de arrastre, para que el unitManager
			//pueda saber si su unidad ha sido seleccionada.
			dLoc[0] = new Vector3(Mathf.Min(selectBoxStart.x, selectBoxEnd.x), Mathf.Min (selectBoxStart.y, selectBoxEnd.y), 0);
			dLoc[1] = new Vector3(Mathf.Max(selectBoxStart.x, selectBoxEnd.x), Mathf.Max (selectBoxStart.y, selectBoxEnd.y), 0);
			
			//Dibujamos el cuadro de seleccion. 
			GUI.Box (new Rect(selectBoxStart.x, screenHeight-selectBoxStart.y, selectBoxEnd.x-selectBoxStart.x, selectBoxStart.y-selectBoxEnd.y), "", dragBoxStyle);
			
		}
	}

	/*****************************
	 *     EFECTOS DE CAMARA
	 * **************************/
	public void iniciarEfectoCamara(int animacion, int velocidad) {
	
		//Buscamos la camara efectos.
		GameObject Cam = GameObject.Find("Camara Efectos");
		//Si no hay ninguna animacion iniciada
		if (!Cam.GetComponent<EfectosCamara>().volando) {
			//Indicamos que animacion
			Cam.GetComponent<EfectosCamara>().animacionSeleccionada = animacion;
			//Reproducimos el efecto de camara a la velocidad seleccionada.
			Cam.GetComponent<EfectosCamara>().ComenzarAnimacion(velocidad);																	
		}

	}

	/*************************************
	 * FIN DEL JUEGO: OBJETIVO(s) CONSEGUIDO(s)
	 * *********************************/
	public void EndGameWon()
	{
		endGameWon = true;
	}

	/*************************************
	 * FIN DEL JUEGO: HAS FALLADO
	 * *********************************/
	public void EndGameLost()
	{
		endGameLost = true;
	}
	
	/************************************
	 *   SALIENDO DE TERCERA PERSONA
	 * **********************************/
	public void saliendoTerceraPersona(GameObject manifestante) {

		//Salimos de tercera persona
		manifestante.GetComponent<ComportamientoHumano>().terceraPersona = false;
		manifestante.GetComponent<unitManager>().terceraPersona = false;

		//El manifestante comienza a caminar
		manifestante.GetComponent<Animator>().SetFloat("Speed",manifestante.GetComponent<unitManager>().prisa);

		//Desactivamos la camara personal.
		Camera cPersonal = GameObject.Find("CamaraPersonal").camera;
		cPersonal.depth = 1;

		//Activamos el audio listener en la camara principal
		cPersonal.GetComponent<AudioListener>().enabled = false;
		Camera.main.GetComponent<AudioListener>().enabled = true;

		//Posicionamos la camara sobre el manifestante que acabamos de dejar
		GameObject.Find("Control camara principal").transform.Translate(new Vector3(transform.position.x - manifestante.transform.position.x, 
		                                                                             0, 
		                                                                            manifestante.transform.position.z - manifestante.transform.position.z),
		                                                                Space.Self);
	}
	
	/************************************
	 *   INICIANDO DISTURBIOS
	 * **********************************/
	private void iniciarDisturbios(Transform objetivo) {			
			int cont  = selectedManager.temp.objects.Count-1;
			//Recorremos la lista desde el final hasta el principio[0], eliminanado a los 'no bastante activistas'
			while (cont > -1) {
				GameObject g = selectedManager.temp.objects[cont].gameObject;
				//Definimos el valor minimo y el activismo minimo, para unirse a los disturbios
				int minimoActivismoNecesatio = 20;
				int minimoValorNecesatio = 10;
				if (g.GetComponent<unitManager>().activismo < minimoActivismoNecesatio 
				    || g.GetComponent<unitManager>().valor < minimoValorNecesatio) {							    
					//Si no es lo bastante activo, se le quita de la seleccion. 
					selectedManager.temp.objects.Remove(g);								
					//Si es el ultimo de la lista, reducimos el contador
					if (cont == selectedManager.temp.objects.Count) 
						cont --;
				}
				else //Decrementamos contador si no se borra, porque si se borra la lista se reduce sola.
					cont --;
			}
			
			//Los manifestantes que quedan seleccionados empiezan a atacar. 
			foreach (GameObject g in selectedManager.temp.objects) {
						g.GetComponent<unitManager> ().estaAtacando = true;
						g.GetComponent<unitManager> ().objetivoInteractuar = objetivo;
				}
		}

	//BARRAS de estado general de la mani.
    private void dibujarBarrasAmbiente() {

		//Definimos el color de fondo y del texto de las barras
		ambienteBarStyle.normal.background = greenTexture;
		concienciaBarStyle.normal.background = redTexture;
		repercusionBarStyle.normal.background = redTexture;
		ambienteBarStyle.normal.textColor = Color.white;
		concienciaBarStyle.normal.textColor = Color.white;
		repercusionBarStyle.normal.textColor = Color.white;
				
		//Obtenemos los valores de las barras de nivel
		float tempPower = manager.temp.nivelDesordenRepresion;
		float tempConciencia = manager.temp.nivelConcienciaLocal;
		float tempRepercusion = manager.temp.repercusionMediatica;
		int posX, posY;
		
		//Dibujamos la barra de ambiente en la manifestacion.
		posX = Mathf.RoundToInt(screenWidth-mainMenuWidth) + Margen;
		posY = (screenHeight/4)+Margen*2;
		GUI.Box (new Rect(posX, posY, (mainMenuWidth/100)*tempPower, (screenHeight/32)), "Ambiente", ambienteBarStyle);
		if (Input.mousePosition.x>posX && screenHeight-(Input.mousePosition.y)>posY && Input.mousePosition.x<posX+mainMenuWidth && screenHeight-(Input.mousePosition.y)<posY+(screenHeight/32))
		{
			GUI.Label (new Rect(Input.mousePosition.x+15, (Input.mousePosition.y/2)-Margen, 185, 30), "Como de caldeado esta\nel ambiente en la manifestacion.", infoHoverStyle);
		}
		
		//Dibujamos la barra de Conciencia Local
		posY = (screenHeight/4)+Margen*4;
		GUI.Box (new Rect(posX, posY, (mainMenuWidth/100)*tempConciencia, (screenHeight/32)), "Conciencia Local", concienciaBarStyle);
		if (Input.mousePosition.x>posX && screenHeight-(Input.mousePosition.y)>posY && Input.mousePosition.x<posX+mainMenuWidth && screenHeight-(Input.mousePosition.y)<posY+(screenHeight/32))
		{
			GUI.Label (new Rect(Input.mousePosition.x+15, screenHeight-(Input.mousePosition.y), 185, 30), "Como de concienciada esta\nla gente de esta ciudad.", infoHoverStyle);
		}
		
		//Dibujamos la barra de Impacto Mediatico
		posY = (screenHeight/4)+Margen*6;
		GUI.Box (new Rect(posX, posY, (mainMenuWidth/100)*tempRepercusion, (screenHeight/32)), "Repercusion Mediatica", repercusionBarStyle);
		if (Input.mousePosition.x>posX && screenHeight-(Input.mousePosition.y)>posY && Input.mousePosition.x<posX+mainMenuWidth && screenHeight-(Input.mousePosition.y)<posY+(screenHeight/32))
		{
			GUI.Label (new Rect(Input.mousePosition.x+15, screenHeight-(Input.mousePosition.y), 185, 30), "Cual es el impacto\nmediatico de esta manifestacion.", infoHoverStyle);
		}

	}

	/*****************************
	 *  MOSTRAR CARAS
	 * ***************************/
	//Mostramos las caras, cuando hay mas de un manifestante seleccionado
	private void mostrarCaras(float cuantos, float xini, float yini) {
		float ancho, alto, xact, yact= yini;
		int filas, cont=0;

		//Si hay 4 o menos manifestantes seleccionados, los tamaños son medios, si no son pequeños
		if (cuantos <= 4) {
			xini += Margen;
			xact = xini;
			ancho = Margen * 4;
			alto = Margen * 6;
			filas = 2;
		}
		else {
			xini += Margen/2;
			xact = xini;
			ancho = Margen * 2;
			alto = Margen * 3;
			filas = 4;
		}

		//Por cada manifestante seleccionado, mostramos su cara y sus manos
		foreach (GameObject g in selectedManager.temp.objects)	{
			unitManager uM = g.GetComponent<unitManager>();

			//Si pasamos el raton por encima de la cara mostramos un label conlo que tiene en las manos
			if (Input.mousePosition.x > xact - Margen && Input.mousePosition.x < xact+Margen + ancho
			    && Input.mousePosition.y > screenHeight - yact - alto - Margen && Input.mousePosition.y < screenHeight - yact)
				GUI.Label (new Rect(Input.mousePosition.x + 15, screenHeight-(Input.mousePosition.y / 2) - Margen, 185, 30), 
				           uM.manoIzquierda.name + "\n" + uM.manoDerecha.name, infoHoverStyle);

			//Dibujamos la cara como un boton para seleccionar solo a ese manifestante
			if (GUI.Button (new Rect(xact, yact, ancho, alto),uM.cara)){
				selectedManager.temp.deselectAll();
				selectedManager.temp.addObject (g);
			}

			//Dibujamos las manos como botones, pero con estilo de 'sin accion' //Revisar, no se usan como boton.
			GUI.Button(new Rect(xact, yact+alto-(alto/5), ancho/3, alto/5),uM.manoIzquierda.GetComponent<ObjetoDeMano>().imagenMiniatura,itemButtonStyle);
			GUI.Button(new Rect(xact+ancho-(ancho/3), yact+alto-(alto/5), ancho/3, alto/5),uM.manoDerecha.GetComponent<ObjetoDeMano>().imagenMiniatura,itemButtonStyle);

			//Dibujamos las barras de valor y activismo.
			dibujarBarras(xact+5, yact+5, filas,uM.valor,g.GetComponent<unitManager>().activismo);

			//Incrementamos el contador de manifestantes visualizados
			cont++;

			//Si no es el ultimo de la fila, lo añanidos a la misma, si no pasamos a la siguiente
			if (cont%filas != 0) {
				xact += ancho + Margen/2;
			}
			else {
				xact = xini;
				yact += alto + Margen/2;
			}			   
		}
	}

	/************************
	 *    MOSTRAR CARA+INFO
	 * **********************/
	//Mostramos la cara y los datos, cuando hay un solo manifestante seleccionado o estamos en tercera persona
	private void mostrarCara(float xini,float yini) {
		//Dependiendo de si estamos en tercera persona o no sacamos los datos de un sitio u otro
		GameObject g;

		//Los datos los extraemos de sitios distintos dependiendo de si es un manifestante seleccionado 
		//o estamos en tercera persona
		if (accionActual == acciones.terceraPersona)
			g = personaTerceraPersona;
		else
			g = selectedManager.temp.objects[0];

		//Posicionamiento de la cara 
		float alto, ancho;
		alto = Margen*6;
		ancho = Margen*8;
		GUI.Button (new Rect(xini, yini, alto, ancho),g.GetComponent<unitManager>().cara);
		//******
		// BARRAS DE ESTADO DE LAS PERSONAS 
		//******
		// Molaba que el mensaje que apareciese al poner el raton por encima fuera especifico para cada rango de la barra de estado: "animado" "excitado" "asustado", tec
		// Dibujamos las barras de valor y activismo de la o las personas seleccionadas
		xini = screenWidth - (mainMenuWidth)+Margen*2.5f;
		yini = screenHeight - (screenHeight / 2.0f) + Margen*2f-2f;
		dibujarBarras (xini,yini,1,g.GetComponent<unitManager>().valor, g.GetComponent<unitManager>().activismo);

		//Dibujamos las manos como botones, pero con estilo de 'sin accion'
		GUI.Button(new Rect(xini, yini+alto-(alto/7f), ancho, alto/3f),g.GetComponent<unitManager>().manoIzquierda.GetComponent<ObjetoDeMano>().imagenMiniatura,itemButtonStyle);
		GUI.Button(new Rect(xini+ancho-(ancho/1.8f), yini+alto-(alto/7f), ancho, alto/3f),g.GetComponent<unitManager>().manoDerecha.GetComponent<ObjetoDeMano>().imagenMiniatura,itemButtonStyle);

		//Datos personales
		GUI.Label (new Rect(xini-ancho/6, yini+Margen*8, mainMenuWidth/3, Margen*2), "Nombre: "+ g.GetComponent<unitManager>().nombre, manifestLabel);
		GUI.Label (new Rect(xini-ancho/6, yini+Margen*9, mainMenuWidth/3, Margen*2), "Apellidos: "+ g.GetComponent<unitManager>().apellidos, manifestLabel);
		GUI.Label (new Rect(xini-ancho/6, yini+Margen*10, mainMenuWidth/3, Margen*2), "Edad: "+ g.GetComponent<unitManager>().edad, manifestLabel);
		GUI.Label (new Rect(xini-ancho/6, yini+Margen*11, mainMenuWidth/3, Margen*2), "Salario: "+ g.GetComponent<unitManager>().salario+" euros", manifestLabel);
		GUI.Label (new Rect(xini-ancho/6, yini+Margen*12, mainMenuWidth/3, Margen*2), "Creencias: "+ g.GetComponent<unitManager>().creencias, manifestLabel);
		GUI.Label (new Rect(xini-ancho/6, yini+Margen*13, mainMenuWidth/3, Margen*2), "Energia: "+ g.GetComponent<unitManager>().energia.ToString(), manifestLabel);

		try {
			GUI.Label (new Rect(xini-ancho/6, yini+Margen*14, mainMenuWidth/3, Margen*2), "Viene con: "+ g.GetComponent<unitManager>().empatiaPersona.GetComponent<unitManager>().nombre, manifestLabel);
			if (GUI.Button (new Rect(xini+ancho/4, yini+Margen*15, ancho/4f, alto/3f),g.GetComponent<unitManager>().empatiaPersona.GetComponent<unitManager>().cara)){
				selectedManager.temp.deselectAll();
				selectedManager.temp.addObject (g.GetComponent<unitManager>().empatiaPersona);
			}
		}
		catch{
				GUI.Label (new Rect(xini-ancho/6, yini+Margen*14, mainMenuWidth/3, Margen*2), "Viene solo");
		}
	}

	/******************************************************
	 * Barras de estado de los manifestantes seleccionados
	 * ***************************************************/
	private void dibujarBarras(float xini, float yini, float escala, float valor, float activismo) {

		string valorTxt="", activismoTxt="Activismo";
		//En el tamaño grande mostramos los textos
		if (escala == 1) {
			if (valor > 0)
				valorTxt = "Valor";
			else
				valorTxt = "Miedo";

		}
		//Dibujamos la barra de 'valor', que ira de -50 a +50
		if (valor > 0)
			GUI.Box (new Rect(xini, yini, (valor * 2f)/escala, (Margen / 1.5f)/escala), valorTxt, ambienteBarStyle);
		else if (valor < 0)
			GUI.Box (new Rect(xini + valor * 2f/escala, yini, valor * (-2f)/escala, (Margen / 1.5f)/escala), valorTxt, ambienteBarStyle);
		
		//Dibujamos la barra de 'activismo' que ira de 0 a 100
		GUI.Box (new Rect(xini, yini + Margen/escala, activismo * 2f/escala, (Margen / 1.5f)/escala), activismoTxt, concienciaBarStyle);

	}

	/******************************************************
	//Funcion para generar texturas de un color en concreto
	*******************************************************/
	private Texture2D makeColor(float R, float G, float B, float A)
	{
		Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);   	 	
    	texture.SetPixel(0, 0, new Color(R, G, B, A));   
   		texture.Apply();
		return texture;
	}

	/*************************************************
	//Funcion de cambio de estado del cursor del raton
	**************************************************/
	public void setCursorState(string state)
	{
		switch (state)
		{
		case "normal":
			cursorState = CursorState.normal;
			break;
			
		case "hover":
			cursorState = CursorState.hover;
			break;
			
		case "attack":
			cursorState = CursorState.attack;
			break;

		case "go":
			cursorState = CursorState.go;
			break;
			
		case "invalid":
			cursorState = CursorState.invalid;
			break;

		case "inter":
			cursorState = CursorState.inter;
			break;
			
		default:
			
			break;
		}
		
		//cursorTimer = 0;
	}

	//Funcion que devuelve la posicion de arrastre del raton, para la seleccion multiple de unidades
	public Vector3[] dragLocations()
	{		
		return dLoc;
	}	
}
