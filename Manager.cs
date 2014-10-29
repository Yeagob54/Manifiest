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

	//Puntero a si mismo, para acceder a la instancia
	public static Manager temp;

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
		temp = this;	
	}
	

	/************************************************************
	 * AÑADIR Y QUITAR MANIFESTANTES, COCHES, PEATONES Y POLICIAS
	 * **********************************************************/
	public void AddManifest()
	{
		numeroDeManifestantes ++;
	}
	
	public void LessManifest()
	{
		numeroDeManifestantes --;
	}
	
	public int GetManifest()
	{
		return numeroDeManifestantes;
	}

	public void AddPolicias()
	{
		numeroDePolicias ++;
	}
	
	public void LessPolicias()
	{
		numeroDePolicias --;
	}
	
	public int GetPolicias()
	{
		return numeroDePolicias;
	}
	
	public void AddCoches()
	{
		numeroDeCoches ++;
	}
	
	public void LessCoches()
	{
		numeroDeCoches --;
	}
	
	public int GetCoches()
	{
		return numeroDeCoches;
	}

	public void AddPeatones()
	{
		numeroDePeatones ++;
	}
	
	public void LessPeatones()
	{
		numeroDePeatones --;
	}
	
	public int GetPeatones()
	{
		return numeroDeCoches;
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
	
	public float GetConciencia() {
		return nivelConcienciaLocal;
	}



}
