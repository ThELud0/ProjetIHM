const int axeX = A0;      // signal de l'axe X sur entrée A0
const int axeY = A1;      // signal de l'axe Y sur entrée A1
const int dashBtPin = 7;  // Bouton-poussoir en broche 7
const int potentiometerPin = A5;
const int jumpBtPin = 2;
const int captForcePin = A3;
const int motorPin = 13;
const int ledWall = 3;
const int ledBreak = 4;
const bool DEBUG = false;


char jumpMessageType = 'J';
char movementMessageXType = 'X';
char movementMessageYType = 'Y';
char changeSpeedMessageType = 'S';
char applyPressionMessageType = 'P';
char dashMessageType = 'D';

bool jumpReleased, dashReleased;
unsigned long justJumped, justUpdatedPrint, justDashed;
const unsigned long jumpPressDelay = 200, printDelay = 100;

int outputValueSpeed = 0;
int podometerSensorValue = 0;
int prevPodometerValue = 0;
int X = 0, Y = 0;

bool activateSendMessages = true;
bool activateReceiveMessages = true;

// Types de message
#define ENTER_WIND_ZONE 'E'
#define EXIT_WIND_ZONE 'X'
#define NEAR_CLIMBABLE_WALL 'C'
#define FAR_FROM_CLIMBABLE_WALL 'F'
#define BREAKING_PLATFORM 'B'
#define BROKE_PLATFORM 'Z'

void setup() {
  pinMode(axeX, INPUT);              // définition de A0 comme une entrée
  pinMode(axeY, INPUT);              // définition de A1 comme une entrée
  pinMode(dashBtPin, INPUT);         // définition de 7 comme une entrée
  digitalWrite(dashBtPin, HIGH);     // Activation de la résistance de Pull-Up interne de la carte Uno
  pinMode(jumpBtPin, INPUT_PULLUP);  // définition de 2 comme une entrée
  digitalWrite(jumpBtPin, HIGH);
  pinMode(potentiometerPin, INPUT);  // définition de A5 comme une entrée
  pinMode(captForcePin, INPUT);      // définition de A3 comme une entrée
  pinMode(motorPin, OUTPUT);         //definition de 13 comme sortie
  pinMode(ledWall, OUTPUT);
  pinMode(ledBreak, OUTPUT);

  Serial.begin(115200);
  justJumped = millis();
  justUpdatedPrint = millis();
  jumpReleased = true;
  dashReleased = true;
  digitalWrite(ledBreak, HIGH);
  digitalWrite(ledWall, HIGH);
}


void loop() {

  int jumpButtonRead;
  int dashButtonRead;

  analogControl();

  if (millis() > justUpdatedPrint + printDelay) {

    justUpdatedPrint = millis();
    if (DEBUG)
      Serial.println("Mouvement mis à jour");
    handleMouvementMessage();
  }



  //gérer le dash avec bouton pressoir
  dashButtonRead = digitalRead(dashBtPin);
  if (dashButtonRead == 1)
    dashReleased = true;
  if ((dashButtonRead != 1) && (millis() > justDashed + jumpPressDelay) && dashReleased) {
    justDashed = millis();
    dashReleased = false;
    if (DEBUG)
      Serial.println("dash pressed");
    sendMessage(dashMessageType, 0, nullptr);
    delay(20);
  }


  jumpButtonRead = digitalRead(jumpBtPin);
  if (jumpButtonRead == 1)
    jumpReleased = true;
  if ((jumpButtonRead == 0) && (millis() > justJumped + jumpPressDelay) && jumpReleased) {
    justJumped = millis();
    jumpReleased = false;
    if (DEBUG)
      Serial.println("jump pressed");
    sendMessage(jumpMessageType, 0, nullptr);
    delay(20);
  }



  int pression = analogRead(captForcePin);
  //Serial.print("valeur recue - ");
  //Serial.println(pression);
  uint8_t pressionMapped = map(pression, 0, 1023, 0, 255);
  sendMessage(applyPressionMessageType, 1, &pressionMapped);
  delay(20);

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

void handleMouvementMessage() {
  X = analogRead(axeX);
  Y = analogRead(axeY);
  if (DEBUG) {
    Serial.println(X);
    Serial.println(Y);
  }

  uint8_t payloadX = map(X, 0, 1023, 0, 255);
  uint8_t payloadY = map(Y, 0, 1023, 0, 255);
  sendMessage(movementMessageXType, 1, &payloadX);
  delay(50);
  sendMessage(movementMessageYType, 1, &payloadY);
  delay(50);
}

void analogControl() {
  // read the analog in value:
  podometerSensorValue = analogRead(potentiometerPin);

  // map it to the range of the analog out:
  outputValueSpeed = map(podometerSensorValue, 0, 1023, 0, 255);

  uint8_t payload = outputValueSpeed;  // Payload is a single byte
  if ((podometerSensorValue >= prevPodometerValue + 3) || (podometerSensorValue <= prevPodometerValue - 3)) {
    if (DEBUG)
      Serial.println(podometerSensorValue);
    sendMessage(changeSpeedMessageType, 1, &payload);  // Type = 'S', Length = 1
    delay(20);
  }
  prevPodometerValue = podometerSensorValue;
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

      case NEAR_CLIMBABLE_WALL:
        digitalWrite(ledWall, LOW);
        break;

      case FAR_FROM_CLIMBABLE_WALL:
        digitalWrite(ledWall, HIGH);
        break;

      case BREAKING_PLATFORM:
        digitalWrite(ledBreak, LOW);
        break;

      case BROKE_PLATFORM:
        digitalWrite(ledBreak, HIGH);
        break;

      default:
        break;
    }
  }
}
