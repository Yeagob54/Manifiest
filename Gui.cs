/********************************************************************************************
* Gui.cs
*
* Implementacion de los controles del juego:
* 
* - Menu de la parte derecha de la pantalla, con el encuadre del mini mapa, las barras objetivos,
* los botones de accion y la zona de manifestantes seleccionados. Los botones de accion incluyen:
*  Lider (para localizar y seleccionar al lider), parar, protestar, continuar marcha, iniciar disturbios, 
*  poner musica o entrar en modo tercera persona. 
* 
* - Cambio de los punteros del raton dependiendo del objeto 'sobre' el que estamos.
* 
* - Menu contextual del boton derecho, con las acciones posibles dependiendo del objeto 'sobre' el se pulsa.
* Las acciones posibles son: Ir, ir corriendo, atacar, hablar, etc
* 
* - Seleccion de uno o varios manifestantes, mostrando sus caras, manos y barras, en la zona de manifestantes. 
* Si se selecciona un solo manifestante, se muestran todos sus datos. 
* 
*
* (cc) 2014 Santiago Dopazo 
*********************************************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gui : MonoBehaviour {

	//Variablees de tamaños y posiciones en pantalla
	private int screenHeight;
	private int screenWidth;
	public int Margen;
	private float mouseX, mouseY;
	private Vector3 menuBtnDerPos;
	public float anchoMenuPrincipal = 150;
	private float guiSelectedSize;	

	//Control de los doble clicks
	private float tiempoClick = 0f;     
	
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

	//Botnones play, doble y pause.
	public Texture2D play;
	public Texture2D pause;
	public Texture2D doble;
	public bool mostrarControlesTiempo = true;
	public bool mostrarMensajesAyuda = true;	

	//private Texture2D blackTexture;
	private Texture2D greenTexture;
	private Texture2D redTexture;

	//Animaciones del cursor
	private float cursorTimer = 0;
		
	//Hover Cursor Variables	
	private float hoverSize = 0.8f;
	private float currentHoverSize;
	private float maxHoverSize = 1.5f;
	private float hoverRate = 1.2f;

	//Attack cursor variables
	private float attackRate = 0.2f;
	private int attackCursorState = 0;
	private int attackCursorNum;
	
	//Go cursor variables
	private float goRate = 0.2f;
	private int goCursorState = 0;
	private int goCursorNum;
	
	//Invalid cursor variables
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

	//Modos de camara
	public enum camaras
	{
		lider,
		free,
		terceraPersona,
		recorrido
	};

	//Estado actual del cursor
	public CursorState cursorState = CursorState.normal;
	
	//Accion Actual
	public camaras camaraActual = camaras.free;

	//Variables de estilo del GUI
	private GUIStyle dragBoxStyle = new GUIStyle();
	private GUIStyle ambienteBarStyle = new GUIStyle();
	private GUIStyle concienciaBarStyle = new GUIStyle();
	private GUIStyle repercusionBarStyle = new GUIStyle();	
	private GUIStyle blueLogStyle = new GUIStyle();	
	private GUIStyle yellowLogStyle = new GUIStyle();	
	private GUIStyle normalLogStyle = new GUIStyle();	
	public GUIStyle manifestLabel;	
	public GUIStyle topButtonStyle;	
	public GUIStyle itemButtonStyle;	
	public GUIStyle supportButtonStyle;	
	public GUIStyle endGameStyle;	
	public GUIStyle estadisticasStyle;	
	public GUIStyle estadisticasStyle2;		
	public GUIStyle infoHoverStyle;	
	
	//Variables para control del teclado para los botones de camara
	private bool teclaT = false;
	private bool teclaF = false;
	private bool teclaL = false;
	private bool teclaI = false;
	private bool teclaQ = false;
	//private bool teclaC = false;
	private bool teclaP = false;
	private bool teclaV = false;
	private bool teclaR = false;
	private bool teclaM = false;
	private bool teclaF5 = false;
	//private bool  = false;
	//private bool tecla2 = false;

	//Variables para controlar el arrastre del raton
	private Vector2 dragStart, dragEnd;
	public bool mouseDrag = false;
	private bool disableOnScreenMouse = false;
	
	//private float cameraZOffset;

	//Estados del juego
	private bool endGameWon = false;
	private bool endGameLost = false;
	private bool marchaIniciada = false;

	//Control de camaras del menu principal
	private bool camaraMenuRotando = false;
	private bool camaraMenuVolviendo = false;

	//Variables del menu del boton derecho
	private bool mouseSobreObjetivo = false;
	private bool menuBtnDerOn = false;
	private bool clickBtnIz = false;

	//Propiedades del mini mapa
	public float[] miniMapBounds = new float[4];

	//Objeto que puede ser objetivo de acciones
	private GameObject objetivoInteractuar;
	
	//Variable para saber a quien estamos manejando cuando estamos en 3ª persona
	public GameObject personaTerceraPersona;

	//Variables para mostrar las etiquetas de informacion
	private bool mostrarEtiqueta;
	private int anchoEtiqueta;
	private int altoEtiqueta;
	private string cadenaEtiqueta;

	//Maximos de las barras de objetivo
	private float repercusionMaxima;
	private float ambienteMaxima;
	private float concienciaMaxima;

	//Para saber si hay activistas dentro del grupo seleccionado
	bool hayActivistas = false;

	//Para saber si alguno tiene movil
	bool tienenMovil = false;

	//Para saber si alguno lleva música
	bool tienenMusica = false;
	
	//Variable publica para acceder a una instancia del Gui
	public static Gui temp;

	public bool debug;

	void Start () 
	{
		//inicializamos la instancia estática de esta clase
		temp = this;

		//Velocidad del tiempo normal
		Time.timeScale = 1;

		//Hacemos invisible el cursor y preparamos las variables de tamaño de la pantalla. 
		Screen.showCursor = false;
		screenHeight = Screen.height;
		screenWidth = Screen.width;
		Margen = screenWidth / 72;

		//Inicializamos todos los estilos del GUI
		InicializarEstilos();

		//Inicializamos los maximos de las barras de objetivo
		repercusionMaxima = Manager.temp.GetRepercusionMediaticaMaxima();
		ambienteMaxima = Manager.temp.GetAmbienteManifestacionMaxima();
		concienciaMaxima = Manager.temp.GetNivelConcienciaLocalMaxima();

	}

	//Esta codigo se ejecuta una vez por frame
	void Update () 
	{
		//Control por teclado de las acciones
		teclaT = Input.GetKey(KeyCode.T);					
		teclaF = Input.GetKey(KeyCode.F);
		teclaL = Input.GetKey(KeyCode.L);
		teclaI = Input.GetKey(KeyCode.I);
		teclaP = Input.GetKey(KeyCode.P);
		teclaV = Input.GetKey(KeyCode.V);
		teclaQ = Input.GetKey(KeyCode.Q);
		teclaR = Input.GetKey(KeyCode.R);
		teclaM = Input.GetKey(KeyCode.M);
		teclaF5 = Input.GetKey(KeyCode.F5);
		if (Input.GetKey(KeyCode.Escape))
			selectedManager.temp.deselectAll();

	}

	//Cada vez que se actualiza el GUI
	void OnGUI()	{

		//Flag para determinar si estamos mostrando un mensaje de informacion o no. 
		mostrarEtiqueta = false;

		//Si hemos ganado o perdido mostramos la pantalla de estadisticas de la partida
		if (endGameLost || endGameWon) {
			Manager.temp.Reset();
			PantallaResumen();				
		}
		/* ****************************************
		 * 				JUGANDO
		 * ****************************************/
		else  {

			//Cuantos manifestantes estan seleccionados
			int cuantos = selectedManager.temp.objects.Count;

			//Control de los distintos cursores dependiendo sobre que objeto se pose el mouse
			CursoresMouse(cuantos);

			//Dibujamos el recuadro sobre los manifestantes seleccionados
			MarcarSeleccionados();	

			//Dibujamos la textura de fondo del GUI
			FondoGUI();

			//LABEL: Manifestantes x
			GUI.Label (new Rect(screenWidth-anchoMenuPrincipal+Margen, (screenHeight/4)+Margen/2, anchoMenuPrincipal/3, (screenHeight/8)), 
			           "Manifestantes: "+ Manager.temp.GetManifest ().ToString(), manifestLabel);

			//Dibujamos las tres barras de estado de la mani. Con el tiempo tal vez sean 2 o no esten visibles...
			DibujarBarrasAmbiente();

			//Dibujamos el log de sucesos de la manifestacion
			DibujarLog();


			//Dibujamos y controlamos los botones de acción
			BotonesDeAccion(cuantos);

			//Play, pause y doblre velocidad
			BotonesVelocidad();

			//Activar/desactivar mensajes de ayuda
			if (teclaF5) 
				mostrarMensajesAyuda = !mostrarMensajesAyuda;	
		
			//Control de acciones con el boton derecho del ratón
			BotonDerechoControl();

			//Cabio de los punteros del ratón
			ImagenCursor();

			//Control del arrastre del ratón
			ArrastreRaton();
		}		

		/***************************************
		*       ETIQUETAS MOUSE OVER
		****************************************/
		//Mostramos la etiqueta, si el mouse esta sobre un elemento. 
		//Se hace al final del GUI, para que este sobre todo lo demas.
		if (mostrarEtiqueta)
			GUI.Label (new Rect(Input.mousePosition.x - (anchoEtiqueta) , 
			                    screenHeight - (Input.mousePosition.y) + altoEtiqueta, 
			                    anchoEtiqueta, altoEtiqueta), 
		           	   			cadenaEtiqueta, infoHoverStyle);		

	}

	/*******************************************************
	*       INICIALIZACIÓN DE TODOS LOS ESTILOS DEL GUI
	********************************************************/
	private void InicializarEstilos ()	{

		//Estilo del cuadrado de arrastre
		dragBoxStyle.normal.background = makeColor (0.8f, 0.8f, 0.8f, 0.3f);
		dragBoxStyle.border.bottom = 1;
		dragBoxStyle.border.top = 1;
		dragBoxStyle.border.left = 1;
		dragBoxStyle.border.right = 1;

		//Estilo de las etiquetas de informacion
		infoHoverStyle.normal.background = makeColor (0.8f, 0.8f, 0.8f, 0.7f);
		infoHoverStyle.border.bottom = 1;
		infoHoverStyle.border.top = 1;
		infoHoverStyle.border.left = 1;
		infoHoverStyle.border.right = 1;
		infoHoverStyle.normal.textColor = Color.blue;

		//Estilo de los mensajes en pantalla
		manifestLabel.normal.textColor = Color.white;
		manifestLabel.fontSize = screenWidth / 100;
		manifestLabel.fontStyle = FontStyle.Bold;	

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
		endGameStyle.fontSize = screenHeight / 13;
		endGameStyle.fontStyle = FontStyle.Bold;		

		//Definimos el estilo de las estadisticas
		estadisticasStyle.normal.textColor = Color.white;
		estadisticasStyle.fontSize = screenHeight / 40;
		estadisticasStyle2.normal.textColor = Color.red;
		estadisticasStyle2.fontSize = screenHeight / 30;

		//Establecemos las variables para los cursores dinamicos
		currentHoverSize = maxHoverSize;
		attackCursorNum = attackCursor.Length;
		goCursorNum = goCursor.Length;	
		IntCursorNum = InteractCursor.Length;

	}

	//******************************************************************
	//DIBUJAMOS LOS RECUADROS EN TORNO A LAS UNIDADES SELECCIONADAS....
	//Y APROVECHAMOS PARA VER QUE TIPOS DE MANIFESTANTES ESTAN SELECCIONADOS, PARA DEFINIR QUE camaras PODRAN HACER.
	//***************************************************************************************************************
	private void MarcarSeleccionados () {

		//Si no estamos en tercera persona
		if (camaraActual != camaras.terceraPersona) {
			foreach (GameObject g in selectedManager.temp.objects)
			{
				UnitManager persona = g.GetComponent<UnitManager>();

				//Obtenemos el tamaño de la unidad, en funcion del zoom de la camara
				guiSelectedSize = 60 - Camera.main.fieldOfView / 2;

				//Obtenemos la posicion en pantalla de cada manifestante seleccionado
				Vector3 pos = Camera.main.WorldToScreenPoint(g.transform.position);	

				//Creacion de grupos. Desafio. 
				int gNum = g.GetComponent<selected>().getGroupNumber ();

				//Si estamos Repintando la pantalla y el manifestante esta dentro de la zona de juego
				if (Event.current.type.Equals(EventType.Repaint) && Camera.main.WorldToScreenPoint (g.transform.position).x <screenWidth - Gui.temp.anchoMenuPrincipal)
				{
					//Dibujamos la marca, en rojo, de manifestante seleccionado, que esta atacando
					if (persona.estaAtacando) 						
						Graphics.DrawTexture (new Rect(pos.x-(guiSelectedSize/2),screenHeight-(pos.y+(guiSelectedSize)),
						                               guiSelectedSize,guiSelectedSize), g.GetComponent<selected>().getRedOverlay ());
					//Dibujamos la marca de manifestante seleccionado
					else						
						Graphics.DrawTexture (new Rect(pos.x-(guiSelectedSize/2),screenHeight-(pos.y+(guiSelectedSize)),
						                               guiSelectedSize,guiSelectedSize), g.GetComponent<selected>().getOverlay ());						
				}	

				//Miramos si alguno tiene movil
				if (!tienenMovil && persona.tieneMovil)
						tienenMovil = true;

				//Miramos si alguno tiene musica
				if (!tieneMusica && persona.tieneMusica)
						tienenMusica = true;

				//Miramos si alguno es activista
				if (!hayActivistas && persona.activismo >= Manager.temp.activismoActivista) {
						hayActivistas = true;

				}
			}
		}	
	}

	//******************************************************************
	// BOTONES DE ACCIONES POSIBLES PARA LOS MANIFESTANTES SELECCIONADOS
	//******************************************************************
	private void BotonesDeAccion (int cuantos) {

		//Posicion inicial de los botones
			//Desafío: crear una función llamada botonAccion(...) o algo así, para ordenar esta parte
		float b1x = screenWidth - (anchoMenuPrincipal - Margen * 2);
		float b2x = screenWidth - (anchoMenuPrincipal - Margen);
		float bWidth = Margen * 1.5f;
		float b1y = (screenHeight/4.0f)+Margen*8;
		float b2y = b1y;

		//********************** 
		//BOTON DE ACCION: LIDER
		//*********************
		//para que se seleccione al Lider, 'en la cabeza de la mani'
		if (camaraActual != camaras.terceraPersona) {

			//Dibujamos el boton y averiguamos si se ha pulsado
			if (GUI.Button (new Rect(b2x, b1y, bWidth, bWidth), botonLider, topButtonStyle) || teclaL) {
				teclaL = false;
				camaraActual = camaras.lider;

				//Deseleccionamos todas la unidades
				selectedManager.temp.deselectAll();

				//Seleccionamos al Lider
				selectedManager.temp.addObject(Manager.temp.liderAlpha);

				//Todas la unidades van hacia el lider
				foreach (GameObject g in Manager.temp.unidades) {
					if (g.tag == "Manifestantes" && !g.GetComponent<UnitManager>().esLider) {
						g.GetComponent<UnitManager>().EnMovimiento(true);
						g.GetComponent<ComportamientoHumano>().moviendose = true;

						//Actualizamos los destinos de todos los manifestantes, para que sigan al LiderAlpha
						g.GetComponent<ComportamientoHumano>().destinoTemp = Manager.temp.liderAlpha.transform;
					}
				}
			}

			//Mostramos la informacion del boton, cuando el raton este 'sobre' L:
			if (Input.mousePosition.x > b2x && screenHeight - (Input.mousePosition.y) > b1y 
				&& Input.mousePosition.x < b2x + bWidth 
			    && screenHeight - (Input.mousePosition.y) < b1y + bWidth)
				dibujarEtiqueta("Lider seleccionado.\nTodos los manifestantes\n van hacia el Lider (L)", 145, 45);

			//Incrementamos la posicion del boton
			b2x += bWidth;
		}

		//Lider es boton de ON/OFF
		//Si el boton L esta seleccionado, cambiamos el fondo y la camara sigue al liderd
		if (camaraActual == camaras.lider){
			topButtonStyle.normal.background = constructorSelected;
			topButtonStyle.hover.background = constructorSelected;
			Transform lider = Manager.temp.liderAlpha.transform;
			Camera.main.transform.position = new Vector3(lider.position.x-20, Camera.main.transform.position.y, lider.position.z-30);
			Camera.main.transform.LookAt (lider.position);
			
		}
		else {
			topButtonStyle.normal.background = fondoGUI;
			topButtonStyle.hover.background = constructorHover;
		}

			/**************************
			//BOTON DE ACCION: CAMINAR...  (I)
			***************************/
			if (GUI.Button (new Rect(b2x, b1y, bWidth, bWidth), botonAndar, topButtonStyle) || teclaI)
			{					
				teclaI = false;
				IniciarMarcha();					
			}

			//Info CAMINAR...:
			if (Input.mousePosition.x > b2x && screenHeight - (Input.mousePosition.y) > b1y 
			    && Input.mousePosition.x < b2x + bWidth 
			    && screenHeight - (Input.mousePosition.y) < b1y + bWidth)				
				dibujarEtiqueta("Iniciar/Continuar \nla marcha (I)", 115, 30);

			//Incrementamos la posicion para el siguiente boton
			b2x += bWidth;


		//*************************************************
		//ACCIONES DE MANIFESTANTES SELECCIONADOS
		//*************************************************
		if (cuantos > 0 && camaraActual != camaras.terceraPersona) {

			// MOSTRAR CARAS //
			//Si solo hay un manifestante seleccionado
			if (cuantos == 1) 

				//Mostarmos su cara
				mostrarCara(b1x+Margen/3, b1y+bWidth*2);
			else

				//Mostramos multiples caras de los manifestantes seleccionados. 
				mostrarCaras(cuantos, b1x-Margen*2, b1y+bWidth*2);


			/*******************************************
			//BOTON DE PARAR: STOP/DETENER ACCION (  Q  )		
			*******************************************/
			if ((GUI.Button (new Rect(b2x, b2y, bWidth, bWidth), botonParar,topButtonStyle) || teclaQ)) {
				teclaQ = false;
				//Detenemos a todas las unidades seleccionadas
				detenerUnidadesSeleccionadas();				
			}

			//Info Q:
			if (Input.mousePosition.x > b2x && screenHeight - (Input.mousePosition.y) > b2y 
			    && Input.mousePosition.x < b2x + bWidth && screenHeight - (Input.mousePosition.y) < b2y + bWidth)
				dibujarEtiqueta("Parar / Detener acciones (Q)", 170, 15);

			//Incrementamos la posicion para el siguiente boton.
			b2x += bWidth;

			//Si los botones se acercan al borde, saltamos de linea
			if (b2x >= screenWidth-bWidth * 2) { 
				b2x = screenWidth-(anchoMenuPrincipal - Margen);
				b2y += bWidth;
			}
				

			/******************************
			//BOTON DE ACCION: PROTESTAR   (  P  )
			*******************************/
			//Dibujamos el boton de protesar, o con F4, los manifestantes empiezan a cantar
			if (GUI.Button (new Rect(b2x, b2y, bWidth, bWidth), botonProtestar, topButtonStyle) || teclaP) {
				teclaP = false;
				comenzarAProtestar();
			}

			//Info PROTESTAR:
			if (Input.mousePosition.x > b2x && screenHeight - (Input.mousePosition.y) > b2y 
				&& Input.mousePosition.x < b2x + bWidth && screenHeight - (Input.mousePosition.y) < b2y + bWidth)
				dibujarEtiqueta("Comenzar a cantar\n y protestar (P)", 135, 30);

			//Incrementamos la posicion para el siguiente boton.
			b2x += bWidth;

			/*******************************
			//BOTON DE ACCION: PONER MUSICA ( R )
			********************************/
			//Si la persona seleccionada tiene una radio en la mano
			if (tienenMusica) {
				if (GUI.Button (new Rect(b2x, b2y, bWidth, bWidth), botonBailar , topButtonStyle) || teclaR) {
					teclaR = false;

					//Hacemos que el manifestante con música reproduzca o deje de reproducir su musica. 
					foreach (UnitManager uM in Manager.temp.unidades)
						if (uM.tienenMusica)
							uM.estaReproduciendoMusica = !uM.estaReproduciendoMusica;
				}

				//Info MUSICA:
				if (Input.mousePosition.x>b2x && screenHeight-(Input.mousePosition.y)>b2y && Input.mousePosition.x<b2x+bWidth 
				    && screenHeight-(Input.mousePosition.y)<b2y+bWidth)
					dibujarEtiqueta("Reproducir/Quitar Musica (R)", 185, 15);

				//Incrementamos la posicion para el siguiente boton.
				b2x += bWidth;

				//Si los botones se acercan al borde, saltamos de linea
				if (b2x >= screenWidth-bWidth*2) { 
					b2x = screenWidth-(anchoMenuPrincipal-Margen);
					b2y += bWidth;
				}									
			}

			/******************************
			//BOTON DE ACCION: MOVIL ( M )
			*******************************/
			//Si en el grupo seleccionado hay alguien con movil
			if (Manager.temp.tiempoMovil == 0 && tienenMovil) {
				if (GUI.Button (new Rect(b2x, b2y, bWidth, bWidth), botonMovil , topButtonStyle) || teclaM) {
					teclaM = false;
					selectedManager.temp.deselectAll();
					GameObject interfaceMovil = GameObject.Find("Interface Movil");

					//Mostramos el movil. Desafio que haga distitnas cosas dependiendo del momento...
					interfaceMovil.GetComponent<ComportamientoMovil>().mirarMovil(!interfaceMovil.GetComponent<ComportamientoMovil>().mirandoMovil,10);
				}
				
				//Info movil:
				if (Input.mousePosition.x>b2x && screenHeight-(Input.mousePosition.y)>b2y && Input.mousePosition.x<b2x+bWidth 
				    && screenHeight-(Input.mousePosition.y)<b2y+bWidth)
					dibujarEtiqueta("Difundir por Movil (M)", 155, 15);
				
				//Incrementamos la posicion para el siguiente boton.
				b2x += bWidth;

				//Si los botones se acercan al borde, saltamos de linea
				if (b2x >= screenWidth-bWidth*2) { 
					b2x = screenWidth-(anchoMenuPrincipal-Margen);
					b2y += bWidth;
				}									
			}

			//Si hay activistas en el grupo
			if (hayActivistas) {
				/***************************************
				//BOTON DE ACCION: DISTURBIOS   (  V  )
				***************************************/
				//Aceptaran la orden los que tengan un nivel de activismo suficiente
				if (GUI.Button (new Rect(b2x, b2y, bWidth, bWidth), botonAtacar, topButtonStyle) || teclaV ) {
					teclaV = false;
					IniciarDisturbios(null);
				}

				//Info DISTURBIOS:
				if (Input.mousePosition.x > b2x && screenHeight - (Input.mousePosition.y) > b2y 
					&& Input.mousePosition.x < b2x + bWidth 
				     && screenHeight - (Input.mousePosition.y) < b2y + bWidth)
					dibujarEtiqueta("Disturbios Violentos (V)", 155, 15);

				//Incrementamos la posicion para el siguiente boton.
				b2x += bWidth;

				//Si los botones se acercan al borde, saltamos de linea
				if (b2x >= screenWidth-bWidth*2) { 
					b2x = screenWidth-(anchoMenuPrincipal-Margen);
					b2y += bWidth;
				}
			}
		}

		//Si estamos en TERCERA PERSONA
		else if (camaraActual == camaras.terceraPersona) {

			//Mostramos la cara del payo actual
			mostrarCara(b1x+Margen/3, b1y+bWidth*2);
			/**************************************
			//BOTON DE ACCION: PROTESTAR   (  P  )  [Tercera persona]
			***************************************/				
			//Dibujamos el boton de protesar, o con F4, los manifestantes empiezan a cantar
			if (GUI.Button (new Rect(b2x, b1y, bWidth, bWidth), botonProtestar, topButtonStyle) || teclaP) {
				teclaP = false;
				comenzarAProtestar();

				//Si no esta cantando, lo ponemos a cantar. 
				personaTerceraPersona.GetComponent<Animator>().SetBool("Protestando", true);
				personaTerceraPersona.GetComponent<UnitManager>().EstaCantando(true);
				Manager.temp.Cantar();
			}
			//Info PROTESTAR:
			if (Input.mousePosition.x > b2x && screenHeight - (Input.mousePosition.y )> b1y 
				&& Input.mousePosition.x < b2x + bWidth 
			     && screenHeight - (Input.mousePosition.y) < b1y + bWidth)
				dibujarEtiqueta("Comenzar a cantar\n y Protestar (P)", 135, 30);

			//Incrementamos la posicion para el siguiente boton.
			b2x += bWidth;

			//Si los botones se acercan al borde, saltamos de linea
			if (b2x >= screenWidth - bWidth * 2) { 
				b2x = screenWidth - (anchoMenuPrincipal - Margen);
				b2y += bWidth;
			}									

			/**************************************
			//BOTON DE ACCION: PONER MUSICA   ( R )
			 ***************************************/
			//Si la persona tiene una radio en la mano
			if (personaTerceraPersona.GetComponent < UnitManager>().tieneMusica) {
				if (GUI.Button (new Rect(b2x, b2y, bWidth, bWidth), botonBailar , topButtonStyle) || teclaR) {
					teclaR = false;

					//Hacemos que el manifestante reproduzca o deje de reproducir su musica. 
					personaTerceraPersona.gameObject.GetComponent<UnitManager>().estaReproduciendoMusica = !personaTerceraPersona.gameObject.GetComponent<UnitManager>().estaReproduciendoMusica;
				}
				
				//Info MUSICA:
				if (Input.mousePosition.x > b2x && screenHeight - (Input.mousePosition.y) > b2y 
				    && Input.mousePosition.x < b2x + bWidth && screenHeight - (Input.mousePosition.y) < b2y + bWidth)
					dibujarEtiqueta("Reproducir Musica (R)", 135, 15);
				
				//Incrementamos la posicion para el siguiente boton.
				b2x += bWidth;

				//Si los botones se acercan al borde, saltamos de linea
				if (b2x >= screenWidth-bWidth * 2) { 
					b2x = screenWidth - (anchoMenuPrincipal - Margen);
					b2y += bWidth;
				}									
			}

		}

		/********************************
		//  BOTON DE ACCION: FREE (libre)
		********************************/
		//Dibujamos el boton de Libre, con una F(ree), este servira para que la camara este libre o en modo helicoptero.
		//Solo si estamos siguiendo al lider o en modo tercera persona
		if (camaraActual == camaras.lider || camaraActual == camaras.terceraPersona) {

			//Ponemos el cursor a normal
			cursorState = CursorState.normal;

			//Si hay mas de un manifestante seleccionado, tambien salimos del modo tercera persona.
			if (GUI.Button (new Rect(b2x, b2y, bWidth, bWidth), botonFree, topButtonStyle) || teclaF || cuantos > 1 )	{
				if (camaraActual == camaras.terceraPersona) 				
					saliendoTerceraPersona(personaTerceraPersona);			
				camaraActual = camaras.free;
				teclaF = false;
			}
			//Info ( F ):
			if (Input.mousePosition.x > b2x && screenHeight - (Input.mousePosition.y) > b2y 
			    && Input.mousePosition.x < b2x + bWidth && screenHeight - (Input.mousePosition.y) < b2y + bWidth)
				dibujarEtiqueta("Camara Libre/Free (F)", 125, 15);

			//Incrementamos la posicion para el siguiente boton.
			b2x += bWidth;

			//Si los botones se acercan al borde, saltamos de linea
			if (b2x >= screenWidth - bWidth * 2) { 
				b2x = screenWidth - (anchoMenuPrincipal - Margen);
				b2y += bWidth;
			}

		}

		/*******************************************
		//BOTON DE ACCION: TERCERA PERSONA   (  T  )		
		*******************************************/
		//Pasar a manejar al manifestante directamente con WASD, apareceran las acciones contextuales. 
		if (cuantos == 1 && camaraActual != camaras.terceraPersona) {

			//Dibujamos el boton de Tercera Persona,  para que la camara siga en 3ª persona a la unidad seleccionada.
			if (GUI.Button (new Rect(b2x, b2y, bWidth, bWidth), botonTerceraPersona, topButtonStyle) || teclaT) {
				//Control por teclado de la tecla T
				teclaT = false;
				entrarTerceraPersona();		
			}

			//Info T:
			if (Input.mousePosition.x > b2x+bWidth * 3 && screenHeight - (Input.mousePosition.y) > b2y 
			    && Input.mousePosition.x < b2x + bWidth && screenHeight - (Input.mousePosition.y) < b2y + bWidth)
				dibujarEtiqueta("Control en tercera\npersona (T)", 135, 30);

			//Incrementamos la posicion para el siguiente boton.
			b2x += bWidth;

			//Si los botones se acercan al borde, saltamos de linea
			if (b2x >= screenWidth-bWidth * 2) { 
				b2x = screenWidth-(anchoMenuPrincipal - Margen);
				b2y += bWidth;
			}
		}

	}

	/******************************************
	 * BOTONES PLAY, PAUSE Y DOBLE VELOCIDAD
	 * ****************************************/
	private void BotonesVelocidad () {

		if (camaraActual != camaras.terceraPersona && mostrarControlesTiempo ) {
			float posY = screenHeight - bWidth * 2, posX = bWidth * 2; 

			//Play
			if (GUI.Button (new Rect(posX, posY, bWidth, bWidth), play, topButtonStyle))
				Time.timeScale = 1f;

			//Control de etiqueta hover
			if (Input.mousePosition.x > posX && screenHeight - (Input.mousePosition.y) > posY  
			    && Input.mousePosition.x < posX + bWidth && screenHeight - (Input.mousePosition.y) < posY + bWidth)
				GUI.Label (new Rect(posX + bWidth, screenHeight - bWidth, 130, 15), 
				           "Velociad normal: 1x", infoHoverStyle);

			//Pause
			if (GUI.Button (new Rect(posX + bWidth * 2, posY, bWidth, bWidth), pause, topButtonStyle))
				Time.timeScale = 0f;

			//Control de etiqueta hover
			if (Input.mousePosition.x > posX + bWidth * 2 && screenHeight - (Input.mousePosition.y) > posY  
			    && Input.mousePosition.x < posX + bWidth * 3 && screenHeight - (Input.mousePosition.y) < posY + bWidth)
				GUI.Label (new Rect(posX + bWidth, screenHeight - bWidth, 100, 15), 
				           "Juego en pausa.", infoHoverStyle);

			//Doble
			if (GUI.Button (new Rect(posX + bWidth * 4, posY, bWidth, bWidth), doble, topButtonStyle))
				Time.timeScale = 1.5f;
			if (Input.mousePosition.x > posX + bWidth * 4 && screenHeight - (Input.mousePosition.y) > posY  
			    && Input.mousePosition.x < posX + bWidth * 5 && screenHeight - (Input.mousePosition.y) < posY + bWidth)
				GUI.Label (new Rect(posX + bWidth, screenHeight - bWidth, 150, 15), 
				           "Subir velocidad: 1.5x", infoHoverStyle);

			//Etiqueta Velocidad actual de juego
			GUI.Label (new Rect(posX, screenHeight - bWidth * 3, 130, 15), 
			                    "Velocidad actual:" + Time.timeScale.ToString() +"x", 
			                    manifestLabel);
		}

	}

	/******************************************************
	* 			BOTON DERECHO
	* ****************************************************/
	private void BotonDerechoControl () {

		//Si al pulsar el boton derecho estamos sobre un objeto interactuable, lo indicamos
		if (Input.GetMouseButtonDown(1))
			if (cursorState == CursorState.attack) 
				mouseSobreObjetivo = true;

		//Si presionamos con el boton derecho sobre la pantalla, cambiamos el cursor,
		// para indicar que podemos mover la camara (Desactivado)
		//if (Input.GetMouseButton (1))
		//	cursorState = CursorState.inter;
		
		//CONTROL DEL BOTON DERECHO, IR CORRIENDO A UN PUNTO
		if (Input.GetMouseButtonUp (1) && cuantos > 0 && camaraActual!=camaras.terceraPersona && 
			MouseEnZonaJuego() && !mouseSobreObjetivo) {

			//Posiciones relativas al lugar donde se pulso el boton derecho...
			menuBtnDerPos = Input.mousePosition;

			//Inicia la acción de ir corriendo a...
			BotonDerecho();		
		}

		//Atacar // Acción directa sobre objetivo
		//Si el mouse esta sobre un objetivo, mostramos las opciones de interaccion con dicho objetivo
		else if (mouseSobreObjetivo) {

	    		//Si se pulsa sobre un objetivo con el botón derecho, se ataca a ese objetivo
	    		if (Input.GetMouseButtonUp(1)) {
	    			if (clickBtnIz) {
					menuBtnDerOn = false;
					mouseSobreObjetivo = false;
					if (objetivoInteractuar.layer == LayerMask.NameToLayer("Coches"))
						IniciarDisturbios(objetivoInteractuar.transform.parent);
					else
						IniciarDisturbios(objetivoInteractuar.transform);

					clickBtnIz = false;
				}
				else
					clickBtnIz = true;
			}
		}

	}


	//***************************************************************************
	//						IMAGEN DEL CURSOR
	//***************************************************************************
	// © 2013 Brett Hewitt All Rights Reserved
	private void ImagenCursor () {

	cursorTimer += Time.deltaTime;

		if (Event.current.type.Equals(EventType.Repaint))
		{
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

	}

	/*************************************
	* 		CURSORES MOUSE
	*************************************/
	//Modificación de los cursores, dependiendo sobre que esté el ratón
	private void CursoresMouse(int cuantos) {

		/*****************************************************
		 *  CURSORES DEL MOUSE Y SELECCION DE UNIDADES SUELTAS
		 * ***************************************************/
		//Si la camara no esta en tercera persona, lanzamos un raycast 
		//desde la posicion del mouse, para adaptar el puntero, dependiendo del objeto que estemos apuntando
		//Y si no estamos sobre del main menu...
		if (camaraActual != camaras.terceraPersona && !menuBtnDerOn && MouseEnZonaJuego()) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);			
			RaycastHit hit;

			//Debug Rayhit
			if (debug) {
				Debug.DrawRay (ray.origin, ray.direction * 90, Color.white);		
			}
			
			//Por defecto suponemos que el raton no esta sobre un objetivo
			mouseSobreObjetivo = false;
			if (Physics.Raycast(ray,out hit)) {

				//Partimos de un cursor normal y lo modificamos si hace falta
				cursorState = CursorState.normal;

				//Preguntamos por los TAG del objeto colisionado
				string tagit = hit.collider.tag;
				
				//Si el raton esta sobre un Manifestante
				if (tagit == "Manifestantes") {

					//Cambiamos la forma del cursor: Seleccionar
					cursorState = CursorState.hover;
					
					//Seleccion multiple de unidades
					if (Input.GetMouseButtonDown(0) && Input.GetKey (KeyCode.LeftShift)){

						//Si pulsamos Shift, añadimos unidades a la seleccion
						if (!hit.collider.gameObject.GetComponent<selected>().isSelected)
							seleccionarUnidad(hit.collider.gameObject);
						else
							desseleccionarUnidad(hit.collider.gameObject);						
					}

					//Seleccion de una sola unidad
					else if (Input.GetMouseButtonUp(0))
					{
						selectedManager.temp.deselectAll();
						seleccionarUnidad(hit.collider.gameObject);							
					}										
				}
				//Cursor sobre objetivos 
				else if (tagit == "Policias" || tagit == "Peatones" || tagit == "Contenedores" 
					|| tagit == "Coches" || tagit == "Destruible") {

					//Si hay manifestantes seleccionados
					if (cuantos > 0) {

						//Cambiamos la forma del cursor a 'objetivo'
						cursorState = CursorState.attack;

						//Guardamos el objeto para una posible interaccion
						objetivoInteractuar = hit.collider.gameObject;
						mouseSobreObjetivo = true;
					}
				}
				//Cursor Ir A...
				else if (tagit == "Suelo" && cuantos > 0 ) 					
					cursorState = CursorState.go;
			}
		}		
		else 
			cursorState = CursorState.normal;

	}

	//*******************************************************************************************
	//									ARRASTRE DEL RATON
	//*******************************************************************************************
	private void ArrastreRaton () {

		if (Input.GetMouseButtonDown(0)) {

			//Cuando se presiona con el boton iz dentro del area de juego, 
			//iniciamos las variables de arrastre y bloqueamos el movimiento de bordes de la pantalla.
			if (MouseEnZonaJuego())
			{
				dragStart = Input.mousePosition;
				CamaraAerea.temp.movimientoBordes (false);
				mouseX = Input.mousePosition.x;
				mouseY = Input.mousePosition.y;	
				if (debug)
					Debug.Log("Iniciando arrastre");
			}

			//Click fuera del area de juego
			else
			{
				disableOnScreenMouse = true;
			}
		}

		//Si se esta presionando el boton iz, actualizamos las variables de arrastre.
		else if (Input.GetMouseButton (0) && !disableOnScreenMouse)	{
			dragEnd = Input.mousePosition;

			//Si nos salimos del area de juego, el limite es el ultimo punto de arrastre
			if (dragEnd.x > screenWidth-anchoMenuPrincipal) dragEnd.x = screenWidth-anchoMenuPrincipal;

			//Si el raton se desplaza mas de 4 pixels, con el boton pulsado, consideramos que esta arrastrando.
			if (!(Mathf.Abs(Input.mousePosition.x-mouseX) < 4 && Mathf.Abs (Input.mousePosition.y-mouseY) < 4))
			{
				mouseDrag = true;
			}			
		}

		//Al soltar el boton izquierdo 
		else if (Input.GetMouseButtonUp (0)) {
			disableOnScreenMouse = false;

			//Si estábamos arrastrando, finalizamos el arrastre
			if (mouseDrag) {
				if (!clickBtnIz)
					clickBtnIz = true;
				else {
					mouseDrag = false;

					//Volvemos a activar el movimiento por margenes de pantalla
					CamaraAerea.temp.movimientoBordes (true);
					clickBtnIz = false;
					tiempoClick = 0;
				}
			}
			else {

				//Si hay manifestantes seleccionados y se pincha en el suelo con el botón izquierdo, las 
				//unidades seleccionadas se moverán hacia ahí
				if (selectedManager.temp.objects.Count > 0 && !menuBtnDerOn && MouseEnZonaJuego()) {
					if (tiempoClick < 0.1)
						tiempoClick += Time.deltaTime;
					else {
						Ray rayo = Camera.main.ScreenPointToRay(Input.mousePosition);
						RaycastHit hitPoint;
						Physics.Raycast(rayo,out hitPoint);
						IrA(hitPoint);
						clickBtnIz = false;
						menuBtnDerOn = false;
						if ( debug )
							Debug.Log("Click sobre: " + hitPoint.collider.name);

					}
				}
				/*
				//Cerramos el menuBtnDer, si estaba abierto y se hace click_Iz, en otro punto de la pantalla
				if (Input.GetMouseButtonUp (0) && menuBtnDerOn) {
					if (clickBtnIz){
						menuBtnDerOn = false;				
						clickBtnIz = false;
					}
					else
						clickBtnIz = true;
				}*/
			}			
		}


		//***********************************************
		//  SELECCION DE UNIDADES POR ARRASTRE
		//**********************************************
		if (mouseDrag)	{

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
			
			//Actualizamos en tiempo real la posicion de arrastre, para que el UnitManager
			//pueda saber si su unidad ha sido seleccionada.
			dLoc[0] = new Vector3(Mathf.Min(selectBoxStart.x, selectBoxEnd.x), Mathf.Min (selectBoxStart.y, selectBoxEnd.y), 0);
			dLoc[1] = new Vector3(Mathf.Max(selectBoxStart.x, selectBoxEnd.x), Mathf.Max (selectBoxStart.y, selectBoxEnd.y), 0);
			
			//Dibujamos el cuadro de seleccion. 
			GUI.Box (new Rect(selectBoxStart.x, screenHeight-selectBoxStart.y, selectBoxEnd.x-selectBoxStart.x, selectBoxStart.y-selectBoxEnd.y), "", dragBoxStyle);			
		}

	}
		
	/* ********************
	 *   El FONDO DEL GUI
	 * ********************/
	private void FondoGUI() {
		if (Event.current.type.Equals(EventType.Repaint))
		{		
			//Dibujamos el fondo lateral del menu, en negro.
			Graphics.DrawTexture(new Rect(screenWidth-anchoMenuPrincipal, screenHeight/4, anchoMenuPrincipal, 3*screenHeight/4), fondoGUI);			
			
			//GUI background above mini map
			Graphics.DrawTexture(new Rect(screenWidth-anchoMenuPrincipal, 0, anchoMenuPrincipal, 
			                              (screenHeight/4)-(miniMapBounds[2] - miniMapBounds[3])), fondoGUI);
			
			//GUI background right of mini map
			Graphics.DrawTexture(new Rect(miniMapBounds[1], 0, screenWidth-miniMapBounds[1], (screenHeight/4)), fondoGUI);
			
			//GUI background left of mini map
			Graphics.DrawTexture(new Rect(miniMapBounds[0]-(screenWidth-miniMapBounds[1]), 0, screenWidth-miniMapBounds[1], 
			                              (screenHeight/4)), fondoGUI);
		}
	}

	/*****************************
	 * DUBIJAR ETIQUETAS OVER
	 * **************************/
	private void dibujarEtiqueta(string cadena, int ancho, int alto) {

		mostrarEtiqueta = true;
		anchoEtiqueta = ancho;
		altoEtiqueta = alto;
		cadenaEtiqueta = cadena;

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
	 *   ENTRANDO EN TERCERA PERSONA
	 * **********************************/
	private void entrarTerceraPersona() {

		//Almacenamos en la variable 'personaTerceraPersona' al manifestante seleccionado
		personaTerceraPersona = selectedManager.temp.objects[0];

		//Para evitar que se tumbe...
		personaTerceraPersona.transform.rotation = Quaternion.Euler(0, personaTerceraPersona.transform.rotation.y, 0); 
		camaraActual = camaras.terceraPersona;

		//Activamos el control en primera persona del manifestante
		selectedManager.temp.objects[0].GetComponent<ComportamientoHumano>().terceraPersona = true;
		selectedManager.temp.objects[0].GetComponent<UnitManager>().terceraPersona = true;

		//Encontramos la camara de seguimiento personal y la unimos al manifestante seleccionado
		Camera cPersonal;
		cPersonal = Manager.temp.personaCamera1;

		//Hacemos que la musica, los canticos y los efectos sean los mismos que en la camara anteior
		Manager.temp.audioFxPersonal.clip = Manager.temp.audioFx.clip;	
		Manager.temp.audioMusicaPersonal.clip = Manager.temp.audioMusica.clip;		
		Manager.temp.audioCanticosPersonal.clip = Manager.temp.audioCanticos.clip;		
		Manager.temp.audioCanticosPersonal.volume = Manager.temp.audioCanticos.volume * 2;
		if (Manager.temp.estadoFx())	
			Manager.temp.audioFxPersonal.Play();		
		if (Manager.temp.estadoMusica())	
			Manager.temp.audioMusicaPersonal.Play();		
		if (Manager.temp.EstadoCanticos())			
			Manager.temp.audioCanticosPersonal.Play();	

		//Y ponemos de Main a la CamaraPersonal
		cPersonal.depth = 5;

		//La adjuntamos al manifestante para que se mueva con el
		cPersonal.gameObject.transform.parent = personaTerceraPersona.transform;

		//Ajustamos la posicion y la rotacion, con respecto al manifestante 
		cPersonal.transform.rotation = personaTerceraPersona.transform.rotation;

		//La camara mira hacia el manifestante
		cPersonal.transform.forward = personaTerceraPersona.transform.forward;
		cPersonal.gameObject.transform.position = personaTerceraPersona.transform.position 
												+ personaTerceraPersona.transform.up * 4 
												//+ personaTerceraPersona.transform.right * 3 
												- personaTerceraPersona.transform.forward * 3;

		//Activamos el audio listener y se lo desactivamos a la camara principal
		cPersonal.GetComponent<AudioListener>().enabled = true;
		Manager.temp.mainCamera.GetComponent<AudioListener>().enabled = false;	

		//Desactivamos el control de la cámara
		Manager.temp.controlCamara.GetComponent<FPSInputController>().enabled = false;


		//Eliminamos la seleccion de todo manifestante
		selectedManager.temp.deselectAll();

	}

	/************************************
	 *   SALIENDO DE TERCERA PERSONA
	 * **********************************/
	public void saliendoTerceraPersona(GameObject manifestante) {

		//Salimos de tercera persona
		personaTerceraPersona.GetComponent<ComportamientoHumano>().terceraPersona = false;
		personaTerceraPersona.GetComponent<UnitManager>().terceraPersona = false;

		//El manifestante comienza a caminar
		personaTerceraPersona.GetComponent<Animator>().SetFloat("Speed",personaTerceraPersona.GetComponent<UnitManager>().prisa);

		//Desactivamos la camara personal.
		Camera cPersonal = Manager.temp.personaCamera1;
		cPersonal.depth = 1;

		//Activamos el audio listener en la camara principal
		cPersonal.GetComponent<AudioListener>().enabled = false;
		Camera.main.GetComponent<AudioListener>().enabled = true;

		//Hacemos que la musica y los efectos sean los mismos que en la camara anteior		
		Manager.temp.audioFxPersonal.Stop();
		Manager.temp.audioMusica.clip = Manager.temp.audioMusicaPersonal.clip;		
		Manager.temp.audioMusicaPersonal.Stop();		

		//Y hacemos que la camara mire hacia el
		Vector3 protaPosition = personaTerceraPersona.transform.position;
		Camera.main.transform.position = new Vector3(protaPosition.x-20, Camera.main.transform.position.y, protaPosition.z-30);
		Camera.main.transform.LookAt (protaPosition);

		//Re-Activamos el control de la cámara
		Manager.temp.controlCamara.GetComponent<FPSInputController>().enabled = true;

	}
	
	/************************************
	 *   INICIANDO DISTURBIOS
	 * **********************************/
	private void IniciarDisturbios(Transform objetivo) {			
			int cont  = selectedManager.temp.objects.Count-1;

			//Recorremos la lista desde el final hasta el principio[0], eliminanado a los 'no bastante activistas'
			while (cont > -1) {
				GameObject g = selectedManager.temp.objects[cont].gameObject;

				//Definimos el valor minimo y el activismo minimo, para unirse a los disturbios
				int minimoActivismoNecesatio = 20;
				int minimoValorNecesatio = 10;
				if (g.GetComponent<UnitManager>().activismo < minimoActivismoNecesatio 
				    || g.GetComponent<UnitManager>().valor < minimoValorNecesatio) {	

					//Si no es lo bastante activo, se le quita de la seleccion. 
					selectedManager.temp.objects.Remove(g);					

					//Si es el ultimo de la lista, reducimos el contador
					if (cont == selectedManager.temp.objects.Count) 
						cont --;
				}
				else //Decrementamos contador si no se borra, porque si se borra la lista se reduce sola.
					cont --;
			}
			
			//reiniciamos el contador para saber cuantos quedan
			cont = 0;

			//Los manifestantes que quedan seleccionados empiezan a atacar. 
			foreach (GameObject g in selectedManager.temp.objects) {
				g.GetComponent<UnitManager> ().estaAtacando = true;
				g.GetComponent<UnitManager> ().objetivoInteractuar = objetivo;

				//Por cada manifestante que ataca, sube la barra de ambiente +10
				Manager.temp.IncAmbiente(10);
				cont ++;
			}	

			//Informamos del suceso
			Manager.temp.sucesos.Add (cont.ToString() + " manifestantes han comenzado disturbios.");

		}

	//BARRAS de estado general de la mani.
    private void DibujarBarrasAmbiente() {

		//Definimos el color de fondo y del texto de las barras
		ambienteBarStyle.normal.background = greenTexture;
		concienciaBarStyle.normal.background = redTexture;
		repercusionBarStyle.normal.background = redTexture;
		ambienteBarStyle.normal.textColor = Color.white;
		concienciaBarStyle.normal.textColor = Color.white;
		repercusionBarStyle.normal.textColor = Color.white;
				
		//Definimos los estilos del log de sucesos/objetivos
		blueLogStyle.normal.textColor = Color.green;
		yellowLogStyle.normal.textColor = Color.yellow;
		normalLogStyle.normal.textColor = Color.white;
		blueLogStyle.fontSize = screenWidth / 100;
		yellowLogStyle.fontSize = screenWidth / 100;
		normalLogStyle.fontSize = screenWidth / 100;

		//Obtenemos los valores de las barras de nivel
		float tempPower = Manager.temp.GetAmbiente();
		float tempConciencia = Manager.temp.GetConciencia();
		float tempRepercusion = Manager.temp.GetRepercusion();
		int posX, posY;
		

		//Si la barra de conciencia baja de cero, la ponemos en positivo, para que se dibuje bien, igual... Revisar.
		if (tempConciencia < 0)
			tempConciencia *= -1;

		//Dibujamos la barra de ambiente en la manifestacion.
		posX = Mathf.RoundToInt(screenWidth - anchoMenuPrincipal) + Margen;
		posY = (screenHeight / 4) + Margen  *2;
		GUI.Box (new Rect(posX, posY, (anchoMenuPrincipal / repercusionMaxima) * 
			tempPower, (screenHeight / 32)), "Ambiente", ambienteBarStyle);

		//info
		if (Input.mousePosition.x > posX && screenHeight - (Input.mousePosition.y) > posY 
			&& Input.mousePosition.x < posX + anchoMenuPrincipal 
		     && screenHeight - (Input.mousePosition.y) < posY + (screenHeight / 32))
			dibujarEtiqueta("Como de caldeado esta\nel ambiente en la manifestacion.", 190, 30);
		
		//Dibujamos la barra de Conciencia Local
		posY = (screenHeight / 4) + Margen * 4;
		GUI.Box (new Rect(posX, posY, (anchoMenuPrincipal/ambienteMaxima) * 
			tempConciencia, (screenHeight / 32)), "Conciencia Local", concienciaBarStyle);

		//info
		if (Input.mousePosition.x > posX && screenHeight - (Input.mousePosition.y) > posY 
			&& Input.mousePosition.x < posX+anchoMenuPrincipal 
		     && screenHeight - (Input.mousePosition.y) < posY + (screenHeight / 32))
			dibujarEtiqueta("Como de concienciada esta\nla gente de esta ciudad.", 190, 30);
		
		//Dibujamos la barra de Impacto Mediatico
		posY = (screenHeight / 4) + Margen * 6;
		GUI.Box (new Rect(posX, posY, (anchoMenuPrincipal / concienciaMaxima) * 
			tempRepercusion, (screenHeight / 32)), "Repercusion Mediatica", repercusionBarStyle);

		//info
		if (Input.mousePosition.x > posX && (screenHeight - (Input.mousePosition.y)) > posY
			&& Input.mousePosition.x < posX+anchoMenuPrincipal 
		     && (screenHeight-(Input.mousePosition.y)) < posY + (screenHeight / 32))
			dibujarEtiqueta("Cual es el impacto\nmediatico de esta manifestacion.", 190, 30);

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
			UnitManager uM = g.GetComponent<UnitManager>();

			//Si pasamos el raton por encima de la cara mostramos un label conlo que tiene en las manos
			if (Input.mousePosition.x > xact - Margen && Input.mousePosition.x < xact+Margen + ancho
			    && Input.mousePosition.y > screenHeight - yact - alto - Margen && Input.mousePosition.y < screenHeight - yact)
				dibujarEtiqueta(uM.nombre + "\n" + uM.manoIzquierda.name + "\n" + uM.manoDerecha.name, 145, 45);

			//Dibujamos la cara como un boton para seleccionar solo a ese manifestante.
			//Genera un error. Desafio: resolverlo. 
			if (GUI.Button (new Rect(xact, yact, ancho, alto),uM.cara)){
				selectedManager.temp.deselectAll();
				selectedManager.temp.addObject (g);
			}

			//Revisar!!!
			try{
			//Dibujamos las manos como botones, pero con estilo de 'sin accion' //Desafio, no se usan como boton.
			GUI.Button(new Rect(xact, yact+alto-(alto/5), ancho/3, alto/5),uM.manoIzquierda.GetComponent<ObjetoDeMano>().imagenMiniatura,itemButtonStyle);
			GUI.Button(new Rect(xact+ancho-(ancho/3), yact+alto-(alto/5), ancho/3, alto/5),uM.manoDerecha.GetComponent<ObjetoDeMano>().imagenMiniatura,itemButtonStyle);
			}catch{}

			//Dibujamos las barras de valor y activismo.
			dibujarBarras(xact+5, yact+5, filas,uM.valor,g.GetComponent<UnitManager>().activismo);

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
		UnitManager uM;

		//Los datos los extraemos de sitios distintos dependiendo de si es un manifestante seleccionado 
		//o estamos en tercera persona
		if (camaraActual == camaras.terceraPersona)
			uM = personaTerceraPersona.GetComponent<UnitManager>();
		else
			uM = selectedManager.temp.objects[0].GetComponent<UnitManager>();

		//Posicionamiento de la cara 
		float alto, ancho;
		alto = Margen*6;
		ancho = Margen*8;
		GUI.Button (new Rect(xini, yini, alto, ancho),uM.cara);

		//******
		// BARRAS DE ESTADO DE LAS PERSONAS 
		//******
		// Molaba que el mensaje que apareciese al poner el raton por encima fuera especifico para cada rango de la barra de estado: "animado" "excitado" "asustado", tec
		// Dibujamos las barras de valor y activismo de la o las personas seleccionadas. Revisar!
		xini = screenWidth - (anchoMenuPrincipal) + Margen * 2.5f;
		yini = screenHeight - (screenHeight / 2.0f) + Margen * 2f - 2f;
		dibujarBarras (xini,yini,1,uM.valor, uM.activismo);

		//Dibujamos las manos como botones, pero con estilo de 'sin accion'
		GUI.Button(new Rect(xini, yini + alto - (alto / 7f), ancho, alto / 3f),
			uM.manoIzquierda.GetComponent<ObjetoDeMano>().imagenMiniatura,itemButtonStyle);
		GUI.Button(new Rect(xini + ancho - (ancho / 1.8f), yini + alto - (alto / 7f), ancho, alto / 3f),
			uM.manoDerecha.GetComponent<ObjetoDeMano>().imagenMiniatura, itemButtonStyle);

		//Estado del manifestante
		string estado, activista, fichado;		
		if (uM.estaCantando) 
			estado = "Protestando";
		else if (uM.estaAtacando) 
			estado = "Atacando";
		else if (uM.estaQuemando)
			estado = "Quemando";
		else if (uM.estaPintando)
			estado = "Pintando";
		else if (uM.estaLanzando)
			estado = "Lanzando objeto";
		else if (uM.estaBailando) 
			estado = "Bailando";
		else if (uM.estaReproduciendoMusica) 
			estado = "Reproduciendo musica";
		else if (uM.tiempoCorriendo > 0) 
			estado = "Corriendo";
		else if (uM.estaParado) 
			estado = "Parado";			
		else
			estado = "Caminando";			

		if (uM.activismo >= Manager.temp.activismoActivista)
			activista = "Si";
		else			
			activista = "No";
		if (uM.estaFichado)
			fichado = "Si";
		else			
			fichado = "No";

		//Datos personales
		GUI.Label (new Rect(xini - ancho / 6, yini + Margen * 8, anchoMenuPrincipal / 3, Margen * 2), "Nombre: "+ uM.nombre, manifestLabel);
		GUI.Label (new Rect(xini - ancho / 6, yini + Margen * 9.5f, anchoMenuPrincipal / 3, Margen * 2), "Estado: "+ estado, manifestLabel);
		GUI.Label (new Rect(xini - ancho / 6, yini + Margen * 10.5f, anchoMenuPrincipal / 3, Margen * 2), "Es activista: " + activista, manifestLabel);
		GUI.Label (new Rect(xini - ancho / 6, yini + Margen * 11.5f, anchoMenuPrincipal / 3, Margen * 2), "Esta fichado: " + fichado, manifestLabel);
		GUI.Label (new Rect(xini-ancho / 6, yini + Margen * 12.5f, anchoMenuPrincipal / 3, Margen * 2), "Energia: "+ uM.energia.ToString(), manifestLabel);

		try {
			GUI.Label (new Rect(xini-ancho / 6, yini + Margen * 14, anchoMenuPrincipal / 3, Margen * 2), "Viene con: "+ uM.empatiaPersona.GetComponent<UnitManager>().nombre, manifestLabel);
			if (GUI.Button (new Rect(xini+ancho / 4, yini + Margen * 15, ancho / 4f, alto / 3f),uM.empatiaPersona.GetComponent<UnitManager>().cara)){
				selectedManager.temp.deselectAll();
				selectedManager.temp.addObject (uM.empatiaPersona);
			}
		}
		catch{
				GUI.Label (new Rect(xini-ancho/6, yini+Margen*14, anchoMenuPrincipal/3, Margen*2), "Viene solo");
		}
	}

	/******************************************************
	 * Barras de estado de los manifestantes seleccionados
	 * ***************************************************/
	private void dibujarBarras(float xini, float yini, float escala, float valor, float activismo) {

		string valorTxt="", activismoTxt="";

		//En el tamaño grande mostramos los textos
		if (escala == 1) {
			if (valor > 0)
				valorTxt = "Valor";
			else
				valorTxt = "Miedo";
			activismoTxt="Activismo";
		}

		//Dibujamos la barra de 'activismo' que ira de 0 a 100
		if (activismo > 0)
			GUI.Box (new Rect(xini, yini - (Margen / escala) * 2, 
					activismo / escala, (Margen / 1.5f)/escala), activismoTxt, concienciaBarStyle);
		else
			GUI.Box (new Rect(xini, yini - (Margen / escala) * 2, 
					activismo / escala, (Margen / 1.5f)/escala), activismoTxt, concienciaBarStyle);

		//Incrementamos la posición del xini, para centrar la barra de valor/miedo, en escala 1		
		xini += (Margen * 2.5f)/escala;

		//Dibujamos la barra de 'valor', que ira de -50 a +50
		if (valor > 0)
			GUI.Box (new Rect(xini, yini - (Margen / escala), 
					valor / escala, (Margen / 1.5f)/escala), valorTxt, ambienteBarStyle);
		else if (valor < 0)
			GUI.Box (new Rect(xini + valor*2 / escala, yini - (Margen / escala), 
					(valor * -2f) / escala , (Margen / 1.5f)/escala), valorTxt, repercusionBarStyle);

	}

	/******************************************************
	//Funcion para generar texturas de un color en concreto
	*******************************************************/
	public Texture2D makeColor(float R, float G, float B, float A)
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

	//Seleccionamos la unidad
	public void seleccionarUnidad(GameObject g)
	{
		selectedManager.temp.objects.Add (g);
		g.GetComponent<selected>().isSelected = true;
	}

	//Desseleccionamos la unidad
	public void desseleccionarUnidad(GameObject g)
	{
		selectedManager.temp.objects.Remove (g);
		g.GetComponent<selected>().isSelected = false;
	}

	/****************
	 * LOG DE SUCESOS
	 * *************/
	private void DibujarLog() {

		//Cada vez que se repinte la pantalla
		if (Event.current.type.Equals(EventType.Repaint)) {
			int alto = Screen.height / 4;
			int ancho = Screen.width / 3;
			int cont = 0, maxMensajes = 11;

			//Dibujamos la caja del log. Desafío: añadir un scroll y quitar maxMensajes
			GUI.BeginGroup(new Rect(0, 0, ancho, alto));
			GUI.Box(new Rect(0, 0, ancho, alto), "Sucesos y objetivos de la manifestacion: ");

			//Mostramos todos los objetivos en rojo
			foreach (string mensaje in Manager.temp.objetivos) {
				GUI.Label (new Rect(10, (cont + 1) * 20, ancho, 20), mensaje, blueLogStyle);
				cont++; 
			}

			//Mostramos los ultimos sucesos, del final al principio de la lista
			for (int pos = Manager.temp.sucesos.Count - 1; pos >= 0 ; pos --) {
				if (cont < maxMensajes) {

					//Mostramos los mensajes, si es un objetivo cumplido en amarillo, si no normal. 
					if (Manager.temp.sucesos[pos].Contains(Manager.temp.objetivoConseguido))
						GUI.Label (new Rect(10, (cont + 1) * 20, ancho, 20), 
								Manager.temp.sucesos[pos], yellowLogStyle);
					else
						GUI.Label (new Rect(10, (cont + 1) * 20, ancho, 20), Manager.temp.sucesos[pos], normalLogStyle);
				}
				else
					break;
				cont++;
			}
			GUI.EndGroup();
		}

	}

	//Todos los manifestantes seleccionados empiezan a protestar
	private void comenzarAProtestar() {

		//Para que se asigne una canción nueva
		Manager.temp.yaEstanCantando = false;

		//A cantar		
		Manager.temp.Cantar();		

		//Si no estan cantando o bailando, empiezan a cantar. 
		foreach (GameObject g in selectedManager.temp.objects) {					
			if (!g.GetComponent<UnitManager>().estaCantando && !g.GetComponent<UnitManager>().estaBailando) {
				g.GetComponent<UnitManager>().EstaCantando(true);
			}
		}

	}

	/****************************
	*  	PANTALLA RESUMEN
	*****************************/
	private void PantallaResumen (){

		//Rect R = new Rect(screenWidth / 3, screenHeight / 3, screenWidth / 3, screenHeight / 3);
		Rect R = new Rect(screenWidth / 10, screenHeight / 10, screenWidth - screenWidth / 3 + 30, 
					   screenHeight - screenHeight / 4);
		Rect R2 = new Rect(screenWidth / 8, screenHeight / 8, screenWidth - screenWidth / 3, 
					   screenHeight - screenHeight / 3);
		GUI.BeginGroup(R);
		GUI.Box(R, "Resumen la manifestacion: ");

		//Etiquetas dependiendo del final de la partida
		if (endGameWon)
			GUI.Label (R2, "¡MANIFESTACION TERMINADA COM EXITO!", endGameStyle);				
		else if (endGameLost)
			GUI.Label (R2, "LA MANIFESTACION \n HA SIDO DISUELTA", endGameStyle);

		//Resumen estadístico de la partida
		int  posX = screenWidth / 4, 
			posY = screenHeight / 3 + 25;		
		float puntuacionTotal = Manager.temp.GetRepercusion() + Manager.temp.GetConciencia() + Manager.temp.GetAmbiente();

		if (puntuacionTotal > Manager.temp.GetPuntuacionMaxima())
			Manager.temp.SetPuntuacionMaxima(puntuacionTotal);

		GUI.Label (new Rect(posX, posY, 300, 25), 
				"Maximo de manifestantes conseguido: " + Manager.temp.maximoDeManifestantes.ToString(), estadisticasStyle);
		GUI.Label (new Rect(posX, posY + 25, 300, 25), 
				"Numero de peatones convertidos: " + Manager.temp.peatonesConvertidos.ToString(), estadisticasStyle);
		GUI.Label (new Rect(posX, posY + 50, 300, 25), 
				"Numero de manifestantes detenidos: " + Manager.temp.numeroDeDetenidos.ToString(), estadisticasStyle);
		GUI.Label (new Rect(posX, posY + 75, 300, 25), 
				"Numero de civiles heridos: " + Manager.temp.numeroDeHeridos.ToString(), estadisticasStyle);
		GUI.Label (new Rect(posX, posY + 100, 300, 25), 
				"Numero de policias heridos: " + Manager.temp.numeroDePoliciasHeridos.ToString(), estadisticasStyle);
		GUI.Label (new Rect(posX, posY + 125, 300, 25), 
				"Repercusion Mediatica alcanzada: " + Manager.temp.GetRepercusion(), estadisticasStyle);
		GUI.Label (new Rect(posX, posY + 150, 300, 25), 
				"Conciencia Local alcanzada: " + Manager.temp.GetConciencia(), estadisticasStyle);
		GUI.Label (new Rect(posX, posY + 175, 300, 25), 
				"Ambiente Manifestacion alcanzado: " + Manager.temp.GetAmbiente(), estadisticasStyle);
		GUI.Label (new Rect(posX + 30, posY + 200, 300, 25), 
				"Puntuacion Total: " + puntuacionTotal.ToString() + " / Record: " + 
				Manager.temp.GetPuntuacionMaxima().ToString(), estadisticasStyle2);			

		GUI.Label (new Rect(posX, posY + 235, 350, 25), 
				"(Pulsa Barra Espaciadora para continuar...)");

		GUI.EndGroup();	

		Time.timeScale = 0f;

		//Mostrar menu con opciones: cargar partida, recomenzar o salir
		if (Input.GetKey(KeyCode.Space)) {
			//Finales narrados			
			if (endGameWon)
				Application.LoadLevel (5);
			else	if (Manager.temp.objetivoRecorrido && Manager.temp.objetivoGrafiti && Manager.temp.objetivoPeatones )
				Application.LoadLevel (4);
			else if (Manager.temp.GetAmbiente() > 500)
				Application.LoadLevel (3);
			else 
				Application.LoadLevel (0);
		}

	}

	// Reiniciar las variables del juego
	private void ReiniciarJuego() {

		estadoJuego = estadosJuego.mainMenu;
		endGameWon = false;
		endGameLost = false;

	}

	//Dibujamos una linea que muestra recorrido
	public void DibujarLineaRecorrido() {

		LineRenderer d1 = Manager.temp.puntoReunion.GetComponent<LineRenderer>();
		d1.SetVertexCount(Manager.temp.totalPuntosRecorrido);
		d1.SetPosition(0, Manager.temp.puntoReunion.transform.position);

		//Dibujamos una linea que nos indique el recorrido de la manifestacion
		for( int x = 1; x < Manager.temp.totalPuntosRecorrido; x++) {
			Vector3 pos = GameObject.Find("Destino" + x.ToString()).transform.position;
			d1.SetPosition(x,pos);
		}

	}

	/************************
	*   	ININCIAR MARCHA
	************************/
	//A todos los manifestantes se les asigna el mismo destino que el lider
	public void IniciarMarcha () {

		//Dibujamos las líneas del recorrido actual
		DibujarLineaRecorrido();

		//Localizamos el destino actual del
		Transform destino = Manager.temp.liderAlpha.GetComponent<ComportamientoHumano>().destino;
		int destinoActual = Manager.temp.liderAlpha.GetComponent<ComportamientoHumano>().destinoActual;

		//Ponemos a caminar a las unidades, a su velocidad inicial.
		foreach (GameObject g in Manager.temp.unidades) {
			if (g.tag == "Manifestantes") {
				g.GetComponent<UnitManager>().EnMovimiento(true);
				g.GetComponent<ComportamientoHumano>().moviendose = false;

				//Actualizamos los destinos de todos los manifestantes, para que sigan al LiderAlpha
				g.GetComponent<ComportamientoHumano>().destino = destino;
				g.GetComponent<ComportamientoHumano>().destinoActual = destinoActual;

				//Primer objetivo cumplido: iniciar la manifestacion
				if (g.GetComponent<UnitManager>().esLider && marchaIniciada == false) {
					marchaIniciada = true;
					Manager.temp.sucesos.Add ("[Conseguido] Selecciona a los manifestantes e inicia la marcha.");
					Manager.temp.objetivos.Remove ("[Objetivo] Selecciona a los manifestantes e inicia la marcha.");
					Manager.temp.IncRepercusion(50);
					Manager.temp.IncConciencia(50);							
				}
			}
		}

	}

	//Las unidades seleccionadas van hacia el punto hit
	private void IrA (RaycastHit hit){

		//Posicionamos un duplicado del objeto 'DestinoTemp' en el lugar del click. 
		//Y lo asignamos como destino de los manifestantes
		GameObject destinoT = GameObject.Find("DestinoTemp");
		GameObject destinoTemp = (GameObject)UnityEngine.Object.Instantiate(destinoT, hit.point, transform.rotation);
		destinoTemp.GetComponent<tempMesh>().quitaTextura = true;
		destinoTemp.transform.localScale = new Vector3(1,1,1);

		//Ponemos a caminar a las unidades, hacia el destino indicado
		foreach (GameObject g in selectedManager.temp.objects) {
			g.GetComponent<Animator>().SetFloat("Speed", g.GetComponent<UnitManager>().prisa);

			//Activamos el movimiento por orden directa, hacia el destino indicado
			g.GetComponent<UnitManager>().EnMovimiento(true);
			g.GetComponent<ComportamientoHumano>().moviendose = true;
			g.GetComponent<UnitManager>().StopAttack();
			g.GetComponent<ComportamientoHumano>().destinoTemp = destinoTemp.transform;
			g.transform.LookAt (destinoTemp.transform.position);
		}

	}

	private void detenerUnidadesSeleccionadas() {		

		//Detenemos a todas las unidades seleccionadas. 
		foreach (GameObject g in selectedManager.temp.objects) {
			UnitManager uManifestante = g.GetComponent<UnitManager>();

			//si no esta parado lo paramos
			if (!uManifestante.estaParado) {
				uManifestante.StopAttack();
				uManifestante.EnMovimiento(false);
			}

			//Si ya estaba parado, detenemos sus acciones
			else 
				uManifestante.StopAcciones();
		}

	}

	public bool MouseEnZonaJuego() {

		if (debug)
			Debug.Log ("Posicion limite: " + (screenWidth - anchoMenuPrincipal).ToString() + " / Posicion actual x: " + Input.mousePosition.x.ToString());

		return (Input.mousePosition.x < screenWidth - anchoMenuPrincipal); 

	}

	private void IrCorriendoA(RaycastHit hit) {

		//Aqui la velocidad a la que correran las unidades
		float velocidadCorriendo = 1f;
		
		//Posicionamos un duplicado del objeto 'DestinoTemp' en el lugar del click. 
		//Y lo asignamos como destino de los manifestantes
		GameObject destinoT = GameObject.Find("DestinoTemp");
		GameObject destinoTemp = (GameObject)UnityEngine.Object.Instantiate(destinoT, hit.point, transform.rotation);
		destinoTemp.GetComponent<tempMesh>().quitaTextura = true;

		//Hacemos correr a todas las unidades seleccionadas. 
		foreach (GameObject g in selectedManager.temp.objects) {
			g.GetComponent<Animator>().SetFloat("Speed", velocidadCorriendo);
			//Activamos el movimiento por orden directa, hacia el destino indicado
			g.GetComponent<UnitManager>().EnMovimiento(true);
			g.GetComponent<UnitManager>().tiempoCorriendo += Time.deltaTime;
			g.GetComponent<UnitManager>().StopAttack();
			g.GetComponent<ComportamientoHumano>().moviendose = true;
			g.GetComponent<ComportamientoHumano>().destinoTemp = destinoTemp.transform;
			g.transform.LookAt (destinoTemp.transform.position);
		}

	}


     /********************************
     *  BTN DER CLICK: IR CORRIENDO A...
     /*********************************/
     void BotonDerecho()
     {            

		Ray ray = Camera.main.ScreenPointToRay(menuBtnDerPos);
		RaycastHit hit;
		Physics.Raycast(ray,out hit,Mathf.Infinity, 1<<19);
         	IrCorriendoA(hit);   

     }

}
