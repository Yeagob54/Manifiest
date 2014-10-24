// (cc) 2014 SANTIAGO DOPAZO HILARIO
// 
// Esta clase define el comportamiento de Manifestantes y Peatones

using UnityEngine;
using System;

public class ComportamientoHumano : MonoBehaviour {

	//Variables del movimiento hacia el destino actual.
	private float direccion;
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

	//La altura del salto. 
	public float alturaSalto = 0.2f;

	//Puntero al unitManager de esta unidad
	private unitManager uM;

	//Puntero al Animator
	private Animator anim;

	/*****************
	 *     START
	 * **************/
	void Start () {
		//Inicializamos variables
		direccion = 0;

		//inicializamos la variable anim con el componente Animator
		anim = GetComponent<Animator>();
		//inicializamos la variable uM con el componente unitManager
		uM = GetComponent<unitManager>();

		//Definimos el estilo de las label contextuales [E]
		labelContexto.normal.textColor = Color.white;
		labelContexto.fontSize = 14;
		labelContexto.fontStyle = FontStyle.Bold;

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
		if (uM.esManifestante) {
			try {
				destino = GameObject.Find("Lider Alpha").GetComponent<ComportamientoHumano>().destino;							

			}
			catch(Exception e){
				Debug.Log (name + " / Error asignando destino: " + e.Message);
			}
		}

	}

	/********************
	 *       On GUI
	 * *****************/
	//Cada vez que se repinta el panel GUI
	void OnGUI() {

		//Posicion del label [E] Accion
		float posxLabel, posyLabel;
		posxLabel = (Screen.width/2)-45;
		posyLabel = (Screen.height/2)-5;


		//****************************************************************
		//  MENSAJES Y LAS ACCIONES CONTEXTUALES  [E] EN TERCERA PERSONA
		//****************************************************************
		if (terceraPersona) {
			//Lanzamos un Ray, para ver lo que tenemos delante
			RaycastHit hit = new RaycastHit();

			/************************
			 * [E] PONER MUSICA
			 *************************/
			if (uM.tieneMusica) {
				if (uM.estaReproduciendoMusica)
					GUI.Label(new Rect(posxLabel,posyLabel, 85,10),"[E] Quitar Musica",labelContexto);
				else
					GUI.Label(new Rect(posxLabel,posyLabel, 85,10),"[E] Poner Musica",labelContexto);
				
				if (Input.GetKeyUp(KeyCode.E)) {
					uM.estaReproduciendoMusica = !uM.estaReproduciendoMusica;
					Debug.Log("Reproduciendo musica: " + uM.estaReproduciendoMusica.ToString());
				}
			}
			/************************
			 * [E] BAILAR
			 *************************/
			else if (suenaMusica) {
				if (uM.estaBailando)
					GUI.Label(new Rect(posxLabel-5,posyLabel, 95,10),"[E] Dejar de Bailar",labelContexto);
				else
					GUI.Label(new Rect(posxLabel+20,posyLabel, 55,10),"[E] Bailar",labelContexto);
				
				if (Input.GetKeyDown(KeyCode.E)) 
					uM.estaBailando = !uM.estaBailando;
			} 
			/************************
			 * [E] LANZAR PIEDRA //Revisar!!!
			 *************************/
			else if (uM.manoIzquierda.GetComponent<ObjetoDeMano>().esArrojable || uM.manoDerecha.GetComponent<ObjetoDeMano>().esArrojable) {

				/*fuerza del lanzamiento
				//if (Input.GetKeyDown(KeyCode.E)) {
					fuerzaCarga = 0;
				}
				else if (Input.GetKeyPress(KeyCode.E))
					fuerzaCarga += 1 * Time.deltaTime;
				else if (Input.GetKeyUp(KeyCode.E))
				*/
				
				//Dibujamos el punto de mira y damos la opcion de lanzar
				GUI.Label(new Rect(posxLabel+40,posyLabel+10, 20,10),"( X )",labelContexto);
				GUI.Label(new Rect(posxLabel,posyLabel, 95,10),"[E] Lanzar Piedra",labelContexto);
				//Lanzamos la piedra
				if (Input.GetKeyUp(KeyCode.E)) {
					//Marcamos la persona objetivo como el objetivoInteractuar el unitManager,para futuras acciones
					uM.objetivoInteractuar = hit.collider.gameObject.transform;
					//La accion se lanza desde la animacion. Iniciamos la animacion
					iniciarAccion("Arrojar");
				}

			}
			/************************
			 * [E] PINTAR GRAFITI
			 *************************/
			if (uM.tieneSpray) {
				//Si hay una pared cerca...
				Physics.Raycast(transform.position + transform.up*3, transform.forward, out hit, 2);
				try{ //Hace cosas raras, REVISAR!!				
					if (hit.collider.gameObject.layer == 10) {
						GUI.Label(new Rect(posxLabel,posyLabel, 85,10),"[E] Pintar Grafiti",labelContexto);
						if (Input.GetKeyUp(KeyCode.E)) {
							// No se puede pintar un Grafiti sobre otro
							if ((transform.position.z > ultimoGrafiti.z + 1 || transform.position.z < ultimoGrafiti.z - 1) &&
							    (transform.position.x > ultimoGrafiti.x + 1 || transform.position.x < ultimoGrafiti.x - 1)) {
								//Buscamos el Graffiti original de referencia. Revisar(Asignar graffitis aleatorios)
								GameObject gTemp = GameObject.Find("Grafiti Original");
								//Hacemos una copia del grafiti original, Ajustamos la posicion y la rotacion 
								gTemp = (GameObject) UnityEngine.GameObject.Instantiate(gTemp);//, transform.localPosition -(Vector3.forward)+(Vector3.up*3)-(Vector3.right), rotacionGraffiti);
								gTemp.transform.rotation = transform.rotation;
								gTemp.transform.Rotate(new Vector3(90,0,180));
								gTemp.transform.position = (transform.position +(transform.up*3));
								
								//Guardamos la posicion, para que no puedan pintar mas garfitis ahi
								ultimoGrafiti = transform.position;
							}
						}
					}
				}catch{};
			}
			/************************
			 * [E] QUEMAR OBJETO
			 *************************/
			if (uM.tieneMechero) {
				Physics.Raycast(transform.position, transform.forward, out hit, 2);
				try{
				//Si tenemos un objeto quemable delante, aparece la opcion
				if (hit.collider.gameObject.layer == 13) {
						//Mostramos la opcion de quemar el objeto y controlamos la pulsacion de [E]
						GUI.Label(new Rect(posxLabel,posyLabel, 85,10),"[E] Quemar " + hit.collider.gameObject.name,labelContexto);
						if (Input.GetKeyDown(KeyCode.E)){
							//hacemos una copia del fuego original y lo posicionamos 
							GameObject fuego = GameObject.Find("Fuego Original"); 
							GameObject fuegoTemp = (GameObject)UnityEngine.Object.Instantiate(fuego, hit.collider.gameObject.transform.position, transform.rotation);
							fuegoTemp.transform.parent = hit.collider.gameObject.transform;
							//Quema
							hit.collider.gameObject.tag = "Ardiendo";
						}
					}
				}catch{};
			}
		}
	}
	
	// Este metodo es llamado una vez por cada framedireccion
	void FixedUpdate () {

		//********************************************
		//              MANEJO DE TERCERA PERSONA
		//********************************************
		if (terceraPersona) {
		
			//Para establecer la velocidad de la persona
			int divisor = 10;

			//Mientras se pulsa SHIFT empezamos a CORRER.
			//Para lo cual cambiamos el divisor de la velocidad de desplazamiento
			if (Input.GetKey (KeyCode.LeftShift) && uM.energia > 0) { 
				divisor = 2;
				//Indicamos al unitManager que estamos corriendo
				uM.tiempoCorriendo += Time.deltaTime;
			}
			else {
				divisor = 10;
				//Indicamos al unitManager que dejamos de correr corriendo
				uM.tiempoCorriendo += Time.deltaTime;
			}

			//Establecemos las variables para la animacion de caminar/correr/girar
			anim.SetFloat("Direction", Input.GetAxis("Horizontal")/2); 						
			anim.SetFloat("Speed",Input.GetAxis("Vertical")/divisor);

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
				//anim.SetBool("Empujando", false);
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
		//Si NO ESTAMOS EN TERCERA PERSONA vamos hacia el destino que tenemos marcado
		else {
			Vector3 vectorDireccion;

			//El parametro del Animator 'Direccion', determina, desde -1 a 1 la direccion del personaje
			anim.SetFloat("Direction", direccion); 						

			//****************************
			// COMPORTAMIENTO MANIFESTANTE
			//****************************
		

			//****************************
			// COMPORTAMIENTO PEATON
			//****************************
			// implementar un ray a los lados, si esta sobre la acera, para conocer la distancia a los edificios y tratar de mantenerla
			// en realidad puede que un script de movimiento especifico seria interesante. 
			// O quizas solo variando el margen de correccion de ruta ya sea suficiente. Revisar


			//****************************
			// COMPORTAMIENTO EN GENERAL
			//****************************

			//Calculamos el vector de direccion	al punto de destino		
			//Moviendose por orden directa
			if (moviendose) 
				vectorDireccion = destinoTemp.position - transform.position;
			else
			//O moviendose hacia el siguiente punto de destino
				vectorDireccion = destino.position - transform.position;

			vectorDireccion.Normalize();

			//Obtenemos el angulo entre el vector forward y el vector direccion			
			angulo[0] = Vector3.Angle(transform.forward,vectorDireccion);
			//Y tambien los angulos entre el vector izquierda y derecha
			angulo[1] = Vector3.Angle(transform.right,vectorDireccion);
			angulo[2] = Vector3.Angle(transform.right*(-1),vectorDireccion);

			//Si nos salimos en 5º de la direccion correcta, corregimos.
			if (angulo[0] > 5) {
				if (angulo[1] > angulo[2])
					if (angulo[1] > 80)
						direccion -= 0.1f;
					else
						direccion += 0.1f;
				else 
					if (angulo[2] > 80)
						direccion += 0.1f;
					else
						direccion -= 0.1f;
			}
			else
				direccion = 0f;
		
			//Mantenemos los limites constantes
			if (direccion >1f) 
				direccion = 1f;
			else if(direccion <-1)
				direccion = -1f;

			//********************
			//  LLEGADA A DESTINO
			//********************
			//Si se esta moviendo por orden directa o si se mueve hacia un punto de destino de la manifestacion
			if (moviendose) {
				float distanciaParadaPorOrden = (uM.rangoEscucha)* UnityEngine.Random.Range(0, uM.rangoEscucha) ;
				//Se paran cerca del punto de destino. Generamos un numero aleatorio entre 0 y rangoescucha.
				//Para que se repartan por el espacio y se paren a distintas distancias.
				if (Math.Round(Vector3.Distance (transform.position, destinoTemp.position)) == Math.Round(distanciaParadaPorOrden)
				&& !uM.moveToAttack) {
					moviendose = false;
					uM.isMoving(false);
				}
			}
			else {
				float distanciaDeLlegada;
				//Si el destino es un punto de reunion, se paran a distintas distancias de el
				if (destino.name == "Punto de Reunion)")
					distanciaDeLlegada = uM.rangoEscucha * UnityEngine.Random.value * 4;
				else
					distanciaDeLlegada = uM.rangoEscucha;
				//Si llegan a un punto de destino, cambian al siguiente punto
				if (Vector3.Distance (transform.position, destino.position) < distanciaDeLlegada)
				{
					//Los peatones tienen distintas rutas, el siguiente punto de su ruta es aleatorio.
					if (uM.esPeaton) 
						destinoActual = Mathf.RoundToInt(UnityEngine.Random.value * 10) + 60;
					else {
						uM.isMoving(false);
						if (uM.moveToAttack) {
							uM.moveToAttack = false;
						}
						else {
							destinoActual ++;
						}
					}
					//Asignamos el nuevo destino
					try {
						destino = GameObject.Find("Destino" + destinoActual.ToString()).transform;			
						Debug.Log(name +" Cambia a: Destino" + destinoActual.ToString());
					}
					catch {
						try {
							destino = uM.empatiaPersona.GetComponent<ComportamientoHumano>().destino;
						}
						catch{
							destino = GameObject.Find("Lider Alpha").GetComponent<ComportamientoHumano>().destino;							
						}
					}
				}
			}
		}
	}

	/**************************************
	//Si ENTRAMOS EN CONTACTO con un objeto
	***************************************/
	void OnCollisionEnter (Collision objeto) {
					
		//Si colisiona con un objeto lo esquiva hacia el lado mas logico.
		//Revisar: sustituir los valores de los angulos: 190,210, etc, por la relacion con el destino. 		
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
			uM.valor -= 50;
			uM.energia -= 15;
			uM.estaHerido = true;
		}

		//Si esta atacando y colisiona con su objetivo, el ataque en corto
		if (GetComponent<unitManager> ().estaAtacando && objeto.collider.transform == destinoTemp.transform) {
			//Si tiene mechero le prende fuego, si no es una persona, si no lo ataca en corto.
			if (uM.tieneMechero && objeto.collider.gameObject.layer != 8){
				//Duplicamos el objeto fuegoOrginal y lo posicionamos sobre el objeto quemado
				GameObject fuego = GameObject.Find("Fuego Original"); 
				GameObject fuegoTemp = (GameObject)UnityEngine.Object.Instantiate(fuego, objeto.collider.gameObject.transform.position, transform.rotation);
				fuegoTemp.transform.parent = objeto.collider.gameObject.transform;
				objeto.collider.gameObject.tag = "Ardiendo";
				uM.estaAtacando = false;
			}
			else {
				//se inicia el ataque en corto
				iniciarAccion ("AtaqueCorto");				
				//El cuerpo recibe una fuerza de impacto
				objeto.collider.rigidbody.AddForce (transform.forward * (GetComponent<unitManager> ().fuerza / 10));
				//Y si es persona, recibe impacto
				if (objeto.collider.gameObject.layer == 8)
					objeto.collider.gameObject.GetComponent<unitManager>().recibeImpacto = true;
				//Dejamos de movernos para atacar
				uM.moveToAttack = false;
			}
		}
	}

	/*******************************************
	//MIENTRAS ESTAMOS EN CONTACTO con un objeto
	*********************************************/
	void OnCollisionStay (Collision  objeto) {

		//Si tropieza con una persona parada, nos paramos. Excepto si camina por orden directa. 
		if (objeto.collider.gameObject.layer == 8 && !uM.esLider)  {	
				if (objeto.collider.gameObject.GetComponent<unitManager>().estaParado && !moviendose)
					uM.estaParado = true;
				//Si estamos atacando en corto y tenemos a una persona delante, se considera atacada
				if (ataqueCorto) {
					RaycastHit hit = new RaycastHit();
					Physics.Raycast(transform.position + transform.up*3, transform.forward, out hit, 2);
					if (hit.collider == objeto.collider) {
						objeto.collider.gameObject.GetComponent<unitManager>().recibeImpacto = true;
						ataqueCorto = false;
					}
				}
		}
		//Si colisiona con un objeto de la capa 'Ostaculos' y lo empuja.
		else if ((objeto.collider.gameObject.layer == 13) 
		         && ((uM.energia > 0 && uM.activismo > 30 && uM.esManifestante) || terceraPersona))  {	
			iniciarAccion("Empujando");
			objeto.rigidbody.AddForce(transform.forward);
		}
		//Si colisiona con cualquier otra cosa, que no sea el suelo, comienza a girar para esquivarlo
		else if (!terceraPersona) {			
			if (objeto.collider.tag != "Suelo")
				direccion += 0.3f * setidoGiro;
		}
	}

	//DEJAMOS DE ESTAR EN CONTACTO con un objeto
	void OnCollisionExit (Collision  objeto) {
		if (objeto.collider.tag!= "Suelo")  {	
			setidoGiro = 0;
		}
		//Creo que esta condicion sobra...
		/*if (objeto.collider.tag== "Persona" && !uM.estaParado)  {	
			anim.SetFloat("Speed", 0.03f);
		}*/

		//Si dejamos de estar en contacto con contenedores, cancelamos la animacion
		if (objeto.collider.tag == "Contenedores" )  {	
			anim.SetBool("Empujando", false);
		}
	}

	//Iniciamos la animacion seleccionada
	public void iniciarAccion (string accion) {
		
		anim.SetBool(accion,true);
		
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
				Camera cPersonal = GameObject.Find("CamaraPersonal").camera;
				Camera cPersonal2 = GameObject.Find("CamaraPersonal2").camera;
				cPersonal2.transform.position = cPersonal.transform.position;
				cPersonal2.transform.rotation = cPersonal.transform.rotation;
				//Y ponemos de Main a la CamaraPersonal2
				cPersonal2.depth = 6;
			}
			else {
				Camera cPersonal2 = GameObject.Find("CamaraPersonal2").camera;
				//Y ponemos de Main a la CamaraPersonal2
				cPersonal2.depth = 1;
			}
		}
	}
}
