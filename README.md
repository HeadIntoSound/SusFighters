# English
## Introduction
This document describes the design and architecture of the game created as a solution to Eggscape’s technical test.
  
Made using Unity as the engine and Fish-Net as the networking solution, the game is a simple side-scrolling fighter where two players have to throw off the stage the other and see who scores more points. In order to do so, each player can move and jump around and throw punches forward. Taking inspiration from other fighting games, the characters face each other automatically at all times, giving a more frenetic experience to the players.

The game design process revolved around the specific requirements handed by the company; some key aspects were the focus on minimal latency and the maximum number of players being two. The door to new features is open, but any unnecessary ones were discarded to achieve the main goals.

In addition, most choices were taken accounting for Fish-Net creators, First Gear Games, recommendations, and best practices.

Another important package used during development and testing was ParrelSync, a Unity extension that allows the user to open multiple Editors while keeping them synchronized.

## Architecture

### Networking Components
The architecture of the game was designed to provide a seamless and responsive experience for the players. To achieve that, a client-server model with a Client-Side Prediction (CSP) system was implemented.

Following the requirements, security wasn’t a priority, so the clients have a high degree of authority while following Fish-Net’s server authoritative nature.

The communication layer is established using Fish-Net’s default transport, Tugboat. 

Usually, Unity's built-in physics engine is hard to use for multiplayer games. Luckily, Fish-Net offers a solution to use it and keep clients in sync with minimal effort. This tool is the basis of the game, making the idea of a physics-based fighting game possible.

Following the CSP model, each client calculates their movement and then is reconciled with the server’s calculation at regular intervals of time. This gives a very responsive feeling of movement. As stated above, the clients have a certain level of authority, which is especially noticeable when detecting hits. The client calculates locally the collision between players and then tells the server to update the state on both connected clients. This gives an immediate feeling of response as the player attacks the enemy.
In order to optimize the connections, only certain objects are shared through the network. What can be processed locally in each client will be processed locally.

To handle important data, a Server Controller was implemented. It consists of a singleton running only on the server that has a dictionary containing the list of the players connected and initializes their values on connection. Clients, in turn, have to request information from this instance on the server if needed.

### Non-Networking Controllers and Managers
Inside the game world, each player controls a character made with Unity’s built-in 3D shapes, using a collider and a rigidbody, allowing the usage of Unity’s physics engine. A simple attack animation and a damage taken animation were made using Unity’s animator.

Each behavior is handled by a Controller related to an object in the game. The main case is the Player Controller, responsible for each player’s interactions but which in itself is subdivided into other Controllers to keep the code clean.

In addition, a custom Event Manager was created to handle some communications between objects. This manager is a singleton that declares a series of Unity Events that any controller can subscribe to or invoke at any time.

Below is a list of the Controllers and an explanation of what they do.

* __Player Controller:__ Controls the player and their interactions. Inside this behaviour resides the implementation of most of the systems of the game. These are: CSP system, moving and jumping, attacking, receiving damage and keeping track of the points. This controller has a Skin Controller and a Hitbox Controller.
* __Hitbox Controller:__ Controls the hitbox used to detect attacks. The hitbox is a Box Collider placed in front of the character that turns on during some frames when the attack is taking place.
* __Skin Controller:__  Controls the color of each player. Player 1 is blue and Player 2 is red. Does so by changing the player’s material color. Also, makes the player a little brighter when hit by the enemy.
* __Camera Controller:__ Controls the camera movement. The camera moves only locally and follows the client’s character on screen.
* __Blast Zone Controller:__ Controls the interaction between the blast zone and the player. The blast zone is the area that decides when a player loses a life and has to respawn. It invokes an event and notifies the Respawn Controller.
* __Respawn Controller:__ Controls the respawn when a player hits the blast zone. The player reappears at the top of the stage, above the static platforms, and does so by placing a temporal platform that holds the player for a couple of seconds. If  the temporal platform is occupied the respawning player is placed on the stage directly.
* __Respawn Platform Controller:__ Turns off the temporal respawning platform after some seconds.
* __UI Controller:__ Controls the displayed elements on the UI. Updates the players health and points on the visual interface.

Finally, some utility classes were created. These contain constants of hard-typed values, like Unity’s Tags, or variables that are used to balance the gameplay, and it's useful to have them together in a script.

### Conclusion
Taking on this task was great to keep practicing my game development skills. However, this didn’t come without its challenges, but that is what makes us learn.

At first, having to use a networking solution that I wasn’t familiar with proved to be a little more inconvenient than I thought it would be. That was because of the differences with Mirror, the tool I’m familiar with, which fell into the ‘uncanny valley’ territory. Meaning the subtle changes left me confused, like, for example, wanting to use ClientRpc (Mirror) instead of ObserversRpc (Fish-Net). After a couple of hours, I started to get the hang of Fish-Net. After a couple of days, I realized that Fish-Net really builds upon Mirror to make it better and offers a great upgrade. Now I don’t think I would go back to using Mirror.

In terms of game design, as soon as I read that I had to make some sort of fighting game, I defaulted to my favorite game of the genre, Super Smash Bros. Making the game side-scrolling gave me some advantages and simplified possible weaknesses. At this stage of the development, I wanted to focus as much as possible on making the project somewhat fun but keeping the priority on the requested points of the task.

After defining the game design, I made some drafts of the architecture I would be following. During my time at Buldogo Games, I learned some tricks about networking that I used later on for my personal projects and for this case also. I based most of the architecture on that knowledge.

When implementing the systems I designed, I tried to save as much time as possible by resorting to some of my old developments. To keep me moving, the first couple of days I focused more on having working code, disregarding efficiency and optimization.

In the last couple of days, I dedicated all my efforts to refactoring the code, making it look and work better, optimizing as much as possible. In the end, I had time to make the whole game look prettier, using some free assets from Unity’s Asset Store. And I added some functionalities I placed on hold before in case I couldn’t reach the deadline.

In conclusion, I’m satisfied with the final state of the project for the scope that was set. While it can be improved, I think it’s pretty solid and can give players some fun.

# Español
## Introduction
Este documento describe el diseño y la arquitectura del juego creado como solución a la prueba técnica de Eggscape.

Se utilizó Unity como motor gráfico y Fish-Net como solución para el networking. El proyecto es un juego de pelea en scroll lateral en el que los jugadores deben lanzar del escenario a su rival y ver quién consigue más puntos. Para lograr esto, cada jugador puede moverse, saltar y lanzar golpes. Tomando inspiración en otros juegos de pelea, los personajes se dan la cara en todo momento de manera automática, permitiendo una experiencia de juego más frenética.

El diseño del juego se centró en los requisitos que presentó la empresa, algunos de los aspectos principales fueron el foco en la mínima latencia posible y que la cantidad máxima de jugadores sea dos. La posibilidad de expandir el proyecto está, pero todas las ideas innecesarias fueron descartadas para cumplir con los objetivos principales.

Adicionalmente, la mayor parte de las decisiones se tomaron teniendo en cuenta las buenas prácticas recomendadas por First Gear Games, la compañía que desarrolla Fish-Net.

Otro paquete importante utilizado para el desarrollo y testeo fue ParrelSync, una extensión del Unity que le permite al usuario abrir múltiples instancias del Editor y mantenerlas sincronizadas.

## Arquitectura
### Componentes de red
La arquitectura del juego fue diseñada para brindar al jugador una experiencia sin cortes y responsiva. Para alcanzar este objetivo se utilizó un modelo cliente-servidor y un sistema de predicción del lado del cliente (CSP, por sus siglas en inglés).

Siguiendo los requisitos, la seguridad no era una de las prioridades, por lo tanto los clientes tienen un grado considerablemente alto de autoridad.

La capa de comunicación se establece utilizando el componente de Fish-Net llamado Tugboat.

Normalmente, utilizar el motor de físicas de Unity suele ser contraproducente para el desarrollo de videojuegos multijugador. Afortunadamente, Fish-Net ofrece una solución para utilizarlo y mantener la sincronización entre clientes y servidor sin problema. Esto hizo posible la idea de hacer un juego basado en físicas sin muchos problemas.

Siguiendo el modelo CSP, cada cliente calcula su movimiento y después reconcilia los resultados con los cálculos realizados por el servidor. Esto brinda un movimiento responsivo durante el juego. Como se mencionó anteriormente, los clientes tienen un alto nivel de autoridad, esto se evidencia en el sistema de ataque. El cliente calcula de manera local la colisión entre jugadores y notifica al servidor para que actualice el estado del juego a los clientes conectados. Esto da una sensación de inmediatez al atacar y dar un golpe al enemigo.

Para optimizar la conexión, solo los objetos estrictamente necesarios se comunican a través de la red. Todo lo que pueda ser procesado localmente, será procesado localmente.

Para manejar información importante, se implementó un Controlador de Servidor (ServerController). Este consiste en un singleton que corre únicamente en el servidor y contiene un diccionario con la lista de jugadores conectados, además de inicializar los valores necesarios para cada jugador al momento de la conexión. Los clientes deben pedir información a este singleton de ser necesario.

### Controladores y Managers
Dentro del juego, cada jugador controla un personaje hecho con los modelos 3d que vienen con Unity, un collider y un rigidbody, que permiten utilizar el motor de físicas de Unity. Una animación simple de ataque y una de recibir daño fueron creadas utilizando el animador de Unity

Cada comportamiento está manejado desde un Controlador, relacionado a un objeto dentro del juego. El caso principal es el del Controlador del Jugador (PlayerController), responsable de manejar las interacciones del jugador, este controlador está subdividido en otros controladores para mantener el código fácil de leer.

Adicionalmente, se utilizó un Manager de Eventos (EventManager) para manejar la comunicación entre objetos dentro del juego. Este Manager es un singleton que tiene una serie de UnityEvents, los cuales cualquier controlador puede invocar o suscribirse a.

La lista de controladores es la siguiente:
* __PlayerController:__ Controla el personaje del jugador y sus interacciones. Aquí se encuentra la mayor parte de soluciones creadas para el juego. Estas son: sistema CSP, movimiento y salto, ataque, recibir daño ,y llevar la cuenta de los puntos. Este controlador tiene un SkinController y un HitboxController.
* __HitboxController:__ Controla la hitbox utilizada para detectar ataques. La hitbox es un collider que se encuentra en el frente del personaje y se activa durante los instantes que el ataque está siendo realizado.
* __SkinController:__  Controla el color de cada jugador. El Jugador 1 es color azul, el Jugador 2 es color rojo. Además, hace que el jugador que ha sido golpeado cambie su color un poco.
* __CameraController:__ Controla el comportamiento de la cámara. La cámara se mueve ligeramente cuando el jugador controlado localmente se acerca a los bordes del escenario.
* __BlastZoneController:__ Controla la interacción entre la “Blast Zone” y los jugadores. La “Blast Zone” es el área del mapa que detecta si un jugador perdió una vida y debe reaparecer.
* __RespawnController:__ Controla la reaparición en el escenario del jugador que colisionó con la Blast Zone. El jugador reaparece en una plataforma temporal arriba de las plataformas permanentes del escenario. En caso de que el otro jugador la esté utilizando, reaparece sobre el escenario directamente.
* __RespawnPlatformController:__ Desactiva la plataforma temporal de reaparición después de unos segundos.
* __UIController:__ Controla los elementos que se muestran en la interfaz visual. Estos elementos son la vida y los puntos de los jugadores.

También se crearon algunas clases que brindan ciertas utilidades. Estas contienen constantes tipeadas previamente, como los Tags de Unity, o variables utilizadas para el balance del juego.

## Conclusión
Realizar la prueba fue muy fructífero para practicar mis habilidades de desarrollo. Esto no vino sin sus desafíos, pero eso es lo que nos hace crecer.

En un principio, utilizar una solución de networking con la que no estaba familiarizado fue más difícil de lo que creía inicialmente. Esto fue porque las diferencias sutiles con Mirror, la solución que venía utilizando anteriormente. Esas pequeñas diferencias caen en el territorio de lo que en inglés se conoce como ‘uncanny valley’. Un ejemplo de esto era pensar en términos de ClientRpc y ServerRpc, que en Fish-Net se interpretan como ObserverRpc y ServerRpc. Después de algunas horas comencé a tomar más dominio de Fish-Net. Después de los primeros días, me di cuenta de que Fish-Net es una mejora significativa a Mirror y ofrece soluciones muy útiles. De ahora en más no creo que vuelva a recurrir a Mirror.

En cuanto al diseño del juego, en cuanto vi que tenía que hacer algún tipo de videojuego de peleas, mi mente se dirigió directamente a Super Smash Bros. mi juego favorito del género. Hacer un juego de scroll lateral me dio algunas ventajas y simplificó puntos en los cuales podría haber fallas. En esta etapa del desarrollo, me centré lo más posible en hacer un proyecto medianamente divertido pero manteniendo la prioridad en cumplir con los requisitos de la prueba.

Luego del proceso de diseño, hice algunos borradores de cómo debía ser la arquitectura. Durante mi paso por Buldogo Games aprendí algunas buenas prácticas de networking, las cuales utilicé posteriormente para desarrollos propios. En eso se basa la mayor parte de la arquitectura del juego.

Al implementar los sistemas diseñados, traté de ahorrar la mayor cantidad de tiempo posible recurriendo a proyectos míos anteriores. Para no frenar mucho en cosas pequeñas, los primeros días me centré en tener código que funcionara, no necesariamente en hacerlo lo más eficiente posible.

Durante los últimos días, dediqué mis esfuerzos a limpiar y optimizar el código lo más posible. Al final también tuve tiempo de hacer que el juego en general se vea mejor, utilizando algunos assets de la tienda de Unity. También agregué algunas funcionalidades que inicialmente dejé en segundo plano por si no daban los tiempos.

En conclusión, estoy satisfecho con el estado final del proyecto en relación a los objetivos planteados inicialmente. Siempre se puede mejorar, pero creo que es un prototipo sólido que puede brindar algo de diversión.
