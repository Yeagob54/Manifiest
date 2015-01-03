/********************************************************************************************
* UnitManager.cs
*
* Todas las unidades de a pie: policias, manifestantes y peatones, llevan este script
* y se encarga de darles las propiedades a cada tipo de unidad. Estas propiedades
* abarcan desde el tipo, los estados, los objetos en mano, las acciones en marcha, etc.
*
* Durante el Update analizamos las personas alrededor y el contexto, para actualizar los 
* estados y acciones de cada unidad.  
*
* (cc) 2014 Santiago Dopazo 
*********************************************************************************************/

using UnityEngine;
using System.Collections;

public class UnitManager : MonoBehaviour {
	//Definimos si los valores de la unidad se rellenaran de forma aleatoria
	public bool aleatorio = false;

	//Variable principal de estado de las personas
	public float energia = 50;//0 a 100

	//Variables de estado de los manifestantes
	public float valor = 3;//-50 a 50
	public float activismo = 0; // 0 a 100

	//Variable de estado del peaton
	public float apoyo = -5;//-50 a 50// Inspired By: // © 2013 Brett Hewitt All Rights Reserved

	//Variable de estado de la policia, podriamos
	public float enfado = 15;//-50 a 50

	//Fase del juego en el que existe este objeto
	public int faseExistencia = 0;

	//Caracterisitcas personales 
	public string nombre;
	public string apellidos = "Sanchez Perez";
	public string creencias = "Ciencia";
	public float salario = 1000;
	public int edad = 30;
	public float prisa = 0.01f; 	
	public Texture2D cara;
	public float fuerza = 200f;//Fuerza de lanzamiento de objetos

	//Persona con la que venimos a la mani. Nuestro colega
	public GameObject empatiaPersona;

	//Persona/objeto con la que interactuara la unidad actual 
	public Transform objetivoInteractuar = null;
	
	//Rango de escucha de la unidad
	public float rangoEscucha = 10.0f;

	//Estados del Ser, ¿que es esta unidad?
	public bool esLider = false;
	public bool esManifestante = true;
	public bool esPeaton = false;
	public bool esPeriodista = false;
	public bool esPolicia = false;
	public bool esCapitan = false;
	public bool esRepartidor = false;	//Pendiente de crear un objeto de mano/pamfletos

	//Control de objetos-en-mano
	public GameObject manoIzquierda;
	public GameObject manoDerecha;
	public GameObject manoVacia;

	//Estados si TIENE algun objeto importante
	public bool tieneMusica = false;
	public bool tieneSpray = false;
	public bool tienePancarta = false;
	public bool tieneMechero = false;
	public bool tieneEscudo = false;
	public bool tieneEscopeta = false;
	public bool tieneMovil = false;
	public bool tienePanfleto = false;
	public bool tieneFlor = false;
	public bool tieneOVNI = false;//Objeto arrojadizo

	//Estados del manifestante
	public bool estaCantando = false;
	public bool estaParado = false;
	public bool estaAgachado = false;
	//Si un policia ve al manifestante comenter un delito
	public bool estaFichado = false;
	//Si cae KO un manifestante fichado, es detenido
	public bool estaKO = false;
	public bool estaPintando = false;
	public bool estaQuemando = false;
	public bool estaLanzando = false;
	public bool estaAtacando = false;
	public bool estaBailando = false;
	public bool estaEnActoPoetico = false; //Acciones especiales
	public bool estaReproduciendoMusica = false;
	public bool estaHerido = false;
	public bool estaHuyendo = false;
	public bool estaOcupado = false;//Peatones que no se uniran a la mani aunque quieran
	public bool escuchoDiscurso = false;

    //Estados circunstanciales
	public bool recibeImpacto = false;
	public bool moveToAttack = false;
	public bool terceraPersona = false;
	public bool arrojandoObjeto = false;

	//Tiempos
	public float tiempoExistencia = 100; // Tiempo que pasa antes de desaparecer
	public float tiempoEnAparecer = 0; //Variable usada para las intros
	private float tiempoCantando = 0;
	private float tiempoBailando = 0;
	private float tiempoSolo = 0;
	private float tiempoAturdido = 0;
	public float tiempoCorriendo = 0; 
	public float tiempoParado = 0f; 

	//Acceso directo a ComportamientoHumano
	ComportamientoHumano comportamientoH;
	//Acceso directo al Animator
	Animator anim;


	//Si debugeamos a esta unidad
	public bool debug = false;

	// Use this for initialization
	void Start () {

		//Asigancionn de valores aleatorios para peatones.
		if (aleatorio) 
			ValoresAleatorios();

		//Creamos un acceso al animator
		anim = GetComponent<Animator>();

		//Creamos un acceso al componenete ComportamientoHumano
		comportamientoH = GetComponent<ComportamientoHumano>();


		//Velocidad al Caminar, de los peatones, apronximadamente igual.
		VelocidadCaminar();

		// Inicializamos la unidad, si está en fase de existencia adecuada
		if (faseExistencia >= Manager.temp.faseJuegoActual) 
			InicializarUnidad();

	}

	// Estados y comportamientos de la unidad
	void Update () {		

		//Actualizamos todas las acciones que se están realizando y los estados de cada unidad
		ActualizarAccionesYEstados();

		//Deteccion de las personas alrededor para actualizar los comportamientos
		PersonasAlrededor();

		//Contorlamos si la unidad sigue siendo manifestante
		if (esManifestante) 
			CondicionesManifestante();

		//Policia atacando
		if (esPolicia && estaAtacando) {
			//Si esta atacando, pero aun no habia empezado a moverse, si tiene un objetivo, a por el...
			if (estaParado && objetivoInteractuar) 
				moverseParaAtacar();
		}

		//Manifestante atacando
		if (esManifestante && estaAtacando) 
			ManifestanteAtacando();		

		//Si desde el GUI estamos arrastrando el raton, vemos si nuestro manifestante esta dentro de ese arrastre
		if (esManifestante && Gui.temp.mouseDrag)		
			SeleccionandoUnidades();	

		//Control de los límites de las variables de estado
		ControlLimites();

		//Debug de estado de la unidad
		if (debug) {
		//	Debug.Log (name + " Energia: " + energia.ToString() + " / Activismo: " + activismo.ToString() + " / Valor: " + valor.ToString());
			Debug.Log ("Esta cantando: " + estaCantando.ToString() + " / Esta acacando: " + estaAtacando.ToString() + " / Esta Reproduciendo: " + estaReproduciendoMusica.ToString());
		}

	}



	//Asignamos valores aleatorios para los peatones. Desafío!
	private void ValoresAleatorios () {

			energia = Random.Range(10,100);//10 a 100
			valor = Random.Range(1,50);//1 a 50
			apoyo = Random.Range(0,100);//0 a 100			
			salario = Mathf.RoundToInt(Random.value * 2500) + 400;
			edad = Mathf.RoundToInt(Random.value * 50) + 15;

			//Un cuarto de los peatones jamas se parara, porque esta ocupado
			estaOcupado = (Mathf.RoundToInt(Random.value * 4) == 1) ? true : false ;

	}

	//Asignamos una velocidad al caminar aproximadamente igual
	private void VelocidadCaminar () {

		if (prisa > 0) {
			prisa = (Mathf.RoundToInt(Random.value * 5) + 1) / 100f;
			if (prisa < 0.2f) prisa = 0.2f;		
		}
		if (!estaParado && !terceraPersona) 
			anim.SetFloat("Speed",prisa);

	}

	//Añadimos la unidad al manager e inicializamos sus objetos de mano
	private void InicializarUnidad () {

		//Añadimos la unidad a la lista de unidades del manager
		Manager.temp.unidades.Add (this.gameObject);	

		if (esManifestante) {

			//Contasbilizamos el manifestante
			Manager.temp.AddManifest ();

			/***********************
			 *   OBJETOS EN MANO
			 * ********************/
			ObjetoDeMano manoIz = manoIzquierda.GetComponent<ObjetoDeMano>();
			ObjetoDeMano manoDer = manoDerecha.GetComponent<ObjetoDeMano>();

			//Si tiene un objeto arrojadizo en una mano entonces tiene un OVNI
			tieneOVNI = (manoIz.esArrojable || manoDer.esArrojable);

			//Comprobamos los objetos en mano que tienen								
			tienePancarta = (manoDer.tag == "Pancarta" || manoIz.tag == "Pancarta" 
			                 || manoDer.name == "flag" || manoIz.name == "flag");				 
			tieneMechero = (manoDer.name == "Mechero Fuego" || manoIz.name == "Mechero Fuego");
			tieneMovil =  (manoDer.name == "Movil" || manoIz.name == "Movil");
			tieneSpray =  (manoDer.name == "Spray" || manoIz.name == "Spray");
			tieneMusica =  (manoDer.name == "Radio" || manoIz.name == "Radio");
			esRepartidor = (manoDer.name == "Panfletos" || manoIz.name == "Panfletos");
			esPeriodista = (manoDer.name == "Camara" || manoIz.name == "Camara"); 

		}
		else if (esPolicia) {

			//Contabilizamos al policía
			Manager.temp.AddPolicias();

			//Control de Objetos en Mano-Policias
			ObjetoDeMano manoIz = manoIzquierda.GetComponent<ObjetoDeMano>();
			ObjetoDeMano manoDer = manoDerecha.GetComponent<ObjetoDeMano>();
			tieneEscopeta =  (manoDer.name == "Escopeta" || manoIz.name == "Escopeta");
			tieneEscopeta =  (manoDer.name == "Escudo" || manoIz.name == "Escudo");
		}
		else if (esPeaton) 			
			//Contabilizamos al peatón
			Manager.temp.AddPeatones();
	}	
	
	//Control de los límites de las variables de estado
	private void ControlLimites () {
		if (energia > 100) energia = 100;
		if (valor > 50) valor = 50;
		if (valor < -50) valor = -50;
		if (activismo > 100) activismo = 100;
		if (activismo < 0) activismo = 0;
		if (enfado > 100) enfado = 100;
		if (apoyo > 100) apoyo = 100;


	}

	/**************************************************
	*			ACCIONES Y ESTADOS
	**************************************************/
	private void ActualizarAccionesYEstados () {

		//************************/
		/*ESTA CAMINANDO/CORRIENDO 
		//************************/			
		if (!estaParado) {

			//Si se cansa, se para. 
			if (valor < 0 && energia < 3) 
				EnMovimiento(false);

		}
		
		//**************
		//TIEMPO CORRIENDO
		//**************			
		if (tiempoCorriendo > 0) {

			//Correr quita energia, pero aumenta el valor y el activismo
			energia -= 0.3f * Time.deltaTime;
			valor += 0.1f * Time.deltaTime;			
			activismo += 0.03f * Time.deltaTime;

			//Si es manifestante aun aumenta más el valor y el activismo al correr
			if (esManifestante){
				valor += 0.2f * Time.deltaTime;
				activismo += 0.02f * Time.deltaTime;
			}
			tiempoCorriendo += Time.deltaTime;

			// Si esta muy cansado deja de correr y vuelve a andar
			if ((energia < 20 && valor > -30)) {
				anim.SetFloat("Speed",prisa);
				tiempoCorriendo = 0;
				estaHuyendo = false;
			}

			//Si esta muy cansado se para a descansar antes de caer KO
			if (energia < 5)
				EnMovimiento(false);
			
		}

		//**************
		//ESTA PARADO
		//**************
		if (estaParado && !terceraPersona) {

			//Detenemos la animacion de caminar
			anim.SetFloat("Speed",0);
			tiempoParado += Time.deltaTime;

			//Mientras esta parado, sube la energia, pero baja el activismo
			energia += 0.3f * Time.deltaTime;
			activismo -= 0.2f * Time.deltaTime;
		}
		else 			
			if (anim.GetFloat("Speed") == 0 && !terceraPersona) {
				//Iniciamos la animacion de caminar
				anim.SetFloat("Speed",prisa);
				tiempoParado = 0;
			}

		//*****************
		//  SALIR CORRIENDO revisar...
		//*****************
		if ((valor < -25 && activismo < Manager.temp.activismoActivista) || (valor < -40 && activismo >= Manager.temp.activismoActivista) 
			    || (valor < 0 && apoyo < 50 && esPeaton) && tiempoCorriendo == 0)

			SalirCorriendo(transform.position);

		//********************
		//REPRODUCIENDO MUSICA
		//********************
		if (estaReproduciendoMusica) {			
			if (!GetComponent<AudioSource>().isPlaying) {

				//Reproducimos el audio
				GetComponent<AudioSource>().Play();

				//Iniciamos la animacion de agacharse a poner musica
				anim.SetBool("Agachado", true);
				estaParado = true;
				estaAgachado = true;

				//Aumenta el ambiente en la manifestacion
				Manager.temp.IncAmbiente(10);
			}
			else if (terceraPersona && estaAgachado) {
				anim.SetBool("Agachado", false);
				estaAgachado = false;
			}
			else if (estaCantando)
				EstaCantando(false);
		}
		else 
			if (tieneMusica) {
				GetComponent<AudioSource>().Stop();
				anim.SetBool("Agachado", false);
			}


		//**************
		//ESTA BAILANDO
		//**************			
		if (estaBailando && !tieneMusica) {

			//Si no estaba bailando iniciamos la animacion de bailar
			if (tiempoBailando == 0) 
				anim.SetBool("Bailando", true);

			tiempoBailando += Time.deltaTime;

			//No se puede protestar y bailar a la vez, de momento...
			EstaCantando(false);

			//Cuanto mas activista, mas tiempo bailara
			if (tiempoBailando < activismo) { 

				//Bailar sube la energia y el valor
				energia += 1f * Time.deltaTime;
				valor   += 0.05f * Time.deltaTime;	
			}
			else
				estaBailando = false;
		}
		else if (!estaBailando && tiempoBailando > 0) {
				anim.SetBool("Bailando", false);
				tiempoBailando = 0;

		}

		//**************
		//ESTA CANTANDO
		//**************			
		if (estaCantando && !tieneMusica && !estaBailando) {

			//Se acaba de activar la opcion, iniciamos la acnimacion
			if (tiempoCantando == 0 || !anim.GetBool("Protestando")) {
				anim.SetBool("Protestando", true);
				EstaCantando(true);
			}
			tiempoCantando += Time.deltaTime;

			//Cantar sube el valor y el activismo. La energia baja, dependiendo del activmismo
			valor += 0.05f * Time.deltaTime;	
			activismo += 0.54f * Time.deltaTime;	
			energia -= 0.2f * Time.deltaTime;

			//Si lleva tiempo cantando para
			if (tiempoCantando > energia + activismo)
				EstaCantando(false);				
		}
		else if (esManifestante) {

			//Si no esta cantando, detenemos la animacion y el audio
			if (!estaCantando && anim.GetBool("Protestando"))
				anim.SetBool("Protestando", false);

			tiempoCantando = 0;
			EstaCantando(false);

		}

		//**************
		//ESTA ATACANDO
		//**************
		//La implemantacion de la accion se lleva a cabo cuando se analizan los objetos de alrededor, 
		//para aprobechar y buscar objetivos. Aqui solo las variaciones del estado
		if (estaAtacando) {

			//Si es muy activista le sube la energia, si no le baja
			energia += ((activismo-75) / 100) * Time.deltaTime; 
			valor += 0.02f * Time.deltaTime;

		}

		//*************************
		// DESAPARICION DE PEATONES
		//*************************
		//Si es peaton y esta mucho tiempo Solo, desaparace
		if (tiempoSolo > tiempoExistencia && esPeaton)
		{

			//Destruimos el peaton
			Manager.temp.unidades.Remove (this.gameObject);			
			Destroy (this.gameObject);

			//reducimos la cantidad de peatones
			Manager.temp.LessPeatones();

		}
	
		//**************
		//RECIVE UN IMPACTO
		//**************		
		if (recibeImpacto) {

			//Cualquier unidad que reciba un impacto pierde valor y energia.
			valor -= 10;
			energia -= 10;
			enfado += 10;
		
			//Hay un 25% de posibilidades de quedar aturdido, si no es el lider, ese no se aturde.
			if (Mathf.Round(Random.value * 4) == 3 && !esLider)			
				tiempoAturdido = (150 - energia + valor)/5;

			//Si es peaton pierde mucho valor, energia y apoyo
			if (esPeaton) {
				valor -= 20;
				energia -= 20;
				apoyo -= 20;
				enfado += 10;
				Manager.temp.LessConciencia(20);
			}

			//Si es manifestante pierde un poco mas que si es activista
			if (esManifestante) {
				valor -= 10;
				energia -= 10;
			}
							
			//Si es activista el valor y el activismo suben, la energia baja igual
			if (activismo >= Manager.temp.activismoActivista) {
				valor += 25;
				activismo += 5;
			}
			
			//Si es lider, solo pierde un poco de energia, el valor y el activismo suben
			if (esLider) {
				valor +=5;
				activismo += 5;
				energia +=5;
			}

			//Si es policia el enfado sube y si tiene escudo, sufre la mitad
			if (esPolicia) {
				enfado += 5;
				if (tieneEscudo) {
					valor += 5;
					energia += 5;
					tiempoAturdido = 0;
					Manager.temp.IncNivelCarga(20);
				}
				else
					Manager.temp.IncNivelCarga(50);
			}

		 	//Hay un 25% de posibilidades de que la unidad quede herida.
			if (Mathf.Round(Random.Range(0, 4)) == 3 && !estaHerido) {
				estaHerido = true;

				if (esPolicia)
					Manager.temp.AddPoliciasHeridos();
				else
					Manager.temp.AddHeridos(name);
			}
			Manager.temp.IncAmbiente(20);
			recibeImpacto = false;

		}

		//**************
		//TIEMPO ATURDIDO
		//**************
		//El tiempo que la persona pasa ATURDIDA depende de la cantidad de energia que tenga y de lo enfadado que este
		if (tiempoAturdido > 0) {		

			tiempoAturdido -= Time.deltaTime;
			energia += 0.1f * Time.deltaTime;
			EnMovimiento(false);

		}	

		//************
		//     K.O.
		//************
		if (energia < 0 && !estaKO) {

			if (esPeaton)
				GameObject.Destroy(this);

			//Animacion de caer KO
			anim.SetBool("KO", true);

			//Desactivamos todo movimiento
			EnMovimiento(false);
			StopAcciones();
			StopAttack();

			//Marcamos esta unidad como KO
			estaKO = true;			
			tag = "KO";

			//Para evitar el loop en la animcion.
			if(!anim.IsInTransition(0)) {
				try {
					anim.animation.Stop();				
				}
				catch{}
			}	
		}

	}


	/*************************************************
	*			PERSONAS ALREDEDOR
	*************************************************/
	private void PersonasAlrededor () {

		//Personas alrededor. Esta accion se va a realizar 1/10 frames estadiasticamente
		bool turnoAnalisis = false;
		float distanciaAlLider;

		//Control de 'objetos alrededor' de la unidad
		Collider[] objectsInRange = new Collider[0];	

		//Solo 1 de cada 5 frames (estadisticamente hablando) analizamos lo que hay alrededor
		if (Random.Range(0,5) == 0) {
			objectsInRange = Physics.OverlapSphere(transform.position, rangoEscucha, 1 << 8);
			turnoAnalisis = true;
		}
		
		//Si hay mas de 2 persona alrededor, tenemos en cuenta su influencia sobre esta unidad
		if (objectsInRange.Length > 2 && turnoAnalisis) {	

			// Variables de influenciacion
			int cuantosParados = 0, cuantosCantando = 0, cuantosPolicias = 0, cuantosAtacando = 0, cuantasPancartas = 0, 
			cuantosManifestantes = 0, cuantosActosPoeticos = 0, cuantosBailando = 0, cuantaPrensa = 0;
			GameObject objetivoCercano = null;
			bool liderCerca = false;
			bool capitanCerca = false;
			bool suenaMusica = false;

			//Hacemos un analisis de lo que ocurre a nuestro alrededor.
			foreach (Collider persona in objectsInRange)  {			

				//Analizamos el UnitManager de cada personaCercana a nuestro alrededor
				UnitManager personaCercana = persona.gameObject.GetComponent<UnitManager>();

				//Incrementamos los contadores de las circunstancias que pueden influir a la aunidad
				if (personaCercana.esManifestante) 
					cuantosManifestantes ++;							
				if (personaCercana.tienePancarta) 
					cuantasPancartas ++;
				if (personaCercana.estaParado) 
					cuantosParados ++;
				if (personaCercana.estaCantando) 
					cuantosCantando ++;				
				if (personaCercana.estaEnActoPoetico)
					cuantosActosPoeticos ++;
				if (personaCercana.esPeriodista)
					cuantaPrensa ++;
				if (personaCercana.estaBailando)
					cuantosBailando ++;			
				if (personaCercana.esCapitan)
					capitanCerca = true;		
				if (!liderCerca && personaCercana.esLider) 
					liderCerca = true;										
				if (!suenaMusica && personaCercana.estaReproduciendoMusica)
					suenaMusica = true;

				//Si esta pintando y esta unidad es policia, lo ficha, lo marca como objetivo y va a por el.
				if((personaCercana.estaPintando || personaCercana.estaQuemando) && esPolicia) {
						objetivoCercano = persona.gameObject;
						personaCercana.GetComponent<UnitManager>().estaFichado = true;
						moverseParaAtacar();
						Manager.temp.IncNivelCarga(30);
						break;
				}					

				//Si un manifestante, no activista, ve quemar algo, se asusta. Si es activista, se anima. 
				if (personaCercana.estaQuemando && esManifestante) {
					if ( activismo < Manager.temp.activismoActivista) {
						valor -= Time.deltaTime;
						activismo -= Time.deltaTime;
					}
					else {
						valor += Time.deltaTime;
						activismo += Time.deltaTime;					
					}
				}				

				//Si hay policías cerca, los manifestantes activistas los marcan como posibles objetivos
				if (personaCercana.esPolicia) {
					cuantosPolicias ++;

					//Buscamos al policia mas cercano, a cada manifestante
					if (estaAtacando && activismo > Manager.temp.activismoActivista && esManifestante) {
						if (objetivoCercano == null) 
							objetivoCercano = persona.gameObject;

						//Si encontramos un policia mas cercano que el anterior, lo asignamos 
						else if (Vector3.Distance(transform.position,objetivoCercano.transform.position) > 
							     Vector3.Distance(transform.position, persona.transform.position))
							objetivoCercano = persona.gameObject;
					}
				}				

				//Si alguien está atacando, puede ser fichado por la policía
				if (personaCercana.estaAtacando) {
					cuantosAtacando ++;			

					//Si este unidad es policia, marca al atacante como objetivo y hay un 
					if (personaCercana.esManifestante && esPolicia) {		

						//Identificamos al atacante mas cercano, como objetivo de a cada policia
						if (objetivoCercano == null) 
							objetivoCercano = persona.gameObject;

						//Si encontramos un atacante mas cercano que el anterior, lo asignamos
						else if (Vector3.Distance(transform.position,objetivoCercano.transform.position) > 
						         Vector3.Distance(transform.position, persona.gameObject.transform.position))
							objetivoCercano = persona.gameObject;	

						//Asignamos ese objetivo como destinoTemp
						comportamientoH.destinoTemp = objetivoCercano.transform;

						//Hay un 25% de posibilidades de que ese manifestante sea fichado
						if (Mathf.Round(Random.value * 4) == 3)			
							persona.gameObject.GetComponent<UnitManager>().estaFichado = true;
					}
				}

			
				//Comportamiento especial solo para Repartidores. 
				if (this.esRepartidor) {
									
					//Si esta muy cerca de esta persona, le damos el panfleto
					if (Vector3.Distance(persona.transform.position,transform.position) <= 1 
						&& !personaCercana.tienePanfleto) {

						//Hacemos que se miren 
						personaCercana.transform.LookAt(transform.position);
						transform.LookAt (personaCercana.transform.position);

						//Si es peaton le sube el 'apoyo', si es manifestante el 'activismo'	
						if (personaCercana.esPeaton)						
							//A mayor salario y mayor edad, menor apoyo.
							personaCercana.apoyo+=(10/(personaCercana.salario/1000))/(edad/30);
						else if (personaCercana.esManifestante)
							personaCercana.activismo+=10;

						//Marcamos que esa persona ya tiene el panfleto
						personaCercana.tienePanfleto = true;

						//Aumenta la barra de conciencia local
						Manager.temp.IncConciencia(1);

						//Si la persona era objetivo, deja de serlo.
						if (objetivoInteractuar == persona.gameObject.transform)
								objetivoInteractuar = null;
					}
					else {
						//Si no tenia objetivo, asignamos uno y vamos hacia el.
						if (!objetivoInteractuar) {
							objetivoInteractuar = persona.gameObject.transform;
							comportamientoH.destino = objetivoInteractuar;
							comportamientoH.moviendose = true;
							EnMovimiento(true);
						}
					}
				}

			  }	//Recuento de influencias finalizado.
			//Pasamos a aplicar los efectos sobre cada tipo de unidad

			
			//Si se ha definido un objetivo cercano, lo asignamos como objetivo con el que interactuar
			if(objetivoCercano)
				objetivoInteractuar	= objetivoCercano.transform;

			//***************
			//   POLICIA
			//**************
			//Modificamos el estado de cada policia, en funcion de lo que ocurre a su alrededor
			if (esPolicia) {
					valor -= ((cuantosManifestantes + cuantosCantando + cuantosBailando - cuantosPolicias * 2) / 100) * Time.deltaTime;
					enfado += ((cuantosManifestantes + cuantosCantando + cuantosBailando + cuantosAtacando - cuantosPolicias * 2) / 100) * Time.deltaTime;
					energia += (cuantosPolicias - cuantasPancartas)/ 100 * Time.deltaTime;

					if (capitanCerca) {
						valor += 0.1f * Time.deltaTime;	
						energia += 0.03f * Time.deltaTime;	
					}

					//Si recibio un impacto, ficha al objetivo mas cercano
					if (recibeImpacto && objetivoCercano)
						objetivoCercano.GetComponent<UnitManager>().estaFichado = true;

					//Dependiendo de con cuantos policías esté puede detener o no. 
						//Desafío: El policía se agacha y el manifestante está tumbado, si está así x tiempo la detención se completa
					if (cuantosPolicias > 3)
						GetComponent<ComportamientoPolicia>().puedeDetener = true;
					else
						GetComponent<ComportamientoPolicia>().puedeDetener = false;

					//Salir corriendo
					if (valor < -30)
						SalirCorriendo(transform.position + transform.forward);
			}

			//***************
			//    PEATON
			//**************
			//Modificamos el estado de cada peaton, en funcion de lo que ocurre a su alrededor
			if (esPeaton) {
				apoyo += (cuantosManifestantes + cuantasPancartas + cuantosBailando - cuantosPolicias) / (salario / (100 - edad)) 
						+ (cuantosCantando / (salario / (500 - edad * 3))) * Time.deltaTime;
				valor -= cuantosAtacando * ((salario / 1000) + (edad / 100))* Time.deltaTime;
				energia += (apoyo * cuantosCantando) / 100 * Time.deltaTime;

				// Si no tiene manifestantes alrededor, incrementamos el contador de tiempoSolo, si no lo reiniciamos
				if (cuantosManifestantes == 0)
					tiempoSolo += Time.deltaTime;
				else
					tiempoSolo = 0;

				/*******************************************************
				// CONDICIONES PARA CONVERTIR UN PEATON EN MANIFESTANTE
				********************************************************/
				float distanciaLider = Vector3.Distance(transform.position, Manager.temp.liderAlpha.transform.position);
				if (apoyo > (25 + (salario / 500) - ((65 - edad) / 10)) && energia > 10 && valor > 0 && !estaOcupado 
					&& distanciaLider < Manager.temp.distanciaMaximaLider / 2) {
					esPeaton = false;
					esManifestante = true;		
					tag = "Manifestantes";		

					//BUscamos al lider y le ponemos el mismo destino
					comportamientoH.destino = Manager.temp.liderAlpha.GetComponent<ComportamientoHumano>().destino;
					empatiaPersona = Manager.temp.liderAlpha;

					//Actualizamos los contadores e manifestantes y peatones, del manager
					Manager.temp.AddPeatonToManifestante();
					Manager.temp.LessPeatones();
					Manager.temp.IncConciencia(20);

					//Le damos un grado de apoyo, valor y activismo minimos
					if (valor < 0) 
						valor += cuantosManifestantes + cuantosCantando;
					if (apoyo < Manager.temp.activismoActivista) 
						apoyo = Manager.temp.activismoActivista + cuantosManifestantes + cuantasPancartas;
					activismo += cuantosManifestantes;

					//Añadimos al log el suceso
					Manager.temp.sucesos.Add (name + " se unio a la manifestacion.");	
				}
			}

			//***************
			// MANIFESTANTE
			//***************
			//Modificamos el estado de cada manifestante, en funcion de lo que ocurre a su alrededor
			if (esManifestante) {				
				energia += cuantosBailando / 100 * (Time.deltaTime);
				activismo += (cuantasPancartas + cuantosManifestantes) / 100 + (cuantosCantando/50) - (cuantosAtacando / 100) * Time.deltaTime;
				valor  -= ((cuantosPolicias + cuantosAtacando/10) - ((cuantosManifestantes + cuantosCantando) / 100) 
					- cuantosBailando / 100)* Time.deltaTime;

				//Si el lider esta cerca, influencia en las unicades
				if (liderCerca) {
					activismo += (valor / 200f) * Time.deltaTime;
					valor += (activismo / 200f) * Time.deltaTime;
				}

				//Control del audio-canticos en tercera persona. Si hay gente cerca cantando o no
				if (terceraPersona)
					if ( cuantosCantando == 0 && !estaCantando)
						Manager.temp.audioCanticosPersonal.mute = true;
					else
						Manager.temp.audioCanticosPersonal.mute = false;

				//El lider nunca deja de ser activista
				if (esLider && activismo < Manager.temp.activismoActivista) activismo = 55;
									
				//Si suena musica y hay energia, baila
				if (suenaMusica && activismo > 20 && !estaCantando){ 
						if (!terceraPersona) {
							estaBailando = true;
						}
						else
							comportamientoH.suenaMusica = true;
				}
				else { 
					//Revisar: no se porqué da erro esto!!
					try {
						if ((estaBailando || comportamientoH.suenaMusica) && !suenaMusica){
							comportamientoH.suenaMusica = false;
							estaBailando = false;
						}
					}catch{}
				}

				//Si esta solo o con poca gente, va cerca de su persona empatia, si ya esta cerca, se van con el lider. 
				if (cuantosManifestantes < 2) { 
					activismo -= (valor / 200f) * Time.deltaTime;
					valor -= (activismo / 200f) * Time.deltaTime;
					if (Vector3.Distance(empatiaPersona.transform.position, transform.position) > rangoEscucha)
						comportamientoH.destinoTemp = empatiaPersona.transform;
					else
						comportamientoH.destinoTemp = Manager.temp.liderAlpha.transform;
					//Iniciamos el movimiento por orden directa
					comportamientoH.moviendose = true;
				}

				//Si esta cantando solo o casi solo, deja de cantar
				if (estaCantando &&  tiempoCantando > cuantosCantando * (activismo / (10-cuantosCantando))) {
					EstaCantando (false);
					tiempoCantando = 0f; 
				}

				//Algoritmo para, si no esta cantando, empezar a cantar, REVISO!!!
				else if (!estaCantando && cuantosCantando > 3 && (energia + activismo) > (200 / cuantosCantando)) {
					EstaCantando (true);
					tiempoCantando += Time.deltaTime; 
				}

				//***************
				//   ACTIVISTA
				//**************
				if (activismo >= Manager.temp.activismoActivista) {
					//los Activistas tambien son manifestantes, asi que esto se suma a lo anterior
					energia += (cuantosManifestantes + cuantosCantando) / 100  - (Time.deltaTime/10);
					apoyo   += (cuantosManifestantes + cuantosCantando) / 100 + (cuantosAtacando / 100);
					valor  += (cuantosPolicias / 200) + (cuantosAtacando / 100);

					//El lider nunca pierde el valor del todo
					if (esLider && valor < 0) valor = 10;
				}
			}
		}//Analisis, para < 2 personas alrededor
		else if (turnoAnalisis) {
			//Si está lejos del lider, su valor, energía y activismo bajan mucho más rápido
			distanciaAlLider = Vector3.Distance(transform.position, Manager.temp.liderAlpha.transform.position);
			if (esManifestante && Manager.temp.marchaIniciada && distanciaAlLider > Manager.temp.distanciaMaximaLider/2) {
				energia -= (Time.deltaTime) * 5;
				activismo -= (Time.deltaTime) * 15;
				valor -= (Time.deltaTime) * 10;
			}
		}
	}

	/*********************************************
	//C0NDICI0NES PARA DEJAR DE SER MANIFESTANTE
	/********************************************/
	private void CondicionesManifestante () {
		
		float distanciaAlLider = Vector3.Distance(transform.position, Manager.temp.liderAlpha.transform.position);

		// SI tiene mucho miedo o esta muy cansado, se convierte en peaton.
		if ((valor < -20 && activismo < Manager.temp.activismoActivista) || valor < -40 
			|| (energia < 10 && activismo < Manager.temp.activismoActivista ) 
		    || energia <= 0 || distanciaAlLider > Manager.temp.distanciaMaximaLider) {
			esPeaton = true;
			tag = "Peatones";
			esManifestante = false;
			StopAttack();
			StopAcciones();

			try {
				//Asignamos un destino de el peaton padre 
				comportamientoH.destino = GameObject.Find("Generador de Peatones 1").
									 GetComponent<Generador>().destinoCercano;

				//Lo ponemos en movimiento hacia alli
				EnMovimiento(true);

			}catch{}

			//Actualizamos los contadores e manifestantes y peatones, del manager
			Manager.temp.AddPeatones();
			Manager.temp.LessManifest();
			Manager.temp.LessConciencia(10);

			//Si el lider Alpha deja la mani, ponemos como lider a otro manifestante
			if (esLider) {
				esLider = false;					
				Manager.temp.NuevoLider();					
			}

			//Añadimos al log el suceso
			Manager.temp.sucesos.Add (name + " deja de ser manifestante.");	

			//Si estaba en tercera persona, salimos
			if (terceraPersona)
				Gui.temp.saliendoTerceraPersona(this.gameObject);

			//Desseleccionamos esta unidad					
			selectedManager.temp.deselect(this.gameObject);					
		}
	}

	/***************************************************
	*			MANIFESTANTE ATACANDO
	***************************************************/
	private void ManifestanteAtacando () {

		//Si esta atacando, pero no tiene un objetivo
		if (!objetivoInteractuar || objetivoInteractuar.tag == "Ardiendo" || objetivoInteractuar.tag == "KO")  

			//Le buscamos uno
			objetivoInteractuar = buscarObjetivo();	
		else {

			//El manifestante mira hacia el objetivo
			transform.LookAt(objetivoInteractuar.position);

			// Si el objetivo esta fuera dle rango de escucha, nos movemos hacia el
			if (Vector3.Distance(this.transform.position, objetivoInteractuar.position) > rangoEscucha) {
				moverseParaAtacar();
			}

			//Si el objetivo esta a tiro, iniciamos el ataque
			else {

				//Si tiene objto arrojadizo y no esta arrojando, iniciamos ataque a distancia
				if (tieneOVNI && !arrojandoObjeto) {

					//Indicamos que accion/animacion, desde la cual se llama a accionArrojar();
					GetComponent<ComportamientoHumano>().iniciarAccion("Arrojar");
					//accionArrojar();
				}
				//SI no tiene objeto arrojadizo, nos movemos hacia el objetivo para atacar cuerpo a cuerpo
				else 					
					moverseParaAtacar();
			}
		}

	}

	/**************************************
	//		SELECCIONANDO UNIDADES
	/*************************************/
	private void SeleccionandoUnidades () {

		//Posiciones del cuadro de arrastre
		Vector3[] dragPos = Gui.temp.dragLocations();

		//Posicion del manifestante actual en pantalla.
		Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

		//Si esta dentro, lo marcamos como seleccionado.
		if (screenPos.x > dragPos[0].x && screenPos.x < dragPos[1].x && screenPos.y > dragPos[0].y && screenPos.y < dragPos[1].y)
			if (!GetComponent<selected>().isSelected)
				Gui.temp.seleccionarUnidad (this.gameObject);
		else
			if (GetComponent<selected>().isSelected)
				Gui.temp.desseleccionarUnidad (this.gameObject);

	}

	/**************************************
	//		MOVERSE PARA ATACAR
	/*************************************/
	public void moverseParaAtacar() {

		//Indicamos el objetivo a atacar e iniciamos el movimiento por orden directa
		if (objetivoInteractuar) {
			if (objetivoInteractuar.tag == "Coches")
				this.GetComponent<ComportamientoHumano>().destinoTemp = objetivoInteractuar.parent;
			else
				this.GetComponent<ComportamientoHumano>().destinoTemp = objetivoInteractuar;
		}
		this.GetComponent<ComportamientoHumano>().moviendose = true;

		//marcador para dibujar lineas de ataque
		moveToAttack = true;
		estaAtacando = true;

		//Si esta parado iniciara el movimiento
		EnMovimiento(true);

	}

	//Buscamos el objeto atacable mas cercano
	public Transform buscarObjetivo() {

		Transform objetoCercano = null;
		Collider[] objectsInRange = new Collider[0];	

		//Posibles objetivos alrededor
		objectsInRange = Physics.OverlapSphere(transform.position, rangoEscucha);

		//Hacemos un analisis de los objetos a nuestro alrededor.
		foreach (Collider objeto in objectsInRange)  {

			//Si hay algun objeto atacable, lo marcamos como objetivo
			if (objeto.tag == "Contenedores" || objeto.tag == "Policias" || objeto.tag == "Destruible"
			    || objeto.tag == "Coches"|| objeto.tag == "Destruibles") {

				//Buscamos el objeto atacable mas cercano
				if (!objetoCercano) 
					objetoCercano = objeto.gameObject.transform;
				else if (Vector3.Distance(this.transform.position,objeto.transform.position) 
				         < Vector3.Distance(this.transform.position,objetoCercano.position))
						objetoCercano = objeto.gameObject.transform;
			}
		}

		return objetoCercano;

	}

	//Detenemos las acciones que se estan llevando a cabo
	public void StopAcciones()
	{
		estaBailando = false;
		EstaCantando (false);
		estaReproduciendoMusica = false;
		estaEnActoPoetico = false;

	}

	//Detenemos el ataque
	public void StopAttack()
	{
		moveToAttack = false;
		estaAtacando = false;
		GetComponent<Animator> ().SetBool ("AtaqueCorto",false);
		//Reiniciamos los componenetes de rotación del manifestante, pues a veces se descolocan al atacar
		transform.rotation = Quaternion.Euler(0, transform.rotation.y, 0); 

	}

	//Iniciamos o detenemos el movimiento de una unidad
	public void EnMovimiento(bool m)
	{

		//desactivamos/activamos el movimiento, si no esta KO
		if (!estaKO) {
			estaParado = !m;
			tiempoParado += Time.deltaTime;
			if (!m) {
				tiempoCorriendo = 0;
				estaHuyendo = false;
			}
		}
		else 
			estaParado = true;

	}

	//Detenemos el lanzamiento de objetos
	public void stopArrojar()
	{
		if (arrojandoObjeto) {
			arrojandoObjeto = false;
			GetComponent<ComportamientoHumano>().cambioCamara(false);

			//Correccion de un giro que da cuando arroja una piedra
			transform.Rotate(Vector3.up,80);
		}

	}

	//Iniciamos el lanzamiento de objetos
	public void accionArrojar()
	{
		//Descubrimos en que mano tiene el objeto y lo arrojamos 
		if (manoIzquierda.GetComponent<ObjetoDeMano>().esArrojable) {

			//Arrojamos una copia del objeto arrojable de la mano izquierda
			Arrojar(manoIzquierda.GetComponent<ObjetoDeMano>().objetoArrojar);

			//Objetos infinitos. Descomentar para objeto unico
			//manoIzquierda = manoVacia;
		}
		else if (manoDerecha.GetComponent<ObjetoDeMano>().esArrojable) {		

			//Arrojamos una copia del objeto arrojable de la mano izquierda
			Arrojar(manoDerecha.GetComponent<ObjetoDeMano>().objetoArrojar);

			//Objetos infinitos. Descomentar para objeto unico
			//manoDerecha = manoVacia;
		}
		else 
			tieneOVNI = false;

		//Para que no se quede atascado en la animación, la finalizamos después de cada lanzamiento. 
		GetComponent<ComportamientoHumano>().finalizarAccion("Arrojar");

		//SI ES MUY ACTIVISTA, TIENE MAS OBJETOS EN LA MOCHILA?? (Desafio: objetos en la mochila)
	}

	//Accion de arrojar un objeto
	public void Arrojar (GameObject objetoArrojar) {

		//Correccion del eje de rotacion modificado por la animacion
		//transform.Rotate(Vector3.up,40);
		//Generamos una copia del objeto fisico generico que vamos a arrojar. 
		GameObject objetoArrojado = (GameObject)UnityEngine.Object.Instantiate(objetoArrojar, 
		                                                                       transform.position + transform.forward + transform.up*3, 
		                                                                       transform.rotation);

		//Si estamos debugeando esta unidad mostramos la linea de fuerza en el lanzamiento
		if (debug) {
			Debug.DrawLine(objetoArrojado.transform.position, objetoArrojado.transform.position + 
			               (transform.forward * fuerza + Vector3.up * fuerza));
			Debug.Log("Fuerza arrojada: " + fuerza.ToString());
		}

		//Si no estamos en tercera persona la fuerza será proporcional a la distancia
		if (!terceraPersona && objetivoInteractuar != null)
			fuerza = Vector3.Distance(objetivoInteractuar.position, transform.position) * 3f;

		//Calculamos el vector direccion de lanzamiento:  adelante + arriba
		Vector3 forceDirection = (transform.forward * fuerza + Vector3.up * fuerza/2);

		//Añadimos la fuerza de lanzamiento al objeto clonado
		objetoArrojado.GetComponent<Rigidbody>().AddForce(forceDirection, ForceMode.Impulse);

		//Asignamos las varaiables de estado del objeto arrojado
		objetoArrojado.GetComponent<ObjetoDeMano>().enElSuelo = false;
		objetoArrojado.GetComponent<ObjetoDeMano>().enVuelo = true;
	}

	//Salir corriendo
	public void SalirCorriendo (Vector3 deQue) {

		//Miramos a aquello de lo que huimos, para dar media vuelta
		transform.LookAt(deQue);

		//Media vuelta y a correr
		transform.rotation.Set(0,180,0,0);			
		GetComponent<Animator>()	.SetFloat("Speed",1f);
		estaHuyendo = true;

		//Detenemos toda acción previa
		StopAttack();
		StopAcciones();

		//Indicamos que esta corriendo
		tiempoCorriendo += Time.deltaTime;
	}

	//Está Cantando
	public void EstaCantando(bool flag) {

		estaCantando = flag;

		//Actualizamos el volumen de los cánticos y la cantidad de manifestntes cantando		
		Manager.temp.CuantosCantando(flag);

	}
}
