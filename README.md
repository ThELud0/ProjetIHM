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

La caméra sera centrée sur le personnage qui restera donc fixe au milieu de l'écran.

### Mouvements du personnage :

#### Saut :

- paramètres:
    1. hauteur
    2. vitesse
- commande mannette:
    1. Touche A (bas)
- commande clavier:
    1. Touche Espace

#### Déplacement : 

- paramètres:
    1. vitesse
    2. direction
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
    2. direction
- commande mannette:
    1. Touche B (droite)
- commande clavier:
    1. Shift

### Environnement :
    
- Plateformes mobiles
- Lianes
- Pièges
- Projectiles ennemis

