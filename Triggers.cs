/********************************************************************************************
* Gui.cs
*
* Eventos lanzados por objetos de tipo trigger. 
* Control de eventos dentro del juego. 
* Tipos de trigger: 
*	- Objetivo Comenzar Marcha
*	- Objetivo Ministerio de Trabajo
*	- Resetear la posicion de los coches
*	- Destruir objetos que entren
	- Cambiar el recorrido de la manifestación
	- Misión cumplida
	- Iniciar carga
	- Cambiar fase del juego
*
* (cc) 2014 Santiago Dopazo 
*********************************************************************************************/

using UnityEngine;
using System.Collections;

public class Triggers : MonoBehaviour {

	public bool comenzarMarcha = false;
	public bool ministerioTrabajo = false;
	public bool resetCoches = false;
	public bool destruirCoches = false;
	public bool destruirPeatones = false;
	public bool cambiarRecorrido = false;
	public bool iniciarCarga = false;
	public bool iniciarDisturbios = false;
	public bool misionCumplida = false;
	public bool cambiarDeFase = false;
	public bool detenerCarga = false;
	public int cambiarAFase;

	//Variables de control de Objetivo: Ministerio de Trabajo
	private int numeroManifestantes = 0;

	public bool debug = false;

	public Transform objetoDisturbios;

	void OnTriggerEnter(Collider col) {
		
		 if (col.tag == "Manifestantes" && col.gameObject.GetComponent<UnitManager>().esLider) {

			//Si llegan al ayuntamiento, consigen el objetivo recorrido
		 	if (misionCumplida) 
				MisionCumplida();
				
			//Primera misión: Comenzar Marcha
			if (comenzarMarcha) 
				ComenzarMarcha(col);
			//Segunda misión: Ministerio de Trabajo
			else if (ministerioTrabajo)
				MinisterioTrabajo(col);
			//Cambio de recorrido	
			else if ( col.gameObject.GetComponent<UnitManager>().esLider 
				&& cambiarRecorrido && !Manager.temp.recorridoSecundario)
				RecorridoSecundario(col);

			//Iniciar carga policial
			if (iniciarCarga)
				Manager.temp.IniciarCarga();

			//Iniciar disturbios entre los manifestantes
			if (iniciarDisturbios)
				Gui.temp.IniciarDisturbios(objetoDisturbios);

			//Detener carga
			if (detenerCarga) 
				Manager.temp.DetenerCarga();			

			//Cambiar de fase de juego
			if (cambiarDeFase)
				Manager.temp.CambiarFaseJuego(cambiarAFase);


		}
		//Destruir coches y peatones
		else if (col.tag == "Coches") {
			if (destruirCoches) {
				Manager.temp.unidades.Remove(col.gameObject);
				Destroy (col.gameObject);
			}
			//Devolver coches a su punto de origen		
			else if (resetCoches)
				ResetCoches(col);	
		}	
		else if (destruirPeatones && col.tag == "Peatones") {
			Manager.temp.unidades.Remove(col.gameObject);
			Destroy (col.gameObject);	
		}

	}

	void OnTriggerStay(Collider col) {

		//Cada en cada frame, si el objetivo aun no se haya cumplido, actualizamos el número de manifestantes protestando
		if (col.gameObject.tag == "Manifestantes" && ministerioTrabajo) {

			//Referencial al Unit Manager de cada manifestante
			UnitManager uM = col.gameObject.GetComponent<UnitManager>();

			//Añadimos a los manifestantes que cumplan la condición
			if (uM.estaCantando && uM.estaParado)

				//Aumentamos el número de manifestantes
				numeroManifestantes ++;		
		}

	}

	public void FixedUpdate() {

		//Recontamos en cada frame cuantos manifestantes hay 
		if (ministerioTrabajo && Manager.temp.ministerioTrabajo) {		

			//Comprobamos si se ha cumplido el objetivo			
			ActualizaObjetivoMinisterio();
		}

	}

	//Control de manifestantes cumpliendo la condición. Actualizamos el objetivo
	private void ActualizaObjetivoMinisterio() {

		//Si se cumplió el objetivo de 10 manifestantes protestando ante el ministerio, activamos el siguiente objetivo
		if (numeroManifestantes >= 10 && !Manager.temp.objetivoRecorrido) {

			//Eliminamos el objetivo antiguo
			Manager.temp.objetivos.Remove("[Objetivo] 10 manifestantes parados y protestando ante el Ministerio de Trabajo.");

			//Añadimos el suceso
			Manager.temp.sucesos.Add ("[Conseguido] 10 manifestantes protestando ante el Ministerio de Trabajo.");


			if (debug)
				Debug.Log("[Ministerio de Trabajo] Objetivo Conseguido!!!");

			//Deshabilitamos este trigger
			ministerioTrabajo = false;	

			//Añadimos los objetivos nuevos
			Manager.temp.objetivos.Add ("[Objetivo] Convierte a " + (30 - Manager.temp.peatonesConvertidos).ToString() +
								   " peatones en manifestantes.");		
			Manager.temp.objetivos.Add ("[Objetivo] Llega hasta el ayuntamiento");

			/*Manager.temp.objetivos.Add ("[Objetivo] Pinta " + (5 - Manager.temp.grafitisPintados).ToString() + 
				                       " garfitis a lo largo del recorrido.");	*/	

			//Cambiamos a la siguiente fase del juego
			Manager.temp.CambiarFaseJuego(3);

			//Incrementamos la conciencia y el impacto mediático
			Manager.temp.IncConciencia(300);
			Manager.temp.IncRepercusion(150);


			if (debug)
				Debug.Log("[Ministerio de Trabajo] Objetivos Nuevos añadidos!!!");			
		}

		//Reiniciamos el numero de manifestantes en cada frame, para el recuento
		numeroManifestantes = 0;

	}

	//Misión: 10 manifestantes parados protestando en este trigger
	private void MinisterioTrabajo(Collider col) {

		//La primera vez que llega uno, activamos el objetivo
		if (!Manager.temp.ministerioTrabajo) {
			Manager.temp.ministerioTrabajo = true;				
			Manager.temp.sucesos.Add ("Los manifestantes empiezan a llegar al Ministerio de Trabajo.");
			Manager.temp.objetivos.Remove ("[Objetivo] Protestar ante el ministerio de trabajo.");
			Manager.temp.objetivos.Add ("[Objetivo] 10 manifestantes parados y protestando ante el Ministerio de Trabajo.");
		}

	}

	//Reiniciamos la posición de los coches que impacten con este trigger
	private void ResetCoches (Collider col) {

		//Si el objeto es de la capa 9, coches, lo reseteamos
		if (col.gameObject.layer == 9) 
				col.gameObject.GetComponent<ComportamientoCoche>().Reset();

	}

	//Modificación de la posición de los puntos de destino y repintado de la linea de recorrido
	private void RecorridoSecundario (Collider col){
		Manager.temp.recorridoSecundario = true;

		//Asignamos la nueva posición de los destinos del recorrido
		Manager.temp.destinos[4].transform.position = Manager.temp.destinosAlternativos[0].transform.position;
		Manager.temp.destinos[5].transform.position = Manager.temp.destinosAlternativos[1].transform.position;
		Manager.temp.destinos[6].transform.position = Manager.temp.destinosAlternativos[2].transform.position;

		//Mostramos el suceso
		Manager.temp.sucesos.Add("Se ha cambiado la ruta de la manifestacion!");

		//Redibujamos el recorrido
		Gui.temp.DibujarLineaRecorrido();

	}


	//Se inicia la manifestacion
	private void ComenzarMarcha(Collider col) {		

		//Si el lider entra en esta zona, comenzamos la marcha
		if(col.gameObject == Manager.temp.liderAlpha) {
			if (!Manager.temp.marchaIniciada) {

				//Hacemos que el destino sea el punto 2 ya y lo contagiamos a todos
				Manager.temp.liderAlpha.GetComponent<ComportamientoHumano>().destinoActual = 1;
				Manager.temp.liderAlpha.GetComponent<ComportamientoHumano>().destino = GameObject.Find("Destino1").transform;		
				Manager.temp.marchaIniciada = true;

				//Añadimos al log el suceso
				Manager.temp.sucesos.Add ("Los manifestantes han comenzado la marcha.");
				Manager.temp.objetivos.Add ("[Objetivo] Protestar ante el ministerio de trabajo.");	
				Manager.temp.objetivos.Add ("[Objetivo] Pinta " + (5 - Manager.temp.grafitisPintados).ToString() + " grafitis.");

				//Activamos la música
				Manager.temp.audioMusica.audio.Play();
				Manager.temp.audioMusica.GetComponent<ListaMusicas>().reproduciendoMusica = true;

				//Hacemos que todos los manifestantes dejen de ir al punto de encuentro 
				//y comiencen a recorrer los puntos del recorrido
				Gui.temp.IniciarMarcha();

			}
		}
	}	

	//ultimo trigger de la demo
	private void MisionCumplida () {
		Manager.temp.objetivoRecorrido = true;
		Manager.temp.objetivos.Remove ("[Objetivo] Llega hasta el ayuntamiento.");
		Manager.temp.sucesos.Add ("[Conseguido] Llega hasta el ayuntamiento.");
		Manager.temp.objetivos.Add ("[Objetivo] Resiste la carga policial.");

		//Incrementamos la conciencia y el impacto mediático
		Manager.temp.IncConciencia(400);
		Manager.temp.IncRepercusion(250);
	}
}
