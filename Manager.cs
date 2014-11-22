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
	const int MINIMO_MANIFESTANTES = 5;
	
	//Modificacion de interfaces y controles para modo tablet
	public bool versionTablet = false;

	//Referencia al lider de la manifestacion
	public GameObject liderAlpha;

	//Referencia al punto de encuentro inicial
	public GameObject puntoReunion;

	//Referencia a las cámaras
	public Camera mainMenuCamera;

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

	private float repercusionMediaticaMaxima = 500;	
	private float ambienteManifestacionMaxima = 500; 
	private float nivelConcienciaLocalMaxima = 500; 

	//Configuracion de la sensibilidad de los objetivos alcanzados
	public int nivelRepercusionMinima = 100;
	public int nivelRepercusionObjetivo = 300;
	public int nivelRepercusionEpica = 700;

	//Configuracion de la sensibilidad de la policia y los 3 niveles de carga.
	private int nivelCarga = 0;
	public int nivelCargaLeve = 300;
	public int nivelCargaFuerte = 550;
	public int nivelCargaTotal = 700;

	//Cuanto activismo es necesario para que un manifestante sea activista
	public int activismoActivista = 50; 

	//Velociadad a la que transcurre el tiempo en el juego
	public float velocidadTiempo = 1;

	//Puntos de destino que conforman el recorrido inicial de la manifestacion
	public int totalPuntosRecorrido = 8;

	//Objetivos de la manifestacion
	public string objetivoConseguido= "Conseguido";
	public bool marchaIniciada = false;
	public bool ministerioTrabajo = false;
	public bool xGrafitis = false;
	public bool xManifestantes = false;	
	public int grafitisPintados = 0;

	//Si se ha iniciado una carga policial
	public bool cargaIniciada = false;
	
  	//Puntero a si mismo, para acceder a la instancia
	public static Manager temp;

	//Musica cuando se inicia una carga policial
	public AudioClip musicaCarga;

	//Canticos posibles de los manifestantes
	public AudioClip[] canticos;
	private int cantidadCanticos;
	private int cancionActual;	
	public bool yaEstanCantando = false;


	// Inicializamos
	void Start () 	{
		temp = this;	

		//Mostramos el objetivo incial
		//Desafio: crear clase Objetivos.
		objetivos.Add ("[Objetivo] Selecciona a los manifestantes e inicia la marcha.");

		//Cuantos cánticos hay cargados
		cantidadCanticos = canticos.Length;	

	}
	

	/************************************************************
	 * AÑADIR Y QUITAR MANIFESTANTES, COCHES, PEATONES Y POLICIAS
	 * **********************************************************/
	public void AddManifest()	{
		numeroDeManifestantes ++;
		if (numeroDeManifestantes > maximoDeManifestantes)
			maximoDeManifestantes = numeroDeManifestantes;
	}
	public void AddPeatonToManifestante()	{

		objetivos.Remove ("[Objetivo] Convierte a " + (30 - peatonesConvertidos).ToString() +" peatones en manifestantes.");
		numeroDeManifestantes ++;
		peatonesConvertidos ++;
		//Si ya se cumplio el objetivo del ministerio de trabajo, actualizamos el objetivo de los grafitis
		if (ministerioTrabajo)
			objetivos.Add ("[Objetivo] Convierte a " + (30 - peatonesConvertidos).ToString() +" peatones en manifestantes.");
	}
	
	public void LessManifest()	{
		numeroDeManifestantes --;
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

	public void AddHeridos()	{
		numeroDeHeridos ++;
		Manager.temp.IncRepercusion(30);
		Manager.temp.IncAmbiente(20);
	}

	public void AddPoliciasHeridos()	{
		numeroDePoliciasHeridos ++;
		Manager.temp.IncRepercusion(100);
	}

	public void AddDetenidos()	{
		numeroDeDetenidos ++;
		Manager.temp.IncRepercusion(50);
		Manager.temp.IncAmbiente(20);
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
			Manager.temp.IncRepercusion(50);
			Manager.temp.IncAmbiente(20);
		}
	}
	
	public float GetNivelCarga() {
		return nivelCarga;
	}

	private void IniciarMusicaCarga () {
		//Musica cañera para acompañar la carga
		Camera.main.gameObject.audio.clip = musicaCarga;			
		Camera.main.gameObject.audio.Play();
			
	}

	public AudioClip getCancionActual() {
		if (!yaEstanCantando) {
			yaEstanCantando = true;
			cancionActual = Random.Range(1,cantidadCanticos);
		}
		return canticos[cancionActual];
	}

	/****************************
	*       CARGA POLICIAL
	****************************/
	public void IniciarCarga() {
		if (!cargaIniciada) {
			IniciarMusicaCarga();
			cargaIniciada = true;
			//Añadimos al log el suceso
			sucesos.Add ("¡Se ha iniciado una carga policial!");
			//Hacemos que cada policia empiece a cargar
			foreach (GameObject Poli in unidades) {
				//Si es policia y no coche, inicia ataque
				if (Poli.tag == "Policias" && Poli.layer == LayerMask.NameToLayer("Personas")) {
					Poli.GetComponent<UnitManager>().moverseParaAtacar();
					//Si no tiene un objetivo, va a por el lider
					if (!Poli.GetComponent<UnitManager>().objetivoInteractuar)
						Poli.GetComponent<ComportamientoHumano>().destinoTemp = liderAlpha.transform;
				}
			}
		}
	}


	//Mensajes de objetivos relacionado con los grafitis pintados
	public void addGrafiti() {
		objetivos.Remove ("[Objetivo] Pinta " + (10 - grafitisPintados).ToString() + " garfitis a lo largo del recorrido.");
		grafitisPintados ++;
		//Si ya se cumplio el objetivo del ministerio de trabajo, actualizamos el objetivo de los grafitis
		if (ministerioTrabajo)
			objetivos.Add ("[Objetivo] Pinta " + (10 - grafitisPintados).ToString() + " garfitis a lo largo del recorrido.");
	}

	//Función para saber si la manifestación se ha disuelto
	public bool TerminaLaManifestacion() {

		return (numeroDeManifestantes < MINIMO_MANIFESTANTES);
	}

	/******************************
	*       REINICIAR EL JUEGO
	******************************/
	public void ReiniciarJuego() {
		mainMenuCamera.depth = 6; 
		//Mejor un LoadScene... 
		repercusionMediatica = ambienteManifestacion = nivelConcienciaLocal = numeroDeManifestantes = 
		numeroDePeatones = numeroDePolicias = numeroDeCoches = maximoDeManifestantes = peatonesConvertidos = 0;
		}

}
