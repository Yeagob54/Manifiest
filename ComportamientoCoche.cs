/*******************************************************
* ComportaminetoCoche.cs
*
* Características y tipos de coche, dependiendo de su comportamiento
* con respecto a los manifestantes. 
*
*  Algoritmo de conducción autimática, detectando a peatones y edificios,
*  recorriendo puntos de destino secuenciales. 
*
* (cc) 2014 Santiago Dopazo
********************************************************/

using UnityEngine;

public class ComportamientoCoche : MonoBehaviour {
	
	//Deteccion de personas alrededor
	private Collider[] personasAlrededor;
	public float rango = 20.0f;
	
	//Variables de direccion, velocidad y frenado
	private float direccion = 0f;
	private float gas = 0f;
	public float velocidad = 0.2f;
	public float vida = 100f;
	public float velocidadGiro = 3f;
	public float velocidadMaxima = 10f;
	public float fuerzaFrenada = 5f;
	public float distanciaGiro = 20f;
	public float distanciaFrenado = 3f;
	public bool estaParado = false;
	public bool estaAparcado = false;
	//Margen para evitar que el ray impacte contar el propio coche
	public int frontalOffSet = 3;
	public int lateralOffSet = 2;
	//Fase del juego en el que existe el coche
	public int faseExistencia = 0;

	//Distancia a las paredes laterales, frontales y posibles personas alrededor
	float[] distancia = {0,0,0,0};
	
	//Puntos de destino
	public Transform destino;
	private bool cambiandoDestino = false;
	public byte destinoActual = 52;
	
	//Tipo de seguimiento de la manifestacion
	public bool delanteMani = false;
	public bool detrasMani = false;
	public bool esPolicia = false;
	
	//Tiempo antes de arrancar
	public float tiempoEspera = 0f;

	//Activamos el debug sobre este objeto
	public bool debug = false;

	//Posicion inicial, para reiniciar el coche
	private Vector3 posicionInicial;
	
	
	/******************
	 *      START
	 * ***************/
	void Start () {
		//Si no se inicializo el destino, lo inicializamos nosotros. 
		if (!destino) {
			try {
				destino = GameObject.Find("Destino" + destinoActual.ToString()).transform;
			}
			catch {
				destino = transform;
			}
		}

		//Guardamos la posicion inicial del vehiculo
		posicionInicial = transform.position;

		//Añadimos el vehiculo al manager
		if (faseExistencia >= Manager.temp.faseJuegoActual) {
			Manager.temp.AddCoches ();
			Manager.temp.unidades.Add (this.gameObject);	
		}

	}
	
	/********************
	 *      UPDATE
	 * ******************/
	void FixedUpdate () {

		TiempoEspera();

		//Si no está aprcado, roto o ardiendo...
		if (!estaAparcado && tag != "Ardiendo" &&  tag != "KO") {
			DeteccionPersonasAlrededor();

			RaycastFrontal();

			RaycastLateral();

			AceleracionDireccion();

			PuntosDeDestino();		
		}

	}


	//*********************************************
	//   DETECCION DE PERSONAS ALREDEDOR (RANGO)
	//*********************************************
	private void DeteccionPersonasAlrededor() {
		personasAlrededor = Physics.OverlapSphere(transform.position, rango, 1 << 8);	
		float angulo = 0.0f; 
		Vector3 personaDir;
		
		//Si hay alguna persona dentro del rango
		if (personasAlrededor.Length > 0)	{		    

			//Inspeccionamos todas las personas alrededor, para ver si estan por delante o por detras
			foreach (Collider persona in personasAlrededor)  {
				
				//Calculamos el vector direccion de la persona detectada, el angulo con respecto a 'forward' y la distancia
				personaDir = persona.gameObject.transform.position - transform.position;
				personaDir.Normalize();
				angulo = Vector3.Angle (-transform.forward, personaDir);
				if (distancia[3] < Vector3.Distance(transform.position,persona.gameObject.transform.position))
					distancia[3] = Vector3.Distance(transform.position,persona.gameObject.transform.position);
				
				//Si estamos detras de la mani, tenemos que esperar a que los manifestantes avencen
				if (persona.tag == "Manifestantes" && detrasMani) {
					gas = 0;
					estaParado = true;
					break;
				}				
				//Persona delante, cerca del coche, frenamos
				else if (angulo < 15) {
					gas -= velocidad * fuerzaFrenada; 
					if (distancia[3] <= distanciaFrenado)
						estaParado = true;
				}
				//Si la persona no esta delante y esta lejos
				else if (distancia[3] > distanciaFrenado) {
					//Si controla la mani desde alante, avanza
					if (delanteMani)
						estaParado = false;
					/*gas += velocidad;
						//Si estan delante de la mani, avanzan solo si hay manifestantes alrededor.
						else if (persona.tag == "Manifestantes")
							gas += velocidad;
						estaParado = false;*/
				}
				
			}//for
			if (!estaParado) gas += velocidad;
		}
		//Si no hay nadie en el rango...
		else { 
			//Si es policia delante de la mani, espera
			if (delanteMani) {
				estaParado = true;
			}
			//Si no, acelera
			else {
				//Si esta detras de la mani, solo avanza si la marcha se inicio
				if (!detrasMani || (detrasMani && Manager.temp.marchaIniciada)){
					gas += velocidad;			
					estaParado = false;
				}
			}
		}

	}

	// ***************
	// RAYCAST FRONTAL
	// ***************
	///Desde la posicion del coche, hacia adelante.
	private void RaycastFrontal() {
		RaycastHit hit = new RaycastHit();
		int capaImpacto;

		hit.point = Vector3.zero;
		Physics.Raycast (transform.position - transform.forward * frontalOffSet, -transform.forward, out hit, distanciaGiro);
		//Mostramos este ray en el debug en rojo
		if (debug)
			Debug.DrawLine (transform.position - transform.forward * frontalOffSet, 
		                transform.position - transform.forward * (frontalOffSet + distanciaGiro) , Color.red);
		
		//Si el Raycast ha impactado con algo a la distacia de giro
		if (hit.point != Vector3.zero || cambiandoDestino) {
			
			//Calculamos la distancia al punto de impacto
			distancia[0] = Vector3.Distance(transform.position, hit.point );
			capaImpacto = hit.collider.gameObject.layer;
			
			//Si hay un edificio o una acera en la linea del ray, cerca, giramos hacia el objetivo
			if (capaImpacto == ((1 << 10) | (1 << 11)) || cambiandoDestino) {
				
				//Calculamos si el destino esta a la izquierda o a la derecha
				float dista3 = Vector3.Distance(destino.position, transform.position + transform.right);
				float dista4 = Vector3.Distance(destino.position, transform.position - transform.right);
				//Decidimos el sentido de giro.
				if (dista3 > dista4)
					direccion +=  velocidadGiro;
				else
					direccion -=  velocidadGiro;
				
				cambiandoDestino = false;
			}
			//En cualquier caso hay algo delante, asi que frenamos un poco...
			gas -= velocidad*(1/fuerzaFrenada);
			
			//Si tiene a un coche o persona delante, frena.
			if (capaImpacto == (LayerMask.NameToLayer("Coches") | LayerMask.NameToLayer("Personas"))) {
				gas -= velocidad*(1/fuerzaFrenada);
				//Distasncia al vehiculo de delante
				if (distancia[0] < distanciaFrenado) {
					estaParado = true;
					gas = 0;
				}
			if (debug)
				Debug.Log("RAycast>> name:" + hit.collider.gameObject.name + " / Distancia: " + distancia[0].ToString());

			}

		}
		//Si no tenemos ningun obstaculo delante, seguimos recto.
		else
			direccion = 0;
	}


	// *****************
	// RAYCAST LATERALES (pendiente de lanzarlos contra la acera, no los edificios)
	// *****************
	private void RaycastLateral() {
		RaycastHit hit = new RaycastHit();
		hit.point = Vector3.zero;

		//Lanzamos un rycast a der para saber la distancia a los objetos de la derecha
		Physics.Raycast (transform.position + transform.right * lateralOffSet, transform.right, out hit, distanciaGiro/4);
		//Mostramos este ray en el debug en azul
		if (debug)
			Debug.DrawLine (transform.position + transform.right * lateralOffSet, 
		                transform.position + transform.right * (lateralOffSet + distanciaGiro/4), Color.blue);

		
		//Distancia a la pared de la derecha
		if (hit.point != Vector3.zero) {
			distancia[1] = Vector3.Distance(transform.position, hit.point);
			//Si estamos muy cerca, y en movimiento, corregimos direccion
			if (distancia[1] < distanciaGiro/5 && !estaParado)
				direccion += velocidadGiro;
		}
		
		//Reiniciamos el hit
		hit.point = Vector3.zero;
		
		//Lanzamos un rycast a iz para saber la distancia a los objetos de la izquierda
		Physics.Raycast (transform.position - transform.right * lateralOffSet, -transform.right, out hit, distanciaGiro/4);
		//Mostramos este ray en el debug en amarillo
		if (debug)		
			Debug.DrawLine (transform.position - transform.right * lateralOffSet, 
		                transform.position - transform.right * (lateralOffSet + distanciaGiro/4), Color.yellow);

		
		//Debug.DrawRay(transform.position, transform.right*(-1),Color.red);
		//Distancia a la pared de la izquierda
		if (hit.point != Vector3.zero) {
			distancia[2] = Vector3.Distance(transform.position,hit.point);
			//Si estamos muy cerca y en movimiento, corregimos direccion
			if (distancia[2] < distanciaGiro/5 && gas > 1)
				direccion -= velocidadGiro;			
		}
	}


	/*************************
	 * ACELERACION Y GIRO
	************************/
	private void AceleracionDireccion () {		
		if (debug) 
			Debug.Log (name + ": Personas alrededor = " + personasAlrededor.Length.ToString()
			           + " / gas = " + gas.ToString() + " / parado = " + estaParado.ToString());
		//Controlamos que el 'gas' este dentro de los limites.
		if (gas > velocidadMaxima)
			gas = velocidadMaxima;
		//Si esta parado o el gas baja de cero, gas = cero
		else if (gas < 0 || estaParado)
			gas = 0;

;

		//Si hay aceleracion aplicamos el giro y el movimiento
		if (gas != 0) {

			//Movimiento relativo a la cantidad de gas acumulado
			float moveDist = gas * Time. fixedDeltaTime;

			//Direccion y velocidad de giro
			float anguloGiro = direccion * velocidadGiro * Time.fixedDeltaTime;// * (-1);
			
			//Aplicamos el giro
			transform.Rotate(new Vector3(0, anguloGiro, 0));
			
			//Aplicamos el movimiento, si no esta parado
			transform.Translate(Vector3.forward * (moveDist * (-1)));
		}
	}


	/**********************
	 *   PUNTO DE DESTINO
	 * ********************/
	private void PuntosDeDestino() {
		//Si llegamos al destino, incrementamos el contador de destino, para ir al siguiente punto.
		if (Vector3.Distance (transform.position, destino.position) < rango && !estaParado){
			destinoActual ++;
			try {
				destino = GameObject.Find("Destino" + destinoActual.ToString()).transform;			
				//	Debug.Log(name +" Cambia a: Destino" + destinoActual.ToString());
				cambiandoDestino = true;
			}
			//Si llegamos al ultimo destino, nos paramos
			catch {
				//	Debug.Log(name +" ha llegado a su destino y se parara." + destinoActual.ToString());
				estaParado = true;			   
			}
		}
		
		//**************************************************
		// Cambiando de direccion en los puntos de destino.
		//**************************************************
		//Desafio.
		if (cambiandoDestino) {
			//Llegar a un destino implica girar
			//Calculamos si el destino esta mas a la izquierda o a la derecha
			float dista3 = Vector3.Distance(destino.position, transform.position + transform.right);
			float dista4 = Vector3.Distance(destino.position, transform.position - transform.right);
			//Decidimos el sentido de giro.
			if (dista3 > dista4)
				direccion +=  velocidadGiro;
			else
				direccion -=  velocidadGiro;
			//Debug.Log (name + " direccion " + direccion);
			cambiandoDestino = false;
		}
	}


	//**************
	//TIEMPO ESPERA
	//**************/
	private void TiempoEspera () {
		if (tiempoEspera > 0) {
			estaParado = true;
			tiempoEspera -= Time.fixedDeltaTime;
		}
		else {
			estaParado = false;
		}
	}

	//Devolvemos el coche al punto inicial
	public void Reset() {
		transform.position = posicionInicial;
	}

}
