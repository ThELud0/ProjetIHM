const int axeX = A0;      // signal de l'axe X sur entrée A0
const int axeY = A1;      // signal de l'axe Y sur entrée A1
const int dashBtPin = 7;  // Bouton-poussoir en broche 7
const int potentiometerPin = A5;
const int jumpBtPin = 2;
const int captForcePin = A3;
const int motorPin = 13;


char jumpMessageType = 'J';
char movementMessageXType = 'X';
char movementMessageYType = 'Y';
char changeSpeedMessageType = 'S';
char applyPressionMessageType = 'P';

bool jumpReleased;
unsigned long justJumped, justUpdatedPrint;
const unsigned long jumpPressDelay = 200, printDelay = 500;
int outputValueX = 0;
int outputValueY = 0;

int outputValueSpeed = 0;
int podometerSensorValue = 0; 

bool activateSendMessages = true;
bool activateReceiveMessages = true;

// Types de message
#define ENTER_WIND_ZONE 'E'
#define EXIT_WIND_ZONE 'X'

void setup() {
  pinMode(axeX, INPUT);           // définition de A0 comme une entrée
  pinMode(axeY, INPUT);           // définition de A1 comme une entrée
  pinMode(dashBtPin, INPUT);      // définition de 7 comme une entrée
  digitalWrite(dashBtPin, HIGH);  // Activation de la résistance de Pull-Up interne de la carte Uno
  pinMode(jumpBtPin, INPUT_PULLUP);// définition de 2 comme une entrée
  digitalWrite(jumpBtPin, HIGH);
  pinMode(potentiometerPin, INPUT); // définition de A5 comme une entrée
  pinMode(captForcePin, INPUT); // définition de A3 comme une entrée
  pinMode (motorPin,OUTPUT); //definition de 13 comme sortie

  Serial.begin(115200);
  justJumped = millis();
  justUpdatedPrint = millis();
  jumpReleased = true;
}


void loop() {

  int jumpButtonRead;
  analogControl();
  if (millis() > justUpdatedPrint + printDelay) {

    justUpdatedPrint = millis();
    //Serial.println("Mouvement mis à jour");
    //sendMovementMessage(); -> to sed movement but makes the game lag as fuck
  }
  //gérer le dash avec bouton pressoir: je laisse l'ancien code du jump avec bouton poussoir 
  /*
  buttonRead = digitalRead(jumpBtPin);
  if (buttonRead == 1)
    jumpReleased = true;
  if ((buttonRead != 1) && (millis() > justJumped + jumpPressDelay) && jumpReleased) {
    justJumped = millis();
    jumpReleased = false;
    Serial.println("Bouton-poussoir actif");
    sendJumpMessage();
  }
  */

  jumpButtonRead = digitalRead(jumpBtPin);
  if(jumpButtonRead == 0)
    sendMessage(jumpMessageType, 0, nullptr);

  int pression = analogRead(captForcePin);
  //Serial.print("valeur recue - ");
  //Serial.println(pression);

  uint8_t pressionMapped = map(pression, 0, 1023, 0, 255);
  sendMessage(applyPressionMessageType, 1, &pressionMapped);

  if (Serial.available() > 1) {
    char messageType = Serial.read();
    uint8_t payloadLength = Serial.read();
    uint8_t payload[payloadLength];
    if (payloadLength > 0)
      Serial.readBytes(payload, payloadLength);
    
    processMessage(messageType, payload);
  }
  delay(50);
}

void sendMovementMessage() {
  int X, Y;
  uint8_t payloadX, payloadY;
  X = analogRead(axeX);
  Y = analogRead(axeY);

/*
  Serial.print("Axe X:");
  Serial.print(X);
  Serial.print(", ");
  Serial.print("Axe Y:");
  Serial.print(Y);
  Serial.println("");
  */


  outputValueX = map(X, 0, 1023, 0, 255);
  outputValueY = map(Y, 0, 1023, 0, 255);
  payloadX = outputValueX;
  payloadY = outputValueY;
  
  /*
  outputValueX = map(X, 0, 1023, 0, 255) - 126;
  outputValueY = map(Y, 0, 1023, 0, 255) - 124;

  payloadX = outputValueX;
  if (outputValueX > 120)
    payloadX = 130;
  else if (outputValueX < -120)
    payloadX = -130;

  payloadY = -outputValueY;
  if (outputValueY > 120)
    payloadY = 130;
  else if (outputValueY < -120)
    payloadY = -130;

  Serial.print("Payload X:");
  Serial.print(payloadX);
  Serial.print(", ");
  Serial.print("Payload Y:");
  Serial.print(payloadY);
  Serial.println("");
*/

  sendMessage(movementMessageXType, 1, &payloadX);
  sendMessage(movementMessageYType, 1, &payloadY);
}

void analogControl(){
  // read the analog in value:
  podometerSensorValue = analogRead(potentiometerPin);
  //Serial.println(podometerSensorValue);
  // map it to the range of the analog out:
  outputValueSpeed = map(podometerSensorValue, 0, 1023, 0, 255);

  uint8_t payload = outputValueSpeed;  // Payload is a single byte
  sendMessage(changeSpeedMessageType, 1, &payload);  // Type = 'S', Length = 1
  delay(20);
}

// Function to send messages with a fixed header
void sendMessage(char type, uint8_t length, uint8_t* data) {

  if (activateSendMessages) {
    Serial.write(type);    // Write the message type
    Serial.write(length);  // Write the length of the payload
    if (data != nullptr && length > 0) {
      Serial.write(data, length);  // Write the payload if it exists
    }
  }
}

void processMessage(char messageType, uint8_t payload) {
  if (activateReceiveMessages) {
    switch (messageType) {
          case ENTER_WIND_ZONE:
            digitalWrite(motorPin, HIGH);
            break;

          case EXIT_WIND_ZONE:
            digitalWrite(motorPin, LOW);
            break;

          default:
            break;
     }
  }
}
