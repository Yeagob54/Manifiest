/********************************************************************************************
* Gui.cs
*
* Eventos lanzados por objetos de tipo trigger. 
* Control de eventos dentro del juego. 
* Tipos de trigger: 
*	- Objetivo Comenzar Marcha
*	- Objetivo Ministerio de Trabajo
*	- Resetear la posicion de los coches
*	- Destruir cualquier objeto que entre
*
* (cc) 2014 Santiago Dopazo 
*********************************************************************************************/

using UnityEngine;
using System.Collections;

public class Triggers : MonoBehaviour {

	public bool comenzarMarcha = false;
	public bool ministerioTrabajo = false;
	public bool resetCoches = false;
	public bool destruirObjetos = false;

	private bool protestando = false;	

	//Variables de control de Objetivo: Ministerio de Trabajo
	private int numeroManifestantes;

	//Referencia al Manager.temp

	public void Start() {
	}

	void OnTriggerEnter(Collider col) {
		if (comenzarMarcha) 
			ComenzarMarcha(col);
		if (ministerioTrabajo)
			MinisterioTrabajo(col);
		if (resetCoches)
			ResetCoches(col);	
		if (destruirObjetos){
			Manager.temp.unidades.Remove(col.gameObject);
			Destroy (col.gameObject);
		}

	}

	//Se inicia la manifestacion
	private void ComenzarMarcha(Collider col) {		
		//Si el lider entra en esta zona, comenzamos la marcha
		if(col.gameObject == Manager.temp.liderAlpha) {
			if (!Manager.temp.marchaIniciada) {
				Manager.temp.marchaIniciada = true;
				//Añadimos al log el suceso
				Manager.temp.sucesos.Add ("Los manifestantes han comenzado la marcha.");
				Manager.temp.objetivos.Add ("[Objetivo] Protestar ante el ministerio de trabajo.");
				
				//Hacemos que todos los manifestantes dejen de ir al punto de encuentro 
				//y comiencen a recorrer los puntos del recorrido
				foreach (GameObject g in Manager.temp.unidades) {
					if (g.layer != 9) {
						if (g.GetComponent<UnitManager>().esManifestante) {
							g.GetComponent<UnitManager>().isMoving(true);
							g.GetComponent<ComportamientoHumano>().moviendose = false;
						}
					}
				}
			}
		}
	}


	//Controlamos la cantidad de manifestantes que están en la zona 
	private void MinisterioTrabajo(Collider col) {
			

		//Si llega un manifestante...
		if (col.gameObject.tag == "Manifestantes") {	

			//La primera vez que llega uno, activamos el objetivo
			if (!Manager.temp.ministerioTrabajo) {
				Manager.temp.ministerioTrabajo = true;				
				Manager.temp.sucesos.Add ("Los manifestantes empiezan a llegar al Ministerio de Trabajo.");
				Manager.temp.objetivos.Remove ("[Objetivo] Protestar ante el ministerio de trabajo.");				
			}

			//Aumentamos el número de manifestantes en la zona			
			llegaManifestanteAlMinisterioTrabajo();

			//Detectamos si está protestando
			if (!protestando && col.gameObject.GetComponent<UnitManager>().estaCantando 
				&& col.gameObject.GetComponent<UnitManager>().estaParado) {
				protestando = true;

			}
			//Si se cumple el objetivo de 10 manifestantes protestando ante el ministerio
			else if (numeroManifestantes >= 10 ) {
				//Eliminamos el objetivo antiguo
				Manager.temp.objetivos.Remove ("[Objetivo] " + (10 - numeroManifestantes).ToString() + 
			   	" Manifestantes, parados, protestando ante el Ministerio de Trabajo.");			
				Manager.temp.sucesos.Add ("[Conseguido] 10 manifestantes protestando ante el Ministerio de Trabajo.");
				//Añadimos objetivos nuevos
				Manager.temp.objetivos.Add ("[Objetivo] Pinta 10 garfitis a lo largo del recorrido.");
				Manager.temp.objetivos.Add ("[Objetivo] Convierte a 30 peatones en manifestantes.");	
				//Deshabilitamos las colisiones
				GetComponent<BoxCollider>().enabled = false;			
			}
		}
	}

	private void llegaManifestanteAlMinisterioTrabajo() {

		//Borramos el objetivo anterior
		Manager.temp.objetivos.Remove ("[Objetivo] " + (10 - numeroManifestantes).ToString() + 
		   " Manifestantes, parados, protestando ante el Ministerio de Trabajo.");			
		//Aumentamos el número de manifestantes
		numeroManifestantes ++;
		//Añadimos el nuevo objetivo
		Manager.temp.objetivos.Add ("[Objetivo] " + (10 - numeroManifestantes).ToString() + 
		   " Manifestantes, parados, protestando ante el Ministerio de Trabajo.");			

	}

	//Reiniciamos la posición de los coches que impacten con este trigger
	private void ResetCoches (Collider col) {

		//Si el objeto es de la capa 9, coches, lo reseteamos
		if (col.gameObject.layer == 9) 
				col.gameObject.GetComponent<ComportamientoCoche>().Reset();
	}
}
