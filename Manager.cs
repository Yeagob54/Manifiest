/******************************************************************************
 * Manager.cs
 * 
 * Clase global, instanciada en temp, para controlar la cantidad de manifestantes, 
 * peatones, coches y policias. 
 * 
 * Control de las barras de objetivo: Repercusion mediatica,
 * ambiente de la manifestacion y nivel de conciencia local. 
 * 
 * **************************************************************************/

using UnityEngine;
using System.Collections.Generic;

public class Manager : MonoBehaviour {

	//Constantes del juego
	const int MINIMO_MANIFESTANTES = 14;
	
	//Modificacion de interfaces y controles para modo tablet
	public bool versionTablet = false;

	//Audio Sources de efectos, musica y canticos
	public AudioSource audioFx;
	public AudioSource audioMusica;
	public AudioSource audioCanticos;
	public AudioSource audioFxPersonal;
	public AudioSource audioMusicaPersonal;
	public AudioSource audioCanticosPersonal;

	//Musicas del juego
	public AudioClip[] musicasJuego;

	//Tiempo en poder volver a convocar con el movil
	public float tiempoMovil = 0;
	public float tiempoReinicioMovil = 30;

	//Referencia al lider de la manifestacion
	public GameObject liderAlpha;

	//Distancia máxima que puede estar un manifestante del lider, sin dejar de serlo
	public float distanciaMaximaLider = 200f;

	//Referencia al punto de encuentro inicial
	public GameObject puntoReunion;

	//Referencia al objeto latente, para controlar la creación destrucción de objetos/unidades
	public GameObject objetoLatente;

	//Referencia al control de la cámara aerea
	public GameObject controlCamara;

	//Referencias a 5 manifestantes, 3 activistas y 2 artistas
	public GameObject cincoManifestantes;
	public GameObject tresActivistas;
	public GameObject dosArtistas;

	//Referencias a los destinos de la manifestación
	public Transform [] destinos = new Transform[9];
	public Transform [] destinosAlternativos = new Transform[3];

	//Puntos de destino que conforman el recorrido inicial de la manifestacion
	public int totalPuntosRecorrido = 8;


	//Determina los objetos de zona que son creados/destruidos. 
	public int faseJuegoActual = 0;

	//Si se cambió a la ruta secundaria de la manifestación
	public bool recorridoSecundario = false;

	//Referencia a objetos clonables, originales
	public GameObject grafitiOriginal;
	public GameObject fuegoOriginal;
	public GameObject piedraOriginal;	
	public GameObject furgonOriginal;

	//Referencia a las camaras
	public Camera mainMenuCamera;
	public Camera mainCamera;
	public Camera miniMapaCamera;
	public Camera personaCamera1;
	public Camera personaCamera2;

	//Listado de las unidades-manifestantes del juego 
	public List<GameObject> unidades = new List<GameObject>();

	//Listado/Log de sucesos en el juego
	public List<string> sucesos = new List<string>();

	//Listado de objetivos actuales
	public List<string> objetivos = new List<string>();

	//Control de la cantidad de manifestantes, peatones, policias y coches
	private int numeroDeManifestantes = 0, numeroDePeatones = 0, numeroDePolicias = 0, numeroDeCoches = 0;

 	//Maximo de manifestantes alcanzado en esta partida y cantidad de peatonesConvertidos
	public int maximoDeManifestantes = 0, peatonesConvertidos = 0;

	//Control de la cantidad de heridos y detenidos
	public int numeroDeHeridos = 0, numeroDePoliciasHeridos = 0, numeroDeDetenidos = 0;	

	//Barras de estado de la manifestacion y de objetivos
	private float repercusionMediatica = 0;	
	private float ambienteManifestacion = 0; 
	private float nivelConcienciaLocal = 0; 

	private static float puntuacionMaxima = 0;

	private float repercusionMediaticaMaxima = 3000;	
	private float ambienteManifestacionMaxima = 3000; 
	private float nivelConcienciaLocalMaxima = 3000; 

	//Configuracion de la sensibilidad de los objetivos alcanzados
	public int nivelRepercusionMinima = 100;
	public int nivelRepercusionObjetivo = 300;
	public int nivelRepercusionEpica = 700;

	//Configuracion de la sensibilidad de la policia y los 3 niveles de carga.
	private int nivelCarga = 0;
	public int nivelCargaLeve = 200;
	public int nivelCargaFuerte = 550;
	public int nivelCargaTotal = 700;

	//Cuanto activismo es necesario para que un manifestante sea activista
	public int activismoActivista = 50; 

	//Velociadad a la que transcurre el tiempo en el juego
	public float velocidadTiempo = 1;

	//Objetivos de la manifestacion
	public string objetivoConseguido= "Conseguido";
	public bool marchaIniciada = false;
	public bool ministerioTrabajo = false;	
	public bool objetivoGrafiti = false;
	public bool objetivoRecorrido = false;
	public bool objetivoPeatones = false;
	public bool xGrafitis = false;
	public bool xManifestantes = false;	
	public int grafitisPintados = 0;

	//Si se ha iniciado una carga policial
	public bool cargaIniciada = false;
	
  	//Puntero a si mismo, para acceder a la instancia
	public static Manager temp;

	//Musica cuando se inicia una carga policial
	public AudioClip musicaCarga;
	public AudioClip sonidoAmbiente;

	//Musica, Fx y Cántivos activados/desactivados
	public static bool canticosOn = true;			
	public static bool musicaOn = true;
	public static bool fxOn = true;
		
	//Canticos posibles de los manifestantes
	public AudioClip[] canticos;
	private int cantidadCanticos;
	private int cuantosCantando = 0;	
	public bool yaEstanCantando = false;

	//Referencia al script MensajesAyuda
	public MensajesAyuda mensajesAyuda;


	// Inicializamos
	void Start () 	{

		temp = this;	

		//Mostramos el objetivo incial
		//Desafio: crear clase Objetivos.
		objetivos.Add ("[Objetivo] Selecciona a los manifestantes e inicia la marcha.");

		//Inicializamos la referencia al script
		mensajesAyuda = GetComponent<MensajesAyuda>();
	
		//Mostramos el primer mensaje del tutorial
		mensajesAyuda.MostrarMensaje(1);

		//Cuantos cánticos hay cargados
		cantidadCanticos = canticos.Length;

		//Activar/desactivar los AudioSource adecuado
		audioCanticos.mute = !canticosOn;
		audioMusica.mute = !musicaOn;
		audioFx.mute = !fxOn;

	}

	void Update () 	{

		//Exito máximo
		if (objetivoRecorrido && objetivoPeatones && objetivoGrafiti)
			Gui.temp.EndGameWon();

		//Menos de X manifestantes, la manifestación ha sido disuelta
		if (TerminaLaManifestacion())
			Gui.temp.EndGameLost();
	}

	/************************************************************
	 * AÑADIR Y QUITAR MANIFESTANTES, COCHES, PEATONES Y POLICIAS
	 * **********************************************************/
	public void AddManifest()	{

		numeroDeManifestantes++;
		if (numeroDeManifestantes > maximoDeManifestantes)
			maximoDeManifestantes = numeroDeManifestantes;
		if (numeroDeManifestantes > 100)
			Gui.temp.EndGameWon();

	}

	public void AddPeatonToManifestante()	{

		//Estaba dando problemas, así que nos curamos en salud...
		//No hacer nunca así. Desafío, mejorar esto.
		for (int x = 0 ; x < 6 ; x++)
			objetivos.Remove ("[Objetivo] Convierte a " + (30 - peatonesConvertidos).ToString() + " peatones en manifestantes.");

		numeroDeManifestantes ++;
		peatonesConvertidos ++;
		
		if (peatonesConvertidos >= 30) {
			objetivoPeatones = true;

			//Añadimos el suceso
			Manager.temp.sucesos.Add ("[Conseguido] 30 peatones convertidos en manifestantes.");
			IncConciencia(200);
		}			

		//Si ya se cumplio el objetivo del ministerio de trabajo, actualizamos el objetivo de los peatones
		if (faseJuegoActual > 0 && !objetivoPeatones)
			objetivos.Add ("[Objetivo] Convierte a " + (30 - peatonesConvertidos).ToString() + " peatones en manifestantes.");		
	}

	//Mensajes de objetivos relacionado con los grafitis pintados
	public void addGrafiti() {
		//Estaba dando problemas, así que eliminamos todos los mensajes
		//No hacer nunca así. Desafío, mejorar esto.
		for (int x = 0 ; x < 6 ; x++)
			objetivos.Remove ("[Objetivo] Pinta " + x.ToString() + " grafitis.");

		//Incrementamos el contador de garfitis pintados
		grafitisPintados ++;

		if (grafitisPintados >= 5) {
			objetivoGrafiti = true;

			//Añadimos el suceso
			Manager.temp.sucesos.Add ("[Conseguido] 5 garfitis pintados.");			
			IncConciencia(150);
			IncAmbiente(50);
		}

		//Si ya comenzó la marcha, actualizamos el objetivo de los grafitis
		if (!objetivoGrafiti)
			objetivos.Add ("[Objetivo] Pinta " + (5 - grafitisPintados).ToString() + " grafitis.");		
	}

	
	public void LessManifest()	{
		numeroDeManifestantes--;
	}
	
	public int GetManifest()	{
		return numeroDeManifestantes;
	}

	public void AddPolicias()	{
		numeroDePolicias ++;
	}
	
	public void LessPolicias()	{
		numeroDePolicias --;
	}
	
	public int GetPolicias()	{
		return numeroDePolicias;
	}
	
	public void AddCoches()	{
		numeroDeCoches ++;
	}
	
	public void LessCoches()	{
		numeroDeCoches --;
	}
	
	public int GetCoches()	{
		return numeroDeCoches;
	}

	public void AddPeatones()	{
		numeroDePeatones ++;
	}
	
	public void LessPeatones()	{
		numeroDePeatones --;
	}
	
	public int GetPeatones()	{
		return numeroDeCoches;
	}

	/**************************************
	 * AÑADIR Y QUITAR HERIDOS Y DETENIDOS
	 * ************************************/

	public void AddHeridos(string name) {

		numeroDeHeridos ++;
		IncRepercusion(30);
		IncAmbiente(20);
		LessConciencia(10);
		//Añadimos al log el suceso
		sucesos.Add (name + " ha resultado herido/a.  Total herido/as: " + numeroDeHeridos.ToString());	

	}

	public void AddPoliciasHeridos() {

		numeroDePoliciasHeridos ++;
		IncRepercusion(100);
		LessConciencia(30);

		//Añadimos al log el suceso
		sucesos.Add ("Un policia ha resultado herido/a.  Total policias heridos: " + numeroDePoliciasHeridos.ToString());	

	}

	public void AddDetenidos(GameObject detenido) {

		string nombre = detenido.GetComponent<UnitManager>().nombre;

		//Detenemos todas sus acciones
		detenido.GetComponent<UnitManager>().StopAcciones();

		//Eliminamos al detenido de la lista de unidades y de seleccionados
		unidades.Remove(detenido);
		selectedManager.temp.objects.Remove(detenido);

		//Incrementamos las variables de estado del juego
		numeroDeDetenidos ++;
		LessManifest();
		IncRepercusion(50);
		IncAmbiente(20);		
		LessConciencia(20);

		//Añadimos al log el suceso
		sucesos.Add (nombre + " ha sido detenido. Total detenidos: " + numeroDeDetenidos.ToString());	

		//Si el lider es detenido, asignamos un nuevo lider
		if (detenido.GetComponent<UnitManager>().esLider)
			NuevoLider();

		//Destruimos el objeto
		Destroy(detenido);

	}


	/*****************************************************************
	 *    MODIFICACION Y SOLICITAR VALOR DE LAS BARRAS DE OBJETIVO
	 * ***************************************************************/
	public void IncRepercusion(float cuanto) {
		repercusionMediatica += cuanto;
	}

	public float GetRepercusion() {
		return repercusionMediatica;
	}

	public void IncAmbiente(float cuanto) {
		ambienteManifestacion += cuanto;
	}
	
	public float GetAmbiente() {
		return ambienteManifestacion;
	}

	public void IncConciencia(float cuanto) {
		nivelConcienciaLocal += cuanto;
	}

	public void LessConciencia(float cuanto) {
		nivelConcienciaLocal -= cuanto;
	}

	public float GetConciencia() {
		return nivelConcienciaLocal;
	}

	/*Maximos de las barras de objetivo
	 */
	public float GetRepercusionMediaticaMaxima(){
		return repercusionMediaticaMaxima;
	}
	public void SetRepercusionMediaticaMaxima(float R){
		repercusionMediaticaMaxima = R;
	}
	public float GetAmbienteManifestacionMaxima(){
		return ambienteManifestacionMaxima;
	}
	public void SetAmbienteManifestacionMaxima(float A){
		ambienteManifestacionMaxima = A;
	}

	public float GetNivelConcienciaLocalMaxima(){
		return nivelConcienciaLocalMaxima;
	}
	public void SetNivelConcienciaLocalMaxima(float N){
		nivelConcienciaLocalMaxima = N;
	}


	public void IncNivelCarga(int cuanto) {

		nivelCarga += cuanto;
		if (nivelCarga > nivelCargaLeve && !cargaIniciada) {
			IniciarCarga();
			IncRepercusion(100);
			IncAmbiente(50);
		}

	}
	
	public float GetNivelCarga() {
		return nivelCarga;
	}

	private void IniciarMusicaCarga (bool encender) {

		if (encender){
			//Musica cañera para acompañar la carga
			audioMusica.clip = musicaCarga;
			audioMusica.Play();
			audioMusica.loop = true;
			audioMusica.GetComponent<ListaMusicas>().reproduciendoMusica = false;
		}
		else {
			audioMusica.Stop();
			audioMusica.loop = false;
			audioMusica.GetComponent<ListaMusicas>().reproduciendoMusica = true;
		}
			
	}

	/****************************************
	*				CANTAR
	****************************************/

	public void Cantar() {

		if (!yaEstanCantando) {
			yaEstanCantando = true;
			audioCanticos.clip = canticos[Random.Range(1,cantidadCanticos)];
			audioCanticos.Play();
		}

	}

	public void StopCantar() {

		if (yaEstanCantando) {
			yaEstanCantando = false;
			audioCanticos.Stop();
		}

	}

	public void CuantosCantando(bool flag) {
	
		//Incrementamos o decrementamos el contador de manifestantes cantando
		cuantosCantando+= flag? 1:-1;

		//Modificamos el volumen de los cánticos dependiendo de cuantos estén cantado
		audioCanticos.volume = cuantosCantando / 100f;		


		if (cuantosCantando > 0) 
			Cantar ();
		else {
			StopCantar();
			cuantosCantando = 0;
		}

	}

	/****************************
	*       CARGA POLICIAL
	****************************/
	//Estaría bien recibir como parámetro el Policía-Capitán que inicia la carga
	public void IniciarCarga() {

		if (!cargaIniciada) {
			IniciarMusicaCarga(true);
			cargaIniciada = true;

			//Añadimos al log el suceso
			sucesos.Add ("Se ha iniciado una carga policial!");				

			//Hacemos que cada policia empiece a cargar
			foreach (GameObject Poli in unidades) {

				//Si es policia y no coche, inicia ataque
				if (Poli && Poli.tag == "Policias" && Poli.layer == LayerMask.NameToLayer("Personas")) {
					Poli.GetComponent<UnitManager>().moverseParaAtacar();

					//Si no tiene un objetivo, va a por el lider
					if (!Poli.GetComponent<UnitManager>().objetivoInteractuar)
						Poli.GetComponent<ComportamientoHumano>().destinoTemp = liderAlpha.transform;
				}
			}
		}

	}

	public void DetenerCarga() {

		if (cargaIniciada) {
			ComportamientoHumano cM;
			IniciarMusicaCarga(false);
			cargaIniciada = false;

			//Añadimos al log el suceso
			sucesos.Add ("La policia ha dejado de cargar.");

			//Hacemos que cada policia empiece a cargar
			foreach (GameObject Poli in unidades) {

				//Si es policia y no coche, inicia ataque
				if (Poli && Poli.tag == "Policias" && Poli.layer == LayerMask.NameToLayer("Personas")) {
					cM = Poli.GetComponent<ComportamientoHumano>();
					Poli.GetComponent<UnitManager>().estaAtacando = false;

					//Cada policía vuelve a su destino inial
					cM.destinoTemp = cM.destino;
					cM.moviendose = true;
				}
			}
		}

	}

	//Función para saber si la manifestación se ha disuelto
	public bool TerminaLaManifestacion() {
		return (numeroDeManifestantes < MINIMO_MANIFESTANTES);
	}

	/****************************************
	*       CAMBIANOS FASE DEL JUEGO
	*****************************************/
	public void CambiarFaseJuego(int fase) {
		faseJuegoActual = fase;

		//Cada vez que cambiamos de fase, destruimos todos los peatones
		DestruirPeatones();
	}

	// DESTRUIR PEATONES
	private void DestruirPeatones() {

		//Creamos una lista de peatones
		foreach (GameObject g in GameObject.FindGameObjectsWithTag("Peatones")) {
			unidades.Remove (g);
			Destroy (g.gameObject);			
		}

	}

	// CANTICOS ON OFF
	public void CanticosOnOff(){
		canticosOn = !canticosOn;
	}
	public bool EstadoCanticos(){
		return canticosOn;
	}
	// SONIDO ON OFF
	public void musicaOnOff(){
		musicaOn = !musicaOn;
	}
	public bool estadoMusica(){
		return musicaOn;
	}
	public void fxOnOff(){
		fxOn = !fxOn;
	}
	public bool estadoFx(){
		return fxOn;
	}	

	public void SetPuntuacionMaxima(float puntuacion) {
		puntuacionMaxima = puntuacion;
	}
	public float GetPuntuacionMaxima() {
		return puntuacionMaxima;
	}
	//NUEVO LIDER
	public void NuevoLider(){
		foreach (GameObject unidad in unidades) {
			//Si es manifestante, es el nuevo lider
			if (unidad && unidad.tag == "Manifestantes") {
				liderAlpha = unidad;
				unidad.GetComponent<UnitManager>().esLider = true;
				break;
			}
		}
	}

	public void Reset (){
		foreach (GameObject unidad in unidades) {
			Destroy(unidad);
		}
		unidades.Clear();
	}

}
