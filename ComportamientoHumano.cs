/******************************************************************
 * comportaminetoHumano.cs
 * 
 * En esta clase esta incluido el manejo en tercera persona de los manifestantes, 
 * con la implementacion de las acciones contextuales.
 * 
 * A demas controla y activa las animaciones comunes a cualquier unidad 
 * humana(andar, correr, pararse, etc). 
 * Asi como la direccion en la que camina o corre, para llegar al siguiente punto
 * de destino de la manifestacion o para moverse a un punto concreto, por orden directa. 
 * 
 * (cc) 2014 Santiago Dopazo 
 *******************************************************************/

using UnityEngine;
using System;

public class ComportamientoHumano : MonoBehaviour {

	const float BAR_WIDTH = 200f;
	const float velocidadBarra = 15f;

	//Variables del movimiento hacia el destino actual.
	private float direccion = 0;
	private int setidoGiro;
	private float[] angulo = new float[3];

	//Posicion del ultimo grafiti pintado, para evitar poner muchos grafitis en el mismo sitio.
	private Vector3 ultimoGrafiti = new Vector3();

	//Estilo de la etiquetaGui de acciones contextuales [E]
	public GUIStyle labelContexto;

	//Destino de la manifestacion
	public Transform destino = null;
	//Destino temporal, por orden directa
	public Transform destinoTemp = null;
	//Indice de destino incremental
	public int destinoActual = 1;
	//Moviendose por ORDEN DIRECTA
	public bool moviendose = false;

	//Atacando en corto
	private bool ataqueCorto = false;

	//Manejo de este personaje desde tercera persona
	public bool terceraPersona = false;

	//Suena musica en el RangoEscucha
	public bool suenaMusica = false;

	//Actualmente se da una doble key, con esto la controlamos
	private bool keyPress = false;
	
	//La altura del salto. 
	public float alturaSalto = 0.2f;

	//Para la barra de fuerza
	private float minFuerza = 2f;
	private float maxFuerza = 200f;
	private float fuerzaActual;
	private bool mostrarBarra = false;
	public Texture2D fuerzaTexture;
	private float timer = 0;
	private string textoBarra;
	
	//Puntero al UnitManager de esta unidad
	private UnitManager uM;

	//Puntero al Animator
	private Animator anim;

	//Flag para debugear este objeto
	public bool debug = false;

	/*****************
	 *     START
	 * **************/
	void Start () {

		//Inicializamos la variable anim con el componente Animator
		anim = GetComponent<Animator>();

		//Inicializamos la variable uM con el componente UnitManager
		uM = GetComponent<UnitManager>();

		//Definimos el estilo de las label contextuales [E]
		labelContexto.normal.textColor = Color.white;
		labelContexto.fontSize = 14;
		labelContexto.fontStyle = FontStyle.Bold;

		//Inicializamos las fuerzas
		maxFuerza = uM.fuerza;
		minFuerza = uM.fuerza/20;

		//Asignamos el destino inicial del humano
		AsignarDestinoInicial();

	}


	// Este metodo es llamado una vez por cada framedireccion
	void FixedUpdate () {

		//Si estamos en tercera persona, gestionamos los controles, de este modo
		if (terceraPersona) {		
			ControlTerceraPersona();					
		}
		//Si no, vamos hacia el destino que tenemos marcado
		else {
			//El parametro del Animator 'Direccion', determina, desde -1 a 1 la direccion del personaje
			anim.SetFloat("Direction", direccion); 

			//Calculamos las variaciones en la direccion para mantener el rumbo hacia el destino. SI no está huyendo.
			if (!uM.estaHuyendo )
				DireccionMovimiento();

			//Control de llegada a destino y parar o continuar a siguiente punto de destino
			LlegadaDestino();
		}
	}

	/********************
	 *       On GUI
	 * *****************/
	//Cada vez que se repinta el GUI
	void OnGUI() {

		//En funcion del contexto iniciamos mostramos labels de acciones contextuales
		//Y controlamos la pulsacion de [E] para realizar dichas acciones
		if (terceraPersona) {
			AccionesTerceraPersona();		
			if (mostrarBarra)
				MostrarBarraFuerza ();
		}
	}


	//Mostramos la barra de fuerza, si tiene un objeto arrojadizo
	private void MostrarBarraFuerza () {
		GUI.BeginGroup(new Rect(20, Screen.height - 70, 200, 50));
		GUI.Label(new Rect(0, 0, 100, 25), textoBarra);
		GUI.skin.box.border = new RectOffset(5, 5, 5, 5);
		GUI.Box(new Rect(0, 25, BAR_WIDTH, 25), "");
		GUI.DrawTexture(new Rect(0, 25, (fuerzaActual - minFuerza) * BAR_WIDTH / (maxFuerza - minFuerza), 25), fuerzaTexture);
		GUI.EndGroup();
	}

	//****************************************************************
	//  MENSAJES Y LAS ACCIONES CONTEXTUALES  [E] EN TERCERA PERSONA
	//****************************************************************
	private void AccionesTerceraPersona() {

		//Posicion del label [E] Accion
		float posxLabel, posyLabel;		
		posxLabel = (Screen.width/2)-45;
		posyLabel = (Screen.height/2)-5;
		
		//Lanzamos un Ray, para ver lo que tenemos delante
		RaycastHit hit = new RaycastHit();
		
		/****************************
		*   [E] HABLAR DE LA CAUSA
		*****************************/
		//Si el manifestante es muy activista y nos acercamos a una persona
		if (uM.activismo > Manager.temp.activismoActivista 
			&& Physics.Raycast(transform.position + transform.up*3, transform.forward, out hit, 5, 1 << 8)) {

			//Si esa persona aun no había escuchado el discurso...
			if (!hit.collider.gameObject.GetComponent<UnitManager>().escuchoDiscurso){
				GUI.Label(new Rect(posxLabel,posyLabel, 105,10),"[E] Hablar de la causa", labelContexto);

				//Comenzamos a cargar la barra de tiempo hablando
				if (Input.GetKeyDown(KeyCode.E)) {
					textoBarra = "Hablando...";
					mostrarBarra = true;

					//La persona se para a escuchar
					hit.collider.gameObject.GetComponent<UnitManager>().estaParado = true;
				}

				//Aumentamos el contador de tiempo
				else if (Input.GetKey(KeyCode.E)) 
					fuerzaActual += Time.deltaTime * velocidadBarra/4; 

				//Cuando el tiempo se ha cumplido ponemos el grafiti
				if (fuerzaActual >= BAR_WIDTH / 3 || Input.GetKeyUp(KeyCode.E)) {

					//Referencia al componente UnitManager del objetivo
					UnitManager persona = hit.collider.gameObject.GetComponent<UnitManager>();

					//Calculamos dependiendo de la barra de carga y del activismo de nuestro manifestante, cuanto aumenta
					int cuanto = Mathf.RoundToInt((fuerzaActual / maxFuerza) * uM.activismo / 5);

					//Si es peatón aumenta el apoyo y si es manifestante el activismo
					if (persona.esPeaton) {
						persona.apoyo += cuanto;
						Manager.temp.sucesos.Add(persona.nombre + " aumenta en " + cuanto.ToString() + " su apoyo a la manifestación.");
					}
					else if (persona.esManifestante){
						persona.activismo += cuanto;
						Manager.temp.sucesos.Add(persona.nombre + " aumenta en " + cuanto.ToString() + " su nivel de activismo.");
					}

					//Y si fuera policía??
					persona.estaParado = false;

					//Marcamos la persona como que ya escuchó el discurso una vez
					persona.escuchoDiscurso = true;

					//Reiniciamos la barra
					fuerzaActual = 0;
					mostrarBarra = false;
				}
			}

		}

		/************************
		 * [E] PONER MUSICA
		 *************************/
		else if (uM.tieneMusica) {
			if (uM.estaReproduciendoMusica)
				GUI.Label(new Rect(posxLabel,posyLabel, 85,10),"[E] Quitar Musica",labelContexto);
			else
				GUI.Label(new Rect(posxLabel,posyLabel, 85,10),"[E] Poner Musica",labelContexto);
			
			if (Input.GetKeyUp(KeyCode.E)) {
				if (keyPress) {
					uM.estaReproduciendoMusica = !uM.estaReproduciendoMusica;
					if (debug)
						Debug.Log("Reproduciendo musica: " + uM.estaReproduciendoMusica.ToString());
					keyPress = false;
				}
				else
					keyPress = true;
			}
		}
		/********************************
		 * [E] BAILAR  en tercera persona. Desafío.
		 ******************************
		else if (suenaMusica) {
			if (uM.estaBailando)
				GUI.Label(new Rect(posxLabel-5,posyLabel, 95,10),"[E] Dejar de Bailar",labelContexto);
			else
				GUI.Label(new Rect(posxLabel+20,posyLabel, 55,10),"[E] Bailar",labelContexto);
			
			if (Input.GetKeyUp(KeyCode.E)) 
				uM.estaBailando = !uM.estaBailando;
		} */

		/************************
		 * [E] LANZAR PIEDRA 
		 *************************/
		else if (uM.tieneOVNI) {
			
			//Dibujamos el punto de mira y damos la opcion de lanzar
			//GUI.Label(new Rect(posxLabel-50,posyLabel, 20,10),"( X )",labelContexto);
			GUI.Label(new Rect(posxLabel - 100, posyLabel + 30, 95,10),"[E] Lanzar Piedra",labelContexto);

			//Controla mos la pulsación de [E] para lanzar una piedra
			ControlLanzarPiedra();			
		}

		/************************
		 * [E] PINTAR GRAFITI
		 *************************/
		 //tiene Spray y Si hay una pared cerca..
		if (uM.tieneSpray && Physics.Raycast(transform.position + transform.up*3, transform.forward, out hit, 2)) {
			if (hit.point != Vector3.zero && hit.collider.gameObject.layer == LayerMask.NameToLayer("Edificios")) {

				// No se puede pintar un Grafiti sobre otro
				if ((transform.position.z > ultimoGrafiti.z + 1 || transform.position.z < ultimoGrafiti.z - 1) &&
				    (transform.position.x > ultimoGrafiti.x + 1 || transform.position.x < ultimoGrafiti.x - 1)) {

					GUI.Label(new Rect(posxLabel,posyLabel, 85,10),"[E] Pintar Grafiti",labelContexto);

					//Comenzamos a cargar la barra de tiempo pintando el grafiti
					if (Input.GetKeyDown(KeyCode.E)) {
						textoBarra = "Pintando grafiti...";
						mostrarBarra = true;
						uM.estaPintando = true;
					}

					//Aumentamos el contador de tiempo
					else if (Input.GetKey(KeyCode.E)) 
						fuerzaActual += Time.deltaTime * velocidadBarra/5; //Molaria dividirlo entre el tamaño del grafiti					

					//Cuando el tiempo se ha cumplido ponemos el grafiti
					if (fuerzaActual >= BAR_WIDTH / 3) {

						//Pintamos el grafiti sobre la pared
						PintarGrafiti();

						//Incrementamos las barras de objetivo de la manifestacion
						Manager.temp.IncConciencia(30);
						Manager.temp.IncAmbiente(10);							
						Manager.temp.addGrafiti();

						//Reiniciamos la barra
						mostrarBarra = false;
						fuerzaActual = 0;

						//Guardamos la posicion, para que no puedan pintar mas garfitis ahi
						ultimoGrafiti = transform.position;
						uM.estaPintando = false;
					}

					//Si soltamos la E antes de que se complete el grafiti
					else if (Input.GetKeyUp(KeyCode.E)) {

						//Reiniciamos la barra
						mostrarBarra = false;
						fuerzaActual = 0;
						uM.estaPintando = false;
					}
				}
			}
		}
		/*********************
		*   [E] QUEMAR OBJETO
		**********************/
		if (uM.tieneMechero && Physics.Raycast(transform.position + transform.up * 2, transform.forward, out hit, 3)) {	

			//Si tenemos un objeto quemable delante, aparece la opcion
			if (hit.collider.gameObject.tag != "Ardiendo" && hit.collider.gameObject.layer != 8) {

				//Mostramos la opcion de quemar el objeto y controlamos la pulsacion de [E]
				GUI.Label(new Rect(posxLabel,posyLabel, 85,10),"[E] Quemar " + hit.collider.gameObject.name,labelContexto);
		
				//Comenzamos a cargar la barra de tiempo Quemando el objeto
				if (Input.GetKeyDown(KeyCode.E)) {
					textoBarra = "Prendiendo fuego...";
					mostrarBarra = true;
					uM.estaQuemando = true;
				}

				//Aumentamos el contador de tiempo
				else if (Input.GetKey(KeyCode.E)) 
					fuerzaActual += Time.deltaTime * velocidadBarra/7; //Molaria dividirlo entre el tamaño del grafiti	

				//Cuando el tiempo se ha cumplido ponemos el grafiti
				if (fuerzaActual >= BAR_WIDTH / 3) {

					//Prendemos fuego al objeto
					QuemarObjeto(hit.collider.gameObject);									

					//Reiniciamos la barra
					mostrarBarra = false;
					fuerzaActual = 0;
					uM.estaQuemando = false;

				}

				//Si soltamos la E antes de que se complete el grafiti
				else if (Input.GetKeyUp(KeyCode.E)) {

					//Reiniciamos la barra
					mostrarBarra = false;
					fuerzaActual = 0;
					uM.estaQuemando = false;
				}
				
			}			
		}
	}
	/**************************************
	//Si ENTRAMOS EN CONTACTO con un objeto
	***************************************/
	void OnCollisionEnter (Collision objeto) {
					
		//Si colisiona con un objeto lo esquiva hacia el lado mas logico.
		//Desafio: sustituir los valores de los angulos: 190,210, etc, por la relacion con el destino. 		
		if (objeto.collider.tag != "Suelo")  {	

			//Comenzamos a esquivar el objeto por el angulo mas adecuado al destino
			if (angulo[1] < angulo[2])
					setidoGiro = 1;
			else
					setidoGiro = -1;
		}
		else {
			setidoGiro = 0;
		}
	
		//El toca fuego, se quema...
		if (objeto.collider.tag == "Ardiendo") {
			uM.valor -= 30;
			uM.energia -= 15;
			if (!uM.estaHerido) {
				uM.estaHerido = true;	
				Manager.temp.AddHeridos(uM.name);
				uM.SalirCorriendo(objeto.collider.transform.position);		
			}
		}

		//Si alguien fichado toca a un policía saldrá corriendo
		if (objeto.collider.tag == "Policia" && uM.estaFichado){
			uM.SalirCorriendo(objeto.collider.transform.position);
			uM.valor -= 10;
			uM.energia += 5;
		}

		//Si esta atacando y colisiona con su objetivo, inicia el ataque en corto
		if (GetComponent<UnitManager>().estaAtacando && destinoTemp != null && (objeto.collider.transform == destinoTemp.transform 
			|| objeto.collider.transform.parent == destinoTemp.transform)) {

			//Si tiene mechero le prende fuego y sale corriendo
			if (uM.tieneMechero && objeto.collider.gameObject.layer != 8){
				QuemarObjeto(objeto.collider.gameObject);				
				uM.SalirCorriendo(destinoTemp.transform.position);

			}
			else {

				//se inicia el ataque en corto
				iniciarAccion ("AtaqueCorto");			

				//El cuerpo recibe una fuerza de impacto
				objeto.collider.rigidbody.AddForce (transform.forward * (GetComponent<UnitManager> ().fuerza / 10), ForceMode.Impulse);

				//Y si es persona, recibe impacto
				if (objeto.collider.gameObject.layer == LayerMask.NameToLayer("Personas"))
					objeto.collider.gameObject.GetComponent<UnitManager>().recibeImpacto = true;

				//Dejamos de movernos para atacar
				uM.moveToAttack = false;

				//Incrementamos el ambiente un poco
				Manager.temp.IncAmbiente(5);
			}
		}

		//Si colisionan con una acera, saltan 
		if (objeto.collider.gameObject.layer == LayerMask.NameToLayer("Acera")) {
			iniciarAccion("Jump");
			transform.Translate((transform.forward + transform.up) * alturaSalto * 2); //0,alturaSalto,0);
		}
	}

	/*******************************************
	//MIENTRAS ESTAMOS EN CONTACTO con un objeto
	*********************************************/
	void OnCollisionStay (Collision  objeto) {

		//Si tropieza con una persona parada, nos paramos. Excepto si camina por orden directa. 
		if (objeto.collider.gameObject.layer == LayerMask.NameToLayer("Personas") && !uM.esLider)  {				
			UnitManager uMColision = objeto.collider.gameObject.GetComponent<UnitManager>();

			//Si el objetivo de un ataque está KO, buscamos otro
			if ((objeto.gameObject.tag == "KO" || objeto.gameObject.tag == "Ardiendo") && uM.estaAtacando && 
				objeto.collider.gameObject.transform == uM.objetivoInteractuar)	
				uM.objetivoInteractuar = uM.buscarObjetivo();

			//Si chocamos con alguien parado y no nos estamos moviendo por orden directa, nos paramos también
			if (uMColision.estaParado && !moviendose && uM.tiempoCorriendo == 0 && !uM.estaHuyendo)
				uM.estaParado = true;

			//Si estamos atacando en corto y tenemos a una persona delante, se considera atacada
			if (ataqueCorto) {
				RaycastHit hit = new RaycastHit();
				Physics.Raycast(transform.position + transform.up*3, transform.forward, out hit, 2);
				if (hit.collider == objeto.collider) {

					//La persona se considera atacada
					uMColision.recibeImpacto = true;

					//Y ataca a su vez
					uMColision.objetivoInteractuar = transform;
					uMColision.estaAtacando = true;
					objeto.collider.gameObject.transform.LookAt(transform.position);
					ataqueCorto = false;
				}
			}


		}
		//Si colisiona con un objeto de la capa 'Ostaculos' y lo empuja.
		else if ((objeto.collider.gameObject.layer == LayerMask.NameToLayer("Obstaculos")) 
		         && ((uM.energia > 0 && uM.activismo > 30 && uM.esManifestante) || terceraPersona))  {	
			iniciarAccion("Empujando");

			//Revisar que todos los objetos de la capa obsjtaculos tengan Rigidbody
			try{
				objeto.rigidbody.AddForce(transform.forward);
			}
			catch{}
		}

		//Si colisiona con cualquier otra cosa, que no sea el suelo, comienza a girar para esquivarlo
		else if (!terceraPersona) {			
			if (objeto.collider.tag != "Suelo")
				direccion += 0.3f * setidoGiro * Time.deltaTime;
		}		

		//Si el objetivo de nuestro ataque está ardiendo o KO, buscamos otro objetivo. 
		else if ((objeto.collider.tag == "KO" || objeto.collider.tag == "Ardiendo") && uM.estaAtacando 
			    && (objeto.collider.gameObject.transform == uM.objetivoInteractuar ||
			    	   objeto.collider.gameObject.transform.parent == uM.objetivoInteractuar))	
				uM.objetivoInteractuar = uM.buscarObjetivo();	
		if (debug) 
			Debug.Log("Colisiona con: " + objeto.collider.name);

	}

	//DEJAMOS DE ESTAR EN CONTACTO con un objeto
	void OnCollisionExit (Collision  objeto) {

		if (objeto.collider.tag!= "Suelo")  {	
			setidoGiro = 0;
		}

		//Si el objetivo se va, detenemos el ataque en corto y lo seguimmos		
		if  (uM.estaAtacando && (objeto.collider.gameObject.transform == uM.objetivoInteractuar 
						|| objeto.collider.gameObject.transform.parent == uM.objetivoInteractuar)) {
			ataqueCorto = false;
			finalizarAccion("AtaqueCorto");
			uM.moverseParaAtacar();
		}

		//Si dejamos de estar en contacto con contenedores, cancelamos la animacion
		if ( objeto.collider.gameObject.layer == LayerMask.NameToLayer("Obstaculos")  && uM.esManifestante)  {	
			anim.SetBool("Empujando", false);
		}

		//Si salen de una acera, dejan de saltar
		if (objeto.collider.gameObject.layer == LayerMask.NameToLayer("Acera")) 
			finalizarAccion("Jump");	

	}
	
	//Si la animacion de ataque golpea varias veces, llama a este metodo en cada golpe de la animacion
	public void atacandoEnCorto() {

		ataqueCorto = true;

	}

	//Hacemos un cambio de camaras, por una camara no dependiente del manifestante,
	//Pues hay animaciones que mueven demasiado	al manifestante
	public void cambioCamara(bool on) {

		//Solo cuando estamos en modo tercera persona.
		if (terceraPersona) {
			if (on) {

				//Buscamos las camaras personales y las igualamos.
				Camera cPersonal = Manager.temp.personaCamera1;
				Camera cPersonal2 = Manager.temp.personaCamera2;
				cPersonal2.transform.position = cPersonal.transform.position;
				cPersonal2.transform.rotation = cPersonal.transform.rotation;

				//Y ponemos de Main a la CamaraPersonal2
				cPersonal2.depth = 6;
			}
			else {				
				//Y ponemos de Main a la CamaraPersonal2
				Manager.temp.personaCamera2.depth = 1;
			}
		}

	}

	/***************************
	 * ASIGNAR DESTINO INICIAL 
	 * *************************/
	private void AsignarDestinoInicial() {

		//Asignamos el destino inicial
		if (destinoActual == 0) destinoActual = 1;

		//Si es peaton sus destinos son los > 60
		//Pendiente_: convertir esto en funcion
		if ((uM.esPeaton && destinoActual < 10) || !destino) {
			destinoActual = Mathf.RoundToInt(UnityEngine.Random.value * 10) + 60;
			try {
				destino = GameObject.Find("Destino" + destinoActual.ToString()).transform;
			}
			catch (Exception e) {
				Debug.Log (name + " / Destino Actual: " + destinoActual.ToString() + " / " + e.Message);
			}
		}

		// Si es manifestante le asignamos el mismo destino que al lider, inicialmente. Si falla, le asignamos
		else if (uM.esManifestante) {
			try {
				destino = Manager.temp.liderAlpha.GetComponent<ComportamientoHumano>().destino;											
			}
			catch(Exception e){
				Debug.Log (name + " / Error asignando destino: " + e.Message);
			}
		}

		//Si es policía, su destino es el capitán más cercano
		else if (uM.esPolicia) {

			//Control de 'objetos alrededor' de la unidad
			Collider[] objectsInRange = new Collider[0];	
			objectsInRange = Physics.OverlapSphere(transform.position, 25, LayerMask.NameToLayer("Personas"));
		
			//Si hay otros policias alrededor
			if (objectsInRange.Length > 0) {	

				//Buscamos al capitán
				foreach (Collider policia in objectsInRange)  
					if (policia.gameObject.GetComponent<UnitManager>().esCapitan)
						//Asignamos al capitán como destino
						destino = policia.transform;
			}
		}

	}

	//********************************************
	//          MANEJO DE TERCERA PERSONA
	//********************************************
	private void ControlTerceraPersona() {

		//Para establecer la velocidad de la persona
		int divisor = 10;
		
		//Mientras se pulsa SHIFT empezamos a CORRER.
		//Para lo cual cambiamos el divisor de la velocidad de desplazamiento
		if (Input.GetKey (KeyCode.LeftShift) && uM.energia > 0) { 
			divisor = 2;

			//Indicamos al UnitManager que estamos corriendo
			uM.tiempoCorriendo += Time.deltaTime;
		}
		else {
			divisor = 10;

			//Indicamos al UnitManager que dejamos de correr 
			uM.tiempoCorriendo = 0;
		}
		
		//Establecemos las variables para la animacion de caminar/correr/girar
		anim.SetFloat("Direction", Input.GetAxis("Horizontal")/2); 						
		anim.SetFloat("Speed",Input.GetAxis("Vertical")/divisor);

		if (anim.GetFloat("Speed") == 0)
			uM.estaParado = true;
		else
			uM.estaParado = false;
		
		//Hacemos que la camara(aerea) siga a este personaje. 
		Camera.main.transform.position =  new Vector3 (transform.position.x,Camera.main.transform.position.y, transform.position.z);
		
		//******************
		// CONTROL DE SALTOS
		//******************
		if (Input.GetButtonDown("Jump")){
			iniciarAccion("Jump");
			transform.Translate(0,alturaSalto,0);
		}
		
		//Apagamos las variables de control de las acnimaciones, una vez iniciadas...
		if(!anim.IsInTransition(0)) {
			anim.SetBool("Jump",false);
			anim.SetBool("Arrojar",false);			
			anim.SetBool("Empujando", false);
		}
		
		//*******************************
		// CONTROL DE ATAQUE EN CORTO
		//*******************************
		if (Input.GetKeyDown(KeyCode.LeftControl)){
			iniciarAccion("AtaqueCorto");
			cambioCamara(true);
			ataqueCorto = true;
		}
		if (Input.GetKeyUp(KeyCode.LeftControl)){
			anim.SetBool("AtaqueCorto",false);
			cambioCamara(false);
			ataqueCorto = false;
		}

	}

	private void DireccionMovimiento() {

		Vector3 vectorDireccion;
				
		//****************************
		// COMPORTAMIENTO PEATON
		//****************************
		// implementar un ray a los lados, si esta sobre la acera, para conocer la distancia a los edificios y tratar de mantenerla
		// en realidad puede que un script de movimiento especifico seria interesante. 
		// O quizas solo variando el margen de correccion de ruta ya sea suficiente. DESAFIO!		

		//****************************
		// COMPORTAMIENTO EN GENERAL
		//****************************
		
		//Adaptación de la posicion del punto de destino al plano del humano
		Vector3 destinoPos = new Vector3 (destino.position.x, transform.position.y, destino.position.z);

		//Si el destino fue destruido o detenido, deja de moverse
		if (destinoTemp == null) 
			moviendose = false;

		//Calculamos el vector de direccion	al punto de destino		
		//Moviendose por orden directa
		if (moviendose) 
			vectorDireccion = destinoTemp.position - transform.position;
		else 
			//O moviendose hacia el siguiente punto de destino
			vectorDireccion = destinoPos - transform.position;
		
		//Normalizamos el vector dirección
		vectorDireccion.Normalize();

		//Obtenemos el angulo entre el vector forward y el vector direccion			
		angulo[0] = Vector3.Angle(transform.forward, vectorDireccion);

		//Y tambien los angulos entre el vector izquierda y derecha
		angulo[1] = Vector3.Angle(transform.right,vectorDireccion);
		angulo[2] = Vector3.Angle(transform.right*(-1),vectorDireccion);

		//Si nos salimos en 10º de la direccion correcta, corregimos.
		if (angulo[0] > 10) {
			if (angulo[1] > angulo[2]){
				if (angulo[1] > 80)
					direccion -= 0.1f * Time.deltaTime;			
				else
					direccion += 0.1f * Time.deltaTime;
			}
			else { 
				if (angulo[2] > 80)
					direccion += 0.1f * Time.deltaTime;
				else
					direccion -= 0.1f * Time.deltaTime;
				direccion += 0.1f * Time.deltaTime;
			}
			
			//direccion += 0.1f;
		}
		else
			direccion = 0f;
		
		//Mantenemos los limites constantes
		if (direccion >1f) 
			direccion = 1f;
		else if(direccion <-1)
			direccion = -1f;

		//Debugeamos si el flag de debug esta activado para este objeto
		if (debug) {
			//Debug.Log (name + ": Angulo[0] = " +  angulo[0].ToString() + " / Angulo[1] = " + angulo[1].ToString() 
			//	      + " / Angulo[2] = " + angulo[2].ToString() + " / Direccion: " + vectorDireccion.ToString());
			//Debug.Log (name + ": direccion = " + direccion.ToString());
			Debug.DrawLine (transform.position, transform.position + vectorDireccion * 10, Color.red);
			Debug.DrawLine (transform.position, transform.position + transform.forward * 20, Color.blue);
		}

	}

	//********************
	//  LLEGADA A DESTINO
	//********************
	private void LlegadaDestino () {

		//Si se esta moviendo por orden directa o si se mueve hacia un punto de destino de la manifestacion
		if (moviendose) {
			float distanciaParadaPorOrden = UnityEngine.Random.Range(0, uM.rangoEscucha) ;

			//Se paran cerca del punto de destino. Generamos un numero aleatorio entre 0 y rangoescucha.
			//Para que se repartan por el espacio y se paren a distintas distancias.
			if (Math.Round(Vector3.Distance (transform.position, destinoTemp.position)) == Math.Round(distanciaParadaPorOrden)
			    && !uM.moveToAttack) {
				moviendose = false;
				uM.EnMovimiento(false);
			}
			else if (uM.moveToAttack && Math.Round(Vector3.Distance (transform.position, destinoTemp.position)) <= 1) {
				uM.moveToAttack = false;
				moviendose = false;
				uM.EnMovimiento(false);
			}
		}
		else {
			float distanciaDeLlegada;

			//Si el destino es un punto de reunion, se paran a distintas distancias de el
			if (destino.name == "Punto de Reunion")
				distanciaDeLlegada = uM.rangoEscucha * UnityEngine.Random.value * 4;
			else
				distanciaDeLlegada = uM.rangoEscucha;

			//if (debug)
			//	Debug.Log("Distancia al destino: " + (Vector3.Distance (transform.position, destino.position).ToString()));

			//Si llegan a un punto de destino, cambian al siguiente punto
			if (Vector3.Distance (transform.position, destino.position) < distanciaDeLlegada)
			{
				//Los peatones tienen distintas rutas, el siguiente punto de su ruta es aleatorio.
				if (uM.esPeaton) 
					destinoActual = UnityEngine.Random.Range(1,3) + 60;
				else {
					if (destino.name == "Destino Punto Reunion")
						uM.EnMovimiento(false);
					else 
						destinoActual ++;
				}

				//Asignamos el nuevo destino
				try {

					//La policía no cambia de destino, se para al llegar
					if (uM.esPolicia)					
						uM.EnMovimiento(false);
					else {
						destino = GameObject.Find("Destino" + destinoActual.ToString()).transform;		
					
						if (debug)
							Debug.Log(name +" Cambia a: Destino" + destinoActual.ToString());
					}
				}
				catch {

					//Si es peaton y no existe destino siguiente, se destruye
					if (uM.esPeaton)
						Destroy(this);
					else if (uM.esManifestante) {
						destino = Manager.temp.liderAlpha.GetComponent<ComportamientoHumano>().destino;							
					}
				}
			}
		}

	}

	//Iniciamos la animacion seleccionada
	public void iniciarAccion (string accion) {
		
		anim.SetBool(accion, true);
		if (accion == "Arrojar")
			GetComponent<UnitManager>().arrojandoObjeto = true;
		
	}
	//Finalizamos la animacion seleccionada
	public void finalizarAccion (string accion) {
		
		anim.SetBool(accion, false);
		
	}

	//Quemar un objeto
	private void QuemarObjeto (GameObject objeto) {

		//Duplicamos el objeto fuegoOrginal y lo posicionamos sobre el objeto quemado		
		GameObject fuegoTemp = (GameObject)UnityEngine.Object.Instantiate(Manager.temp.fuegoOriginal, 
															 objeto.transform.position, transform.rotation);
		fuegoTemp.transform.parent = objeto.transform;
		objeto.tag = "Ardiendo";

		//Quemar cosas tiene consecuencias...
		Manager.temp.IncAmbiente(50);
		Manager.temp.IncRepercusion(50);
		Manager.temp.LessConciencia(50);
		Manager.temp.IncNivelCarga(50);

	}

	//Pintar un grafiti
	private void PintarGrafiti () {

		//Buscamos el Graffiti original de referencia. Desafio(Asignar graffitis aleatorios)
		GameObject gTemp;
		Texture T = Resources.Load<Texture2D>("Grafiti" + UnityEngine.Random.Range(1,10).ToString());

		//Hacemos una copia del grafiti original, Ajustamos la posicion y la rotacion 
		gTemp = (GameObject) UnityEngine.GameObject.Instantiate(Manager.temp.grafitiOriginal);

		//Asignamos el la textura y ajustamos la posición
		gTemp.renderer.material.mainTexture = T;
		gTemp.transform.rotation = transform.rotation;
		gTemp.transform.Rotate(new Vector3(90,0,180));
		gTemp.transform.position = (transform.position +(transform.up*3));
		//DESAFÍO: TAMAÑO DEL GRAFFITI AJUSTADO
	}

	private void ControlLanzarPiedra() {

		if (Input.GetKeyDown(KeyCode.E)){

			//Reiniciamos las variables
			fuerzaActual = 0;
			timer = 0;
			mostrarBarra = true;
			textoBarra = "Fuerza";

			//Activamos el estado 
			uM.estaLanzando = true;
		}
		else if (Input.GetKey(KeyCode.E)) {
			timer += Time.deltaTime * velocidadBarra;
			fuerzaActual = Mathf.PingPong(timer, maxFuerza - minFuerza) +  minFuerza;
		}
		//Lanzamos la piedra
		else if (Input.GetKeyUp(KeyCode.E)) {

			//Asignamos la fuerza del lanzamiento de la piedra
			uM.fuerza = fuerzaActual;

			//La accion se lanza desde la animacion. Iniciamos la animacion
			iniciarAccion("Arrojar");

			//Incrementamos el ambiente en la mani
			Manager.temp.IncAmbiente(1);

			//Cambiamos la camara para que no se mueva con la animacion
			cambioCamara(true);

			//Ocultamos la barra
			mostrarBarra = false;

			//Desactivamos el estado
			uM.estaLanzando = false;
		}
	}

}
