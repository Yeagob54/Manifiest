// (cc) 2014 SANTIAGO DOPAZO HILARIO

using UnityEngine;
using System.Collections;

public class comportamientoCoche : MonoBehaviour {

	//Deteccion de objetos alrededor
	private Collider[] objectsInRange;
	public float rango = 20.0f;
	//Direccion y velocidad
	private float direccion = 0f;
	private float gas = 0f;
	public float velocidad = 0.2f;
	public float velocidadGiro = 3f;
	public float velocidadMaxima = 10f;
	public float fuerzaFrenada = 5f;
	public float distanciaGiro = 20f;
	public float distanciaFrenado = 3f;
	public bool estaParado = false;
	//Puntos de destino
	public Transform puntoDestino;
	public bool cambiandoDestino = false;
	public byte destinoActual = 52;
	//Seguimiento de la manifestacion
	public bool delanteMani = false;
	public bool detrasMani = false;

	//Tiempo antes de arrancar
	public float tiempoEspera = 0f;


	/******************
	 *      START
	 * ***************/
	void Start () {
		//Si no se inicializo el destino, lo inicializamos nosotros. 
		if (!puntoDestino) {
			try {
				puntoDestino = GameObject.Find("Destino"+destinoActual.ToString()).transform;
			}
			catch {
				puntoDestino = transform;
			}
		}
	}

	/********************
	 *      UPDATE
	 * ******************/
	void FixedUpdate () {

		//Angulo con respecto al punto de destino
		float angleBetween = 0.0f;
		//Distancia a las paredes laterales, frontales y posibles personas alrededor
		float[] distancia = {0,0,0,0};
		Vector3 personaDir;
		RaycastHit hit = new RaycastHit();
		int capaImpacto;
		hit.point = Vector3.zero;

		//**************
		//TIEMPO ESPERA
		//**************			
		if (tiempoEspera > 0) {
			estaParado = true;
			tiempoEspera -= Time.deltaTime;
		}
		else {
			estaParado = false;
		}

		//*********************************************
		//   DETECCION DE PERSONAS ALREDEDOR (RANGO)
		//*********************************************
		objectsInRange = Physics.OverlapSphere(transform.position, rango, 8);	
		
		//Si hay alguna persona dentro del rango
		if (objectsInRange.Length > 0)	{		    
		
			foreach (Collider persona in objectsInRange)  {

				//Calculamos el vector direccion de la persona detectada, el angulo con respecto a 'forward' y la distancia
				personaDir = persona.gameObject.transform.position - transform.position;
				personaDir.Normalize();
				angleBetween = Vector3.Angle (-transform.forward, personaDir);
				if (distancia[3] < Vector3.Distance(transform.position,persona.gameObject.transform.position))
					distancia[3] = Vector3.Distance(transform.position,persona.gameObject.transform.position);

				//Si estamos detras de la mani, tenemos que esperar a que los manifestantes avencen
				if (persona.tag == "Manifestantes" && detrasMani) {
					gas = 0;
					estaParado = true;
				}

				//Persona delante, cerca del coche, frenamos
				if (angleBetween < 15) {
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
				gas += velocidad;			
				estaParado = false;
			}
		}
	
		// ***************
		// RAYCAST FRONTAL
		// ***************
		///Desde la posicion del coche, hacia adelante.
		Physics.Raycast (transform.position, -transform.forward, out hit, distanciaGiro);

		//Si el Raycast ha impactado con algo a la distacia de giro
		if (hit.point != Vector3.zero || cambiandoDestino) {
			
			//Calculamos la distancia al punto de impacto
			distancia[0] = Vector3.Distance(transform.position, hit.point );
			capaImpacto = hit.collider.gameObject.layer;
			
			//Si hay un edificio en la linea del ray, cerca, que no sea un coche, miramos hacia que lado girar
			if (capaImpacto == 10 || cambiandoDestino) {

				//Calculamos si el destino esta a la izquierda o a la derecha
				float dista3 = Vector3.Distance(puntoDestino.position, transform.position + transform.right);
				float dista4 = Vector3.Distance(puntoDestino.position, transform.position - transform.right);
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
			if (capaImpacto == 9 || capaImpacto == 8) {
				gas -= velocidad*(1/fuerzaFrenada);
				//Distasncia al vehiculo de delante
				if (distancia[0] < distanciaFrenado) 
					estaParado = true;
			}

		//	Debug.Log("RAycast>> name:" + hit.collider.gameObject.name + "Distancia: " + distancia[0].ToString());
		}
		//Si no tenemos ningun obstaculo delante, seguimos recto.
		else
			direccion = 0;

		// *****************
		// RAYCAST LATERALES (pendiente de lanzarlos contra la acera, no los edificios. 
		// *****************

		//Reiniciamos el hit
		hit.point = Vector3.zero;
		
		//Lanzamos un rycast a der para saber la distancia a los objetos de la derecha
		Physics.Raycast (transform.position, transform.right, out hit, distanciaGiro/4);
		
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
		Physics.Raycast (transform.position, -transform.right, out hit, distanciaGiro/4);
		//Debug.DrawRay(transform.position, transform.right*(-1),Color.red);
		//Distancia a la pared de la izquierda
		if (hit.point != Vector3.zero) {
			distancia[2] = Vector3.Distance(transform.position,hit.point);
			//Si estamos muy cerca y en movimiento, corregimos direccion
			if (distancia[2] < distanciaGiro/5 && gas > 1)
				direccion -= velocidadGiro;			
		}
		
		/*	Debug.Log("Distancia 0: " + distancia[0].ToString());
		Debug.Log("Distancia 1: " + distancia[1].ToString());
		Debug.Log("Distancia 2: " + distancia[2].ToString());*/

		/*************************
		 * ACELERACION Y GIRO
		 ************************/

		//Controlamos que el 'gas' este dentro de los limites.
		if (gas > velocidadMaxima)
			gas = velocidadMaxima;
		//Si esta parado o el gas baja de cero, gas = cero
		else if (gas < 0 || estaParado)
			gas = 0;

		//Si hay aceleracion aplicamos el giro y el movimiento
		if (gas != 0) {
			
			//Movimiento relativo a la cantidad de gas acumulado
			float moveDist=gas*Time.deltaTime;
			
			//Direccion y velocidad de giro
			float turnAngle= direccion * velocidadGiro * Time.deltaTime;// * (-1);
			
			//Aplicamos el giro
			transform.Rotate(new Vector3(0,turnAngle,0));
			
			//Aplicamos el movimiento, si no esta parado
			transform.Translate(Vector3.forward*(moveDist* (-1)));
		}

		/**********************
		 *   PUNTO DE DESTINO
		 * ********************/
		//Si llegamos al destino, incrementamos el contador de destino, para ir al siguiente punto.
		if (Vector3.Distance (transform.position, puntoDestino.position) < rango && !estaParado){
			destinoActual ++;
			try {
				puntoDestino = GameObject.Find("Destino" + destinoActual.ToString()).transform;			
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
		//Revisar.
		if (cambiandoDestino) {
			//Llegar a un destino implica girar
			//Calculamos si el destino esta mas a la izquierda o a la derecha
			float dista3 = Vector3.Distance(puntoDestino.position, transform.position + transform.right);
			float dista4 = Vector3.Distance(puntoDestino.position, transform.position - transform.right);
			//Decidimos el sentido de giro.
			if (dista3 > dista4)
				direccion +=  velocidadGiro;
			else
				direccion -=  velocidadGiro;
				//Debug.Log (name + " direccion " + direccion);
			cambiandoDestino = false;
		}
	}
}
