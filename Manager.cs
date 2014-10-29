/******************************************************************************
 * manager.cs
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

	//Puntero a si mismo, para instanciarse
	public static manager temp;

	//Listado de objetos-unidad del juego 
	public List<GameObject> unidades = new List<GameObject>();

	//Control de la cantidad de manifestantes, peatones, policias y coches
	private int numeroDeManifestantes=0, numeroDePeatones=0, numeroDePolicias=0, numeroDeCoches=0;	

	//Barras de estado de la manifestacion y de objetivos
	private float repercusionMediatica = 0;	
	private float ambienteManifestacion = 0; 
	private float nivelConcienciaLocal = 0; 

	//Configuracion de la sensibilidad de los objetivos alcanzados
	public int nivelRepercusionMinima = 100;
	public int nivelRepercusionObjetivo = 300;
	public int nivelRepercusionEpica = 700;

	//Configuracion de la sensibilidad de la policia y los 3 niveles de carga.
	public int nivelCargaLeve = 30;
	public int nivelCargaFuerte = 50;
	public int nivelCargaTotal = 70;

	//Velociadad a la que transcurre el tiempo en el juego
	public float velocidadTiempo = 1;
	
	// Inicializamos
	void Start () 	{
	  //Creamos en temp una instancia de la clase. 
		temp = this;	
	}
	
	/************************************************************
	 * AÑADIR Y QUITAR MANIFESTANTES, COCHES, PEATONES Y POLICIAS
	 * **********************************************************/
	public void addManifest()
	{
		numeroDeManifestantes ++;
	}
	
	public void lessManifest()
	{
		numeroDeManifestantes --;
	}
	
	public int getManifest()
	{
		return numeroDeManifestantes;
	}

	public void addPolicias()
	{
		numeroDePolicias ++;
	}
	
	public void lessPolicias()
	{
		numeroDePolicias --;
	}
	
	public int getPolicias()
	{
		return numeroDePolicias;
	}
	
	public void addCoches()
	{
		numeroDeCoches ++;
	}
	
	public void lessCoches()
	{
		numeroDeCoches --;
	}
	
	public int getCoches()
	{
		return numeroDeCoches;
	}

	public void addPeatones()
	{
		numeroDePeatones ++;
	}
	
	public void lessPeatones()
	{
		numeroDePeatones --;
	}
	
	public int getPeatones()
	{
		return numeroDeCoches;
	}

	/*****************************************************************
	 *    MODIFICACION Y SOLICITAR VALOR DE LAS BARRAS DE OBJETIVO
	 * ***************************************************************/
	public float incRepercusion(float cuanto) {
		repercusionMediatica += cuanto;
	}

	public float getRepercusion() {
		return repercusionMediatica;
	}

	public float incAmbiente(float cuanto) {
		ambienteManifestacion += cuanto;
	}
	
	public float getAmbiente() {
		return ambienteManifestacion;
	}

	public float incConciencia(float cuanto) {
		nivelConcienciaLocal += cuanto;
	}
	
	public float getConciencia() {
		return nivelConcienciaLocal;
	}

}
