# ProjetIHM

Enoncé du sujet :

"Le but du projet est de programmer le contrôle d’une entité et de la caméra dans un jeu puis de le rendre « intéressant » à contrôler. 
L’objectif principal du projet est de concevoir et développer le contrôleur des actions de l’acteur principal et de la caméra d’un jeu à choisir parmi les propositions de types suivantes : 
    1. Jeu de plateforme en vue de côté (contrôle des mouvements) 
    2. Jeu de course en vue de dessus (contrôle des déplacements) 
    3. Jeu de pêche en vue de dessus ou de côté (contrôle de la mécanique de pêche) L’interface visée principalement sera la manette mais le projet devra être jouable au clavier. " @ https://web4.ensiie.fr/~guillaume.bouyer/IHM/IHM%20Projet%20Manette%202024.pdf

Ici, nous choisirons de faire un jeu de plateforme en vue de côté.

## Spécifications et design :

### Caméra : 

La caméra sera fixe et montrera l'ensemble du niveau.

### Mouvements du personnage :

#### Saut :

- paramètres:
    1. vitesse (= hauteur indirectement)
    2. nombre de sauts
- commande mannette:
    1. Touche A (bas)
- commande clavier:
    1. Touche Espace

#### Déplacement : 

- paramètres:
    1. vitesse
- commande mannette:
    1. Joystick / ControlPad
- commande clavier:
    1. Z/Q/S/D et/ou flèches directionnelles

#### S'accrocher :

- paramètres:
    1. /
- commande mannette:
    1. RB
- commande clavier:
    1. CTRL

####  Grimper :
- paramètres:
    1. vitesse
- commande mannette:
    1. RB + Joystick / ControlPad
- commande clavier:
    1. CTRL + Z/S

#### Dash :
- paramètres:
    1. vitesse
    2. temps 
    3. direction
- commande mannette:
    1. Touche B (droite)
- commande clavier:
    1. Shift

#### Sprint :
- paramètres:
    1. coefficient de vitesse
- commande mannette:
    1. LB
- commande clavier:
    1. Double tap Q/D

### Environnement :
    
- Plateformes mobiles
- Platformes cassable
- Platforme Glace
- Lianes
- Pièges
- Projectiles ennemis
- Checkpoints

TODO:
|Tache|Etat|Commentaire|
|----|-----|------------|
|Jump| Fait| |
|Wall jump| Fait/A revoir | Voir le nombre de wall jump possible|
|Sprint|Fait|  |
|Déplacement | Fait |  |
|S'accrocher | Fait |  |
|Grimper| Fait |  |
|Dash | Fait | Voir Bug log |
|Checkpoint | A rajouter dans les scènes ||
|Plateformes mobiles| A rajouter dans les scènes ||
|Platformes cassable| Fait ||
|Platforme Glace| Fait ||
| Lianes| Fait ||
| Eau | A faire ||
| Vent | Fait ||
| Vibration platforme | Fait||
| Menu Départ | A faire L | |
| Feedback saut | Fait ||
| Feedback grimper | A faire L ||
| Menu Settings | A faire L | GUI pour activer/desactiver feedback |
| Trigger movement caméra | A faire N | Faire une zone ou deux surpoposé qui quand triggered, vient déplacer la caméra vers une autre partie du niveau |
| Feedback Dash | A Faire N ||






Bug found:

|Problème | Reproduction|
|---------|------------|
|Problème de collision| Se mettre sur le test de wall jump et sauter/Dasher vers le haut plusieurs fois jusqu'à que le perso traverse le sol|
|Dash se désactive par moment| Je ne sais pas, peut être un pb de DashCounter ?|
